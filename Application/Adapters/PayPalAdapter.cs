using OnlineJobs.Application.ExternalGateways;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Adapters;

public class PayPalAdapter : IPaymentProcessor
{
    private readonly PayPalGatewaySDK _payPalSdk;

    public PaymentGateway Gateway => PaymentGateway.PayPal;

    public PayPalAdapter()
    {
        _payPalSdk = new PayPalGatewaySDK();
    }


    public async Task<PaymentResult> ProcessPaymentAsync(string userEmail, decimal amount, string currency, string description)
    {
        return await Task.Run(() =>
        {
            try
            {
                var response = _payPalSdk.CreatePayment(
                    email: userEmail,
                    amount: (double)amount,
                    currencyCode: currency,
                    description: description
                );

                if (response.Status == "APPROVED")
                {
                    return PaymentResult.Successful(response.PaymentId);
                }
                else
                {
                    return PaymentResult.Failed(response.ErrorCode ?? "Payment declined by PayPal");
                }
            }
            catch (Exception ex)
            {
                return PaymentResult.Failed($"PayPal error: {ex.Message}");
            }
        });
    }

  
    public async Task<bool> VerifyPaymentAsync(string transactionId)
    {
        return await Task.Run(() =>
        {
            try
            {
                var status = _payPalSdk.GetPaymentStatus(transactionId);
                return status == "APPROVED";
            }
            catch
            {
                return false;
            }
        });
    }
}