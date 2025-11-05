using eVOL.Domain.RepositoriesInteraces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Infrastructure.Persistence
{
    public class MongoUnitOfWork : IMongoUnitOfWork
    {
        private readonly IClientSessionHandle _session;

        public MongoUnitOfWork(IClientSessionHandle session)
        {
            _session = session;
        }

        public void BeginTransactionAsync() =>
            _session.StartTransaction();

        public async Task CommitAsync() =>
            await _session.CommitTransactionAsync();

        public async Task RollbackAsync() =>
            await _session.AbortTransactionAsync();
    }

}
