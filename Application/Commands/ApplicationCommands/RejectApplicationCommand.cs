using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Commands.ApplicationCommands
{
    public class RejectApplicationCommand : ICommand
    {
        private readonly IRepository<JobApplication> _applicationRepository;
        private readonly Guid _applicationId;
        private readonly string? _reason;
        private ApplicationStatus _previousStatus;

        public RejectApplicationCommand(
            IRepository<JobApplication> applicationRepository,
            Guid applicationId,
            string? reason = null)
        {
            _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
            _applicationId = applicationId;
            _reason = reason;
        }

        public async Task ExecuteAsync()
        {
            var application = await _applicationRepository.GetByIdAsync(_applicationId);
            if (application == null)
                throw new InvalidOperationException("Application not found");

            _previousStatus = application.Status;
            application.Reject(_reason);
            await _applicationRepository.UpdateAsync(application);
        }

        public async Task UndoAsync()
        {
            var application = await _applicationRepository.GetByIdAsync(_applicationId);
            if (application != null)
            {
                application.RestoreStatus(_previousStatus);
                await _applicationRepository.UpdateAsync(application);
            }
        }

        public string GetDescription()
        {
            return $"Reject application {_applicationId}";
        }
    }
}
