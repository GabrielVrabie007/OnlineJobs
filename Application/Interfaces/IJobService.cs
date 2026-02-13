using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Interfaces
{

    public interface IJobService
    {
        Task<JobPosting> CreateJobAsync(string title, string description, Guid employerId, Guid companyId);
        Task<JobPosting> GetJobByIdAsync(Guid jobId);
        Task<IEnumerable<JobPosting>> GetActiveJobsAsync();
        Task UpdateJobAsync(JobPosting job);
        Task PublishJobAsync(Guid jobId);
        Task CloseJobAsync(Guid jobId);
        Task<IEnumerable<JobPosting>> SearchByTitleAsync(string title);
        Task<IEnumerable<JobPosting>> GetJobsByEmployerAsync(Guid employerId);
    }
}