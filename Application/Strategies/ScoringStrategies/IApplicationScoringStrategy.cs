using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Strategies.ScoringStrategies
{
    public interface IApplicationScoringStrategy
    {
        int CalculateScore(JobSeeker jobSeeker, JobPosting jobPosting);
    }
}
