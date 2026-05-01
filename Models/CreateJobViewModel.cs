using System.ComponentModel.DataAnnotations;

namespace OnlineJobs.Models
{
    public class CreateJobViewModel
    {
        [Required(ErrorMessage = "Job title is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Job description is required")]
        [StringLength(5000, MinimumLength = 20, ErrorMessage = "Description must be at least 20 characters")]
        public string Description { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Requirements { get; set; }

        [Range(0, 1000000, ErrorMessage = "Please enter a valid salary")]
        public decimal? SalaryMin { get; set; }

        [Range(0, 1000000, ErrorMessage = "Please enter a valid salary")]
        public decimal? SalaryMax { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Employment type is required")]
        [StringLength(100)]
        public string EmploymentType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        public string? ExperienceLevel { get; set; }

        public bool IsCompanyRevealed { get; set; } = true;

        public Guid CompanyId { get; set; }
    }
}