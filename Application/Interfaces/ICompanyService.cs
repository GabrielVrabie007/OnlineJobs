using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<Company> CreateCompanyAsync(string name, string location);
        Task<Company> GetCompanyByIdAsync(Guid companyId);
        Task<IEnumerable<Company>> GetAllCompaniesAsync();
        Task UpdateCompanyAsync(Company company);
        Task DeleteCompanyAsync(Guid companyId);

        Task<IEnumerable<Company>> SearchCompaniesByNameAsync(string name);
        Task<Company> GetCompanyWithJobsAsync(Guid companyId);
        Task<int> GetActiveJobCountAsync(Guid companyId);
    }
}