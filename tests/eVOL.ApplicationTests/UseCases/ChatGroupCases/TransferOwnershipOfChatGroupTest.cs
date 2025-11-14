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
    public class TransferOwnershipOfChatGroupTest
    {

        [Fact]
        public async Task TransferOwnershipOfChatGroup_SuccessfullyTransferOwnership_ReturnChatGroup()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<TransferOwnershipOfChatGroupUseCase>>();

            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);
            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeCurrentOwner = new User
            {
                UserId = 1,
                Name = "CurrentOwner"
            };

            var fakeNewOwner = new User
            {
                UserId = 2,
                Name = "NewOwner"
            };

            var fakeChatGroup = new ChatGroup
            {
                Id = 1,
                Name = "TestGroup",
                OwnerId = 1
            };

            userRepoMock.Setup(u => u.GetUserById(1))
                .ReturnsAsync(fakeCurrentOwner);

            userRepoMock.Setup(u => u.GetUserById(2))
                .ReturnsAsync(fakeNewOwner);

            chatGroupRepoMock.Setup(c => c.GetChatGroupById(1))
                .ReturnsAsync(fakeChatGroup);

            var sut = new TransferOwnershipOfChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(fakeCurrentOwner.UserId, fakeNewOwner.UserId, fakeChatGroup.Id);

            // Assert

            Assert.NotNull(result);
            Assert.Equal(fakeNewOwner.UserId, result.OwnerId);
            Assert.Equal(fakeChatGroup.Id, result.Id);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Once);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(u => u.GetUserById(It.IsAny<int>()), Times.Exactly(2));

            chatGroupRepoMock.Verify(c => c.GetChatGroupById(It.IsAny<int>()), Times.Once);

        }

        [Fact]
        public async Task TransferOwnershipOfChatGroup_NewOwnerOrCurrentOwnerOrChatGroupNull_ReturnNull()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<TransferOwnershipOfChatGroupUseCase>>();

            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);
            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            userRepoMock.Setup(u => u.GetUserById(It.IsAny<int>()))
                .ReturnsAsync((User?)null);

            chatGroupRepoMock.Setup(c => c.GetChatGroupById(It.IsAny<int>()))
                .ReturnsAsync((ChatGroup?)null);

            var sut = new TransferOwnershipOfChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(1, 2, 1);

            // Assert

            Assert.Null(result);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(u => u.GetUserById(It.IsAny<int>()), Times.Exactly(2));

            chatGroupRepoMock.Verify(c => c.GetChatGroupById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task TransferOwnershipOfChatGroup_CurrentOwnerNotOwner_ReturnNull()
        {
            // Arrange
            var uowMock = new Mock<IMySqlUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<TransferOwnershipOfChatGroupUseCase>>();

            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);
            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeCurrentOwner = new User
            {
                UserId = 1,
                Name = "CurrentOwner"
            };

            var fakeNewOwner = new User
            {
                UserId = 2,
                Name = "NewOwner"
            };

            var fakeChatGroup = new ChatGroup
            {
                Id = 1,
                Name = "TestGroup",
                OwnerId = 3
            };

            userRepoMock.Setup(u => u.GetUserById(1))
                .ReturnsAsync(fakeCurrentOwner);

            userRepoMock.Setup(u => u.GetUserById(2))
                .ReturnsAsync(fakeNewOwner);

            chatGroupRepoMock.Setup(c => c.GetChatGroupById(It.IsAny<int>()))
                .ReturnsAsync(fakeChatGroup);

            var sut = new TransferOwnershipOfChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(fakeCurrentOwner.UserId, fakeNewOwner.UserId, fakeChatGroup.Id);

            // Assert

            Assert.Null(result);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(u => u.GetUserById(It.IsAny<int>()), Times.Exactly(2));

            chatGroupRepoMock.Verify(c => c.GetChatGroupById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task TransferOwnershipOfChatGroup_ExceptionThrown_RollbackTransaction()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<TransferOwnershipOfChatGroupUseCase>>();

            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);
            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            userRepoMock.Setup(u => u.GetUserById(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Database error"));

            var sut = new TransferOwnershipOfChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act & Assert

            await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(1, 2, 1));

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Once);

            userRepoMock.Verify(u => u.GetUserById(It.IsAny<int>()), Times.Once);

            chatGroupRepoMock.Verify(c => c.GetChatGroupById(It.IsAny<int>()), Times.Never);
        }


    }
}
