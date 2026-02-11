using Microsoft.AspNetCore.Mvc;

namespace LymmHolidayLets.Api.Controllers
{
    public class StripeWebHookController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
