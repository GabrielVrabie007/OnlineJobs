using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<JobPosting> _jobRepository;

        public CompanyService(
            IRepository<Company> companyRepository,
            IRepository<JobPosting> jobRepository)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _jobRepository = jobRepository ?? throw new ArgumentNullException(nameof(jobRepository));
        }

        public async Task<Company> CreateCompanyAsync(string name, string location)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Company name cannot be empty");

            var existing = await _companyRepository.FindAsync(c =>
                c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (existing.Any())
                throw new InvalidOperationException("A company with this name already exists");

            var company = new Company(name, location);
            await _companyRepository.AddAsync(company);

            return company;
        }

        public async Task<Company> GetCompanyByIdAsync(Guid companyId)
        {
            return await _companyRepository.GetByIdAsync(companyId);
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            return await _companyRepository.GetAllAsync();
        }

        public async Task UpdateCompanyAsync(Company company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            await _companyRepository.UpdateAsync(company);
        }

        public async Task DeleteCompanyAsync(Guid companyId)
        {
            var activeJobCount = await GetActiveJobCountAsync(companyId);
            if (activeJobCount > 0)
                throw new InvalidOperationException("Cannot delete company with active job postings");

            await _companyRepository.DeleteAsync(companyId);
        }

        public async Task<IEnumerable<Company>> SearchCompaniesByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return await GetAllCompaniesAsync();

            return await _companyRepository.FindAsync(c =>
                c.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Company> GetCompanyWithJobsAsync(Guid companyId)
        {
            var company = await GetCompanyByIdAsync(companyId);
            if (company == null)
                return null;

            var jobs = await _jobRepository.FindAsync(j => j.CompanyId == companyId);
            company.JobPostings = jobs.ToList();

            return company;
        }

        public async Task<int> GetActiveJobCountAsync(Guid companyId)
        {
            var jobs = await _jobRepository.FindAsync(j =>
                j.CompanyId == companyId && j.Status == Domain.Enums.JobStatus.Active);

            return jobs.Count();
        }
    }
}