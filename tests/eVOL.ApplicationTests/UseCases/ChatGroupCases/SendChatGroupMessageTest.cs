using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using eVOL.Domain.RepositoriesInteraces;
using Microsoft.Extensions.Logging;
using eVOL.Application.UseCases.ChatGroupCases;
using eVOL.Domain.Entities;


namespace eVOL.ApplicationTests.UseCases.ChatGroupCases
{
    public class SendChatGroupMessageTest
    {
        [Fact]
        public async Task SendChatGroupMessage_SendMessageToChatGroup_ReturnsChatMessageAndUser()
        {
            // Arrange

            var uowMysqlMock = new Mock<IMySqlUnitOfWork>();
            var uowMongoMock = new Mock<IMongoUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<SendChatGroupMessageUseCase>>();

            uowMysqlMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);
            uowMysqlMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            uowMysqlMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMongoMock.Setup(u => u.BeginTransactionAsync()).Verifiable();

            uowMysqlMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMongoMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            uowMysqlMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);
            uowMongoMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeUser = new User
            {
                UserId = 1,
                Name = "User"
            };

            var fakeChatGroup = new ChatGroup
            {
                Id = 1,
                Name = "TestGroup"
            };

            userRepoMock.Setup(u => u.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(fakeUser);

            chatGroupRepoMock.Setup(c => c.GetChatGroupByName(It.IsAny<string>()))
                .ReturnsAsync(fakeChatGroup);

            uowMongoMock.Setup(m => m.Message.AddChatMessageToDb(It.IsAny<ChatMessage>()));

            var sut = new SendChatGroupMessageUseCase(uowMongoMock.Object, uowMysqlMock.Object, loggerMock.Object);

            // Act

            var (chatMessage, user) = await sut.ExecuteAsync("Test Message", "TestGroup", fakeUser.UserId);

            // Assert

            Assert.NotNull(chatMessage);
            Assert.NotNull(user);
            Assert.Equal("Test Message", chatMessage.Text);
            Assert.Equal(fakeUser.UserId, chatMessage.SenderId);
            Assert.Equal(fakeChatGroup.Id, chatMessage.ReceiverId);

            uowMysqlMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMongoMock.Verify(u => u.BeginTransactionAsync(), Times.Once);

            uowMysqlMock.Verify(u => u.CommitAsync(), Times.Once);
            uowMongoMock.Verify(u => u.CommitAsync(), Times.Once);

            uowMysqlMock.Verify(u => u.RollbackAsync(), Times.Never);
            uowMongoMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(u => u.GetUserById(It.IsAny<int>()), Times.Once);

            chatGroupRepoMock.Verify(c => c.GetChatGroupByName(It.IsAny<string>()), Times.Once);

            uowMongoMock.Verify(m => m.Message.AddChatMessageToDb(It.IsAny<ChatMessage>()), Times.Once);

        }

        [Fact]
        public async Task SendChatGroupMessage_ChatGroupOrUserNull_ReturnNull()
        {
            // Arrange

            var uowMysqlMock = new Mock<IMySqlUnitOfWork>();
            var uowMongoMock = new Mock<IMongoUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<SendChatGroupMessageUseCase>>();

            uowMysqlMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);
            uowMysqlMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            uowMysqlMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMongoMock.Setup(u => u.BeginTransactionAsync()).Verifiable();

            uowMysqlMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMongoMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            uowMysqlMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);
            uowMongoMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeUser = new User
            {
                UserId = 1,
                Name = "User"
            };

            userRepoMock.Setup(u => u.GetUserById(It.IsAny<int>()))
                .ReturnsAsync((User?)null);

            chatGroupRepoMock.Setup(c => c.GetChatGroupByName(It.IsAny<string>()))
                .ReturnsAsync((ChatGroup?)null);

            var sut = new SendChatGroupMessageUseCase(uowMongoMock.Object, uowMysqlMock.Object, loggerMock.Object);

            // Act

            var (chatMessage, user) = await sut.ExecuteAsync("Test Message", "TestGroup", fakeUser.UserId);

            // Assert

            Assert.Null(chatMessage);
            Assert.Null(user);

            uowMysqlMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMongoMock.Verify(u => u.BeginTransactionAsync(), Times.Once);

            uowMysqlMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMongoMock.Verify(u => u.CommitAsync(), Times.Never);

            uowMysqlMock.Verify(u => u.RollbackAsync(), Times.Never);
            uowMongoMock.Verify(u => u.RollbackAsync(), Times.Never);

            chatGroupRepoMock.Verify(c => c.GetChatGroupByName(It.IsAny<string>()), Times.Once);

            userRepoMock.Verify(u => u.GetUserById(It.IsAny<int>()), Times.Once);

            uowMongoMock.Verify(m => m.Message.AddChatMessageToDb(It.IsAny<ChatMessage>()), Times.Never);
        }

        [Fact]
        public async Task SendChatGroupMessage_ThrowException_ReturnNothing()
        {
            // Arrange
            var uowMysqlMock = new Mock<IMySqlUnitOfWork>();
            var uowMongoMock = new Mock<IMongoUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<SendChatGroupMessageUseCase>>();

            uowMysqlMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);
            uowMysqlMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            uowMysqlMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMongoMock.Setup(u => u.BeginTransactionAsync()).Verifiable();

            uowMysqlMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMongoMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            uowMysqlMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);
            uowMongoMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            chatGroupRepoMock.Setup(c => c.GetChatGroupByName(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Database error"));

            var sut = new SendChatGroupMessageUseCase(uowMongoMock.Object, uowMysqlMock.Object, loggerMock.Object);

            // Act & Assert

            await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync("Test Message", "TestGroup", 1));

            uowMysqlMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMongoMock.Verify(u => u.BeginTransactionAsync(), Times.Once);

            uowMysqlMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMongoMock.Verify(u => u.CommitAsync(), Times.Never);

            uowMysqlMock.Verify(u => u.RollbackAsync(), Times.Once);
            uowMongoMock.Verify(u => u.RollbackAsync(), Times.Once);

            chatGroupRepoMock.Verify(c => c.GetChatGroupByName(It.IsAny<string>()), Times.Once);

            userRepoMock.Verify(u => u.GetUserById(It.IsAny<int>()), Times.Never);

            uowMongoMock.Verify(m => m.Message.AddChatMessageToDb(It.IsAny<ChatMessage>()), Times.Never);
        }
    }
}
