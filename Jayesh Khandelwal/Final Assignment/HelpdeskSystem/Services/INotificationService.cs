using HelpdeskSystem.Models;

namespace HelpdeskSystem.Services
{
    public interface INotificationService
    {
        Task<bool> CreateAsync( string userId, int ticketId,  string message);

        Task<List<Notification>> GetUnreadAsync();

        Task<bool> MarkAsReadAsync(int id);
    }
}