namespace OnlineJobs.Domain.Enums
{
    /// <summary>
    /// Defines the types of users in the system
    /// Follows OCP: Can be extended with new user types without modifying existing code
    /// </summary>
    public enum UserType
    {
        JobSeeker = 1,
        Employer = 2,
        Administrator = 3
    }
}