using FluentAssertions;
using DomainCheckout = LymmHolidayLets.Domain.Model.Checkout.Entity.Checkout;
using Xunit;

namespace LymmHolidayLets.UnitTests.DomainModel.Checkout.Entity;

public class CheckoutTests
{
    [Fact]
    public void IsExpired_WhenWithinTtl_ReturnsFalse()
    {
        var created = new DateTime(2026, 4, 11, 10, 0, 0, DateTimeKind.Utc);
        var checkout = CreateCheckout(created);

        checkout.IsExpired(TimeSpan.FromHours(1), created.AddMinutes(30)).Should().BeFalse();
    }

    [Fact]
    public void IsExpired_WhenAtOrBeyondTtl_ReturnsTrue()
    {
        var created = new DateTime(2026, 4, 11, 10, 0, 0, DateTimeKind.Utc);
        var checkout = CreateCheckout(created);

        checkout.IsExpired(TimeSpan.FromHours(1), created.AddHours(1)).Should().BeTrue();
        checkout.IsExpired(TimeSpan.FromHours(1), created.AddHours(2)).Should().BeTrue();
    }

    private static DomainCheckout CreateCheckout(DateTime created)
    {
        return new DomainCheckout(
            propertyID: 1,
            checkIn: new DateOnly(2026, 4, 20),
            checkOut: new DateOnly(2026, 4, 23),
            stripeNightProductID: "prod_123",
            stripeNightDefaultPriceID: "price_123",
            stripeNightDefaultUnitPrice: 150m,
            stripeNightCouponID: "coupon_123",
            stripeNightPercentage: 10m,
            overallPrice: 450m)
        {
            Created = created
        };
    }
}
