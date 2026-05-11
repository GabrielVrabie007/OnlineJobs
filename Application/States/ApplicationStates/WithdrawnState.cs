using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.States.ApplicationStates
{
    public class WithdrawnState : IApplicationState
    {
        public string StateName => "Withdrawn";

        public void StartReview(JobApplication application)
        {
            throw new InvalidOperationException("Cannot review a withdrawn application");
        }

        public void MoveToInterview(JobApplication application)
        {
            throw new InvalidOperationException("Cannot interview a withdrawn application");
        }

        public void Accept(JobApplication application)
        {
            throw new InvalidOperationException("Cannot accept a withdrawn application");
        }

        public void Reject(JobApplication application, string reason)
        {
            throw new InvalidOperationException("Cannot reject a withdrawn application");
        }

        public void Withdraw(JobApplication application)
        {
            throw new InvalidOperationException("Application is already withdrawn");
        }

        public bool CanTransitionTo(string targetState)
        {
            return false;
        }
    }
}
