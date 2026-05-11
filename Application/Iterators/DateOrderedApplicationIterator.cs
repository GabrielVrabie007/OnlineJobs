using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Iterators
{
    public class DateOrderedApplicationIterator : IIterator<JobApplication>
    {
        private readonly List<JobApplication> _sortedApplications;
        private int _currentPosition = 0;

        public DateOrderedApplicationIterator(List<JobApplication> applications, bool ascending = true)
        {
            _sortedApplications = ascending
                ? applications?.OrderBy(a => a.AppliedAt).ToList() ?? new List<JobApplication>()
                : applications?.OrderByDescending(a => a.AppliedAt).ToList() ?? new List<JobApplication>();
        }

        public bool HasNext()
        {
            return _currentPosition < _sortedApplications.Count;
        }

        public JobApplication Next()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException("No more elements in collection");
            }

            return _sortedApplications[_currentPosition++];
        }

        public void Reset()
        {
            _currentPosition = 0;
        }
    }
}
