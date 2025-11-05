using eVOL.Domain.RepositoriesInteraces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVOL.Infrastructure.Data;

namespace eVOL.Infrastructure.Persistence
{
    public class MySqlUnitOfWork : IMySqlUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction? _transaction;

        public IUserRepository Users { get; }
        public IAuthRepository Auth { get; }
        public IAdminRepository Admin { get; }
        public IChatGroupRepository ChatGroup { get; }
        public ISupportTicketRepository SupportTicket { get; }

        public MySqlUnitOfWork(ApplicationDbContext dbContext, IUserRepository users, IAuthRepository auth, IAdminRepository admin, IChatGroupRepository chatGroup, ISupportTicketRepository supportTicket)
        {
            _dbContext = dbContext;
            Users = users;
            Auth = auth;
            Admin = admin;
            ChatGroup = chatGroup;
            SupportTicket = supportTicket;
        }

        public async Task BeginTransactionAsync() =>
            _transaction = await _dbContext.Database.BeginTransactionAsync();

        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
            if (_transaction != null)
                await _transaction.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }
    }

}
