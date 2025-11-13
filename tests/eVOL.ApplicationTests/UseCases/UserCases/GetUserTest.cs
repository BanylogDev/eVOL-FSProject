using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using eVOL.Domain.RepositoriesInteraces;
using Microsoft.Extensions.Logging;
using eVOL.Application.UseCases.UserCases;
using eVOL.Domain.Entities;
using AutoMapper;
using eVOL.Application.DTOs.Responses.User;

namespace eVOL.ApplicationTests.UseCases.UserCases
{
    public class GetUserTest
    {
        [Fact]
        public async Task GetUser_GetUserById_ReturnMappedUserResponse()
        {
            //Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<GetUserUseCase>>();
            var mapperMock = new Mock<IMapper>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            var fakeUser = new User
            {
                UserId = 1,
            };

            userRepoMock.Setup(u => u.GetUserById(1)).ReturnsAsync(fakeUser);

            var mappedResponse = new GetUserResponse
            {
                UserId = fakeUser.UserId
            };

            mapperMock.Setup(m => m.Map<GetUserResponse>(fakeUser)).Returns(mappedResponse);

            var sut = new GetUserUseCase(uowMock.Object, loggerMock.Object, mapperMock.Object);

            //Act

            var result = await sut.ExecuteAsync(1);

            //Assert

            Assert.NotNull(result);
            Assert.Equal(fakeUser.UserId, result.UserId);

            userRepoMock.Verify(u => u.GetUserById(1), Times.Once);

        }

        [Fact]
        public async Task GetUser_GetNullUserById_ReturnMapperdUserResponseNull()
        {
            //Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<GetUserUseCase>>();
            var mapperMock = new Mock<IMapper>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            userRepoMock.Setup(u => u.GetUserById(1)).ReturnsAsync((User?)null);

            var sut = new GetUserUseCase(uowMock.Object, loggerMock.Object, mapperMock.Object);

            //Act

            var result = await sut.ExecuteAsync(1);

            //Assert

            Assert.Null(result);

            userRepoMock.Verify(u => u.GetUserById(1), Times.Once);
        }
    }
}
