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

namespace eVOL.ApplicationTests.UseCases.SupportTicketCases
{
    public class GetSupportTicketByIdTest
    {
        [Fact]
        public async Task GetSupportTicketById_GetSupportTicketSuccessfully_ReturnSupportTicket()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var supportTicketRepoMock = new Mock<ISupportTicketRepository>();
            var loggerMock = new Mock<ILogger<GetSupportTicketByIdUseCase>>();

            uowMock.Setup(u => u.SupportTicket).Returns(supportTicketRepoMock.Object);

            var fakeSupportTicket = new SupportTicket
            {
                Id = 1,
            };

            supportTicketRepoMock.Setup(s => s.GetSupportTicketById(1)).ReturnsAsync(fakeSupportTicket);

            var sut = new GetSupportTicketByIdUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(1);

            // Assert

            Assert.NotNull(result);
            Assert.Equal(fakeSupportTicket.Id, result.Id);

            supportTicketRepoMock.Verify(s =>  s.GetSupportTicketById(1), Times.Once());
        }

        [Fact]
        public async Task GetSupportTicketById_SupportTicketNull_ReturnNull()
        {
            // Arrange

            var uowMock = new Mock<IMySqlUnitOfWork>();
            var supportTicketRepoMock = new Mock<ISupportTicketRepository>();
            var loggerMock = new Mock<ILogger<GetSupportTicketByIdUseCase>>();

            uowMock.Setup(u => u.SupportTicket).Returns(supportTicketRepoMock.Object);

            var fakeSupportTicket = new SupportTicket
            {
                Id = 1,
            };

            supportTicketRepoMock.Setup(s => s.GetSupportTicketById(1)).ReturnsAsync((SupportTicket?)null);

            var sut = new GetSupportTicketByIdUseCase(uowMock.Object, loggerMock.Object);

            // Act

            var result = await sut.ExecuteAsync(1);

            // Assert

            Assert.Null(result);

            supportTicketRepoMock.Verify(s => s.GetSupportTicketById(1), Times.Once());
        }
    }
}
