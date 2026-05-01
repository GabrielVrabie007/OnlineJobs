using System.Text;

namespace OnlineJobs.Application.Reporting.Exporters
{
    /// <summary>
    /// Bridge Pattern - LAB 5
    /// Concrete Implementor - CSV export format.
    /// </summary>
    public class CSVExporter : IReportExporter
    {
        public string Format => "CSV";
        public string FileExtension => ".csv";

        public async Task<string> ExportAsync(string reportTitle, Dictionary<string, object> data)
        {
            return await Task.Run(() =>
            {
                var sb = new StringBuilder();

                sb.AppendLine(GenerateHeader(reportTitle));
                sb.AppendLine("\n\"Field\",\"Value\"");

                foreach (var kvp in data)
                {
                    // Escape quotes in CSV
                    var field = kvp.Key.Replace("\"", "\"\"");
                    var value = kvp.Value?.ToString()?.Replace("\"", "\"\"") ?? "";

                    sb.AppendLine($"\"{field}\",\"{value}\"");
                }

                sb.AppendLine(GenerateFooter());

                return sb.ToString();
            });
        }

        public string GenerateHeader(string title)
        {
            return $"# CSV REPORT: {title}\n" +
                   $"# Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }

        public string GenerateFooter()
        {
            return $"\n# End of CSV report";
        }
    }
}
