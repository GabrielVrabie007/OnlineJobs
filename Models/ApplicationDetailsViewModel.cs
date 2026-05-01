using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Models
{

    public class ApplicationDetailsViewModel
    {
        // Application properties
        public Guid Id { get; set; }
        public Guid JobSeekerId { get; set; }
        public Guid JobPostingId { get; set; }
        public string CoverLetter { get; set; } = string.Empty;
        public ApplicationStatus Status { get; set; }
        public DateTime AppliedAt { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public string? PortfolioLink { get; set; }
        public DateTime? AvailableStartDate { get; set; }
        public string? AdditionalInfo { get; set; }
        public string? ResumeUrl { get; set; }

        // Job properties
        public string JobTitle { get; set; } = string.Empty;
        public string JobLocation { get; set; } = string.Empty;
        public string JobEmploymentType { get; set; } = string.Empty;
        public string JobCategory { get; set; } = string.Empty;
        public decimal? JobSalaryMin { get; set; }
        public decimal? JobSalaryMax { get; set; }

        // Company properties
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyDescription { get; set; }
        public string? CompanyWebsite { get; set; }

        /// <summary>
        /// Creates a ViewModel from domain entities
        /// </summary>
        public static ApplicationDetailsViewModel FromEntities(
            JobApplication application,
            JobPosting? job,
            Company? company)
        {
            return new ApplicationDetailsViewModel
            {
                // Application data
                Id = application.Id,
                JobSeekerId = application.JobSeekerId,
                JobPostingId = application.JobPostingId,
                CoverLetter = application.CoverLetter,
                Status = application.Status,
                AppliedAt = application.AppliedAt,
                ExpectedSalary = application.ExpectedSalary,
                PortfolioLink = application.PortfolioLink,
                AvailableStartDate = application.AvailableStartDate,
                AdditionalInfo = application.AdditionalInfo,
                ResumeUrl = application.ResumeUrl,

                // Job data
                JobTitle = job?.Title ?? "Unknown Position",
                JobLocation = job?.Location ?? "Not specified",
                JobEmploymentType = job?.EmploymentType ?? "Not specified",
                JobCategory = job?.Category ?? "Not specified",
                JobSalaryMin = job?.SalaryMin,
                JobSalaryMax = job?.SalaryMax,

                // Company data
                CompanyName = company?.Name ?? "Unknown Company",
                CompanyDescription = company?.Description,
                CompanyWebsite = company?.Website
            };
        }
    }
}
