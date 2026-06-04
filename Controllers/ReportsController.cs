using Microsoft.AspNetCore.Mvc;
using OnlineJobs.Application.Services;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ReportingService _reportingService;

        public ReportsController(ReportingService reportingService)
        {
            _reportingService = reportingService;
        }

        public IActionResult Index()
        {
            if (!IsEmployer())
            {
                TempData["ErrorMessage"] = "Only employers can access reports.";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Generate(string reportType)
        {
            if (!IsEmployer())
                return Unauthorized();

            try
            {
                // Build the data once and preview it on screen; downloads render the
                // same data into a real file (Bridge pattern).
                var document = await _reportingService.BuildAsync(reportType, GetCurrentUserId());
                ViewBag.Document = document;
                ViewBag.ReportType = reportType;
                return View("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Could not generate report: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Download(string reportType, string format)
        {
            if (!IsEmployer())
                return Unauthorized();

            try
            {
                var file = await _reportingService.ExportAsync(reportType, format, GetCurrentUserId());
                return File(file.Content, file.ContentType, file.FileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Could not download report: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        private bool IsEmployer()
        {
            var userType = HttpContext.Session.GetString("UserType");
            return userType == UserType.Employer.ToString();
        }

        private Guid? GetCurrentUserId()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (Guid.TryParse(userIdString, out var userId))
                return userId;
            return null;
        }
    }
}
