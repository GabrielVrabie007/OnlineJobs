using Microsoft.EntityFrameworkCore;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;
using OnlineJobs.Infrastructure.Data;

namespace OnlineJobs.Infrastructure.Repositories
{
    public class EFRepository<T> : IRepository<T> where T : class
    {
        private readonly OnlineJobsDbContext _context;
        private readonly DbSet<T> _dbSet;

        public EFRepository(OnlineJobsDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        private IQueryable<T> GetQueryableWithIncludes()
        {
            IQueryable<T> query = _dbSet;

   
            if (typeof(T) == typeof(JobPosting))
            {
                query = query
                    .Include("Company")
                    .Include("Employer");
            }
            else if (typeof(T) == typeof(JobApplication))
            {
                query = query
                    .Include("JobPosting")
                    .Include("JobPosting.Company")
                    .Include("JobSeeker");
            }
            else if (typeof(T) == typeof(Employer))
            {
                query = query.Include("Company");
            }

            return query;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            return entity != null;
        }

        public async Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate)
        {
            var allEntities = await GetQueryableWithIncludes().ToListAsync();
            return allEntities.Where(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await GetQueryableWithIncludes().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {

            if (typeof(T) == typeof(JobPosting))
            {
                var result = await _context.Set<JobPosting>()
                    .Include(j => j.Company)
                    .Include(j => j.Employer)
                    .FirstOrDefaultAsync(j => j.Id == id);
                return (T)(object)result;
            }
            else if (typeof(T) == typeof(JobApplication))
            {
                var result = await _context.Set<JobApplication>()
                    .Include(a => a.JobPosting)
                        .ThenInclude(j => j.Company)
                    .Include(a => a.JobSeeker)
                    .FirstOrDefaultAsync(a => a.Id == id);
                return (T)(object)result;
            }
            else if (typeof(T) == typeof(Employer))
            {
                var result = await _context.Set<Employer>()
                    .Include(e => e.Company)
                    .FirstOrDefaultAsync(e => e.Id == id);
                return (T)(object)result;
            }

            return await _dbSet.FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
