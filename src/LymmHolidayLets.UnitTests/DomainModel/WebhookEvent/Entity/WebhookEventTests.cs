using FluentAssertions;
using LymmHolidayLets.Domain.Model.WebhookEvent.Enum;
using DomainWebhookEvent = LymmHolidayLets.Domain.Model.WebhookEvent.Entity.WebhookEvent;
using Xunit;

namespace LymmHolidayLets.UnitTests.DomainModel.WebhookEvent.Entity;

public class WebhookEventTests
{
    [Fact]
    public void MarkAsProcessing_WhenPending_TransitionsToProcessing()
    {
        var now = new DateTime(2026, 4, 11, 12, 0, 0, DateTimeKind.Utc);
        var webhookEvent = new DomainWebhookEvent("evt_123", "{}", WebhookEventState.Pending);

        webhookEvent.MarkAsProcessing(now);

        webhookEvent.State.Should().Be(WebhookEventState.Processing);
        webhookEvent.UpdatedAt.Should().Be(now);
    }

    [Fact]
    public void MarkAsProcessing_WhenFailed_TransitionsBackToProcessingAndClearsError()
    {
        var now = new DateTime(2026, 4, 11, 12, 5, 0, DateTimeKind.Utc);
        var webhookEvent = new DomainWebhookEvent("evt_123", "{}", WebhookEventState.Processing);
        webhookEvent.MarkAsFailed("boom", now.AddMinutes(-5));

        webhookEvent.MarkAsProcessing(now);

        webhookEvent.State.Should().Be(WebhookEventState.Processing);
        webhookEvent.ProcessingErrors.Should().BeNull();
        webhookEvent.UpdatedAt.Should().Be(now);
    }

    [Fact]
    public void MarkAsProcessed_WhenProcessing_TransitionsToProcessed()
    {
        var now = new DateTime(2026, 4, 11, 13, 0, 0, DateTimeKind.Utc);
        var webhookEvent = new DomainWebhookEvent("evt_123", "{}", WebhookEventState.Pending);
        webhookEvent.MarkAsProcessing(now.AddMinutes(-5));

        webhookEvent.MarkAsProcessed(now);

        webhookEvent.State.Should().Be(WebhookEventState.Processed);
        webhookEvent.UpdatedAt.Should().Be(now);
    }

    [Fact]
    public void MarkAsProcessed_WhenNotProcessing_Throws()
    {
        var webhookEvent = new DomainWebhookEvent("evt_123", "{}", WebhookEventState.Pending);

        var action = () => webhookEvent.MarkAsProcessed();

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void MarkAsFailed_SetsStateAndError()
    {
        var now = new DateTime(2026, 4, 11, 14, 0, 0, DateTimeKind.Utc);
        var webhookEvent = new DomainWebhookEvent("evt_123", "{}", WebhookEventState.Processing);

        webhookEvent.MarkAsFailed("boom", now);

        webhookEvent.State.Should().Be(WebhookEventState.Failed);
        webhookEvent.ProcessingErrors.Should().Be("boom");
        webhookEvent.UpdatedAt.Should().Be(now);
    }
}
