namespace OnlineJobs.Application.Strategies.SalaryStrategies
{
    /// <summary>
    /// Picks the salary-calculation Strategy that matches a job's employment type, so
    /// the same number is presented correctly (per-year, per-hour, per-project, …).
    /// This is the runtime selection that makes the Strategy pattern useful.
    /// </summary>
    public class SalaryStrategyFactory
    {
        public ISalaryCalculationStrategy ForEmploymentType(string? employmentType)
        {
            var type = (employmentType ?? string.Empty).Trim().ToLowerInvariant();
            return type switch
            {
                "part-time" or "contract" or "temporary" or "hourly" => new HourlyRateStrategy(),
                "freelance" => new FreelanceProjectStrategy(),
                "commission" or "sales" => new CommissionBasedStrategy(),
                _ => new AnnualSalaryStrategy(),
            };
        }
    }
}
