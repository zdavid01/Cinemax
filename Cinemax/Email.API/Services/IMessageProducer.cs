using EventBus.Messages.Events;

namespace Email.API.Services
{
    public interface IMessageProducer
    {
        Task PublishEmailAsync(SendEmailEvent emailEvent);
    }
}


