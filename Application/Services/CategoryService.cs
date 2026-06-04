using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Services
{
    /// <summary>
    /// Builds a Composite tree from the job postings' real categories, grouped under a
    /// few top-level areas. Leaves are the actual categories that exist in the data (with
    /// real counts); branches and leaves share one interface (<see cref="JobCategory"/>),
    /// so counts roll up recursively and the page renders the whole tree uniformly.
    /// </summary>
    public class CategoryService
    {
        private readonly IJobService _jobService;

        public CategoryService(IJobService jobService)
        {
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
        }

        // Top-level areas in display order, each with the keywords that route a real
        // category into it. Checked top to bottom; first match wins.
        private static readonly (string Area, string[] Keywords)[] Areas =
        {
            ("Data & AI",          new[] { "machine learning", "data scien", "data eng", "analytic", "computer vision", "deep learning" }),
            ("Design",             new[] { "design", "ux", "ui" }),
            ("Product",            new[] { "product", "business analy" }),
            ("Finance & Business", new[] { "invest", "bank", "risk", "finance", "account", "sales", "marketing", "customer success", "manufactur", "business" }),
            ("Engineering & IT",   new[] { "develop", "engineer", "devops", "sre", "cloud", "architect", "security", "cyber", "infrastructure", "system", "programming", "salesforce", "quant", "web", "mobile", "backend", "frontend", "full stack", "hardware", "electrical", "embedded", "graphics", "vr", "ar", "technology", "solution" }),
        };

        public async Task<CategoryComposite> BuildCategoryTreeAsync()
        {
            var jobs = (await _jobService.GetActiveJobsAsync()).ToList();
            var root = new CategoryComposite("All Categories", "Browse jobs by area");

            var areaNodes = Areas.ToDictionary(a => a.Area, a => new CategoryComposite(a.Area, string.Empty));
            var other = new CategoryComposite("Other", string.Empty);

            // One leaf per real category, holding its jobs.
            var grouped = jobs
                .GroupBy(j => string.IsNullOrWhiteSpace(j.Category) ? "Uncategorized" : j.Category.Trim())
                .OrderByDescending(g => g.Count());

            foreach (var group in grouped)
            {
                var leaf = new CategoryLeaf(group.Key, string.Empty);
                foreach (var job in group) leaf.AddJob(job);

                var name = group.Key.ToLowerInvariant();
                var areaName = Areas.FirstOrDefault(a => a.Keywords.Any(name.Contains)).Area;
                var target = areaName != null && areaNodes.TryGetValue(areaName, out var node) ? node : other;
                target.Add(leaf);
            }

            // Only show areas that actually contain categories, biggest first.
            foreach (var node in areaNodes.Values
                         .Where(n => n.GetChildCount() > 0)
                         .OrderByDescending(n => n.GetJobCount()))
            {
                root.Add(node);
            }
            if (other.GetChildCount() > 0) root.Add(other);

            return root;
        }
    }
}
