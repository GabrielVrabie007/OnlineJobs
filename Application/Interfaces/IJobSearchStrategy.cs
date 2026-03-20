using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Interfaces;

public interface IJobSearchStrategy
{
    Task<IEnumerable<JobPosting>> SearchAsync(IEnumerable<JobPosting> jobs, string searchTerm);
    
    string StrategyName { get; }
}