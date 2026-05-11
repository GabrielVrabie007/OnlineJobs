using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Observers
{
    public class AuditLogObserver : IObserver
    {
        private readonly List<string> _auditLog = new();

        public List<string> AuditLog => _auditLog;

        public Task UpdateAsync(object data)
        {
            var timestamp = DateTime.UtcNow;
            string logEntry = string.Empty;

            if (data is JobPosting job)
            {
                logEntry = $"[{timestamp:yyyy-MM-dd HH:mm:ss}] Job Posted: {job.Id} - {job.Title}";
            }
            else if (data is JobApplication application)
            {
                logEntry = $"[{timestamp:yyyy-MM-dd HH:mm:ss}] Application Status Changed: {application.Id} - {application.Status}";
            }

            if (!string.IsNullOrEmpty(logEntry))
            {
                _auditLog.Add(logEntry);
                Console.WriteLine($"[Audit] {logEntry}");
            }

            return Task.CompletedTask;
        }
    }
}
