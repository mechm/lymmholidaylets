using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Models.Checkout;
using LymmHolidayLets.Api.Models.Email;
using LymmHolidayLets.Api.Models.Homepage;
using LymmHolidayLets.Application.Model.Service;
using LymmHolidayLets.IntegrationTests.Infrastructure;
using Moq;
using Xunit;

namespace LymmHolidayLets.IntegrationTests.Workflows;

/// <summary>
/// End-to-end workflow tests that simulate the user journey from
/// browsing the homepage through to initiating checkout.
/// Uses mocked data layer — no real database required.
/// </summary>
public class BookingWorkflowTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task HomepageThenCheckout_HappyPath_BothReturn200()
    {
        // Arrange: homepage returns data
        factory.HomepageService
            .Setup(s => s.GetHomepageDataAsync())
            .ReturnsAsync(new HomepageModel([], []));

        // Arrange: checkout returns a valid session
        factory.CheckoutService
            .Setup(s => s.CheckoutAsync(
                It.IsAny<byte>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
                It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CheckoutResponse.Success(new CheckoutResult
            {
                SessionId = "cs_test_workflow",
                SessionUrl = "https://checkout.stripe.com/pay/cs_test_workflow",
                CheckIn = new DateOnly(2026, 8, 1),
                CheckOut = new DateOnly(2026, 8, 8)
            }));

        // Act: load homepage
        var homepageResponse = await _client.GetAsync("/api/v1/homepage/init");

        // Act: initiate checkout
        var checkoutResponse = await _client.PostAsJsonAsync(
            "/api/v1/checkout/create-checkout-session",
            new CheckoutItemForm
            {
                PropertyId = 1,
                Checkin = new DateOnly(2026, 8, 1),
                Checkout = new DateOnly(2026, 8, 8),
                NumberOfAdults = 2
            });

        // Assert
        homepageResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        checkoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var checkoutBody = await checkoutResponse.Content.ReadFromJsonAsync<ApiResponse<object>>();
        checkoutBody!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task EnquirySubmission_HappyPath_Returns200()
    {
        factory.RecaptchaValidationService
            .Setup(r => r.ValidateAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        factory.EmailEnquiryService
            .Setup(s => s.ProcessEnquiryAsync(It.IsAny<EmailEnquiryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var response = await _client.PostAsJsonAsync("/api/v1/email", new EmailEnquiryRequest
        {
            Name = "Alice",
            EmailAddress = "alice@example.com",
            Message = "Is the property available in September?",
            ReCaptchaToken = "valid-token"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        body!.Success.Should().BeTrue();
    }
}
