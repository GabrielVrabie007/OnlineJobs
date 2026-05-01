namespace OnlineJobs.Domain.Entities;

public class CompanyReveal
{
    public Guid Id { get; set; }
    public Guid JobSeekerId { get; set; }
    public Guid JobPostingId { get; set; }
    public Guid PaymentTransactionId { get; set; }
    public DateTime RevealedDate { get; set; }

    public JobSeeker? JobSeeker { get; set; }
    public JobPosting? JobPosting { get; set; }
    public PaymentTransaction? PaymentTransaction { get; set; }

    public CompanyReveal()
    {
        Id = Guid.NewGuid();
        RevealedDate = DateTime.UtcNow;
    }
    
    public bool IsValid()
    {
        return true;
    }
}