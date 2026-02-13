using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Domain.Entities
{
    public class Employer : User
    {
        // Properties from User class (duplicated)
        private string _email;
        private string _firstName;
        private string _lastName;

        public Guid Id { get; private set; }

        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Email cannot be empty");

                if (!value.Contains("@"))
                    throw new ArgumentException("Invalid email format");

                _email = value;
            }
        }

        public string PasswordHash { get; set; }

        public string FirstName
        {
            get => _firstName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("First name cannot be empty");
                _firstName = value;
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Last name cannot be empty");
                _lastName = value;
            }
        }

        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public string? PhoneNumber { get; set; }
        public UserType UserType => UserType.Employer;

        // Employer-specific properties
        public Guid? CompanyId { get; set; }
        public Company? Company { get; set; }
        public string? Position { get; set; }

        public List<JobPosting> JobPostings { get; set; }

        public Employer(string email, string firstName, string lastName)
        {
            Id = Guid.NewGuid();
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            JobPostings = new List<JobPosting>();
        }

        public Employer()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            JobPostings = new List<JobPosting>();
        }

        public Employer(Guid id)
        {
            Id = id;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            JobPostings = new List<JobPosting>();
        }

        // Methods from User class (duplicated)
        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }

        public string GetDisplayInfo()
        {
            var companyInfo = Company != null ? $" at {Company.Name}" : "";
            return $"{GetFullName()} ({Email}) - Employer{companyInfo}";
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

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