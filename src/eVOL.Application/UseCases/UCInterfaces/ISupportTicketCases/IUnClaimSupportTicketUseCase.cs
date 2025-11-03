using eVOL.Application.DTOs;
using eVOL.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Application.UseCases.UCInterfaces.ISupportTicketCases
{
    public interface IUnClaimSupportTicketUseCase
    {
        Task<User?> ExecuteAsync(ClaimSupportTicketDTO dto);
    }
}
