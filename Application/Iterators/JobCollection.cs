using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Iterators
{
    public class JobCollection : ICollection<JobPosting>
    {
        private readonly List<JobPosting> _jobs;

        public JobCollection(IEnumerable<JobPosting> jobs)
        {
            _jobs = jobs?.ToList() ?? new List<JobPosting>();
        }

        public IIterator<JobPosting> CreateIterator()
        {
            return new JobIterator(_jobs);
        }

        public IIterator<JobPosting> CreatePaginatedIterator(int pageSize, int pageNumber)
        {
            return new PaginatedJobIterator(_jobs, pageSize, pageNumber);
        }
    }
}
