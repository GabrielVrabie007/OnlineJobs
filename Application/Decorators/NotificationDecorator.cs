using OnlineJobs.Application.Interfaces;

namespace OnlineJobs.Application.Decorators
{

    public abstract class NotificationDecorator : INotification
    {
        protected readonly INotification _wrappedNotification;

        protected NotificationDecorator(INotification notification)
        {
            _wrappedNotification = notification ?? throw new ArgumentNullException(nameof(notification));
        }

        public virtual async Task SendAsync(string recipient, string subject, string message)
        {
            await _wrappedNotification.SendAsync(recipient, subject, message);
        }

        public virtual string GetDescription()
        {
            return _wrappedNotification.GetDescription();
        }
    }
}
