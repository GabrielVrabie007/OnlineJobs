using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Domain.Entities
{
    /// <summary>
    /// JobApplication entity
    /// Demonstrates:
    /// - ENCAPSULATION: Private fields with proper access control
    /// - SRP: Single responsibility - managing application lifecycle
    /// - State management through business methods
    /// </summary>
    public class JobApplication
    {
        private string? _coverLetter;

        public Guid Id { get; private set; }

        // Foreign keys
        public Guid JobPostingId { get; set; }
        public Guid JobSeekerId { get; set; }

        // Navigation properties
        public JobPosting? JobPosting { get; set; }
        public JobSeeker? JobSeeker { get; set; }

        public string CoverLetter
        {
            get => _coverLetter ?? string.Empty;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Cover letter cannot be empty");
                _coverLetter = value;
            }
        }

        public string? ResumeUrl { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateTime AppliedDate { get; private set; }
        public DateTime? ReviewedDate { get; set; }
        public string? ReviewNotes { get; set; }

        // Constructor
        public JobApplication(Guid jobPostingId, Guid jobSeekerId, string coverLetter)
        {
            Id = Guid.NewGuid();
            JobPostingId = jobPostingId;
            JobSeekerId = jobSeekerId;
            CoverLetter = coverLetter; // Uses property setter with validation
            AppliedDate = DateTime.UtcNow;
            Status = ApplicationStatus.Submitted;
        }

        // Parameterless constructor
        public JobApplication()
        {
            Id = Guid.NewGuid();
            AppliedDate = DateTime.UtcNow;
            Status = ApplicationStatus.Submitted;
        }

        // Business methods for state transitions
        public void StartReview()
        {
            if (Status == ApplicationStatus.Submitted)
            {
                Status = ApplicationStatus.UnderReview;
                ReviewedDate = DateTime.UtcNow;
            }
        }

        public void MoveToInterview()
        {
            if (Status == ApplicationStatus.UnderReview)
            {
                Status = ApplicationStatus.Interviewing;
            }
        }

        public void Accept()
        {
            Status = ApplicationStatus.Accepted;
        }

        public void Reject(string reason = null)
        {
            Status = ApplicationStatus.Rejected;
            if (!string.IsNullOrWhiteSpace(reason))
            {
                ReviewNotes = reason;
            }
        }

        public void Withdraw()
        {
            if (Status != ApplicationStatus.Accepted && Status != ApplicationStatus.Rejected)
            {
                Status = ApplicationStatus.Withdrawn;
            }
        }

        public bool CanBeWithdrawn()
        {
            return Status != ApplicationStatus.Accepted &&
                   Status != ApplicationStatus.Rejected &&
                   Status != ApplicationStatus.Withdrawn;
        }

        public bool IsInFinalState()
        {
            return Status == ApplicationStatus.Accepted ||
                   Status == ApplicationStatus.Rejected ||
                   Status == ApplicationStatus.Withdrawn;
        }

        public int GetDaysSinceApplication()
        {
            return (DateTime.UtcNow - AppliedDate).Days;
        }
    }
}