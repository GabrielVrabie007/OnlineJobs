using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Interfaces
{
    /// <summary>
    /// Job service interface
    /// Demonstrates:
    /// - ISP: Only job posting operations, segregated from other concerns
    /// - SRP: Single responsibility - job posting management
    /// - DIP: Controllers depend on this abstraction
    /// </summary>
    public interface IJobService
    {
        // Job CRUD operations
        Task<JobPosting> CreateJobAsync(string title, string description, Guid employerId, Guid companyId);
        Task<JobPosting> GetJobByIdAsync(Guid jobId);
        Task<IEnumerable<JobPosting>> GetAllJobsAsync();
        Task<IEnumerable<JobPosting>> GetActiveJobsAsync();
        Task UpdateJobAsync(JobPosting job);
        Task DeleteJobAsync(Guid jobId);

        // Job state management
        Task PublishJobAsync(Guid jobId);
        Task CloseJobAsync(Guid jobId);
        Task CancelJobAsync(Guid jobId);

        // Search and filtering
        Task<IEnumerable<JobPosting>> SearchByTitleAsync(string title);
        Task<IEnumerable<JobPosting>> GetJobsByEmployerAsync(Guid employerId);
        Task<IEnumerable<JobPosting>> GetJobsByCompanyAsync(Guid companyId);
        Task<IEnumerable<JobPosting>> GetJobsByStatusAsync(JobStatus status);

        // Statistics
        Task<int> GetTotalJobCountAsync();
        Task<int> GetActiveJobCountAsync();
    }
}