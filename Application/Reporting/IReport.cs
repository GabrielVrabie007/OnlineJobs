namespace OnlineJobs.Application.Reporting
{

    public interface IReport
    {
        string Title { get; }
        IReportExporter Exporter { get; set; }

        Task<Dictionary<string, object>> GenerateDataAsync();
        Task<string> ExportAsync();
    }
}
