using FluentAssertions;
using LymmHolidayLets.Api.Controllers;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Application.Interface.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Api.Controllers;

public class StripeWebHookControllerTests
{
    private readonly Mock<IStripeWebhookProcessor> _webhookProcessor = new();
    private readonly StripeWebHookController _sut;

    public StripeWebHookControllerTests()
    {
        _sut = new StripeWebHookController(_webhookProcessor.Object);
    }

    [Fact]
    public async Task Index_WhenProcessingFails_ReturnsBadRequest()
    {
        _webhookProcessor
            .Setup(p => p.ProcessEventAsync(It.IsAny<string>(), It.IsAny<string?>()))
            .ReturnsAsync(false);

        var request = new StripeWebhookRequest { Json = "{}", Signature = "invalid-sig" };

        var result = await _sut.Index(request);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Index_WhenProcessingSucceeds_ReturnsOk()
    {
        _webhookProcessor
            .Setup(p => p.ProcessEventAsync(It.IsAny<string>(), It.IsAny<string?>()))
            .ReturnsAsync(true);

        var request = new StripeWebhookRequest { Json = "{}", Signature = "whsec_valid" };

        var result = await _sut.Index(request);

        result.Should().BeOfType<OkResult>();
    }
}
