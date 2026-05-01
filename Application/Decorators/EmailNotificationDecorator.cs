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
            // TODO: Integrate with actual email service (SendGrid, SMTP, etc.)
            await Task.CompletedTask;
        }

        public override string GetDescription()
        {
            return base.GetDescription() + " + Email";
        }
    }
}
