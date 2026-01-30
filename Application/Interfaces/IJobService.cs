using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Interfaces
{
    public interface IJobService
    {
        Task<JobPosting> CreateJobAsync(string title, string description, Guid employerId, Guid companyId);
        Task<JobPosting> GetJobByIdAsync(Guid jobId);
        Task<IEnumerable<JobPosting>> GetAllJobsAsync();
        Task<IEnumerable<JobPosting>> GetActiveJobsAsync();
        Task UpdateJobAsync(JobPosting job);
        Task DeleteJobAsync(Guid jobId);

        Task PublishJobAsync(Guid jobId);
        Task CloseJobAsync(Guid jobId);
        Task CancelJobAsync(Guid jobId);

        Task<IEnumerable<JobPosting>> SearchByTitleAsync(string title);
        Task<IEnumerable<JobPosting>> GetJobsByEmployerAsync(Guid employerId);
        Task<IEnumerable<JobPosting>> GetJobsByCompanyAsync(Guid companyId);
        Task<IEnumerable<JobPosting>> GetJobsByStatusAsync(JobStatus status);

        Task<int> GetTotalJobCountAsync();
        Task<int> GetActiveJobCountAsync();
    }
}