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

        public override async Task<Dictionary<string, object>> GenerateDataAsync()
        {
            var jobs = await _jobService.GetActiveJobsAsync();

            var data = new Dictionary<string, object>
            {
                { "Total Active Jobs", jobs.Count() },
                { "Report Date", DateTime.Now.ToString("yyyy-MM-dd") },
                { "Average Salary Min", jobs.Where(j => j.SalaryMin.HasValue).Average(j => j.SalaryMin ?? 0).ToString("C0") },
                { "Average Salary Max", jobs.Where(j => j.SalaryMax.HasValue).Average(j => j.SalaryMax ?? 0).ToString("C0") },
                { "Top Category", jobs.GroupBy(j => j.Category).OrderByDescending(g => g.Count()).First().Key },
                { "Top Location", jobs.GroupBy(j => j.Location).OrderByDescending(g => g.Count()).First().Key },
                { "Jobs Posted This Month", jobs.Count(j => j.PostedDate >= DateTime.Now.AddMonths(-1)) },
                { "Total Applications", jobs.Sum(j => j.GetApplicationCount()) }
            };

            return data;
        }
    }
}
