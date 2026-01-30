using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;
using OnlineJobs.Application.Interfaces;

namespace OnlineJobs.Application.Services
{
    public class JobService : IJobService
    {
        private readonly IRepository<JobPosting> _jobRepository;
        private readonly IRepository<Employer> _employerRepository;

        public JobService(
            IRepository<JobPosting> jobRepository,
            IRepository<Employer> employerRepository)
        {
            _jobRepository = jobRepository ?? throw new ArgumentNullException(nameof(jobRepository));
            _employerRepository = employerRepository ?? throw new ArgumentNullException(nameof(employerRepository));
        }

        public async Task<JobPosting> CreateJobAsync(string title, string description, Guid employerId, Guid companyId)
        {
            var employer = await _employerRepository.GetByIdAsync(employerId);
            if (employer == null)
                throw new InvalidOperationException("Employer not found");

            if (!employer.CanPostJobs())
                throw new InvalidOperationException("Employer is not authorized to post jobs");

            var job = new JobPosting(title, description, employerId, companyId);
            await _jobRepository.AddAsync(job);

            return job;
        }

        public async Task<JobPosting> GetJobByIdAsync(Guid jobId)
        {
            return await _jobRepository.GetByIdAsync(jobId);
        }

        public async Task<IEnumerable<JobPosting>> GetAllJobsAsync()
        {
            return await _jobRepository.GetAllAsync();
        }

        public async Task<IEnumerable<JobPosting>> GetActiveJobsAsync()
        {
            return await _jobRepository.FindAsync(j => j.Status == JobStatus.Active);
        }

        public async Task UpdateJobAsync(JobPosting job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            await _jobRepository.UpdateAsync(job);
        }

        public async Task DeleteJobAsync(Guid jobId)
        {
            await _jobRepository.DeleteAsync(jobId);
        }

        public async Task PublishJobAsync(Guid jobId)
        {
            var job = await GetJobByIdAsync(jobId);
            if (job == null)
                throw new InvalidOperationException("Job not found");

            job.Publish();
            await _jobRepository.UpdateAsync(job);
        }

        public async Task CloseJobAsync(Guid jobId)
        {
            var job = await GetJobByIdAsync(jobId);
            if (job == null)
                throw new InvalidOperationException("Job not found");

            job.Close();
            await _jobRepository.UpdateAsync(job);
        }

        public async Task CancelJobAsync(Guid jobId)
        {
            var job = await GetJobByIdAsync(jobId);
            if (job == null)
                throw new InvalidOperationException("Job not found");

            job.Cancel();
            await _jobRepository.UpdateAsync(job);
        }

        public async Task<IEnumerable<JobPosting>> SearchByTitleAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return await GetAllJobsAsync();

            return await _jobRepository.FindAsync(j =>
                j.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<JobPosting>> GetJobsByEmployerAsync(Guid employerId)
        {
            return await _jobRepository.FindAsync(j => j.EmployerId == employerId);
        }

        public async Task<IEnumerable<JobPosting>> GetJobsByCompanyAsync(Guid companyId)
        {
            return await _jobRepository.FindAsync(j => j.CompanyId == companyId);
        }

        public async Task<IEnumerable<JobPosting>> GetJobsByStatusAsync(JobStatus status)
        {
            return await _jobRepository.FindAsync(j => j.Status == status);
        }

        public async Task<int> GetTotalJobCountAsync()
        {
            return await _jobRepository.CountAsync();
        }

        public async Task<int> GetActiveJobCountAsync()
        {
            var activeJobs = await GetActiveJobsAsync();
            return activeJobs.Count();
        }
    }
}