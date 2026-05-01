using System.Text;

namespace OnlineJobs.Application.Reporting.Exporters
{

    public class PDFExporter : IReportExporter
    {
        public string Format => "PDF";
        public string FileExtension => ".pdf";

        public async Task<string> ExportAsync(string reportTitle, Dictionary<string, object> data)
        {
            return await Task.Run(() =>
            {
                var sb = new StringBuilder();

                sb.AppendLine(GenerateHeader(reportTitle));
                sb.AppendLine("\n[PDF Content]");

                foreach (var kvp in data)
                {
                    sb.AppendLine($"  • {kvp.Key}: {kvp.Value}");
                }

                sb.AppendLine(GenerateFooter());
                sb.AppendLine($"\n[Binary PDF data would be here - File: {reportTitle}{FileExtension}]");

                return sb.ToString();
            });
        }

        public string GenerateHeader(string title)
        {
            return $"╔══════════════════════════════════════════╗\n" +
                   $"║  PDF REPORT: {title.PadRight(24)} ║\n" +
                   $"╚══════════════════════════════════════════╝";
        }

        public string GenerateFooter()
        {
            return $"\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                   $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                   $"Format: PDF | Page 1 of 1";
        }
    }
}
