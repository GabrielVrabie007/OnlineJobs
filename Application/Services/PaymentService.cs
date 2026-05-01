using OnlineJobs.Application.Adapters;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;
using OnlineJobs.Domain.Interfaces;

namespace OnlineJobs.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IRepository<PaymentTransaction> _paymentRepository;
    private readonly Dictionary<PaymentGateway, IPaymentProcessor> _paymentProcessors;

    public PaymentService(IRepository<PaymentTransaction> paymentRepository)
    {
        _paymentRepository = paymentRepository;


        _paymentProcessors = new Dictionary<PaymentGateway, IPaymentProcessor>
        {
            { PaymentGateway.PayPal, new PayPalAdapter() },
            { PaymentGateway.Stripe, new StripeAdapter() },
            { PaymentGateway.GooglePay, new GooglePayAdapter() }
        };
    }


    public async Task<PaymentTransaction> ProcessPaymentAsync(Guid userId, decimal amount, PaymentGateway gateway, string description)
    {
        var payment = new PaymentTransaction
        {
            UserId = userId,
            Amount = amount,
            Currency = "USD",
            Gateway = gateway,
            Status = PaymentStatus.Processing,
            Description = description
        };

        await _paymentRepository.AddAsync(payment);

        try
        {
            var processor = _paymentProcessors[gateway];
            
            var result = await processor.ProcessPaymentAsync(
                userEmail: $"user{userId}@example.com",
                amount: amount,
                currency: "USD",
                description: description
            );

            if (result.Success)
            {
                payment.MarkAsCompleted(result.TransactionId);
                Console.WriteLine($"✓ Payment successful via {gateway}: {result.TransactionId}");
            }
            else
            {
                payment.MarkAsFailed(result.ErrorMessage ?? "Unknown error");
                Console.WriteLine($"✗ Payment failed via {gateway}: {result.ErrorMessage}");
            }

            await _paymentRepository.UpdateAsync(payment);
        }
        catch (Exception ex)
        {
            payment.MarkAsFailed(ex.Message);
            await _paymentRepository.UpdateAsync(payment);
            Console.WriteLine($"✗ Payment exception via {gateway}: {ex.Message}");
        }

        return payment;
    }

  
    public async Task<PaymentTransaction?> GetPaymentByIdAsync(Guid paymentId)
    {
        return await _paymentRepository.GetByIdAsync(paymentId);
    }

    public async Task<IEnumerable<PaymentTransaction>> GetUserPaymentsAsync(Guid userId)
    {
        return await _paymentRepository.FindAsync(p => p.UserId == userId);
    }
}