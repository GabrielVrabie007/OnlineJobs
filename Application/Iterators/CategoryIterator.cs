using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Iterators
{
    public class CategoryIterator : IIterator<JobCategory>
    {
        private readonly List<JobCategory> _categories;
        private int _currentPosition = 0;

        public CategoryIterator(List<JobCategory> categories)
        {
            _categories = categories ?? new List<JobCategory>();
        }

        public bool HasNext()
        {
            return _currentPosition < _categories.Count;
        }

        public JobCategory Next()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException("No more elements in collection");
            }

            return _categories[_currentPosition++];
        }

        public void Reset()
        {
            _currentPosition = 0;
        }
    }
}
