using ClosedXML.Excel;

namespace OnlineJobs.Application.Reporting.Exporters
{
    /// <summary>Bridge Implementor — a real .xlsx workbook (Summary + Records sheets).</summary>
    public class ExcelExporter : IReportExporter
    {
        public string Format => "Excel";
        public string FileExtension => ".xlsx";
        public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public byte[] Export(ReportDocument document)
        {
            using var workbook = new XLWorkbook();

            // --- Summary sheet ---
            var summary = workbook.Worksheets.Add("Summary");
            summary.Cell(1, 1).Value = document.Title;
            summary.Cell(1, 1).Style.Font.Bold = true;
            summary.Cell(1, 1).Style.Font.FontSize = 16;
            summary.Cell(2, 1).Value = $"Generated {document.GeneratedAt:yyyy-MM-dd HH:mm}";
            summary.Cell(2, 1).Style.Font.FontColor = XLColor.Gray;

            var r = 4;
            foreach (var kvp in document.Summary)
            {
                summary.Cell(r, 1).Value = kvp.Key;
                summary.Cell(r, 1).Style.Font.Bold = true;
                summary.Cell(r, 2).Value = kvp.Value;
                r++;
            }
            summary.Columns().AdjustToContents();

            // --- Records sheet ---
            if (document.Columns.Count > 0)
            {
                var sheet = workbook.Worksheets.Add("Records");
                for (var c = 0; c < document.Columns.Count; c++)
                {
                    var cell = sheet.Cell(1, c + 1);
                    cell.Value = document.Columns[c];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#0E1B2D");
                    cell.Style.Font.FontColor = XLColor.White;
                }

                for (var i = 0; i < document.Rows.Count; i++)
                    for (var c = 0; c < document.Rows[i].Length; c++)
                        sheet.Cell(i + 2, c + 1).Value = document.Rows[i][c];

                sheet.Range(1, 1, document.Rows.Count + 1, document.Columns.Count)
                     .SetAutoFilter();
                sheet.SheetView.FreezeRows(1);
                sheet.Columns().AdjustToContents();
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
