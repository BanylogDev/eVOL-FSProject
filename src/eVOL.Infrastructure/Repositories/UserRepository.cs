using eVOL.Application.ServicesInterfaces;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using eVOL.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace eVOL.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        public UserRepository(ApplicationDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<User?> GetUserById(int id)
        {
            var cacheKey = $"users:{id}:recent";

            var cached = await _cacheService.GetAsync(cacheKey);

            if (cached != null)
            {
                Console.WriteLine("Cache Hit");
                return JsonSerializer.Deserialize<User>(cached);
            }

            Console.WriteLine("Cache Miss");

            var user = await _context.Users.FindAsync(id);

            await _cacheService.SetAsync(cacheKey,
                JsonSerializer.Serialize(user),
                TimeSpan.FromMinutes(2));

            return user;
        }

        public async Task<User?> GetUserByName(string name)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Name == name);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void RemoveUser(User user)
        {
            _context.Remove(user);
        }


    }
}
