namespace OnlineJobs.Application.Reporting
{

    public interface IReportExporter
    {
        string Format { get; }
        string FileExtension { get; }

        Task<string> ExportAsync(string reportTitle, Dictionary<string, object> data);
        string GenerateHeader(string title);
        string GenerateFooter();
    }
}
