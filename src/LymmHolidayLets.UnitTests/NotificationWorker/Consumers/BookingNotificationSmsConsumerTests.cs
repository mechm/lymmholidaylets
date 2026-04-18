using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Service;
using LymmHolidayLets.Contracts;
using LymmHolidayLets.NotificationWorker.Consumers;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace LymmHolidayLets.UnitTests.NotificationWorker.Consumers;

public class BookingNotificationSmsConsumerTests
{
    [Fact]
    public async Task Consume_WhenSmsDisabled_DoesNotSendText()
    {
        var textMessageService = new Mock<ITextMessageService>();
        var consumer = CreateSut(
            textMessageService,
            Microsoft.Extensions.Options.Options.Create(new SmsNotificationOptions
            {
                Enabled = false,
                Recipients = ["+447966809226"]
            }));

        await consumer.Consume(CreateContext());

        textMessageService.Verify(
            service => service.SendText(It.IsAny<string>(), It.IsAny<string[]>()),
            Times.Never);
    }

    [Fact]
    public async Task Consume_WhenSmsEnabled_FormatsBookingSummaryForSms()
    {
        var textMessageService = new Mock<ITextMessageService>();
        var consumer = CreateSut(
            textMessageService,
            Microsoft.Extensions.Options.Options.Create(new SmsNotificationOptions
            {
                Enabled = true,
                Recipients = ["+447966809226", "+447989838440"]
            }));

        await consumer.Consume(CreateContext(amountTotal: 48000));

        textMessageService.Verify(
            service => service.SendText(
                It.Is<string>(message =>
                    message.Contains("New booking: Matthew Chambers booked Lymm Village Apartment (4 nights)") &&
                    message.Contains("10/05/2026-14/05/2026") &&
                    message.Contains("for £480") &&
                    message.Contains("2 adults, 1 child") &&
                    message.Contains("Ref: cs_test_TESTBOOKING001") &&
                    message.Contains("Tel: +44 7700 900000")),
                It.Is<string[]>(numbers => numbers.Length == 2 && numbers[0] == "+447966809226" && numbers[1] == "+447989838440")),
            Times.Once);
    }

    private static BookingNotificationSmsConsumer CreateSut(
        Mock<ITextMessageService> textMessageService,
        IOptions<SmsNotificationOptions> smsOptions) =>
        new(
            textMessageService.Object,
            smsOptions,
            new Mock<ILogger<BookingNotificationSmsConsumer>>().Object);

    private static ConsumeContext<BookingNotificationRequested> CreateContext(long? amountTotal = 48000)
    {
        var context = new Mock<ConsumeContext<BookingNotificationRequested>>();
        context.SetupGet(x => x.Message).Returns(new BookingNotificationRequested(
            PropertyName: "Lymm Village Apartment",
            CheckIn: new DateOnly(2026, 5, 10),
            CheckOut: new DateOnly(2026, 5, 14),
            NoAdult: 2,
            NoChildren: 1,
            NoInfant: 0,
            Name: "Matthew Chambers",
            Email: "matthew@lymmholidaylets.com",
            Telephone: "+44 7700 900000",
            PostalCode: "WA13 0AB",
            Country: "England",
            AmountTotal: amountTotal,
            PropertyId: 1,
            BookingReference: "cs_test_TESTBOOKING001"));

        return context.Object;
    }
}
