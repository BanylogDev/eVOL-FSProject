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
    public class UnClaimSupportTicketUseCase
    {
        private readonly ISupportTicketRepository _supportTicketRepo;
        private readonly IUserRepository _userRepo;

        public UnClaimSupportTicketUseCase(ISupportTicketRepository supportTicketRepo, IUserRepository userRepo)
        {
            _supportTicketRepo = supportTicketRepo;
            _userRepo = userRepo;
        }

        public async Task<User?> ExecuteAsync(ClaimSupportTicketDTO dto)
        {
            var user = await _userRepo.GetUserById(dto.OpenedBy);

            var supportTicket = await _supportTicketRepo.GetSupportTicketById(dto.Id);

            if (user == null || supportTicket == null || supportTicket.ClaimedStatus == false)
            {
                return null;
            }

            supportTicket.ClaimedBy = 0;
            supportTicket.ClaimedStatus = false;

            await _supportTicketRepo.SaveChangesAsync();

            return user;
        }
    }
}
