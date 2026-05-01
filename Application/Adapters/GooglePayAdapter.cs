using OnlineJobs.Application.ExternalGateways;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Adapters;


public class GooglePayAdapter : IPaymentProcessor
{
    private readonly GooglePayGatewaySDK _googlePaySdk;

    public PaymentGateway Gateway => PaymentGateway.GooglePay;

    public GooglePayAdapter()
    {
        _googlePaySdk = new GooglePayGatewaySDK();
    }
    
    public async Task<PaymentResult> ProcessPaymentAsync(string userEmail, decimal amount, string currency, string description)
    {
        return await Task.Run(() =>
        {
            try
            {
                var request = new GooglePayRequest
                {
                    UserEmail = userEmail,
                    TotalAmount = amount,
                    CurrencyCode = currency,
                    Description = description
                };

                var result = _googlePaySdk.ProcessTransaction(request);
                
                if (result.ResultCode == 0)
                {
                    return PaymentResult.Successful(result.TransactionReference);
                }
                else
                {
                    return PaymentResult.Failed($"Google Pay error: {result.ResultMessage}");
                }
            }
            catch (Exception ex)
            {
                return PaymentResult.Failed($"Google Pay error: {ex.Message}");
            }
        });
    }

    public async Task<bool> VerifyPaymentAsync(string transactionId)
    {
        return await Task.Run(() =>
        {
            try
            {
                return _googlePaySdk.VerifyTransaction(transactionId);
            }
            catch
            {
                return false;
            }
        });
    }
}