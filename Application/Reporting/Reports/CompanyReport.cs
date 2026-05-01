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

        public override async Task<Dictionary<string, object>> GenerateDataAsync()
        {
            var companies = await _companyService.GetAllCompaniesAsync();

            var data = new Dictionary<string, object>
            {
                { "Total Companies", companies.Count() },
                { "Report Date", DateTime.Now.ToString("yyyy-MM-dd") },
                { "Average Employees", ((int)companies.Average(c => c.EmployeeCount)).ToString() },
                { "Largest Company", companies.OrderByDescending(c => c.EmployeeCount).First().Name },
                { "Top Industry", companies.GroupBy(c => c.Industry).OrderByDescending(g => g.Count()).First().Key },
                { "Companies with Website", companies.Count(c => !string.IsNullOrEmpty(c.Website)) },
                { "Total Employees", companies.Sum(c => c.EmployeeCount).ToString() }
            };

            return data;
        }
    }
}
