using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Proxies
{

    public class JobPostingProtectionProxy : IJobPostingAccess
    {
        private readonly RealJobPostingAccess _realAccess;
        private readonly bool _isAuthenticated;
        private readonly Guid? _currentUserId;

        public JobPostingProtectionProxy(RealJobPostingAccess realAccess, bool isAuthenticated, Guid? currentUserId = null)
        {
            _realAccess = realAccess ?? throw new ArgumentNullException(nameof(realAccess));
            _isAuthenticated = isAuthenticated;
            _currentUserId = currentUserId;
        }

        public async Task<JobPosting> GetJobDetailsAsync(Guid jobId)
        {
            if (!_isAuthenticated)
            {
                return await GetPreviewJobAsync(jobId);
            }

            return await _realAccess.GetJobDetailsAsync(jobId);
        }

        public async Task<IEnumerable<JobPosting>> GetAllJobsAsync()
        {
            var jobs = await _realAccess.GetAllJobsAsync();
            // Return jobs (authentication affects what details are visible via GetCompanyName/GetSalaryRange)
            return jobs;
        }

        public string GetCompanyName(JobPosting job)
        {
            if (!_isAuthenticated || !job.IsCompanyRevealed)
            {
                return "*** Company Hidden - Login to view ***";
            }

            return _realAccess.GetCompanyName(job);
        }

        public decimal? GetSalaryRange(JobPosting job)
        {
            if (!_isAuthenticated)
            {
                return null;
            }

            return _realAccess.GetSalaryRange(job);
        }

        private async Task<JobPosting> GetPreviewJobAsync(Guid jobId)
        {
            var job = await _realAccess.GetJobDetailsAsync(jobId);

            // Create a preview version with limited information
            var preview = new JobPosting
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description.Length > 200
                    ? job.Description.Substring(0, 197) + "..."
                    : job.Description,
                Location = job.Location,
                EmploymentType = job.EmploymentType,
                Category = job.Category,
                PostedDate = job.PostedDate,
                Status = job.Status,
                // Hide sensitive information
                Requirements = "Login to view requirements",
                SalaryMin = null,
                SalaryMax = null,
                IsCompanyRevealed = false
            };

            return preview;
        }
    }
}
