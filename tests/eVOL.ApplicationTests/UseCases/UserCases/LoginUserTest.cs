using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using eVOL.Domain.RepositoriesInteraces;
using eVOL.Application.ServicesInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using eVOL.Application.UseCases.UserCases;
using Mapster;
using eVOL.Domain.Entities;
using eVOL.Application.DTOs.Responses.User;
using eVOL.Application.DTOs;


namespace eVOL.ApplicationTests.UseCases.UserCases
{
    public class LoginUserTest
    {
        [Fact]
        public async Task LoginUser_LoginUserWithCredentials_ReturnsMappedUser()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var jwtServiceMock = new Mock<IJwtService>();
            var configMock = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger<LoginUserUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeUser = new User
            {
                UserId = 1,
                Email = "email",
                Name = "username",
                Password = "hashedPassword"
            };

            uowMock.Setup(u => u.Users.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(fakeUser);

            passwordHasherMock.Setup(p => p.VerifyPassword("password", "hashedPassword")).Returns(true);

            jwtServiceMock.Setup(j => j.GenerateJwtToken(fakeUser, It.IsAny<IConfiguration>())).Returns("accessToken");
            jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns("refresh");

            var sut = new LoginUserUseCase(
                uowMock.Object,
                passwordHasherMock.Object,
                jwtServiceMock.Object,
                configMock.Object,
                loggerMock.Object
            );

            // Act

            var result = await sut.ExecuteAsync(new LoginDTO
            {
                Email = "email",
                Password = "password"
            });

            // Assert

            Assert.NotNull(result);
            Assert.Equal(fakeUser.UserId, result.UserId);
            Assert.Equal("email", result.Email);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Once);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            uowMock.Verify(u => u.Users.GetUserByEmail(It.IsAny<string>()), Times.Once);

            passwordHasherMock.Verify(p => p.VerifyPassword("password", "hashedPassword"), Times.Once);

            jwtServiceMock.Verify(j => j.GenerateJwtToken(fakeUser, It.IsAny<IConfiguration>()), Times.Once);
            jwtServiceMock.Verify(j => j.GenerateRefreshToken(), Times.Once);

        }

        [Fact] 
        public async Task LoginUser_LoginUserNull_ReturnsNull()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var jwtServiceMock = new Mock<IJwtService>();
            var configMock = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger<LoginUserUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);

            uowMock.Setup(u => u.Users.GetUserByEmail(It.IsAny<string>())).ReturnsAsync((User?)null);

            var sut = new LoginUserUseCase(
                uowMock.Object,
                passwordHasherMock.Object,
                jwtServiceMock.Object,
                configMock.Object,
                loggerMock.Object
            );

            // Act

            var result = await sut.ExecuteAsync(new LoginDTO
            {
                Email = "email",
                Password = "password"
            });

            // Assert

            Assert.Null(result);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);

            uowMock.Verify(u => u.Users.GetUserByEmail(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task LoginUser_ThrowException_ReturnsException()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var jwtServiceMock = new Mock<IJwtService>();
            var configMock = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger<LoginUserUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            uowMock.Setup(u => u.Users.GetUserByEmail(It.IsAny<string>())).ThrowsAsync(new Exception("Database error"));

            var sut = new LoginUserUseCase(
                uowMock.Object,
                passwordHasherMock.Object,
                jwtServiceMock.Object,
                configMock.Object,
                loggerMock.Object
            );

            // Act & Assert

            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await sut.ExecuteAsync(new LoginDTO
                {
                    Email = "email",
                    Password = "password"
                });
            });

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Once);

            uowMock.Verify(u => u.Users.GetUserByEmail(It.IsAny<string>()), Times.Once);
        }
    }
}
