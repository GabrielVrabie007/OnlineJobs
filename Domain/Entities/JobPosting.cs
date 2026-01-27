using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Domain.Entities
{
    /// <summary>
    /// JobPosting entity
    /// Demonstrates:
    /// - ENCAPSULATION: Private fields with validated properties
    /// - SRP: Single responsibility - managing job posting information
    /// - Proper state management through methods
    /// </summary>
    public class JobPosting
    {
        private string _title;
        private string _description;

        public Guid Id { get; private set; }

        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Job title cannot be empty");
                if (value.Length < 5)
                    throw new ArgumentException("Job title must be at least 5 characters");
                _title = value;
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Job description cannot be empty");
                _description = value;
            }
        }

        public string Requirements { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string Location { get; set; }
        public string EmploymentType { get; set; } // Full-time, Part-time, Contract, etc.
        public string Category { get; set; } // IT, Marketing, Sales, etc.

        // Foreign keys
        public Guid EmployerId { get; set; }
        public Guid CompanyId { get; set; }

        // Navigation properties
        public Employer Employer { get; set; }
        public Company Company { get; set; }
        public List<JobApplication> Applications { get; set; }

        // Status and timestamps
        public JobStatus Status { get; set; }
        public DateTime PostedDate { get; private set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        // Constructor
        public JobPosting(string title, string description, Guid employerId, Guid companyId)
        {
            Id = Guid.NewGuid();
            Title = title; // Uses property setter with validation
            Description = description;
            EmployerId = employerId;
            CompanyId = companyId;
            PostedDate = DateTime.UtcNow;
            Status = JobStatus.Draft;
            Applications = new List<JobApplication>();
        }

        // Parameterless constructor
        public JobPosting()
        {
            Id = Guid.NewGuid();
            PostedDate = DateTime.UtcNow;
            Status = JobStatus.Draft;
            Applications = new List<JobApplication>();
        }

        // Business methods for state management
        public void Publish()
        {
            if (Status == JobStatus.Draft)
            {
                Status = JobStatus.Active;
            }
        }

        public void Close()
        {
            if (Status == JobStatus.Active)
            {
                Status = JobStatus.Closed;
                ClosedDate = DateTime.UtcNow;
            }
        }

        public void Cancel()
        {
            Status = JobStatus.Cancelled;
        }

        public bool IsAcceptingApplications()
        {
            return Status == JobStatus.Active &&
                   (ExpiryDate == null || ExpiryDate > DateTime.UtcNow);
        }

        public int GetApplicationCount()
        {
            return Applications?.Count ?? 0;
        }

        public string GetSalaryRange()
        {
            if (SalaryMin.HasValue && SalaryMax.HasValue)
                return $"${SalaryMin:N0} - ${SalaryMax:N0}";
            if (SalaryMin.HasValue)
                return $"From ${SalaryMin:N0}";
            if (SalaryMax.HasValue)
                return $"Up to ${SalaryMax:N0}";
            return "Salary not disclosed";
        }
    }
}