using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.SupportTicketCases
{
    public class DeleteSupportTicketUseCase
    {
        private readonly ISupportTicketRepository _supportTicketRepo;

        public DeleteSupportTicketUseCase(ISupportTicketRepository supportTicketRepo)
        {
            _supportTicketRepo = supportTicketRepo;
        }

        public async Task<SupportTicket?> ExecuteAsync(int id)
        {
            var supportTicket = await _supportTicketRepo.GetSupportTicketById(id);

            if (supportTicket == null)
            {
                return null;
            }

            return await _supportTicketRepo.DeleteSupportTicket(supportTicket);
        }
    }
}
