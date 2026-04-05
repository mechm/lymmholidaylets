using FluentAssertions;
using LymmHolidayLets.Api.Models.Property;
using LymmHolidayLets.Application.Model.Property;
using Xunit;

namespace LymmHolidayLets.UnitTests.Api.Models;

public class ReviewResponseTests
{
    [Fact]
    public void FromApplicationModel_WithDateThreeDaysAgo_FormatsCorrectly()
    {
        var reviewDate = DateTime.Now.AddDays(-3);
        var applicationModel = new PropertyReviewResult
        {
            Name = "John Doe",
            Description = "Great place!",
            Rating = 5,
            ReviewType = "Google",
            DateTimeAdded = reviewDate
        };

        var result = ReviewResponse.FromApplicationModel(applicationModel);

        result.DateToDisplay.Should().Be("3 days ago");
        result.Name.Should().Be("John Doe");
        result.DateTimeAdded.Should().Be(reviewDate);
    }

    [Fact]
    public void FromApplicationModel_WithNullDate_ReturnsEmptyString()
    {
        var applicationModel = new PropertyReviewResult
        {
            Name = "Jane Doe",
            Description = "Nice!",
            Rating = 4,
            ReviewType = "TripAdvisor",
            DateTimeAdded = null
        };

        var result = ReviewResponse.FromApplicationModel(applicationModel);

        result.DateToDisplay.Should().BeEmpty();
        result.DateTimeAdded.Should().BeNull();
    }

    [Fact]
    public void FromApplicationModel_Today_ReturnsToday()
    {
        var applicationModel = new PropertyReviewResult
        {
            Name = "Test User",
            Description = "Test",
            Rating = 5,
            ReviewType = "Airbnb",
            DateTimeAdded = DateTime.Now.AddHours(-13)
        };

        var result = ReviewResponse.FromApplicationModel(applicationModel);

        result.DateToDisplay.Should().Be("today");
    }

    [Fact]
    public void FromApplicationModel_OneWeekAgo_ReturnsCorrectFormat()
    {
        var applicationModel = new PropertyReviewResult
        {
            Name = "Test User",
            Description = "Test",
            Rating = 5,
            ReviewType = "Google",
            DateTimeAdded = DateTime.Now.AddDays(-7)
        };

        var result = ReviewResponse.FromApplicationModel(applicationModel);

        result.DateToDisplay.Should().Be("a week ago");
    }

    [Fact]
    public void FromApplicationModel_TwoMonthsAgo_ReturnsCorrectFormat()
    {
        var applicationModel = new PropertyReviewResult
        {
            Name = "Test User",
            Description = "Test",
            Rating = 5,
            ReviewType = "Google",
            DateTimeAdded = DateTime.Now.AddMonths(-2)
        };

        var result = ReviewResponse.FromApplicationModel(applicationModel);

        result.DateToDisplay.Should().Be("2 months ago");
    }

    [Fact]
    public void FromApplicationModel_OneYearAgo_ReturnsCorrectFormat()
    {
        var applicationModel = new PropertyReviewResult
        {
            Name = "Test User",
            Description = "Test",
            Rating = 5,
            ReviewType = "Google",
            DateTimeAdded = DateTime.Now.AddYears(-1)
        };

        var result = ReviewResponse.FromApplicationModel(applicationModel);

        result.DateToDisplay.Should().Be("a year ago");
    }
}
