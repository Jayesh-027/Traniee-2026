using HelpdeskSystem.Models;

namespace HelpdeskSystem.Repositories
{
    public interface ITicketCommentRepository
    {
        Task<bool> AddAsync(TicketComment comment);
        Task<List<TicketComment>> GetByTicketIdAsync(int ticketId);
    }
}