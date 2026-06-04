using OnlineJobs.Application.Interfaces;

namespace OnlineJobs.Application.Reporting.Reports
{
    public class CompanyReport : BaseReport
    {
        private readonly ICompanyService _companyService;
        public override string Title => "Companies Report";

        public CompanyReport(IReportExporter exporter, ICompanyService companyService)
            : base(exporter)
        {
            _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
        }

        public override async Task<ReportDocument> BuildAsync()
        {
            var companies = (await _companyService.GetAllCompaniesAsync()).ToList();
            var doc = new ReportDocument { Title = Title };

            doc.AddSummary("Total companies", companies.Count.ToString());
            doc.AddSummary("Total employees", companies.Sum(c => c.EmployeeCount ?? 0).ToString("N0"));
            doc.AddSummary("Average company size",
                companies.Any() ? ((int)companies.Average(c => c.EmployeeCount ?? 0)).ToString("N0") : "—");
            doc.AddSummary("With a website", companies.Count(c => !string.IsNullOrEmpty(c.Website)).ToString());
            doc.AddSummary("Most common industry",
                companies.Any() ? companies.GroupBy(c => c.Industry ?? "—").OrderByDescending(g => g.Count()).First().Key : "—");

            doc.Columns.AddRange(new[] { "Company", "Industry", "Location", "Employees", "Website" });
            foreach (var c in companies.OrderByDescending(c => c.EmployeeCount))
            {
                doc.Rows.Add(new[]
                {
                    c.Name,
                    string.IsNullOrWhiteSpace(c.Industry) ? "—" : c.Industry,
                    string.IsNullOrWhiteSpace(c.Location) ? "—" : c.Location,
                    (c.EmployeeCount ?? 0).ToString("N0"),
                    string.IsNullOrWhiteSpace(c.Website) ? "—" : c.Website
                });
            }

            return doc;
        }
    }
}
