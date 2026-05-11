namespace OnlineJobs.Application.Strategies.SalaryStrategies
{
    public interface ISalaryCalculationStrategy
    {
        decimal CalculateAnnualSalary(decimal baseAmount, int hoursOrDays = 0);
        string GetSalaryDescription(decimal baseAmount);
    }
}
