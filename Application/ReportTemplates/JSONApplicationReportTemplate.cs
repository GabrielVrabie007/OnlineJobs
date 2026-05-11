using System.Text;
using System.Text.Json;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.ReportTemplates
{
    public class JSONApplicationReportTemplate : ApplicationReportTemplate
    {
        public override string ReportName => "ApplicationReport";
        public override string FileExtension => ".json";

        public JSONApplicationReportTemplate(IRepository<JobApplication> applicationRepository)
            : base(applicationRepository)
        {
        }

        protected override ReportData<JobApplication> FormatReportData(IEnumerable<JobApplication> data)
        {
            return new ReportData<JobApplication>
            {
                Title = "Applications Report (JSON)",
                GeneratedAt = DateTime.UtcNow,
                Data = data.ToList()
            };
        }

        protected override Task<ReportResult> ExportReportAsync(ReportData<JobApplication> reportData)
        {
            var reportObject = new
            {
                title = reportData.Title,
                generatedAt = reportData.GeneratedAt,
                totalApplications = reportData.Data.Count,
                applications = reportData.Data.Select(a => new
                {
                    id = a.Id,
                    jobPostingId = a.JobPostingId,
                    jobSeekerId = a.JobSeekerId,
                    status = a.Status.ToString(),
                    appliedDate = a.AppliedDate,
                    coverLetterLength = a.CoverLetter.Length
                })
            };

            var json = JsonSerializer.Serialize(reportObject, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var bytes = Encoding.UTF8.GetBytes(json);
            var fileName = $"{ReportName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}{FileExtension}";

            return Task.FromResult(ReportResult.SuccessResult(fileName, bytes, "application/json"));
        }
    }
}
