using HelpdeskSystem.Data;
using HelpdeskSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskSystem.Repositories
{
    public class TicketHistoryRepository : ITicketHistoryRepository
    {
        private readonly ApplicationDbContext _context;
        public TicketHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(TicketHistory history)
        {
            _context.TicketHistories.Add(history);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<List<TicketHistory>> GetByTicketIdAsync(int ticketId)
        {
            return await _context.TicketHistories
                .Include(h => h.User)
                .Where(h => h.TicketId == ticketId)
                .OrderByDescending(h => h.Timestamp)
                .ToListAsync();
        }
        
    }
}
