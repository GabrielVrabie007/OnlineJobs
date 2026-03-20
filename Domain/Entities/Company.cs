using OnlineJobs.Domain.Interfaces;

namespace OnlineJobs.Domain.Entities
{
    public class Company : IPrototype<Company>
    {
        private string? _name;

        public Guid Id { get; set; }

        public string Name
        {
            get => _name ?? string.Empty;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Company name cannot be empty");
                _name = value;
            }
        }

        public string? Description { get; set; }
        public string? Website { get; set; }
        public string? Location { get; set; }
        public string? Industry { get; set; }
        public int? EmployeeCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<JobPosting> JobPostings { get; set; }
        public List<Employer> Employers { get; set; }

        public Company(string name, string location)
        /// Overloading
        {
            Id = Guid.NewGuid();
            Name = name;
            Location = location;
            CreatedAt = DateTime.UtcNow;
            JobPostings = new List<JobPosting>();
            Employers = new List<Employer>();
        }

        public Company()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            JobPostings = new List<JobPosting>();
            Employers = new List<Employer>();
        }

        public int GetActiveJobCount()
        {
            return JobPostings?.Count(j => j.Status == Enums.JobStatus.Active) ?? 0;
        }

        public bool HasActiveJobs()
        {
            return GetActiveJobCount() > 0;
        }

        /// <summary>
        /// Creates a deep copy of the Company.
        /// Useful when creating similar companies (e.g., franchises, subsidiaries, templates).
        ///
        /// Deep Copy Behavior:
        /// - Creates new Company instance with new ID
        /// - Copies all property values
        /// - Creates new empty collections (JobPostings and Employers are not cloned)
        /// - Sets new CreatedAt timestamp
        /// - All primitive and string properties are copied
        /// </summary>
        /// <returns>A new Company instance with copied values</returns>
        public Company Clone()
        {
            var clonedCompany = new Company
            {
                // New identity
                Id = Guid.NewGuid(),

                // Copy company details
                Name = this.Name,
                Description = this.Description,
                Website = this.Website,
                Location = this.Location,
                Industry = this.Industry,
                EmployeeCount = this.EmployeeCount,

                // New timestamp
                CreatedAt = DateTime.UtcNow,

                // New empty collections (deep copy - don't clone job postings or employers)
                JobPostings = new List<JobPosting>(),
                Employers = new List<Employer>()
            };

            return clonedCompany;
        }

        /// <summary>
        /// Creates a shallow copy of the Company.
        /// Shares the same collections (JobPostings and Employers lists).
        /// Generally not recommended - use Clone() instead for safety.
        /// </summary>
        /// <returns>A shallow copy of the Company</returns>
        public Company ShallowCopy()
        {
            return (Company)this.MemberwiseClone();
        }

        /// <summary>
        /// Creates a clone with a new name.
        /// Useful for creating franchises or subsidiaries.
        /// </summary>
        public Company CloneWithNewName(string newName)
        {
            var clone = this.Clone();
            clone.Name = newName;
            return clone;
        }

        /// <summary>
        /// Creates a clone as a subsidiary in a different location.
        /// </summary>
        public Company CloneAsSubsidiary(string subsidiaryName, string newLocation)
        {
            var clone = this.Clone();
            clone.Name = subsidiaryName;
            clone.Location = newLocation;
            return clone;
        }

        /// <summary>
        /// Creates a clone with modifications.
        /// Allows customization via action delegate.
        /// </summary>
        public Company CloneWith(Action<Company> modifications)
        {
            var clone = this.Clone();
            modifications?.Invoke(clone);
            return clone;
        }
    }
}