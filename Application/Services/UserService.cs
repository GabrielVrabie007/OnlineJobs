using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Services
{
    /// <summary>
    /// User service implementation
    /// Demonstrates:
    /// - SRP: Single responsibility - user authentication and management
    /// - DIP: Depends on IRepository abstraction, not concrete implementation
    /// - Constructor injection for dependencies
    /// - Business logic separation from data access
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<JobSeeker> _jobSeekerRepository;
        private readonly IRepository<Employer> _employerRepository;

        // Constructor injection (DIP)
        public UserService(
            IRepository<User> userRepository,
            IRepository<JobSeeker> jobSeekerRepository,
            IRepository<Employer> employerRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _jobSeekerRepository = jobSeekerRepository ?? throw new ArgumentNullException(nameof(jobSeekerRepository));
            _employerRepository = employerRepository ?? throw new ArgumentNullException(nameof(employerRepository));
        }

        public async Task<User> RegisterJobSeekerAsync(string email, string firstName, string lastName, string password)
        {
            // Validation
            if (await GetUserByEmailAsync(email) != null)
                throw new InvalidOperationException("User with this email already exists");

            // Create job seeker
            var jobSeeker = new JobSeeker(email, firstName, lastName)
            {
                PasswordHash = HashPassword(password) // Simple hash for demo
            };

            await _jobSeekerRepository.AddAsync(jobSeeker);
            await _userRepository.AddAsync(jobSeeker);

            return jobSeeker;
        }

        public async Task<User> RegisterEmployerAsync(string email, string firstName, string lastName, string password, Guid? companyId = null)
        {
            // Validation
            if (await GetUserByEmailAsync(email) != null)
                throw new InvalidOperationException("User with this email already exists");

            // Create employer
            var employer = new Employer(email, firstName, lastName)
            {
                PasswordHash = HashPassword(password),
                CompanyId = companyId
            };

            await _employerRepository.AddAsync(employer);
            await _userRepository.AddAsync(employer);

            return employer;
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);

            if (user == null)
                throw new InvalidOperationException("Invalid email or password");

            if (!ValidatePassword(password, user.PasswordHash))
                throw new InvalidOperationException("Invalid email or password");

            user.UpdateLastLogin();
            await _userRepository.UpdateAsync(user);

            return user;
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);
            return user != null && ValidatePassword(password, user.PasswordHash);
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var users = await _userRepository.FindAsync(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return users.FirstOrDefault();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await _userRepository.UpdateAsync(user);

            // Update in specific repository based on type
            if (user is JobSeeker jobSeeker)
                await _jobSeekerRepository.UpdateAsync(jobSeeker);
            else if (user is Employer employer)
                await _employerRepository.UpdateAsync(employer);
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            await _userRepository.DeleteAsync(userId);
        }

        public async Task<JobSeeker> GetJobSeekerByIdAsync(Guid userId)
        {
            return await _jobSeekerRepository.GetByIdAsync(userId);
        }

        public async Task<Employer> GetEmployerByIdAsync(Guid userId)
        {
            return await _employerRepository.GetByIdAsync(userId);
        }

        public async Task<IEnumerable<JobSeeker>> GetAllJobSeekersAsync()
        {
            return await _jobSeekerRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Employer>> GetAllEmployersAsync()
        {
            return await _employerRepository.GetAllAsync();
        }

        // Private helper methods for password hashing (simplified for demo)
        private string HashPassword(string password)
        {
            // In production, use BCrypt, Argon2, or ASP.NET Core Identity
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private bool ValidatePassword(string password, string hash)
        {
            var passwordHash = HashPassword(password);
            return passwordHash == hash;
        }
    }
}