using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Exception;
using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.ReadModel.Checkout;
using Microsoft.AspNetCore.Http;
using Stripe;
using Stripe.Checkout;

namespace LymmHolidayLets.Application.Service
{
    // db additional products in deploy dacpac
    // discounts in stripe and dacpac - nightly coupon
    // session logic and cache
    // tests around all
    // extraction of domain logic
    // validation updates
    // database overflow - take care not repeat saves
    // honeypot
    // rate limit

    public sealed class CheckoutSession
    {
        public CheckoutSession(string sessionId, DateOnly checkIn, DateOnly checkout)
        {
            SessionId = sessionId;
            CheckIn = checkIn;
            Checkout = checkout;
            Added = DateTime.UtcNow;
        }

        public string SessionId { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly Checkout { get; set; }
        public DateTime Added { get; set; }
    }

    public sealed class CheckoutService : ICheckoutService
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IManageCheckoutSessionService _manageCheckoutSessionService;

        private readonly ICheckoutCommand _checkoutCommand;
        private readonly ICheckoutQuery _checkoutQuery;
        private readonly IStripeService _stripeService;


        // Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
        private static readonly SemaphoreSlim SemaphoreSlim = new(initialCount: 1, maxCount: 1);

        public CheckoutService(ILogger logger, IHttpContextAccessor httpContextAccessor, IManageCheckoutSessionService manageCheckoutSessionService,
            ICheckoutCommand checkoutCommand, ICheckoutQuery checkoutQuery, IStripeService stripeService)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _manageCheckoutSessionService = manageCheckoutSessionService;
            _checkoutCommand = checkoutCommand;
            _checkoutQuery = checkoutQuery;
            _stripeService = stripeService;
        }

        public (string?, Session?) Checkout(string host, byte propertyId, DateOnly checkIn, DateOnly checkout, short? numberOfAdults, short? numberOfChildren, short? numberOfInfants, bool available = true)
        {
            try
            {
                (Product? product, Coupon? coupon, IEnumerable<PropertyAdditionalProduct> additionalProducts, string? propertyName, string? error) 
                    = GetCreateProductAndCouponAdditionalProducts(propertyId, checkIn, checkout, available);

                if (product is null || propertyName is null) 
                {
                    return (error, null);
                }
                
                Session? session = _stripeService.CreateSession(host, propertyName,product, coupon, additionalProducts, 
                                                             propertyId, checkIn, checkout, numberOfAdults, numberOfChildren, numberOfInfants);

                if (session == null) return (null, session);

                _manageCheckoutSessionService.AddUpdateSessionCache(session, checkIn, checkout);

                return (null, session);                
            }
            catch (InvalidCheckoutDataException ex)
            {
                _logger.LogError("Error with Checkout", _httpContextAccessor.HttpContext, null, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error with Checkout", _httpContextAccessor.HttpContext, null, ex);
            }

            return (null, null);
        }

        private (Product?, Coupon?, IEnumerable<PropertyAdditionalProduct>, string?, string?) GetCreateProductAndCouponAdditionalProducts(byte propertyId, DateOnly checkIn, DateOnly checkout, bool available)
        {
            // Asynchronously wait to enter the Semaphore. If no-one has been granted access to the Semaphore, code execution will proceed, otherwise this thread waits here until the semaphore is released 
            SemaphoreSlim.Wait();
            try
            {
                CheckoutAggregate? propertyCheckout = _checkoutQuery.GetByPropertyIdAndDate(propertyId, checkIn, checkout, available);

                if (propertyCheckout is null)
                {
                    return (null, null, new List<PropertyAdditionalProduct>(), null, "No Property Available");
                }

                if (propertyCheckout.TotalNightlyPrice is null)
                {
                    return (null, null, propertyCheckout.PropertyAdditionalProduct, null, "No Price Available for dates selected, please change date selection");
                }

                // for extraction where should this belong though????????
                string productName = GetProductName(propertyCheckout.Property.FriendlyName, checkIn, checkout);
                string productDescription = GetProductDescription(checkIn, checkout);

                (decimal? percentOff, _) = CalculateService.CalculateApplicableDiscountPercentage(propertyCheckout.PropertyNightCoupon, checkIn, checkout);

                decimal additional = propertyCheckout.PropertyAdditionalProduct.Sum(val => val.Quantity * val.StripeDefaultUnitPrice);

                (Product product, Coupon? coupon) = _stripeService.CreateProductAndCoupon(propertyCheckout.PreviousCheckout, 
                                                                                            productName, productDescription,
                                                                                            propertyCheckout.TotalNightlyPrice.Value, percentOff);

                // check for overflow issue - continuously saving checkouts
                UpsertCheckout(previousCheckout: propertyCheckout.PreviousCheckout, propertyId: propertyId, checkIn: checkIn, checkout: checkout, 
                    stripeNightProductId: product.Id, stripeNightDefaultPriceId: product.DefaultPriceId, 
                    stripeNightDefaultUnitPrice: propertyCheckout.TotalNightlyPrice.Value, stripeNightCouponId: coupon?.Id, stripeNightPercentage: percentOff,
                    overallPrice: propertyCheckout.TotalNightlyPrice.Value + additional);

                return (product, coupon, propertyCheckout.PropertyAdditionalProduct, propertyCheckout.Property.FriendlyName, null);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error with Checkout", _httpContextAccessor.HttpContext, null, ex);
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                SemaphoreSlim.Release();
            }

            return (null, null, Enumerable.Empty<PropertyAdditionalProduct>(), null, null);
        }


        private void UpsertCheckout(Checkout? previousCheckout, byte propertyId, DateOnly checkIn, DateOnly checkout, 
            string stripeNightProductId, string stripeNightDefaultPriceId,
            decimal stripeNightDefaultUnitPrice, string? stripeNightCouponId, decimal? stripeNightPercentage, decimal overallPrice)
        {
            if (previousCheckout == null)
            {
                _checkoutCommand.Create(new Model.Command.Checkout(
                    propertyId,
                    checkIn,
                    checkout,
                    stripeNightProductId,
                    stripeNightDefaultPriceId,
                    stripeNightDefaultUnitPrice,
                    stripeNightCouponId,
                    stripeNightPercentage,
                    overallPrice));
            }
            else
            {
                _checkoutCommand.Update(new Model.Command.Checkout(
                    previousCheckout.Id,
                    propertyId,
                    checkIn,
                    checkout,
                    stripeNightProductId,
                    stripeNightDefaultPriceId,
                    stripeNightDefaultUnitPrice,
                    stripeNightCouponId,
                    stripeNightPercentage,
                    overallPrice));
            }
        }

        private static string GetProductName(string friendlyName, DateOnly checkIn, DateOnly checkout)
        {
            return $"{friendlyName} - {checkIn:dd/MM/yyyy} to {checkout:dd/MM/yyyy}";
        }

        private static string GetProductDescription(DateOnly checkIn, DateOnly checkout)
        {
            return $"Price for {checkout.DayNumber - checkIn.DayNumber} Nights";
        }
    }
}
