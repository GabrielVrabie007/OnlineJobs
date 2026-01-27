namespace OnlineJobs.Application.Interfaces
{
    /// <summary>
    /// Generic repository interface for data access operations
    /// Demonstrates:
    /// - ISP: Small, focused interface with only essential CRUD operations
    /// - DIP: Abstracts data access, allowing different implementations
    /// - Generic type parameter for reusability (DRY principle)
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        // Query operations
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate);

        // Command operations
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);

        // Utility
        Task<bool> ExistsAsync(Guid id);
        Task<int> CountAsync();
    }
}