using eVOL.Domain.Entities;
using eVOL.Domain.RepositoriesInteraces;
using eVOL.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVOL.Infrastructure.Repositories
{
    public class SupportTicketRepository : ISupportTicketRepository
    {
        private readonly ApplicationDbContext _context;

        public SupportTicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SupportTicket> CreateSupportTicket(SupportTicket supportTicket)
        {

            await _context.SupportTickets.AddAsync(supportTicket);
            await _context.SaveChangesAsync();

            return supportTicket;
        }

        public async Task<SupportTicket?> DeleteSupportTicket(SupportTicket supportTicket)
        {

            _context.SupportTickets.Remove(supportTicket);
            await _context.SaveChangesAsync();

            return supportTicket;
        } 

        public async Task<SupportTicket?> GetSupportTicketById(int id)
        {
            return await _context.SupportTickets.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
