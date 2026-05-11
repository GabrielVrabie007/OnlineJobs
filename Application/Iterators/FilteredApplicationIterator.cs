using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Iterators
{
    public class FilteredApplicationIterator : IIterator<JobApplication>
    {
        private readonly List<JobApplication> _filteredApplications;
        private int _currentPosition = 0;

        public FilteredApplicationIterator(List<JobApplication> applications, ApplicationStatus status)
        {
            _filteredApplications = applications?
                .Where(a => a.Status == status)
                .ToList() ?? new List<JobApplication>();
        }

        public bool HasNext()
        {
            return _currentPosition < _filteredApplications.Count;
        }

        public JobApplication Next()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException("No more elements in collection");
            }

            return _filteredApplications[_currentPosition++];
        }

        public void Reset()
        {
            _currentPosition = 0;
        }
    }
}
