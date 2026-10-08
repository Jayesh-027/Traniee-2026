using HelpdeskSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskSystem.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("/api/notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var notifications = await _notificationService.GetUnreadAsync();

            if (notifications.IsFailure)
            {
                return BadRequest(notifications.ErrorMessage);
            }

            var result = notifications.Value.Select(n => new
            {
                id = n.Id,
                message = n.Message,
                ticketId = n.TicketId,
                createdAt = n.CreatedAt.ToString("dd MMM yyyy, hh:mm tt")
            });

            return Json(result);
        }

        [HttpPost("/api/notifications/{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _notificationService.MarkAsReadAsync(id);

            if (result.IsFailure)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(new
            {
                success = true
            });
        }
    }
}