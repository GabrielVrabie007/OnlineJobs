using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Reporting;
using OnlineJobs.Application.Reporting.Exporters;
using OnlineJobs.Application.Reporting.Reports;

namespace OnlineJobs.Application.Services
{
    /// <summary>
    /// Entry point for reports. Bridge pattern: a report type (abstraction) is paired
    /// with an export format (implementor). Build once to preview on screen, or export
    /// to a real downloadable file.
    /// </summary>
    public class ReportingService
    {
        private readonly IJobService _jobService;
        private readonly IApplicationService _applicationService;
        private readonly ICompanyService _companyService;

        public ReportingService(
            IJobService jobService,
            IApplicationService applicationService,
            ICompanyService companyService)
        {
            _jobService = jobService;
            _applicationService = applicationService;
            _companyService = companyService;
        }

        /// <summary>Builds the report data (for the on-screen preview).</summary>
        public Task<ReportDocument> BuildAsync(string reportType, Guid? employerId = null)
            => CreateReport(reportType, new CSVExporter(), employerId).BuildAsync();

        /// <summary>Builds and renders the report into a downloadable file.</summary>
        public Task<ExportedFile> ExportAsync(string reportType, string format, Guid? employerId = null)
            => CreateReport(reportType, GetExporter(format), employerId).ExportAsync();

        private IReport CreateReport(string reportType, IReportExporter exporter, Guid? employerId) => reportType switch
        {
            "jobs" => new JobReport(exporter, _jobService),
            "applications" => new ApplicationReport(exporter, _applicationService, employerId),
            "companies" => new CompanyReport(exporter, _companyService),
            _ => throw new ArgumentException($"Invalid report type: {reportType}")
        };

        private IReportExporter GetExporter(string format) => format.ToUpperInvariant() switch
        {
            "PDF" => new PDFExporter(),
            "EXCEL" or "XLSX" => new ExcelExporter(),
            "JSON" => new JSONExporter(),
            "CSV" => new CSVExporter(),
            _ => throw new ArgumentException($"Unsupported format: {format}")
        };
    }
}
