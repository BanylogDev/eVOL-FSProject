using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using eVOL.Domain.RepositoriesInteraces;
using Microsoft.Extensions.Logging;
using eVOL.Domain.Entities;
using System.Text.Json;

namespace eVOL.WorkersTests
{
    public class RabbitMqConsumerTest
    {
        [Fact]
        public async Task RabbitMqConsumer_ConsumeMessageSuccessfully_ReturnNothing()
        {
            // Arrange

            var uowMock = new Mock<IMongoUnitOfWork>();
            var loggerMock = new Mock<ILogger<RabbitMqConsumer>>();

            uowMock.Setup(u => u.BeginTransactionAsync()).Verifiable();
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);


            var chatMessage = new ChatMessage
            {
                Text = "Test",
                SenderId = 1,
                ReceiverId = 1,
                CreatedAt = DateTime.UtcNow,
                MessageId = 1,
            };

            uowMock.Setup(u => u.Message.AddChatMessageToDb(chatMessage));

            var sut = new RabbitMqConsumer(loggerMock.Object, uowMock.Object);

            // Act

            await sut.HandleMessageAsync(JsonSerializer.Serialize(chatMessage));

            // Assert

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Once);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            uowMock.Verify(u => u.Message.AddChatMessageToDb(It.IsAny<ChatMessage>()), Times.Once);
        }

    }
}
