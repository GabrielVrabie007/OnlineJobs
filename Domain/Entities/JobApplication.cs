using OnlineJobs.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineJobs.Domain.Entities
{
    public class JobApplication
    {
        private string? _coverLetter;
        private object? _currentState;

        public Guid Id { get; private set; }

        public Guid JobPostingId { get; set; }
        public Guid JobSeekerId { get; set; }

        public JobPosting? JobPosting { get; set; }
        public JobSeeker? JobSeeker { get; set; }

        [NotMapped]
        public object? CurrentState
        {
            get => _currentState;
            private set => _currentState = value;
        }

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

        // New properties for enhanced application features
        public decimal? ExpectedSalary { get; set; }
        public string? PortfolioLink { get; set; }
        public DateTime? AvailableStartDate { get; set; }
        public string? AdditionalInfo { get; set; }

        // Alias for AppliedDate to match frontend expectations
        public DateTime AppliedAt => AppliedDate;

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

        /// <summary>
        /// Restores a previous status. Used by the Command pattern to undo a status
        /// change (Approve/Reject/Withdraw) — an explicit, intent-revealing revert that
        /// replaces brittle reflection on the Status property.
        /// </summary>
        public void RestoreStatus(ApplicationStatus previousStatus)
        {
            Status = previousStatus;
            // Clearing the reviewed timestamp keeps the record consistent after an undo.
            if (previousStatus == ApplicationStatus.Submitted)
            {
                ReviewedDate = null;
                ReviewNotes = null;
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

        public void TransitionToState(object newState)
        {
            CurrentState = newState;
        }

        public T Accept<T>(object visitor)
        {
            var method = visitor.GetType().GetMethod("VisitJobApplication");
            if (method != null)
            {
                var result = method.Invoke(visitor, new object[] { this });
                return result != null ? (T)result : default!;
            }
            throw new InvalidOperationException("Visitor does not support JobApplication");
        }
    }
}