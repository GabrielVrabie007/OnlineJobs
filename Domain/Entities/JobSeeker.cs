using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Domain.Entities
{
    public class JobSeeker : User
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
        public UserType UserType => UserType.JobSeeker;

        // JobSeeker-specific properties
        public string Resume { get; set; }
        public string Skills { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public List<JobApplication> Applications { get; set; }

        public JobSeeker(string email, string firstName, string lastName)
        {
            Id = Guid.NewGuid();
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            Applications = new List<JobApplication>();
        }

        public JobSeeker()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            Applications = new List<JobApplication>();
        }

        public JobSeeker(Guid id)
        {
            Id = id;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            Applications = new List<JobApplication>();
        }

        // Methods from User class (duplicated)
        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }

        public string GetDisplayInfo()
        {
            return $"{GetFullName()} ({Email}) - Job Seeker";
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        // JobSeeker-specific methods
        public bool CanApplyToJobs()
        {
            return IsActive;
        }

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