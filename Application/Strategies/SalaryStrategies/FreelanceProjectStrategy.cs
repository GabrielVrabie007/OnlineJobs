namespace OnlineJobs.Application.Strategies.SalaryStrategies
{
    public class FreelanceProjectStrategy : ISalaryCalculationStrategy
    {
        private const int AverageProjectsPerYear = 12;

        public decimal CalculateAnnualSalary(decimal projectRate, int projectsPerYear = AverageProjectsPerYear)
        {
            if (projectsPerYear <= 0) projectsPerYear = AverageProjectsPerYear;
            return projectRate * projectsPerYear;
        }

        public string GetSalaryDescription(decimal projectRate)
        {
            return $"${projectRate:N0}/project";
        }
    }
}
