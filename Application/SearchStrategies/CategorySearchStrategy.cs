using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.SearchStrategies;


public class CategorySearchStrategy : IJobSearchStrategy
{
    public string StrategyName => "Category Search";

    public Task<IEnumerable<JobPosting>> SearchAsync(IEnumerable<JobPosting> jobs, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Task.FromResult(jobs);
        }

        var normalizedSearch = searchTerm.Trim().ToLowerInvariant();

        var results = jobs.Where(job =>
            !string.IsNullOrEmpty(job.Category) &&
            job.Category.ToLowerInvariant().Contains(normalizedSearch)
        );

        return Task.FromResult(results);
    }
}