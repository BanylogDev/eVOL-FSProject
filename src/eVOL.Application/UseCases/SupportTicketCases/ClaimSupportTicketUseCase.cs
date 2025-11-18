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
    public class ClaimSupportTicketUseCase : IClaimSupportTicketUseCase
    {
        private readonly IMySqlUnitOfWork _uow;
        private readonly ILogger<ClaimSupportTicketUseCase> _logger;

        public ClaimSupportTicketUseCase(IMySqlUnitOfWork uow, ILogger<ClaimSupportTicketUseCase> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<User?> ExecuteAsync(ClaimSupportTicketDTO dto)
        {

            _logger.LogInformation("Starting ClaimSupportTicketUseCase for SupportTicket ID: {SupportTicketId} by User ID: {UserId}", dto.Id, dto.OpenedBy);

            await _uow.BeginTransactionAsync();

            try
            {
                var user = await _uow.Users.GetUserById(dto.OpenedBy);

                var supportTicket = await _uow.SupportTicket.GetSupportTicketById(dto.Id);

                if (user == null || supportTicket == null || supportTicket.ClaimedStatus == true)
                {
                    _logger.LogWarning("ClaimSupportTicketUseCase failed: User or SupportTicket not found, or SupportTicket already claimed.");
                    return null;
                }

                _logger.LogInformation("Claiming SupportTicket ID: {SupportTicketId} by User ID: {UserId}", dto.Id, dto.OpenedBy);
                supportTicket.ClaimedById = dto.OpenedBy;
                supportTicket.ClaimedBy = user;
                supportTicket.ClaimedStatus = true;
                _logger.LogInformation("SupportTicket ID: {SupportTicketId} successfully claimed by User ID: {UserId}", dto.Id, dto.OpenedBy);

                await _uow.CommitAsync();

                _logger.LogInformation("ClaimSupportTicketUseCase completed successfully for SupportTicket ID: {SupportTicketId} by User ID: {UserId}", dto.Id, dto.OpenedBy);

                return user;
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "ClaimSupportTicketUseCase failed and rolled back for SupportTicket ID: {SupportTicketId} by User ID: {UserId}", dto.Id, dto.OpenedBy);
                throw;
            }

        }
    }
}
