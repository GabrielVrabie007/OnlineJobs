namespace OnlineJobs.Domain.ValueObjects
{

    public class WorkExperience
    {
        public string Company { get; private set; }
        public string Position { get; private set; }
        public string Location { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public string? Description { get; private set; }
        public List<string> Responsibilities { get; private set; }
        public List<string> Achievements { get; private set; }

        public WorkExperience(
            string company,
            string position,
            string location,
            DateTime startDate,
            DateTime? endDate = null,
            string? description = null,
            List<string>? responsibilities = null,
            List<string>? achievements = null)
        {
            if (string.IsNullOrWhiteSpace(company))
                throw new ArgumentException("Company cannot be empty", nameof(company));
            if (string.IsNullOrWhiteSpace(position))
                throw new ArgumentException("Position cannot be empty", nameof(position));
            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Location cannot be empty", nameof(location));
            if (endDate.HasValue && endDate < startDate)
                throw new ArgumentException("End date cannot be before start date");

            Company = company;
            Position = position;
            Location = location;
            StartDate = startDate;
            EndDate = endDate;
            Description = description;
            Responsibilities = responsibilities ?? new List<string>();
            Achievements = achievements ?? new List<string>();
        }

        public bool IsCurrentPosition()
        {
            return !EndDate.HasValue;
        }

        public TimeSpan GetDuration()
        {
            var end = EndDate ?? DateTime.UtcNow;
            return end - StartDate;
        }

        public string GetDurationText()
        {
            var duration = GetDuration();
            var years = duration.Days / 365;
            var months = (duration.Days % 365) / 30;

            if (years > 0 && months > 0)
                return $"{years} year{(years > 1 ? "s" : "")} {months} month{(months > 1 ? "s" : "")}";
            if (years > 0)
                return $"{years} year{(years > 1 ? "s" : "")}";
            if (months > 0)
                return $"{months} month{(months > 1 ? "s" : "")}";
            return "Less than a month";
        }

        public override string ToString()
        {
            var endDateStr = EndDate.HasValue ? EndDate.Value.ToString("MMM yyyy") : "Present";
            return $"{Position} at {Company} ({StartDate:MMM yyyy} - {endDateStr}) - {Location}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not WorkExperience other) return false;
            return Company == other.Company &&
                   Position == other.Position &&
                   StartDate == other.StartDate;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Company, Position, StartDate);
        }
    }
}
