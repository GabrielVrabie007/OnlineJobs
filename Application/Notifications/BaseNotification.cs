using OnlineJobs.Application.Interfaces;

namespace OnlineJobs.Application.Notifications
{

    public class BaseNotification : INotification
    {
        public virtual async Task SendAsync(string recipient, string subject, string message)
        {
            await Task.CompletedTask;
        }

        public virtual string GetDescription()
        {
            return "Base Notification";
        }
    }
}
