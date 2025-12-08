using Microsoft.AspNetCore.Mvc;

namespace lymmholidaylets.api.Controllers
{
    public sealed class PropertyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
