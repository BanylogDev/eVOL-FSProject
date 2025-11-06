using eVOL.Application.UseCases.UCInterfaces.ISupportTicketCases;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
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

        public DeleteSupportTicketUseCase(IMySqlUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SupportTicket?> ExecuteAsync(int id)
        {

            await _uow.BeginTransactionAsync(); 

            try
            {
                var supportTicket = await _uow.SupportTicket.GetSupportTicketById(id);

                if (supportTicket == null)
                {
                    return null;
                }

                await _uow.SupportTicket.DeleteSupportTicket(supportTicket);
                await _uow.CommitAsync();

                return supportTicket;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }

        }
    }
}
