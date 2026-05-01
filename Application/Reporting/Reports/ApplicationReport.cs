using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Reporting.Reports
{

    public class ApplicationReport : BaseReport
    {
        private readonly IApplicationService _applicationService;
        private readonly Guid? _employerId;

        public override string Title => _employerId.HasValue
            ? $"Applications Report - Employer {_employerId}"
            : "All Applications Report";

        public ApplicationReport(IReportExporter exporter, IApplicationService applicationService, Guid? employerId = null)
            : base(exporter)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _employerId = employerId;
        }

        public override async Task<Dictionary<string, object>> GenerateDataAsync()
        {
            IEnumerable<Domain.Entities.JobApplication> applications;

            if (_employerId.HasValue)
            {
                applications = await _applicationService.GetApplicationsByEmployerAsync(_employerId.Value);
            }
            else
            {
                applications = new List<Domain.Entities.JobApplication>(); // Placeholder
            }

            var data = new Dictionary<string, object>
            {
                { "Total Applications", applications.Count() },
                { "Report Date", DateTime.Now.ToString("yyyy-MM-dd") },
                { "Submitted", applications.Count(a => a.Status == ApplicationStatus.Submitted) },
                { "Under Review", applications.Count(a => a.Status == ApplicationStatus.UnderReview) },
                { "Interviewing", applications.Count(a => a.Status == ApplicationStatus.Interviewing) },
                { "Accepted", applications.Count(a => a.Status == ApplicationStatus.Accepted) },
                { "Rejected", applications.Count(a => a.Status == ApplicationStatus.Rejected) },
                { "Applications This Week", applications.Count(a => a.AppliedDate >= DateTime.Now.AddDays(-7)) }
            };

            return data;
        }
    }
}
