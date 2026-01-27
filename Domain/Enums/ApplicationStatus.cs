namespace OnlineJobs.Domain.Enums
{
    /// <summary>
    /// Defines the status of a job application
    /// Follows OCP: Extensible for new workflow states
    /// </summary>
    public enum ApplicationStatus
    {
        Submitted = 1,
        UnderReview = 2,
        Interviewing = 3,
        Accepted = 4,
        Rejected = 5,
        Withdrawn = 6
    }
}