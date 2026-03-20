using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.SearchStrategies;

namespace OnlineJobs.Application.Factories;


public class JobSearchStrategyFactory
{

    public enum SearchType
    {
        Title,
        Location,
        Category
    }

    public IJobSearchStrategy CreateSearchStrategy(SearchType searchType)
    {
        return searchType switch
        {
            SearchType.Title => new TitleSearchStrategy(),
            SearchType.Location => new LocationSearchStrategy(),
            SearchType.Category => new CategorySearchStrategy(),
            _ => throw new ArgumentException($"Unsupported search type: {searchType}", nameof(searchType))
        };
    }
    
    public IJobSearchStrategy CreateSearchStrategy(string searchType)
    {
        if (string.IsNullOrWhiteSpace(searchType))
        {
            return new TitleSearchStrategy();
        }

        var normalizedType = searchType.Trim().ToLowerInvariant();

        return normalizedType switch
        {
            "title" => new TitleSearchStrategy(),
            "location" => new LocationSearchStrategy(),
            "category" => new CategorySearchStrategy(),
            _ => throw new ArgumentException($"Unsupported search type: {searchType}", nameof(searchType))
        };
    }

    public IEnumerable<IJobSearchStrategy> GetAllStrategies()
    {
        return new List<IJobSearchStrategy>
        {
            new TitleSearchStrategy(),
            new LocationSearchStrategy(),
            new CategorySearchStrategy()
        };
    }
}