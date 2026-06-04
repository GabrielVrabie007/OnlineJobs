using Microsoft.AspNetCore.Mvc;
using OnlineJobs.Application.Notifications;

namespace OnlineJobs.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly NotificationStore _store;

        public NotificationsController(NotificationStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public IActionResult Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var items = _store.GetForUser(userId.Value);
            _store.MarkAllRead(userId.Value); // opening the inbox clears the unread badge
            return View(items);
        }

        private Guid? GetCurrentUserId()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            return Guid.TryParse(userIdString, out var userId) ? userId : null;
        }
    }
}
