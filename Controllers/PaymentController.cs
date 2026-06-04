using Microsoft.AspNetCore.Mvc;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Notifications;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Controllers
{
    /// <summary>
    /// Endpoint behind the "Reveal Company" purchase. The job seeker picks a gateway
    /// (PayPal / Stripe / Google Pay) and CompanyRevealService processes it through the
    /// matching <c>IPaymentProcessor</c> adapter — the Adapter pattern in real use.
    /// </summary>
    public class PaymentController : Controller
    {
        private readonly ICompanyRevealService _companyRevealService;
        private readonly NotificationStore _notificationStore;

        public PaymentController(ICompanyRevealService companyRevealService, NotificationStore notificationStore)
        {
            _companyRevealService = companyRevealService ?? throw new ArgumentNullException(nameof(companyRevealService));
            _notificationStore = notificationStore ?? throw new ArgumentNullException(nameof(notificationStore));
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCompanyReveal(Guid jobId, PaymentGateway paymentGateway)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!IsJobSeeker())
            {
                TempData["ErrorMessage"] = "Only job seekers can reveal companies.";
                return RedirectToAction("Details", "Job", new { id = jobId });
            }

            try
            {
                var reveal = await _companyRevealService.PurchaseCompanyRevealAsync(userId.Value, jobId, paymentGateway);
                if (reveal != null)
                {
                    var price = _companyRevealService.GetRevealPrice();
                    TempData["SuccessMessage"] = $"Payment of ${price:0.00} via {paymentGateway} succeeded — company revealed!";
                    _notificationStore.Add(userId.Value, "Company revealed",
                        $"You unlocked the company for this job via {paymentGateway}.", "bi-unlock");
                }
                else
                {
                    TempData["ErrorMessage"] = "Payment could not be completed, or you already have access.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Payment failed: {ex.Message}";
            }

            return RedirectToAction("Details", "Job", new { id = jobId });
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
