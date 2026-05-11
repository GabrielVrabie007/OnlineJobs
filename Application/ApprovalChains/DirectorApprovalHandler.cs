using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.ApprovalChains
{
    public class DirectorApprovalHandler : BaseApprovalHandler
    {
        private const decimal MaxApprovalAmount = 500000m;

        public override async Task<ApprovalResult> HandleAsync(ApprovalRequest request)
        {
            if (request.RequesterRole != UserType.Employer)
            {
                return ApprovalResult.Rejected("DirectorApproval",
                    "Only employers can approve applications");
            }

            if (request.JobSalary.HasValue && request.JobSalary.Value <= MaxApprovalAmount)
            {
                return ApprovalResult.Approved("DirectorApproval",
                    $"Approved by director (executive authority)");
            }

            return ApprovalResult.Rejected("DirectorApproval",
                $"Salary exceeds director authority (max ${MaxApprovalAmount:N0})");
        }
    }
}
