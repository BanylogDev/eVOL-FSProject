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
    public class ClaimSupportTicketUseCase
    {
        private readonly ISupportTicketRepository _supportTicketRepo;
        private readonly IUserRepository _userRepo;

        public ClaimSupportTicketUseCase(ISupportTicketRepository supportTicketRepo, IUserRepository userRepo)
        {
            _supportTicketRepo = supportTicketRepo;
            _userRepo = userRepo;
        }

        public async Task<User?> ExecuteAsync(ClaimSupportTicketDTO dto)
        {
            var user = await _userRepo.GetUserById(dto.OpenedBy);

            var supportTicket = await _supportTicketRepo.GetSupportTicketById(dto.Id);

            if (user == null || supportTicket == null || supportTicket.ClaimedStatus == true)
            {
                return null;
            }

            supportTicket.ClaimedBy = dto.OpenedBy;
            supportTicket.ClaimedStatus = true;

            await _supportTicketRepo.SaveChangesAsync();

            return user;
        }
    }
}
