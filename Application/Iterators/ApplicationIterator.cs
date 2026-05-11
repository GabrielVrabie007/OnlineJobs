using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Iterators
{
    public class ApplicationIterator : IIterator<JobApplication>
    {
        private readonly List<JobApplication> _applications;
        private int _currentPosition = 0;

        public ApplicationIterator(List<JobApplication> applications)
        {
            _applications = applications ?? new List<JobApplication>();
        }

        public bool HasNext()
        {
            return _currentPosition < _applications.Count;
        }

        public JobApplication Next()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException("No more elements in collection");
            }

            return _applications[_currentPosition++];
        }

        public void Reset()
        {
            _currentPosition = 0;
        }
    }
}
