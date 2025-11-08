using eVOL.Application.DTOs;
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
    public class UnClaimSupportTicketUseCase : IUnClaimSupportTicketUseCase
    {
        private readonly IMySqlUnitOfWork _uow;
        private readonly ILogger<UnClaimSupportTicketUseCase> _logger;

        public UnClaimSupportTicketUseCase(IMySqlUnitOfWork uow, ILogger<UnClaimSupportTicketUseCase> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<User?> ExecuteAsync(ClaimSupportTicketDTO dto)
        {

            _logger.LogInformation("Starting UnClaimSupportTicketUseCase for SupportTicket ID: {SupportTicketId} by User ID: {UserId}", dto.Id, dto.OpenedBy);

            await _uow.BeginTransactionAsync(); 

            try
            {
                var user = await _uow.Users.GetUserById(dto.OpenedBy);

                var supportTicket = await _uow.SupportTicket.GetSupportTicketById(dto.Id);

                if (user == null || supportTicket == null || supportTicket.ClaimedStatus == false)
                {
                    _logger.LogWarning("UnClaimSupportTicketUseCase failed: User or SupportTicket not found, or SupportTicket is not claimed.");
                    return null;
                }

                _logger.LogInformation("Unclaiming SupportTicket ID: {SupportTicketId} by User ID: {UserId}", dto.Id, dto.OpenedBy);

                supportTicket.ClaimedBy = 0;
                supportTicket.ClaimedStatus = false;

                _logger.LogInformation("SupportTicket ID: {SupportTicketId} successfully unclaimed by User ID: {UserId}", dto.Id, dto.OpenedBy);

                await _uow.CommitAsync();

                _logger.LogInformation("UnClaimSupportTicketUseCase completed successfully for SupportTicket ID: {SupportTicketId} by User ID: {UserId}", dto.Id, dto.OpenedBy);

                return user;
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "UnClaimSupportTicketUseCase failed and rolled back for SupportTicket ID: {SupportTicketId} by User ID: {UserId}", dto.Id, dto.OpenedBy);
                throw;
            }

        }
    }
}
