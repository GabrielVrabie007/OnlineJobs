using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Proxies
{

    public interface IApplicationListAccess
    {
        Task<IEnumerable<JobApplication>> GetApplicationsAsync();
        Task<int> GetApplicationCountAsync();
        Task<JobApplication> GetApplicationByIdAsync(Guid applicationId);
    }
}
