using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Domain.Entities
{
    public class JobSeeker : User
    {
        public string Resume { get; set; }
        public string Skills { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public List<JobApplication> Applications { get; set; }

        public override UserType UserType => UserType.JobSeeker;

        public JobSeeker(string email, string firstName, string lastName)
            : base(email, firstName, lastName)
        {
            Applications = new List<JobApplication>();
        }

        public JobSeeker() : base()
        {
            Applications = new List<JobApplication>();
        }

        public JobSeeker(Guid id) : base(id)
        {
            Applications = new List<JobApplication>();
        }

        public override bool CanApplyToJobs()
        {
            return IsActive;
        }

        public override string GetDisplayInfo()
        {
            var baseInfo = base.GetDisplayInfo();
            return $"{baseInfo} - Job Seeker";
        }

        public bool HasResume()
        {
            return !string.IsNullOrWhiteSpace(Resume);
        }

        public int GetApplicationCount()
        {
            return Applications?.Count ?? 0;
        }
    }
}