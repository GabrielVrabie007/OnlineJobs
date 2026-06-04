using Microsoft.AspNetCore.Mvc;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Factories;
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
        private readonly ICompanyRevealService _companyRevealService;
        private readonly OnlineJobs.Application.Strategies.SalaryStrategies.SalaryStrategyFactory _salaryStrategyFactory;
        private readonly SkillFlyweightFactory _skillFlyweightFactory;

        public JobController(
            IJobService jobService,
            ICompanyService companyService,
            IApplicationService applicationService,
            IUserService userService,
            ICompanyRevealService companyRevealService,
            OnlineJobs.Application.Strategies.SalaryStrategies.SalaryStrategyFactory salaryStrategyFactory,
            SkillFlyweightFactory skillFlyweightFactory)
        {
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _companyRevealService = companyRevealService ?? throw new ArgumentNullException(nameof(companyRevealService));
            _salaryStrategyFactory = salaryStrategyFactory ?? throw new ArgumentNullException(nameof(salaryStrategyFactory));
            _skillFlyweightFactory = skillFlyweightFactory ?? throw new ArgumentNullException(nameof(skillFlyweightFactory));
        }

        public async Task<IActionResult> Index(string? searchTerm = null, string searchBy = "Title")
        {
            IEnumerable<Domain.Entities.JobPosting> jobs;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Strategy pattern: the factory picks the search algorithm (by title,
                // location or category) chosen by the user at runtime.
                if (!Enum.TryParse<JobSearchStrategyFactory.SearchType>(searchBy, true, out var searchType))
                    searchType = JobSearchStrategyFactory.SearchType.Title;

                jobs = await _jobService.SearchJobsAsync(searchTerm, searchType);
                ViewBag.SearchTerm = searchTerm;
                ViewBag.SearchBy = searchType.ToString();
            }
            else
            {
                jobs = await _jobService.GetActiveJobsAsync();
                ViewBag.SearchBy = "Title";
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

                // If the employer hid the company, a job seeker who paid to reveal it
                // (Adapter/payment flow) — or the owning employer — sees it anyway.
                if (!job.IsCompanyRevealed && userId.HasValue)
                {
                    var ownsJob = job.EmployerId == userId.Value;
                    var purchasedReveal = IsJobSeeker() &&
                        await _companyRevealService.HasAccessToCompanyAsync(userId.Value, id);
                    if (ownsJob || purchasedReveal)
                        job.IsCompanyRevealed = true;
                }

                ViewBag.Company = job.Company;
                ViewBag.CompanyName = jobProxy.GetCompanyName(job);
                ViewBag.SalaryRange = jobProxy.GetSalaryRange(job);
                ViewBag.IsAuthenticated = isAuthenticated;
                ViewBag.ApplicationCount = 0;
                ViewBag.HasApplied = false;

                // Strategy pattern: present the pay figure using the calculation that
                // matches the employment type (per-year / per-hour / per-project / …).
                if (isAuthenticated && job.SalaryMin.HasValue)
                {
                    var salaryStrategy = _salaryStrategyFactory.ForEmploymentType(job.EmploymentType);
                    ViewBag.SalaryBasis = salaryStrategy.GetSalaryDescription(job.SalaryMin.Value);
                }

                // Flyweight pattern: intern the job's required skills so the same skill
                // (e.g. "C#") is one shared object across every job — viewing jobs grows
                // a single shared pool instead of duplicating skill objects.
                if (!string.IsNullOrWhiteSpace(job.Requirements))
                {
                    var tokens = job.Requirements
                        .Split(new[] { ',', '\n', '\r', ';', '•', '|' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(t => t.Trim())
                        .Where(t => t.Length >= 2 && t.Length <= 30)
                        .Take(12);
                    ViewBag.Skills = _skillFlyweightFactory.GetSkills(tokens);
                    ViewBag.SkillPoolSize = _skillFlyweightFactory.GetPoolSize();
                }

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

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (!IsEmployer())
            {
                TempData["ErrorMessage"] = "Only employers can edit job postings.";
                return RedirectToAction("AccessDenied", "Account");
            }

            var employerId = GetCurrentUserId();
            if (!employerId.HasValue)
                return RedirectToAction("Login", "Account");

            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
                return NotFound();
            if (job.EmployerId != employerId.Value)
                return Forbid();

            var model = new EditJobViewModel
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Requirements = job.Requirements,
                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax,
                Location = job.Location,
                EmploymentType = job.EmploymentType,
                Category = job.Category,
                ExperienceLevel = job.ExperienceLevel,
                IsCompanyRevealed = job.IsCompanyRevealed,
                CompanyId = job.CompanyId,
                Status = job.Status.ToString()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditJobViewModel model)
        {
            if (!IsEmployer())
            {
                TempData["ErrorMessage"] = "Only employers can edit job postings.";
                return RedirectToAction("AccessDenied", "Account");
            }

            if (!ModelState.IsValid)
                return View(model);

            var employerId = GetCurrentUserId();
            if (!employerId.HasValue)
                return RedirectToAction("Login", "Account");

            try
            {
                var job = await _jobService.GetJobByIdAsync(model.Id);
                if (job == null)
                    return NotFound();
                if (job.EmployerId != employerId.Value)
                    return Forbid();

                job.Title = model.Title;
                job.Description = model.Description;
                job.Requirements = model.Requirements ?? string.Empty;
                job.SalaryMin = model.SalaryMin;
                job.SalaryMax = model.SalaryMax;
                job.Location = model.Location;
                job.EmploymentType = model.EmploymentType;
                job.Category = model.Category;
                job.ExperienceLevel = model.ExperienceLevel;
                job.IsCompanyRevealed = model.IsCompanyRevealed;

                await _jobService.UpdateJobAsync(job);

                TempData["SuccessMessage"] = "Job posting updated.";
                return RedirectToAction("Details", new { id = job.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
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
        public async Task<IActionResult> Duplicate(Guid id)
        {
            if (!IsEmployer())
                return Unauthorized();

            var employerId = GetCurrentUserId();
            if (!employerId.HasValue)
                return RedirectToAction("Login", "Account");

            try
            {
                var clone = await _jobService.DuplicateJobAsync(id, employerId.Value);
                TempData["SuccessMessage"] = "Job duplicated as a draft — edit and publish it when ready.";
                return RedirectToAction("Details", new { id = clone.Id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("MyJobs");
            }
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