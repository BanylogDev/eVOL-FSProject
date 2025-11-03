using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using eVOL.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;
        
        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserInfoAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
    }
}
