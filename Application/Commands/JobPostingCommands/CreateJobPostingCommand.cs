using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Commands.JobPostingCommands
{
    public class CreateJobPostingCommand : ICommand
    {
        private readonly IRepository<JobPosting> _jobRepository;
        private readonly string _title;
        private readonly string _description;
        private readonly Guid _employerId;
        private readonly Guid _companyId;
        private JobPosting? _createdJob;

        public CreateJobPostingCommand(
            IRepository<JobPosting> jobRepository,
            string title,
            string description,
            Guid employerId,
            Guid companyId)
        {
            _jobRepository = jobRepository ?? throw new ArgumentNullException(nameof(jobRepository));
            _title = title;
            _description = description;
            _employerId = employerId;
            _companyId = companyId;
        }

        public async Task ExecuteAsync()
        {
            _createdJob = new JobPosting(_title, _description, _employerId, _companyId);
            await _jobRepository.AddAsync(_createdJob);
        }

        public async Task UndoAsync()
        {
            if (_createdJob != null)
            {
                await _jobRepository.DeleteAsync(_createdJob.Id);
            }
        }

        public string GetDescription()
        {
            return $"Create job posting: {_title}";
        }
    }
}
