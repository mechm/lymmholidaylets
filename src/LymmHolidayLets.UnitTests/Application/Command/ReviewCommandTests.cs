using FluentAssertions;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;
using Moq;
using DomainReview = LymmHolidayLets.Domain.Model.Review.Entity.Review;
using ReviewCommandHandler = LymmHolidayLets.Application.Command.ReviewCommand;
using Xunit;

namespace LymmHolidayLets.UnitTests.Application.Command;

public class ReviewCommandTests
{
    private readonly Mock<IReviewRepository> _reviewRepository = new();
    private readonly Mock<IPropertyCacheInvalidator> _cacheInvalidator = new();

    private ReviewCommandHandler CreateSut() => new(_reviewRepository.Object, _cacheInvalidator.Object);

    [Fact]
    public void Create_WhenApproved_PersistsApprovedReview()
    {
        DomainReview? savedReview = null;
        _reviewRepository
            .Setup(repository => repository.Create(It.IsAny<DomainReview>()))
            .Callback<DomainReview>(review => savedReview = review);

        var command = CreateReviewCommand(approved: true);

        CreateSut().Create(command);

        savedReview.Should().NotBeNull();
        savedReview!.Approved.Should().BeTrue();
        savedReview.DateTimeApproved.Should().NotBeNull();
    }

    [Fact]
    public void Update_WhenReviewAlreadyApproved_PreservesRegistrationCodeAndApprovalTimestamp()
    {
        var approvedAt = new DateTime(2026, 4, 10, 9, 0, 0, DateTimeKind.Utc);
        var createdAt = new DateTime(2026, 4, 1, 9, 0, 0, DateTimeKind.Utc);
        var registrationCode = Guid.NewGuid();
        var currentReview = new DomainReview(
            propertyID: 1,
            reviewId: 7,
            company: "Company",
            description: "Original",
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
            dateTimeAdded: createdAt,
            dateTimeApproved: approvedAt,
            registrationCode: registrationCode,
            approved: true,
            created: createdAt);

        DomainReview? savedReview = null;
        _reviewRepository.Setup(repository => repository.GetById(7)).Returns(currentReview);
        _reviewRepository
            .Setup(repository => repository.Update(It.IsAny<DomainReview>()))
            .Callback<DomainReview>(review => savedReview = review);

        var command = CreateReviewCommand(approved: true);
        command.ReviewId = 7;
        command.Description = "Updated";

        CreateSut().Update(command);

        savedReview.Should().NotBeNull();
        savedReview!.RegistrationCode.Should().Be(registrationCode);
        savedReview.DateTimeApproved.Should().Be(approvedAt);
        savedReview.Created.Should().Be(createdAt);
        savedReview.Approved.Should().BeTrue();
    }

    private static Review CreateReviewCommand(bool approved)
    {
        return new Review
        {
            ReviewId = 1,
            PropertyID = 1,
            Company = "Company",
            Description = "Great stay",
            PrivateNote = null,
            Name = "Guest",
            EmailAddress = "guest@example.com",
            Position = null,
            Rating = 5,
            Cleanliness = 5,
            Accuracy = 4,
            Communication = 5,
            Location = 4,
            Checkin = 5,
            Facilities = 4,
            Comfort = 5,
            Value = 4,
            ReviewTypeId = 1,
            LinkToView = null,
            ShowOnHomepage = true,
            DateTimeAdded = new DateTime(2026, 4, 10, 12, 0, 0, DateTimeKind.Utc),
            DateTimeApproved = null,
            Approved = approved,
            Created = new DateTime(2026, 4, 10, 12, 0, 0, DateTimeKind.Utc)
        };
    }
}
