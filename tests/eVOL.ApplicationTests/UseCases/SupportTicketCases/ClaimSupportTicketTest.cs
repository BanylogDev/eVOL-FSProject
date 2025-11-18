using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using eVOL.Domain.RepositoriesInteraces;
using Microsoft.Extensions.Logging;
using eVOL.Application.UseCases.SupportTicketCases;
using eVOL.Domain.Entities;
using eVOL.Application.DTOs.Requests;


namespace eVOL.ApplicationTests.UseCases.SupportTicketCases
{
    public class ClaimSupportTicketTest
    {

        [Fact]
        public async Task ClaimSupportTicket_ClaimSupportTicketSuccessfully_ReturnUser()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var supportTicketRepoMock = new Mock<ISupportTicketRepository>();
            var loggerMock = new Mock<ILogger<ClaimSupportTicketUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.SupportTicket).Returns(supportTicketRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeUser = new User
            {
                UserId = 1,
            };

            var fakeSupportTicket = new SupportTicket
            {
                Id = 1,
                ClaimedStatus = false,
                ClaimedById = 0,
                OpenedById = 1,
                OpenedBy = fakeUser,
            };

            userRepoMock.Setup(r => r.GetUserById(It.IsAny<int>())).ReturnsAsync(fakeUser);

            supportTicketRepoMock.Setup(r => r.GetSupportTicketById(It.IsAny<int>())).ReturnsAsync(fakeSupportTicket);

            var sut = new ClaimSupportTicketUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(new ClaimSupportTicketDTO
            {
                Id = 1,
                OpenedBy = 1
            });

            // Assert

            Assert.NotNull(result);
            Assert.Equal(fakeUser.UserId, result.UserId);
            Assert.True(fakeSupportTicket.ClaimedStatus);
            Assert.Equal(fakeUser.UserId, fakeSupportTicket.ClaimedById);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Once);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(r => r.GetUserById(It.IsAny<int>()), Times.Once);

            supportTicketRepoMock.Verify(r => r.GetSupportTicketById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task ClaimSupportTicket_UserOrSupportTicketNull_ReturnNull()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var supportTicketRepoMock = new Mock<ISupportTicketRepository>();
            var loggerMock = new Mock<ILogger<ClaimSupportTicketUseCase>>();    

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.SupportTicket).Returns(supportTicketRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);  
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            userRepoMock.Setup(r => r.GetUserById(It.IsAny<int>())).ReturnsAsync((User?)null);
            supportTicketRepoMock.Setup(r => r.GetSupportTicketById(It.IsAny<int>())).ReturnsAsync((SupportTicket?)null);

            var sut = new ClaimSupportTicketUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(new ClaimSupportTicketDTO
            {
                Id = 1,
                OpenedBy = 1
            });

            // Assert

            Assert.Null(result);
            
            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(r => r.GetUserById(It.IsAny<int>()), Times.Once);

            supportTicketRepoMock.Verify(r => r.GetSupportTicketById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task ClaimSupportTicket_AlreadyClaimed_ReturnNull()
        {
            // Arrange
            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var supportTicketRepoMock = new Mock<ISupportTicketRepository>();
            var loggerMock = new Mock<ILogger<ClaimSupportTicketUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.SupportTicket).Returns(supportTicketRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeUser = new User
            {
                UserId = 1,
            };

            var fakeSupportTicket = new SupportTicket
            {
                Id = 1,
                ClaimedStatus = true,
                ClaimedById = 2,
                OpenedById = 1,
                OpenedBy = fakeUser,
            };

            userRepoMock.Setup(r => r.GetUserById(It.IsAny<int>())).ReturnsAsync(fakeUser);

            supportTicketRepoMock.Setup(r => r.GetSupportTicketById(It.IsAny<int>())).ReturnsAsync(fakeSupportTicket);

            var sut = new ClaimSupportTicketUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(new ClaimSupportTicketDTO
            {
                Id = 1,
                OpenedBy = 1
            });

            // Assert

            Assert.Null(result);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            userRepoMock.Verify(r => r.GetUserById(It.IsAny<int>()), Times.Once);

            supportTicketRepoMock.Verify(r => r.GetSupportTicketById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task ClaimSupportTicket_ThrowException_ReturnNothing()
        {
            // Arrange
            var uowMock = new Mock<IMySqlUnitOfWork>();
            var userRepoMock = new Mock<IUserRepository>();
            var supportTicketRepoMock = new Mock<ISupportTicketRepository>();
            var loggerMock = new Mock<ILogger<ClaimSupportTicketUseCase>>();

            uowMock.Setup(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.SupportTicket).Returns(supportTicketRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            userRepoMock.Setup(r => r.GetUserById(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            var sut = new ClaimSupportTicketUseCase(uowMock.Object, loggerMock.Object);

            // Act & Assert

            await Assert.ThrowsAsync<Exception>(async () => await sut.ExecuteAsync(new ClaimSupportTicketDTO
            {
                Id = 1,
                OpenedBy = 1
            }));

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Once);

            userRepoMock.Verify(r => r.GetUserById(It.IsAny<int>()), Times.Once);

            supportTicketRepoMock.Verify(r => r.GetSupportTicketById(It.IsAny<int>()), Times.Never);
        }


    }
}
