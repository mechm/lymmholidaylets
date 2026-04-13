using FluentAssertions;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Model.Service;
using LymmHolidayLets.Application.Service;
using LymmHolidayLets.Domain.Model.WebhookEvent.Entity;
using LymmHolidayLets.Domain.Model.WebhookEvent.Enum;
using LymmHolidayLets.Infrastructure.Services;
using MassTransit;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Application.Services;

public class StripeWebhookProcessorTests
{
    private readonly Mock<IConfiguration> _config = new();
    private readonly Mock<ILogger<StripeWebhookProcessor>> _logger = new();
    private readonly Mock<LymmHolidayLets.Application.Interface.Query.IPropertyQuery> _propertyQuery = new();
    private readonly Mock<IPublishEndpoint> _publishEndpoint = new();
    private readonly Mock<IManageCheckoutSessionService> _sessionService = new();
    private readonly Mock<IStripeWebhookEventParser> _webhookEventParser = new();
    private readonly Mock<IStripeService> _stripeService = new();
    private readonly Mock<LymmHolidayLets.Application.Interface.Command.IBookingCommand> _bookingCommand = new();
    private readonly Mock<IWebhookEventQuery> _webhookEventQuery = new();
    private readonly Mock<LymmHolidayLets.Application.Interface.Command.IWebhookEventCommand> _webhookEventCommand = new();
    private readonly ApplicationMemoryCache _cache = new(new MemoryCache(new MemoryCacheOptions()));

    private StripeWebhookProcessor CreateSut() => new(
        _config.Object,
        _logger.Object,
        _cache,
        _webhookEventParser.Object,
        _propertyQuery.Object,
        _publishEndpoint.Object,
        _sessionService.Object,
        _stripeService.Object,
        _bookingCommand.Object,
        _webhookEventQuery.Object,
        _webhookEventCommand.Object);

    [Fact]
    public async Task ProcessEventAsync_WhenParserThrows_ReturnsFalse()
    {
        _webhookEventParser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string?>()))
            .Throws(new System.Exception("invalid"));

        var result = await CreateSut().ProcessEventAsync("{}", null);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ProcessEventAsync_WhenParserReturnsNull_ReturnsFalse()
    {
        _webhookEventParser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string?>()))
            .Returns((ParsedStripeWebhookEvent?)null);

        var result = await CreateSut().ProcessEventAsync("not-valid-json", "sig");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ProcessEventAsync_DuplicateEvent_ReturnsFalse()
    {
        _webhookEventParser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string?>()))
            .Returns(new ParsedStripeWebhookEvent
            {
                EventId = "evt_123",
                EventType = "checkout.session.completed"
            });

        // Simulate idempotency: event already exists
        _webhookEventQuery
            .Setup(w => w.GetByExternalId(It.IsAny<string>()))
            .Returns(new WebhookEvent("evt_123", "{}", WebhookEventState.Processed));

        var result = await CreateSut().ProcessEventAsync("{}", "invalid-sig");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ProcessEventAsync_NewUnhandledEvent_CreatesAndSavesAggregate()
    {
        _webhookEventParser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string?>()))
            .Returns(new ParsedStripeWebhookEvent
            {
                EventId = "evt_123",
                EventType = "unknown.event"
            });

        _webhookEventQuery
            .Setup(q => q.GetByExternalId("evt_123"))
            .Returns((WebhookEvent?)null);

        var result = await CreateSut().ProcessEventAsync("{}", "sig");

        result.Should().BeTrue();
        _webhookEventCommand.Verify(c => c.Create(It.Is<WebhookEvent>(e => e.ExternalId == "evt_123")), Times.Once);
        _webhookEventCommand.Verify(c => c.Save(It.Is<WebhookEvent>(e => e.ExternalId == "evt_123" && e.State == WebhookEventState.Processed)), Times.Exactly(2));
    }
}
