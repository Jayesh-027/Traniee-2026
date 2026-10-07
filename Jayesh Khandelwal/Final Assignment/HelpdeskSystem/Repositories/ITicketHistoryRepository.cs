using HelpdeskSystem.Models;

namespace HelpdeskSystem.Repositories
{
    public interface ITicketHistoryRepository
    {
        Task<bool> AddAsync(TicketHistory history);
        Task<List<TicketHistory>> GetByTicketIdAsync(int ticketId);
        
    }
}
