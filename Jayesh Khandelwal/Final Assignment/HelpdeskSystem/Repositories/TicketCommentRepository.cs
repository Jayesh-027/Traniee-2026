using HelpdeskSystem.Data;
using HelpdeskSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskSystem.Repositories
{
    public class TicketCommentRepository : ITicketCommentRepository
    {
        private readonly ApplicationDbContext _context;
        public TicketCommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddAsync(TicketComment comment)
        {
            _context.TicketComments.Add(comment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<TicketComment>> GetByTicketIdAsync(int ticketId)
        {
            return await _context.TicketComments
                .Include(c => c.User)
                .Where(c => c.TicketId == ticketId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}