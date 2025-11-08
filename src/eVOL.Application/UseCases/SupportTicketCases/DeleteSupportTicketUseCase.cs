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
    public class DeleteSupportTicketUseCase : IDeleteSupportTicketUseCase
    {
        private readonly IMySqlUnitOfWork _uow;
        private readonly ILogger<DeleteSupportTicketUseCase> _logger;

        public DeleteSupportTicketUseCase(IMySqlUnitOfWork uow, ILogger<DeleteSupportTicketUseCase> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<SupportTicket?> ExecuteAsync(int id)
        {

            _logger.LogInformation("Starting DeleteSupportTicketUseCase for SupportTicket ID: {SupportTicketId}", id);

            await _uow.BeginTransactionAsync(); 

            try
            {
                var supportTicket = await _uow.SupportTicket.GetSupportTicketById(id);

                if (supportTicket == null)
                {
                    _logger.LogWarning("DeleteSupportTicketUseCase failed: SupportTicket not found.");
                    return null;
                }

                _logger.LogInformation("Deleting SupportTicket ID: {SupportTicketId}", id);

                _uow.SupportTicket.DeleteSupportTicket(supportTicket);
                await _uow.CommitAsync();

                _logger.LogInformation("DeleteSupportTicketUseCase completed successfully for SupportTicket ID: {SupportTicketId}", id);

                return supportTicket;
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.LogError(ex, "DeleteSupportTicketUseCase failed and rolled back for SupportTicket ID: {SupportTicketId}", id);
                throw;
            }

        }
    }
}
