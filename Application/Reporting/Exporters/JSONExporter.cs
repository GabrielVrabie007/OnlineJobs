using System.Text;
using System.Text.Json;

namespace OnlineJobs.Application.Reporting.Exporters
{
    /// <summary>Bridge Implementor — valid, machine-readable JSON.</summary>
    public class JSONExporter : IReportExporter
    {
        public string Format => "JSON";
        public string FileExtension => ".json";
        public string ContentType => "application/json";

        public byte[] Export(ReportDocument document)
        {
            var payload = new
            {
                title = document.Title,
                generatedAt = document.GeneratedAt,
                summary = document.Summary.ToDictionary(k => k.Key, v => v.Value),
                columns = document.Columns,
                rows = document.Rows.Select(row =>
                    document.Columns
                        .Select((col, i) => new { col, val = i < row.Length ? row[i] : string.Empty })
                        .ToDictionary(x => x.col, x => x.val))
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
