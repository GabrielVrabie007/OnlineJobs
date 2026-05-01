using System.Text;
using System.Text.Json;

namespace OnlineJobs.Application.Reporting.Exporters
{

    public class JSONExporter : IReportExporter
    {
        public string Format => "JSON";
        public string FileExtension => ".json";

        public async Task<string> ExportAsync(string reportTitle, Dictionary<string, object> data)
        {
            return await Task.Run(() =>
            {
                var sb = new StringBuilder();

                sb.AppendLine(GenerateHeader(reportTitle));

                var reportData = new
                {
                    Title = reportTitle,
                    GeneratedAt = DateTime.UtcNow,
                    Format = "JSON",
                    Data = data
                };

                var json = JsonSerializer.Serialize(reportData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                sb.AppendLine(json);
                sb.AppendLine(GenerateFooter());

                return sb.ToString();
            });
        }

        public string GenerateHeader(string title)
        {
            return $"// JSON REPORT: {title}\n" +
                   $"// Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }

        public string GenerateFooter()
        {
            return $"\n// End of JSON report";
        }
    }
}
