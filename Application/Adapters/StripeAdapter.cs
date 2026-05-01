using OnlineJobs.Application.ExternalGateways;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Adapters;


public class StripeAdapter : IPaymentProcessor
{
    private readonly StripeGatewaySDK _stripeSdk;

    public PaymentGateway Gateway => PaymentGateway.Stripe;

    public StripeAdapter()
    {
        _stripeSdk = new StripeGatewaySDK();
    }


    public async Task<PaymentResult> ProcessPaymentAsync(string userEmail, decimal amount, string currency, string description)
    {
        return await Task.Run(() =>
        {
            try
            {
                int amountInCents = (int)(amount * 100);

                var result = _stripeSdk.Charge(
                    amountInCents: amountInCents,
                    currency: currency.ToLower(), 
                    customerEmail: userEmail,
                    memo: description
                );

                if (result.Succeeded)
                {
                    return PaymentResult.Successful(result.ChargeId);
                }
                else
                {
                    return PaymentResult.Failed(result.FailureMessage ?? "Payment declined by Stripe");
                }
            }
            catch (Exception ex)
            {
                return PaymentResult.Failed($"Stripe error: {ex.Message}");
            }
        });
    }


    public async Task<bool> VerifyPaymentAsync(string transactionId)
    {
        return await Task.Run(() =>
        {
            try
            {
                var charge = _stripeSdk.RetrieveCharge(transactionId);
                return charge.Succeeded;
            }
            catch
            {
                return false;
            }
        });
    }
}