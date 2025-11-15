using eVOL.Application.Messaging.Interfaces;
using eVOL.Domain.Entities;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMqPublisher : IRabbitMqPublisher
{
    public async Task PublishAsync(ChatMessage message)
    {
        var factory = new ConnectionFactory { HostName = "evol.rabbitmq" };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync("chat_exchange", "direct", durable: true);
        await channel.QueueDeclareAsync("chat_queue", durable: true, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync("chat_queue", "chat_exchange", "chat_key");

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        await channel.BasicPublishAsync(
            exchange: "chat_exchange",
            routingKey: "chat_key",
            mandatory: true,
            basicProperties: new BasicProperties { Persistent = true },
            body: body);

    }
}
