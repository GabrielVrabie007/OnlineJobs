using Microsoft.AspNetCore.Mvc;
using OnlineJobs.Application.Interfaces;

namespace OnlineJobs.Controllers
{
    public class HomeController : Controller
    {
        private readonly IJobService _jobService;
        private readonly ICompanyService _companyService;

        public HomeController(IJobService jobService, ICompanyService companyService)
        {
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
        }

        public async Task<IActionResult> Index()
        {
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