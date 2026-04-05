using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Models.Email;
using LymmHolidayLets.IntegrationTests.Infrastructure;
using Moq;
using Xunit;

namespace LymmHolidayLets.IntegrationTests.Controllers;

public class EmailEndpointTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static EmailEnquiryRequest ValidRequest() => new()
    {
        Name = "Jane Smith",
        EmailAddress = "jane@example.com",
        Message = "I'd like to enquire about availability.",
        ReCaptchaToken = "valid-token"
    };

    [Fact]
    public async Task Post_Email_WhenSuccess_Returns200()
    {
        factory.RecaptchaValidationService
            .Setup(r => r.ValidateAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        factory.EmailEnquiryService
            .Setup(s => s.ProcessEnquiryAsync(It.IsAny<EmailEnquiryRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await _client.PostAsJsonAsync("/api/v1/email", ValidRequest());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Post_Email_WhenRecaptchaFails_Returns400()
    {
        factory.RecaptchaValidationService
            .Setup(r => r.ValidateAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var response = await _client.PostAsJsonAsync("/api/v1/email", ValidRequest());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_Email_WhenServiceThrows_Returns500()
    {
        factory.RecaptchaValidationService
            .Setup(r => r.ValidateAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        factory.EmailEnquiryService
            .Setup(s => s.ProcessEnquiryAsync(It.IsAny<EmailEnquiryRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var response = await _client.PostAsJsonAsync("/api/v1/email", ValidRequest());

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        body!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Post_Email_WithMissingRequiredFields_Returns400()
    {
        // Name is required — omit it to trigger model validation failure
        var invalidRequest = new { Message = "Hello" };

        var response = await _client.PostAsJsonAsync("/api/v1/email", invalidRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
