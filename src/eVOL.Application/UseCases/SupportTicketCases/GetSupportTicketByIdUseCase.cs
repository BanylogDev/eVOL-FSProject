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
    public class GetSupportTicketByIdUseCase : IGetSupportTicketByIdUseCase
    {
        private readonly IMySqlUnitOfWork _uow;

        public GetSupportTicketByIdUseCase(IMySqlUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SupportTicket?> ExecuteAsync(int id)
        {
            return await _uow.SupportTicket.GetSupportTicketById(id);
        }
    }
}
