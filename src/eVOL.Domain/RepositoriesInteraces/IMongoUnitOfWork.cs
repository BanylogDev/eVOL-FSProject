using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Domain.RepositoriesInteraces
{
    public interface IMongoUnitOfWork
    {
        IMessageRepository Message { get; }

        void BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
