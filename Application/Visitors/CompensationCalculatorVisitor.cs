using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Visitors
{
    public class CompensationCalculatorVisitor : IApplicationVisitor<CompensationPackage>
    {
        private const decimal StandardBenefitsPercentage = 0.20m;
        private const decimal StandardEquityPercentage = 0.05m;
        private const decimal StandardBonusPercentage = 0.10m;

        public CompensationPackage VisitJobPosting(JobPosting jobPosting)
        {
            var baseSalary = jobPosting.SalaryMin.HasValue && jobPosting.SalaryMax.HasValue
                ? (jobPosting.SalaryMin.Value + jobPosting.SalaryMax.Value) / 2
                : jobPosting.SalaryMin ?? jobPosting.SalaryMax ?? 0;

            var benefits = baseSalary * StandardBenefitsPercentage;
            var equity = baseSalary * StandardEquityPercentage;
            var bonus = baseSalary * StandardBonusPercentage;

            return new CompensationPackage
            {
                BaseSalary = baseSalary,
                BenefitsValue = benefits,
                EquityValue = equity,
                BonusValue = bonus,
                TotalValue = baseSalary + benefits + equity + bonus,
                Description = $"Total compensation package for {jobPosting.Title}"
            };
        }

        public CompensationPackage VisitJobApplication(JobApplication application)
        {
            var negotiatedSalary = application.ExpectedSalary ?? 0;

            var benefits = negotiatedSalary * StandardBenefitsPercentage;
            var equity = negotiatedSalary * StandardEquityPercentage;
            var bonus = negotiatedSalary * StandardBonusPercentage;

            return new CompensationPackage
            {
                BaseSalary = negotiatedSalary,
                BenefitsValue = benefits,
                EquityValue = equity,
                BonusValue = bonus,
                TotalValue = negotiatedSalary + benefits + equity + bonus,
                Description = $"Expected compensation for application {application.Id}"
            };
        }
    }
}
