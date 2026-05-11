namespace OnlineJobs.Application.ReportTemplates
{
    public class ReportResult
    {
        public bool Success { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public string ErrorMessage { get; set; }

        public ReportResult()
        {
            FileName = string.Empty;
            Content = Array.Empty<byte>();
            ContentType = string.Empty;
            ErrorMessage = string.Empty;
        }

        public static ReportResult SuccessResult(string fileName, byte[] content, string contentType)
        {
            return new ReportResult
            {
                Success = true,
                FileName = fileName,
                Content = content,
                ContentType = contentType
            };
        }

        public static ReportResult FailureResult(string errorMessage)
        {
            return new ReportResult
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }
}
