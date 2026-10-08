using HelpdeskSystem.Models;
using HelpdeskSystem.Helpers;
namespace HelpdeskSystem.Services
{
    public interface INotificationService
    {
        Task<Result<bool>> CreateAsync( string userId, int ticketId,  string message);

        Task<Result<List<Notification>>> GetUnreadAsync();

        Task<Result<bool>> MarkAsReadAsync(int id);
    }
}