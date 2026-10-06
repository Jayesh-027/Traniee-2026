using HelpdeskSystem.Data;
using HelpdeskSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskSystem.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;
       public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Notification>> GetUnreadAsync( string userId, int companyId)
        {
            return await _context.Notifications
                .Where(n =>
                    n.UserId == userId &&
                    n.CompanyId == companyId &&
                    !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> MarkAsReadAsync(int id, string userId,int companyId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.Id == id &&
                    n.UserId == userId &&
                    n.CompanyId == companyId);

            if (notification == null)
                return false;

            notification.IsRead = true;

            return await _context.SaveChangesAsync() > 0;
        }
    }
}