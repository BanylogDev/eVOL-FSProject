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
    public class DeleteChatGroupTest
    {

        [Fact]
        public async Task DeleteChatGroup_DeleteChatGroupSuccessfully_ReturnsChatGroup()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var loggerMock = new Mock<ILogger<DeleteChatGroupUseCase>>();

            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeChatGroup = new ChatGroup
            {
                Id = 1,
                Name = "TestGroup",
                OwnerId = 2,
            };

            chatGroupRepoMock.Setup(c => c.GetChatGroupById(1)).ReturnsAsync(fakeChatGroup);
            chatGroupRepoMock.Setup(c => c.DeleteChatGroup(fakeChatGroup));

            var sut = new DeleteChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(1, 2);

            // Assert

            Assert.NotNull(result);
            Assert.Equal(fakeChatGroup.Name, result.Name);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Once);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            chatGroupRepoMock.Verify(c => c.GetChatGroupById(1), Times.Once);
            chatGroupRepoMock.Verify(c => c.DeleteChatGroup(fakeChatGroup), Times.Once);
        }

        [Fact]
        public async Task DeleteChatGroup_ChatGroupNull_ReturnNull()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var loggerMock = new Mock<ILogger<DeleteChatGroupUseCase>>();

            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            chatGroupRepoMock.Setup(c => c.GetChatGroupById(1)).ReturnsAsync((ChatGroup?)null);

            var sut = new DeleteChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(1, 2);

            // Assert

            Assert.Null(result);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            chatGroupRepoMock.Verify(c => c.GetChatGroupById(1), Times.Once);
            chatGroupRepoMock.Verify(c => c.DeleteChatGroup(It.IsAny<ChatGroup>()), Times.Never);
        }

        [Fact]
        public async Task DeleteChatGroup_ThrowsException_ReturnsNothing()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var loggerMock = new Mock<ILogger<DeleteChatGroupUseCase>>();

            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            chatGroupRepoMock.Setup(c => c.GetChatGroupById(1)).ThrowsAsync(new Exception("Database error"));

            var sut = new DeleteChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act & Assert

            await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(1, 2));

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Once);

            chatGroupRepoMock.Verify(c => c.GetChatGroupById(1), Times.Once);
        }

    }
}
