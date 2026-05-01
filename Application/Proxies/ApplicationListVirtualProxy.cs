using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Proxies
{

    public class ApplicationListVirtualProxy : IApplicationListAccess
    {
        private readonly RealApplicationListAccess _realAccess;
        private IEnumerable<JobApplication>? _cachedApplications;
        private bool _isLoaded = false;
        private readonly object _lock = new object();

        public ApplicationListVirtualProxy(RealApplicationListAccess realAccess)
        {
            _realAccess = realAccess ?? throw new ArgumentNullException(nameof(realAccess));
        }

        public async Task<IEnumerable<JobApplication>> GetApplicationsAsync()
        {
            if (!_isLoaded)
            {
                lock (_lock)
                {
                    if (!_isLoaded) // Double-check locking
                    {
                        _cachedApplications = _realAccess.GetApplicationsAsync().Result;
                        _isLoaded = true;
                    }
                }
            }

            return _cachedApplications ?? Enumerable.Empty<JobApplication>();
        }

        public async Task<int> GetApplicationCountAsync()
        {
            if (!_isLoaded)
            {
                // In a real implementation, we could query just the count from database
                // For now, we'll load the data
                await GetApplicationsAsync();
            }

            return _cachedApplications?.Count() ?? 0;
        }

        public async Task<JobApplication> GetApplicationByIdAsync(Guid applicationId)
        {
            if (!_isLoaded)
            {
                await GetApplicationsAsync();
            }

            return _cachedApplications?.FirstOrDefault(a => a.Id == applicationId);
        }

        /// <summary>
        /// Checks if data has been loaded (useful for monitoring)
        /// </summary>
        public bool IsLoaded() => _isLoaded;

        /// <summary>
        /// Forces a reload of data (clears cache)
        /// </summary>
        public void Invalidate()
        {
            lock (_lock)
            {
                _cachedApplications = null;
                _isLoaded = false;
            }
        }
    }
}
