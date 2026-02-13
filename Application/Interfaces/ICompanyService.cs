using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Interfaces
{

    public interface ICompanyService
    {
        Task<Company> GetCompanyByIdAsync(Guid companyId);
        Task<IEnumerable<Company>> GetAllCompaniesAsync();
    }
}