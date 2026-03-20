using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Documents;

public class ApplicationReportDocument : IDocument
{
    private readonly JobPosting _jobPosting;
    private readonly IEnumerable<JobApplication> _applications;

    public string DocumentType => "ApplicationReport";

    public ApplicationReportDocument(JobPosting jobPosting, IEnumerable<JobApplication> applications)
    {
        _jobPosting = jobPosting ?? throw new ArgumentNullException(nameof(jobPosting));
        _applications = applications ?? throw new ArgumentNullException(nameof(applications));
    }

    public string Generate()
    {
        if (!Validate())
        {
            throw new InvalidOperationException("Cannot generate application report: validation failed");
        }

        var content = new System.Text.StringBuilder();
        content.AppendLine("=== APPLICATION REPORT ===");
        content.AppendLine();
        content.AppendLine($"Job: {_jobPosting.Title}");
        content.AppendLine($"Company: {_jobPosting.Company?.Name ?? "Not specified"}");
        content.AppendLine($"Report Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        content.AppendLine();
        content.AppendLine("=== SUMMARY ===");
        content.AppendLine($"Total Applications: {_applications.Count()}");

        var groupedByStatus = _applications.GroupBy(a => a.Status);
        foreach (var group in groupedByStatus)
        {
            content.AppendLine($"  - {group.Key}: {group.Count()}");
        }

        content.AppendLine();
        content.AppendLine("=== APPLICATION DETAILS ===");
        content.AppendLine();

        var counter = 1;
        foreach (var application in _applications.OrderByDescending(a => a.AppliedDate))
        {
            content.AppendLine($"[{counter}] Application ID: {application.Id}");
            content.AppendLine($"    Applicant: {application.JobSeeker?.GetFullName() ?? "Unknown"}");
            content.AppendLine($"    Email: {application.JobSeeker?.Email ?? "N/A"}");
            content.AppendLine($"    Applied: {application.AppliedDate:yyyy-MM-dd}");
            content.AppendLine($"    Status: {application.Status}");
            content.AppendLine($"    Days Since Application: {application.GetDaysSinceApplication()}");

            if (application.ReviewedDate.HasValue)
                content.AppendLine($"    Reviewed: {application.ReviewedDate.Value:yyyy-MM-dd}");

            if (!string.IsNullOrEmpty(application.ReviewNotes))
                content.AppendLine($"    Notes: {application.ReviewNotes}");

            content.AppendLine();
            counter++;
        }

        content.AppendLine("=== END OF REPORT ===");

        return content.ToString();
    }

    public bool Validate()
    {
        return _jobPosting != null &&
               _applications != null &&
               !string.IsNullOrWhiteSpace(_jobPosting.Title);
    }

    public string GetFileExtension()
    {
        return ".txt";
    }
}