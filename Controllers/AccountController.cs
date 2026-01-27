using Microsoft.AspNetCore.Mvc;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Web.Models;
using System.Text.Json;

namespace OnlineJobs.Web.Controllers
{
    /// <summary>
    /// Account controller
    /// Demonstrates:
    /// - SRP: Single responsibility - user authentication and registration
    /// - DIP: Depends on IUserService and ICompanyService abstractions
    /// - Thin controller pattern - business logic in services
    /// - Proper separation of concerns
    /// </summary>
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICompanyService _companyService;

        // Constructor injection (DIP)
        public AccountController(IUserService userService, ICompanyService companyService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = await _userService.LoginAsync(model.Email, model.Password);

                // Store user info in session (simple authentication for demo)
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserName", user.GetFullName());
                HttpContext.Session.SetString("UserType", user.UserType.ToString());

                TempData["SuccessMessage"] = $"Welcome back, {user.GetFullName()}!";

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            // Get companies for employer registration
            var companies = await _companyService.GetAllCompaniesAsync();
            ViewBag.Companies = companies;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var companies = await _companyService.GetAllCompaniesAsync();
                ViewBag.Companies = companies;
                return View(model);
            }

            try
            {
                if (model.UserType == "JobSeeker")
                {
                    await _userService.RegisterJobSeekerAsync(
                        model.Email,
                        model.FirstName,
                        model.LastName,
                        model.Password
                    );
                }
                else if (model.UserType == "Employer")
                {
                    await _userService.RegisterEmployerAsync(
                        model.Email,
                        model.FirstName,
                        model.LastName,
                        model.Password,
                        model.CompanyId
                    );
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid user type");
                    return View(model);
                }

                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var companies = await _companyService.GetAllCompaniesAsync();
                ViewBag.Companies = companies;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        // Helper method to check if user is logged in
        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
        }

        // Helper method to get current user ID
        private Guid? GetCurrentUserId()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (Guid.TryParse(userIdString, out var userId))
                return userId;
            return null;
        }
    }
}