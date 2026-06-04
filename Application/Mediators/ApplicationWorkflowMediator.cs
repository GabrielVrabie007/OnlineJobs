using OnlineJobs.Application.Commands;
using OnlineJobs.Application.Commands.ApplicationCommands;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Mementos;
using OnlineJobs.Application.Observers;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Mediators
{
    public class ApplicationWorkflowMediator : IMediator<SubmitApplicationRequest, MediatorResult>
    {
        private readonly IRepository<JobApplication> _applicationRepository;
        private readonly CommandInvoker _commandInvoker;
        private readonly ApplicationStatusSubject _applicationStatusSubject;
        private readonly ApplicationDraftManager _draftManager;

        public ApplicationWorkflowMediator(
            IRepository<JobApplication> applicationRepository,
            CommandInvoker commandInvoker,
            ApplicationStatusSubject applicationStatusSubject,
            ApplicationDraftManager draftManager)
        {
            _applicationRepository = applicationRepository;
            _commandInvoker = commandInvoker;
            _applicationStatusSubject = applicationStatusSubject;
            _draftManager = draftManager;
        }

        public async Task<MediatorResult> HandleAsync(SubmitApplicationRequest request)
        {
            try
            {
                var submitCommand = new SubmitApplicationCommand(
                    _applicationRepository,
                    request.JobId,
                    request.UserId,
                    request.CoverLetter
                );

                await _commandInvoker.ExecuteAsync(submitCommand);

                _draftManager.DeleteDraft(request.UserId, request.JobId);

                return MediatorResult.SuccessResult("Application submitted successfully");
            }
            catch (Exception ex)
            {
                return MediatorResult.FailureResult(ex.Message, new List<string> { ex.ToString() });
            }
        }
    }
}
