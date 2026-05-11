using System.Text;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.ReportTemplates
{
    public class CSVApplicationReportTemplate : ApplicationReportTemplate
    {
        public override string ReportName => "ApplicationReport";
        public override string FileExtension => ".csv";

        public CSVApplicationReportTemplate(IRepository<JobApplication> applicationRepository)
            : base(applicationRepository)
        {
        }

        protected override ReportData<JobApplication> FormatReportData(IEnumerable<JobApplication> data)
        {
            return new ReportData<JobApplication>
            {
                Title = "Applications Report (CSV)",
                GeneratedAt = DateTime.UtcNow,
                Data = data.ToList()
            };
        }

        protected override Task<ReportResult> ExportReportAsync(ReportData<JobApplication> reportData)
        {
            var sb = new StringBuilder();

            sb.AppendLine("ApplicationId,JobPostingId,JobSeekerId,Status,AppliedDate,CoverLetterLength");

            foreach (var app in reportData.Data)
            {
                sb.AppendLine($"{app.Id},{app.JobPostingId},{app.JobSeekerId},{app.Status},{app.AppliedDate:yyyy-MM-dd},{app.CoverLetter.Length}");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"{ReportName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}{FileExtension}";

            return Task.FromResult(ReportResult.SuccessResult(fileName, bytes, "text/csv"));
        }
    }
}
