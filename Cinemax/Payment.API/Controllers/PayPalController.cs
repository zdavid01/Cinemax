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

namespace Payment.API.Controllers;

[Route("api/paypal")]
[ApiController]
public class PayPalController : ControllerBase
{
    private readonly PayPalService? _payPalService;
    private readonly ILogger<PayPalController> _logger;
    private readonly IEmailService _emailService;
    private readonly IMediator _mediator;

    public PayPalController(ILogger<PayPalController> logger, IEmailService emailService, PayPalService payPalService, IMediator mediator)
    {
        _payPalService = payPalService ?? throw new ArgumentNullException(nameof(payPalService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
        // Endpoint to initiate the payment process
        [HttpPost("create-payment")]
        [Authorize]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest paymentRequest)
        {
            try
            {
                // Get the user's username from the JWT token
                var userEmail = GetUserEmailFromToken();
                var username = userEmail.Split('@')[0]; // Use email prefix as username
                
                // Create return URL with username parameter
                var returnUrl = $"http://localhost:8004/api/paypal/return?username={username}";
                var cancelUrl = "http://localhost:8004/api/paypal/cancel";
                
                var (paymentId, approvalUrl) = await _payPalService.CreatePayment(paymentRequest.Amount, paymentRequest.Currency, returnUrl, cancelUrl);
                
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
    
    [HttpGet("return")]
    public async Task<IActionResult> ReturnFromPayPal(string paymentId, string payerId, string? username = null)
    {
        try
        {
            // Execute the payment after the user approves it
            var paymentState = await _payPalService.ExecutePayment(paymentId, payerId);

            // If the payment was successful, save to database and send emails
            if (paymentState == "approved")
            {
                await ProcessSuccessfulPayment(paymentId, payerId, username);
                
                var html = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Payment Successful</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; text-align: center; padding: 50px; background-color: #f0f8f0; }}
                        .success {{ color: #4caf50; font-size: 24px; margin-bottom: 20px; }}
                        .message {{ color: #333; font-size: 16px; margin-bottom: 30px; }}
                        .close-btn {{ background-color: #4caf50; color: white; padding: 10px 20px; border: none; border-radius: 5px; cursor: pointer; font-size: 16px; }}
                    </style>
                </head>
                <body>
                    <div class='success'>✅ Payment Successful!</div>
                    <div class='message'>Your payment has been processed and a confirmation email has been sent.</div>
                    <button class='close-btn' onclick='closeAndNotify()'>Close Window</button>
                    
                    <script>
                        function closeAndNotify() {{
                            // Notify the parent window about the successful payment
                            if (window.opener) {{
                                window.opener.postMessage({{
                                    type: 'PAYPAL_PAYMENT_SUCCESS',
                                    paymentId: '{paymentId}',
                                    payerId: '{payerId}',
                                    state: '{paymentState}'
                                }}, '*');
                            }}
                            window.close();
                        }}
                        
                        // Auto-close after 3 seconds
                        setTimeout(closeAndNotify, 3000);
                    </script>
                </body>
                </html>";
                
                return Content(html, "text/html");
            }
            else
            {
                var html = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Payment Failed</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; text-align: center; padding: 50px; background-color: #ffeaea; }}
                        .error {{ color: #f44336; font-size: 24px; margin-bottom: 20px; }}
                        .message {{ color: #333; font-size: 16px; margin-bottom: 30px; }}
                        .close-btn {{ background-color: #f44336; color: white; padding: 10px 20px; border: none; border-radius: 5px; cursor: pointer; font-size: 16px; }}
                    </style>
                </head>
                <body>
                    <div class='error'>❌ Payment Failed</div>
                    <div class='message'>Payment was not approved. Please try again.</div>
                    <button class='close-btn' onclick='closeAndNotify()'>Close Window</button>
                    
                    <script>
                        function closeAndNotify() {{
                            // Notify the parent window about the failed payment
                            if (window.opener) {{
                                window.opener.postMessage({{
                                    type: 'PAYPAL_PAYMENT_FAILED',
                                    paymentId: '{paymentId}',
                                    payerId: '{payerId}',
                                    state: '{paymentState}'
                                }}, '*');
                            }}
                            window.close();
                        }}
                        
                        // Auto-close after 3 seconds
                        setTimeout(closeAndNotify, 3000);
                    </script>
                </body>
                </html>";
                
                return Content(html, "text/html");
            }
        }
        catch (Exception ex)
        {
            var html = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <title>Payment Error</title>
                <style>
                    body {{ font-family: Arial, sans-serif; text-align: center; padding: 50px; background-color: #ffeaea; }}
                    .error {{ color: #f44336; font-size: 24px; margin-bottom: 20px; }}
                    .message {{ color: #333; font-size: 16px; margin-bottom: 30px; }}
                    .close-btn {{ background-color: #f44336; color: white; padding: 10px 20px; border: none; border-radius: 5px; cursor: pointer; font-size: 16px; }}
                </style>
            </head>
            <body>
                <div class='error'>❌ Payment Error</div>
                <div class='message'>An error occurred: {ex.Message}</div>
                <button class='close-btn' onclick='window.close()'>Close Window</button>
            </body>
            </html>";
            
            return Content(html, "text/html");
        }
    }


    private async Task ProcessSuccessfulPayment(string paymentId, string payerId, string? username = null)
    {
        try
        {
            // Save payment to database
            await SavePaymentToDatabase(paymentId, payerId, username);
            
            // Send emails
            await SendPaymentEmails(paymentId, payerId);
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
            
            // Create a payment entry for the database
            var createPaymentCommand = new CreatePaymentCommand
            {
                BuyerId = payerId, // Using PayPal payer ID as buyer ID
                BuyerUsername = actualUsername, // Use actual logged-in user's username
                Currency = "USD",
                PaymentItems = new List<PaymentItemDTO>
                {
                    new PaymentItemDTO
                    {
                        MovieName = "PayPal Payment",
                        MovieId = $"PAYPAL_{paymentId}",
                        Price = 0.10m, // Default amount for now
                        Quantity = 1
                    }
                }
            };

            var paymentDbId = await _mediator.Send(createPaymentCommand);
            _logger.LogInformation("Payment saved to database with ID: {PaymentDbId} for PayPal payment: {PaymentId} for user: {Username}", paymentDbId, paymentId, actualUsername);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to save payment to database for {PaymentId}", paymentId);
        }
    }

    private async Task SendPaymentEmails(string paymentId, string payerId)
    {
        // Get user's email from JWT token
        var userEmailAddress = GetUserEmailFromToken();
        
        // Send detailed payment notification to user
        try
        {
            var userEmail = new Email
            {
                To = userEmailAddress,
                Subject = $"Payment Confirmation - {paymentId}",
                Body = $"Your PayPal payment has been completed successfully.\n\nPayment ID: {paymentId}\nPayer ID: {payerId}\nAmount: $0.10 USD\nStatus: Approved\nTimestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n\nThank you for your payment!"
            };
            
            await _emailService.SendEmail(userEmail);
            _logger.LogInformation("User payment notification sent successfully to {Email} for payment {PaymentId}", userEmailAddress, paymentId);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to send user payment notification to {Email} for payment {PaymentId}. Email service may be down or credentials expired.", userEmailAddress, paymentId);
        }

        // Send admin notification email
        try
        {
            var adminEmail = new Email
            {
                To = "vida33085@gmail.com", // Admin email
                Subject = $"New PayPal Payment Completed - {paymentId}",
                Body = $"A new PayPal payment has been completed successfully.\n\nPayment ID: {paymentId}\nPayer ID: {payerId}\nUser Email: {userEmailAddress}\nAmount: $0.10 USD\nStatus: Approved\nTimestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC"
            };
            
            await _emailService.SendEmail(adminEmail);
            _logger.LogInformation("Admin email sent successfully for payment {PaymentId}", paymentId);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to send admin email for payment {PaymentId}. Email service may be down or credentials expired.", paymentId);
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