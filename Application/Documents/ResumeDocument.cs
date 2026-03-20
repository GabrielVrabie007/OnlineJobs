using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Documents;


public class ResumeDocument : IDocument
{
    private readonly JobSeeker _jobSeeker;

    public string DocumentType => "Resume";

    public ResumeDocument(JobSeeker jobSeeker)
    {
        _jobSeeker = jobSeeker ?? throw new ArgumentNullException(nameof(jobSeeker));
    }

    public string Generate()
    {
        if (!Validate())
        {
            throw new InvalidOperationException("Cannot generate resume: validation failed");
        }

        var content = new System.Text.StringBuilder();
        content.AppendLine("=== RESUME ===");
        content.AppendLine();
        content.AppendLine($"Name: {_jobSeeker.GetFullName()}");
        content.AppendLine($"Email: {_jobSeeker.Email}");

        if (!string.IsNullOrEmpty(_jobSeeker.PhoneNumber))
            content.AppendLine($"Phone: {_jobSeeker.PhoneNumber}");

        if (!string.IsNullOrEmpty(_jobSeeker.Address))
            content.AppendLine($"Address: {_jobSeeker.Address}");

        if (_jobSeeker.DateOfBirth.HasValue)
            content.AppendLine($"Date of Birth: {_jobSeeker.DateOfBirth.Value:yyyy-MM-dd}");

        content.AppendLine();
        content.AppendLine("SKILLS:");
        content.AppendLine(_jobSeeker.Skills ?? "Not specified");
        content.AppendLine();
        content.AppendLine("PROFESSIONAL SUMMARY:");
        content.AppendLine(_jobSeeker.Resume ?? "Not provided");
        content.AppendLine();
        content.AppendLine($"Total Applications: {_jobSeeker.GetApplicationCount()}");
        content.AppendLine($"Member Since: {_jobSeeker.CreatedAt:yyyy-MM-dd}");
        content.AppendLine();
        content.AppendLine("=== END OF RESUME ===");

        return content.ToString();
    }

    public bool Validate()
    {
        return _jobSeeker != null &&
               _jobSeeker.IsActive &&
               !string.IsNullOrWhiteSpace(_jobSeeker.FirstName) &&
               !string.IsNullOrWhiteSpace(_jobSeeker.LastName);
    }

    public string GetFileExtension()
    {
        return ".txt";
    }
}