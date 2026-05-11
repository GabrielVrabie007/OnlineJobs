namespace OnlineJobs.Application.ReportTemplates
{
    public abstract class BaseReportTemplate<T> : IReportTemplate
    {
        public abstract string ReportName { get; }
        public abstract string FileExtension { get; }

        public async Task<ReportResult> GenerateReportAsync()
        {
            try
            {
                var rawData = await FetchDataAsync();

                ValidateData(rawData);

                var processedData = ProcessData(rawData);

                var reportData = FormatReportData(processedData);

                var result = await ExportReportAsync(reportData);

                return result;
            }
            catch (Exception ex)
            {
                return ReportResult.FailureResult($"Report generation failed: {ex.Message}");
            }
        }

        protected abstract Task<IEnumerable<T>> FetchDataAsync();

        protected virtual void ValidateData(IEnumerable<T> data)
        {
            if (data == null || !data.Any())
            {
                throw new InvalidOperationException("No data available for report");
            }
        }

        protected virtual IEnumerable<T> ProcessData(IEnumerable<T> data)
        {
            return data;
        }

        protected abstract ReportData<T> FormatReportData(IEnumerable<T> data);

        protected abstract Task<ReportResult> ExportReportAsync(ReportData<T> reportData);

        protected virtual void PreExportHook(ReportData<T> reportData)
        {
        }

        protected virtual void PostExportHook(ReportResult result)
        {
        }
    }
}
