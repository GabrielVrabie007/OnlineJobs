namespace OnlineJobs.Application.Observers
{
    public class JobPostingSubject : ISubject
    {
        private readonly List<IObserver> _observers = new();
        private readonly object _lock = new();

        public void Attach(IObserver observer)
        {
            lock (_lock)
            {
                if (!_observers.Contains(observer))
                {
                    _observers.Add(observer);
                }
            }
        }

        public void Detach(IObserver observer)
        {
            lock (_lock)
            {
                _observers.Remove(observer);
            }
        }

        public async Task NotifyAsync(object data)
        {
            List<IObserver> observersCopy;
            lock (_lock)
            {
                observersCopy = new List<IObserver>(_observers);
            }

            var tasks = observersCopy.Select(observer => observer.UpdateAsync(data));
            await Task.WhenAll(tasks);
        }
    }
}
