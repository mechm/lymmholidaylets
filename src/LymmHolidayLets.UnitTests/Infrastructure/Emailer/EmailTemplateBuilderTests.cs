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
        html.Should().Contain("You don't need to do anything else right now");
        html.Should().Contain("From 15:00");
        html.Should().Contain("Before 10:00");
        html.Should().Contain("Where you're staying");
        html.Should().Contain("5 Church Road, Lymm, Cheshire, WA13 0QG, England");
        html.Should().Contain("https://maps.example.com/route");
        html.Should().Contain("Arrival details");
        html.Should().Contain("Use the side entrance and collect the keys from the lockbox.");
        html.Should().Contain("2 adults, 1 child");
        html.Should().Contain("No pets");
        html.Should().Contain("Smoke alarm");
        html.Should().Contain("3 nights</td>");
        html.Should().Contain("Cleaning fee</td>");
        html.Should().Contain("&#xA3;456.00");
        html.Should().Contain(">Phone</td>");
        html.Should().Contain("white-space:nowrap");
        html.Should().Contain("matthew@lymmholidaylets.com");
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

    [Fact]
    public async Task BuildHtmlBookingEmailToCustomer_DoesNotRenderArrivalHeader_WhenArrivalInstructionsMissing()
    {
        var html = await _sut.BuildHtmlBookingEmailToCustomer(new BookingConfirmationForCustomer
        {
            PropertyName = "Lymm Village Apartment",
            CheckIn = new DateOnly(2026, 6, 1),
            CheckOut = new DateOnly(2026, 6, 4),
            NoAdult = 2,
            NoChildren = 0,
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
            ArrivalInstructions = null,
            HouseRules = [],
            SafetyItems = [],
            CancellationPolicyText = "Non-refundable.",
            PaymentLines = []
        });

        html.Should().Contain("Where you're staying");
        html.Should().Contain("https://maps.example.com/route");
        html.Should().NotContain("Arrival details");
    }

    [Fact]
    public async Task BuildGuestPreArrivalEmail_RendersConfiguredTemplatePlaceholdersInsideSharedShell()
    {
        var model = new GuestPreArrivalEmail
        {
            PropertyName = "Lymm Village Apartment",
            BookingReference = "LHL-ABC123456789",
            CheckIn = new DateOnly(2026, 10, 29),
            CheckOut = new DateOnly(2026, 11, 2),
            Name = "Jane & John",
            Email = "jane@example.com",
            Telephone = "07700900123",
            PostalCode = "WA13 0QG",
            Country = "England",
            Total = 52500,
            CheckInTimeAfter = new TimeOnly(15, 0),
            CheckOutTimeBefore = new TimeOnly(10, 0),
            FullAddress = "5 Church Road, Lymm, Cheshire, WA13 0QG, England",
            DirectionsUrl = "https://maps.example.com/route",
            ArrivalInstructions = "Collect the keys from the lockbox.",
            SubjectTemplate = "Arrival details for {propertyname}",
            PreviewTextTemplate = "You can arrive from {arrivaltime} on {arrivaldate}.",
            HtmlBodyTemplate = """
                <p>Hello {guestname},</p>
                <p>You are welcome to arrive from {arrivaltime} on {arrivaldate}.</p>
                <p>Your booking reference is {{BookingReference}}.</p>
                """
        };

        var subject = await _sut.BuildSubjectGuestPreArrivalEmail(model);
        var html = await _sut.BuildHtmlGuestPreArrivalEmail(model);

        subject.Should().Be("Arrival details for Lymm Village Apartment");
        html.Should().Contain("Your stay at Lymm Village Apartment starts soon");
        html.Should().Contain("Jane &amp; John");
        html.Should().Contain("You can arrive from 15:00 on Thursday 29 October 2026.");
        html.Should().Contain("Your booking reference is LHL-ABC123456789.");
        html.Should().Contain("Thu, 29 Oct 2026");
        html.Should().Contain("Mon, 2 Nov 2026");
        html.Should().Contain("From 15:00");
        html.Should().Contain("Before 10:00");
        html.Should().Contain("5 Church Road, Lymm, Cheshire, WA13 0QG, England");
        html.Should().Contain("https://maps.example.com/route");
        html.Should().Contain("Collect the keys from the lockbox.");
        html.Should().NotContain("{guestname}");
        html.Should().NotContain("{arrivaltime}");
        html.Should().NotContain("{{BookingReference}}");
    }
}
