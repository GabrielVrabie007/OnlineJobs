using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Documents;

public class JobPostingDocument : IDocument
{
    private readonly JobPosting _jobPosting;

    public string DocumentType => "JobPosting";

    public JobPostingDocument(JobPosting jobPosting)
    {
        _jobPosting = jobPosting ?? throw new ArgumentNullException(nameof(jobPosting));
    }

    public string Generate()
    {
        if (!Validate())
        {
            throw new InvalidOperationException("Cannot generate job posting: validation failed");
        }

        var content = new System.Text.StringBuilder();
        content.AppendLine("=== JOB POSTING ===");
        content.AppendLine();
        content.AppendLine($"Job ID: {_jobPosting.Id}");
        content.AppendLine($"Title: {_jobPosting.Title}");
        content.AppendLine($"Company: {_jobPosting.Company?.Name ?? "Not specified"}");
        content.AppendLine($"Status: {_jobPosting.Status}");
        content.AppendLine();

        if (!string.IsNullOrEmpty(_jobPosting.Location))
            content.AppendLine($"Location: {_jobPosting.Location}");

        if (!string.IsNullOrEmpty(_jobPosting.EmploymentType))
            content.AppendLine($"Employment Type: {_jobPosting.EmploymentType}");

        if (!string.IsNullOrEmpty(_jobPosting.Category))
            content.AppendLine($"Category: {_jobPosting.Category}");

        content.AppendLine($"Salary Range: {_jobPosting.GetSalaryRange()}");
        content.AppendLine();
        content.AppendLine("DESCRIPTION:");
        content.AppendLine(_jobPosting.Description);
        content.AppendLine();

        if (!string.IsNullOrEmpty(_jobPosting.Requirements))
        {
            content.AppendLine("REQUIREMENTS:");
            content.AppendLine(_jobPosting.Requirements);
            content.AppendLine();
        }

        content.AppendLine($"Posted Date: {_jobPosting.PostedDate:yyyy-MM-dd}");

        if (_jobPosting.ExpiryDate.HasValue)
            content.AppendLine($"Expires: {_jobPosting.ExpiryDate.Value:yyyy-MM-dd}");

        content.AppendLine($"Total Applications: {_jobPosting.GetApplicationCount()}");
        content.AppendLine();
        content.AppendLine("=== END OF JOB POSTING ===");

        return content.ToString();
    }

    public bool Validate()
    {
        return _jobPosting != null &&
               !string.IsNullOrWhiteSpace(_jobPosting.Title) &&
               !string.IsNullOrWhiteSpace(_jobPosting.Description);
    }

    public string GetFileExtension()
    {
        return ".txt";
    }
}