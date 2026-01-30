using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterJobSeekerAsync(string email, string password, string firstName, string lastName);
        Task<User> RegisterEmployerAsync(string email, string password, string firstName, string lastName, Guid? companyId = null);
        Task<User> LoginAsync(string email, string password);
        Task<bool> ValidateCredentialsAsync(string email, string password);

        Task<User> GetUserByIdAsync(Guid userId);
        Task<User> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(Guid userId);

        Task<JobSeeker> GetJobSeekerByIdAsync(Guid userId);
        Task<Employer> GetEmployerByIdAsync(Guid userId);
        Task<IEnumerable<JobSeeker>> GetAllJobSeekersAsync();
        Task<IEnumerable<Employer>> GetAllEmployersAsync();
    }
}
