using FluentAssertions;
using LymmHolidayLets.Domain.Model.Review.ValueObject;
using Xunit;

namespace LymmHolidayLets.UnitTests.DomainModel.Review.ValueObject;

public class ReviewRatingsTests
{
    [Fact]
    public void Summarize_WithRatings_ReturnsAverages()
    {
        var ratings = new[]
        {
            new ReviewRatings(5, 4, 5, 4, 5, 4, 5, 4, 5),
            new ReviewRatings(3, null, 3, 2, null, 5, null, 4, 3)
        };

        var summary = ReviewRatings.Summarize(ratings, totalReviews: 8);

        summary.Should().NotBeNull();
        summary!.Overall.Should().Be(4);
        summary.Accuracy.Should().Be(4);
        summary.Cleanliness.Should().Be(4);
        summary.Communication.Should().Be(3);
        summary.CheckIn.Should().Be(5);
        summary.Location.Should().Be(4.5);
        summary.Facilities.Should().Be(5);
        summary.Comfort.Should().Be(4);
        summary.Value.Should().Be(4);
        summary.TotalReviews.Should().Be(8);
    }

    [Fact]
    public void Summarize_WithNoRatings_ReturnsNull()
    {
        var summary = ReviewRatings.Summarize([], totalReviews: 0);

        summary.Should().BeNull();
    }
}
