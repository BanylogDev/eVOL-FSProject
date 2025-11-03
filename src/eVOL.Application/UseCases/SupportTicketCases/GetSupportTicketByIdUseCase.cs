using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.SupportTicketCases
{
    public class GetSupportTicketByIdUseCase
    {
        private readonly ISupportTicketRepository _supportTicketRepo;

        public GetSupportTicketByIdUseCase(ISupportTicketRepository supportTicketRepo)
        {
            _supportTicketRepo = supportTicketRepo;
        }

        public async Task<SupportTicket?> ExecuteAsync(int id)
        {
            return await _supportTicketRepo.GetSupportTicketById(id);
        }
    }
}
