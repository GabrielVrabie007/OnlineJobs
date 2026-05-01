using OnlineJobs.Application.Reporting;
using OnlineJobs.Application.Reporting.Exporters;
using Xunit;

namespace OnlineJobs.Tests.Patterns
{

    public class BridgePatternTests
    {
        [Fact]
        public async Task PDFExporter_ShouldExportCorrectly()
        {
            IReportExporter exporter = new PDFExporter();
            var data = new Dictionary<string, object>
            {
                { "Field1", "Value1" },
                { "Field2", 123 }
            };

            var result = await exporter.ExportAsync("Test Report", data);

            Assert.Contains("PDF", result);
            Assert.Contains("Test Report", result);
            Assert.Contains("Field1", result);
        }

        [Fact]
        public async Task ExcelExporter_ShouldExportCorrectly()
        {
            // Arrange
            IReportExporter exporter = new ExcelExporter();
            var data = new Dictionary<string, object>
            {
                { "Total", 100 },
                { "Average", 50 }
            };

            // Act
            var result = await exporter.ExportAsync("Stats Report", data);

            // Assert
            Assert.Contains("Excel", result);
            Assert.Contains("Stats Report", result);
        }

        [Fact]
        public async Task JSONExporter_ShouldProduceValidJSON()
        {
            // Arrange
            IReportExporter exporter = new JSONExporter();
            var data = new Dictionary<string, object>
            {
                { "Name", "Test" },
                { "Count", 42 }
            };

            // Act
            var result = await exporter.ExportAsync("JSON Report", data);

            // Assert
            Assert.Contains("\"Name\"", result);
            Assert.Contains("\"Count\"", result);
            Assert.Contains("JSON Report", result);
        }

        [Fact]
        public async Task CSVExporter_ShouldProduceCSV()
        {
            // Arrange
            IReportExporter exporter = new CSVExporter();
            var data = new Dictionary<string, object>
            {
                { "Header1", "ValueA" },
                { "Header2", "ValueB" }
            };

            // Act
            var result = await exporter.ExportAsync("CSV Report", data);

            // Assert
            Assert.Contains("\"Field\",\"Value\"", result);
            Assert.Contains("Header1", result);
        }

        [Fact]
        public void AllExporters_ShouldHaveCorrectFormat()
        {
            // Arrange & Act
            var pdf = new PDFExporter();
            var excel = new ExcelExporter();
            var json = new JSONExporter();
            var csv = new CSVExporter();

            // Assert
            Assert.Equal("PDF", pdf.Format);
            Assert.Equal("Excel", excel.Format);
            Assert.Equal("JSON", json.Format);
            Assert.Equal("CSV", csv.Format);
        }

        [Fact]
        public void AllExporters_ShouldHaveCorrectFileExtension()
        {
            // Arrange & Act
            var pdf = new PDFExporter();
            var excel = new ExcelExporter();
            var json = new JSONExporter();
            var csv = new CSVExporter();

            // Assert
            Assert.Equal(".pdf", pdf.FileExtension);
            Assert.Equal(".xlsx", excel.FileExtension);
            Assert.Equal(".json", json.FileExtension);
            Assert.Equal(".csv", csv.FileExtension);
        }
    }
}
