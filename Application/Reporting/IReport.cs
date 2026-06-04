namespace OnlineJobs.Application.Reporting
{
    /// <summary>
    /// Bridge pattern — Abstraction. A report type (jobs, applications, companies) that
    /// builds its data once and delegates file rendering to an <see cref="IReportExporter"/>.
    /// </summary>
    public interface IReport
    {
        string Title { get; }
        IReportExporter Exporter { get; set; }

        Task<ReportDocument> BuildAsync();
        Task<ExportedFile> ExportAsync();
    }
}
