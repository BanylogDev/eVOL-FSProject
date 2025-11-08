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
    public class GetSupportTicketByIdUseCase : IGetSupportTicketByIdUseCase
    {
        private readonly IMySqlUnitOfWork _uow;
        private readonly ILogger<GetSupportTicketByIdUseCase> _logger;

        public GetSupportTicketByIdUseCase(IMySqlUnitOfWork uow, ILogger<GetSupportTicketByIdUseCase> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<SupportTicket?> ExecuteAsync(int id)
        {
            var supportTicket = await _uow.SupportTicket.GetSupportTicketById(id);

            if (supportTicket == null)
            {
                _logger.LogWarning("GetSupportTicketByIdUseCase: SupportTicket with ID {SupportTicketId} not found.", id);
                return null;
            }

            _logger.LogInformation("GetSupportTicketByIdUseCase: Successfully retrieved SupportTicket with ID {SupportTicketId}.", id);

            return supportTicket;
        }
    }
}
