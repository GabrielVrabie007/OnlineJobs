using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Interfaces;


public interface IPaymentProcessor
{

    PaymentGateway Gateway { get; }
    Task<PaymentResult> ProcessPaymentAsync(string userEmail, decimal amount, string currency, string description);
    Task<bool> VerifyPaymentAsync(string transactionId);
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public DateTime ProcessedAt { get; set; }

    public static PaymentResult Successful(string transactionId)
    {
        return new PaymentResult
        {
            Success = true,
            TransactionId = transactionId,
            ProcessedAt = DateTime.UtcNow
        };
    }

    public static PaymentResult Failed(string errorMessage)
    {
        return new PaymentResult
        {
            Success = false,
            ErrorMessage = errorMessage,
            ProcessedAt = DateTime.UtcNow
        };
    }
}