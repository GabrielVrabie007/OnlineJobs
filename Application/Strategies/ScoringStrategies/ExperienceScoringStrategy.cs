using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Strategies.ScoringStrategies
{
    public class ExperienceScoringStrategy : IApplicationScoringStrategy
    {
        public int CalculateScore(JobSeeker jobSeeker, JobPosting jobPosting)
        {
            if (jobSeeker?.WorkHistory == null)
                return 0;

            var totalYears = jobSeeker.WorkHistory
                .Select(exp => (exp.EndDate ?? DateTime.Now).Year - exp.StartDate.Year)
                .Sum();

            if (totalYears >= 10) return 100;
            if (totalYears >= 7) return 90;
            if (totalYears >= 5) return 80;
            if (totalYears >= 3) return 70;
            if (totalYears >= 1) return 60;

            return 40;
        }
    }
}
