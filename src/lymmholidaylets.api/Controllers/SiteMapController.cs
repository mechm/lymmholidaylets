using Microsoft.AspNetCore.Mvc;

namespace LymmHolidayLets.Api.Controllers
{
    public class SiteMapController : Controller
    {
        /// <summary>
     /// https://daypicker.dev/selections/disabling-dates
     /// </summary>
     /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
