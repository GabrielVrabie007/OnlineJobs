using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Strategies.ScoringStrategies
{
    public class EducationScoringStrategy : IApplicationScoringStrategy
    {
        public int CalculateScore(JobSeeker jobSeeker, JobPosting jobPosting)
        {
            if (jobSeeker?.EducationHistory == null || !jobSeeker.EducationHistory.Any())
                return 50;

            var highestDegree = jobSeeker.EducationHistory
                .OrderByDescending(e => GetDegreeLevel(e.Degree))
                .FirstOrDefault();

            if (highestDegree == null)
                return 50;

            return GetDegreeLevel(highestDegree.Degree) switch
            {
                4 => 100,
                3 => 90,
                2 => 80,
                1 => 70,
                _ => 50
            };
        }

        private int GetDegreeLevel(string degree)
        {
            if (string.IsNullOrWhiteSpace(degree))
                return 0;

            var degreeLower = degree.ToLower();

            if (degreeLower.Contains("phd") || degreeLower.Contains("doctorate"))
                return 4;
            if (degreeLower.Contains("master") || degreeLower.Contains("mba") || degreeLower.Contains("ms"))
                return 3;
            if (degreeLower.Contains("bachelor") || degreeLower.Contains("bs") || degreeLower.Contains("ba"))
                return 2;
            if (degreeLower.Contains("associate"))
                return 1;

            return 0;
        }
    }
}
