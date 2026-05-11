using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Iterators
{
    public class PaginatedJobIterator : IIterator<JobPosting>
    {
        private readonly List<JobPosting> _pageJobs;
        private int _currentPosition = 0;

        public int TotalPages { get; }
        public int CurrentPage { get; }
        public int PageSize { get; }
        public int TotalItems { get; }

        public PaginatedJobIterator(List<JobPosting> allJobs, int pageSize, int pageNumber)
        {
            if (pageSize <= 0) throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));
            if (pageNumber <= 0) throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

            allJobs ??= new List<JobPosting>();

            TotalItems = allJobs.Count;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((double)TotalItems / pageSize);
            CurrentPage = Math.Min(pageNumber, TotalPages > 0 ? TotalPages : 1);

            var skip = (CurrentPage - 1) * pageSize;
            _pageJobs = allJobs.Skip(skip).Take(pageSize).ToList();
        }

        public bool HasNext()
        {
            return _currentPosition < _pageJobs.Count;
        }

        public JobPosting Next()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException("No more elements in current page");
            }

            return _pageJobs[_currentPosition++];
        }

        public void Reset()
        {
            _currentPosition = 0;
        }
    }
}
