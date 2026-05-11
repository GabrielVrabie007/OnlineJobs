using System.Text.Json;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Visitors
{
    public class ExportVisitor : IApplicationVisitor<string>
    {
        private readonly string _format;

        public ExportVisitor(string format = "json")
        {
            _format = format.ToLower();
        }

        public string VisitJobApplication(JobApplication application)
        {
            return _format switch
            {
                "json" => ExportAsJson(application),
                "xml" => ExportAsXml(application),
                "csv" => ExportAsCsv(application),
                _ => ExportAsJson(application)
            };
        }

        public string VisitJobPosting(JobPosting jobPosting)
        {
            return _format switch
            {
                "json" => ExportAsJson(jobPosting),
                "xml" => ExportAsXml(jobPosting),
                "csv" => ExportAsCsv(jobPosting),
                _ => ExportAsJson(jobPosting)
            };
        }

        private string ExportAsJson(JobApplication application)
        {
            var obj = new
            {
                id = application.Id,
                jobPostingId = application.JobPostingId,
                jobSeekerId = application.JobSeekerId,
                status = application.Status.ToString(),
                appliedDate = application.AppliedDate,
                coverLetterLength = application.CoverLetter.Length
            };
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
        }

        private string ExportAsJson(JobPosting jobPosting)
        {
            var obj = new
            {
                id = jobPosting.Id,
                title = jobPosting.Title,
                description = jobPosting.Description,
                location = jobPosting.Location,
                salaryRange = jobPosting.GetSalaryRange(),
                status = jobPosting.Status.ToString()
            };
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
        }

        private string ExportAsXml(JobApplication application)
        {
            return $@"<JobApplication>
    <Id>{application.Id}</Id>
    <JobPostingId>{application.JobPostingId}</JobPostingId>
    <JobSeekerId>{application.JobSeekerId}</JobSeekerId>
    <Status>{application.Status}</Status>
    <AppliedDate>{application.AppliedDate:yyyy-MM-dd}</AppliedDate>
</JobApplication>";
        }

        private string ExportAsXml(JobPosting jobPosting)
        {
            return $@"<JobPosting>
    <Id>{jobPosting.Id}</Id>
    <Title>{jobPosting.Title}</Title>
    <Location>{jobPosting.Location}</Location>
    <Status>{jobPosting.Status}</Status>
</JobPosting>";
        }

        private string ExportAsCsv(JobApplication application)
        {
            return $"{application.Id},{application.JobPostingId},{application.JobSeekerId},{application.Status},{application.AppliedDate:yyyy-MM-dd}";
        }

        private string ExportAsCsv(JobPosting jobPosting)
        {
            return $"{jobPosting.Id},{jobPosting.Title},{jobPosting.Location},{jobPosting.Status}";
        }
    }
}
