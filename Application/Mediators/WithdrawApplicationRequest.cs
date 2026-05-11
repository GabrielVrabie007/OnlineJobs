namespace OnlineJobs.Application.Mediators
{
    public class WithdrawApplicationRequest
    {
        public Guid ApplicationId { get; set; }
        public Guid UserId { get; set; }

        public WithdrawApplicationRequest(Guid applicationId, Guid userId)
        {
            ApplicationId = applicationId;
            UserId = userId;
        }
    }
}
