using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Domain.Entities
{
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
        public string EmploymentType { get; set; }
        public string Category { get; set; }

        public Guid EmployerId { get; set; }
        public Guid CompanyId { get; set; }

        public Employer Employer { get; set; }
        public Company Company { get; set; }
        public List<JobApplication> Applications { get; set; }

        public JobStatus Status { get; set; }
        public DateTime PostedDate { get; private set; }
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
    }
}