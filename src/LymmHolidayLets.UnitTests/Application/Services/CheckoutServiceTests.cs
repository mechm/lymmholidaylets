using FluentAssertions;
using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Interface.Service;
using AppCheckoutService = LymmHolidayLets.Application.Service.CheckoutService;
using LymmHolidayLets.Domain.ReadModel.Checkout;
using Microsoft.Extensions.Logging;
using Moq;
using Stripe;
using Stripe.Checkout;
using Xunit;

namespace LymmHolidayLets.UnitTests.Application.Services;

public class CheckoutServiceTests
{
    private readonly Mock<ILogger<AppCheckoutService>> _logger = new();
    private readonly Mock<ICheckoutCommand> _checkoutCommand = new();
    private readonly Mock<ICheckoutQuery> _checkoutQuery = new();
    private readonly Mock<IStripeService> _stripeService = new();
    private readonly Mock<ICalculateService> _calculateService = new();

    private AppCheckoutService CreateSut() => new(
        _logger.Object,
        _checkoutCommand.Object,
        _checkoutQuery.Object,
        _stripeService.Object,
        _calculateService.Object);

    [Fact]
    public void Checkout_WhenNoPropertyAvailable_ReturnsError()
    {
        _checkoutQuery
            .Setup(q => q.GetByPropertyIdAndDate(It.IsAny<byte>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), true))
            .Returns((CheckoutAggregate?)null);

        var (error, result) = CreateSut().Checkout(
            "https://example.com", 1,
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 8),
            2, 0, 0);

        error.Should().NotBeNull();
        result.Should().BeNull();
    }

    [Fact]
    public void Checkout_WhenNoPriceAvailable_ReturnsError()
    {
        _checkoutQuery
            .Setup(q => q.GetByPropertyIdAndDate(It.IsAny<byte>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), true))
            .Returns(new CheckoutAggregate(
                new Domain.ReadModel.Checkout.Property { FriendlyName = "Test" },
                null,
                [],
                [],
                null));

        var (error, result) = CreateSut().Checkout(
            "https://example.com", 1,
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 8),
            2, 0, 0);

        error.Should().NotBeNull();
        result.Should().BeNull();
    }

    [Fact]
    public void Checkout_WhenStripeSessionReturnsNull_ReturnsError()
    {
        SetupValidCheckoutAggregate();
        _stripeService
            .Setup(s => s.CreateProductAndCoupon(
                It.IsAny<Domain.ReadModel.Checkout.Checkout?>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<decimal>(), It.IsAny<decimal?>()))
            .Returns((new Stripe.Product { Id = "prod_1", DefaultPriceId = "price_1" }, null));
        _stripeService
            .Setup(s => s.CreateSession(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string?>(), It.IsAny<IEnumerable<PropertyAdditionalProduct>>(),
                It.IsAny<short>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
                It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<short?>()))
            .Returns((Session?)null);

        var (error, result) = CreateSut().Checkout(
            "https://example.com", 1,
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 8),
            2, 0, 0);

        error.Should().NotBeNull();
        result.Should().BeNull();
    }

    [Fact]
    public void Checkout_WhenSuccess_ReturnsSessionResult()
    {
        SetupValidCheckoutAggregate();
        SetupStripeSuccess();

        var (error, result) = CreateSut().Checkout(
            "https://example.com", 1,
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 8),
            2, 0, 0);

        error.Should().BeNull();
        result.Should().NotBeNull();
        result!.SessionId.Should().Be("cs_test_123");
        result.SessionUrl.Should().Be("https://checkout.stripe.com/pay/cs_test_123");
    }

    [Fact]
    public void Checkout_WhenSuccess_PersistsCheckout()
    {
        SetupValidCheckoutAggregate();
        SetupStripeSuccess();

        CreateSut().Checkout(
            "https://example.com", 1,
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 8),
            2, 0, 0);

        _checkoutCommand.Verify(
            c => c.Upsert(It.IsAny<LymmHolidayLets.Application.Model.Command.Checkout>()),
            Times.Once);
    }

    private void SetupValidCheckoutAggregate()
    {
        _checkoutQuery
            .Setup(q => q.GetByPropertyIdAndDate(It.IsAny<byte>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), true))
            .Returns(new CheckoutAggregate(
                new Domain.ReadModel.Checkout.Property { FriendlyName = "Lymm Holiday Let" },
                100m,
                [],
                [],
                null));
        _calculateService
            .Setup(c => c.CalculateApplicableDiscountPercentage(
                It.IsAny<IEnumerable<PropertyNightCoupon>>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
            .Returns((null, 7));
    }

    private void SetupStripeSuccess()
    {
        _stripeService
            .Setup(s => s.CreateProductAndCoupon(
                It.IsAny<Domain.ReadModel.Checkout.Checkout?>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<decimal>(), It.IsAny<decimal?>()))
            .Returns((new Stripe.Product { Id = "prod_1", DefaultPriceId = "price_1" }, null));
        _stripeService
            .Setup(s => s.CreateSession(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string?>(), It.IsAny<IEnumerable<PropertyAdditionalProduct>>(),
                It.IsAny<short>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
                It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<short?>()))
            .Returns(new Session { Id = "cs_test_123", Url = "https://checkout.stripe.com/pay/cs_test_123" });
    }
}
