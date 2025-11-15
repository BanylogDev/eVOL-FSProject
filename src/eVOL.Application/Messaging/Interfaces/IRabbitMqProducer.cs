using eVOL.Domain.Entities;

namespace eVOL.Application.Messaging.Interfaces;

public interface IRabbitMqPublisher
{
    Task PublishAsync(ChatMessage message);
}
