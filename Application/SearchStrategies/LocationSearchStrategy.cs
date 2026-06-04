using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.SearchStrategies;

public class LocationSearchStrategy : IJobSearchStrategy
{
    public string StrategyName => "Location Search";

    public Task<IEnumerable<JobPosting>> SearchAsync(IEnumerable<JobPosting> jobs, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Task.FromResult(jobs);
        }

        var normalizedSearch = SearchText.Normalize(searchTerm);

        var results = jobs.Where(job =>
            SearchText.Contains(job.Location, normalizedSearch)
        );

        return Task.FromResult(results);
    }
}