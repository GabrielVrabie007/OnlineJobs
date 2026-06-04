using System.Text;

namespace OnlineJobs.Application.Reporting.Exporters
{
    /// <summary>Bridge Implementor — clean, valid CSV that opens directly in Excel.</summary>
    public class CSVExporter : IReportExporter
    {
        public string Format => "CSV";
        public string FileExtension => ".csv";
        public string ContentType => "text/csv";

        public byte[] Export(ReportDocument document)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Escape(document.Title));
            sb.AppendLine($"Generated,{document.GeneratedAt:yyyy-MM-dd HH:mm}");
            sb.AppendLine();

            sb.AppendLine("Summary");
            foreach (var kvp in document.Summary)
                sb.AppendLine($"{Escape(kvp.Key)},{Escape(kvp.Value)}");
            sb.AppendLine();

            if (document.Columns.Count > 0)
            {
                sb.AppendLine("Records");
                sb.AppendLine(string.Join(",", document.Columns.Select(Escape)));
                foreach (var row in document.Rows)
                    sb.AppendLine(string.Join(",", row.Select(Escape)));
            }

            // UTF-8 BOM so Excel renders accents/symbols correctly.
            return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        }

        private static string Escape(string? value)
        {
            value ??= string.Empty;
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }
    }
}
