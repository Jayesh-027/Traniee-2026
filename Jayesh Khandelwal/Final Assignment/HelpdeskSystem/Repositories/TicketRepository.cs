using HelpdeskSystem.Data;
using HelpdeskSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskSystem.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ticket>> GetAllAsync( string? status,string? priority,string? assignee,bool overdue, string? search,string? sort,int page,int pageSize,string userId)
        {
            var tickets = _context.Tickets.AsQueryable();
            if (userId != null)
                tickets = tickets.Where(t => t.CreatedByUserId == userId);
            if (status != null)
                tickets = tickets.Where(t => t.Status.ToString() == status);
            if (priority != null)
                tickets = tickets.Where(t => t.Priority.ToString() == priority);
            if (assignee != null)
                tickets = tickets.Where(t => t.AssignedToUserId == assignee);
            if (overdue)
                tickets = tickets.Where(t =>
                    (!t.FirstRespondedAt.HasValue && t.FirstResponseDue < DateTime.Now) ||
                    (t.Status != TicketStatus.Resolved &&
                     t.Status != TicketStatus.Closed &&
                     t.ResolutionDue < DateTime.Now));
            if (search != null)
                tickets = tickets.Where(t =>
                    t.Title.Contains(search) ||
                    t.Description.Contains(search));
            if (sort == "oldest")
                tickets = tickets.OrderBy(t => t.CreatedAt);
            else
                tickets = tickets.OrderByDescending(t => t.CreatedAt);

            return await tickets
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(Ticket ticket)
        {
            _context.Tickets.Add(ticket);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Ticket?> GetByIdAsync(int id)
        {
            return await _context.Tickets.Include(t => t.AssignedToUser)
                    .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<bool> UpdateAsync(Ticket ticket)
        {
            try
            {
                _context.Tickets.Update(ticket);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }
        public async Task<bool> DeleteAsync(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);

                return await _context.SaveChangesAsync() > 0;
        }
    }
}