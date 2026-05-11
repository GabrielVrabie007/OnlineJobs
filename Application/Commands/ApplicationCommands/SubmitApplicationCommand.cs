using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Commands.ApplicationCommands
{
    public class SubmitApplicationCommand : ICommand
    {
        private readonly IRepository<JobApplication> _applicationRepository;
        private readonly Guid _jobPostingId;
        private readonly Guid _jobSeekerId;
        private readonly string _coverLetter;
        private JobApplication? _createdApplication;

        public SubmitApplicationCommand(
            IRepository<JobApplication> applicationRepository,
            Guid jobPostingId,
            Guid jobSeekerId,
            string coverLetter)
        {
            _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
            _jobPostingId = jobPostingId;
            _jobSeekerId = jobSeekerId;
            _coverLetter = coverLetter;
        }

        public async Task ExecuteAsync()
        {
            _createdApplication = new JobApplication(_jobPostingId, _jobSeekerId, _coverLetter);
            await _applicationRepository.AddAsync(_createdApplication);
        }

        public async Task UndoAsync()
        {
            if (_createdApplication != null)
            {
                await _applicationRepository.DeleteAsync(_createdApplication.Id);
            }
        }

        public string GetDescription()
        {
            return $"Submit application for job {_jobPostingId}";
        }
    }
}
