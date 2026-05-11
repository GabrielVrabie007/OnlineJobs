namespace OnlineJobs.Application.Mediators
{
    public class SubmitApplicationRequest
    {
        public Guid JobId { get; set; }
        public Guid UserId { get; set; }
        public string CoverLetter { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public string? PortfolioLink { get; set; }
        public DateTime? AvailableStartDate { get; set; }

        public SubmitApplicationRequest(Guid jobId, Guid userId, string coverLetter)
        {
            JobId = jobId;
            UserId = userId;
            CoverLetter = coverLetter;
        }
    }
}
