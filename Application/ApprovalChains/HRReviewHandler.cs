using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.ApprovalChains
{
    public class HRReviewHandler : BaseApprovalHandler
    {
        private const decimal MaxApprovalAmount = 80000m;

        public override async Task<ApprovalResult> HandleAsync(ApprovalRequest request)
        {
            if (request.RequesterRole != UserType.Employer)
            {
                return ApprovalResult.Rejected("HRReview", "Only employers can approve applications");
            }

            if (request.TargetStatus == ApplicationStatus.Rejected)
            {
                return ApprovalResult.Approved("HRReview", "Rejection approved");
            }

            if (request.JobSalary.HasValue && request.JobSalary.Value <= MaxApprovalAmount)
            {
                return ApprovalResult.Approved("HRReview",
                    $"Approved within HR authority (${MaxApprovalAmount:N0})");
            }

            return await PassToNextAsync(request);
        }
    }
}
