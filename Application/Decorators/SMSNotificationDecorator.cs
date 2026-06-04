using OnlineJobs.Application.Interfaces;

namespace OnlineJobs.Application.Decorators
{

    public class SMSNotificationDecorator : NotificationDecorator
    {
        public SMSNotificationDecorator(INotification notification) : base(notification)
        {
        }

        public override async Task SendAsync(string recipient, string subject, string message)
        {
            await base.SendAsync(recipient, subject, message);
            await SendSMS(recipient, message);
        }

        private async Task SendSMS(string recipient, string message)
        {
            string smsMessage = message.Length > 160
                ? message.Substring(0, 157) + "..."
                : message;

            // Simulated external channel — swap for Twilio/AWS SNS in production.
            Console.WriteLine($"[Notify/SMS] -> {recipient}: {smsMessage}");
            await Task.CompletedTask;
        }

        public override string GetDescription()
        {
            return base.GetDescription() + " + SMS";
        }
    }
}
