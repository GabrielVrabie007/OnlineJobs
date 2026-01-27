namespace OnlineJobs.Domain.Enums
{
    /// <summary>
    /// Defines the status of a job posting
    /// Follows OCP: New statuses can be added without modifying existing functionality
    /// </summary>
    public enum JobStatus
    {
        Draft = 1,
        Active = 2,
        Closed = 3,
        Expired = 4,
        Cancelled = 5
    }
}