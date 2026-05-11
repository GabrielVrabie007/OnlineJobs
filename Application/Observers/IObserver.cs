namespace OnlineJobs.Application.Observers
{
    public interface IObserver
    {
        Task UpdateAsync(object data);
    }
}
