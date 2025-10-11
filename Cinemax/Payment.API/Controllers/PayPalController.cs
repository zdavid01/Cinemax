using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Payment.Infrastructure.PayPal;
using RestSharp;
using Payment.Application.Contracts.Infrastructure;
using Payment.Application.Models;
using MediatR;
using Payment.Application.Features.Payments.Commands.CreatePayment;
using Payment.Application.Features.Payments.Queries.ViewModels;
using Payment.Application.Features.Payments.Commands.DTOs;
using StackExchange.Redis;

namespace Payment.API.Controllers;

[Route("api/paypal")]
[ApiController]
public class PayPalController : ControllerBase
{
    private readonly PayPalService? _payPalService;
    private readonly ILogger<PayPalController> _logger;
    private readonly IEmailService _emailService;
    private readonly IMediator _mediator;
    private readonly IConnectionMultiplexer _redis;
    private readonly IConfiguration _configuration;

    public PayPalController(ILogger<PayPalController> logger, IEmailService emailService, PayPalService payPalService, IMediator mediator, IConnectionMultiplexer redis, IConfiguration configuration)
    {
        _payPalService = payPalService ?? throw new ArgumentNullException(nameof(payPalService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    
        // Endpoint to initiate the payment process
        [HttpPost("create-payment")]
        [Authorize]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest paymentRequest)
        {
            try
            {
                // Get the actual username and email from the JWT token claims
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest(new { error = "Username not found in token" });
                }
                
                _logger.LogInformation("Creating PayPal payment for user: {Username} ({Email}), Amount: {Amount}", username, userEmail, paymentRequest.Amount);
                
                // Create return URL that redirects back to Angular app with username
                var returnUrl = $"http://localhost:8004/api/paypal/return?username={username}";
                var cancelUrl = $"http://localhost:4200/basket?payment=cancelled";
                
                var (paymentId, approvalUrl) = await _payPalService.CreatePayment(paymentRequest.Amount, paymentRequest.Currency, returnUrl, cancelUrl);
                
                // Store user email in Redis temporarily (expires in 1 hour) for later retrieval
                if (!string.IsNullOrEmpty(userEmail) && !string.IsNullOrEmpty(paymentId))
                {
                    try
                    {
                        var db = _redis.GetDatabase();
                        await db.StringSetAsync($"payment:email:{paymentId}", userEmail, TimeSpan.FromHours(1));
                        _logger.LogInformation("Stored email {Email} for payment {PaymentId}", userEmail, paymentId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to store email in Redis for payment {PaymentId}", paymentId);
                    }
                }
                
                // Return the expected JSON structure
                var response = new
                {
                    id = paymentId,
                    state = "created",
                    links = new[]
                    {
                        new
                        {
                            href = approvalUrl,
                            rel = "approval_url",
                            method = "REDIRECT"
                        }
                    }
                };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    // Endpoint to execute the payment after approval
    [HttpPost("execute-payment")]
    [Authorize]
    public async Task<IActionResult> ExecutePayment([FromBody] ExecutePaymentRequest executeRequest)
    {
        try
        {
            var state = await _payPalService.ExecutePayment(executeRequest.PaymentId, executeRequest.PayerId);
            if (state == "approved")
            {
                await ProcessSuccessfulPayment(executeRequest.PaymentId, executeRequest.PayerId);
                return Ok(new { state, message = "Payment successfully completed, saved to database, and emails sent." });
            }
            return Ok(new { state });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // Endpoint to create premium subscription payment
    [HttpPost("create-premium-payment")]
    [Authorize]
    public async Task<IActionResult> CreatePremiumPayment([FromBody] PaymentRequest paymentRequest)
    {
        try
        {
            // Get the actual username and email from the JWT token claims
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { error = "Username not found in token" });
            }
            
            _logger.LogInformation("Creating premium subscription PayPal payment for user: {Username} ({Email}), Amount: {Amount}", username, userEmail, paymentRequest.Amount);
            
            // Create return URL for premium subscription (different from regular checkout)
            var returnUrl = $"http://localhost:8004/api/paypal/premium-return?username={username}";
            var cancelUrl = $"http://localhost:4200/premium?payment=cancelled";
            
            var (paymentId, approvalUrl) = await _payPalService.CreatePayment(paymentRequest.Amount, paymentRequest.Currency, returnUrl, cancelUrl);
            
            // Store user email in Redis temporarily (expires in 1 hour) for later retrieval
            if (!string.IsNullOrEmpty(userEmail) && !string.IsNullOrEmpty(paymentId))
            {
                try
                {
                    var db = _redis.GetDatabase();
                    await db.StringSetAsync($"payment:email:{paymentId}", userEmail, TimeSpan.FromHours(1));
                    _logger.LogInformation("Stored email {Email} for premium payment {PaymentId}", userEmail, paymentId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to store email in Redis for premium payment {PaymentId}", paymentId);
                }
            }
            
            // Return the expected JSON structure
            var response = new
            {
                id = paymentId,
                state = "created",
                links = new[]
                {
                    new
                    {
                        href = approvalUrl,
                        rel = "approval_url",
                        method = "REDIRECT"
                    }
                }
            };
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // Endpoint to handle PayPal return for premium subscription
    [HttpGet("premium-return")]
    public async Task<IActionResult> PremiumReturnFromPayPal(string paymentId, string payerId, string? username = null)
    {
        try
        {
            _logger.LogInformation("Premium subscription PayPal return callback: PaymentId={PaymentId}, PayerId={PayerId}, Username={Username}", 
                paymentId, payerId, username);

            // Execute the payment after the user approves it
            var paymentState = await _payPalService.ExecutePayment(paymentId, payerId);
            _logger.LogInformation("Premium subscription PayPal payment executed with state: {State}", paymentState);

            // If the payment was successful, redirect to Angular with success
            if (paymentState == "approved")
            {
                _logger.LogInformation("Premium subscription payment approved for user: {Username}", username);
                
                // Redirect back to Angular app with success status
                var redirectUrl = $"http://localhost:4200/premium?payment=success&username={username}";
                return Redirect(redirectUrl);
            }
            else
            {
                // Redirect back to Angular app with failure status
                var redirectUrl = $"http://localhost:4200/premium?payment=failed";
                return Redirect(redirectUrl);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing premium subscription PayPal return for PaymentId={PaymentId}", paymentId);
            
            // Redirect back to Angular app with error status
            var redirectUrl = $"http://localhost:4200/premium?payment=error&message={Uri.EscapeDataString(ex.Message)}";
            return Redirect(redirectUrl);
        }
    }
    
    [HttpGet("return")]
    public async Task<IActionResult> ReturnFromPayPal(string paymentId, string payerId, string? username = null)
    {
        try
        {
            _logger.LogInformation("PayPal return callback received: PaymentId={PaymentId}, PayerId={PayerId}, Username={Username}", 
                paymentId, payerId, username);

            // Execute the payment after the user approves it
            var paymentState = await _payPalService.ExecutePayment(paymentId, payerId);
            _logger.LogInformation("PayPal payment executed with state: {State}", paymentState);

            // If the payment was successful, save to database and send emails
            if (paymentState == "approved")
            {
                await ProcessSuccessfulPayment(paymentId, payerId, username);
                
                // Redirect back to Angular app with success status
                var redirectUrl = $"http://localhost:4200/payment-success?paymentId={paymentId}&status=success";
                return Redirect(redirectUrl);
            }
            else
            {
                // Redirect back to Angular app with failure status
                var redirectUrl = $"http://localhost:4200/payment-success?paymentId={paymentId}&status=failed";
                return Redirect(redirectUrl);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing PayPal return for PaymentId={PaymentId}", paymentId);
            
            // Redirect back to Angular app with error status
            var redirectUrl = $"http://localhost:4200/payment-success?status=error&message={Uri.EscapeDataString(ex.Message)}";
            return Redirect(redirectUrl);
        }
    }


    private async Task ProcessSuccessfulPayment(string paymentId, string payerId, string? username = null)
    {
        try
        {
            // Save payment to database
            await SavePaymentToDatabase(paymentId, payerId, username);
            
            // Send emails with username
            await SendPaymentEmails(paymentId, payerId, username);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to process successful payment for {PaymentId}", paymentId);
        }
    }

    private async Task SavePaymentToDatabase(string paymentId, string payerId, string? username = null)
    {
        try
        {
            // Use passed username or fallback to extracting from token
            string actualUsername;
            if (!string.IsNullOrEmpty(username))
            {
                actualUsername = username;
            }
            else
            {
                // Fallback: try to get from JWT token
                var userEmail = GetUserEmailFromToken();
                actualUsername = userEmail.Split('@')[0]; // Use email prefix as username
            }
            
            // Get the basket from Redis (stored as hash by IDistributedCache)
            var db = _redis.GetDatabase();
            var basketKey = actualUsername;
            var basketData = await db.HashGetAsync(basketKey, "data");
            
            List<PaymentItemDTO> paymentItems = new List<PaymentItemDTO>();
            decimal totalAmount = 0;
            
            if (!basketData.IsNullOrEmpty)
            {
                try
                {
                    // Parse basket JSON from the hash field
                    var basket = System.Text.Json.JsonSerializer.Deserialize<BasketData>(basketData!);
                    
                    if (basket?.Items != null && basket.Items.Any())
                    {
                        // Convert basket items to payment items
                        foreach (var item in basket.Items)
                        {
                            paymentItems.Add(new PaymentItemDTO
                            {
                                MovieName = item.Title,
                                MovieId = item.MovieId,
                                Price = item.Price,
                                Quantity = 1 // Assuming quantity 1 per item
                            });
                            totalAmount += item.Price;
                        }
                        
                        _logger.LogInformation("Retrieved {ItemCount} items from basket for user {Username}, Total: ${Total}", basket.Items.Count, actualUsername, totalAmount);
                    }
                    else
                    {
                        _logger.LogWarning("Basket found but has no items for user {Username}", actualUsername);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to parse basket data for user {Username}", actualUsername);
                }
            }
            else
            {
                _logger.LogWarning("No basket found in Redis for user {Username}", actualUsername);
            }
            
            // If no basket items found, create a fallback payment item
            if (!paymentItems.Any())
            {
                _logger.LogWarning("Using fallback payment item for user {Username} as no basket was found", actualUsername);
                paymentItems.Add(new PaymentItemDTO
                {
                    MovieName = GetRandomMovieName(),
                    MovieId = $"CINEMAX_TICKET_{paymentId.Substring(0, Math.Min(8, paymentId.Length))}",
                    Price = 0.10m,
                    Quantity = 1
                });
                totalAmount = 0.10m;
            }
            
            // Create a payment entry for the database
            var createPaymentCommand = new CreatePaymentCommand
            {
                BuyerId = payerId, // Using PayPal payer ID as buyer ID
                BuyerUsername = actualUsername,
                Currency = "USD",
                PaymentItems = paymentItems
            };

            var paymentDbId = await _mediator.Send(createPaymentCommand);
            _logger.LogInformation("Payment saved to database with ID: {PaymentDbId} for PayPal payment: {PaymentId} for user: {Username} with {ItemCount} items totaling ${Total}", 
                paymentDbId, paymentId, actualUsername, paymentItems.Count, totalAmount);
            
            // Clear the basket after successful payment
            await db.KeyDeleteAsync(basketKey);
            _logger.LogInformation("Basket cleared for user {Username} after successful payment", actualUsername);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to save payment to database for {PaymentId}", paymentId);
        }
    }

    private async Task SendPaymentEmails(string paymentId, string payerId, string? username = null)
    {
        // First, try to get email from Redis (stored during payment creation)
        string? userEmailAddress = null;
        try
        {
            var db = _redis.GetDatabase();
            var storedEmail = await db.StringGetAsync($"payment:email:{paymentId}");
            if (!storedEmail.IsNullOrEmpty)
            {
                userEmailAddress = storedEmail.ToString();
                _logger.LogInformation("Retrieved email {Email} from Redis for payment {PaymentId}", userEmailAddress, paymentId);
                
                // Clean up the temporary Redis entry
                await db.KeyDeleteAsync($"payment:email:{paymentId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to retrieve email from Redis for payment {PaymentId}", paymentId);
        }
        
        // Fallback: try to get email from username or JWT token
        if (string.IsNullOrEmpty(userEmailAddress))
        {
            if (!string.IsNullOrEmpty(username))
            {
                userEmailAddress = await GetUserEmailByUsername(username);
            }
            else
            {
                userEmailAddress = GetUserEmailFromToken();
            }
        }
        
        // Send detailed payment notification to user
        try
        {
            var userEmail = new Email
            {
                To = userEmailAddress,
                Subject = $"Payment Confirmation - {paymentId}",
                Body = $"Your PayPal payment has been completed successfully.\n\nPayment ID: {paymentId}\nPayer ID: {payerId}\nStatus: Approved\nTimestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n\nThank you for your payment! You can view your payment details in your account."
            };
            
            await _emailService.SendEmail(userEmail);
            _logger.LogInformation("User payment notification sent successfully to {Email} for payment {PaymentId}", userEmailAddress, paymentId);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to send user payment notification to {Email} for payment {PaymentId}. Email service may be down or credentials expired.", userEmailAddress, paymentId);
        }

        // Send admin notification email
        var adminEmailAddress = _configuration["EmailSettings:AdminEmail"] ?? "vida33085@gmail.com";
        try
        {
            var adminEmail = new Email
            {
                To = adminEmailAddress,
                Subject = $"New PayPal Payment Completed - {paymentId}",
                Body = $"A new PayPal payment has been completed successfully.\n\nPayment ID: {paymentId}\nPayer ID: {payerId}\nUser: {username ?? "Unknown"}\nUser Email: {userEmailAddress}\nStatus: Approved\nTimestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC"
            };
            
            await _emailService.SendEmail(adminEmail);
            _logger.LogInformation("Admin email sent successfully for payment {PaymentId}", paymentId);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to send admin email for payment {PaymentId}. Email service may be down or credentials expired.", paymentId);
        }
    }
    
    private async Task<string> GetUserEmailByUsername(string username)
    {
        try
        {
            // Check if username looks like an email
            if (username.Contains("@"))
            {
                return username;
            }
            
            // Try to get from User claim if available (when authenticated)
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
                if (!string.IsNullOrEmpty(emailClaim))
                {
                    _logger.LogInformation("Got email from authenticated user claims: {Email}", emailClaim);
                    return emailClaim;
                }
            }
            
            // Query Identity API to get user details
            try
            {
                var identityApiUrl = _configuration["IdentityApi:BaseUrl"] ?? "http://identity.api:8080";
                using var httpClient = new HttpClient();
                
                // Try to get user details from Identity API
                var response = await httpClient.GetAsync($"{identityApiUrl}/api/v1/User/by-username/{username}");
                
                if (response.IsSuccessStatusCode)
                {
                    var userJson = await response.Content.ReadAsStringAsync();
                    var userDetails = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(userJson);
                    
                    if (userDetails.TryGetProperty("email", out var emailProp))
                    {
                        var email = emailProp.GetString();
                        if (!string.IsNullOrEmpty(email))
                        {
                            _logger.LogInformation("Got email from Identity API for {Username}: {Email}", username, email);
                            return email;
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("Identity API returned {StatusCode} for username {Username}", response.StatusCode, username);
                }
            }
            catch (Exception apiEx)
            {
                _logger.LogWarning(apiEx, "Failed to query Identity API for username {Username}", username);
            }
            
            // Fallback: use a default admin email so at least someone gets notified
            _logger.LogWarning("Could not determine email for username {Username}, using fallback", username);
            var adminEmail = _configuration["EmailSettings:AdminEmail"] ?? "vida33085@gmail.com";
            return adminEmail;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting email for username {Username}", username);
            var adminEmail = _configuration["EmailSettings:AdminEmail"] ?? "vida33085@gmail.com";
            return adminEmail;
        }
    }

        private string GetUserEmailFromToken()
        {
            // Check if user is authenticated
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                _logger.LogWarning("User is not authenticated when trying to get email from token. Using default email for PayPal callback.");
                return "vida33085@gmail.com"; // Default email for PayPal callbacks
            }

            // Get the user's email from the JWT token
            var emailClaim = User.FindFirst(ClaimTypes.Email);
            if (emailClaim != null && !string.IsNullOrEmpty(emailClaim.Value))
            {
                _logger.LogInformation("Successfully extracted email from JWT token: {Email}", emailClaim.Value);
                return emailClaim.Value;
            }

            _logger.LogWarning("Email claim not found in JWT token. Using default email.");
            return "vida33085@gmail.com"; // Fallback email
        }

        private string GetRandomMovieName()
        {
            var movies = new[]
            {
                "The Avengers: Endgame",
                "Spider-Man: No Way Home", 
                "Black Widow",
                "Venom: Let There Be Carnage",
                "The Matrix Resurrections",
                "No Time to Die",
                "Lightyear",
                "Super Mario Bros. Movie",
                "Avatar: The Way of Water",
                "Top Gun: Maverick",
                "Jurassic World: Dominion",
                "Thor: Love and Thunder",
                "Doctor Strange in the Multiverse of Madness",
                "The Batman",
                "Sonic the Hedgehog 2"
            };

            var random = new Random();
            return movies[random.Next(movies.Length)];
        }


}

public class PaymentRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}

public class ExecutePaymentRequest
{
    public string PaymentId { get; set; }
    public string PayerId { get; set; }
}

// Classes for deserializing basket data from Redis
public class BasketData
{
    public string Username { get; set; } = string.Empty;
    public List<BasketItem> Items { get; set; } = new List<BasketItem>();
    public decimal TotalPrice { get; set; }
}

public class BasketItem
{
    public decimal Price { get; set; }
    public string MovieId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Rating { get; set; }
}