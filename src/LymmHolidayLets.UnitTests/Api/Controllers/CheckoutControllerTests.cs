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

    private void SetupCheckoutFailure(string error) =>
        _checkoutService
            .Setup(s => s.CheckoutAsync(
                It.IsAny<byte>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
                It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CheckoutResponse.Failure(error));

    private void SetupCheckoutSuccess() =>
        _checkoutService
            .Setup(s => s.CheckoutAsync(
                It.IsAny<byte>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
                It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<short?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CheckoutResponse.Success(ValidResult()));

    [Fact]
    public async Task Create_WhenServiceReturnsError_ReturnsBadRequest()
    {
        SetupCheckoutFailure("Property is not available for the selected dates");

        var result = await _sut.Create(ValidForm(), CancellationToken.None);

        var bad = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var body = bad.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        body.Success.Should().BeFalse();
        body.Message.Should().Be("Property is not available for the selected dates");
    }

    [Fact]
    public async Task Create_WhenSuccess_ReturnsOkWithStripeUrl()
    {
        SetupCheckoutSuccess();

        var result = await _sut.Create(ValidForm(), CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<ApiResponse<CheckoutSessionResponse>>().Subject;
        body.Success.Should().BeTrue();
        body.Data!.Url.Should().Be("https://checkout.stripe.com/pay/cs_test_123");
    }

    [Fact]
    public async Task Create_WhenSuccess_UpdatesSessionCache()
    {
        SetupCheckoutSuccess();

        await _sut.Create(ValidForm(), CancellationToken.None);

        _sessionService.Verify(
            s => s.AddUpdateSessionCache(
                "cs_test_123",
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 6, 8)),
            Times.Once);
    }
}
