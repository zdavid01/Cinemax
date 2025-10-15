using EventBus.Messages.Events;
using MassTransit;

namespace Email.API.Services
{
    public class MessageProducer : IMessageProducer
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<MessageProducer> _logger;

        public MessageProducer(IPublishEndpoint publishEndpoint, ILogger<MessageProducer> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishEmailAsync(SendEmailEvent emailEvent)
        {
            try
            {
                await _publishEndpoint.Publish(emailEvent);
                _logger.LogInformation("Email event published successfully for {To}", emailEvent.To);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish email event for {To}", emailEvent.To);
                throw;
            }
        }
    }
}


