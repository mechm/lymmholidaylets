using LymmHolidayLets.Contracts;
using MassTransit;

Console.WriteLine("Publishing BookingNotificationRequested to RabbitMQ...");

var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("localhost", "/", h =>
    {
        h.Username("guest");
        h.Password("guest");
    });
});

await busControl.StartAsync();

try
{
    var message = new BookingNotificationRequested(
        PropertyName:    "Lymm Village Apartment",
        CheckIn:         new DateOnly(2026, 5, 10),
        CheckOut:        new DateOnly(2026, 5, 14),
        NoAdult:         2,
        NoChildren:      1,
        NoInfant:        0,
        Name:            "Matthew Chambers",
        Email:           "matthew@lymmholidaylets.com",
        Telephone:       "+44 7700 900000",
        PostalCode:      "WA13 0AB",
        Country:         "England",
        AmountTotal:     48000,   // £480.00 in pence
        PropertyId:      1,
        SmsRecipients:   [],
        BookingReference: "cs_test_TESTBOOKING001"
    );

    await busControl.Publish(message);

    Console.WriteLine("✓ Message published. Check the notification worker logs and your inbox at matthew@lymmholidaylets.com");
}
finally
{
    await busControl.StopAsync();
}
