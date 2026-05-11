using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.States.ApplicationStates
{
    public static class ApplicationStateContext
    {
        public static IApplicationState GetStateFromStatus(ApplicationStatus status)
        {
            return status switch
            {
                ApplicationStatus.Submitted => new SubmittedState(),
                ApplicationStatus.UnderReview => new UnderReviewState(),
                ApplicationStatus.Interviewing => new InterviewingState(),
                ApplicationStatus.Accepted => new AcceptedState(),
                ApplicationStatus.Rejected => new RejectedState(),
                ApplicationStatus.Withdrawn => new WithdrawnState(),
                _ => throw new ArgumentException($"Unknown application status: {status}")
            };
        }

        public static void InitializeState(this JobApplication application)
        {
            var state = GetStateFromStatus(application.Status);
            application.TransitionToState(state);
        }
    }
}
