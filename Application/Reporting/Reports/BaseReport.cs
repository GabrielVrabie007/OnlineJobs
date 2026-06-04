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

        /// <summary>Subclasses assemble the report's summary + records here.</summary>
        public abstract Task<ReportDocument> BuildAsync();

        /// <summary>Bridge in action: build the data once, render it via the chosen exporter.</summary>
        public virtual async Task<ExportedFile> ExportAsync()
        {
            var document = await BuildAsync();
            var bytes = Exporter.Export(document);
            var safeTitle = string.Concat(Title.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
            var fileName = $"{safeTitle} - {DateTime.Now:yyyy-MM-dd}{Exporter.FileExtension}";
            return new ExportedFile(bytes, Exporter.ContentType, fileName);
        }
    }
}
