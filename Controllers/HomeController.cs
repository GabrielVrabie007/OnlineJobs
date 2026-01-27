using Microsoft.AspNetCore.Mvc;
using OnlineJobs.Application.Interfaces;

namespace OnlineJobs.Web.Controllers
{
    /// <summary>
    /// Home controller
    /// Demonstrates:
    /// - SRP: Single responsibility - handling home page requests
    /// - DIP: Depends on service abstractions, not concrete implementations
    /// - Thin controller - delegates business logic to services
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IJobService _jobService;
        private readonly ICompanyService _companyService;

        // Constructor injection (DIP)
        public HomeController(IJobService jobService, ICompanyService companyService)
        {
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
        }

        public async Task<IActionResult> Index()
        {
            // Get active jobs for homepage
            var activeJobs = await _jobService.GetActiveJobsAsync();
            return View(activeJobs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}