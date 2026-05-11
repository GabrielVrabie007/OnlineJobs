using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Iterators
{
    public class BreadthFirstCategoryIterator : IIterator<JobCategory>
    {
        private readonly List<JobCategory> _flattenedCategories;
        private int _currentPosition = 0;

        public BreadthFirstCategoryIterator(JobCategory root)
        {
            _flattenedCategories = new List<JobCategory>();
            if (root != null)
            {
                FlattenBreadthFirst(root);
            }
        }

        private void FlattenBreadthFirst(JobCategory root)
        {
            var queue = new Queue<JobCategory>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                _flattenedCategories.Add(current);

                if (current is CategoryComposite composite)
                {
                    foreach (var child in composite.Children)
                    {
                        queue.Enqueue(child);
                    }
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
