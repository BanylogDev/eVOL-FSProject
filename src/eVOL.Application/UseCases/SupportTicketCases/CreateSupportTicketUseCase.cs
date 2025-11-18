using eVOL.Application.DTOs.Requests;
using eVOL.Application.UseCases.UCInterfaces.ISupportTicketCases;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.SupportTicketCases
{
    public class CreateSupportTicketUseCase : ICreateSupportTicketUseCase
    {
        private readonly IMySqlUnitOfWork _uow;
        private readonly ILogger<CreateSupportTicketUseCase> _logger;

        public CreateSupportTicketUseCase(IMySqlUnitOfWork uow, ILogger<CreateSupportTicketUseCase> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<SupportTicket?> ExecuteAsync(SupportTicketDTO dto)
        {

            _logger.LogInformation("Starting CreateSupportTicketUseCase for User ID: {UserId}", dto.OpenedBy);

            await _uow.BeginTransactionAsync();

            try
            {

                var user = await _uow.Users.GetUserById(dto.OpenedBy);

                if (user == null)
                {
                    return null;
                }

                var newSupportTicket = new SupportTicket()
                {
                    Category = dto.Category,
                    Text = dto.Text,
                    OpenedById = dto.OpenedBy,
                    ClaimedById = 0,
                    OpenedBy = user,
                    CreatedAt = DateTime.UtcNow

                };

                _logger.LogInformation("Creating SupportTicket for User ID: {UserId}", dto.OpenedBy);

                await _uow.SupportTicket.CreateSupportTicket(newSupportTicket);
                await _uow.CommitAsync();

                _logger.LogInformation("CreateSupportTicketUseCase completed successfully for User ID: {UserId}", dto.OpenedBy);

                return newSupportTicket;
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "CreateSupportTicketUseCase failed and rolled back for User ID: {UserId}", dto.OpenedBy);
                throw;
            }

        }
    }
}
