using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Domain.RepositoriesInteraces
{
    public interface IMySqlUnitOfWork
    {
        IUserRepository Users { get; }
        IAuthRepository Auth { get; }
        IAdminRepository Admin { get; }
        IChatGroupRepository ChatGroup { get; }
        ISupportTicketRepository SupportTicket { get; }

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
