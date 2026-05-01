using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Reporting;
using OnlineJobs.Application.Reporting.Exporters;
using OnlineJobs.Application.Reporting.Reports;

namespace OnlineJobs.Application.Services
{

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

        public async Task<string> GenerateJobReportAsync(string format)
        {
            var exporter = GetExporter(format);
            var report = new JobReport(exporter, _jobService);
            return await report.ExportAsync();
        }

        public async Task<string> GenerateApplicationReportAsync(string format, Guid? employerId = null)
        {
            var exporter = GetExporter(format);
            var report = new ApplicationReport(exporter, _applicationService, employerId);
            return await report.ExportAsync();
        }

        public async Task<string> GenerateCompanyReportAsync(string format)
        {
            var exporter = GetExporter(format);
            var report = new CompanyReport(exporter, _companyService);
            return await report.ExportAsync();
        }

        private IReportExporter GetExporter(string format)
        {
            return format.ToUpperInvariant() switch
            {
                "PDF" => new PDFExporter(),
                "EXCEL" or "XLSX" => new ExcelExporter(),
                "JSON" => new JSONExporter(),
                "CSV" => new CSVExporter(),
                _ => throw new ArgumentException($"Unsupported format: {format}")
            };
        }
    }
}
