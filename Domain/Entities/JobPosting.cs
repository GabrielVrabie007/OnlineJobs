using OnlineJobs.Domain.Enums;
using OnlineJobs.Domain.Interfaces;

namespace OnlineJobs.Domain.Entities
{
    public class JobPosting : IPrototype<JobPosting>
    {
        private string _title;
        private string _description;

        public Guid Id { get; set; }

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
        public string EmploymentType { get; set; }
        public string Category { get; set; }

        public Guid EmployerId { get; set; }
        public Guid CompanyId { get; set; }

        public Employer Employer { get; set; }
        public Company Company { get; set; }
        public List<JobApplication> Applications { get; set; }

        public JobStatus Status { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public JobPosting(string title, string description, Guid employerId, Guid companyId)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            EmployerId = employerId;
            CompanyId = companyId;
            PostedDate = DateTime.UtcNow;
            Status = JobStatus.Draft;
            Applications = new List<JobApplication>();
        }

        public JobPosting()
        {
            Id = Guid.NewGuid();
            PostedDate = DateTime.UtcNow;
            Status = JobStatus.Draft;
            Applications = new List<JobApplication>();
        }

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

        /// <summary>
        /// Creates a deep copy of the JobPosting.
        /// Useful when employers want to create similar job postings.
        ///
        /// Deep Copy Behavior:
        /// - Creates new JobPosting instance with new ID
        /// - Copies all property values
        /// - Creates new empty Applications list (applications are not cloned)
        /// - Resets status to Draft
        /// - Sets new PostedDate
        /// - Preserves same EmployerId and CompanyId
        /// - Employer and Company navigation properties are set to null (should be loaded separately)
        /// </summary>
        /// <returns>A new JobPosting instance with copied values</returns>
        public JobPosting Clone()
        {
            var clonedJob = new JobPosting
            {
                // New identity
                Id = Guid.NewGuid(),

                // Copy job details
                Title = this.Title,
                Description = this.Description,
                Requirements = this.Requirements,
                SalaryMin = this.SalaryMin,
                SalaryMax = this.SalaryMax,
                Location = this.Location,
                EmploymentType = this.EmploymentType,
                Category = this.Category,

                // Copy employer/company references
                EmployerId = this.EmployerId,
                CompanyId = this.CompanyId,

                // Reset state for new posting
                Status = JobStatus.Draft,
                PostedDate = DateTime.UtcNow,
                ClosedDate = null,
                ExpiryDate = this.ExpiryDate, // Can copy expiry logic

                // New empty collections (deep copy - don't clone applications)
                Applications = new List<JobApplication>(),

                // Navigation properties set to null - should be loaded from repository
                Employer = null,
                Company = null
            };

            return clonedJob;
        }

        /// <summary>
        /// Creates a shallow copy of the JobPosting.
        /// Shares the same collections (Applications list).
        /// Generally not recommended - use Clone() instead for safety.
        /// </summary>
        /// <returns>A shallow copy of the JobPosting</returns>
        public JobPosting ShallowCopy()
        {
            return (JobPosting)this.MemberwiseClone();
        }

        /// <summary>
        /// Creates a clone with a new title.
        /// Convenience method for creating similar jobs with different titles.
        /// </summary>
        public JobPosting CloneWithNewTitle(string newTitle)
        {
            var clone = this.Clone();
            clone.Title = newTitle;
            return clone;
        }

        /// <summary>
        /// Creates a clone with modifications.
        /// Allows customization via action delegate.
        /// </summary>
        public JobPosting CloneWith(Action<JobPosting> modifications)
        {
            var clone = this.Clone();
            modifications?.Invoke(clone);
            return clone;
        }
    }
}