using LymmHolidayLets.Application.Interface.Service;
using Microsoft.AspNetCore.Mvc;

namespace LymmHolidayLets.Api.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController(ICheckoutService checkoutService) : ControllerBase
    {
        private readonly ICheckoutService _checkoutService = checkoutService;

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public ActionResult Create([ModelBinder(BinderType = typeof(EditViewModelBinder))] CheckoutItemForm model)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
			     //
        //     var (error, session) = _checkoutService.Checkout(host:$"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}", model.PropertyId, model.Checkin, model.Checkout, model.NumberOfAdults, model.NumberOfChildren, model.NumberOfInfants);
        //     if (error != null)
        //     {
        //         ModelState.AddModelError(string.Empty, error);
        //         return BadRequest(ModelState);
        //     }
        //
        //     if (session != null)
        //     {
        //         return Ok(session.Url);
        //     }
        //     
        //     ModelState.AddModelError(string.Empty, "Please try again later or contact us for further support");
        //     return BadRequest(ModelState);
        //
        // }
    }
}
