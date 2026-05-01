using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Proxies
{

    public interface IJobPostingAccess
    {
        Task<JobPosting> GetJobDetailsAsync(Guid jobId);
        Task<IEnumerable<JobPosting>> GetAllJobsAsync();
        string GetCompanyName(JobPosting job);
        decimal? GetSalaryRange(JobPosting job);
    }
}
