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

        public IMessageRepository Message { get; }

        public MongoUnitOfWork(IClientSessionHandle session, IMessageRepository message )
        {
            _session = session;
            Message = message;
        }

        public void BeginTransactionAsync() =>
            _session.StartTransaction();

        public async Task CommitAsync() =>
            await _session.CommitTransactionAsync();

        public async Task RollbackAsync() =>
            await _session.AbortTransactionAsync();
    }

}
