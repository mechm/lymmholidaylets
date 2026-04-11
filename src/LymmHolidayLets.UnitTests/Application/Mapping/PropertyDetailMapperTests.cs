using FluentAssertions;
using LymmHolidayLets.Application.Mapping;
using LymmHolidayLets.Domain.ReadModel.Property;
using Xunit;

namespace LymmHolidayLets.UnitTests.Application.Mapping;

public class PropertyDetailMapperTests
{
    [Fact]
    public void Map_WithReviewRatings_MapsRatingSummaryFromValueObject()
    {
        var aggregate = new PropertyDetailAggregate(
            new PropertyBooking
            {
                ID                      = 1,
                HostName                = "Host",
                HostJobTitle            = "Owner",
                MinimumNumberOfAdult    = 1,
                MaximumNumberOfGuests   = 4,
                MaximumNumberOfAdult    = 4,
                MaximumNumberOfChildren = 2,
                MaximumNumberOfInfants  = 1
            },
            [],
            [],
            [
                new Review
                {
                    Name             = "Alice",
                    Description      = "Great stay",
                    ReviewType       = "Google",
                    Rating           = 5,
                    Accuracy         = 4,
                    Cleanliness      = 5,
                    Communication    = 5,
                    Checkin          = 4,
                    Location         = 5,
                    Facilities       = null,
                    Comfort          = 4,
                    Value            = 4,
                    TotalReviewCount = 9
                },
                new Review
                {
                    Name             = "Bob",
                    Description      = "Lovely",
                    ReviewType       = "TripAdvisor",
                    Rating           = 3,
                    Accuracy         = null,
                    Cleanliness      = 3,
                    Communication    = 4,
                    Checkin          = null,
                    Location         = 4,
                    Facilities       = 5,
                    Comfort          = 3,
                    Value            = 3,
                    TotalReviewCount = 9
                }
            ],
            [],
            [],
            []);

        var result = PropertyDetailMapper.Map(aggregate);

        result.RatingSummary.Should().NotBeNull();
        result.RatingSummary!.Rating.Should().Be(4);
        result.RatingSummary.Accuracy.Should().Be(4);
        result.RatingSummary.Cleanliness.Should().Be(4);
        result.RatingSummary.Communication.Should().Be(4.5);
        result.RatingSummary.CheckInExperience.Should().Be(4);
        result.RatingSummary.Location.Should().Be(4.5);
        result.RatingSummary.Facilities.Should().Be(5);
        result.RatingSummary.Comfort.Should().Be(3.5);
        result.RatingSummary.Value.Should().Be(3.5);
        result.RatingSummary.TotalReviews.Should().Be(9);
    }
}
