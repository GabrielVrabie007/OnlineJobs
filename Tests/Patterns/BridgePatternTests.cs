using System.Text;
using System.Text.Json;
using OnlineJobs.Application.Reporting;
using OnlineJobs.Application.Reporting.Exporters;
using Xunit;

namespace OnlineJobs.Tests.Patterns
{
    public class BridgePatternTests
    {
        private static ReportDocument SampleDocument()
        {
            var doc = new ReportDocument { Title = "Test Report" };
            doc.AddSummary("Total", "100");
            doc.AddSummary("Average", "50");
            doc.Columns.AddRange(new[] { "Name", "Count" });
            doc.Rows.Add(new[] { "Alice", "42" });
            doc.Rows.Add(new[] { "Bob, Jr.", "7" }); // comma to exercise CSV escaping
            return doc;
        }

        [Fact]
        public void PdfExporter_ProducesRealPdfBytes()
        {
            var bytes = new PDFExporter().Export(SampleDocument());
            Assert.NotEmpty(bytes);
            // PDF files start with "%PDF".
            Assert.Equal("%PDF", Encoding.ASCII.GetString(bytes, 0, 4));
        }

        [Fact]
        public void ExcelExporter_ProducesRealXlsxBytes()
        {
            var bytes = new ExcelExporter().Export(SampleDocument());
            Assert.NotEmpty(bytes);
            // .xlsx is a ZIP archive — starts with "PK".
            Assert.Equal(0x50, bytes[0]);
            Assert.Equal(0x4B, bytes[1]);
        }

        [Fact]
        public void JsonExporter_ProducesValidJson()
        {
            var bytes = new JSONExporter().Export(SampleDocument());
            var json = Encoding.UTF8.GetString(bytes);

            using var parsed = JsonDocument.Parse(json); // throws if invalid → test fails
            Assert.Equal("Test Report", parsed.RootElement.GetProperty("title").GetString());
            Assert.Equal("100", parsed.RootElement.GetProperty("summary").GetProperty("Total").GetString());
            Assert.Equal(2, parsed.RootElement.GetProperty("rows").GetArrayLength());
        }

        [Fact]
        public void CsvExporter_EscapesAndIncludesRecords()
        {
            var bytes = new CSVExporter().Export(SampleDocument());
            var csv = Encoding.UTF8.GetString(bytes);

            Assert.Contains("Name,Count", csv);     // header row
            Assert.Contains("Alice,42", csv);        // data row
            Assert.Contains("\"Bob, Jr.\"", csv);    // comma value is quoted
        }

        [Fact]
        public void AllExporters_ExposeFormatExtensionAndContentType()
        {
            var exporters = new IReportExporter[]
            {
                new PDFExporter(), new ExcelExporter(), new JSONExporter(), new CSVExporter()
            };
            Assert.Equal(new[] { "PDF", "Excel", "JSON", "CSV" }, exporters.Select(e => e.Format));
            Assert.Equal(new[] { ".pdf", ".xlsx", ".json", ".csv" }, exporters.Select(e => e.FileExtension));
            Assert.All(exporters, e => Assert.False(string.IsNullOrWhiteSpace(e.ContentType)));
        }
    }
}
