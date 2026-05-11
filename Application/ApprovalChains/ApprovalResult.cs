namespace OnlineJobs.Application.ApprovalChains
{
    public class ApprovalResult
    {
        public bool IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public string Reason { get; set; }
        public bool RequiresEscalation { get; set; }
        public string NextApproverLevel { get; set; }

        public ApprovalResult(bool isApproved, string approvedBy, string reason)
        {
            IsApproved = isApproved;
            ApprovedBy = approvedBy;
            Reason = reason;
            RequiresEscalation = false;
        }

        public static ApprovalResult Approved(string approvedBy, string reason = "Approved")
        {
            return new ApprovalResult(true, approvedBy, reason);
        }

        public static ApprovalResult Rejected(string rejectedBy, string reason)
        {
            return new ApprovalResult(false, rejectedBy, reason);
        }

        public static ApprovalResult Escalated(string currentLevel, string nextLevel, string reason)
        {
            return new ApprovalResult(false, currentLevel, reason)
            {
                RequiresEscalation = true,
                NextApproverLevel = nextLevel
            };
        }
    }
}
