namespace OnlineJobs.Application.Mementos
{
    public class ApplicationFormMemento : IMemento
    {
        public Guid JobPostingId { get; }
        public Guid JobSeekerId { get; }
        public string CoverLetter { get; }
        public Dictionary<string, string> AdditionalFields { get; }
        public DateTime CreatedAt { get; }

        public ApplicationFormMemento(
            Guid jobPostingId,
            Guid jobSeekerId,
            string coverLetter,
            Dictionary<string, string>? additionalFields = null)
        {
            JobPostingId = jobPostingId;
            JobSeekerId = jobSeekerId;
            CoverLetter = coverLetter ?? string.Empty;
            AdditionalFields = additionalFields ?? new Dictionary<string, string>();
            CreatedAt = DateTime.UtcNow;
        }

        public string GetDescription()
        {
            return $"Application form snapshot from {CreatedAt:yyyy-MM-dd HH:mm:ss}";
        }
    }
}
