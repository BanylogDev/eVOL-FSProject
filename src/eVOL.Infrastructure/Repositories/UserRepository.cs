using eVOL.Application.ServicesInterfaces;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using eVOL.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext context, ICacheService cacheService, ILogger<UserRepository> logger)
        {
            _context = context;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<User?> GetUserById(int id)
        {
            var cacheKey = $"users:{id}";

            var cached = await _cacheService.GetAsync(cacheKey);

            if (cached != null)
            {
                _logger.LogInformation("Cache hit in user repository by GetUserById method");
                return JsonSerializer.Deserialize<User>(cached);
            }

            _logger.LogInformation("Cache miss in user repository by GetUserById method");

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user != null)
            {
                await _cacheService.SetAsync(
                    cacheKey,
                    JsonSerializer.Serialize(user),
                    TimeSpan.FromMinutes(2));
            }

            return user;
        }

        public async Task<User?> GetUserByName(string name)
        {

            var cacheKey = $"users:{name}";

            var cached = await _cacheService.GetAsync(cacheKey);

            if (cached != null)
            {
                _logger.LogInformation("Cache hit in user repository by GetUserByName method");
                return JsonSerializer.Deserialize<User>(cached);
            }

            _logger.LogInformation("Cache miss in user repository by GetUserByName method");

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Name == name);

            if (user != null)
            {
                await _cacheService.SetAsync(
                    cacheKey,
                    JsonSerializer.Serialize(user),
                    TimeSpan.FromMinutes(2));
            }

            return user;
        }

        public async Task<User?> GetUserByEmail(string email)
        {

            var cacheKey = $"user:{email}";

            var cached = await _cacheService.GetAsync(cacheKey);

            if (cached != null)
            {
                _logger.LogInformation("Cache hit in user repository by GetUserByEmail method");
                return JsonSerializer.Deserialize<User>(cached);
            }

            _logger.LogInformation("Cache miss in user repository by GetUserByEmail method");

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                await _cacheService.SetAsync(
                    cacheKey,
                    JsonSerializer.Serialize(user),
                    TimeSpan.FromMinutes(2));
            }

            return user;
        }

        public void RemoveUser(User user)
        {
            _context.Remove(user);
        }


    }
}
