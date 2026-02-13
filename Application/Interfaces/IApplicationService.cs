using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Interfaces
{

    public interface IApplicationService
    {
        Task<JobApplication> SubmitApplicationAsync(Guid jobPostingId, Guid jobSeekerId, string coverLetter);
        Task<JobApplication> GetApplicationByIdAsync(Guid applicationId);
        Task<IEnumerable<JobApplication>> GetApplicationsByJobSeekerAsync(Guid jobSeekerId);
        Task<IEnumerable<JobApplication>> GetApplicationsByEmployerAsync(Guid employerId);
        Task StartReviewAsync(Guid applicationId);
        Task MoveToInterviewAsync(Guid applicationId);
        Task AcceptApplicationAsync(Guid applicationId);
        Task RejectApplicationAsync(Guid applicationId, string reason = null);
        Task WithdrawApplicationAsync(Guid applicationId);
        Task<bool> HasAlreadyAppliedAsync(Guid jobPostingId, Guid jobSeekerId);
        Task<int> GetApplicationCountForJobAsync(Guid jobPostingId);
    }
}