using HelpdeskSystem.Models;

namespace HelpdeskSystem.Repositories
{
    public interface ITicketRepository
    {
        Task<List<Ticket>> GetAllAsync(string? status,string? priority,string? assignee,bool overdue, string? search,string? sort,int page,int pageSize);

        Task<List<Ticket>> GetByUserIdAsync(string userId);
        Task<bool> CreateAsync(Ticket ticket);
        Task<Ticket?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Ticket ticket);
        Task<bool> DeleteAsync(Ticket ticket);
    }
}