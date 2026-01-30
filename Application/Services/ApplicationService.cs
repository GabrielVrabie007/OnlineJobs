using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IRepository<JobApplication> _applicationRepository;
        private readonly IRepository<JobPosting> _jobRepository;
        private readonly IRepository<JobSeeker> _jobSeekerRepository;

        public ApplicationService(
            IRepository<JobApplication> applicationRepository,
            IRepository<JobPosting> jobRepository,
            IRepository<JobSeeker> jobSeekerRepository)
        {
            _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
            _jobRepository = jobRepository ?? throw new ArgumentNullException(nameof(jobRepository));
            _jobSeekerRepository = jobSeekerRepository ?? throw new ArgumentNullException(nameof(jobSeekerRepository));
        }

        public async Task<JobApplication> SubmitApplicationAsync(Guid jobPostingId, Guid jobSeekerId, string coverLetter)
        {
            var job = await _jobRepository.GetByIdAsync(jobPostingId);
            if (job == null)
                throw new InvalidOperationException("Job posting not found");

            if (!job.IsAcceptingApplications())
                throw new InvalidOperationException("Job is not accepting applications");

            var jobSeeker = await _jobSeekerRepository.GetByIdAsync(jobSeekerId);
            if (jobSeeker == null)
                throw new InvalidOperationException("Job seeker not found");

            if (!jobSeeker.CanApplyToJobs())
                throw new InvalidOperationException("Job seeker is not authorized to apply");

            if (await HasAlreadyAppliedAsync(jobPostingId, jobSeekerId))
                throw new InvalidOperationException("You have already applied to this job");

            var application = new JobApplication(jobPostingId, jobSeekerId, coverLetter);
            await _applicationRepository.AddAsync(application);

            return application;
        }

        public async Task<JobApplication> GetApplicationByIdAsync(Guid applicationId)
        {
            return await _applicationRepository.GetByIdAsync(applicationId);
        }

        public async Task<IEnumerable<JobApplication>> GetAllApplicationsAsync()
        {
            return await _applicationRepository.GetAllAsync();
        }

        public async Task<IEnumerable<JobApplication>> GetApplicationsByJobSeekerAsync(Guid jobSeekerId)
        {
            return await _applicationRepository.FindAsync(a => a.JobSeekerId == jobSeekerId);
        }

        public async Task<IEnumerable<JobApplication>> GetApplicationsByJobPostingAsync(Guid jobPostingId)
        {
            return await _applicationRepository.FindAsync(a => a.JobPostingId == jobPostingId);
        }

        public async Task<IEnumerable<JobApplication>> GetApplicationsByEmployerAsync(Guid employerId)
        {
            var employerJobs = await _jobRepository.FindAsync(j => j.EmployerId == employerId);
            var jobIds = employerJobs.Select(j => j.Id).ToHashSet();

            return await _applicationRepository.FindAsync(a => jobIds.Contains(a.JobPostingId));
        }

        public async Task StartReviewAsync(Guid applicationId)
        {
            var application = await GetApplicationByIdAsync(applicationId);
            if (application == null)
                throw new InvalidOperationException("Application not found");

            application.StartReview();
            await _applicationRepository.UpdateAsync(application);
        }

        public async Task MoveToInterviewAsync(Guid applicationId)
        {
            var application = await GetApplicationByIdAsync(applicationId);
            if (application == null)
                throw new InvalidOperationException("Application not found");

            application.MoveToInterview();
            await _applicationRepository.UpdateAsync(application);
        }

        public async Task AcceptApplicationAsync(Guid applicationId)
        {
            var application = await GetApplicationByIdAsync(applicationId);
            if (application == null)
                throw new InvalidOperationException("Application not found");

            application.Accept();
            await _applicationRepository.UpdateAsync(application);
        }

        public async Task RejectApplicationAsync(Guid applicationId, string reason = null)
        {
            var application = await GetApplicationByIdAsync(applicationId);
            if (application == null)
                throw new InvalidOperationException("Application not found");

            application.Reject(reason);
            await _applicationRepository.UpdateAsync(application);
        }

        public async Task WithdrawApplicationAsync(Guid applicationId)
        {
            var application = await GetApplicationByIdAsync(applicationId);
            if (application == null)
                throw new InvalidOperationException("Application not found");

            if (!application.CanBeWithdrawn())
                throw new InvalidOperationException("Application cannot be withdrawn in its current state");

            application.Withdraw();
            await _applicationRepository.UpdateAsync(application);
        }

        public async Task<IEnumerable<JobApplication>> GetApplicationsByStatusAsync(ApplicationStatus status)
        {
            return await _applicationRepository.FindAsync(a => a.Status == status);
        }

        public async Task<bool> HasAlreadyAppliedAsync(Guid jobPostingId, Guid jobSeekerId)
        {
            var applications = await _applicationRepository.FindAsync(a =>
                a.JobPostingId == jobPostingId && a.JobSeekerId == jobSeekerId);

            return applications.Any();
        }

        public async Task<int> GetApplicationCountForJobAsync(Guid jobPostingId)
        {
            var applications = await GetApplicationsByJobPostingAsync(jobPostingId);
            return applications.Count();
        }

        public async Task<int> GetApplicationCountForJobSeekerAsync(Guid jobSeekerId)
        {
            var applications = await GetApplicationsByJobSeekerAsync(jobSeekerId);
            return applications.Count();
        }
    }
}