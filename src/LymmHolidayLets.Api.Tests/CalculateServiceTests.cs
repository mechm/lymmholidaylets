using System;
using System.Collections.Generic;
using LymmHolidayLets.Application.Service;
using LymmHolidayLets.Domain.ReadModel.Checkout;
using Xunit;

namespace LymmHolidayLets.Api.Tests
{
    public class CalculateServiceTests
    {
        [Fact]
        public void ReturnsNullPercentage_WhenNoDiscountsMatch()
        {
            var discounts = new List<PropertyNightCoupon>
            {
                new PropertyNightCoupon { NoOfNight = 5, Percentage = 10 }
            };
            var checkIn = new DateOnly(2026, 2, 1);
            var checkout = new DateOnly(2026, 2, 2);
            var (percentage, nights) = CalculateService.Calculate(
                discounts, checkIn, checkout);
            Assert.Null(percentage);
            Assert.Equal(1, nights);
        }

        [Fact]
        public void ReturnsCorrectPercentage_ForMatchingDiscount()
        {
            var discounts = new List<PropertyNightCoupon>
            {
                new PropertyNightCoupon { NoOfNight = 1, Percentage = 5 },
                new PropertyNightCoupon { NoOfNight = 3, Percentage = 15 }
            };
            var checkIn = new DateOnly(2026, 2, 1);
            var checkout = new DateOnly(2026, 2, 4);
            var (percentage, nights) = CalculateService.Calculate(
                discounts, checkIn, checkout);
            Assert.Equal(15, percentage);
            Assert.Equal(3, nights);
        }

        [Fact]
        public void ReturnsLastMatchingDiscount_WhenMultipleMatch()
        {
            var discounts = new List<PropertyNightCoupon>
            {
                new PropertyNightCoupon { NoOfNight = 1, Percentage = 5 },
                new PropertyNightCoupon { NoOfNight = 2, Percentage = 10 },
                new PropertyNightCoupon { NoOfNight = 3, Percentage = 15 }
            };
            var checkIn = new DateOnly(2026, 2, 1);
            var checkout = new DateOnly(2026, 2, 4);
            var (percentage, nights) = CalculateService.Calculate(
                discounts, checkIn, checkout);
            Assert.Equal(15, percentage);
            Assert.Equal(3, nights);
        }
    }
}
