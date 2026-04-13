using FluentAssertions;
using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Interface.Service;
using AppCheckoutService = LymmHolidayLets.Application.Service.CheckoutService;
using LymmHolidayLets.Application.Model.Service;
using LymmHolidayLets.Domain.ReadModel.Checkout;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Application.Services;

public class CheckoutServiceTests
{
    private readonly Mock<ILogger<AppCheckoutService>> _logger = new();
    private readonly Mock<IOptions<CheckoutOptions>> _options = new();
    private readonly Mock<ICheckoutCommand> _checkoutCommand = new();
    private readonly Mock<ICheckoutQuery> _checkoutQuery = new();
    private readonly Mock<IStripeService> _stripeService = new();
    private readonly Mock<IManageCheckoutSessionService> _sessionService = new();

    public CheckoutServiceTests()
    {
        _options.Setup(o => o.Value).Returns(new CheckoutOptions { BaseUrl = "https://example.com" });
    }

    private AppCheckoutService CreateSut() => new(
        _logger.Object,
        _options.Object,
        _checkoutCommand.Object,
        _checkoutQuery.Object,
        _stripeService.Object,
        _sessionService.Object);

    [Fact]
    public async Task CheckoutAsync_WhenNoPropertyAvailable_ReturnsError()
    {
        _checkoutQuery
            .Setup(q => q.GetByPropertyIdAndDate(It.IsAny<byte>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
            .Returns(new CheckoutLookupResult.PropertyNotFound());

        var response = await CreateSut().CheckoutAsync(
            1,
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 8),
            2, 0, 0);

        response.IsSuccess.Should().BeFalse();
        response.Error.Should().Contain("was not found");
        response.Result.Should().BeNull();
    }

    [Fact]
    public async Task CheckoutAsync_WhenNoPriceAvailable_ReturnsError()
    {
        _checkoutQuery
            .Setup(q => q.GetByPropertyIdAndDate(It.IsAny<byte>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
            .Returns(new CheckoutLookupResult.DatesUnavailable("Test"));

        var response = await CreateSut().CheckoutAsync(
            1,
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 8),
            2, 0, 0);

        response.IsSuccess.Should().BeFalse();
        response.Error.Should().Contain("No availability");
        response.Result.Should().BeNull();
    }

    [Fact]
    public async Task CheckoutAsync_WhenStripeSessionReturnsNull_ReturnsError()
    {
        SetupValidCheckoutAggregate();
        _stripeService
            .Setup(s => s.CreateProductAndCouponAsync(
                It.IsAny<Domain.ReadModel.Checkout.Checkout?>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<decimal>(), It.IsAny<decimal?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StripeProductAndCouponResult
            {
                Product = new StripeProductResult { Id = "prod_1", DefaultPriceId = "price_1" }
            });
        _stripeService
            .Setup(s => s.CreateSessionAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string?>(), It.IsAny<IEnumerable<PropertyAdditionalProduct>>(),
                It.IsAny<short>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
                It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StripeSessionResult?)null);

        var response = await CreateSut().CheckoutAsync(
            1,
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 8),
            2, 0, 0);

        response.IsSuccess.Should().BeFalse();
        response.Error.Should().NotBeNullOrEmpty();
        response.Result.Should().BeNull();
    }

    [Fact]
    public async Task CheckoutAsync_WhenSuccess_ReturnsSessionResult()
    {
        SetupValidCheckoutAggregate();
        SetupStripeSuccess();

        var response = await CreateSut().CheckoutAsync(
            1,
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 8),
            2, 0, 0);

        response.IsSuccess.Should().BeTrue();
        response.Error.Should().BeNull();
        response.Result.Should().NotBeNull();
        response.Result!.SessionId.Should().Be("cs_test_123");
        response.Result.SessionUrl.Should().Be("https://checkout.stripe.com/pay/cs_test_123");
    }

    [Fact]
    public async Task CheckoutAsync_WhenSuccess_PersistsCheckout()
    {
        SetupValidCheckoutAggregate();
        SetupStripeSuccess();

        await CreateSut().CheckoutAsync(
            1,
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 8),
            2, 0, 0);

        _checkoutCommand.Verify(
            c => c.UpsertAsync(It.IsAny<LymmHolidayLets.Application.Model.Command.Checkout>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CheckoutAsync_WhenSuccess_UpdatesSessionCache()
    {
        SetupValidCheckoutAggregate();
        SetupStripeSuccess();

        await CreateSut().CheckoutAsync(
            1,
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 8),
            2, 0, 0);

        _sessionService.Verify(
            s => s.AddUpdateSessionCache(
                "cs_test_123",
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 6, 8)),
            Times.Once);
    }

    private void SetupValidCheckoutAggregate()
    {
        _checkoutQuery
            .Setup(q => q.GetByPropertyIdAndDate(It.IsAny<byte>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
            .Returns(new CheckoutLookupResult.Available(new CheckoutAggregate(
                new Domain.ReadModel.Checkout.Property { FriendlyName = "Lymm Holiday Let" },
                100m,
                [],
                [],
                null)));
    }

    private void SetupStripeSuccess()
    {
        _stripeService
            .Setup(s => s.CreateProductAndCouponAsync(
                It.IsAny<Domain.ReadModel.Checkout.Checkout?>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<decimal>(), It.IsAny<decimal?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StripeProductAndCouponResult
            {
                Product = new StripeProductResult { Id = "prod_1", DefaultPriceId = "price_1" }
            });
        _stripeService
            .Setup(s => s.CreateSessionAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string?>(), It.IsAny<IEnumerable<PropertyAdditionalProduct>>(),
                It.IsAny<short>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
                It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StripeSessionResult { Id = "cs_test_123", Url = "https://checkout.stripe.com/pay/cs_test_123" });
    }
}
