using HelpdeskSystem.Helpers;
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

        public async Task <Result<bool>> CreateAsync( string userId,int ticketId, string message)
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

            var result = await _notificationRepository.AddAsync(notification);

            if (!result)
            {
                return Result<bool>.Failure("Unable to create notification");
            }

            return Result<bool>.Success(true);
        }

        public async Task<Result<List<Notification>>> GetUnreadAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Result<List<Notification>>.Failure("User not found");
            }


            var companyId = _currentUserService.GetCompanyId();

            var notifications = await _notificationRepository.GetUnreadAsync(userId, companyId);

            return Result<List<Notification>>.Success(notifications);
        }

        public async Task<Result<bool>> MarkAsReadAsync(int id)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Result<bool>.Failure("User not found");
            }
            var companyId = _currentUserService.GetCompanyId();

            var result = await _notificationRepository.MarkAsReadAsync(id, userId, companyId);
            if (!result)
            {
                return Result<bool>.Failure("Unable to mark");
            }
            return Result<bool>.Success(true);
        }
    }
}