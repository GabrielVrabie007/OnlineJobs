using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Observers
{
    public class JobSeekerObserver : IObserver
    {
        private readonly List<string> _notifications = new();

        public List<string> Notifications => _notifications;

        public Task UpdateAsync(object data)
        {
            if (data is JobPosting job)
            {
                var message = $"New job alert: {job.Title} at {job.CompanyId}";
                _notifications.Add(message);
                Console.WriteLine($"[JobSeeker] {message}");
            }
            else if (data is JobApplication application)
            {
                var message = $"Application status updated: {application.Status}";
                _notifications.Add(message);
                Console.WriteLine($"[JobSeeker] {message}");
            }

            return Task.CompletedTask;
        }
    }
}
