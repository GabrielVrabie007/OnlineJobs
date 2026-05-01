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
        public async Task<IActionResult> Generate(string reportType, string format)
        {
            if (!IsEmployer())
            {
                return Unauthorized();
            }

            try
            {
                string reportContent = reportType switch
                {
                    "jobs" => await _reportingService.GenerateJobReportAsync(format),
                    "applications" => await _reportingService.GenerateApplicationReportAsync(format, GetCurrentUserId()),
                    "companies" => await _reportingService.GenerateCompanyReportAsync(format),
                    _ => throw new ArgumentException("Invalid report type")
                };

                ViewBag.ReportContent = reportContent;
                ViewBag.ReportType = reportType;
                ViewBag.Format = format;
                ViewBag.GeneratedAt = DateTime.Now;

                return View("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error generating report: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Download(string reportType, string format)
        {
            if (!IsEmployer())
            {
                return Unauthorized();
            }

            try
            {
                string reportContent = reportType switch
                {
                    "jobs" => await _reportingService.GenerateJobReportAsync(format),
                    "applications" => await _reportingService.GenerateApplicationReportAsync(format, GetCurrentUserId()),
                    "companies" => await _reportingService.GenerateCompanyReportAsync(format),
                    _ => throw new ArgumentException("Invalid report type")
                };

                var contentType = format.ToUpperInvariant() switch
                {
                    "PDF" => "application/pdf",
                    "EXCEL" or "XLSX" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "JSON" => "application/json",
                    "CSV" => "text/csv",
                    _ => "text/plain"
                };

                var fileName = $"{reportType}_report_{DateTime.Now:yyyyMMdd_HHmmss}.{format.ToLower()}";

                return File(System.Text.Encoding.UTF8.GetBytes(reportContent), contentType, fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error downloading report: {ex.Message}";
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
