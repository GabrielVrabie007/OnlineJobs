namespace OnlineJobs.Application.Configuration
{

    public sealed class ApplicationConfiguration
    {
  
        private static readonly Lazy<ApplicationConfiguration> _instance =
            new Lazy<ApplicationConfiguration>(
                () => new ApplicationConfiguration(),
                LazyThreadSafetyMode.ExecutionAndPublication);

        private static readonly object _updateLock = new object();

  
        public static ApplicationConfiguration Instance => _instance.Value;


        public int JobExpiryDays { get; private set; }

    
        public int MaxActiveApplicationsPerUser { get; private set; }

   
        public int MaxActiveJobPostingsPerEmployer { get; private set; }

        public int MinPasswordLength { get; private set; }

  
        public int MaxResumeSizeMB { get; private set; }

        public bool EmailNotificationsEnabled { get; private set; }


        public string ApplicationName { get; private set; }

        public string ApplicationVersion { get; private set; }


        public int SessionExpiryDays { get; private set; }


        public bool RegistrationEnabled { get; private set; }

 
        public int MaxSearchResultsPerPage { get; private set; }


        public DateTime LastUpdated { get; private set; }


        private ApplicationConfiguration()
        {
            // Initialize with default values
            JobExpiryDays = 30;
            MaxActiveApplicationsPerUser = 50;
            MaxActiveJobPostingsPerEmployer = 20;
            MinPasswordLength = 8;
            MaxResumeSizeMB = 5;
            EmailNotificationsEnabled = true;
            ApplicationName = "OnlineJobs Platform";
            ApplicationVersion = "1.0.0";
            SessionExpiryDays = 7;
            RegistrationEnabled = true;
            MaxSearchResultsPerPage = 20;
            LastUpdated = DateTime.UtcNow;
        }


        public void UpdateJobSettings(int expiryDays, int maxPostingsPerEmployer)
        {
            lock (_updateLock)
            {
                if (expiryDays <= 0)
                    throw new ArgumentException("Expiry days must be positive", nameof(expiryDays));
                if (maxPostingsPerEmployer <= 0)
                    throw new ArgumentException("Max postings must be positive", nameof(maxPostingsPerEmployer));

                JobExpiryDays = expiryDays;
                MaxActiveJobPostingsPerEmployer = maxPostingsPerEmployer;
                LastUpdated = DateTime.UtcNow;
            }
        }


        public void UpdateUserSettings(int maxApplications, int minPasswordLength, int sessionExpiryDays)
        {
            lock (_updateLock)
            {
                if (maxApplications <= 0)
                    throw new ArgumentException("Max applications must be positive", nameof(maxApplications));
                if (minPasswordLength < 6)
                    throw new ArgumentException("Min password length must be at least 6", nameof(minPasswordLength));
                if (sessionExpiryDays <= 0)
                    throw new ArgumentException("Session expiry days must be positive", nameof(sessionExpiryDays));

                MaxActiveApplicationsPerUser = maxApplications;
                MinPasswordLength = minPasswordLength;
                SessionExpiryDays = sessionExpiryDays;
                LastUpdated = DateTime.UtcNow;
            }
        }


        public void UpdateFileSettings(int maxResumeSizeMB)
        {
            lock (_updateLock)
            {
                if (maxResumeSizeMB <= 0 || maxResumeSizeMB > 100)
                    throw new ArgumentException("Resume size must be between 1 and 100 MB", nameof(maxResumeSizeMB));

                MaxResumeSizeMB = maxResumeSizeMB;
                LastUpdated = DateTime.UtcNow;
            }
        }


        public void SetEmailNotifications(bool enabled)
        {
            lock (_updateLock)
            {
                EmailNotificationsEnabled = enabled;
                LastUpdated = DateTime.UtcNow;
            }
        }

        public void SetRegistrationEnabled(bool enabled)
        {
            lock (_updateLock)
            {
                RegistrationEnabled = enabled;
                LastUpdated = DateTime.UtcNow;
            }
        }


        public void UpdateSearchSettings(int maxResultsPerPage)
        {
            lock (_updateLock)
            {
                if (maxResultsPerPage <= 0 || maxResultsPerPage > 100)
                    throw new ArgumentException("Results per page must be between 1 and 100", nameof(maxResultsPerPage));

                MaxSearchResultsPerPage = maxResultsPerPage;
                LastUpdated = DateTime.UtcNow;
            }
        }


        public void ResetToDefaults()
        {
            lock (_updateLock)
            {
                JobExpiryDays = 30;
                MaxActiveApplicationsPerUser = 50;
                MaxActiveJobPostingsPerEmployer = 20;
                MinPasswordLength = 8;
                MaxResumeSizeMB = 5;
                EmailNotificationsEnabled = true;
                SessionExpiryDays = 7;
                RegistrationEnabled = true;
                MaxSearchResultsPerPage = 20;
                LastUpdated = DateTime.UtcNow;
            }
        }


        public Dictionary<string, object> GetAllSettings()
        {
            return new Dictionary<string, object>
            {
                { nameof(JobExpiryDays), JobExpiryDays },
                { nameof(MaxActiveApplicationsPerUser), MaxActiveApplicationsPerUser },
                { nameof(MaxActiveJobPostingsPerEmployer), MaxActiveJobPostingsPerEmployer },
                { nameof(MinPasswordLength), MinPasswordLength },
                { nameof(MaxResumeSizeMB), MaxResumeSizeMB },
                { nameof(EmailNotificationsEnabled), EmailNotificationsEnabled },
                { nameof(ApplicationName), ApplicationName },
                { nameof(ApplicationVersion), ApplicationVersion },
                { nameof(SessionExpiryDays), SessionExpiryDays },
                { nameof(RegistrationEnabled), RegistrationEnabled },
                { nameof(MaxSearchResultsPerPage), MaxSearchResultsPerPage },
                { nameof(LastUpdated), LastUpdated }
            };
        }

     
        public override string ToString()
        {
            return $"{ApplicationName} v{ApplicationVersion} - Configuration (Last Updated: {LastUpdated:yyyy-MM-dd HH:mm:ss UTC})";
        }
    }
}
