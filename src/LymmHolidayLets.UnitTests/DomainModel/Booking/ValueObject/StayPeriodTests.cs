using FluentAssertions;
using LymmHolidayLets.Domain.Model.Booking.ValueObject;
using Xunit;

namespace LymmHolidayLets.UnitTests.DomainModel.Booking.ValueObject;

public class StayPeriodTests
{
    [Fact]
    public void Constructor_WhenCheckInIsInPast_Throws()
    {
        var checkIn = DateTime.UtcNow.Date.AddDays(-2);
        var checkOut = DateTime.UtcNow.Date.AddDays(1);

        var action = () => new StayPeriod(checkIn, checkOut);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Reconstitute_WhenCheckInIsInPast_AllowsHistoricalBooking()
    {
        var checkIn = DateTime.UtcNow.Date.AddDays(-10);
        var checkOut = DateTime.UtcNow.Date.AddDays(-7);

        var stay = StayPeriod.Reconstitute(checkIn, checkOut);

        stay.CheckIn.Should().Be(checkIn);
        stay.CheckOut.Should().Be(checkOut);
        stay.Nights.Should().Be(3);
    }
}
