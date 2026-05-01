using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Proxies
{

    public class RealApplicationListAccess : IApplicationListAccess
    {
        private readonly IApplicationService _applicationService;
        private readonly Guid _jobPostingId;

        public RealApplicationListAccess(IApplicationService applicationService, Guid jobPostingId)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _jobPostingId = jobPostingId;
        }

        public async Task<IEnumerable<JobApplication>> GetApplicationsAsync()
        {
            var applications = await _applicationService.GetApplicationsByJobPostingAsync(_jobPostingId);
            return applications;
        }

        public async Task<int> GetApplicationCountAsync()
        {
            var applications = await GetApplicationsAsync();
            return applications.Count();
        }

        public async Task<JobApplication> GetApplicationByIdAsync(Guid applicationId)
        {
            var applications = await GetApplicationsAsync();
            return applications.FirstOrDefault(a => a.Id == applicationId);
        }
    }
}
