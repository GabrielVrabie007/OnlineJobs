using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.ApprovalChains
{
    public class ApprovalChainFactory
    {
        private readonly IRepository<JobApplication> _applicationRepository;
        private readonly IRepository<JobSeeker> _jobSeekerRepository;

        public ApprovalChainFactory(
            IRepository<JobApplication> applicationRepository,
            IRepository<JobSeeker> jobSeekerRepository)
        {
            _applicationRepository = applicationRepository;
            _jobSeekerRepository = jobSeekerRepository;
        }

        public IApprovalHandler CreateStandardChain()
        {
            var automatedScreening = new AutomatedScreeningHandler(_applicationRepository, _jobSeekerRepository);
            var hrReview = new HRReviewHandler();
            var managerApproval = new ManagerApprovalHandler();
            var directorApproval = new DirectorApprovalHandler();

            automatedScreening
                .SetNext(hrReview)
                .SetNext(managerApproval)
                .SetNext(directorApproval);

            return automatedScreening;
        }

        public IApprovalHandler CreateFastTrackChain()
        {
            var hrReview = new HRReviewHandler();
            var managerApproval = new ManagerApprovalHandler();

            hrReview.SetNext(managerApproval);

            return hrReview;
        }
    }
}
