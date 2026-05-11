using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.States.ApplicationStates
{
    public class AcceptedState : IApplicationState
    {
        public string StateName => "Accepted";

        public void StartReview(JobApplication application)
        {
            throw new InvalidOperationException("Cannot review an accepted application");
        }

        public void MoveToInterview(JobApplication application)
        {
            throw new InvalidOperationException("Cannot interview an accepted application");
        }

        public void Accept(JobApplication application)
        {
            throw new InvalidOperationException("Application is already accepted");
        }

        public void Reject(JobApplication application, string reason)
        {
            throw new InvalidOperationException("Cannot reject an accepted application");
        }

        public void Withdraw(JobApplication application)
        {
            throw new InvalidOperationException("Cannot withdraw an accepted application");
        }

        public bool CanTransitionTo(string targetState)
        {
            return false;
        }
    }
}
