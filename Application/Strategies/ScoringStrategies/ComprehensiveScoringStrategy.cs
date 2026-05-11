using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Strategies.ScoringStrategies
{
    public class ComprehensiveScoringStrategy : IApplicationScoringStrategy
    {
        private readonly SkillMatchScoringStrategy _skillStrategy = new();
        private readonly ExperienceScoringStrategy _experienceStrategy = new();
        private readonly EducationScoringStrategy _educationStrategy = new();

        private const double SkillWeight = 0.50;
        private const double ExperienceWeight = 0.30;
        private const double EducationWeight = 0.20;

        public int CalculateScore(JobSeeker jobSeeker, JobPosting jobPosting)
        {
            var skillScore = _skillStrategy.CalculateScore(jobSeeker, jobPosting);
            var experienceScore = _experienceStrategy.CalculateScore(jobSeeker, jobPosting);
            var educationScore = _educationStrategy.CalculateScore(jobSeeker, jobPosting);

            var weightedScore = (skillScore * SkillWeight) +
                              (experienceScore * ExperienceWeight) +
                              (educationScore * EducationWeight);

            return (int)Math.Round(weightedScore);
        }
    }
}
