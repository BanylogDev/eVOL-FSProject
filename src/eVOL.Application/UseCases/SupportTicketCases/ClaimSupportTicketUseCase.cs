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
        private readonly IMySqlUnitOfWork _uow;

        public ClaimSupportTicketUseCase(IMySqlUnitOfWork uow)
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

                if (user == null || supportTicket == null || supportTicket.ClaimedStatus == true)
                {
                    return null;
                }

                supportTicket.ClaimedBy = dto.OpenedBy;
                supportTicket.ClaimedStatus = true;

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
