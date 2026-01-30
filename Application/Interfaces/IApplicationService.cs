using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Interfaces
{
    public interface IApplicationService
    {
        Task<JobApplication> SubmitApplicationAsync(Guid jobPostingId, Guid jobSeekerId, string coverLetter);
        Task<JobApplication> GetApplicationByIdAsync(Guid applicationId);
        Task<IEnumerable<JobApplication>> GetAllApplicationsAsync();

        Task<IEnumerable<JobApplication>> GetApplicationsByJobSeekerAsync(Guid jobSeekerId);
        Task<IEnumerable<JobApplication>> GetApplicationsByJobPostingAsync(Guid jobPostingId);
        Task<IEnumerable<JobApplication>> GetApplicationsByEmployerAsync(Guid employerId);

        Task StartReviewAsync(Guid applicationId);
        Task MoveToInterviewAsync(Guid applicationId);
        Task AcceptApplicationAsync(Guid applicationId);
        Task RejectApplicationAsync(Guid applicationId, string reason = null);
        Task WithdrawApplicationAsync(Guid applicationId);

        Task<IEnumerable<JobApplication>> GetApplicationsByStatusAsync(ApplicationStatus status);

        Task<bool> HasAlreadyAppliedAsync(Guid jobPostingId, Guid jobSeekerId);

        Task<int> GetApplicationCountForJobAsync(Guid jobPostingId);
        Task<int> GetApplicationCountForJobSeekerAsync(Guid jobSeekerId);
    }
}