using LymmHolidayLets.Application.Interface.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LymmHolidayLets.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class PropertyController : ControllerBase
    {
        private static IHttpContextAccessor? _httpContextAccessor;

        private readonly Domain.Interface.ILogger _logger;
        private readonly IMemoryCache _cache;
       
        private readonly IPropertyQuery _propertyQuery;
        private readonly IPriceQuery _priceQuery;

        public PropertyController(Domain.Interface.ILogger logger, IMemoryCache cache,
            IHttpContextAccessor httpContextAccessor,
            IPropertyQuery propertyQuery, IPriceQuery priceQuery)
        {
            _logger = logger;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            _propertyQuery = propertyQuery;
            _priceQuery = priceQuery;
        }

        //[HttpGet("detail/{id}")]
        //public IActionResult Detail(byte id)
        //{
        //    var propertyKey = $"property-detail-{id}";

        //    if (_cache.TryGetValue(propertyKey, out PropertyDetailViewModel? property) && property != null)
        //    {
        //        return Ok(property);
        //    }

        //    PropertyDetailAggregate? propertyResult = _propertyQuery.GetPropertyDetailById(id);

        //    if (propertyResult == null)
        //    {
        //        _logger.LogWarning($"Unable to retrieve property detail for property id - {id}.", HttpContext);
        //        return NotFound();
        //    }

        //    PropertyHost propertyHost = id switch
        //    {
        //        1 => new()
        //        {
        //            Name = "",
        //            NumberOfProperties = 1,
        //            YearsExperience = 30,
        //            JobTitle = "",
        //        },
        //        _ => new()
        //        {
        //            Name = "",
        //            NumberOfProperties = 1,
        //            YearsExperience = 30,
        //            JobTitle = "",
        //        },
        //    };
        //    property = new(_httpContextAccessor)
        //    {
        //        PropertyId = id,
        //        CheckoutItemForm = new CheckoutItemForm(propertyResult.PropertyBooking.MaximumNumberOfAdult)
        //        {
        //            PropertyId = id,
        //            NumberOfAdults = propertyResult.PropertyBooking.MinimumNumberOfAdult,
        //            MaxNumberOfGuests = propertyResult.PropertyBooking.MaximumNumberOfGuests,
        //            MaxNumberOfAdult = propertyResult.PropertyBooking.MaximumNumberOfAdult,
        //            MaxNumberOfChildren = propertyResult.PropertyBooking.MaximumNumberOfChildren,
        //            MaxNumberOfInfants = propertyResult.PropertyBooking.MaximumNumberOfInfants,
        //            DatesBooked = propertyResult.DatesBooked,
        //        },
        //        FAQ = propertyResult.FAQs,
        //        Host = propertyHost,
        //    };

        //    if (propertyResult.Review.Any())
        //    {
        //        property.ReviewAggregate = new ReviewAggregate()
        //        {
        //            OverallRating = propertyResult.Review.Average(i => i.Rating),
        //            OverallAccuracy = propertyResult.Review.Average(i => i.Accuracy),
        //            OverallCleanliness = propertyResult.Review.Average(i => i.Cleanliness),
        //            OverallCommunication = propertyResult.Review.Average(i => i.Communication),
        //            OverallCheckIn = propertyResult.Review.Average(i => i.Checkin),
        //            OverallValue = propertyResult.Review.Average(i => i.Value),
        //            OverallLocation = propertyResult.Review.Average(i => i.Location),
        //            OverallFacilities = propertyResult.Review.Average(i => i.Facilities),
        //            OverallComfort = propertyResult.Review.Average(i => i.Comfort),
        //            Reviews = propertyResult.Review.Select(
        //                i => new Models.ViewModels.Property.Review()
        //                {
        //                    Name = i.Name,
        //                    Company = i.Company,
        //                    Position = i.Position,
        //                    Description = i.Description,
        //                    Rating = i.Rating,
        //                    DateTimeAdded = i.DateTimeAdded,
        //                    ReviewType = i.ReviewType,
        //                    LinkToView = i.LinkToView
        //                })
        //        };
        //    }

        //    property.Map = id switch
        //    {
        //        1 => new MapViewModel()
        //        {
        //            ShowMap = true,
        //            ShowStreetView = true,
        //            Latitude = 53.380915699999996,
        //            Longitude = -2.4791106999999997,
        //            MapZoom = 19,
        //            StreetViewLatitude = 53.38090722730504,
        //            StreetViewLongitude = -2.4794449534595806,
        //            Pitch = -0.19483784909857604,
        //            Yaw = 93.16289611265945,
        //            Zoom = 3.2246421403255385
        //        },
        //        2 => new MapViewModel() { ShowMap = false, ShowStreetView = false, },
        //        3 => new MapViewModel()
        //        {
        //            ShowMap = true,
        //            ShowStreetView = true,
        //            Latitude = 53.379527854036276,
        //            Longitude = -2.4858714660912127,
        //            MapZoom = 19,
        //            StreetViewLatitude = 53.379527854036276,
        //            StreetViewLongitude = -2.4858714660912127,
        //            Pitch = -1.2166272444616482,
        //            Yaw = 343.3876913518914,
        //            Zoom = 1.224642140325539
        //        },
        //        _ => property.Map
        //    };

        //    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
        //        .SetPriority(CacheItemPriority.NeverRemove)
        //        .SetAbsoluteExpiration(DateTimeOffset.Now.AddMinutes(10));

        //    _cache.Set(propertyKey, property, cacheEntryOptions);

        //    return Ok(property);
        //}

        //[HttpPost("price")]
        //public IActionResult Price([FromBody] PriceRequest priceRequest)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    PriceAggregate priceData = _priceQuery.GetByPropertyIdAndDate(priceRequest.PropertyId, priceRequest.CheckIn, priceRequest.Checkout);

        //    if (!priceData.TotalNightlyPrice.HasValue)
        //    {
        //        return NoContent();
        //    }

        //    (decimal? percentage, int noOfNights) = CalculateService.CalculateApplicableDiscountPercentage(priceData.NightCoupon, priceRequest.CheckIn, priceRequest.Checkout);

        //    List<AdditionalCharges> additionalCharges = priceData.AdditionalProduct.Select(x => new AdditionalCharges(x.StripeName, x.StripeDefaultUnitPrice)).ToList();

        //    PriceResponse priceResponse = new(noOfNights, priceData.TotalNightlyPrice.Value, percentage, additionalCharges);

        //    return Ok(priceResponse);
        //}
    }
}

