using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.States.ApplicationStates
{
    public class SubmittedState : IApplicationState
    {
        public string StateName => "Submitted";

        public void StartReview(JobApplication application)
        {
            application.Status = ApplicationStatus.UnderReview;
            application.ReviewedDate = DateTime.UtcNow;
            application.TransitionToState(new UnderReviewState());
        }

        public void MoveToInterview(JobApplication application)
        {
            throw new InvalidOperationException("Application must be reviewed before scheduling interview");
        }

        public void Accept(JobApplication application)
        {
            throw new InvalidOperationException("Application must be reviewed and interviewed before acceptance");
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
            application.Status = ApplicationStatus.Withdrawn;
            application.TransitionToState(new WithdrawnState());
        }

        public bool CanTransitionTo(string targetState)
        {
            return targetState == "UnderReview" || targetState == "Rejected" || targetState == "Withdrawn";
        }
    }
}
