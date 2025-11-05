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
        private readonly IMySqlUnitOfWork _uow;

        public UnClaimSupportTicketUseCase(IMySqlUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<User?> ExecuteAsync(ClaimSupportTicketDTO dto)
        {

            await _uow.BeginTransactionAsync(); 

            try
            {
                var user = await _uow.Users.GetUserById(dto.OpenedBy);

                var supportTicket = await _uow.SupportTicket.GetSupportTicketById(dto.Id);

                if (user == null || supportTicket == null || supportTicket.ClaimedStatus == false)
                {
                    return null;
                }

                supportTicket.ClaimedBy = 0;
                supportTicket.ClaimedStatus = false;

                await _uow.CommitAsync();

                return user;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }

        }
    }
}
