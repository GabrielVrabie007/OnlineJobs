namespace OnlineJobs.Domain.ValueObjects
{

    public class Skill
    {
        public string Name { get; private set; }
        public SkillProficiency Proficiency { get; private set; }
        public int? YearsOfExperience { get; private set; }
        public string? Category { get; private set; }

        public Skill(
            string name,
            SkillProficiency proficiency,
            int? yearsOfExperience = null,
            string? category = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Skill name cannot be empty", nameof(name));
            if (yearsOfExperience.HasValue && yearsOfExperience < 0)
                throw new ArgumentException("Years of experience cannot be negative");

            Name = name;
            Proficiency = proficiency;
            YearsOfExperience = yearsOfExperience;
            Category = category;
        }

        public override string ToString()
        {
            var experienceStr = YearsOfExperience.HasValue
                ? $" ({YearsOfExperience} year{(YearsOfExperience > 1 ? "s" : "")})"
                : "";
            return $"{Name} - {Proficiency}{experienceStr}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Skill other) return false;
            return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Name.ToLowerInvariant().GetHashCode();
        }
    }


    public enum SkillProficiency
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3,
        Expert = 4
    }
}
