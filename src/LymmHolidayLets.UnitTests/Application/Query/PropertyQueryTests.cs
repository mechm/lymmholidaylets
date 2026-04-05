using FluentAssertions;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Property;
using LymmHolidayLets.Domain.Repository.EF;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Application.Query;

public class PropertyQueryTests
{
    private readonly Mock<IDapperPropertyDataAdapter> _propertyDataAdapter = new();
    private readonly Mock<IPropertyRepositoryEF> _propertyRepositoryEf = new();

    private IPropertyQuery CreateSut() =>
        new PropertyQuery(_propertyDataAdapter.Object, _propertyRepositoryEf.Object);

    [Fact]
    public async Task GetPropertyDetailByIdAsync_PropertyNotFound_ReturnsNull()
    {
        _propertyDataAdapter
            .Setup(a => a.GetPropertyDetailByIdAsync(1))
            .ReturnsAsync((PropertyDetailAggregate?)null);

        var result = await CreateSut().GetPropertyDetailByIdAsync(1);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPropertyDetailByIdAsync_PropertyFound_MapsAllFields()
    {
        var aggregate = new PropertyDetailAggregate(
            new PropertyBooking
            {
                ID                      = 1,
                DisplayAddress          = "123 Test Street",
                PageDescription         = "A lovely test property",
                MinimumNumberOfAdult    = 2,
                MaximumNumberOfGuests   = 8,
                MaximumNumberOfAdult    = 6,
                MaximumNumberOfChildren = 2,
                MaximumNumberOfInfants  = 1,
                HostName                = "Test Host",
                HostLocation            = "Test Location",
                NumberOfProperties      = 5,
                HostYearsExperience     = 10,
                HostJobTitle            = "Host Manager",
                HostProfileBio          = "Experienced host",
                HostImagePath           = "/images/host.jpg",
                ShowMap                 = true,
                ShowStreetView          = false,
                Latitude                = 51.5074,
                Longitude               = -0.1278,
                MapZoom                 = 12,
                StreetViewLatitude      = 51.5075,
                StreetViewLongitude     = -0.1279,
                Pitch                   = 5,
                Yaw                     = 90,
                Zoom                    = 1.5
            },
            [new DateOnly(2026, 8, 15), new DateOnly(2026, 8, 16)],
            [new FAQ { Question = "Check-in time?", Answer = "After 3 PM" }],
            [
                new Review
                {
                    Name        = "Test Reviewer",
                    Description = "Great property!",
                    Rating      = 5,
                    ReviewType  = "Google"
                }
            ]
        );

        _propertyDataAdapter
            .Setup(a => a.GetPropertyDetailByIdAsync(1))
            .ReturnsAsync(aggregate);

        var result = await CreateSut().GetPropertyDetailByIdAsync(1);

        result.Should().NotBeNull();
        result!.PropertyId.Should().Be(1);
        result.DisplayAddress.Should().Be("123 Test Street");
        result.PageDescription.Should().Be("A lovely test property");
        result.MinimumNumberOfAdult.Should().Be(2);
        result.MaximumNumberOfGuests.Should().Be(8);
        result.DatesBooked.Should().HaveCount(2);
        result.FaQs.Should().HaveCount(1);
        result.ReviewAggregate.Should().NotBeNull();
        result.ReviewAggregate!.Reviews.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPropertyDetailByIdAsync_PropertyFound_MapsHostInformation()
    {
        var aggregate = new PropertyDetailAggregate(
            new PropertyBooking
            {
                ID                      = 1,
                MinimumNumberOfAdult    = 1,
                MaximumNumberOfGuests   = 4,
                MaximumNumberOfAdult    = 4,
                MaximumNumberOfChildren = 0,
                MaximumNumberOfInfants  = 0,
                HostName                = "Jane Smith",
                HostLocation            = "Manchester",
                NumberOfProperties      = 3,
                HostYearsExperience     = 7,
                HostJobTitle            = "Property Owner",
                HostProfileBio          = "Passionate about hospitality",
                HostImagePath           = "/images/jane.jpg",
                ShowMap                 = false,
                ShowStreetView          = false,
                Latitude                = 0,
                Longitude               = 0,
                MapZoom                 = 0,
                StreetViewLatitude      = 0,
                StreetViewLongitude     = 0,
                Pitch                   = 0,
                Yaw                     = 0,
                Zoom                    = 0
            },
            [],
            [],
            []
        );

        _propertyDataAdapter
            .Setup(a => a.GetPropertyDetailByIdAsync(1))
            .ReturnsAsync(aggregate);

        var result = await CreateSut().GetPropertyDetailByIdAsync(1);

        result.Should().NotBeNull();
        result!.Host.Should().NotBeNull();
        result.Host!.Name.Should().Be("Jane Smith");
        result.Host.Location.Should().Be("Manchester");
        result.Host.NumberOfProperties.Should().Be(3);
        result.Host.YearsExperience.Should().Be(7);
        result.Host.JobTitle.Should().Be("Property Owner");
        result.Host.ProfileBio.Should().Be("Passionate about hospitality");
        result.Host.ImagePath.Should().Be("/images/jane.jpg");
    }

    [Fact]
    public async Task GetPropertyDetailByIdAsync_PropertyFound_MapsMapInformation()
    {
        var aggregate = new PropertyDetailAggregate(
            new PropertyBooking
            {
                ID                      = 1,
                MinimumNumberOfAdult    = 1,
                MaximumNumberOfGuests   = 4,
                MaximumNumberOfAdult    = 4,
                MaximumNumberOfChildren = 0,
                MaximumNumberOfInfants  = 0,
                HostName                = "Test",
                HostJobTitle            = "Test",
                ShowMap                 = true,
                ShowStreetView          = true,
                Latitude                = 53.3811,
                Longitude               = -2.4730,
                MapZoom                 = 15,
                StreetViewLatitude      = 53.3812,
                StreetViewLongitude     = -2.4731,
                Pitch                   = 10,
                Yaw                     = 165,
                Zoom                    = 1.2
            },
            [],
            [],
            []
        );

        _propertyDataAdapter
            .Setup(a => a.GetPropertyDetailByIdAsync(1))
            .ReturnsAsync(aggregate);

        var result = await CreateSut().GetPropertyDetailByIdAsync(1);

        result.Should().NotBeNull();
        result!.Map.Should().NotBeNull();
        result.Map!.ShowMap.Should().BeTrue();
        result.Map.ShowStreetView.Should().BeTrue();
        result.Map.Latitude.Should().Be(53.3811);
        result.Map.Longitude.Should().Be(-2.4730);
        result.Map.MapZoom.Should().Be(15);
        result.Map.StreetViewLatitude.Should().Be(53.3812);
        result.Map.StreetViewLongitude.Should().Be(-2.4731);
        result.Map.Pitch.Should().Be(10);
        result.Map.Yaw.Should().Be(165);
        result.Map.Zoom.Should().Be(1.2);
    }

    [Fact]
    public async Task GetPropertyDetailByIdAsync_NoReviews_ReturnsNullReviewAggregate()
    {
        var aggregate = new PropertyDetailAggregate(
            new PropertyBooking
            {
                ID                      = 1,
                MinimumNumberOfAdult    = 1,
                MaximumNumberOfGuests   = 4,
                MaximumNumberOfAdult    = 4,
                MaximumNumberOfChildren = 0,
                MaximumNumberOfInfants  = 0,
                HostName                = "Test",
                HostJobTitle            = "Test",
                ShowMap                 = false,
                ShowStreetView          = false,
                Latitude                = 0,
                Longitude               = 0,
                MapZoom                 = 0,
                StreetViewLatitude      = 0,
                StreetViewLongitude     = 0,
                Pitch                   = 0,
                Yaw                     = 0,
                Zoom                    = 0
            },
            [],
            [],
            []
        );

        _propertyDataAdapter
            .Setup(a => a.GetPropertyDetailByIdAsync(1))
            .ReturnsAsync(aggregate);

        var result = await CreateSut().GetPropertyDetailByIdAsync(1);

        result.Should().NotBeNull();
        result!.ReviewAggregate.Should().BeNull();
    }

    [Fact]
    public async Task GetPropertyDetailByIdAsync_WithReview_MapsReviewData()
    {
        var reviewDate = DateTime.Now.AddDays(-3);
        var aggregate = new PropertyDetailAggregate(
            new PropertyBooking
            {
                ID                      = 1,
                DisplayAddress          = "123 Test Street",
                PageDescription         = "Test",
                MinimumNumberOfAdult    = 1,
                MaximumNumberOfGuests   = 4,
                MaximumNumberOfAdult    = 2,
                MaximumNumberOfChildren = 1,
                MaximumNumberOfInfants  = 1,
                HostName                = "Test Host",
                HostLocation            = "",
                NumberOfProperties      = 1,
                HostYearsExperience     = 5,
                HostJobTitle            = "Host",
                HostProfileBio          = "Bio",
                HostImagePath           = "/path",
                ShowMap                 = true,
                ShowStreetView          = false,
                Latitude                = 53.0,
                Longitude               = -2.5,
                MapZoom                 = 15,
                StreetViewLatitude      = 53.0,
                StreetViewLongitude     = -2.5,
                Pitch                   = 0,
                Yaw                     = 0,
                Zoom                    = 1
            },
            [DateOnly.FromDateTime(DateTime.Today.AddDays(5))],
            [],
            [
                new Review
                {
                    Name          = "John Doe",
                    Company       = "Test Corp",
                    Position      = "Tester",
                    Description   = "Great place!",
                    Rating        = 5,
                    DateTimeAdded = reviewDate,
                    ReviewType    = "Google",
                    LinkToView    = "http://example.com"
                }
            ]
        );

        _propertyDataAdapter
            .Setup(a => a.GetPropertyDetailByIdAsync(1))
            .ReturnsAsync(aggregate);

        var result = await CreateSut().GetPropertyDetailByIdAsync(1);

        result.Should().NotBeNull();
        result!.ReviewAggregate.Should().NotBeNull();
        result.ReviewAggregate!.Reviews.Should().HaveCount(1);
        
        var review = result.ReviewAggregate.Reviews.First();
        review.DateTimeAdded.Should().Be(reviewDate);
        review.Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetPropertyDetailByIdAsync_WithNullReviewDate_MapsCorrectly()
    {
        var aggregate = new PropertyDetailAggregate(
            new PropertyBooking
            {
                ID                      = 1,
                DisplayAddress          = "123 Test Street",
                PageDescription         = "Test",
                MinimumNumberOfAdult    = 1,
                MaximumNumberOfGuests   = 4,
                MaximumNumberOfAdult    = 2,
                MaximumNumberOfChildren = 1,
                MaximumNumberOfInfants  = 1,
                HostName                = "Test Host",
                HostLocation            = "",
                NumberOfProperties      = 1,
                HostYearsExperience     = 5,
                HostJobTitle            = "Host",
                HostProfileBio          = "Bio",
                HostImagePath           = "/path",
                ShowMap                 = true,
                ShowStreetView          = false,
                Latitude                = 53.0,
                Longitude               = -2.5,
                MapZoom                 = 15,
                StreetViewLatitude      = 53.0,
                StreetViewLongitude     = -2.5,
                Pitch                   = 0,
                Yaw                     = 0,
                Zoom                    = 1
            },
            [],
            [],
            [
                new Review
                {
                    Name          = "Jane Doe",
                    Description   = "Nice!",
                    Rating        = 4,
                    DateTimeAdded = null,
                    ReviewType    = "TripAdvisor"
                }
            ]
        );

        _propertyDataAdapter
            .Setup(a => a.GetPropertyDetailByIdAsync(1))
            .ReturnsAsync(aggregate);

        var result = await CreateSut().GetPropertyDetailByIdAsync(1);

        result.Should().NotBeNull();
        result!.ReviewAggregate.Should().NotBeNull();
        result.ReviewAggregate!.Reviews.Should().HaveCount(1);
        
        var review = result.ReviewAggregate.Reviews.First();
        review.DateTimeAdded.Should().BeNull();
    }
}
