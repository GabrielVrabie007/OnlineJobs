namespace OnlineJobs.Application.Strategies.SalaryStrategies
{
    public class HourlyRateStrategy : ISalaryCalculationStrategy
    {
        private const int StandardWorkHoursPerWeek = 40;
        private const int WeeksPerYear = 52;

        public decimal CalculateAnnualSalary(decimal hourlyRate, int hoursPerWeek = StandardWorkHoursPerWeek)
        {
            if (hoursPerWeek <= 0) hoursPerWeek = StandardWorkHoursPerWeek;
            return hourlyRate * hoursPerWeek * WeeksPerYear;
        }

        public string GetSalaryDescription(decimal hourlyRate)
        {
            return $"${hourlyRate:N2}/hour";
        }
    }
}
