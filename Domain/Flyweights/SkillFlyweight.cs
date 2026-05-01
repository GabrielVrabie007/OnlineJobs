namespace OnlineJobs.Domain.Flyweights
{

    public class SkillFlyweight
    {
        public string Name { get; }
        public string Category { get; }

        public SkillFlyweight(string name, string category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Skill name cannot be empty", nameof(name));

            Name = name;
            Category = category ?? "General";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not SkillFlyweight other) return false;
            return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) &&
                   Category.Equals(other.Category, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Name.ToLowerInvariant(),
                Category.ToLowerInvariant()
            );
        }

        public override string ToString()
        {
            return $"{Name} ({Category})";
        }
    }
}
