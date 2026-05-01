namespace OnlineJobs.Application.ExternalGateways;


public class PayPalGatewaySDK
{

    public PayPalPaymentResponse CreatePayment(string email, double amount, string currencyCode, string description)
    {
        Console.WriteLine($"[PayPal SDK] Creating payment for {email}: {amount} {currencyCode}");

        Thread.Sleep(100);

        bool success = new Random().Next(100) < 95;

        return new PayPalPaymentResponse
        {
            PaymentId = $"PAYPAL-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
            Status = success ? "APPROVED" : "DECLINED",
            Timestamp = DateTime.UtcNow,
            ErrorCode = success ? null : "INSUFFICIENT_FUNDS"
        };
    }


    public string GetPaymentStatus(string paymentId)
    {
        return "APPROVED";
    }
}


public class PayPalPaymentResponse
{
    public string PaymentId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? ErrorCode { get; set; }
}