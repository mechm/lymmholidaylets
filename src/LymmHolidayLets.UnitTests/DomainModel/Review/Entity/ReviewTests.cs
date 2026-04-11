using FluentAssertions;
using DomainReview = LymmHolidayLets.Domain.Model.Review.Entity.Review;
using Xunit;

namespace LymmHolidayLets.UnitTests.DomainModel.Review.Entity;

public class ReviewTests
{
    [Fact]
    public void Approve_WithExplicitTimestamp_SetsApprovalState()
    {
        var review = CreateReview();
        var approvedAt = new DateTime(2026, 4, 11, 10, 0, 0, DateTimeKind.Utc);

        review.Approve(approvedAt);

        review.Approved.Should().BeTrue();
        review.DateTimeApproved.Should().Be(approvedAt);
    }

    [Fact]
    public void Approve_WhenAlreadyApproved_Throws()
    {
        var review = CreateReview();
        review.Approve(new DateTime(2026, 4, 11, 10, 0, 0, DateTimeKind.Utc));

        var act = () => review.Approve();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Review is already approved.");
    }

    private static DomainReview CreateReview()
    {
        return new DomainReview(
            propertyID: 1,
            company: "Company",
            description: "Great stay",
            privateNote: null,
            name: "Guest",
            emailAddress: "guest@example.com",
            position: null,
            rating: 5,
            cleanliness: 5,
            accuracy: 4,
            communication: 5,
            location: 4,
            checkin: 5,
            facilities: 4,
            comfort: 5,
            value: 4,
            reviewTypeId: 1,
            linkToView: null,
            showOnHomepage: true,
            dateTimeAdded: new DateTime(2026, 4, 10, 12, 0, 0, DateTimeKind.Utc),
            dateTimeApproved: null,
            approved: false);
    }
}
