using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Iterators
{
    public class JobIterator : IIterator<JobPosting>
    {
        private readonly List<JobPosting> _jobs;
        private int _currentPosition = 0;

        public JobIterator(List<JobPosting> jobs)
        {
            _jobs = jobs ?? new List<JobPosting>();
        }

        public bool HasNext()
        {
            return _currentPosition < _jobs.Count;
        }

        public JobPosting Next()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException("No more elements in collection");
            }

            return _jobs[_currentPosition++];
        }

        public void Reset()
        {
            _currentPosition = 0;
        }
    }
}
