using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.SearchStrategies;


public class TitleSearchStrategy : IJobSearchStrategy
{
    public string StrategyName => "Title and Description Search";

    public Task<IEnumerable<JobPosting>> SearchAsync(IEnumerable<JobPosting> jobs, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Task.FromResult(jobs);
        }

        var normalizedSearch = searchTerm.Trim().ToLowerInvariant();

        var results = jobs.Where(job =>
            job.Title.ToLowerInvariant().Contains(normalizedSearch) ||
            job.Description.ToLowerInvariant().Contains(normalizedSearch)
        );

        return Task.FromResult(results);
    }
}