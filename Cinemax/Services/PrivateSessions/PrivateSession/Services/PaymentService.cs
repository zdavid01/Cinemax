using System.Text.Json;

namespace PrivateSession.Services;

public class PaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(HttpClient httpClient, IConfiguration configuration, ILogger<PaymentService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IEnumerable<string>> GetPurchasedMovieNamesAsync(string username)
    {
        try
        {
            var paymentApiUrl = _configuration.GetValue<string>("PaymentApi:BaseUrl") ?? "http://payment.api:8080";
            var url = $"{paymentApiUrl}/Payment/get-payments/{username}";
            
            _logger.LogInformation($"Fetching purchased movies for user: {username} from {url}");
            
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Failed to get payments for user {username}. Status: {response.StatusCode}");
                return Enumerable.Empty<string>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var payments = JsonSerializer.Deserialize<List<PaymentViewModel>>(content, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });

            if (payments == null || !payments.Any())
            {
                _logger.LogInformation($"No payments found for user {username}");
                return Enumerable.Empty<string>();
            }

            // Extract unique movie names from all payment items
            var movieNames = payments
                .SelectMany(p => p.PaymentItems ?? Enumerable.Empty<PaymentItemViewModel>())
                .Select(item => item.MovieName)
                .Distinct()
                .ToList();

            _logger.LogInformation($"User {username} has purchased {movieNames.Count} unique movies");
            
            return movieNames;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching purchased movies for user {username}");
            return Enumerable.Empty<string>();
        }
    }
}

// DTOs to match Payment API response
public class PaymentViewModel
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "";
    public string BuyerId { get; set; } = "";
    public string BuyerUsername { get; set; } = "";
    public DateTime PaymentDate { get; set; }
    public decimal TotalPrice { get; set; }
    public IEnumerable<PaymentItemViewModel>? PaymentItems { get; set; }
}

public class PaymentItemViewModel
{
    public int Id { get; set; }
    public string MovieName { get; set; } = "";
    public string MovieId { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

