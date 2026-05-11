namespace OnlineJobs.Application.ApprovalChains
{
    public interface IApprovalHandler
    {
        IApprovalHandler SetNext(IApprovalHandler next);
        Task<ApprovalResult> HandleAsync(ApprovalRequest request);
    }
}
