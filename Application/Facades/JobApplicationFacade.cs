using OnlineJobs.Application.Configuration;
using OnlineJobs.Application.DTOs;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;
using OnlineJobs.Domain.Interfaces;

namespace OnlineJobs.Application.Facades;


public class JobApplicationFacade
{
    private readonly IUserService _userService;
    private readonly IJobService _jobService;
    private readonly IApplicationService _applicationService;
    private readonly INotificationService _notificationService;
    private readonly IRepository<JobSeeker> _jobSeekerRepository;
    private readonly IRepository<JobPosting> _jobPostingRepository;

    public JobApplicationFacade(
        IUserService userService,
        IJobService jobService,
        IApplicationService applicationService,
        INotificationService notificationService,
        IRepository<JobSeeker> jobSeekerRepository,
        IRepository<JobPosting> jobPostingRepository)
    {
        _userService = userService;
        _jobService = jobService;
        _applicationService = applicationService;
        _notificationService = notificationService;
        _jobSeekerRepository = jobSeekerRepository;
        _jobPostingRepository = jobPostingRepository;
    }


    public async Task<ApplicationResult> SubmitJobApplicationAsync(Guid jobSeekerId, Guid jobPostingId, string coverLetter)
    {

        var errors = new List<string>();

        var jobSeeker = await _jobSeekerRepository.GetByIdAsync(jobSeekerId);

        if (jobSeeker == null)
        {
            return ApplicationResult.Failed("Job seeker not found");
        }

        // Incomplete profile is a nudge, not a blocker — like real job boards, the
        // candidate can still apply but gets reminded to round out their profile.
        if (!jobSeeker.HasCompleteProfile())
        {
            await _notificationService.SendProfileCompletionReminderAsync(jobSeekerId);
        }

        if (!jobSeeker.CanApplyToJobs())
        {
            errors.Add("Your account is not active");
            return ApplicationResult.Failed("Account not active", errors);
        }

 
        var config = ApplicationConfiguration.Instance;
        var existingApplications = await _applicationService.GetApplicationsByJobSeekerAsync(jobSeekerId);
        var activeApplications = existingApplications.Count(a =>
            a.Status == ApplicationStatus.Submitted ||
            a.Status == ApplicationStatus.UnderReview ||
            a.Status == ApplicationStatus.Interviewing);

        if (activeApplications >= config.MaxActiveApplicationsPerUser)
        {
            errors.Add($"You have reached the maximum of {config.MaxActiveApplicationsPerUser} active applications");
            return ApplicationResult.Failed("Application limit reached", errors);
        }

        var alreadyApplied = existingApplications.Any(a => a.JobPostingId == jobPostingId);
        if (alreadyApplied)
        {
            errors.Add("You have already applied to this job");
            return ApplicationResult.Failed("Duplicate application", errors);
        }

        var jobPosting = await _jobPostingRepository.GetByIdAsync(jobPostingId);

        if (jobPosting == null)
        {
            return ApplicationResult.Failed("Job posting not found");
        }

        if (!jobPosting.IsAcceptingApplications())
        {
            errors.Add($"This job is no longer accepting applications (Status: {jobPosting.Status})");
            return ApplicationResult.Failed("Job not accepting applications", errors);
        }

        var application = await _applicationService.SubmitApplicationAsync(
            jobPostingId,
            jobSeekerId,
            coverLetter
        );


        await _notificationService.SendApplicationConfirmationAsync(
            jobSeekerId,
            jobPosting.Title
        );

        await _notificationService.NotifyEmployerNewApplicationAsync(
            jobPosting.EmployerId,
            $"{jobSeeker.FirstName} {jobSeeker.LastName}",
            jobPosting.Title
        );


        return ApplicationResult.Successful(application);
    }
    
    public async Task<string> GetApplicationStatusAsync(Guid applicationId)
    {
        var application = await _applicationService.GetApplicationByIdAsync(applicationId);

        if (application == null)
            return "Application not found";

        return application.Status switch
        {
            ApplicationStatus.Submitted => "Your application has been submitted",
            ApplicationStatus.UnderReview => "Your application is being reviewed",
            ApplicationStatus.Interviewing => "Congratulations! You've been invited for an interview",
            ApplicationStatus.Accepted => "Congratulations! Your application has been accepted",
            ApplicationStatus.Rejected => "Unfortunately, your application was not successful",
            ApplicationStatus.Withdrawn => "You have withdrawn this application",
            _ => "Unknown status"
        };
    }
}