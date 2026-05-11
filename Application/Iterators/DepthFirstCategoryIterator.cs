using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Iterators
{
    public class DepthFirstCategoryIterator : IIterator<JobCategory>
    {
        private readonly List<JobCategory> _flattenedCategories;
        private int _currentPosition = 0;

        public DepthFirstCategoryIterator(JobCategory root)
        {
            _flattenedCategories = new List<JobCategory>();
            if (root != null)
            {
                FlattenDepthFirst(root);
            }
        }

        private void FlattenDepthFirst(JobCategory category)
        {
            _flattenedCategories.Add(category);

            if (category is CategoryComposite composite)
            {
                foreach (var child in composite.Children)
                {
                    FlattenDepthFirst(child);
                }
            }
        }

        public bool HasNext()
        {
            return _currentPosition < _flattenedCategories.Count;
        }

        public JobCategory Next()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException("No more elements in collection");
            }

            return _flattenedCategories[_currentPosition++];
        }

        public void Reset()
        {
            _currentPosition = 0;
        }
    }
}
