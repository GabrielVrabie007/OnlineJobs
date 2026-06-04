namespace OnlineJobs.Application.Reporting
{
    /// <summary>
    /// Format-agnostic representation of a report: a few headline numbers (Summary)
    /// plus the actual records as a table (Columns + Rows). Every exporter renders
    /// this same document into its own file format — the heart of the Bridge pattern.
    /// </summary>
    public class ReportDocument
    {
        public string Title { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        /// <summary>Headline figures shown at the top of the report (label → value).</summary>
        public List<KeyValuePair<string, string>> Summary { get; } = new();

        /// <summary>Column headers for the records table.</summary>
        public List<string> Columns { get; } = new();

        /// <summary>The actual records, one string[] per row aligned to <see cref="Columns"/>.</summary>
        public List<string[]> Rows { get; } = new();

        public void AddSummary(string label, string value) => Summary.Add(new(label, value));
    }

    /// <summary>A generated file ready to stream to the browser.</summary>
    public class ExportedFile
    {
        public byte[] Content { get; }
        public string ContentType { get; }
        public string FileName { get; }

        public ExportedFile(byte[] content, string contentType, string fileName)
        {
            Content = content;
            ContentType = contentType;
            FileName = fileName;
        }
    }
}
