using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.ApprovalChains
{
    public class ApprovalRequest
    {
        public Guid ApplicationId { get; set; }
        public Guid JobPostingId { get; set; }
        public Guid RequesterId { get; set; }
        public ApplicationStatus TargetStatus { get; set; }
        public decimal? JobSalary { get; set; }
        public UserType RequesterRole { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        public ApprovalRequest(Guid applicationId, Guid jobPostingId, Guid requesterId,
            ApplicationStatus targetStatus, UserType requesterRole)
        {
            ApplicationId = applicationId;
            JobPostingId = jobPostingId;
            RequesterId = requesterId;
            TargetStatus = targetStatus;
            RequesterRole = requesterRole;
            Metadata = new Dictionary<string, object>();
        }
    }
}
