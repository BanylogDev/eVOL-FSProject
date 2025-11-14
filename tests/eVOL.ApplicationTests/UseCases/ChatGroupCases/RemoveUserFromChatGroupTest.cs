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
    public class RemoveUserFromChatGroupTest
    {
        [Fact]
        public async Task RemoveUserFromChatGroup_RemoveUserFromChatGroup_ReturnsUser()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var loggerMock = new Mock<ILogger<RemoveUserFromChatGroupUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeUser = new User
            {
                UserId = 1,
            };

            var fakeChatGroup = new ChatGroup
            {
                Id = 1,
                Name = "TestGroup",
                TotalUsers = 2,
                GroupUsers = new List<User> { fakeUser },
                OwnerId = 2,
            };

            userRepoMock.Setup(u => u.GetUserById(It.IsAny<int>())).ReturnsAsync(fakeUser);

            chatGroupRepoMock.Setup(c => c.GetChatGroupByName(It.IsAny<string>())).ReturnsAsync(fakeChatGroup);

            var sut = new RemoveUserFromChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(fakeUser.UserId, "TestGroup");

            // Assert

            Assert.NotNull(result);
            Assert.Equal(fakeUser.UserId, result.UserId);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Once);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(u => u.GetUserById(fakeUser.UserId), Times.Once);

            chatGroupRepoMock.Verify(c => c.GetChatGroupByName("TestGroup"), Times.Once);
        }

        [Fact]
        public async Task RemoveUserFromChatGroup_UserNotInTheChatGroup_ReturnsNull()
        {
            // Arrange
            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var loggerMock = new Mock<ILogger<RemoveUserFromChatGroupUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeUser = new User
            {
                UserId = 1,
            };

            var fakeChatGroup = new ChatGroup
            {
                Id = 1,
                Name = "TestGroup",
                TotalUsers = 1,
                GroupUsers = new List<User>(),
                OwnerId = 2,
            };
            userRepoMock.Setup(u => u.GetUserById(It.IsAny<int>())).ReturnsAsync(fakeUser);

            chatGroupRepoMock.Setup(c => c.GetChatGroupByName(It.IsAny<string>())).ReturnsAsync(fakeChatGroup);

            var sut = new RemoveUserFromChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(fakeUser.UserId, "TestGroup");

            // Assert

            Assert.Null(result);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(u => u.GetUserById(fakeUser.UserId), Times.Once);

            chatGroupRepoMock.Verify(c => c.GetChatGroupByName("TestGroup"), Times.Once);
        }

        [Fact]
        public async Task RemoveUserFromChatGroup_UserIsOwner_ReturnNull()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var loggerMock = new Mock<ILogger<RemoveUserFromChatGroupUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeUser = new User
            {
                UserId = 2,
            };

            var fakeChatGroup = new ChatGroup
            {
                Id = 1,
                Name = "TestGroup",
                TotalUsers = 1,
                GroupUsers = new List<User> { fakeUser },
                OwnerId = 2,
            };

            userRepoMock.Setup(u => u.GetUserById(It.IsAny<int>())).ReturnsAsync(fakeUser);

            chatGroupRepoMock.Setup(c => c.GetChatGroupByName(It.IsAny<string>())).ReturnsAsync(fakeChatGroup);

            var sut = new RemoveUserFromChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(fakeUser.UserId, "TestGroup");

            // Assert

            Assert.Null(result);
            Assert.Equal(fakeChatGroup.OwnerId, fakeUser.UserId);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(u => u.GetUserById(fakeUser.UserId), Times.Once);

            chatGroupRepoMock.Verify(c => c.GetChatGroupByName("TestGroup"), Times.Once);
        }

        [Fact]
        public async Task RemoveUserFromChatGroup_ThrowException_ReturnNothing()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var loggerMock = new Mock<ILogger<RemoveUserFromChatGroupUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            userRepoMock.Setup(u => u.GetUserById(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            var sut = new RemoveUserFromChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act & Assert

            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await sut.ExecuteAsync(1, "TestGroup");
            });

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Once);

            userRepoMock.Verify(u => u.GetUserById(It.IsAny<int>()), Times.Once);
        }
    }
}
