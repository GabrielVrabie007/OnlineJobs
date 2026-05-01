using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Interfaces;


public interface ICompanyRevealService
{

    Task<CompanyReveal?> PurchaseCompanyRevealAsync(Guid jobSeekerId, Guid jobPostingId, PaymentGateway gateway);


    Task<bool> HasAccessToCompanyAsync(Guid jobSeekerId, Guid jobPostingId);


    Task<IEnumerable<CompanyReveal>> GetJobSeekerRevealsAsync(Guid jobSeekerId);


    decimal GetRevealPrice();
}