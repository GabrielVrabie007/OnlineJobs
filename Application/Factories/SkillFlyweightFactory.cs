using System.Collections.Concurrent;
using OnlineJobs.Domain.Flyweights;

namespace OnlineJobs.Application.Factories
{
    /// <summary>
    /// Flyweight factory: returns a single shared <see cref="SkillFlyweight"/> per
    /// (name, category). Across thousands of jobs/candidates that all require "C#",
    /// only one "C#" object exists in memory. Registered as a singleton; the
    /// ConcurrentDictionary makes it thread-safe without explicit locks.
    /// </summary>
    public class SkillFlyweightFactory
    {
        private readonly ConcurrentDictionary<string, SkillFlyweight> _skillPool =
            new(StringComparer.OrdinalIgnoreCase);

        public SkillFlyweight GetSkill(string name, string category = "General")
            => _skillPool.GetOrAdd(GetKey(name, category), _ => new SkillFlyweight(name, category));

        /// <summary>Interns a batch of skill names, returning the shared flyweights.</summary>
        public IReadOnlyList<SkillFlyweight> GetSkills(IEnumerable<string> names, string category = "General")
            => names
                .Select(n => n?.Trim())
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => GetSkill(n!, category))
                .ToList();

        public int GetPoolSize() => _skillPool.Count;

        public void Clear() => _skillPool.Clear();

        public string GetPoolStatistics()
        {
            var values = _skillPool.Values.ToList();
            var grouped = values.GroupBy(s => s.Category)
                .Select(g => $"  [{g.Key}]: {string.Join(", ", g.Select(s => s.Name))}");
            return $"Flyweight pool: {values.Count} unique skills shared across all jobs/candidates\n"
                   + string.Join("\n", grouped);
        }

        private static string GetKey(string name, string category)
            => $"{name.Trim().ToLowerInvariant()}:{category.Trim().ToLowerInvariant()}";
    }
}
