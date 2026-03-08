using FluentAssertions;
using LymmHolidayLets.Api.Controllers;
using LymmHolidayLets.Api.Models;
using LymmHolidayLets.Api.Models.Checkout;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.Api.Controllers;

public class CheckoutControllerTests
{
    private readonly Mock<ICheckoutService> _checkoutService = new();
    private readonly Mock<IManageCheckoutSessionService> _sessionService = new();
    private readonly Mock<ILogger<CheckoutController>> _logger = new();
    private readonly CheckoutController _sut;

    public CheckoutControllerTests()
    {
        _sut = new CheckoutController(_checkoutService.Object, _sessionService.Object, _logger.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }

    private static CheckoutItemForm ValidForm() => new()
    {
        PropertyId = 1,
        Checkin = new DateOnly(2026, 6, 1),
        Checkout = new DateOnly(2026, 6, 8),
        NumberOfAdults = 2
    };

    private CheckoutResult ValidResult() => new()
    {
        SessionId = "cs_test_123",
        SessionUrl = "https://checkout.stripe.com/pay/cs_test_123",
        CheckIn = new DateOnly(2026, 6, 1),
        CheckOut = new DateOnly(2026, 6, 8)
    };

    [Fact]
    public void Create_WhenServiceReturnsError_ReturnsBadRequest()
    {
        _checkoutService
            .Setup(s => s.Checkout(
                It.IsAny<string>(), It.IsAny<byte>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
                It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<short?>()))
            .Returns(("No Property Available", null));

        var result = _sut.Create(ValidForm());

        var bad = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var body = bad.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        body.Success.Should().BeFalse();
        body.Message.Should().Be("No Property Available");
    }

    [Fact]
    public void Create_WhenServiceReturnsNullResult_ReturnsBadRequest()
    {
        _checkoutService
            .Setup(s => s.Checkout(
                It.IsAny<string>(), It.IsAny<byte>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
                It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<short?>()))
            .Returns((null, (CheckoutResult?)null));

        var result = _sut.Create(ValidForm());

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Create_WhenSuccess_ReturnsOkWithStripeUrl()
    {
        _checkoutService
            .Setup(s => s.Checkout(
                It.IsAny<string>(), It.IsAny<byte>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
                It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<short?>()))
            .Returns((null, ValidResult()));

        var result = _sut.Create(ValidForm());

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        body.Success.Should().BeTrue();
    }

    [Fact]
    public void Create_WhenSuccess_UpdatesSessionCache()
    {
        _checkoutService
            .Setup(s => s.Checkout(
                It.IsAny<string>(), It.IsAny<byte>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
                It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<short?>()))
            .Returns((null, ValidResult()));

        _sut.Create(ValidForm());

        _sessionService.Verify(
            s => s.AddUpdateSessionCache(
                "cs_test_123",
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 6, 8)),
            Times.Once);
    }
}
