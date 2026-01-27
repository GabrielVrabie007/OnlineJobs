using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Domain.Entities
{
    /// <summary>
    /// JobSeeker user type - inherits from User
    /// Demonstrates:
    /// - INHERITANCE: Extends User base class
    /// - LSP (Liskov Substitution): Can replace User without breaking functionality
    /// - SRP: Responsible only for job seeker specific data
    /// </summary>
    public class JobSeeker : User
    {
        // JobSeeker-specific properties (Encapsulation)
        public string Resume { get; set; }
        public string Skills { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // Navigation property for applications
        public List<JobApplication> Applications { get; set; }

        public override UserType UserType => UserType.JobSeeker;

        // Constructor
        public JobSeeker(string email, string firstName, string lastName)
            : base(email, firstName, lastName)
        {
            Applications = new List<JobApplication>();
        }

        // Parameterless constructor
        public JobSeeker() : base()
        {
            Applications = new List<JobApplication>();
        }

        // Constructor with specific Id (for loading existing entities)
        public JobSeeker(Guid id) : base(id)
        {
            Applications = new List<JobApplication>();
        }

        // POLYMORPHISM: Override base class method
        public override bool CanApplyToJobs()
        {
            return IsActive;
        }

        public override string GetDisplayInfo()
        {
            var baseInfo = base.GetDisplayInfo();
            return $"{baseInfo} - Job Seeker";
        }

        // JobSeeker-specific behavior
        public bool HasResume()
        {
            return !string.IsNullOrWhiteSpace(Resume);
        }

        public int GetApplicationCount()
        {
            return Applications?.Count ?? 0;
        }
    }
}