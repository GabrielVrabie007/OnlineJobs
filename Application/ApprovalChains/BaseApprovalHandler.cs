namespace OnlineJobs.Application.ApprovalChains
{
    public abstract class BaseApprovalHandler : IApprovalHandler
    {
        private IApprovalHandler? _next;

        public IApprovalHandler SetNext(IApprovalHandler next)
        {
            _next = next;
            return next;
        }

        public abstract Task<ApprovalResult> HandleAsync(ApprovalRequest request);

        protected async Task<ApprovalResult> PassToNextAsync(ApprovalRequest request)
        {
            if (_next != null)
            {
                return await _next.HandleAsync(request);
            }

            return ApprovalResult.Rejected("System",
                "Request could not be approved at any level");
        }
    }
}
