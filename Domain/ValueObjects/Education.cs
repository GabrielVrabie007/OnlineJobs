namespace OnlineJobs.Domain.ValueObjects
{

    public class Education
    {
        public string Degree { get; private set; }
        public string Institution { get; private set; }
        public string FieldOfStudy { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public double? GPA { get; private set; }
        public string? Description { get; private set; }

        public Education(
            string degree,
            string institution,
            string fieldOfStudy,
            DateTime startDate,
            DateTime? endDate = null,
            double? gpa = null,
            string? description = null)
        {
            if (string.IsNullOrWhiteSpace(degree))
                throw new ArgumentException("Degree cannot be empty", nameof(degree));
            if (string.IsNullOrWhiteSpace(institution))
                throw new ArgumentException("Institution cannot be empty", nameof(institution));
            if (string.IsNullOrWhiteSpace(fieldOfStudy))
                throw new ArgumentException("Field of study cannot be empty", nameof(fieldOfStudy));
            if (endDate.HasValue && endDate < startDate)
                throw new ArgumentException("End date cannot be before start date");
            if (gpa.HasValue && (gpa < 0 || gpa > 4.0))
                throw new ArgumentException("GPA must be between 0 and 4.0");

            Degree = degree;
            Institution = institution;
            FieldOfStudy = fieldOfStudy;
            StartDate = startDate;
            EndDate = endDate;
            GPA = gpa;
            Description = description;
        }

        public bool IsCurrentlyEnrolled()
        {
            return !EndDate.HasValue || EndDate > DateTime.UtcNow;
        }

        public override string ToString()
        {
            var endDateStr = EndDate.HasValue ? EndDate.Value.ToString("MMM yyyy") : "Present";
            var gpaStr = GPA.HasValue ? $" (GPA: {GPA:F2})" : "";
            return $"{Degree} in {FieldOfStudy}, {Institution} ({StartDate:MMM yyyy} - {endDateStr}){gpaStr}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Education other) return false;
            return Degree == other.Degree &&
                   Institution == other.Institution &&
                   FieldOfStudy == other.FieldOfStudy &&
                   StartDate == other.StartDate;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Degree, Institution, FieldOfStudy, StartDate);
        }
    }
}
