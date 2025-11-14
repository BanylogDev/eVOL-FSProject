using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using eVOL.Domain.RepositoriesInteraces;
using eVOL.Application.UseCases.SupportTicketCases;
using Microsoft.Extensions.Logging;
using eVOL.Domain.Entities;


namespace eVOL.ApplicationTests.UseCases.SupportTicketCases
{
    public class DeleteSupportTicketTest
    {

        [Fact]
        public async Task DeleteSupportTicket_DeleteSupportTicketSuccessfully_ReturnSupportTicket()
        {
            // Arrange
            var uowMock = new Mock<IMySqlUnitOfWork>();
            var supportTicketRepoMock = new Mock<ISupportTicketRepository>();
            var loggerMock = new Mock<ILogger<DeleteSupportTicketUseCase>>();

            uowMock.Setup(u => u.SupportTicket).Returns(supportTicketRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeSupportTicket = new SupportTicket
            {
                Id = 1,
                Category = "Test",
                Text = "Test Message",
                OpenedBy = 1
            };

            supportTicketRepoMock.Setup(r => r.GetSupportTicketById(1)).ReturnsAsync(fakeSupportTicket);
            supportTicketRepoMock.Setup(r => r.DeleteSupportTicket(fakeSupportTicket));

            var sut = new DeleteSupportTicketUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(1);

            // Assert

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Once);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            supportTicketRepoMock.Verify(r => r.GetSupportTicketById(1), Times.Once);
            supportTicketRepoMock.Verify(r => r.DeleteSupportTicket(fakeSupportTicket), Times.Once);

        }

        [Fact]
        public async Task DeleteSupportTicket_SupportTicketNull_ReturnNull()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var supportTicketRepoMock = new Mock<ISupportTicketRepository>();
            var loggerMock = new Mock<ILogger<DeleteSupportTicketUseCase>>();

            uowMock.Setup(u => u.SupportTicket).Returns(supportTicketRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            supportTicketRepoMock.Setup(r => r.GetSupportTicketById(1)).ReturnsAsync((SupportTicket?)null);

            var sut = new DeleteSupportTicketUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(1);

            // Assert

            Assert.Null(result);

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            supportTicketRepoMock.Verify(r => r.GetSupportTicketById(1), Times.Once);
        }

        [Fact]
        public async Task DeleteSupportTicket_ThrowException_ReturnNothing()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var supportTicketRepoMock = new Mock<ISupportTicketRepository>();
            var loggerMock = new Mock<ILogger<DeleteSupportTicketUseCase>>();

            uowMock.Setup(u => u.SupportTicket).Returns(supportTicketRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            supportTicketRepoMock.Setup(s => s.GetSupportTicketById(1))
                .ThrowsAsync(new Exception("Database error"));

            var sut = new DeleteSupportTicketUseCase(uowMock.Object, loggerMock.Object);

            // Act & Assert

            await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(1));

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Once);

            supportTicketRepoMock.Verify(s => s.GetSupportTicketById(It.IsAny<int>()), Times.Once);

        }
    }
}
