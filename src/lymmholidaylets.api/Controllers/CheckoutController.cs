using LymmHolidayLets.Application.Interface.Service;
using Microsoft.AspNetCore.Mvc;
using LymmHolidayLets.Api.Models.Checkout;

namespace LymmHolidayLets.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class CheckoutController(ICheckoutService checkoutService) : ControllerBase
    {
        private readonly ICheckoutService _checkoutService = checkoutService;

        [HttpPost("create-checkout-session")]
        // [ValidateAntiForgeryToken] // Note: Typically requires additional configuration for JSON APIs/Next.js
        public ActionResult Create([FromBody] CheckoutItemForm model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (error, session) = _checkoutService.Checkout(
                host: $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}", 
                model.PropertyId, 
                model.Checkin, 
                model.Checkout, 
                model.NumberOfAdults, 
                model.NumberOfChildren, 
                model.NumberOfInfants);

            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                return BadRequest(ModelState);
            }

            if (session == null)
            {
                ModelState.AddModelError(string.Empty, "Please try again later or contact us for further support");
                return BadRequest(ModelState);
            }

            return Ok(new { url = session.Url });
        }
    }
}
