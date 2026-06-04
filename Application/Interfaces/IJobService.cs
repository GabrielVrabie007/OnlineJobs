using OnlineJobs.Domain.Entities;
using OnlineJobs.Application.Factories;

namespace OnlineJobs.Application.Interfaces
{

    public interface IJobService
    {
        Task<JobPosting> CreateJobAsync(string title, string description, Guid employerId, Guid companyId);

        /// <summary>Prototype pattern: clone an existing posting into a fresh Draft owned by the same employer.</summary>
        Task<JobPosting> DuplicateJobAsync(Guid jobId, Guid employerId);

        Task<JobPosting> GetJobByIdAsync(Guid jobId);
        Task<IEnumerable<JobPosting>> GetActiveJobsAsync();
        Task UpdateJobAsync(JobPosting job);
        Task PublishJobAsync(Guid jobId);
        Task CloseJobAsync(Guid jobId);
        Task<IEnumerable<JobPosting>> SearchByTitleAsync(string title);
        Task<IEnumerable<JobPosting>> GetJobsByEmployerAsync(Guid employerId);
        Task<IEnumerable<JobPosting>> SearchJobsAsync(string searchTerm, JobSearchStrategyFactory.SearchType searchType);
    }
}