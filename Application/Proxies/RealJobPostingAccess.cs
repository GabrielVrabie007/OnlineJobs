using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Proxies
{

    public class RealJobPostingAccess : IJobPostingAccess
    {
        private readonly IJobService _jobService;

        public RealJobPostingAccess(IJobService jobService)
        {
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
        }

        public async Task<JobPosting> GetJobDetailsAsync(Guid jobId)
        {
            return await _jobService.GetJobByIdAsync(jobId);
        }

        public async Task<IEnumerable<JobPosting>> GetAllJobsAsync()
        {
            return await _jobService.GetActiveJobsAsync();
        }

        public string GetCompanyName(JobPosting job)
        {
            return job.Company?.Name ?? "Unknown Company";
        }

        public decimal? GetSalaryRange(JobPosting job)
        {
            return job.SalaryMax;
        }
    }
}
