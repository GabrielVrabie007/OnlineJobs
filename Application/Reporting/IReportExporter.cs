namespace OnlineJobs.Application.Reporting
{
    /// <summary>
    /// Bridge pattern — Implementor. A concrete export format (CSV, JSON, Excel, PDF)
    /// that turns a <see cref="ReportDocument"/> into real file bytes.
    /// </summary>
    public interface IReportExporter
    {
        string Format { get; }        // e.g. "PDF"
        string FileExtension { get; } // e.g. ".pdf"
        string ContentType { get; }   // MIME type for the HTTP response

        byte[] Export(ReportDocument document);
    }
}
