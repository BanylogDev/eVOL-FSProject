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
    public class CreateSupportTicketTest
    {

        [Fact]
        public async Task CreateSupportTicket_CreateSupportTicketSuccessfully_ReturnSupportTicket()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var supportTicketRepoMock = new Mock<ISupportTicketRepository>();
            var loggerMock = new Mock<ILogger<CreateSupportTicketUseCase>>();

            uowMock.Setup(u => u.SupportTicket).Returns(supportTicketRepoMock.Object);

            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            var fakeUser = new User
            {
                UserId = 1,
            };

            uowMock.Setup(u => u.Users.GetUserById(It.IsAny<int>())).ReturnsAsync(fakeUser);

            supportTicketRepoMock.Setup(r => r.CreateSupportTicket(It.IsAny<SupportTicket>()));

            var sut = new CreateSupportTicketUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(new SupportTicketDTO
            {
                Category = "Technical",
                Text = "I need help with my account.",
                OpenedBy = 1
            });

            // Assert

            Assert.NotNull(result);
            
            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Once);
            uowMock.Verify(u => u.RollbackAsync(), Times.Never);

            supportTicketRepoMock.Verify(r => r.CreateSupportTicket(It.IsAny<SupportTicket>()), Times.Once);
        }

        [Fact]
        public async Task CreateSupportTicket_ThrowException_ReturnNothing()
        {
            // Arrange
            var uowMock = new Mock<IMySqlUnitOfWork>();
            var loggerMock = new Mock<ILogger<CreateSupportTicketUseCase>>();


            uowMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
            uowMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

            uowMock.Setup(u => u.Users.GetUserById(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            var sut = new CreateSupportTicketUseCase(uowMock.Object, loggerMock.Object);

            // Act & Assert 

            await Assert.ThrowsAsync<Exception>(async () => 
                await sut.ExecuteAsync(new SupportTicketDTO
                {
                    Category = "Test",
                    Text = "Test Message",
                    OpenedBy = 1
                })
            );

            uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            uowMock.Verify(u => u.CommitAsync(), Times.Never);
            uowMock.Verify(u => u.RollbackAsync(), Times.Once);

            uowMock.Verify(u => u.Users.GetUserById(It.IsAny<int>()), Times.Once);

        }

    }
}
