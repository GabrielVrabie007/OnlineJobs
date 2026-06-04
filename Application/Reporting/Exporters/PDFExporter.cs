using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace OnlineJobs.Application.Reporting.Exporters
{
    /// <summary>Bridge Implementor — a real, print-ready PDF rendered with QuestPDF.</summary>
    public class PDFExporter : IReportExporter
    {
        private const string Ink = "#0E1B2D";
        private const string Muted = "#64748B";
        private const string Line = "#E6E9EE";

        static PDFExporter()
        {
            // QuestPDF Community licence (free for this use).
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public string Format => "PDF";
        public string FileExtension => ".pdf";
        public string ContentType => "application/pdf";

        public byte[] Export(ReportDocument document)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(28);
                    page.DefaultTextStyle(t => t.FontSize(9).FontColor(Ink));

                    page.Header().Column(header =>
                    {
                        header.Item().Text(document.Title).FontSize(18).Bold().FontColor(Ink);
                        header.Item().Text($"Generated {document.GeneratedAt:dddd, dd MMMM yyyy 'at' HH:mm}")
                            .FontSize(9).FontColor(Muted);
                    });

                    page.Content().PaddingVertical(14).Column(content =>
                    {
                        // Summary chips
                        content.Item().PaddingBottom(12).Row(row =>
                        {
                            foreach (var kvp in document.Summary)
                            {
                                row.RelativeItem().Border(0.5f).BorderColor(Line).Padding(8).Column(c =>
                                {
                                    c.Item().Text(kvp.Value).FontSize(15).Bold().FontColor(Ink);
                                    c.Item().Text(kvp.Key).FontSize(7.5f).FontColor(Muted);
                                });
                            }
                        });

                        if (document.Columns.Count > 0)
                        {
                            content.Item().Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    foreach (var _ in document.Columns) cols.RelativeColumn();
                                });

                                foreach (var col in document.Columns)
                                    table.Cell().Background(Ink).Padding(5)
                                        .Text(col).FontColor("#FFFFFF").Bold().FontSize(8);

                                var alt = false;
                                foreach (var dataRow in document.Rows)
                                {
                                    var background = alt ? "#F7F8FA" : "#FFFFFF";
                                    alt = !alt;
                                    foreach (var cell in dataRow)
                                        table.Cell().Background(background).BorderBottom(0.5f).BorderColor(Line)
                                            .Padding(5).Text(cell ?? string.Empty).FontSize(8);
                                }
                            });
                        }
                        else
                        {
                            content.Item().Text("No records for this report.").FontColor(Muted).Italic();
                        }
                    });

                    page.Footer().AlignRight().Text(text =>
                    {
                        text.Span("OnlineJobs · ").FontSize(8).FontColor(Muted);
                        text.CurrentPageNumber().FontSize(8).FontColor(Muted);
                        text.Span(" / ").FontSize(8).FontColor(Muted);
                        text.TotalPages().FontSize(8).FontColor(Muted);
                    });
                });
            }).GeneratePdf();
        }
    }
}
