using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Domain.Entities;


public class PaymentTransaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentGateway Gateway { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? ExternalTransactionId { get; set; }
    public string? Description { get; set; }
    public string? ErrorMessage { get; set; }

    public PaymentTransaction()
    {
        Id = Guid.NewGuid();
        TransactionDate = DateTime.UtcNow;
        Status = PaymentStatus.Pending;
    }


    public void MarkAsCompleted(string externalTransactionId)
    {
        Status = PaymentStatus.Completed;
        ExternalTransactionId = externalTransactionId;
    }

  
    public void MarkAsFailed(string errorMessage)
    {
        Status = PaymentStatus.Failed;
        ErrorMessage = errorMessage;
    }

  
    public bool IsSuccessful() => Status == PaymentStatus.Completed;
}