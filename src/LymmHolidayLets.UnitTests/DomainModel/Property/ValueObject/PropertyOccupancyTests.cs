using FluentAssertions;
using LymmHolidayLets.Domain.ReadModel.Property;
using Xunit;

namespace LymmHolidayLets.UnitTests.DomainModel.Property.ValueObject;

public class PropertyOccupancyTests
{
    [Fact]
    public void Occupancy_MapsGuestCapacityFieldsIntoValueObject()
    {
        var booking = new PropertyBooking
        {
            ID = 1,
            MinimumNumberOfAdult = 2,
            MaximumNumberOfGuests = 6,
            MaximumNumberOfAdult = 4,
            MaximumNumberOfChildren = 2,
            MaximumNumberOfInfants = 1,
            HostName = "Host",
            HostJobTitle = "Owner"
        };

        booking.Occupancy.MinimumAdults.Should().Be(2);
        booking.Occupancy.MaximumGuests.Should().Be(6);
        booking.Occupancy.MaximumAdults.Should().Be(4);
        booking.Occupancy.MaximumChildren.Should().Be(2);
        booking.Occupancy.MaximumInfants.Should().Be(1);
    }
}
