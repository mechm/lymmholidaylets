using System.Globalization;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Exception;
using LymmHolidayLets.Application.Model.Service;
using LymmHolidayLets.Domain.ReadModel.Checkout;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;
using StripeProduct = Stripe.Product;

namespace LymmHolidayLets.Infrastructure.Services;

/// <summary>
/// Responsible for all Stripe API interactions: creating and reusing products, coupons,
/// and checkout sessions, as well as expiring active sessions.
/// </summary>
public sealed class StripeService(ILogger<StripeService> logger) : IStripeService
{
    public async Task<StripeProductAndCouponResult> CreateProductAndCouponAsync(
        Checkout? previousCheckout,
        string productName,
        string productDescription,
        decimal unitAmount,
        decimal? percentOff,
        CancellationToken cancellationToken = default)
    {
        StripeProduct nightlyProduct = await GetOrCreateProductAsync(previousCheckout?.StripeNightProductId, productName, productDescription, unitAmount, cancellationToken);
        Coupon? nightlyCoupon = await GetOrCreateCouponAsync(previousCheckout?.StripeNightCouponId, productName, percentOff, nightlyProduct.Id, cancellationToken);

        return new StripeProductAndCouponResult
        {
            Product = new StripeProductResult
            {
                Id = nightlyProduct.Id,
                DefaultPriceId = nightlyProduct.DefaultPriceId
            },
            Coupon = nightlyCoupon is null
                ? null
                : new StripeCouponResult
                {
                    Id = nightlyCoupon.Id,
                    PercentOff = nightlyCoupon.PercentOff
                }
        };
    }

    public async Task<StripeSessionResult?> CreateSessionAsync(
        string host,
        string propertyName,
        string productId,
        string defaultPriceId,
        string? couponId,
        IEnumerable<PropertyAdditionalProduct> additionalProducts,
        short propertyId,
        DateOnly checkIn,
        DateOnly checkout,
        short? numberOfAdults,
        short? numberOfChildren,
        short? numberOfInfants,
        CancellationToken cancellationToken = default)
    {
        SessionCreateOptions options = new()
        {
            PhoneNumberCollection = new SessionPhoneNumberCollectionOptions { Enabled = true },
            Metadata = new Dictionary<string, string>
            {
                { "PropertyID", propertyId.ToString() },
                { "PropertyName", propertyName },
                { "CheckInDate", checkIn.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
                { "CheckoutDate", checkout.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
                { "NoAdult", (numberOfAdults ?? 0).ToString() },
                { "NoChildren", (numberOfChildren ?? 0).ToString() },
                { "NoInfant", (numberOfInfants ?? 0).ToString() }
            },
            Mode = "payment",
            SuccessUrl = host + "/payment/success",
            CancelUrl = host + "/payment/cancel",
            AutomaticTax = new SessionAutomaticTaxOptions { Enabled = true },
            LineItems =
            [
                new SessionLineItemOptions
                {
                    Price = defaultPriceId,
                    Quantity = 1,
                }
            ],
            ExpiresAt = DateTime.UtcNow + new TimeSpan(0, 30, 0)
        };

        foreach (var additional in additionalProducts)
        {
            options.LineItems.Add(new SessionLineItemOptions
            {
                Price = additional.StripeDefaultPriceID,
                Quantity = additional.Quantity,
            });
        }

        if (couponId != null)
        {
            options.Discounts =
            [
                new SessionDiscountOptions
                {
                    Coupon = couponId,
                }
            ];
        }

        SessionService sessionService = new();
        var session = await sessionService.CreateAsync(options, cancellationToken: cancellationToken);

        return new StripeSessionResult
        {
            Id = session.Id,
            Url = session.Url
        };
    }

    public async Task<StripeSessionResult?> ExpireSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        SessionService sessionService = new();

        try
        {
            var session = await sessionService.ExpireAsync(sessionId, cancellationToken: cancellationToken);
            return new StripeSessionResult
            {
                Id = session.Id,
                Url = session.Url
            };
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, "Error expiring Stripe session {SessionId}", sessionId);
        }

        return null;
    }

    private static async Task<StripeProduct> GetOrCreateProductAsync(string? id, string name, string description, decimal unitAmount, CancellationToken cancellationToken)
    {
        if (id == null)
        {
            return await CreateProductAsync(name, description, unitAmount, cancellationToken);
        }

        StripeProduct product = await GetProductAsync(id, cancellationToken);
        Price price = await GetPriceAsync(product.DefaultPriceId, cancellationToken);

        return !price.UnitAmount.HasValue || price.UnitAmount.Value / 100m != unitAmount
            ? await CreateProductAsync(name, description, unitAmount, cancellationToken)
            : product;
    }

    private static async Task<StripeProduct> GetProductAsync(string id, CancellationToken cancellationToken)
    {
        var productService = new ProductService();

        try
        {
            return await productService.GetAsync(id, cancellationToken: cancellationToken);
        }
        catch (System.Exception ex)
        {
            throw new InvalidCheckoutDataException($"Error getting stripe product by id {id}", ex);
        }
    }

    private static async Task<StripeProduct> CreateProductAsync(string name, string description, decimal unitAmount, CancellationToken cancellationToken)
    {
        var productService = new ProductService();

        var product = new ProductCreateOptions
        {
            Name = name,
            Description = description,
            DefaultPriceData = new ProductDefaultPriceDataOptions
            {
                TaxBehavior = "inclusive",
                UnitAmountDecimal = unitAmount * 100,
                Currency = "gbp",
            }
        };

        return await productService.CreateAsync(product, cancellationToken: cancellationToken);
    }

    private static async Task<Price> GetPriceAsync(string id, CancellationToken cancellationToken)
    {
        var priceService = new PriceService();

        try
        {
            return await priceService.GetAsync(id, cancellationToken: cancellationToken);
        }
        catch (System.Exception ex)
        {
            throw new InvalidCheckoutDataException($"Error getting stripe price by id {id}", ex);
        }
    }

    private static async Task<Coupon?> GetOrCreateCouponAsync(string? couponId, string productName, decimal? percentOff, string productId, CancellationToken cancellationToken)
    {
        if (percentOff == null)
        {
            return null;
        }

        if (couponId == null)
        {
            return await CreateCouponAsync(productName + percentOff.Value, percentOff, "forever", [productId], cancellationToken);
        }

        Coupon coupon = await GetCouponAsync(couponId, cancellationToken);

        if (!coupon.AppliesTo.Products.Contains(productId) ||
            coupon.PercentOff.HasValue && coupon.PercentOff.Value.ToString("n2") != percentOff.Value.ToString("n2"))
        {
            return await CreateCouponAsync(productName + percentOff.Value, percentOff, "forever", [productId], cancellationToken);
        }

        return coupon;
    }

    private static async Task<Coupon> GetCouponAsync(string id, CancellationToken cancellationToken)
    {
        var service = new CouponService();

        try
        {
            return await service.GetAsync(id, new CouponGetOptions
            {
                Expand = ["applies_to"],
            },
            cancellationToken: cancellationToken);
        }
        catch (System.Exception ex)
        {
            throw new InvalidCheckoutDataException($"Error getting coupon by id {id}", ex);
        }
    }

    private static async Task<Coupon> CreateCouponAsync(string name, decimal? percentage, string duration, List<string> products, CancellationToken cancellationToken)
    {
        var options = new CouponCreateOptions
        {
            Name = name,
            PercentOff = percentage,
            Duration = duration,
            AppliesTo = new CouponAppliesToOptions
            {
                Products = products
            },
        };
        var service = new CouponService();
        return await service.CreateAsync(options, cancellationToken: cancellationToken);
    }
}
