using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Iterators
{
    public class ApplicationCollection : ICollection<JobApplication>
    {
        private readonly List<JobApplication> _applications;

        public ApplicationCollection(IEnumerable<JobApplication> applications)
        {
            _applications = applications?.ToList() ?? new List<JobApplication>();
        }

        public IIterator<JobApplication> CreateIterator()
        {
            return new ApplicationIterator(_applications);
        }

        public IIterator<JobApplication> CreateFilteredIterator(ApplicationStatus status)
        {
            return new FilteredApplicationIterator(_applications, status);
        }

        public IIterator<JobApplication> CreateDateOrderedIterator(bool ascending = true)
        {
            return new DateOrderedApplicationIterator(_applications, ascending);
        }
    }
}
