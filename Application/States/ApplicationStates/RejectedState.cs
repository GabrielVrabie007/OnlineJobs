using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.States.ApplicationStates
{
    public class RejectedState : IApplicationState
    {
        public string StateName => "Rejected";

        public void StartReview(JobApplication application)
        {
            throw new InvalidOperationException("Cannot review a rejected application");
        }

        public void MoveToInterview(JobApplication application)
        {
            throw new InvalidOperationException("Cannot interview a rejected application");
        }

        public void Accept(JobApplication application)
        {
            throw new InvalidOperationException("Cannot accept a rejected application");
        }

        public void Reject(JobApplication application, string reason)
        {
            throw new InvalidOperationException("Application is already rejected");
        }

        public void Withdraw(JobApplication application)
        {
            throw new InvalidOperationException("Cannot withdraw a rejected application");
        }

        public bool CanTransitionTo(string targetState)
        {
            return false;
        }
    }
}
