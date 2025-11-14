using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using eVOL.Domain.RepositoriesInteraces;
using Microsoft.Extensions.Logging;
using eVOL.Application.UseCases.SupportTicketCases;
using eVOL.Domain.Entities;


namespace eVOL.ApplicationTests.UseCases.SupportTicketCases
{
    public class SendSupportTicketMessageTest
    {

        [Fact]
        public async Task SendSupportTicketMessage_SendMessageSuccessfully_ReturnChatMessageAndUser()
        {
            // Arrange

            var uowMysqlMock = new Mock<IMySqlUnitOfWork>();
            var uowMongoMock = new Mock<IMongoUnitOfWork>();
            var supportTicketRepoMock = new Mock<ISupportTicketRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var messageRepoMock = new Mock<IMessageRepository>();
            var loggerMock = new Mock<ILogger<SendSupportTicketMessageUseCase>>();

            uowMysqlMock.Setup(u => u.SupportTicket).Returns(supportTicketRepoMock.Object);
            uowMysqlMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            uowMongoMock.Setup(u => u.Message).Returns(messageRepoMock.Object);

            uowMysqlMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMongoMock.Setup(u => u.BeginTransactionAsync()).Verifiable();

            uowMysqlMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMongoMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            uowMysqlMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);
            uowMongoMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeUser = new User
            {
                UserId = 1, 
            };

            var fakeSupportTicket = new SupportTicket
            {
                Id = 1,
                Name = "Test Support Ticket"
            };

            supportTicketRepoMock.Setup(s => s.GetSupportTicketByName("Test Support Ticket"))
                .ReturnsAsync(fakeSupportTicket);

            userRepoMock.Setup(u => u.GetUserById(1))
                .ReturnsAsync(fakeUser);

            messageRepoMock.Setup(m => m.AddChatMessageToDb(It.IsAny<ChatMessage>()));

            var sut = new SendSupportTicketMessageUseCase(uowMongoMock.Object, uowMysqlMock.Object, loggerMock.Object);

            // Act

            var (chatMessage, user) = await sut.ExecuteAsync("Test Message", fakeSupportTicket.Name, fakeUser.UserId);

            // Assert

            Assert.NotNull(chatMessage);
            Assert.NotNull(user);
            Assert.Equal(fakeUser.UserId, user.UserId);
            Assert.Equal(fakeUser.Name, user.Name);

            uowMysqlMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMongoMock.Verify(u => u.BeginTransactionAsync(), Times.Once);

            uowMysqlMock.Verify(u => u.CommitAsync(), Times.Once);
            uowMongoMock.Verify(u => u.CommitAsync(), Times.Once);

            uowMysqlMock.Verify(u => u.RollbackAsync(), Times.Never);
            uowMongoMock.Verify(u => u.RollbackAsync(), Times.Never);

            supportTicketRepoMock.Verify(s => s.GetSupportTicketByName(It.IsAny<string>()), Times.Once);

            userRepoMock.Verify(u => u.GetUserById(1), Times.Once);

            messageRepoMock.Verify(m => m.AddChatMessageToDb(It.IsAny<ChatMessage>()), Times.Once);
        }

        [Fact]
        public async Task SendSupportTicketMessage_SupportTicketOrUserNull_ReturnNull()
        {

            // Arrange

            var uowMysqlMock = new Mock<IMySqlUnitOfWork>();
            var uowMongoMock = new Mock<IMongoUnitOfWork>();
            var supportTicketRepoMock = new Mock<ISupportTicketRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<SendSupportTicketMessageUseCase>>();

            uowMysqlMock.Setup(u => u.SupportTicket).Returns(supportTicketRepoMock.Object);
            uowMysqlMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            uowMysqlMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMongoMock.Setup(u => u.BeginTransactionAsync()).Verifiable();

            uowMysqlMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMongoMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            uowMysqlMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);
            uowMongoMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            supportTicketRepoMock.Setup(s => s.GetSupportTicketByName("Test Support Ticket"))
                .ReturnsAsync((SupportTicket?)null);

            userRepoMock.Setup(u => u.GetUserById(1))
                .ReturnsAsync((User?)null);

            var sut = new SendSupportTicketMessageUseCase(uowMongoMock.Object, uowMysqlMock.Object, loggerMock.Object);

            // Act

            var (chatMessage, user) = await sut.ExecuteAsync("Test Message", "Test Support Ticket", 1);

            // Assert

            Assert.Null(chatMessage);
            Assert.Null(user);

            uowMysqlMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMongoMock.Verify(u => u.BeginTransactionAsync(), Times.Once);

            uowMysqlMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMongoMock.Verify(u => u.CommitAsync(), Times.Never);

            uowMysqlMock.Verify(u => u.RollbackAsync(), Times.Never);
            uowMongoMock.Verify(u => u.RollbackAsync(), Times.Never);

            supportTicketRepoMock.Verify(s => s.GetSupportTicketByName(It.IsAny<string>()), Times.Once);

            userRepoMock.Verify(u => u.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task SendSupportTicketMessage_ThrowException_ReturnNothing()
        {
            // Arrange

            var uowMysqlMock = new Mock<IMySqlUnitOfWork>();
            var uowMongoMock = new Mock<IMongoUnitOfWork>();
            var supportTicketRepoMock = new Mock<ISupportTicketRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<SendSupportTicketMessageUseCase>>();

            uowMysqlMock.Setup(u => u.SupportTicket).Returns(supportTicketRepoMock.Object);
            uowMysqlMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            uowMysqlMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMongoMock.Setup(u => u.BeginTransactionAsync()).Verifiable();

            uowMysqlMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMongoMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            uowMysqlMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);
            uowMongoMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            supportTicketRepoMock.Setup(s => s.GetSupportTicketByName("Test Support Ticket"))
                .ThrowsAsync(new Exception("Database error"));

            var sut = new SendSupportTicketMessageUseCase(uowMongoMock.Object, uowMysqlMock.Object, loggerMock.Object);

            // Act & Assert

            await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync("Test Message", "Test Support Ticket", 1));

            uowMysqlMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMongoMock.Verify(u => u.BeginTransactionAsync(), Times.Once);

            uowMysqlMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMongoMock.Verify(u => u.CommitAsync(), Times.Never);

            uowMysqlMock.Verify(u => u.RollbackAsync(), Times.Once);
            uowMongoMock.Verify(u => u.RollbackAsync(), Times.Once);

            supportTicketRepoMock.Verify(s => s.GetSupportTicketByName(It.IsAny<string>()), Times.Once);
        }

    }
}
