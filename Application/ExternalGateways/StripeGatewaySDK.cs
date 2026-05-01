namespace OnlineJobs.Application.ExternalGateways;

public class StripeGatewaySDK
{

    public StripeChargeResult Charge(int amountInCents, string currency, string customerEmail, string memo)
    {

        Thread.Sleep(150);

        bool success = new Random().Next(100) < 97;

        return new StripeChargeResult
        {
            ChargeId = $"ch_{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 24)}",
            Succeeded = success,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            FailureMessage = success ? null : "Card declined"
        };
    }


    public StripeChargeResult RetrieveCharge(string chargeId)
    {
        return new StripeChargeResult
        {
            ChargeId = chargeId,
            Succeeded = true,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
    }
}


public class StripeChargeResult
{
    public string ChargeId { get; set; } = string.Empty;
    public bool Succeeded { get; set; }
    public long CreatedAt { get; set; }
    public string? FailureMessage { get; set; }
}