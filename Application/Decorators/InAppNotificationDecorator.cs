using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Notifications;

namespace OnlineJobs.Application.Decorators
{
    /// <summary>
    /// Notification channel that delivers to the user's in-app bell (NotificationStore).
    /// This is the channel the Decorator chain produces that the user actually sees —
    /// the other channels (Email/SMS/Push) are simulated external integrations.
    /// </summary>
    public class InAppNotificationDecorator : NotificationDecorator
    {
        private readonly NotificationStore _store;
        private readonly Guid _userId;
        private readonly string _icon;

        public InAppNotificationDecorator(INotification notification, NotificationStore store, Guid userId, string icon = "bi-bell")
            : base(notification)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _userId = userId;
            _icon = icon;
        }

        public override async Task SendAsync(string recipient, string subject, string message)
        {
            await base.SendAsync(recipient, subject, message);
            _store.Add(_userId, subject, message, _icon);
        }

        public override string GetDescription() => base.GetDescription() + " + In-App";
    }
}
