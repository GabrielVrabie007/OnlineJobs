using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Observers
{
    public class EmailAlertObserver : IObserver
    {
        public Task UpdateAsync(object data)
        {
            if (data is JobPosting job)
            {
                Console.WriteLine($"[Email] Sending email alert for new job: {job.Title}");
            }
            else if (data is JobApplication application)
            {
                Console.WriteLine($"[Email] Sending email notification for application status: {application.Status}");
            }

            return Task.CompletedTask;
        }
    }
}
