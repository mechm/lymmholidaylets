using Microsoft.AspNetCore.Mvc;

namespace LymmHolidayLets.Api.Controllers
{
    public class PropertyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
