using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Domain.Entities
{
    public class Employer : User
    {
        public Guid? CompanyId { get; set; }
        public Company? Company { get; set; }
        public string? Position { get; set; }
        public string? PhoneNumber { get; set; }

        public List<JobPosting> JobPostings { get; set; }

        public override UserType UserType => UserType.Employer;

        public Employer(string email, string firstName, string lastName)
            : base(email, firstName, lastName)
        {
            JobPostings = new List<JobPosting>();
        }

        public Employer() : base()
        {
            JobPostings = new List<JobPosting>();
        }

        public override bool CanPostJobs()
        {
            return IsActive && CompanyId.HasValue;
        }

        public override string GetDisplayInfo()
        {
            var baseInfo = base.GetDisplayInfo();
            var companyInfo = Company != null ? $" at {Company.Name}" : "";
            return $"{baseInfo} - Employer{companyInfo}";
        }

        public bool IsAssociatedWithCompany()
        {
            return CompanyId.HasValue;
        }

        public void AssignToCompany(Company company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            CompanyId = company.Id;
            Company = company;
        }

        public int GetPostedJobsCount()
        {
            return JobPostings?.Count ?? 0;
        }
    }
}