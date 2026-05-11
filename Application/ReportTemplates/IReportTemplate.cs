namespace OnlineJobs.Application.ReportTemplates
{
    public interface IReportTemplate
    {
        Task<ReportResult> GenerateReportAsync();
        string ReportName { get; }
        string FileExtension { get; }
    }
}
