using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Interfaces
{
    /// <summary>
    /// Application service interface
    /// Demonstrates:
    /// - ISP: Only job application operations, segregated from other concerns
    /// - SRP: Single responsibility - job application workflow management
    /// - DIP: Controllers depend on this abstraction
    /// </summary>
    public interface IApplicationService
    {
        // Application submission
        Task<JobApplication> SubmitApplicationAsync(Guid jobPostingId, Guid jobSeekerId, string coverLetter);
        Task<JobApplication> GetApplicationByIdAsync(Guid applicationId);
        Task<IEnumerable<JobApplication>> GetAllApplicationsAsync();

        // Query by relationships
        Task<IEnumerable<JobApplication>> GetApplicationsByJobSeekerAsync(Guid jobSeekerId);
        Task<IEnumerable<JobApplication>> GetApplicationsByJobPostingAsync(Guid jobPostingId);
        Task<IEnumerable<JobApplication>> GetApplicationsByEmployerAsync(Guid employerId);

        // Application state management
        Task StartReviewAsync(Guid applicationId);
        Task MoveToInterviewAsync(Guid applicationId);
        Task AcceptApplicationAsync(Guid applicationId);
        Task RejectApplicationAsync(Guid applicationId, string reason = null);
        Task WithdrawApplicationAsync(Guid applicationId);

        // Query by status
        Task<IEnumerable<JobApplication>> GetApplicationsByStatusAsync(ApplicationStatus status);

        // Validation
        Task<bool> HasAlreadyAppliedAsync(Guid jobPostingId, Guid jobSeekerId);

        // Statistics
        Task<int> GetApplicationCountForJobAsync(Guid jobPostingId);
        Task<int> GetApplicationCountForJobSeekerAsync(Guid jobSeekerId);
    }
}