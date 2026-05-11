namespace OnlineJobs.Application.Visitors
{
    public class CompensationPackage
    {
        public decimal BaseSalary { get; set; }
        public decimal BenefitsValue { get; set; }
        public decimal EquityValue { get; set; }
        public decimal BonusValue { get; set; }
        public decimal TotalValue { get; set; }
        public string Description { get; set; }

        public CompensationPackage()
        {
            Description = string.Empty;
        }

        public string GetFormattedTotal()
        {
            return $"${TotalValue:N0}";
        }

        public string GetBreakdown()
        {
            return $"Base: ${BaseSalary:N0}, Benefits: ${BenefitsValue:N0}, Equity: ${EquityValue:N0}, Bonus: ${BonusValue:N0}";
        }
    }
}
