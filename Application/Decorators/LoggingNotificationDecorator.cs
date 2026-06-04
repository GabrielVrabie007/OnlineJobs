using OnlineJobs.Application.Interfaces;

namespace OnlineJobs.Application.Decorators
{

    public class LoggingNotificationDecorator : NotificationDecorator
    {
        // Instance state (was a shared static list, which raced across requests).
        private readonly List<string> _notificationLog = new List<string>();

        public LoggingNotificationDecorator(INotification notification) : base(notification)
        {
        }

        public override async Task SendAsync(string recipient, string subject, string message)
        {
            LogNotification(recipient, subject, message);
            await base.SendAsync(recipient, subject, message);
            LogCompletion(recipient);
        }

        private void LogNotification(string recipient, string subject, string message)
        {
            var logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Sending to: {recipient} | Subject: {subject}";
            _notificationLog.Add(logEntry);
            Console.WriteLine($"[Notify/Log] {logEntry}");
        }

        private void LogCompletion(string recipient)
        {
            var logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Completed notification to: {recipient}";
            _notificationLog.Add(logEntry);
        }

        public override string GetDescription()
        {
            return base.GetDescription() + " + Logging";
        }

        public IReadOnlyList<string> GetNotificationLog() => _notificationLog.ToList();
    }
}
