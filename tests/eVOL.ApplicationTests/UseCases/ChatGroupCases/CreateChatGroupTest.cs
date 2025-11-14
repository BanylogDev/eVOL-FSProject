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
using eVOL.Application.DTOs.Requests;


namespace eVOL.ApplicationTests.UseCases.ChatGroupCases
{
    public class CreateChatGroupTest
    {

        [Fact]
        public async Task CreateChatGroup_CreateNewChatGroup_ReturnsChatGroup()
        {
            // Arrange
            
            var uowMock = new Mock<IMySqlUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var loggerMock = new Mock<ILogger<CreateChatGroupUseCase>>();

            uowMock.Setup(uow => uow.ChatGroup).Returns(chatGroupRepoMock.Object);

            uowMock.Setup(uow => uow.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(uow => uow.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeChatGroup = new ChatGroup
            {
                Id = 1,
                Name = "ChatGroup",
            };

            chatGroupRepoMock.Setup(g => g.CreateChatGroup(fakeChatGroup));

            var sut = new CreateChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(new ChatGroupDTO
            {
                Name = "ChatGroup",
                OwnerId = 2,
                TotalUsers = 1,
                GroupUsers = new List<User>()
            });

            // Assert

            Assert.NotNull(result);
            Assert.Equal(fakeChatGroup.Name, result.Name);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Once);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            chatGroupRepoMock.Verify(c => c.CreateChatGroup(It.IsAny<ChatGroup>()), Times.Once);
        }

        [Fact]
        public async Task CreateChatGroup_ThrowsException_ReturnsNothing()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var loggerMock = new Mock<ILogger<CreateChatGroupUseCase>>();

            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            chatGroupRepoMock.Setup(c => c.CreateChatGroup(It.IsAny<ChatGroup>())).ThrowsAsync(new Exception("Database error"));

            var sut = new CreateChatGroupUseCase(uowMock.Object, loggerMock.Object);

            // Act & Assert

            await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(new ChatGroupDTO { }));

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Once);

            chatGroupRepoMock.Verify(c => c.CreateChatGroup(It.IsAny<ChatGroup>()), Times.Once);
        }

    }
}
