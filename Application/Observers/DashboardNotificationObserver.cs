using OnlineJobs.Domain.Entities;
using OnlineJobs.Application.Notifications;

namespace OnlineJobs.Application.Observers
{
    /// <summary>
    /// Reacts to status changes by putting a real in-app notification in the affected
    /// job seeker's bell (NotificationStore). This is the user-visible side of the
    /// Observer pattern: when an employer approves/rejects, the seeker sees it.
    /// </summary>
    public class DashboardNotificationObserver : IObserver
    {
        private readonly NotificationStore _store;

        public DashboardNotificationObserver(NotificationStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public Task UpdateAsync(object data)
        {
            if (data is JobApplication application)
            {
                _store.Add(
                    application.JobSeekerId,
                    "Application update",
                    $"Your application is now {application.Status}.",
                    icon: "bi-arrow-repeat");
            }
            else if (data is JobPosting job)
            {
                Console.WriteLine($"[Dashboard] New job posted: {job.Title}");
            }

            return Task.CompletedTask;
        }
    }
}
