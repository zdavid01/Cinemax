using Email.API.Models;
using System.Text.Json;

namespace Email.API.Services
{
    /// <summary>
    /// Client service for other microservices to send emails via Email.API
    /// </summary>
    public class EmailApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EmailApiClient> _logger;
        private readonly string _emailApiBaseUrl;

        public EmailApiClient(HttpClient httpClient, ILogger<EmailApiClient> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _emailApiBaseUrl = configuration["EmailApi:BaseUrl"] ?? "http://email.api:8080";
        }

        /// <summary>
        /// Send email asynchronously via Email.API
        /// </summary>
        /// <param name="emailRequest">Email details</param>
        /// <returns>Email event ID if successful</returns>
        public async Task<string?> SendEmailAsync(EmailRequest emailRequest)
        {
            try
            {
                var json = JsonSerializer.Serialize(emailRequest);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_emailApiBaseUrl}/api/email/send-async", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<string>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse?.Success == true)
                    {
                        _logger.LogInformation("Email queued successfully for {To}, Event ID: {EventId}", 
                            emailRequest.To, apiResponse.Data);
                        return apiResponse.Data;
                    }
                }

                _logger.LogWarning("Failed to send email to {To}. Status: {StatusCode}", 
                    emailRequest.To, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {To}", emailRequest.To);
                return null;
            }
        }

        /// <summary>
        /// Send simple email with basic parameters
        /// </summary>
        /// <param name="to">Recipient email</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body</param>
        /// <param name="isHtml">Whether body is HTML</param>
        /// <returns>Email event ID if successful</returns>
        public async Task<string?> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            var emailRequest = new EmailRequest
            {
                To = to,
                Subject = subject,
                Body = body,
                IsHtml = isHtml
            };

            return await SendEmailAsync(emailRequest);
        }

        /// <summary>
        /// Check if Email.API service is healthy
        /// </summary>
        /// <returns>True if service is healthy</returns>
        public async Task<bool> IsHealthyAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_emailApiBaseUrl}/health");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking Email.API health");
                return false;
            }
        }
    }

    /// <summary>
    /// Extension methods for registering EmailApiClient
    /// </summary>
    public static class EmailApiClientExtensions
    {
        public static IServiceCollection AddEmailApiClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<EmailApiClient>();
            services.AddScoped<EmailApiClient>();
            return services;
        }
    }
}


