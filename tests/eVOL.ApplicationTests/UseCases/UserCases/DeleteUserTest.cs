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
using eVOL.Application.UseCases.UserCases;
using eVOL.Domain.Entities;
using eVOL.Application.DTOs.Requests;


namespace eVOL.ApplicationTests.UseCases.UserCases
{
    public class DeleteUserTest
    {
        [Fact]
        public async Task DeleteUser_DeleteUser_ReturnsUser()
        {
            //Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var loggerMock = new Mock<ILogger<DeleteUserUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeUser = new User
            {
                UserId = 1,
                Password = "hashedPassword"
            };

            var fakeDeleteAccountDTO = new DeleteAccountDTO
            {
                Id = 1,
                Password = "password",
                ConfirmPassword = "password"
            };

            userRepoMock.Setup(u => u.GetUserById(1)).ReturnsAsync(fakeUser);
            userRepoMock.Setup(u => u.RemoveUser(fakeUser));

            passwordHasherMock.Setup(p => p.VerifyPassword(fakeDeleteAccountDTO.Password, fakeUser.Password)).Returns(true);

            var sut = new DeleteUserUseCase(uowMock.Object, passwordHasherMock.Object, loggerMock.Object);

            //Act

            var result = sut.ExecuteAsync(fakeDeleteAccountDTO);

            //Assert

            Assert.NotNull(result);
            Assert.Equal(fakeUser.UserId, result.Id);
            Assert.Equal(fakeDeleteAccountDTO.Password, fakeDeleteAccountDTO.ConfirmPassword);


            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Once);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(u => u.GetUserById(1), Times.Once);
            userRepoMock.Verify(u => u.RemoveUser(fakeUser), Times.Once);

        }

        [Fact]
        public async Task DeleteUser_UserNotFound_ReturnsNull()
        {
            //Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var loggerMock = new Mock<ILogger<DeleteUserUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeDeleteAccountDTO = new DeleteAccountDTO
            {
                Id = 1,
                Password = "password",
                ConfirmPassword = "password"
            };

            userRepoMock.Setup(u => u.GetUserById(1)).ReturnsAsync((User?)null);

            var sut = new DeleteUserUseCase(uowMock.Object, passwordHasherMock.Object, loggerMock.Object);

            //Act

            var result = await  sut.ExecuteAsync(fakeDeleteAccountDTO);

            //Assert

            Assert.Null(result);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(u => u.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ExceptionTrhown_RollbackCalled()
        {
            //Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var loggerMock = new Mock<ILogger<DeleteUserUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeDeleteAccountDTO = new DeleteAccountDTO
            {
                Id = 1,
                Password = "password",
                ConfirmPassword = "password"
            };

            userRepoMock.Setup(u => u.GetUserById(1)).ThrowsAsync(new Exception("Database error"));

            var sut = new DeleteUserUseCase(uowMock.Object, passwordHasherMock.Object, loggerMock.Object);

            //Act & Assert

            await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(fakeDeleteAccountDTO));

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Once);

            userRepoMock.Verify(u => u.GetUserById(1), Times.Once);
        }

    }
}
