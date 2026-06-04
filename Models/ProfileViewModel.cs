using System.ComponentModel.DataAnnotations;

namespace OnlineJobs.Models
{
    public class ProfileViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Professional summary")]
        [MinLength(50, ErrorMessage = "Make it at least 50 characters so it reads well.")]
        public string? ProfessionalSummary { get; set; }

        [Display(Name = "Skills (comma separated)")]
        public string? Skills { get; set; }

        [Url]
        [Display(Name = "LinkedIn URL")]
        public string? LinkedInUrl { get; set; }

        [Url]
        [Display(Name = "GitHub URL")]
        public string? GitHubUrl { get; set; }

        [Url]
        [Display(Name = "Portfolio URL")]
        public string? PortfolioUrl { get; set; }
    }
}
