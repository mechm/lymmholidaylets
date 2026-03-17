using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Models.Checkout;
using LymmHolidayLets.Application.Model.Service;
using LymmHolidayLets.IntegrationTests.Infrastructure;
using Moq;
using Xunit;

namespace LymmHolidayLets.IntegrationTests.Controllers;

public class CheckoutEndpointTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static CheckoutItemForm ValidForm() => new()
    {
        PropertyId = 1,
        Checkin = new DateOnly(2026, 6, 1),
        Checkout = new DateOnly(2026, 6, 8),
        NumberOfAdults = 2
    };

    [Fact]
    public async Task Post_CreateCheckoutSession_WhenSuccess_Returns200WithUrl()
    {
        factory.CheckoutService
            .Setup(s => s.CheckoutAsync(
                It.IsAny<byte>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
                It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CheckoutResponse.Success(new CheckoutResult
            {
                SessionId = "cs_test_123",
                SessionUrl = "https://checkout.stripe.com/pay/cs_test_123",
                CheckIn = new DateOnly(2026, 6, 1),
                CheckOut = new DateOnly(2026, 6, 8)
            }));

        var response = await _client.PostAsJsonAsync("/api/v1/checkout/create-checkout-session", ValidForm());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Post_CreateCheckoutSession_WhenPropertyUnavailable_Returns400()
    {
        factory.CheckoutService
            .Setup(s => s.CheckoutAsync(
                It.IsAny<byte>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
                It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CheckoutResponse.Failure("Property 1 was not found."));

        var response = await _client.PostAsJsonAsync("/api/v1/checkout/create-checkout-session", ValidForm());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_CreateCheckoutSession_InvalidBody_Returns400()
    {
        // Send a null/empty body to trigger model binding failure
        var response = await _client.PostAsJsonAsync("/api/v1/checkout/create-checkout-session", (CheckoutItemForm?)null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
