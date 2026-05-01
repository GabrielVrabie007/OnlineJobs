using System.Text;

namespace OnlineJobs.Application.Reporting.Exporters
{

    public class ExcelExporter : IReportExporter
    {
        public string Format => "Excel";
        public string FileExtension => ".xlsx";

        public async Task<string> ExportAsync(string reportTitle, Dictionary<string, object> data)
        {
            return await Task.Run(() =>
            {
                var sb = new StringBuilder();

                sb.AppendLine(GenerateHeader(reportTitle));
                sb.AppendLine("\n[Excel Spreadsheet]");
                sb.AppendLine("Row | Column A       | Column B");
                sb.AppendLine("----+----------------+-----------------");

                int row = 1;
                foreach (var kvp in data)
                {
                    sb.AppendLine($" {row++}  | {kvp.Key,-14} | {kvp.Value}");
                }

                sb.AppendLine(GenerateFooter());
                sb.AppendLine($"\n[Excel binary data - File: {reportTitle}{FileExtension}]");

                return sb.ToString();
            });
        }

        public string GenerateHeader(string title)
        {
            return $"╔══════════════════════════════════════════╗\n" +
                   $"║ EXCEL REPORT: {title.PadRight(23)} ║\n" +
                   $"╚══════════════════════════════════════════╝";
        }

        public string GenerateFooter()
        {
            return $"\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                   $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                   $"Format: Excel XLSX";
        }
    }
}
