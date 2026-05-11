using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.ReportTemplates
{
    public abstract class ApplicationReportTemplate : BaseReportTemplate<JobApplication>
    {
        protected readonly IRepository<JobApplication> _applicationRepository;

        protected ApplicationReportTemplate(IRepository<JobApplication> applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        protected override async Task<IEnumerable<JobApplication>> FetchDataAsync()
        {
            return await _applicationRepository.GetAllAsync();
        }

        protected override IEnumerable<JobApplication> ProcessData(IEnumerable<JobApplication> data)
        {
            return data.OrderByDescending(a => a.AppliedDate);
        }
    }
}
