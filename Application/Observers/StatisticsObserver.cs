using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Observers
{
    public class StatisticsObserver : IObserver
    {
        private int _jobPostCount = 0;
        private int _applicationStatusChanges = 0;

        public int JobPostCount => _jobPostCount;
        public int ApplicationStatusChanges => _applicationStatusChanges;

        public Task UpdateAsync(object data)
        {
            if (data is JobPosting)
            {
                _jobPostCount++;
                Console.WriteLine($"[Statistics] Total job posts: {_jobPostCount}");
            }
            else if (data is JobApplication)
            {
                _applicationStatusChanges++;
                Console.WriteLine($"[Statistics] Total status changes: {_applicationStatusChanges}");
            }

            return Task.CompletedTask;
        }
    }
}
