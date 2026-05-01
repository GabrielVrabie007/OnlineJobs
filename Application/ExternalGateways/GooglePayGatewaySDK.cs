namespace OnlineJobs.Application.ExternalGateways;


public class GooglePayGatewaySDK
{

    public GooglePayTransactionResult ProcessTransaction(GooglePayRequest request)
    {
        Console.WriteLine($"[Google Pay SDK] Processing transaction for {request.UserEmail}: {request.TotalAmount} {request.CurrencyCode}");

        Thread.Sleep(120);

        bool success = new Random().Next(100) < 96;

        return new GooglePayTransactionResult
        {
            TransactionReference = $"GP-{DateTime.UtcNow.Ticks}-{new Random().Next(1000, 9999)}",
            ResultCode = success ? 0 : 1001,
            ResultMessage = success ? "SUCCESS" : "PAYMENT_FAILED",
            ProcessedTimestamp = DateTime.UtcNow.ToString("o")
        };
    }


    public bool VerifyTransaction(string transactionReference)
    {
        Console.WriteLine($"[Google Pay SDK] Verifying transaction {transactionReference}");
        return true;
    }
}


public class GooglePayRequest
{
    public string UserEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public string Description { get; set; } = string.Empty;
}


public class GooglePayTransactionResult
{
    public string TransactionReference { get; set; } = string.Empty;
    public int ResultCode { get; set; }
    public string ResultMessage { get; set; } = string.Empty;
    public string ProcessedTimestamp { get; set; } = string.Empty;
}