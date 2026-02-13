using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Interfaces
{

    public interface IUserService
    {
        Task<User> RegisterJobSeekerAsync(string email, string password, string firstName, string lastName);
        Task<User> RegisterEmployerAsync(string email, string password, string firstName, string lastName, Guid? companyId = null);
        Task<User> LoginAsync(string email, string password);
    }
}
