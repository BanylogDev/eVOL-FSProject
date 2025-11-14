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
    public class LeaveChatGroupTest
    {

        [Fact]
        public async Task LeaveChatGroup_LeaveChatGroup_WithoutDeletionOfGroup_ReturnsUser()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<LeaveChatGroupUseCase>>();

            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);
            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

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
                TotalUsers = 5,
                GroupUsers = new List<User> { fakeUser },
                OwnerId = 1,
            };

            userRepoMock.Setup(u => u.GetUserById(1))
                .ReturnsAsync(fakeUser);

            chatGroupRepoMock.Setup(c => c.GetChatGroupByName("TestGroup"))
                .ReturnsAsync(fakeChatGroup);



            var sut = new LeaveChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(1, "TestGroup");

            // Assert   

            Assert.NotNull(result);
            Assert.Equal(fakeUser.UserId, result.UserId);
            
            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Once);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(ur => ur.GetUserById(1), Times.Once);

            chatGroupRepoMock.Verify(cg => cg.GetChatGroupByName("TestGroup"), Times.Once);
            chatGroupRepoMock.Verify(cg => cg.DeleteChatGroup(It.IsAny<ChatGroup>()), Times.Never);

        }


        [Fact]
        public async Task LeaveChatGroup_LeaveChatGroup_WithDeletionOfGroup_ReturnsUser()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<LeaveChatGroupUseCase>>();

            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);
            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

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
                GroupUsers = new List<User> { fakeUser },
                OwnerId = 1,
            };

            userRepoMock.Setup(u => u.GetUserById(1))
                .ReturnsAsync(fakeUser);

            chatGroupRepoMock.Setup(c => c.GetChatGroupByName("TestGroup"))
                .ReturnsAsync(fakeChatGroup);

            chatGroupRepoMock.Setup(c => c.DeleteChatGroup(fakeChatGroup))
                .Returns(fakeChatGroup);

            var sut = new LeaveChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(1, "TestGroup");

            // Assert   

            Assert.NotNull(result);
            Assert.Equal(fakeUser.UserId, result.UserId);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Once);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(ur => ur.GetUserById(1), Times.Once);

            chatGroupRepoMock.Verify(cg => cg.GetChatGroupByName("TestGroup"), Times.Once);
            chatGroupRepoMock.Verify(cg => cg.DeleteChatGroup(fakeChatGroup), Times.Once);

        }

        [Fact]
        public async Task LeaveChatGroup_ChatGroupOrUserNull_ReturnNull()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<LeaveChatGroupUseCase>>();

            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);
            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            userRepoMock.Setup(u => u.GetUserById(1))
                .ReturnsAsync((User?)null);

            chatGroupRepoMock.Setup(c => c.GetChatGroupByName("TestGroup"))
                .ReturnsAsync((ChatGroup?)null);

            var sut = new LeaveChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(1, "TestGroup");

            // Assert   

            Assert.Null(result);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(ur => ur.GetUserById(1), Times.Once);

            chatGroupRepoMock.Verify(cg => cg.GetChatGroupByName("TestGroup"), Times.Once);
            chatGroupRepoMock.Verify(cg => cg.DeleteChatGroup(It.IsAny<ChatGroup>()), Times.Never);
        }

        [Fact]
        public async Task LeaveChatGroup_UserNotInChatGroup_ReturnNull()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<LeaveChatGroupUseCase>>();

            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);
            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

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
                TotalUsers = 5,
                GroupUsers = new List<User>(),
                OwnerId = 1,
            };

            userRepoMock.Setup(u => u.GetUserById(1))
                .ReturnsAsync(fakeUser);

            chatGroupRepoMock.Setup(c => c.GetChatGroupByName("TestGroup"))
                .ReturnsAsync(fakeChatGroup);

            var sut = new LeaveChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(1, "TestGroup");

            // Assert   

            Assert.Null(result);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(ur => ur.GetUserById(1), Times.Once);

            chatGroupRepoMock.Verify(cg => cg.GetChatGroupByName("TestGroup"), Times.Once);
            chatGroupRepoMock.Verify(cg => cg.DeleteChatGroup(It.IsAny<ChatGroup>()), Times.Never);
        }

        [Fact]
        public async Task LeaveChatGroup_ThrowException_ReturnNothing()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<LeaveChatGroupUseCase>>();

            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);
            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            userRepoMock.Setup(u => u.GetUserById(1))
                .ThrowsAsync(new Exception("Database error"));

            var sut = new LeaveChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act & Assert

            await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(1, "TestGroup"));

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Once);

            userRepoMock.Verify(ur => ur.GetUserById(1), Times.Once);
        }

    }
}
