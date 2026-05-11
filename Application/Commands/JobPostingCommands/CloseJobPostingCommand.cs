using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Commands.JobPostingCommands
{
    public class CloseJobPostingCommand : ICommand
    {
        private readonly IRepository<JobPosting> _jobRepository;
        private readonly Guid _jobId;
        private JobStatus _previousStatus;

        public CloseJobPostingCommand(
            IRepository<JobPosting> jobRepository,
            Guid jobId)
        {
            _jobRepository = jobRepository ?? throw new ArgumentNullException(nameof(jobRepository));
            _jobId = jobId;
        }

        public async Task ExecuteAsync()
        {
            var job = await _jobRepository.GetByIdAsync(_jobId);
            if (job == null)
                throw new InvalidOperationException("Job not found");

            _previousStatus = job.Status;
            job.Close();
            await _jobRepository.UpdateAsync(job);
        }

        public async Task UndoAsync()
        {
            var job = await _jobRepository.GetByIdAsync(_jobId);
            if (job != null)
            {
                typeof(JobPosting).GetProperty(nameof(JobPosting.Status))!
                    .SetValue(job, _previousStatus);
                await _jobRepository.UpdateAsync(job);
            }
        }

        public string GetDescription()
        {
            return $"Close job posting {_jobId}";
        }
    }
}
