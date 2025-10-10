using Email.API.Models;
using Email.API.Services;
using EventBus.Messages.Events;
using MassTransit;

namespace Email.API.Consumers
{
    public class SendEmailConsumer : IConsumer<SendEmailEvent>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<SendEmailConsumer> _logger;

        public SendEmailConsumer(IEmailService emailService, ILogger<SendEmailConsumer> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SendEmailEvent> context)
        {
            try
            {
                _logger.LogInformation("Processing email event for {To}", context.Message.To);

                var emailRequest = new EmailRequest
                {
                    To = context.Message.To,
                    Subject = context.Message.Subject,
                    Body = context.Message.Body,
                    From = context.Message.From,
                    IsHtml = context.Message.IsHtml,
                    Cc = context.Message.Cc,
                    Bcc = context.Message.Bcc,
                    Attachments = context.Message.Attachments,
                    Priority = context.Message.Priority
                };

                var result = await _emailService.SendEmailAsync(emailRequest);

                if (result)
                {
                    _logger.LogInformation("Email sent successfully for event {EventId} to {To}", 
                        context.Message.Id, context.Message.To);
                }
                else
                {
                    _logger.LogError("Failed to send email for event {EventId} to {To}", 
                        context.Message.Id, context.Message.To);
                    
                    // You might want to implement retry logic or dead letter queue here
                    throw new Exception($"Failed to send email to {context.Message.To}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing email event {EventId} for {To}", 
                    context.Message.Id, context.Message.To);
                throw;
            }
        }
    }
}

