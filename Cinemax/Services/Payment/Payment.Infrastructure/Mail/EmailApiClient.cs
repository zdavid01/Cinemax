using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Payment.Application.Contracts.Infrastructure;
using Payment.Application.Models;

namespace Payment.Infrastructure.Mail;

/// <summary>
/// Email client that sends emails via Email.API microservice
/// </summary>
public class EmailApiClient : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmailApiClient> _logger;
    private readonly string _emailApiBaseUrl;

    public EmailApiClient(HttpClient httpClient, ILogger<EmailApiClient> logger, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Get Email.API base URL from configuration, default to docker service name
        _emailApiBaseUrl = configuration["EmailApi:BaseUrl"] ?? "http://email.api:8080";
        _httpClient.BaseAddress = new Uri(_emailApiBaseUrl);
    }

    public async Task<bool> SendEmail(Email emailRequest)
    {
        try
        {
            var request = new
            {
                to = emailRequest.To,
                subject = emailRequest.Subject,
                body = emailRequest.Body,
                isHtml = true,
                priority = 1
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending email via Email.API to: {To}", emailRequest.To);

            var response = await _httpClient.PostAsync("/api/email/send-async", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email queued successfully via Email.API for {To}", emailRequest.To);
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to queue email via Email.API. Status: {StatusCode}, Error: {Error}", 
                    response.StatusCode, errorContent);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email via Email.API to {To}", emailRequest.To);
            return false;
        }
    }
}

