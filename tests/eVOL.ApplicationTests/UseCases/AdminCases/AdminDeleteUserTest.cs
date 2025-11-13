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
using eVOL.Application.UseCases.AdminCases;
using eVOL.Domain.Entities;


namespace eVOL.ApplicationTests.UseCases.AdminCases
{
    public class AdminDeleteUserTest
    {
        [Fact]
        public async Task AdminDeleteUser_DeleteUser_ReturnsUser()
        {
            // Arrange
            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var loggerMock = new Mock<ILogger<AdminDeleteUserUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);


            var fakeUser = new User
            {
                UserId = 1
            };

            userRepoMock.Setup(u => u.GetUserById(1)).ReturnsAsync(fakeUser);

            userRepoMock.Setup(u => u.RemoveUser(fakeUser));

            var sut = new AdminDeleteUserUseCase(uowMock.Object, passwordHasherMock.Object, loggerMock.Object);


            // Act

            var result = await sut.ExecuteAsync(1);

            // Assert

            Assert.NotNull(result);
            Assert.Equal(fakeUser.UserId, result.UserId);

            userRepoMock.Verify(r => r.GetUserById(1), Times.Once);
            userRepoMock.Verify(r => r.RemoveUser(fakeUser), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Once);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task AdminDeleteUser_UserDoesNotExist_ReturnsNull()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var passwordHasherRepoMock = new Mock<IPasswordHasher>();
            var loggerMock = new Mock<ILogger<AdminDeleteUserUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            userRepoMock.Setup(u => u.GetUserById(1)).ReturnsAsync((User?)null);

            var sut = new AdminDeleteUserUseCase(uowMock.Object, passwordHasherRepoMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(1);

            // Assert

            Assert.Null(result);
            userRepoMock.Verify(u => u.RemoveUser(It.IsAny<User>()), Times.Never);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task AdminDeleteUser_ThrowsException_PerformsRollback()
        {
            // Arrange
            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var loggerMock = new Mock<ILogger<AdminDeleteUserUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            userRepoMock.Setup(r => r.GetUserById(1))
                        .ThrowsAsync(new Exception("DB error"));

            var sut = new AdminDeleteUserUseCase(uowMock.Object, passwordHasherMock.Object, loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(1));

            uowMock.Verify(u => u.RollbackAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
