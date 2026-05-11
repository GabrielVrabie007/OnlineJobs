using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.ApprovalChains
{
    public class ManagerApprovalHandler : BaseApprovalHandler
    {
        private const decimal MaxApprovalAmount = 150000m;

        public override async Task<ApprovalResult> HandleAsync(ApprovalRequest request)
        {
            if (request.RequesterRole != UserType.Employer)
            {
                return ApprovalResult.Rejected("ManagerApproval",
                    "Only employers can approve applications");
            }

            if (request.JobSalary.HasValue && request.JobSalary.Value <= MaxApprovalAmount)
            {
                return ApprovalResult.Approved("ManagerApproval",
                    $"Approved by manager (${MaxApprovalAmount:N0} authority)");
            }

            return await PassToNextAsync(request);
        }
    }
}
