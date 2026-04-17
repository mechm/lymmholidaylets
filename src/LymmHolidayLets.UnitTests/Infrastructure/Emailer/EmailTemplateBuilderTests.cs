using FluentAssertions;
using LymmHolidayLets.Domain.Dto.Email;
using LymmHolidayLets.Infrastructure.Emailer;
using Xunit;

namespace LymmHolidayLets.UnitTests.Infrastructure.Emailer;

public class EmailTemplateBuilderTests
{
    private readonly EmailTemplateBuilder _sut = new();

    [Fact]
    public async Task BuildHtmlBookingEmailToCustomer_RendersDynamicBookingValues_AndRemovesLegacyMarketplaceCopy()
    {
        var html = await _sut.BuildHtmlBookingEmailToCustomer(new BookingConfirmationForCustomer
        {
            PropertyName = "Lymm Village Apartment",
            CheckIn = new DateOnly(2026, 6, 1),
            CheckOut = new DateOnly(2026, 6, 4),
            NoAdult = 2,
            NoChildren = 1,
            NoInfant = 0,
            Name = "Jane Smith",
            Email = "jane@example.com",
            Telephone = "07700900123",
            PostalCode = "WA13 0QG",
            Country = "United Kingdom",
            Total = 45600,
            CheckInTimeAfter = new TimeOnly(15, 0),
            CheckOutTimeBefore = new TimeOnly(10, 0),
            FullAddress = "5 Church Road, Lymm, Cheshire, WA13 0QG, England",
            DirectionsUrl = "https://maps.example.com/route",
            ArrivalInstructions = "Use the side entrance and collect the keys from the lockbox.",
            HeroImageUrl = "https://lymmholidaylets.com/images/property.jpg",
            HeroImageAltText = "Lymm Village Apartment exterior",
            HouseRules = ["No pets", "Self check-in with lockbox"],
            SafetyItems = ["Smoke alarm", "Carbon monoxide alarm"],
            CancellationPolicyText = "Free cancellation within 48 hours.\nAfter that, this reservation is non-refundable.",
            PaymentLines =
            [
                new BookingConfirmationPaymentLine("3 nights", 450m),
                new BookingConfirmationPaymentLine("Cleaning fee", 90m)
            ]
        });

        html.Should().Contain("Lymm Holiday Lets");
        html.Should().Contain("Jane Smith");
        html.Should().Contain("Lymm Village Apartment");
        html.Should().Contain("Mon, 1 Jun 2026");
        html.Should().Contain("Thu, 4 Jun 2026");
        html.Should().Contain("From 15:00");
        html.Should().Contain("Before 10:00");
        html.Should().Contain("5 Church Road, Lymm, Cheshire, WA13 0QG, England");
        html.Should().Contain("https://maps.example.com/route");
        html.Should().Contain("Use the side entrance and collect the keys from the lockbox.");
        html.Should().Contain("2 adults, 1 child");
        html.Should().Contain("No pets");
        html.Should().Contain("Smoke alarm");
        html.Should().Contain("3 nights</td>");
        html.Should().Contain("Cleaning fee</td>");
        html.Should().Contain("&#xA3;456.00");
        html.Should().NotContain("Airbnb");
        html.Should().Contain("this reservation is non-refundable.");
    }

    [Fact]
    public async Task BuildHtmlBookingEmailToCompany_RendersDetailedGuestAndBookingInformation()
    {
        var html = await _sut.BuildHtmlBookingEmailToCompany(new BookingConfirmationForCompany(
            "Lymm Cottage",
            new DateOnly(2026, 7, 10),
            new DateOnly(2026, 7, 12),
            2,
            0,
            1,
            "John Doe",
            "john@example.com",
            "07700900124",
            "M1 1AA",
            "United Kingdom",
            32500));

        html.Should().Contain("New booking received");
        html.Should().Contain("Lymm Cottage");
        html.Should().Contain("John Doe");
        html.Should().Contain("john@example.com");
        html.Should().Contain("07700900124");
        html.Should().Contain("M1 1AA");
        html.Should().Contain("United Kingdom");
        html.Should().Contain("2 adults, 1 infant");
        html.Should().Contain("&#xA3;325.00");
    }
}
