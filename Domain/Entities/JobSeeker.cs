using OnlineJobs.Domain.Enums;
using OnlineJobs.Domain.ValueObjects;

namespace OnlineJobs.Domain.Entities
{
    public class JobSeeker : User
    {
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

        // Legacy fields (kept for backward compatibility)
        public string Resume { get; set; }
        public string Skills { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // New structured profile fields (Builder Pattern)
        public string? ProfessionalSummary { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public List<Education> EducationHistory { get; set; }
        public List<WorkExperience> WorkHistory { get; set; }
        public List<Skill> SkillSet { get; set; }
        public List<Certification> Certifications { get; set; }

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
            EducationHistory = new List<Education>();
            WorkHistory = new List<WorkExperience>();
            SkillSet = new List<Skill>();
            Certifications = new List<Certification>();
        }

        public JobSeeker()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            Applications = new List<JobApplication>();
            EducationHistory = new List<Education>();
            WorkHistory = new List<WorkExperience>();
            SkillSet = new List<Skill>();
            Certifications = new List<Certification>();
        }

        public JobSeeker(Guid id)
        {
            Id = id;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            Applications = new List<JobApplication>();
            EducationHistory = new List<Education>();
            WorkHistory = new List<WorkExperience>();
            SkillSet = new List<Skill>();
            Certifications = new List<Certification>();
        }

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

        public bool HasCompleteProfile()
        {
            return !string.IsNullOrWhiteSpace(ProfessionalSummary) &&
                   EducationHistory.Any() &&
                   WorkHistory.Any() &&
                   SkillSet.Any();
        }

        public int GetTotalYearsOfExperience()
        {
            if (!WorkHistory.Any()) return 0;
            return (int)WorkHistory.Sum(w => w.GetDuration().TotalDays) / 365;
        }

        public List<Skill> GetSkillsByProficiency(SkillProficiency proficiency)
        {
            return SkillSet.Where(s => s.Proficiency == proficiency).ToList();
        }

        public List<Certification> GetValidCertifications()
        {
            return Certifications.Where(c => c.IsValid()).ToList();
        }
    }
}