using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Observers
{
    public class DashboardNotificationObserver : IObserver
    {
        private readonly List<string> _dashboardNotifications = new();

        public List<string> DashboardNotifications => _dashboardNotifications;

        public Task UpdateAsync(object data)
        {
            if (data is JobPosting job)
            {
                var notification = $"New job posted: {job.Title}";
                _dashboardNotifications.Add(notification);
                Console.WriteLine($"[Dashboard] {notification}");
            }
            else if (data is JobApplication application)
            {
                var notification = $"Application #{application.Id} status: {application.Status}";
                _dashboardNotifications.Add(notification);
                Console.WriteLine($"[Dashboard] {notification}");
            }

            return Task.CompletedTask;
        }
    }
}
