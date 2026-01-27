using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Domain.Entities
{
    /// <summary>
    /// Employer user type - inherits from User
    /// Demonstrates:
    /// - INHERITANCE: Extends User base class
    /// - LSP (Liskov Substitution): Can replace User without breaking functionality
    /// - SRP: Responsible only for employer specific data
    /// - COMPOSITION: Employer belongs to a Company
    /// </summary>
    public class Employer : User
    {
        // Employer-specific properties
        public Guid? CompanyId { get; set; }
        public Company? Company { get; set; }
        public string? Position { get; set; }
        public string? PhoneNumber { get; set; }

        // Navigation property for job postings
        public List<JobPosting> JobPostings { get; set; }

        public override UserType UserType => UserType.Employer;

        // Constructor
        public Employer(string email, string firstName, string lastName)
            : base(email, firstName, lastName)
        {
            JobPostings = new List<JobPosting>();
        }

        // Parameterless constructor
        public Employer() : base()
        {
            JobPostings = new List<JobPosting>();
        }

        // POLYMORPHISM: Override base class method
        public override bool CanPostJobs()
        {
            return IsActive && CompanyId.HasValue;
        }

        public override string GetDisplayInfo()
        {
            var baseInfo = base.GetDisplayInfo();
            var companyInfo = Company != null ? $" at {Company.Name}" : "";
            return $"{baseInfo} - Employer{companyInfo}";
        }

        // Employer-specific behavior
        public bool IsAssociatedWithCompany()
        {
            return CompanyId.HasValue;
        }

        public void AssignToCompany(Company company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            CompanyId = company.Id;
            Company = company;
        }

        public int GetPostedJobsCount()
        {
            return JobPostings?.Count ?? 0;
        }
    }
}