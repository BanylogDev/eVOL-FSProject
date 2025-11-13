using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using eVOL.Domain.RepositoriesInteraces;
using eVOL.Application.ServicesInterfaces;
using Microsoft.Extensions.Logging;
using eVOL.Domain.Entities;
using eVOL.Application.UseCases.UserCases;
using eVOL.Application.DTOs.Requests;


namespace eVOL.ApplicationTests.UseCases.UserCases
{
    public class UpdateUserTest
    {
        [Fact]
        public async Task UpdateUser_UpdateUserWithNewData_ReturnsUser()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var loggerMock = new Mock<ILogger<UpdateUserUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeUser = new User
            {
                UserId = 1,
                Name = "OldName",
                Email = "OldEmail",
                Password = "HashedNewPassword",
            };

            userRepoMock.Setup(u => u.GetUserById(1)).ReturnsAsync(fakeUser);

            passwordHasherMock.Setup(p => p.HashPassword("OldPassword")).Returns("HashedNewPassword");

            var sut = new UpdateUserUseCase(
                uowMock.Object,
                passwordHasherMock.Object,
                loggerMock.Object
            );

            // Act

            var result = await sut.ExecuteAsync(new UpdateDTO
            {
                Id = 1,
                Name = "NewName",
                Email = "NewEmail",
                Password = "OldPassword",
                ConfirmPassword = "OldPassword",
            });

            // Assert

            Assert.NotNull(result);
            Assert.Equal(fakeUser.Name, result!.Name);
            Assert.Equal(fakeUser.Email, result.Email);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Once);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(u => u.GetUserById(1), Times.Once);

            passwordHasherMock.Verify(p => p.HashPassword("OldPassword"), Times.Exactly(2));
        }

        [Fact]

        public async Task UpdateUser_UpdateUserNull_ReturnsNull()
        {
            // Arrange
            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var loggerMock = new Mock<ILogger<UpdateUserUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);

            userRepoMock.Setup(u => u.GetUserById(1)).ReturnsAsync((User?)null);

            var sut = new UpdateUserUseCase(
                uowMock.Object,
                passwordHasherMock.Object,
                loggerMock.Object
            );

            // Act

            var result = await sut.ExecuteAsync(new UpdateDTO
            {
                Id = 1,
                Name = "NewName",
                Email = "NewEmail",
                Password = "OldPassword",
                ConfirmPassword = "OldPassword",
            });

            // Assert

            Assert.Null(result);
            
            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);

            userRepoMock.Verify(u => u.GetUserById(1), Times.Once);

        }

        [Fact]
        public async Task UpdateUser_ThrowException_ReturnNothing()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var loggerMock = new Mock<ILogger<UpdateUserUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            userRepoMock.Setup(u => u.GetUserById(1)).ThrowsAsync(new Exception("Database error"));

            var sut = new UpdateUserUseCase(
                uowMock.Object,
                passwordHasherMock.Object,
                loggerMock.Object
            );

            // Act & Assert

            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await sut.ExecuteAsync(new UpdateDTO
                {
                    Id = 1,
                    Name = "NewName",
                    Email = "NewEmail",
                    Password = "OldPassword",
                    ConfirmPassword = "OldPassword",
                });
            });

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.RollbackAsync(), Times.Once);

            userRepoMock.Verify(u => u.GetUserById(1), Times.Once);
        }
    }
}

