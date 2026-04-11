using FluentAssertions;
using DomainCalendar = LymmHolidayLets.Domain.Model.Calendar.Entity.Calendar;
using Xunit;

namespace LymmHolidayLets.UnitTests.DomainModel.Calendar.Entity;

public class CalendarTests
{
    [Fact]
    public void IsAvailable_WhenAvailableAndNotBooked_ReturnsTrue()
    {
        var calendar = new DomainCalendar(
            propertyID: 1,
            date: new DateTime(2026, 4, 11),
            price: 150m,
            minimumStay: 2,
            maximumStay: 7,
            available: true,
            booked: false,
            bookingID: null);

        calendar.IsAvailable().Should().BeTrue();
    }

    [Fact]
    public void BlockForBooking_ReturnsBookedCalendarEntry()
    {
        var calendar = new DomainCalendar(
            id: 5,
            propertyID: 1,
            date: new DateTime(2026, 4, 11),
            price: 150m,
            minimumStay: 2,
            maximumStay: 7,
            available: true,
            booked: false,
            bookingID: null);

        var blocked = calendar.BlockForBooking(42);

        blocked.ID.Should().Be(5);
        blocked.PropertyID.Should().Be(1);
        blocked.Available.Should().BeFalse();
        blocked.Booked.Should().BeTrue();
        blocked.BookingID.Should().Be(42);
    }
}
