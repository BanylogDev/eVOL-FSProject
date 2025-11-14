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
    public class GetChatGroupByIdTest
    {
        [Fact]
        public async Task GetChatGroupById_GetChatGroupWithId_ReturnsChatGroup()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var loggerMock = new Mock<ILogger<GetChatGroupByIdUseCase>>();

            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);

            var fakeChatGroup = new ChatGroup
            {
                Id = 1,
                Name = "TestGroup",
                OwnerId = 2,
            };

            chatGroupRepoMock.Setup(c => c.GetChatGroupById(1)).ReturnsAsync(fakeChatGroup);

            var sut = new GetChatGroupByIdUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(1);

            // Assert

            Assert.NotNull(result);
            Assert.Equal(fakeChatGroup.Name, result.Name);

            chatGroupRepoMock.Verify(c => c.GetChatGroupById(1), Times.Once);
        }

        [Fact]
        public async Task GetChatGroupById_GetNullChatGroup_ReturnsNull()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var chatGroupRepoMock = new Mock<IChatGroupRepository>();
            var loggerMock = new Mock<ILogger<GetChatGroupByIdUseCase>>();

            uowMock.Setup(u => u.ChatGroup).Returns(chatGroupRepoMock.Object);

            chatGroupRepoMock.Setup(c => c.GetChatGroupById(1)).ReturnsAsync((ChatGroup?)null);

            var sut = new GetChatGroupByIdUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(1);

            // Assert

            Assert.Null(result);

            chatGroupRepoMock.Verify(c => c.GetChatGroupById(1), Times.Once);
        }
    }
}
