using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Strategies.ScoringStrategies
{
    public class SkillMatchScoringStrategy : IApplicationScoringStrategy
    {
        public int CalculateScore(JobSeeker jobSeeker, JobPosting jobPosting)
        {
            if (jobSeeker?.SkillSet == null || jobPosting?.SkillRequirements == null)
                return 0;

            var jobSeekerSkills = jobSeeker.SkillSet.Select(s => s.Name.ToLower()).ToHashSet();
            var requiredSkills = jobPosting.SkillRequirements.Select(s => s.Skill.Name.ToLower()).ToHashSet();

            if (requiredSkills.Count() == 0)
                return 50;

            var matchingSkills = jobSeekerSkills.Intersect(requiredSkills).Count();
            var matchPercentage = (double)matchingSkills / requiredSkills.Count();

            return (int)(matchPercentage * 100);
        }
    }
}
