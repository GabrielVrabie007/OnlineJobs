using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Documents;


public class CoverLetterDocument : IDocument
{
    private readonly JobSeeker _jobSeeker;
    private readonly JobPosting _jobPosting;
    private readonly string _customMessage;

    public string DocumentType => "CoverLetter";

    public CoverLetterDocument(JobSeeker jobSeeker, JobPosting jobPosting, string customMessage = null)
    {
        _jobSeeker = jobSeeker ?? throw new ArgumentNullException(nameof(jobSeeker));
        _jobPosting = jobPosting ?? throw new ArgumentNullException(nameof(jobPosting));
        _customMessage = customMessage;
    }

    public string Generate()
    {
        if (!Validate())
        {
            throw new InvalidOperationException("Cannot generate cover letter: validation failed");
        }

        var content = new System.Text.StringBuilder();
        content.AppendLine("=== COVER LETTER ===");
        content.AppendLine();
        content.AppendLine($"Date: {DateTime.UtcNow:yyyy-MM-dd}");
        content.AppendLine();
        content.AppendLine($"From: {_jobSeeker.GetFullName()}");
        content.AppendLine($"Email: {_jobSeeker.Email}");
        content.AppendLine();
        content.AppendLine($"To: {_jobPosting.Company?.Name ?? "Hiring Manager"}");
        content.AppendLine($"Position: {_jobPosting.Title}");
        content.AppendLine();
        content.AppendLine("Dear Hiring Manager,");
        content.AppendLine();

        if (!string.IsNullOrEmpty(_customMessage))
        {
            content.AppendLine(_customMessage);
        }
        else
        {
            content.AppendLine($"I am writing to express my strong interest in the {_jobPosting.Title} position at your company.");
            content.AppendLine();
            content.AppendLine("With my skills and experience, I believe I would be a valuable addition to your team.");
            content.AppendLine();
            content.AppendLine("My relevant skills include:");
            content.AppendLine(_jobSeeker.Skills ?? "Various professional skills");
        }

        content.AppendLine();
        content.AppendLine("I look forward to the opportunity to discuss how I can contribute to your organization.");
        content.AppendLine();
        content.AppendLine("Sincerely,");
        content.AppendLine(_jobSeeker.GetFullName());
        content.AppendLine();
        content.AppendLine("=== END OF COVER LETTER ===");

        return content.ToString();
    }

    public bool Validate()
    {
        return _jobSeeker != null &&
               _jobSeeker.IsActive &&
               _jobPosting != null &&
               !string.IsNullOrWhiteSpace(_jobSeeker.FirstName) &&
               !string.IsNullOrWhiteSpace(_jobSeeker.LastName) &&
               !string.IsNullOrWhiteSpace(_jobPosting.Title);
    }

    public string GetFileExtension()
    {
        return ".txt";
    }
}