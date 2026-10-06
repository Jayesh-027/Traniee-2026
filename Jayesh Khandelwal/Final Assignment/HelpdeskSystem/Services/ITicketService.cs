using HelpdeskSystem.Helpers;
using HelpdeskSystem.Models;
using HelpdeskSystem.ViewModels;

namespace HelpdeskSystem.Services
{
    public interface ITicketService
    {
        Task<List<Ticket>> GetTicketsAsync(string? status,string? priority, string? assignee,bool overdue,string? search,string? sort,int page,int pageSize);
        Task<List<ApplicationUser>> GetAgentsAsync();
        Task<bool> CreateTicketAsync(CreateTicketViewModel model);
        Task<Result<Ticket>> GetTicketByIdAsync(int id);
        Task<Result<bool>> ChangeStatusAsync(int id, TicketStatus newStatus);
        Task<Result<bool>> AssignTicketAsync(int id, string agentId);
        Task<Result<bool>> AddCommentAsync(int ticketId, string comment);
        Task<List<TicketComment>> GetCommentsAsync(int ticketId);
        Task<List<TicketHistory>> GetHistoryAsync(int ticketId);
        Task<Result<bool>> DeleteTicketAsync(int id);
        Task<DashboardViewModel> DashboardDataAsync();
    }
}