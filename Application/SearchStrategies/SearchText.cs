using System.Globalization;
using System.Text;

namespace OnlineJobs.Application.SearchStrategies
{
    /// <summary>
    /// Text normalization for search: trims, lowercases and strips diacritics so that
    /// matching is partial and accent/case-insensitive (e.g. "chisinau" matches
    /// "Chișinău", "DEVOPS" matches "DevOps").
    /// </summary>
    public static class SearchText
    {
        public static string Normalize(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            var lowered = value.Trim().ToLowerInvariant();
            var decomposed = lowered.Normalize(NormalizationForm.FormD);

            var sb = new StringBuilder(decomposed.Length);
            foreach (var ch in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>True if <paramref name="field"/> contains <paramref name="normalizedTerm"/> (already normalized).</summary>
        public static bool Contains(string? field, string normalizedTerm)
            => Normalize(field).Contains(normalizedTerm);
    }
}
