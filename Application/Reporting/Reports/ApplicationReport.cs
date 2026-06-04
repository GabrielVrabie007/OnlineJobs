using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Reporting.Reports
{
    public class ApplicationReport : BaseReport
    {
        private readonly IApplicationService _applicationService;
        private readonly Guid? _employerId;

        public override string Title => "Applications Report";

        public ApplicationReport(IReportExporter exporter, IApplicationService applicationService, Guid? employerId = null)
            : base(exporter)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _employerId = employerId;
        }

        public override async Task<ReportDocument> BuildAsync()
        {
            var applications = _employerId.HasValue
                ? (await _applicationService.GetApplicationsByEmployerAsync(_employerId.Value)).ToList()
                : new List<Domain.Entities.JobApplication>();

            var doc = new ReportDocument { Title = Title };

            doc.AddSummary("Total applications", applications.Count.ToString());
            doc.AddSummary("Awaiting review", applications.Count(a => a.Status == ApplicationStatus.Submitted).ToString());
            doc.AddSummary("In progress", applications.Count(a =>
                a.Status == ApplicationStatus.UnderReview || a.Status == ApplicationStatus.Interviewing).ToString());
            doc.AddSummary("Hired", applications.Count(a => a.Status == ApplicationStatus.Accepted).ToString());
            doc.AddSummary("Rejected", applications.Count(a => a.Status == ApplicationStatus.Rejected).ToString());
            doc.AddSummary("Received this week", applications.Count(a => a.AppliedDate >= DateTime.Now.AddDays(-7)).ToString());

            doc.Columns.AddRange(new[] { "Candidate", "Job", "Status", "Applied" });
            foreach (var a in applications.OrderByDescending(a => a.AppliedDate))
            {
                var candidate = a.JobSeeker != null
                    ? $"{a.JobSeeker.FirstName} {a.JobSeeker.LastName}".Trim()
                    : "—";
                doc.Rows.Add(new[]
                {
                    string.IsNullOrWhiteSpace(candidate) ? "—" : candidate,
                    a.JobPosting?.Title ?? "—",
                    a.Status.ToString(),
                    a.AppliedDate.ToString("yyyy-MM-dd")
                });
            }

            return doc;
        }
    }
}
