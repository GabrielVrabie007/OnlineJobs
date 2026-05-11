namespace OnlineJobs.Application.Mediators
{
    public interface IMediator<TRequest, TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request);
    }
}
