using eVOL.Application.DTOs;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.SupportTicketCases
{
    public class CreateSupportTicketUseCase
    {
        private readonly ISupportTicketRepository _supportTicketRepo;

        public CreateSupportTicketUseCase(ISupportTicketRepository supportTicketRepo)
        {
            _supportTicketRepo = supportTicketRepo;
        }

        public async Task<SupportTicket> ExecuteAsync(SupportTicketDTO dto)
        {
            var newSupportTicket = new SupportTicket()
            {
                Category = dto.Category,
                Text = dto.Text,
                OpenedBy = dto.OpenedBy,
                ClaimedBy = 0,
                CreatedAt = DateTime.UtcNow

            };

            return await _supportTicketRepo.CreateSupportTicket(newSupportTicket);
        }
    }
}
