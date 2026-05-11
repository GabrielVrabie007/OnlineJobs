namespace OnlineJobs.Application.Observers
{
    public interface ISubject
    {
        void Attach(IObserver observer);
        void Detach(IObserver observer);
        Task NotifyAsync(object data);
    }
}
