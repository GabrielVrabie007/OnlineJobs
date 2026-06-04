using Microsoft.AspNetCore.Mvc;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Enums;
using OnlineJobs.Domain.ValueObjects;
using OnlineJobs.Models;

namespace OnlineJobs.Controllers
{
    /// <summary>
    /// Candidate profile editing. The save path uses the Builder pattern
    /// (<see cref="IJobSeekerProfileBuilder"/>) to assemble the profile step by step
    /// with validation, then persists the mappable fields.
    /// </summary>
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;
        private readonly IJobSeekerProfileBuilder _profileBuilder;

        public ProfileController(IUserService userService, IJobSeekerProfileBuilder profileBuilder)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _profileBuilder = profileBuilder ?? throw new ArgumentNullException(nameof(profileBuilder));
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");
            if (!IsJobSeeker())
            {
                TempData["ErrorMessage"] = "Only job seekers have a candidate profile.";
                return RedirectToAction("Index", "Home");
            }

            var seeker = await _userService.GetJobSeekerByIdAsync(userId.Value);
            if (seeker == null)
                return NotFound();

            var model = new ProfileViewModel
            {
                FullName = $"{seeker.FirstName} {seeker.LastName}",
                Email = seeker.Email,
                ProfessionalSummary = seeker.ProfessionalSummary,
                Skills = seeker.Skills,
                LinkedInUrl = seeker.LinkedInUrl,
                GitHubUrl = seeker.GitHubUrl,
                PortfolioUrl = seeker.PortfolioUrl
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");
            if (!IsJobSeeker())
                return Forbid();
            if (!ModelState.IsValid)
                return View(model);

            var seeker = await _userService.GetJobSeekerByIdAsync(userId.Value);
            if (seeker == null)
                return NotFound();

            try
            {
                var skills = (model.Skills ?? string.Empty)
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                // Builder pattern: assemble the profile in clear, validated steps.
                _profileBuilder.WithBasicInfo(seeker.Email, seeker.FirstName, seeker.LastName, seeker.PhoneNumber)
                               .WithOnlinePresence(model.LinkedInUrl, model.GitHubUrl, model.PortfolioUrl);
                if (!string.IsNullOrWhiteSpace(model.ProfessionalSummary))
                    _profileBuilder.WithProfessionalSummary(model.ProfessionalSummary);
                foreach (var skillName in skills)
                    _profileBuilder.AddSkill(new Skill(skillName, SkillProficiency.Intermediate));

                var built = _profileBuilder.Build();

                // Persist the mappable fields onto the existing account.
                seeker.ProfessionalSummary = built.ProfessionalSummary;
                seeker.LinkedInUrl = built.LinkedInUrl;
                seeker.GitHubUrl = built.GitHubUrl;
                seeker.PortfolioUrl = built.PortfolioUrl;
                seeker.Skills = string.Join(", ", skills);
                await _userService.UpdateUserAsync(seeker);

                TempData["SuccessMessage"] = "Your profile has been updated.";
                return RedirectToAction("Edit");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        private Guid? GetCurrentUserId()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            return Guid.TryParse(userIdString, out var userId) ? userId : null;
        }

        private bool IsJobSeeker()
            => HttpContext.Session.GetString("UserType") == UserType.JobSeeker.ToString();
    }
}
