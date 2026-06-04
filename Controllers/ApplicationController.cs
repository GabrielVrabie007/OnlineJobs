using Microsoft.AspNetCore.Mvc;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Commands;
using OnlineJobs.Application.Commands.ApplicationCommands;
using OnlineJobs.Application.Facades;
using OnlineJobs.Application.Iterators;
using OnlineJobs.Application.Mementos;
using OnlineJobs.Application.Observers;
using OnlineJobs.Application.Proxies;
using OnlineJobs.Application.Strategies.ScoringStrategies;
using OnlineJobs.Application.States.ApplicationStates;
using OnlineJobs.Application.ApprovalChains;
using OnlineJobs.Application.Visitors;
using OnlineJobs.Application.Mediators;
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
        private readonly INotificationService _notificationService;
        private readonly IApplicationScoringStrategy _scoringStrategy;
        private readonly JobApplicationFacade _applicationFacade;
        private readonly ApprovalChainFactory _approvalChainFactory;
        private readonly NotificationMediator _notificationMediator;
        private readonly IDocumentGenerationService _documentGenerationService;

        public ApplicationController(
            IApplicationService applicationService,
            IJobService jobService,
            ICompanyService companyService,
            IRepository<JobApplication> applicationRepository,
            IRepository<JobSeeker> jobSeekerRepository,
            CommandInvoker commandInvoker,
            ApplicationStatusSubject applicationStatusSubject,
            ApplicationDraftManager draftManager,
            INotificationService notificationService,
            IApplicationScoringStrategy scoringStrategy,
            JobApplicationFacade applicationFacade,
            ApprovalChainFactory approvalChainFactory,
            NotificationMediator notificationMediator,
            IDocumentGenerationService documentGenerationService)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
            _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
            _jobSeekerRepository = jobSeekerRepository ?? throw new ArgumentNullException(nameof(jobSeekerRepository));
            _commandInvoker = commandInvoker ?? throw new ArgumentNullException(nameof(commandInvoker));
            _applicationStatusSubject = applicationStatusSubject ?? throw new ArgumentNullException(nameof(applicationStatusSubject));
            _draftManager = draftManager ?? throw new ArgumentNullException(nameof(draftManager));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _scoringStrategy = scoringStrategy ?? throw new ArgumentNullException(nameof(scoringStrategy));
            _applicationFacade = applicationFacade ?? throw new ArgumentNullException(nameof(applicationFacade));
            _approvalChainFactory = approvalChainFactory ?? throw new ArgumentNullException(nameof(approvalChainFactory));
            _notificationMediator = notificationMediator ?? throw new ArgumentNullException(nameof(notificationMediator));
            _documentGenerationService = documentGenerationService ?? throw new ArgumentNullException(nameof(documentGenerationService));
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

                // Façade: one call runs the whole workflow — profile/eligibility checks,
                // the Singleton-configured application limit, duplicate guard, the submit,
                // and the Decorator-based confirmation + employer notification.
                var result = await _applicationFacade.SubmitJobApplicationAsync(
                    userId.Value, model.JobId, model.CoverLetter);

                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                    foreach (var validationError in result.ValidationErrors)
                        ModelState.AddModelError(string.Empty, validationError);

                    ViewBag.Job = await _jobService.GetJobByIdAsync(model.JobId);
                    return View(model);
                }

                // The auto-saved draft is no longer needed once the form is submitted.
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

            // Iterator pattern: the chosen traversal does the filtering/ordering — the
            // view just renders what the iterator yields (no client-side re-filtering).
            if (!string.IsNullOrEmpty(filter) && Enum.TryParse<ApplicationStatus>(filter, out var status))
            {
                iterator = applicationCollection.CreateFilteredIterator(status);
            }
            else
            {
                // Default and the explicit "date" sort both show newest first.
                iterator = applicationCollection.CreateDateOrderedIterator(ascending: false);
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
            SetUndoRedoViewBag();

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
                // Virtual Proxy: lazily load this posting's applications and cache them,
                // so the list (and any later count/lookup) come from a single fetch.
                var listAccess = new ApplicationListVirtualProxy(
                    new RealApplicationListAccess(_applicationService, jobId.Value));
                applications = await listAccess.GetApplicationsAsync();
            }
            else
            {
                applications = await _applicationService.GetApplicationsByEmployerAsync(employerId.Value);
            }

            var enrichedApplications = new List<(JobApplication Application, JobPosting? Job, Domain.Entities.JobSeeker JobSeeker, int Score)>();

            foreach (var app in applications)
            {
                var job = await _jobService.GetJobByIdAsync(app.JobPostingId);
                var jobSeeker = await _jobSeekerRepository.GetByIdAsync(app.JobSeekerId);

                if (jobSeeker != null && job != null)
                {
                    // Strategy pattern: the scoring algorithm is injected, so it can be
                    // swapped (skills-only, experience-only, comprehensive…) without
                    // touching this controller.
                    var score = _scoringStrategy.CalculateScore(jobSeeker, job);
                    enrichedApplications.Add((app, job, jobSeeker, score));
                }
            }

            enrichedApplications = enrichedApplications.OrderByDescending(x => x.Score).ToList();

            // Visitor pattern: roll up hiring analytics across these applications.
            var analyticsVisitor = new AnalyticsVisitor();
            var analytics = new AnalyticsResult();
            foreach (var item in enrichedApplications)
            {
                var partial = analyticsVisitor.VisitJobApplication(item.Application);
                analytics.TotalApplications += partial.TotalApplications;
                analytics.InterviewsScheduled += partial.InterviewsScheduled;
                analytics.Hires += partial.Hires;
            }
            analytics.ConversionRate = analytics.TotalApplications > 0
                ? (double)analytics.Hires / analytics.TotalApplications * 100 : 0;
            ViewBag.Analytics = analytics;

            // Chain of Responsibility: pass each application through the approval pipeline
            // (automated screening → HR → manager → director) for a recommendation.
            var approvalChain = _approvalChainFactory.CreateStandardChain();
            var recommendations = new Dictionary<Guid, string>();
            foreach (var item in enrichedApplications)
            {
                var request = new ApprovalRequest(
                    item.Application.Id,
                    item.Application.JobPostingId,
                    employerId.Value,
                    ApplicationStatus.Accepted,
                    UserType.Employer)
                {
                    JobSalary = item.Job?.SalaryMax ?? item.Job?.SalaryMin
                };
                var result = await approvalChain.HandleAsync(request);
                recommendations[item.Application.Id] = result.IsApproved
                    ? $"Cleared by {result.ApprovedBy}"
                    : result.RequiresEscalation
                        ? $"Needs {result.NextApproverLevel}"
                        : result.Reason;
            }
            ViewBag.Recommendations = recommendations;

            SetUndoRedoViewBag();

            return View(enrichedApplications);
        }

        [HttpGet]
        public async Task<IActionResult> ExportApplicationsReport(Guid jobId)
        {
            if (!IsEmployer())
                return Unauthorized();
            var employerId = GetCurrentUserId();
            if (!employerId.HasValue)
                return RedirectToAction("Login", "Account");

            var job = await _jobService.GetJobByIdAsync(jobId);
            if (job == null)
                return NotFound();

            var applications = await _applicationService.GetApplicationsByJobPostingAsync(jobId);

            // Abstract Factory: the employer document factory builds an application report.
            var report = _documentGenerationService.GenerateApplicationReport(job, applications);
            var bytes = System.Text.Encoding.UTF8.GetBytes(report);
            var safeTitle = string.Concat(job.Title.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
            return File(bytes, "text/plain", $"applications-{safeTitle}.txt");
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

            var jobSeeker = await _jobSeekerRepository.GetByIdAsync(application.JobSeekerId);
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

                var withdrawCommand = new WithdrawApplicationCommand(_applicationRepository, id);
                await _commandInvoker.ExecuteAsync(withdrawCommand);

                var updatedApplication = await _applicationService.GetApplicationByIdAsync(id);
                if (updatedApplication != null)
                {
                    // Mediator coordinates the notification fan-out to the Observer subject.
                    await _notificationMediator.NotifyApplicationStatusChangeAsync(updatedApplication);
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
                // State pattern: confirm the requested move is legal from the application's
                // current state (e.g. you can't accept something that wasn't interviewed).
                var current = await _applicationService.GetApplicationByIdAsync(id);
                if (current == null)
                    return NotFound();

                var targetState = status?.ToLower() switch
                {
                    "review" => "UnderReview",
                    "interview" => "Interviewing",
                    "accept" => "Accepted",
                    "reject" => "Rejected",
                    _ => null
                };
                var state = ApplicationStateContext.GetStateFromStatus(current.Status);
                if (targetState != null && !state.CanTransitionTo(targetState))
                {
                    TempData["ErrorMessage"] = $"Cannot move an application from {state.StateName} to {targetState}.";
                    return RedirectToAction("ReceivedApplications");
                }

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
                    // Mediator coordinates the notification fan-out to the Observer subject.
                    await _notificationMediator.NotifyApplicationStatusChangeAsync(updatedApplication);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UndoLastAction(string? returnAction = null)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            var result = await _commandInvoker.UndoAsync();
            TempData[result ? "SuccessMessage" : "ErrorMessage"] =
                result ? "Last action was undone." : "There is nothing to undo.";

            return RedirectToUndoTarget(returnAction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RedoLastAction(string? returnAction = null)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            var result = await _commandInvoker.RedoAsync();
            TempData[result ? "SuccessMessage" : "ErrorMessage"] =
                result ? "Action was redone." : "There is nothing to redo.";

            return RedirectToUndoTarget(returnAction);
        }

        // Sends the user back to whichever list they acted on (employers manage
        // received applications, job seekers manage their own), falling back to role.
        private IActionResult RedirectToUndoTarget(string? returnAction)
        {
            if (returnAction == "MyApplications" || returnAction == "ReceivedApplications")
                return RedirectToAction(returnAction);
            return RedirectToAction(IsEmployer() ? "ReceivedApplications" : "MyApplications");
        }

        // Surfaces the Command pattern's undo/redo availability + the last action's
        // label to the list views so the Undo/Redo buttons can show real state.
        private void SetUndoRedoViewBag()
        {
            ViewBag.CanUndo = _commandInvoker.CanUndo();
            ViewBag.CanRedo = _commandInvoker.CanRedo();
            ViewBag.LastActionLabel = _commandInvoker.GetCommandHistory().FirstOrDefault();
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
    }
}