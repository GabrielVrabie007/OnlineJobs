using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.States.ApplicationStates
{
    public class InterviewingState : IApplicationState
    {
        public string StateName => "Interviewing";

        public void StartReview(JobApplication application)
        {
            throw new InvalidOperationException("Application is already past review stage");
        }

        public void MoveToInterview(JobApplication application)
        {
            throw new InvalidOperationException("Application is already in interview stage");
        }

        public void Accept(JobApplication application)
        {
            application.Status = ApplicationStatus.Accepted;
            application.ReviewedDate = DateTime.UtcNow;
            application.TransitionToState(new AcceptedState());
        }

        public void Reject(JobApplication application, string reason)
        {
            application.Status = ApplicationStatus.Rejected;
            application.ReviewNotes = reason;
            application.ReviewedDate = DateTime.UtcNow;
            application.TransitionToState(new RejectedState());
        }

        public void Withdraw(JobApplication application)
        {
            throw new InvalidOperationException("Cannot withdraw application during interview process");
        }

        public bool CanTransitionTo(string targetState)
        {
            return targetState == "Accepted" || targetState == "Rejected";
        }
    }
}
