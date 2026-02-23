using Microsoft.AspNetCore.Mvc;

namespace LymmHolidayLets.Api.Controllers
{
    public class StripeWebHookController : Controller
    {
        // TODO - Implement this to handle Stripe webhooks for payment processing. This will allow us to receive notifications from Stripe when a payment is successful, failed, or refunded, and update our database accordingly.
        public IActionResult Index()
        {
            return View();
        }
    }
}
