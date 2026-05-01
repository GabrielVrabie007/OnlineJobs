using OnlineJobs.Application.Interfaces;

namespace OnlineJobs.Application.Decorators
{
  
    public class PushNotificationDecorator : NotificationDecorator
    {
        public PushNotificationDecorator(INotification notification) : base(notification)
        {
        }

        public override async Task SendAsync(string recipient, string subject, string message)
        {
            await base.SendAsync(recipient, subject, message);
            await SendPushNotification(recipient, subject, message);
        }

        private async Task SendPushNotification(string recipient, string subject, string message)
        {
            await Task.CompletedTask;
        }

        public override string GetDescription()
        {
            return base.GetDescription() + " + Push";
        }
    }
}
