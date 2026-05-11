using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.States.ApplicationStates
{
    public interface IApplicationState
    {
        string StateName { get; }
        void StartReview(JobApplication application);
        void MoveToInterview(JobApplication application);
        void Accept(JobApplication application);
        void Reject(JobApplication application, string reason);
        void Withdraw(JobApplication application);
        bool CanTransitionTo(string targetState);
    }
}
