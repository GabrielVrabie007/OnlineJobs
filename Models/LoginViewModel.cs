using System.ComponentModel.DataAnnotations;

namespace OnlineJobs.Web.Models
{
    /// <summary>
    /// Login view model
    /// Demonstrates:
    /// - SRP: Single responsibility - capturing login data
    /// - Data validation attributes
    /// - ViewModel pattern (separation from domain entities)
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}