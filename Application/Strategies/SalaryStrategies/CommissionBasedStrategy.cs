namespace OnlineJobs.Application.Strategies.SalaryStrategies
{
    public class CommissionBasedStrategy : ISalaryCalculationStrategy
    {
        private readonly decimal _baseSalary;
        private readonly decimal _commissionPercentage;

        public CommissionBasedStrategy(decimal baseSalary = 50000, decimal commissionPercentage = 0.10m)
        {
            _baseSalary = baseSalary;
            _commissionPercentage = commissionPercentage;
        }

        public decimal CalculateAnnualSalary(decimal estimatedSales, int unused = 0)
        {
            var commissionAmount = estimatedSales * _commissionPercentage;
            return _baseSalary + commissionAmount;
        }

        public string GetSalaryDescription(decimal estimatedSales)
        {
            return $"${_baseSalary:N0} base + {_commissionPercentage:P0} commission";
        }
    }
}
