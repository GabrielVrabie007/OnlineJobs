using OnlineJobs.Application.Interfaces;

namespace OnlineJobs.Application.Decorators
{

    public class EmailNotificationDecorator : NotificationDecorator
    {
        public EmailNotificationDecorator(INotification notification) : base(notification)
        {
        }

        public override async Task SendAsync(string recipient, string subject, string message)
        {
            await base.SendAsync(recipient, subject, message);
            await SendEmail(recipient, subject, message);
        }

        private async Task SendEmail(string recipient, string subject, string message)
        {
            // Simulated external channel — swap for SendGrid/SMTP in production.
            Console.WriteLine($"[Notify/Email] -> {recipient} | {subject}");
            await Task.CompletedTask;
        }

        public override string GetDescription()
        {
            return base.GetDescription() + " + Email";
        }
    }
}
