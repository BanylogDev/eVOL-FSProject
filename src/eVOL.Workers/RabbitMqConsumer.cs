using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class RabbitMqConsumer : BackgroundService
{
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly IMongoUnitOfWork _uow;

    public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger, IMongoUnitOfWork uow)
    {
        _logger = logger;
        _uow = uow;
    }

    public async Task HandleMessageAsync(string json)
    {
        var chatMessage = JsonSerializer.Deserialize<ChatMessage>(json);

        if (chatMessage == null)
        {
            return;
        }

        _uow.BeginTransactionAsync();
        await _uow.Message.AddChatMessageToDb(chatMessage);
        await _uow.CommitAsync();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = "evol.rabbitmq" };
        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        _uow.BeginTransactionAsync();

        try
        {
            await channel.QueueDeclareAsync("chat_queue", durable: true, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += (model, ea) =>
            {
                _ = Task.Run(async () =>
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    await HandleMessageAsync(json);
                });

                return Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(
                queue: "chat_queue",
                autoAck: false,
                consumer: consumer);
        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync();
        }
    }
}
