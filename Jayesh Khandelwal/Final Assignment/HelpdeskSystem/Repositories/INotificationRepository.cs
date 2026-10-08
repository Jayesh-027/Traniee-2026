using HelpdeskSystem.Models;

namespace HelpdeskSystem.Repositories
{
    public interface INotificationRepository
    {
        Task<bool> AddAsync(Notification notification);
        Task<List<Notification>> GetUnreadAsync(string userId, int companyId);

        Task<bool> MarkAsReadAsync( int id,string userId,int companyId);
        Task<List<Notification>> GetAllAsync(string userId);
    }
}
