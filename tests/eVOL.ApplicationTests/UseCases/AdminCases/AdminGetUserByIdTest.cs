using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using eVOL.Domain.RepositoriesInteraces;
using eVOL.Application.UseCases.AdminCases;
using eVOL.Domain.Entities;
using Microsoft.Extensions.Logging;


namespace eVOL.ApplicationTests.UseCases.AdminCases
{
    public class AdminGetUserByIdTest
    {
        [Fact]
        public async Task AdminGetUserById_GetUserById_ReturnUser()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<AdminGetUserUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            var fakeUser = new User
            {
                UserId = 1
            };

            userRepoMock.Setup(u => u.GetUserById(1)).ReturnsAsync(fakeUser);

            var sut = new AdminGetUserUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(1);

            // Assert

            Assert.NotNull(result);
            Assert.Equal(fakeUser.UserId, result.UserId);

            userRepoMock.Verify(u => u.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task AdminGetUserById_GetUserNullById_ReturnNull()
        {
            // Arrange
            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<AdminGetUserUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            userRepoMock.Setup(u => u.GetUserById(1)).ReturnsAsync((User?)null);

            var sut = new AdminGetUserUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(1);

            // Assert

            Assert.Null(result);

            userRepoMock.Verify(u => u.GetUserById(1), Times.Once);
        }
    }
}
