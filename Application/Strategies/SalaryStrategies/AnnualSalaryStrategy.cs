namespace OnlineJobs.Application.Strategies.SalaryStrategies
{
    public class AnnualSalaryStrategy : ISalaryCalculationStrategy
    {
        public decimal CalculateAnnualSalary(decimal annualSalary, int unused = 0)
        {
            return annualSalary;
        }

        public string GetSalaryDescription(decimal annualSalary)
        {
            return $"${annualSalary:N0}/year";
        }
    }
}
