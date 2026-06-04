namespace OnlineJobs.Application.Observers
{
    public class ApplicationStatusSubject : ISubject
    {
        private readonly List<IObserver> _observers = new();
        private readonly object _lock = new();

        // Parameterless ctor kept for unit tests that attach their own observers.
        public ApplicationStatusSubject() { }

        // DI ctor: the standard observers are attached ONCE for the lifetime of this
        // singleton subject. (Previously controllers attached fresh observers on every
        // request to a singleton subject, which leaked and multiplied notifications.)
        public ApplicationStatusSubject(
            EmailAlertObserver emailObserver,
            DashboardNotificationObserver dashboardObserver,
            AuditLogObserver auditObserver,
            StatisticsObserver statisticsObserver)
        {
            Attach(emailObserver);
            Attach(dashboardObserver);
            Attach(auditObserver);
            Attach(statisticsObserver);
        }

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
