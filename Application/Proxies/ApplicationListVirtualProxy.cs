using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Proxies
{

    public class ApplicationListVirtualProxy : IApplicationListAccess
    {
        private readonly RealApplicationListAccess _realAccess;
        private IEnumerable<JobApplication>? _cachedApplications;
        private bool _isLoaded = false;
        private readonly SemaphoreSlim _gate = new(1, 1);

        public ApplicationListVirtualProxy(RealApplicationListAccess realAccess)
        {
            _realAccess = realAccess ?? throw new ArgumentNullException(nameof(realAccess));
        }

        public async Task<IEnumerable<JobApplication>> GetApplicationsAsync()
        {
            if (_isLoaded)
                return _cachedApplications ?? Enumerable.Empty<JobApplication>();

            // Async-safe lazy load: first caller fetches, others await the same result,
            // then every later access is served from cache (no .Result-under-lock deadlock).
            await _gate.WaitAsync();
            try
            {
                if (!_isLoaded)
                {
                    _cachedApplications = await _realAccess.GetApplicationsAsync();
                    _isLoaded = true;
                }
            }
            finally
            {
                _gate.Release();
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
            _gate.Wait();
            try
            {
                _cachedApplications = null;
                _isLoaded = false;
            }
            finally
            {
                _gate.Release();
            }
        }
    }
}
