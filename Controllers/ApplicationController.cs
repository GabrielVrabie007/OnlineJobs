using Microsoft.AspNetCore.Mvc;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Enums;
using OnlineJobs.Models;

namespace OnlineJobs.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly IApplicationService _applicationService;
        private readonly IJobService _jobService;
        private readonly ICompanyService _companyService;

        public ApplicationController(
            IApplicationService applicationService,
            IJobService jobService,
            ICompanyService companyService)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
        }

        [HttpGet]
        public async Task<IActionResult> Apply(Guid jobId)
        {
            if (!IsJobSeeker())
            {
                TempData["ErrorMessage"] = "Only job seekers can apply to jobs.";
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Apply", new { jobId }) });
            }

            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            if (await _applicationService.HasAlreadyAppliedAsync(jobId, userId.Value))
            {
                TempData["ErrorMessage"] = "You have already applied to this job.";
                return RedirectToAction("Details", "Job", new { id = jobId });
            }

            var job = await _jobService.GetJobByIdAsync(jobId);
            if (job == null)
                return NotFound();

            // Pass job details via ViewBag for display
            ViewBag.Job = job;

            var model = new ApplyJobViewModel
            {
                JobId = jobId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(ApplyJobViewModel model)
        {
            if (!IsJobSeeker())
            {
                TempData["ErrorMessage"] = "Only job seekers can apply to jobs.";
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                    return RedirectToAction("Login", "Account");

                // Create application with enhanced details
                var application = await _applicationService.SubmitApplicationAsync(
                    model.JobId,
                    userId.Value,
                    model.CoverLetter
                );

                // Update additional properties
                application.ExpectedSalary = model.ExpectedSalary;
                application.PortfolioLink = model.PortfolioLink;
                application.AvailableStartDate = model.AvailableStartDate;
                application.AdditionalInfo = model.AdditionalInfo;

                // TODO: Handle Resume file upload
                if (model.Resume != null)
                {
                    // Save resume file and set ResumeUrl
                    // application.ResumeUrl = await SaveResumeFile(model.Resume);
                }

                TempData["SuccessMessage"] = "Application submitted successfully!";
                return RedirectToAction("MyApplications");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> MyApplications()
        {
            if (!IsJobSeeker())
            {
                TempData["ErrorMessage"] = "Only job seekers can view applications.";
                return RedirectToAction("AccessDenied", "Account");
            }

            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var applications = await _applicationService.GetApplicationsByJobSeekerAsync(userId.Value);

            var viewModels = new List<ApplicationDetailsViewModel>();

            foreach (var app in applications)
            {
                var job = await _jobService.GetJobByIdAsync(app.JobPostingId);
                var company = job != null ? await _companyService.GetCompanyByIdAsync(job.CompanyId) : null;
                viewModels.Add(ApplicationDetailsViewModel.FromEntities(app, job, company));
            }

            return View(viewModels);
        }

        public async Task<IActionResult> ReceivedApplications()
        {
            if (!IsEmployer())
            {
                TempData["ErrorMessage"] = "Only employers can view received applications.";
                return RedirectToAction("AccessDenied", "Account");
            }

            var employerId = GetCurrentUserId();
            if (!employerId.HasValue)
                return RedirectToAction("Login", "Account");

            var applications = await _applicationService.GetApplicationsByEmployerAsync(employerId.Value);

            var enrichedApplications = new List<(Domain.Entities.JobApplication Application, Domain.Entities.JobPosting? Job, Domain.Entities.JobSeeker JobSeeker)>();

            foreach (var app in applications)
            {
                var job = await _jobService.GetJobByIdAsync(app.JobPostingId);
                var jobSeeker = await GetJobSeekerByIdAsync(app.JobSeekerId);
                enrichedApplications.Add((app, job, jobSeeker));
            }

            return View(enrichedApplications);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            var application = await _applicationService.GetApplicationByIdAsync(id);
            if (application == null)
                return NotFound();

            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var job = await _jobService.GetJobByIdAsync(application.JobPostingId);
            if (job == null)
                return NotFound();

            bool isAuthorized = application.JobSeekerId == userId.Value || job.EmployerId == userId.Value;

            if (!isAuthorized)
                return Forbid();

            var jobSeeker = await GetJobSeekerByIdAsync(application.JobSeekerId);
            var company = await _companyService.GetCompanyByIdAsync(job.CompanyId);

            ViewBag.Job = job;
            ViewBag.JobSeeker = jobSeeker;
            ViewBag.Company = company;

            return View(application);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(Guid id)
        {
            if (!IsJobSeeker())
                return Unauthorized();

            try
            {
                var application = await _applicationService.GetApplicationByIdAsync(id);
                if (application == null)
                    return NotFound();

                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                    return RedirectToAction("Login", "Account");

                if (application.JobSeekerId != userId.Value)
                    return Forbid();

                await _applicationService.WithdrawApplicationAsync(id);
                TempData["SuccessMessage"] = "Application withdrawn successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("MyApplications");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid id, string status)
        {
            if (!IsEmployer())
                return Unauthorized();

            try
            {
                switch (status?.ToLower())
                {
                    case "review":
                        await _applicationService.StartReviewAsync(id);
                        break;
                    case "interview":
                        await _applicationService.MoveToInterviewAsync(id);
                        break;
                    case "accept":
                        await _applicationService.AcceptApplicationAsync(id);
                        break;
                    case "reject":
                        await _applicationService.RejectApplicationAsync(id);
                        break;
                    default:
                        TempData["ErrorMessage"] = "Invalid status";
                        return RedirectToAction("ReceivedApplications");
                }

                TempData["SuccessMessage"] = "Application status updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("ReceivedApplications");
        }

        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
        }

        private Guid? GetCurrentUserId()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (Guid.TryParse(userIdString, out var userId))
                return userId;
            return null;
        }

        private bool IsEmployer()
        {
            var userType = HttpContext.Session.GetString("UserType");
            return userType == UserType.Employer.ToString();
        }

        private bool IsJobSeeker()
        {
            var userType = HttpContext.Session.GetString("UserType");
            return userType == UserType.JobSeeker.ToString();
        }

        private Task<Domain.Entities.JobSeeker> GetJobSeekerByIdAsync(Guid id)
        {
            return Task.FromResult(new Domain.Entities.JobSeeker(id));
        }
    }
}