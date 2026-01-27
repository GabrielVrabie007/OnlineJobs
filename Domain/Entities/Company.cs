namespace OnlineJobs.Domain.Entities
{
    /// <summary>
    /// Company entity
    /// Demonstrates:
    /// - ENCAPSULATION: Private fields with validated properties
    /// - SRP: Single responsibility - managing company information
    /// - COMPOSITION: Company has JobPostings (has-a relationship)
    /// </summary>
    public class Company
    {
        private string? _name;

        public Guid Id { get; private set; }

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
        public DateTime CreatedAt { get; private set; }

        // Navigation properties
        public List<JobPosting> JobPostings { get; set; }
        public List<Employer> Employers { get; set; }

        // Constructor
        public Company(string name, string location)
        {
            Id = Guid.NewGuid();
            Name = name; // Uses property setter with validation
            Location = location;
            CreatedAt = DateTime.UtcNow;
            JobPostings = new List<JobPosting>();
            Employers = new List<Employer>();
        }

        // Parameterless constructor
        public Company()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            JobPostings = new List<JobPosting>();
            Employers = new List<Employer>();
        }

        // Business methods
        public int GetActiveJobCount()
        {
            return JobPostings?.Count(j => j.Status == Enums.JobStatus.Active) ?? 0;
        }

        public bool HasActiveJobs()
        {
            return GetActiveJobCount() > 0;
        }
    }
}