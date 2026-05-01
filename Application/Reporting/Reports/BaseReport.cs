namespace OnlineJobs.Application.Reporting.Reports
{

    public abstract class BaseReport : IReport
    {
        public abstract string Title { get; }
        public IReportExporter Exporter { get; set; }

        protected BaseReport(IReportExporter exporter)
        {
            Exporter = exporter ?? throw new ArgumentNullException(nameof(exporter));
        }

        public abstract Task<Dictionary<string, object>> GenerateDataAsync();

        public virtual async Task<string> ExportAsync()
        {
            var data = await GenerateDataAsync();
            return await Exporter.ExportAsync(Title, data);
        }
    }
}
