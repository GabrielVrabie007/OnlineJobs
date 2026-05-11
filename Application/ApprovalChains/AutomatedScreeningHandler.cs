using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.ApprovalChains
{
    public class AutomatedScreeningHandler : BaseApprovalHandler
    {
        private readonly IRepository<JobApplication> _applicationRepository;
        private readonly IRepository<JobSeeker> _jobSeekerRepository;

        public AutomatedScreeningHandler(
            IRepository<JobApplication> applicationRepository,
            IRepository<JobSeeker> jobSeekerRepository)
        {
            _applicationRepository = applicationRepository;
            _jobSeekerRepository = jobSeekerRepository;
        }

        public override async Task<ApprovalResult> HandleAsync(ApprovalRequest request)
        {
            var application = await _applicationRepository.GetByIdAsync(request.ApplicationId);
            if (application == null)
            {
                return ApprovalResult.Rejected("AutomatedScreening", "Application not found");
            }

            var jobSeeker = await _jobSeekerRepository.GetByIdAsync(application.JobSeekerId);
            if (jobSeeker == null)
            {
                return ApprovalResult.Rejected("AutomatedScreening", "Job seeker profile not found");
            }

            if (string.IsNullOrWhiteSpace(application.CoverLetter))
            {
                return ApprovalResult.Rejected("AutomatedScreening", "Cover letter is required");
            }

            if (application.CoverLetter.Length < 50)
            {
                return ApprovalResult.Rejected("AutomatedScreening",
                    "Cover letter must be at least 50 characters");
            }

            return await PassToNextAsync(request);
        }
    }
}
