using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Domain.Entities
{
    public class Employer : User
    {
        // All base properties (Id, Email, PasswordHash, FirstName, LastName, etc.)
        // are inherited from User base class

        // Employer-specific properties
        public Guid? CompanyId { get; set; }
        public Company? Company { get; set; }
        public string? Position { get; set; }

        public List<JobPosting> JobPostings { get; set; }

        public Employer(string email, string firstName, string lastName) : base(email, firstName, lastName)
        {
            UserType = UserType.Employer;
            JobPostings = new List<JobPosting>();
        }

        public Employer() : base()
        {
            UserType = UserType.Employer;
            JobPostings = new List<JobPosting>();
        }

        public Employer(Guid id) : base(id)
        {
            UserType = UserType.Employer;
            JobPostings = new List<JobPosting>();
        }

        // GetFullName() and UpdateLastLogin() are inherited from base User class

        // Employer-specific methods
        public bool CanPostJobs()
        {
            return IsActive && CompanyId.HasValue;
        }

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