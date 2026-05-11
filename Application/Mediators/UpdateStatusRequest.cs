using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Mediators
{
    public class UpdateStatusRequest
    {
        public Guid ApplicationId { get; set; }
        public Guid EmployerId { get; set; }
        public string Status { get; set; }
        public ApplicationStatus? TargetStatus { get; set; }

        public UpdateStatusRequest(Guid applicationId, Guid employerId, string status)
        {
            ApplicationId = applicationId;
            EmployerId = employerId;
            Status = status;
        }
    }
}
