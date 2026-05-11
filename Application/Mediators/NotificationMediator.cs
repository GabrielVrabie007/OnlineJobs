using OnlineJobs.Application.Observers;

namespace OnlineJobs.Application.Mediators
{
    public class NotificationMediator
    {
        private readonly ApplicationStatusSubject _statusSubject;
        private readonly JobPostingSubject _jobPostingSubject;

        public NotificationMediator(
            ApplicationStatusSubject statusSubject,
            JobPostingSubject jobPostingSubject)
        {
            _statusSubject = statusSubject;
            _jobPostingSubject = jobPostingSubject;
        }

        public async Task NotifyApplicationStatusChangeAsync(object application)
        {
            var emailObserver = new EmailAlertObserver();
            var dashboardObserver = new DashboardNotificationObserver();
            var auditObserver = new AuditLogObserver();

            _statusSubject.Attach(emailObserver);
            _statusSubject.Attach(dashboardObserver);
            _statusSubject.Attach(auditObserver);

            await _statusSubject.NotifyAsync(application);
        }

        public async Task NotifyJobPostingAsync(object jobPosting)
        {
            var emailObserver = new EmailAlertObserver();
            var dashboardObserver = new DashboardNotificationObserver();
            var statisticsObserver = new StatisticsObserver();

            _jobPostingSubject.Attach(emailObserver);
            _jobPostingSubject.Attach(dashboardObserver);
            _jobPostingSubject.Attach(statisticsObserver);

            await _jobPostingSubject.NotifyAsync(jobPosting);
        }
    }
}
