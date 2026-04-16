using FluentAssertions;
using LymmHolidayLets.Domain.ReadModel.Checkout;
using Xunit;

namespace LymmHolidayLets.UnitTests.DomainModel.Checkout;

public class PropertyNightCouponTests
{
    [Fact]
    public void SelectApplicableDiscountPercentage_WhenNoDiscountsMatch_ReturnsNull()
    {
        var discounts = new[]
        {
            new PropertyNightCoupon { NoOfNight = 5, Percentage = 10 }
        };

        var percentage = PropertyNightCoupon.SelectApplicableDiscountPercentage(discounts, 1);

        percentage.Should().BeNull();
    }

    [Fact]
    public void SelectApplicableDiscountPercentage_WhenMultipleMatch_ReturnsHighestApplicableTier()
    {
        var discounts = new[]
        {
            new PropertyNightCoupon { NoOfNight = 1, Percentage = 5 },
            new PropertyNightCoupon { NoOfNight = 2, Percentage = 10 },
            new PropertyNightCoupon { NoOfNight = 3, Percentage = 15 }
        };

        var percentage = PropertyNightCoupon.SelectApplicableDiscountPercentage(discounts, 3);

        percentage.Should().Be(15);
    }
}
