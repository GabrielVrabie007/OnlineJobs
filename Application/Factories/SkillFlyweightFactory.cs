using OnlineJobs.Domain.Flyweights;

namespace OnlineJobs.Application.Factories
{

    public class SkillFlyweightFactory
    {
        private readonly Dictionary<string, SkillFlyweight> _skillPool;
        private static readonly object _lock = new object();

        public SkillFlyweightFactory()
        {
            _skillPool = new Dictionary<string, SkillFlyweight>(StringComparer.OrdinalIgnoreCase);
        }
        
        public SkillFlyweight GetSkill(string name, string category = "General")
        {
            string key = GetKey(name, category);

            lock (_lock)
            {
                if (!_skillPool.ContainsKey(key))
                {
                    _skillPool[key] = new SkillFlyweight(name, category);
                }

                return _skillPool[key];
            }
        }


        public int GetPoolSize()
        {
            lock (_lock)
            {
                return _skillPool.Count;
            }
        }


        public string GetPoolStatistics()
        {
            lock (_lock)
            {
                var stats = $"Flyweight Pool Statistics:\n";
                stats += $"  Total unique skills: {_skillPool.Count}\n";
                stats += $"  Memory saved: Potentially {(_skillPool.Count * 100)} objects reduced to {_skillPool.Count}\n";
                stats += $"\nSkills in pool:\n";

                var grouped = _skillPool.Values.GroupBy(s => s.Category);
                foreach (var group in grouped)
                {
                    stats += $"  [{group.Key}]: {string.Join(", ", group.Select(s => s.Name))}\n";
                }

                return stats;
            }
        }


        public void Clear()
        {
            lock (_lock)
            {
                _skillPool.Clear();
            }
        }

        private string GetKey(string name, string category)
        {
            return $"{name.ToLowerInvariant()}:{category.ToLowerInvariant()}";
        }
    }
}
