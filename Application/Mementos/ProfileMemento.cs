using OnlineJobs.Domain.ValueObjects;

namespace OnlineJobs.Application.Mementos
{
    public class ProfileMemento : IMemento
    {
        public Guid JobSeekerId { get; }
        public string? PhoneNumber { get; }
        public string? Location { get; }
        public string? Summary { get; }
        public List<Skill> SkillSet { get; }
        public List<WorkExperience> WorkHistory { get; }
        public List<Education> EducationHistory { get; }
        public List<Certification> Certifications { get; }
        public DateTime CreatedAt { get; }

        public ProfileMemento(
            Guid jobSeekerId,
            string? phoneNumber,
            string? location,
            string? summary,
            List<Skill>? skillSet,
            List<WorkExperience>? workHistory,
            List<Education>? educationHistory,
            List<Certification>? certifications)
        {
            JobSeekerId = jobSeekerId;
            PhoneNumber = phoneNumber;
            Location = location;
            Summary = summary;
            SkillSet = skillSet?.ToList() ?? new List<Skill>();
            WorkHistory = workHistory?.ToList() ?? new List<WorkExperience>();
            EducationHistory = educationHistory?.ToList() ?? new List<Education>();
            Certifications = certifications?.ToList() ?? new List<Certification>();
            CreatedAt = DateTime.UtcNow;
        }

        public string GetDescription()
        {
            return $"Profile snapshot from {CreatedAt:yyyy-MM-dd HH:mm:ss}";
        }
    }
}
