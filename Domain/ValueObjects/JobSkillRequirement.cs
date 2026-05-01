using OnlineJobs.Domain.Flyweights;

namespace OnlineJobs.Domain.ValueObjects
{

    public class JobSkillRequirement
    {
        public SkillFlyweight Skill { get; private set; }

        public SkillProficiency RequiredProficiency { get; private set; }
        public int? MinYearsOfExperience { get; private set; }
        public bool IsRequired { get; private set; }

        public JobSkillRequirement(
            SkillFlyweight skill,
            SkillProficiency requiredProficiency,
            int? minYearsOfExperience = null,
            bool isRequired = true)
        {
            Skill = skill ?? throw new ArgumentNullException(nameof(skill));
            RequiredProficiency = requiredProficiency;
            MinYearsOfExperience = minYearsOfExperience;
            IsRequired = isRequired;

            if (minYearsOfExperience.HasValue && minYearsOfExperience < 0)
                throw new ArgumentException("Years of experience cannot be negative");
        }

        public override string ToString()
        {
            var experienceStr = MinYearsOfExperience.HasValue
                ? $", {MinYearsOfExperience}+ years"
                : "";
            var requiredStr = IsRequired ? "Required" : "Preferred";
            return $"{Skill.Name} - {RequiredProficiency}{experienceStr} ({requiredStr})";
        }

        public bool Equals(JobSkillRequirement? other)
        {
            if (other == null) return false;
            return Skill.Equals(other.Skill);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as JobSkillRequirement);
        }

        public override int GetHashCode()
        {
            return Skill.GetHashCode();
        }
    }
}
