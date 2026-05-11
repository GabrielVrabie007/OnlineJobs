using OnlineJobs.Domain.ValueObjects;

namespace OnlineJobs.Application.Mementos
{
    public class JobPostingMemento : IMemento
    {
        public Guid JobPostingId { get; }
        public string Title { get; }
        public string Description { get; }
        public string? Requirements { get; }
        public string? Category { get; }
        public string? EmploymentType { get; }
        public decimal? SalaryMin { get; }
        public decimal? SalaryMax { get; }
        public List<JobSkillRequirement> SkillRequirements { get; }
        public DateTime CreatedAt { get; }

        public JobPostingMemento(
            Guid jobPostingId,
            string title,
            string description,
            string? requirements,
            string? category,
            string? employmentType,
            decimal? salaryMin,
            decimal? salaryMax,
            List<JobSkillRequirement>? skillRequirements)
        {
            JobPostingId = jobPostingId;
            Title = title;
            Description = description;
            Requirements = requirements;
            Category = category;
            EmploymentType = employmentType;
            SalaryMin = salaryMin;
            SalaryMax = salaryMax;
            SkillRequirements = skillRequirements?.ToList() ?? new List<JobSkillRequirement>();
            CreatedAt = DateTime.UtcNow;
        }

        public string GetDescription()
        {
            return $"Job posting '{Title}' snapshot from {CreatedAt:yyyy-MM-dd HH:mm:ss}";
        }
    }
}
