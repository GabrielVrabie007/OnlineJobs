namespace OnlineJobs.Application.ReportTemplates
{
    public class ReportData<T>
    {
        public string Title { get; set; }
        public DateTime GeneratedAt { get; set; }
        public List<T> Data { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        public ReportData()
        {
            Title = string.Empty;
            GeneratedAt = DateTime.UtcNow;
            Data = new List<T>();
            Metadata = new Dictionary<string, object>();
        }
    }
}
