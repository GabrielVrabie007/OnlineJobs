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

        // Observers are attached once to the singleton subjects via DI, so the mediator
        // simply triggers a notification rather than re-attaching observers each call.
        public async Task NotifyApplicationStatusChangeAsync(object application)
        {
            await _statusSubject.NotifyAsync(application);
        }

        public async Task NotifyJobPostingAsync(object jobPosting)
        {
            await _jobPostingSubject.NotifyAsync(jobPosting);
        }
    }
}
