using Microsoft.AspNetCore.Mvc;

namespace lymmholidaylets.api.Controllers
{
    public class PropertyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
