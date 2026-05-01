using Microsoft.AspNetCore.Mvc;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Proxies;
using OnlineJobs.Domain.Enums;
using OnlineJobs.Models;

namespace OnlineJobs.Controllers
{
    public class JobController : Controller
    {
        private readonly IJobService _jobService;
        private readonly ICompanyService _companyService;
        private readonly IApplicationService _applicationService;
        private readonly IUserService _userService;

        public JobController(
            IJobService jobService,
            ICompanyService companyService,
            IApplicationService applicationService,
            IUserService userService)
        {
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

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

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var realAccess = new RealJobPostingAccess(_jobService);
                var userId = GetCurrentUserId();
                var isAuthenticated = IsUserLoggedIn();

                var jobProxy = new JobPostingProtectionProxy(realAccess, isAuthenticated, userId);

                var job = await jobProxy.GetJobDetailsAsync(id);
                if (job == null)
                {
                    TempData["ErrorMessage"] = $"Job with ID {id} not found in database";
                    return RedirectToAction("Index");
                }

                ViewBag.Company = job.Company;
                ViewBag.CompanyName = jobProxy.GetCompanyName(job);
                ViewBag.SalaryRange = jobProxy.GetSalaryRange(job);
                ViewBag.IsAuthenticated = isAuthenticated;
                ViewBag.ApplicationCount = 0;
                ViewBag.HasApplied = false;

                try
                {
                    var applicationCount = await _applicationService.GetApplicationCountForJobAsync(id);
                    ViewBag.ApplicationCount = applicationCount;
                }
                catch (Exception appEx)
                {
                    Console.WriteLine($"Warning: Could not load application count: {appEx.Message}");
                }

                if (userId.HasValue && IsJobSeeker())
                {
                    try
                    {
                        var hasApplied = await _applicationService.HasAlreadyAppliedAsync(id, userId.Value);
                        ViewBag.HasApplied = hasApplied;
                    }
                    catch (Exception applyEx)
                    {
                        Console.WriteLine($"Warning: Could not check if user applied: {applyEx.Message}");
                    }
                }

                return View(job);
            }
            catch (Exception ex)
            {
                var errorDetails = $"Error loading job {id}: {ex.Message}\nStack: {ex.StackTrace}";
                Console.WriteLine(errorDetails);
                TempData["ErrorMessage"] = $"Error loading job details: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

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

                // Get employer's company ID
                var employer = await _userService.GetEmployerAsync(employerId.Value);
                if (employer?.CompanyId == null)
                {
                    ModelState.AddModelError(string.Empty, "Your account is not associated with a company. Please contact support.");
                    var companies = await _companyService.GetAllCompaniesAsync();
                    ViewBag.Companies = companies;
                    return View(model);
                }

                var job = await _jobService.CreateJobAsync(
                    model.Title,
                    model.Description,
                    employerId.Value,
                    employer.CompanyId.Value
                );

                job.Requirements = model.Requirements;
                job.SalaryMin = model.SalaryMin;
                job.SalaryMax = model.SalaryMax;
                job.Location = model.Location;
                job.EmploymentType = model.EmploymentType;
                job.Category = model.Category;

                await _jobService.UpdateJobAsync(job);

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