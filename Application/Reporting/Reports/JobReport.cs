using OnlineJobs.Application.Interfaces;

namespace OnlineJobs.Application.Reporting.Reports
{
    public class JobReport : BaseReport
    {
        private readonly IJobService _jobService;
        public override string Title => "Job Postings Report";

        public JobReport(IReportExporter exporter, IJobService jobService)
            : base(exporter)
        {
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
        }

        public override async Task<ReportDocument> BuildAsync()
        {
            var jobs = (await _jobService.GetActiveJobsAsync()).ToList();
            var doc = new ReportDocument { Title = Title };

            var withMin = jobs.Where(j => j.SalaryMin.HasValue).Select(j => j.SalaryMin!.Value).ToList();
            var withMax = jobs.Where(j => j.SalaryMax.HasValue).Select(j => j.SalaryMax!.Value).ToList();

            doc.AddSummary("Active job postings", jobs.Count.ToString());
            doc.AddSummary("Total applications", jobs.Sum(j => j.GetApplicationCount()).ToString());
            doc.AddSummary("Posted in last 30 days", jobs.Count(j => j.PostedDate >= DateTime.Now.AddMonths(-1)).ToString());
            doc.AddSummary("Average minimum salary", withMin.Any() ? withMin.Average().ToString("C0") : "—");
            doc.AddSummary("Average maximum salary", withMax.Any() ? withMax.Average().ToString("C0") : "—");
            doc.AddSummary("Most common category",
                jobs.Any() ? jobs.GroupBy(j => j.Category ?? "—").OrderByDescending(g => g.Count()).First().Key : "—");

            doc.Columns.AddRange(new[] { "Title", "Company", "Category", "Location", "Type", "Salary", "Status", "Posted", "Applications" });
            foreach (var j in jobs.OrderByDescending(j => j.PostedDate))
            {
                doc.Rows.Add(new[]
                {
                    j.Title,
                    j.Company?.Name ?? "—",
                    j.Category ?? "—",
                    string.IsNullOrWhiteSpace(j.Location) ? "—" : j.Location,
                    string.IsNullOrWhiteSpace(j.EmploymentType) ? "—" : j.EmploymentType,
                    FormatSalary(j.SalaryMin, j.SalaryMax),
                    j.Status.ToString(),
                    j.PostedDate.ToString("yyyy-MM-dd"),
                    j.GetApplicationCount().ToString()
                });
            }

            return doc;
        }

        private static string FormatSalary(decimal? min, decimal? max)
        {
            if (min.HasValue && max.HasValue) return $"{min:C0} - {max:C0}";
            if (min.HasValue) return $"from {min:C0}";
            if (max.HasValue) return $"up to {max:C0}";
            return "-";
        }
    }
}
