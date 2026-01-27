using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Domain.Entities
{
    /// <summary>
    /// Abstract base class for all users in the system
    /// Demonstrates:
    /// - ENCAPSULATION: Private fields with public properties
    /// - ABSTRACTION: Abstract class that cannot be instantiated directly
    /// - OCP: Open for extension (JobSeeker, Employer) but closed for modification
    /// - SRP: Single responsibility - managing user identity and basic information
    /// </summary>
    public abstract class User
    {
        // Encapsulated fields - cannot be accessed directly from outside
        private string _email;
        private string _firstName;
        private string _lastName;

        // Public properties with validation (Encapsulation)
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
        public abstract UserType UserType { get; }

        // Constructor - ensures valid object creation
        protected User(string email, string firstName, string lastName)
        {
            Id = Guid.NewGuid();
            Email = email; // Uses property setter with validation
            FirstName = firstName;
            LastName = lastName;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        // Parameterless constructor for serialization/ORM
        protected User()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        // Constructor with specific Id (for loading existing entities)
        protected User(Guid id)
        {
            Id = id;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        // Public methods (Behavior encapsulation)
        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }

        public virtual string GetDisplayInfo()
        {
            return $"{GetFullName()} ({Email})";
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        // Virtual method for polymorphism - can be overridden by derived classes
        public virtual bool CanPostJobs()
        {
            return false;
        }

        public virtual bool CanApplyToJobs()
        {
            return false;
        }
    }
}