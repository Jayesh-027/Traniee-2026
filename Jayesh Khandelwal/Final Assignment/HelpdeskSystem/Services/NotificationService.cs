using HelpdeskSystem.Models;
using HelpdeskSystem.Repositories;
using System.Security.Claims;

namespace HelpdeskSystem.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationService(
            INotificationRepository notificationRepository,
            ICurrentUserService currentUserService,
            IHttpContextAccessor httpContextAccessor)
        {
            _notificationRepository = notificationRepository;
            _currentUserService = currentUserService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CreateAsync( string userId,int ticketId, string message)
        {
            var companyId = _currentUserService.GetCompanyId();
            var notification = new Notification
            {
                CompanyId = companyId,
                UserId = userId,
                TicketId = ticketId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            return await _notificationRepository.AddAsync(notification);
        }

        public async Task<List<Notification>> GetUnreadAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return new List<Notification>();

            var companyId = _currentUserService.GetCompanyId();

            return await _notificationRepository .GetUnreadAsync(userId, companyId);
        }

        public async Task<bool> MarkAsReadAsync(int id)
        {
            var userId = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return false;

            var companyId = _currentUserService.GetCompanyId();

            return await _notificationRepository
                .MarkAsReadAsync(id, userId, companyId);
        }
    }
}