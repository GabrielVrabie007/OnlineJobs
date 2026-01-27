using Microsoft.AspNetCore.Mvc;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Enums;
using OnlineJobs.Web.Models;

namespace OnlineJobs.Web.Controllers
{
    /// <summary>
    /// Job controller
    /// Demonstrates:
    /// - SRP: Single responsibility - job posting management
    /// - DIP: Depends on service abstractions
    /// - Thin controller - business logic delegated to services
    /// - Proper error handling and validation
    /// </summary>
    public class JobController : Controller
    {
        private readonly IJobService _jobService;
        private readonly ICompanyService _companyService;
        private readonly IApplicationService _applicationService;

        // Constructor injection (DIP)
        public JobController(
            IJobService jobService,
            ICompanyService companyService,
            IApplicationService applicationService)
        {
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
        }

        // GET: Job/Index
        public async Task<IActionResult> Index(string searchTerm = null)
        {
            IEnumerable<Domain.Entities.JobPosting> jobs;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                jobs = await _jobService.SearchByTitleAsync(searchTerm);
                ViewBag.SearchTerm = searchTerm;
            }
            else
            {
                jobs = await _jobService.GetActiveJobsAsync();
            }

            return View(jobs);
        }

        // GET: Job/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
                return NotFound();

            var company = await _companyService.GetCompanyByIdAsync(job.CompanyId);
            ViewBag.Company = company;

            // Get application count
            var applicationCount = await _applicationService.GetApplicationCountForJobAsync(id);
            ViewBag.ApplicationCount = applicationCount;

            // Check if current user has already applied
            var userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                var hasApplied = await _applicationService.HasAlreadyAppliedAsync(id, userId.Value);
                ViewBag.HasApplied = hasApplied;
            }

            return View(job);
        }

        // GET: Job/Create
        public async Task<IActionResult> Create()
        {
            if (!IsEmployer())
            {
                TempData["ErrorMessage"] = "Only employers can create job postings.";
                return RedirectToAction("AccessDenied", "Account");
            }

            var companies = await _companyService.GetAllCompaniesAsync();
            ViewBag.Companies = companies;

            return View();
        }

        // POST: Job/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateJobViewModel model)
        {
            if (!IsEmployer())
            {
                TempData["ErrorMessage"] = "Only employers can create job postings.";
                return RedirectToAction("AccessDenied", "Account");
            }

            if (!ModelState.IsValid)
            {
                var companies = await _companyService.GetAllCompaniesAsync();
                ViewBag.Companies = companies;
                return View(model);
            }

            try
            {
                var employerId = GetCurrentUserId();
                if (!employerId.HasValue)
                    return RedirectToAction("Login", "Account");

                var job = await _jobService.CreateJobAsync(
                    model.Title,
                    model.Description,
                    employerId.Value,
                    model.CompanyId
                );

                // Update additional properties
                job.Requirements = model.Requirements;
                job.SalaryMin = model.SalaryMin;
                job.SalaryMax = model.SalaryMax;
                job.Location = model.Location;
                job.EmploymentType = model.EmploymentType;
                job.Category = model.Category;

                await _jobService.UpdateJobAsync(job);

                // Auto-publish the job
                await _jobService.PublishJobAsync(job.Id);

                TempData["SuccessMessage"] = "Job posted successfully!";
                return RedirectToAction("Details", new { id = job.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var companies = await _companyService.GetAllCompaniesAsync();
                ViewBag.Companies = companies;
                return View(model);
            }
        }

        // GET: Job/MyJobs
        public async Task<IActionResult> MyJobs()
        {
            if (!IsEmployer())
            {
                TempData["ErrorMessage"] = "Only employers can view their job postings.";
                return RedirectToAction("AccessDenied", "Account");
            }

            var employerId = GetCurrentUserId();
            if (!employerId.HasValue)
                return RedirectToAction("Login", "Account");

            var jobs = await _jobService.GetJobsByEmployerAsync(employerId.Value);

            return View(jobs);
        }

        // POST: Job/Close/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(Guid id)
        {
            if (!IsEmployer())
                return Unauthorized();

            try
            {
                await _jobService.CloseJobAsync(id);
                TempData["SuccessMessage"] = "Job posting closed successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("MyJobs");
        }

        // Helper methods
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