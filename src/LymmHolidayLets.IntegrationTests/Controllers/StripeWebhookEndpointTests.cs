using System.Net;
using System.Text;
using FluentAssertions;
using LymmHolidayLets.IntegrationTests.Infrastructure;
using Moq;
using Xunit;

namespace LymmHolidayLets.IntegrationTests.Controllers;

public class StripeWebhookEndpointTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static StringContent JsonBody(string json = "{}") =>
        new(json, Encoding.UTF8, "application/json");

    [Fact]
    public async Task Post_Webhook_WhenProcessorSucceeds_Returns200()
    {
        factory.StripeWebhookProcessor
            .Setup(p => p.ProcessEventAsync(It.IsAny<string>(), It.IsAny<string?>()))
            .ReturnsAsync(true);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/stripewebhook")
        {
            Content = JsonBody()
        };
        request.Headers.Add("Stripe-Signature", "t=1234,v1=abc");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Post_Webhook_WhenProcessorFails_Returns400()
    {
        factory.StripeWebhookProcessor
            .Setup(p => p.ProcessEventAsync(It.IsAny<string>(), It.IsAny<string?>()))
            .ReturnsAsync(false);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/stripewebhook")
        {
            Content = JsonBody()
        };
        request.Headers.Add("Stripe-Signature", "t=1234,v1=abc");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_Webhook_WhenStripeSignatureHeaderMissing_Returns400()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/stripewebhook")
        {
            Content = JsonBody()
        };
        // Deliberately omit Stripe-Signature header

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_Webhook_WhenBodyIsEmpty_Returns400()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/stripewebhook")
        {
            Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Stripe-Signature", "t=1234,v1=abc");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
