using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Visitors
{
    public interface IApplicationVisitor<T>
    {
        T VisitJobApplication(JobApplication application);
        T VisitJobPosting(JobPosting jobPosting);
    }
}
