using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Payment.Application.Contracts.Infrastructure;
using Payment.Application.Models;

namespace Payment.Infrastructure.Mail;

/// <summary>
/// Email publisher that sends email events via RabbitMQ to Email.API microservice
/// </summary>
public class EmailPublisher : IEmailService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<EmailPublisher> _logger;

    public EmailPublisher(IPublishEndpoint publishEndpoint, ILogger<EmailPublisher> logger)
    {
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> SendEmail(Email emailRequest)
    {
        try
        {
            var emailEvent = new SendEmailEvent
            {
                To = emailRequest.To,
                Subject = emailRequest.Subject,
                Body = emailRequest.Body,
                From = "noreply@cinemax.com",
                IsHtml = true,
                Priority = 1
            };

            await _publishEndpoint.Publish(emailEvent);
            
            _logger.LogInformation("Email event published to RabbitMQ for {To} with subject '{Subject}'", 
                emailRequest.To, emailRequest.Subject);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish email event to RabbitMQ for {To}", emailRequest.To);
            return false;
        }
    }
}

