using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.States.ApplicationStates
{
    public class UnderReviewState : IApplicationState
    {
        public string StateName => "UnderReview";

        public void StartReview(JobApplication application)
        {
            throw new InvalidOperationException("Application is already under review");
        }

        public void MoveToInterview(JobApplication application)
        {
            application.Status = ApplicationStatus.Interviewing;
            application.TransitionToState(new InterviewingState());
        }

        public void Accept(JobApplication application)
        {
            throw new InvalidOperationException("Application must go through interview before acceptance");
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
            return targetState == "Interviewing" || targetState == "Rejected" || targetState == "Withdrawn";
        }
    }
}
