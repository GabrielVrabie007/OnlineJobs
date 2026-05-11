using Microsoft.AspNetCore.Mvc;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Commands;
using OnlineJobs.Application.Commands.ApplicationCommands;
using OnlineJobs.Application.Iterators;
using OnlineJobs.Application.Mementos;
using OnlineJobs.Application.Observers;
using OnlineJobs.Application.Strategies.ScoringStrategies;
using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;
using OnlineJobs.Models;

namespace OnlineJobs.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly IApplicationService _applicationService;
        private readonly IJobService _jobService;
        private readonly ICompanyService _companyService;
        private readonly IRepository<JobApplication> _applicationRepository;
        private readonly IRepository<JobSeeker> _jobSeekerRepository;
        private readonly CommandInvoker _commandInvoker;
        private readonly ApplicationStatusSubject _applicationStatusSubject;
        private readonly ApplicationDraftManager _draftManager;

        public ApplicationController(
            IApplicationService applicationService,
            IJobService jobService,
            ICompanyService companyService,
            IRepository<JobApplication> applicationRepository,
            IRepository<JobSeeker> jobSeekerRepository,
            CommandInvoker commandInvoker,
            ApplicationStatusSubject applicationStatusSubject,
            ApplicationDraftManager draftManager)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
            _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
            _jobSeekerRepository = jobSeekerRepository ?? throw new ArgumentNullException(nameof(jobSeekerRepository));
            _commandInvoker = commandInvoker ?? throw new ArgumentNullException(nameof(commandInvoker));
            _applicationStatusSubject = applicationStatusSubject ?? throw new ArgumentNullException(nameof(applicationStatusSubject));
            _draftManager = draftManager ?? throw new ArgumentNullException(nameof(draftManager));
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

                var emailObserver = new EmailAlertObserver();
                var dashboardObserver = new DashboardNotificationObserver();
                var auditObserver = new AuditLogObserver();

                _applicationStatusSubject.Attach(emailObserver);
                _applicationStatusSubject.Attach(dashboardObserver);
                _applicationStatusSubject.Attach(auditObserver);

                var submitCommand = new SubmitApplicationCommand(
                    _applicationRepository,
                    model.JobId,
                    userId.Value,
                    model.CoverLetter
                );

                await _commandInvoker.ExecuteAsync(submitCommand);

                _draftManager.DeleteDraft(userId.Value, model.JobId);

                TempData["SuccessMessage"] = "Application submitted successfully!";
                return RedirectToAction("MyApplications");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult SaveDraft(Guid jobId, string coverLetter)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Json(new { success = false, message = "User not logged in" });

            var memento = new ApplicationFormMemento(jobId, userId.Value, coverLetter);
            _draftManager.SaveDraft(userId.Value, jobId, memento);

            return Json(new { success = true, message = "Draft saved successfully" });
        }

        [HttpGet]
        public IActionResult LoadDraft(Guid jobId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Json(new { success = false });

            var draft = _draftManager.GetDraft(userId.Value, jobId);

            if (draft != null)
            {
                return Json(new { success = true, coverLetter = draft.CoverLetter, createdAt = draft.CreatedAt });
            }

            return Json(new { success = false });
        }

        public async Task<IActionResult> MyApplications(string? filter = null, string? sort = null)
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
            var applicationCollection = new ApplicationCollection(applications);

            IIterator<JobApplication> iterator;

            if (!string.IsNullOrEmpty(filter) && Enum.TryParse<ApplicationStatus>(filter, out var status))
            {
                iterator = applicationCollection.CreateFilteredIterator(status);
            }
            else if (sort == "date")
            {
                iterator = applicationCollection.CreateDateOrderedIterator(ascending: false);
            }
            else
            {
                iterator = applicationCollection.CreateIterator();
            }

            var viewModels = new List<ApplicationDetailsViewModel>();

            while (iterator.HasNext())
            {
                var app = iterator.Next();
                var job = await _jobService.GetJobByIdAsync(app.JobPostingId);
                var company = job != null ? await _companyService.GetCompanyByIdAsync(job.CompanyId) : null;
                viewModels.Add(ApplicationDetailsViewModel.FromEntities(app, job, company));
            }

            ViewBag.Filter = filter;
            ViewBag.Sort = sort;

            return View(viewModels);
        }

        public async Task<IActionResult> ReceivedApplications(Guid? jobId = null)
        {
            if (!IsEmployer())
            {
                TempData["ErrorMessage"] = "Only employers can view received applications.";
                return RedirectToAction("AccessDenied", "Account");
            }

            var employerId = GetCurrentUserId();
            if (!employerId.HasValue)
                return RedirectToAction("Login", "Account");

            IEnumerable<JobApplication> applications;

            if (jobId.HasValue)
            {
                applications = await _applicationService.GetApplicationsByJobPostingAsync(jobId.Value);
            }
            else
            {
                applications = await _applicationService.GetApplicationsByEmployerAsync(employerId.Value);
            }

            var enrichedApplications = new List<(JobApplication Application, JobPosting? Job, Domain.Entities.JobSeeker JobSeeker, int Score)>();
            var scoringStrategy = new ComprehensiveScoringStrategy();

            foreach (var app in applications)
            {
                var job = await _jobService.GetJobByIdAsync(app.JobPostingId);
                var jobSeeker = await _jobSeekerRepository.GetByIdAsync(app.JobSeekerId);

                if (jobSeeker != null && job != null)
                {
                    var score = scoringStrategy.CalculateScore(jobSeeker, job);
                    enrichedApplications.Add((app, job, jobSeeker, score));
                }
            }

            enrichedApplications = enrichedApplications.OrderByDescending(x => x.Score).ToList();

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

                var emailObserver = new EmailAlertObserver();
                var auditObserver = new AuditLogObserver();

                _applicationStatusSubject.Attach(emailObserver);
                _applicationStatusSubject.Attach(auditObserver);

                var withdrawCommand = new WithdrawApplicationCommand(_applicationRepository, id);
                await _commandInvoker.ExecuteAsync(withdrawCommand);

                var updatedApplication = await _applicationService.GetApplicationByIdAsync(id);
                if (updatedApplication != null)
                {
                    await _applicationStatusSubject.NotifyAsync(updatedApplication);
                }

                TempData["SuccessMessage"] = "Application withdrawn successfully. You can undo this action if needed.";
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
                var emailObserver = new EmailAlertObserver();
                var dashboardObserver = new DashboardNotificationObserver();
                var auditObserver = new AuditLogObserver();

                _applicationStatusSubject.Attach(emailObserver);
                _applicationStatusSubject.Attach(dashboardObserver);
                _applicationStatusSubject.Attach(auditObserver);

                ICommand command = status?.ToLower() switch
                {
                    "accept" => new ApproveApplicationCommand(_applicationRepository, id),
                    "reject" => new RejectApplicationCommand(_applicationRepository, id),
                    _ => null
                };

                if (command != null)
                {
                    await _commandInvoker.ExecuteAsync(command);
                }
                else
                {
                    switch (status?.ToLower())
                    {
                        case "review":
                            await _applicationService.StartReviewAsync(id);
                            break;
                        case "interview":
                            await _applicationService.MoveToInterviewAsync(id);
                            break;
                        default:
                            TempData["ErrorMessage"] = "Invalid status";
                            return RedirectToAction("ReceivedApplications");
                    }
                }

                var updatedApplication = await _applicationService.GetApplicationByIdAsync(id);
                if (updatedApplication != null)
                {
                    await _applicationStatusSubject.NotifyAsync(updatedApplication);
                }

                TempData["SuccessMessage"] = "Application status updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("ReceivedApplications");
        }

        [HttpPost]
        public async Task<IActionResult> UndoLastAction()
        {
            var result = await _commandInvoker.UndoAsync();
            if (result)
            {
                TempData["SuccessMessage"] = "Last action undone successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "No action to undo.";
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