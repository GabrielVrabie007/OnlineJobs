using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Interfaces;


public interface IPaymentService
{

    Task<PaymentTransaction> ProcessPaymentAsync(Guid userId, decimal amount, PaymentGateway gateway, string description);
    
    Task<PaymentTransaction?> GetPaymentByIdAsync(Guid paymentId);
    
    Task<IEnumerable<PaymentTransaction>> GetUserPaymentsAsync(Guid userId);
}