using FluentAssertions;
using LymmHolidayLets.Application.Command;
using LymmHolidayLets.Domain.Model.WebhookEvent.Entity;
using LymmHolidayLets.Domain.Repository;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Application.Command;

public class WebhookEventCommandTests
{
    private readonly Mock<IWebhookEventRepository> _repository = new();

    private WebhookEventCommand CreateSut() => new(_repository.Object);

    [Fact]
    public void Create_PersistsWebhookEvent()
    {
        var webhookEvent = new WebhookEvent("evt_123", "{}", Domain.Model.WebhookEvent.Enum.WebhookEventState.Pending);

        CreateSut().Create(webhookEvent);

        _repository.Verify(r => r.Create(webhookEvent), Times.Once);
    }

    [Fact]
    public void Save_PersistsWebhookEvent()
    {
        var webhookEvent = new WebhookEvent("evt_123", "{}", Domain.Model.WebhookEvent.Enum.WebhookEventState.Processing);

        CreateSut().Save(webhookEvent);

        _repository.Verify(r => r.Update(webhookEvent), Times.Once);
    }
}
