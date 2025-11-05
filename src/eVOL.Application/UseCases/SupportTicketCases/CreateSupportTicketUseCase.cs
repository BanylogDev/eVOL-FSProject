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
        private readonly IMySqlUnitOfWork _uow;

        public CreateSupportTicketUseCase(IMySqlUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SupportTicket> ExecuteAsync(SupportTicketDTO dto)
        {

            await _uow.BeginTransactionAsync();

            try
            {
                var newSupportTicket = new SupportTicket()
                {
                    Category = dto.Category,
                    Text = dto.Text,
                    OpenedBy = dto.OpenedBy,
                    ClaimedBy = 0,
                    CreatedAt = DateTime.UtcNow

                };

                await _uow.SupportTicket.CreateSupportTicket(newSupportTicket);
                await _uow.CommitAsync();

                return newSupportTicket;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }

        }
    }
}
