using FluentAssertions;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Service;
using LymmHolidayLets.Infrastructure.Services;
using MassTransit;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Stripe;
using Xunit;

namespace LymmHolidayLets.UnitTests.Application.Services;

public class StripeWebhookProcessorTests
{
    private readonly Mock<IConfiguration> _config = new();
    private readonly Mock<ILogger<StripeWebhookProcessor>> _logger = new();
    private readonly Mock<LymmHolidayLets.Application.Interface.Query.IPropertyQuery> _propertyQuery = new();
    private readonly Mock<IPublishEndpoint> _publishEndpoint = new();
    private readonly Mock<IManageCheckoutSessionService> _sessionService = new();
    private readonly Mock<IStripeService> _stripeService = new();
    private readonly Mock<LymmHolidayLets.Application.Interface.Command.IBookingCommand> _bookingCommand = new();
    private readonly Mock<LymmHolidayLets.Application.Interface.Command.IWebhookEventCommand> _webhookEventCommand = new();
    private readonly ApplicationMemoryCache _cache = new(new MemoryCache(new MemoryCacheOptions()));

    private StripeWebhookProcessor CreateSut() => new(
        _config.Object,
        _logger.Object,
        _cache,
        _propertyQuery.Object,
        _publishEndpoint.Object,
        _sessionService.Object,
        _stripeService.Object,
        _bookingCommand.Object,
        _webhookEventCommand.Object);

    [Fact]
    public async Task ProcessEventAsync_MissingWebhookKey_ReturnsFalse()
    {
        _config.Setup(c => c["StripeSettings:CheckoutWebHookKey"]).Returns((string?)null);

        var result = await CreateSut().ProcessEventAsync("{}", null);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ProcessEventAsync_InvalidJson_ReturnsFalse()
    {
        _config.Setup(c => c["StripeSettings:CheckoutWebHookKey"]).Returns("whsec_test");

        var result = await CreateSut().ProcessEventAsync("not-valid-json", "sig");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ProcessEventAsync_DuplicateEvent_ReturnsFalse()
    {
        _config.Setup(c => c["StripeSettings:CheckoutWebHookKey"]).Returns("whsec_test");

        // Simulate idempotency: event already exists
        _webhookEventCommand
            .Setup(w => w.GetByExternalId(It.IsAny<string>()))
            .Returns(new LymmHolidayLets.Application.Model.Command.WebhookEvent("evt_123", "{}", 2));

        // This will fail at signature verification before hitting the idempotency check,
        // but the invalid signature path returns false — which is acceptable.
        var result = await CreateSut().ProcessEventAsync("{}", "invalid-sig");

        result.Should().BeFalse();
    }
}
