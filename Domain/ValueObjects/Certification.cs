namespace OnlineJobs.Domain.ValueObjects
{

    public class Certification
    {
        public string Name { get; private set; }
        public string Issuer { get; private set; }
        public DateTime DateObtained { get; private set; }
        public DateTime? ExpiryDate { get; private set; }
        public string? CredentialId { get; private set; }
        public string? CredentialUrl { get; private set; }

        public Certification(
            string name,
            string issuer,
            DateTime dateObtained,
            DateTime? expiryDate = null,
            string? credentialId = null,
            string? credentialUrl = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Certification name cannot be empty", nameof(name));
            if (string.IsNullOrWhiteSpace(issuer))
                throw new ArgumentException("Issuer cannot be empty", nameof(issuer));
            if (dateObtained > DateTime.UtcNow)
                throw new ArgumentException("Date obtained cannot be in the future");
            if (expiryDate.HasValue && expiryDate < dateObtained)
                throw new ArgumentException("Expiry date cannot be before date obtained");

            Name = name;
            Issuer = issuer;
            DateObtained = dateObtained;
            ExpiryDate = expiryDate;
            CredentialId = credentialId;
            CredentialUrl = credentialUrl;
        }

        public bool IsValid()
        {
            return !ExpiryDate.HasValue || ExpiryDate > DateTime.UtcNow;
        }

        public bool IsExpired()
        {
            return ExpiryDate.HasValue && ExpiryDate <= DateTime.UtcNow;
        }

        public override string ToString()
        {
            var expiryStr = ExpiryDate.HasValue
                ? $" (Expires: {ExpiryDate.Value:MMM yyyy})"
                : " (No expiry)";
            var statusStr = IsExpired() ? " [EXPIRED]" : "";
            return $"{Name} - {Issuer} (Obtained: {DateObtained:MMM yyyy}){expiryStr}{statusStr}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Certification other) return false;
            return Name == other.Name &&
                   Issuer == other.Issuer &&
                   DateObtained == other.DateObtained;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Issuer, DateObtained);
        }
    }
}
