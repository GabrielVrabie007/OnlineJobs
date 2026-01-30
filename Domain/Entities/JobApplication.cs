using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Domain.Entities
{
    public class JobApplication
    {
        private string? _coverLetter;

        public Guid Id { get; private set; }

        public Guid JobPostingId { get; set; }
        public Guid JobSeekerId { get; set; }

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

        public JobApplication(Guid jobPostingId, Guid jobSeekerId, string coverLetter)
        {
            Id = Guid.NewGuid();
            JobPostingId = jobPostingId;
            JobSeekerId = jobSeekerId;
            CoverLetter = coverLetter;
            AppliedDate = DateTime.UtcNow;
            Status = ApplicationStatus.Submitted;
        }

        public JobApplication()
        {
            Id = Guid.NewGuid();
            AppliedDate = DateTime.UtcNow;
            Status = ApplicationStatus.Submitted;
        }

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