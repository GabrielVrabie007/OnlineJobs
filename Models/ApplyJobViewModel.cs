using System.ComponentModel.DataAnnotations;

namespace OnlineJobs.Web.Models
{

    public class ApplyJobViewModel
    {
        public Guid JobPostingId { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Cover letter is required")]
        [StringLength(2000, MinimumLength = 50, ErrorMessage = "Cover letter must be between 50 and 2000 characters")]
        public string CoverLetter { get; set; }

        public string ResumeUrl { get; set; }
    }
}