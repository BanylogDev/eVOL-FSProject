using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using eVOL.Domain.RepositoriesInteraces;
using Mapster;
using Microsoft.Extensions.Logging;
using eVOL.Application.UseCases.UserCases;
using eVOL.Application.ServicesInterfaces;
using eVOL.Domain.Entities;
using eVOL.Application.DTOs.Requests;
using eVOL.Application.DTOs.Responses.User;


namespace eVOL.ApplicationTests.UseCases.UserCases
{
    public class RegisterUserTest
    {

        [Fact]
        public async Task RegisterUser_RegisterUser_ReturnsUser()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var authRepoMock = new Mock<IAuthRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var loggerMock = new Mock<ILogger<RegisterUserUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.Auth).Returns(authRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeUser = new User
            {
                UserId = 1,
                Name = "username",
                Email = "email",
                Password = "hashedPassword",
            };

            var registerDTO = new RegisterDTO
            {
                Name = "username",
                Email = "email",
                Password = "password",
                Country = "country",
                City = "city",
                AddressName = "addressName",
                AddressNumber = "addressNumber",
                Balance = 100.0,
                Currency = "USD",
                PhoneNumber = "1234567890"
            };

            uowMock.Setup(u => u.Users.GetUserByName(It.IsAny<string>())).ReturnsAsync((User?)null);
            uowMock.Setup(u => u.Users.GetUserByEmail(It.IsAny<string>())).ReturnsAsync((User?)null);

            uowMock.Setup(u => u.Auth.Register(It.IsAny<User>())).ReturnsAsync(fakeUser);

            passwordHasherMock.Setup(p => p.HashPassword("password")).Returns("hashedPassword");

            var sut = new RegisterUserUseCase(
                uowMock.Object,
                passwordHasherMock.Object,
                loggerMock.Object
            );

            // Act

            var result = await sut.ExecuteAsync(registerDTO);

            // Assert

            Assert.NotNull(result);
            Assert.Equal(fakeUser.Name, result.Name);
            Assert.Equal(fakeUser.Email, result.Email);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Once);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            uowMock.Verify(u => u.Users.GetUserByName(It.IsAny<string>()), Times.Once);
            uowMock.Verify(u => u.Users.GetUserByEmail(It.IsAny<string>()), Times.Once);

            uowMock.Verify(u => u.Auth.Register(It.IsAny<User>()), Times.Once);

            passwordHasherMock.Verify(p => p.HashPassword("password"), Times.Once);

        }

        [Fact]
        public async Task RegisterUser_RegisterUserNull_ReturnsNull()
        {
            //Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var loggerMock = new Mock<ILogger<RegisterUserUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var registerDTO = new RegisterDTO
            {
                Name = "username",
                Email = "email",
                Password = "password",
                Country = "country",
                City = "city",
                AddressName = "addressName",
                AddressNumber = "addressNumber",
                Balance = 100.0,
                Currency = "USD",
                PhoneNumber = "1234567890"
            };

            uowMock.Setup(u => u.Users.GetUserByName(It.IsAny<string>())).ReturnsAsync(new User { });

            var sut = new RegisterUserUseCase(
                uowMock.Object,
                passwordHasherMock.Object,
                loggerMock.Object
            );

            // Act

            var result = await sut.ExecuteAsync(registerDTO);

            // Assert

            Assert.Null(result);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);

            uowMock.Verify(u => u.Users.GetUserByName(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RegisterUser_ThrownException_ReturnsNothing()
        {
            //Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var loggerMock = new Mock<ILogger<RegisterUserUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var registerDTO = new RegisterDTO
            {
                Name = "username",
                Email = "email",
                Password = "password",
                Country = "country",
                City = "city",
                AddressName = "addressName",
                AddressNumber = "addressNumber",
                Balance = 100.0,
                Currency = "USD",
                PhoneNumber = "1234567890"
            };

            uowMock.Setup(u => u.Users.GetUserByName(It.IsAny<string>())).ThrowsAsync(new Exception("Database error"));

            var sut = new RegisterUserUseCase(
                uowMock.Object,
                passwordHasherMock.Object,
                loggerMock.Object
            );

            // Act & Assert

            await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(registerDTO));

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Once);

            uowMock.Verify(u => u.Users.GetUserByName(It.IsAny<string>()), Times.Once);
        }
    }
}
