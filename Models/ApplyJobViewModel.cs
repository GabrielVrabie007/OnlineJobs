using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OnlineJobs.Models
{
    public class ApplyJobViewModel
    {
        public Guid JobId { get; set; }

        [Required(ErrorMessage = "Cover letter is required")]
        [StringLength(2000, MinimumLength = 100, ErrorMessage = "Cover letter must be between 100 and 2000 characters")]
        public string CoverLetter { get; set; } = string.Empty;

        [Required(ErrorMessage = "Resume is required")]
        public IFormFile? Resume { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? PortfolioLink { get; set; }

        [Range(0, 1000000, ErrorMessage = "Please enter a valid salary")]
        public decimal? ExpectedSalary { get; set; }

        [DataType(DataType.Date)]
        public DateTime? AvailableStartDate { get; set; }

        [StringLength(1000)]
        public string? AdditionalInfo { get; set; }
    }
}