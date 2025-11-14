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
    public class AddUserToChatGroupTest
    {
        [Fact]
        public async Task AddUserToChatGroup_AddUserToChatGroup_RetursUser()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var loggerMock = new Mock<ILogger<AddUserToChatGroupUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeUser = new User
            {
                UserId = 1,
            };

            userRepoMock.Setup(u => u.GetUserById(It.IsAny<int>())).ReturnsAsync(fakeUser);

            chatGroupRepoMock.Setup(c => c.GetChatGroupByName(It.IsAny<string>())).ReturnsAsync(new ChatGroup
            {
                Id = 1,
                Name = "TestGroup",
                TotalUsers = 0,
                GroupUsers = new List<User>(),
                OwnerId = 2,
            });

            var sut = new AddUserToChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(fakeUser.UserId, "TestGroup");

            // Assert

            Assert.NotNull(result);
            Assert.Equal(fakeUser.UserId, result.UserId);
            
            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Once);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(u => u.GetUserById(It.IsAny<int>()), Times.Once);

            chatGroupRepoMock.Verify(c => c.GetChatGroupByName(It.IsAny<string>()), Times.Once);

        }


        [Fact]
        public async Task AddUserToChatGroup_AddUserNull_ReturnsNull()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var loggerMock = new Mock<ILogger<AddUserToChatGroupUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            userRepoMock.Setup(u => u.GetUserById(It.IsAny<int>())).ReturnsAsync((User?)null);

            var sut = new AddUserToChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(1, "TestGroup");

            // Assert

            Assert.Null(result);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(u => u.GetUserById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task AddUserToChatGroup_ThrowException_ReturnsNothing()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var loggerMock = new Mock<ILogger<AddUserToChatGroupUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            userRepoMock.Setup(u => u.GetUserById(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            var sut = new AddUserToChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act & Assert

            await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(1, "TestGroup"));

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Once);

            userRepoMock.Verify(u => u.GetUserById(It.IsAny<int>()), Times.Once);
        }

    }
}
