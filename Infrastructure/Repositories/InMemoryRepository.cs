using OnlineJobs.Application.Interfaces;
using System.Reflection;

namespace OnlineJobs.Infrastructure.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : class
    {
        protected readonly List<T> _dataStore;
        private readonly object _lock = new object();

        public InMemoryRepository()
        {
            _dataStore = new List<T>();
        }

        public virtual Task<IEnumerable<T>> GetAllAsync()
        {
            lock (_lock)
            {
                return Task.FromResult<IEnumerable<T>>(_dataStore.ToList());
            }
        }

        public virtual Task<T> GetByIdAsync(Guid id)
        {
            lock (_lock)
            {
                var entity = _dataStore.FirstOrDefault(e => GetId(e) == id);
                return Task.FromResult(entity);
            }
        }

        public virtual Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate)
        {
            lock (_lock)
            {
                var results = _dataStore.Where(predicate).ToList();
                return Task.FromResult<IEnumerable<T>>(results);
            }
        }

        public virtual Task<T> AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            lock (_lock)
            {
                _dataStore.Add(entity);
                return Task.FromResult(entity);
            }
        }

        public virtual Task UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            lock (_lock)
            {
                var id = GetId(entity);
                var existing = _dataStore.FirstOrDefault(e => GetId(e) == id);

                if (existing == null)
                    throw new InvalidOperationException($"Entity with ID {id} not found");

                var index = _dataStore.IndexOf(existing);
                _dataStore[index] = entity;

                return Task.CompletedTask;
            }
        }

        public virtual Task DeleteAsync(Guid id)
        {
            lock (_lock)
            {
                var entity = _dataStore.FirstOrDefault(e => GetId(e) == id);

                if (entity == null)
                    throw new InvalidOperationException($"Entity with ID {id} not found");

                _dataStore.Remove(entity);
                return Task.CompletedTask;
            }
        }

        public virtual Task<bool> ExistsAsync(Guid id)
        {
            lock (_lock)
            {
                var exists = _dataStore.Any(e => GetId(e) == id);
                return Task.FromResult(exists);
            }
        }

        public virtual Task<int> CountAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_dataStore.Count);
            }
        }

        protected Guid GetId(T entity)
        {
            var property = typeof(T).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
                throw new InvalidOperationException($"Type {typeof(T).Name} does not have an Id property");

            var value = property.GetValue(entity);
            if (value is Guid id)
                return id;

            throw new InvalidOperationException($"Id property of {typeof(T).Name} is not of type Guid");
        }
    }
}