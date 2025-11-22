using eVOL.Application.ServicesInterfaces;
using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using eVOL.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace eVOL.Infrastructure.Repositories
{
    public class SupportTicketRepository : ISupportTicketRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;
        private readonly ILogger<SupportTicketRepository> _logger;

        public SupportTicketRepository(ApplicationDbContext context, ICacheService cacheService, ILogger<SupportTicketRepository> logger)
        {
            _context = context;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<SupportTicket> CreateSupportTicket(SupportTicket supportTicket)
        {

            await _context.SupportTickets.AddAsync(supportTicket);

            return supportTicket;
        }

        public SupportTicket? DeleteSupportTicket(SupportTicket supportTicket)
        {

            _context.SupportTickets.Remove(supportTicket);

            return supportTicket;
        } 

        public async Task<SupportTicket?> GetSupportTicketById(int id)
        {

            var cacheKey = $"supportTicket:{id}";

            var cache = await _cacheService.GetAsync(cacheKey);

            if (cache != null )
            {
                _logger.LogInformation("Cache hit in support ticket repository by GetSupportTicketById method");
                return JsonSerializer.Deserialize<SupportTicket>(cache);
            }

            _logger.LogInformation("Cache miss in support ticket repository by GetSupportTicketByName method");

            var supportTicket = await _context.SupportTickets
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (supportTicket != null)
            {
                await _cacheService.SetAsync(
                    cacheKey,
                    JsonSerializer.Serialize(supportTicket),
                    TimeSpan.FromMinutes(2));
            }

            return supportTicket;
        }

        public async Task<SupportTicket?> GetSupportTicketByName(string name)
        {

            var cacheKey = $"supportTicket:{name}";

            var cache = await _cacheService.GetAsync(cacheKey);

            if (cache != null)
            {
                _logger.LogInformation("Cache hit in support ticket repository by GetSupportTicketByName method");
                return JsonSerializer.Deserialize<SupportTicket>(cache);
            }

            _logger.LogInformation("Cache miss in support ticket repository by GetSupportTicketByName method");

            var supportTicket = await _context.SupportTickets
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Name == name);

            if (supportTicket != null)
            {
                await _cacheService.SetAsync(
                    cacheKey,
                    JsonSerializer.Serialize(supportTicket),
                    TimeSpan.FromMinutes(2));
            }

            return supportTicket;
        }
    }
}
