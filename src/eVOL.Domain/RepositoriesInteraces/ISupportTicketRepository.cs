using eVOL.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Domain.RepositoriesInteraces
{
    public interface ISupportTicketRepository
    {
        Task<SupportTicket> CreateSupportTicket(SupportTicket supportTicket);
        SupportTicket? DeleteSupportTicket(SupportTicket supportTicket);
        Task<SupportTicket?> GetSupportTicketById(int id);
    }
}
