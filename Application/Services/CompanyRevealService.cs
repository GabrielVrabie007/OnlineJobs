using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;
using OnlineJobs.Domain.Interfaces;

namespace OnlineJobs.Application.Services;


public class CompanyRevealService : ICompanyRevealService
{
    private readonly IRepository<CompanyReveal> _revealRepository;
    private readonly IPaymentService _paymentService;
    private readonly IRepository<JobPosting> _jobRepository;
    private const decimal REVEAL_PRICE = 4.99m;

    public CompanyRevealService(
        IRepository<CompanyReveal> revealRepository,
        IPaymentService paymentService,
        IRepository<JobPosting> jobRepository)
    {
        _revealRepository = revealRepository;
        _paymentService = paymentService;
        _jobRepository = jobRepository;
    }
    
    public async Task<CompanyReveal?> PurchaseCompanyRevealAsync(Guid jobSeekerId, Guid jobPostingId, PaymentGateway gateway)
    {
        if (await HasAccessToCompanyAsync(jobSeekerId, jobPostingId))
        {
            Console.WriteLine($"Job seeker {jobSeekerId} already has access to job {jobPostingId}");
            return null;
        }

        var job = await _jobRepository.GetByIdAsync(jobPostingId);
        if (job == null)
        {
            Console.WriteLine($"Job posting {jobPostingId} not found");
            return null;
        }

        var payment = await _paymentService.ProcessPaymentAsync(
            userId: jobSeekerId,
            amount: REVEAL_PRICE,
            gateway: gateway,
            description: $"Reveal company for job: {job.Title}"
        );

        if (payment.IsSuccessful())
        {
            var reveal = new CompanyReveal
            {
                JobSeekerId = jobSeekerId,
                JobPostingId = jobPostingId,
                PaymentTransactionId = payment.Id
            };

            await _revealRepository.AddAsync(reveal);

            Console.WriteLine($"✓ Company revealed for job seeker {jobSeekerId} on job {jobPostingId}");
            return reveal;
        }

        Console.WriteLine($"✗ Payment failed, company not revealed");
        return null;
    }

    public async Task<bool> HasAccessToCompanyAsync(Guid jobSeekerId, Guid jobPostingId)
    {
        var reveals = await _revealRepository.FindAsync(r =>
            r.JobSeekerId == jobSeekerId &&
            r.JobPostingId == jobPostingId);

        return reveals.Any();
    }


    public async Task<IEnumerable<CompanyReveal>> GetJobSeekerRevealsAsync(Guid jobSeekerId)
    {
        return await _revealRepository.FindAsync(r => r.JobSeekerId == jobSeekerId);
    }


    public decimal GetRevealPrice()
    {
        return REVEAL_PRICE;
    }
}