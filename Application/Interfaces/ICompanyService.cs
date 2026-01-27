using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Interfaces
{
    /// <summary>
    /// Company service interface
    /// Demonstrates:
    /// - ISP: Only company operations, segregated interface
    /// - SRP: Single responsibility - company management
    /// - DIP: Controllers depend on this abstraction
    /// </summary>
    public interface ICompanyService
    {
        // Company CRUD operations
        Task<Company> CreateCompanyAsync(string name, string location);
        Task<Company> GetCompanyByIdAsync(Guid companyId);
        Task<IEnumerable<Company>> GetAllCompaniesAsync();
        Task UpdateCompanyAsync(Company company);
        Task DeleteCompanyAsync(Guid companyId);

        // Company-specific queries
        Task<IEnumerable<Company>> SearchCompaniesByNameAsync(string name);
        Task<Company> GetCompanyWithJobsAsync(Guid companyId);
        Task<int> GetActiveJobCountAsync(Guid companyId);
    }
}