using OnlineJobs.Domain.Enums;
using OnlineJobs.Domain.ValueObjects;

namespace OnlineJobs.Domain.Entities
{
    public class JobSeeker : User
    {
        // All base properties (Id, Email, PasswordHash, FirstName, LastName, etc.)
        // are inherited from User base class

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

        public JobSeeker(string email, string firstName, string lastName) : base(email, firstName, lastName)
        {
            UserType = UserType.JobSeeker;
            Applications = new List<JobApplication>();
            EducationHistory = new List<Education>();
            WorkHistory = new List<WorkExperience>();
            SkillSet = new List<Skill>();
            Certifications = new List<Certification>();
        }

        public JobSeeker() : base()
        {
            UserType = UserType.JobSeeker;
            Applications = new List<JobApplication>();
            EducationHistory = new List<Education>();
            WorkHistory = new List<WorkExperience>();
            SkillSet = new List<Skill>();
            Certifications = new List<Certification>();
        }

        public JobSeeker(Guid id) : base(id)
        {
            UserType = UserType.JobSeeker;
            Applications = new List<JobApplication>();
            EducationHistory = new List<Education>();
            WorkHistory = new List<WorkExperience>();
            SkillSet = new List<Skill>();
            Certifications = new List<Certification>();
        }

        // GetFullName() and UpdateLastLogin() are inherited from base User class

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