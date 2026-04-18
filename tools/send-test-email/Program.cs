using System.Data;
using Dapper;
using LymmHolidayLets.Contracts;
using MassTransit;
using Microsoft.Data.SqlClient;

return await SendTestEmailApp.RunAsync(args);

internal static class SendTestEmailApp
{
    public static async Task<int> RunAsync(string[] args)
    {
        if (HasOption(args, "--help", "-h"))
        {
            PrintUsage();
            return 0;
        }

        var command = GetCommand(args);
        var rabbitMqHost = GetOption(args, "--rabbitmq-host") ?? Environment.GetEnvironmentVariable("RabbitMQ__Host") ?? "localhost";
        var rabbitMqUsername = GetOption(args, "--rabbitmq-username") ?? Environment.GetEnvironmentVariable("RabbitMQ__Username") ?? "guest";
        var rabbitMqPassword = GetOption(args, "--rabbitmq-password") ?? Environment.GetEnvironmentVariable("RabbitMQ__Password") ?? "guest";

        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host(rabbitMqHost, "/", h =>
            {
                h.Username(rabbitMqUsername);
                h.Password(rabbitMqPassword);
            });
        });

        await busControl.StartAsync();

        try
        {
            switch (command)
            {
                case "confirmation":
                    await PublishConfirmationAsync(busControl, GetOption(args, "--to"));
                    return 0;
                case "pre-arrival":
                case "prearrival":
                    return await PublishPreArrivalAsync(busControl, args);
                default:
                    await Console.Error.WriteLineAsync($"Unknown command '{command}'.");
                    PrintUsage();
                    return 1;
            }
        }
        finally
        {
            await busControl.StopAsync();
        }
    }

    private static async Task PublishConfirmationAsync(IBusControl busControl, string? overrideEmail)
    {
        var recipient = overrideEmail ?? "matthew@lymmholidaylets.com";

        Console.WriteLine("Publishing BookingNotificationRequested to RabbitMQ...");

        var message = new BookingNotificationRequested(
            PropertyName: "Lymm Village Apartment",
            CheckIn: new DateOnly(2026, 5, 10),
            CheckOut: new DateOnly(2026, 5, 14),
            NoAdult: 2,
            NoChildren: 1,
            NoInfant: 0,
            Name: "Matthew Chambers",
            Email: recipient,
            Telephone: "+44 7700 900000",
            PostalCode: "WA13 0AB",
            Country: "England",
            AmountTotal: 48000,
            PropertyId: 1,
            BookingReference: "cs_test_TESTBOOKING001");

        await busControl.Publish(message);

        Console.WriteLine($"✓ Confirmation message published for {recipient}");
    }

    private static async Task<int> PublishPreArrivalAsync(IBusControl busControl, string[] args)
    {
        if (!int.TryParse(GetOption(args, "--booking-id"), out var bookingId))
        {
            await Console.Error.WriteLineAsync("The pre-arrival command requires --booking-id <id>.");
            PrintUsage();
            return 1;
        }

        var connectionString = GetOption(args, "--connection-string")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__LymmHolidayLets");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            await Console.Error.WriteLineAsync("The pre-arrival command requires a SQL connection string via --connection-string or ConnectionStrings__LymmHolidayLets.");
            return 1;
        }

        var overrideEmail = GetOption(args, "--to");
        var message = await LoadPreArrivalMessageAsync(bookingId, overrideEmail, connectionString);

        Console.WriteLine($"Publishing GuestPreArrivalEmailRequested for booking {bookingId}...");
        await busControl.Publish(message);
        Console.WriteLine($"✓ Pre-arrival message published for {message.GuestEmail}");

        return 0;
    }

    private static async Task<GuestPreArrivalEmailRequested> LoadPreArrivalMessageAsync(
        int bookingId,
        string? overrideEmail,
        string connectionString)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var booking = await connection.QuerySingleOrDefaultAsync<BookingRow>(
            "Booking_GetByID",
            new { ID = bookingId },
            commandType: CommandType.StoredProcedure);

        if (booking is null)
        {
            throw new InvalidOperationException($"Booking {bookingId} was not found.");
        }

        var property = await connection.QuerySingleOrDefaultAsync<PropertyGuestEmailConfigRow>(
            "Property_GuestPreArrivalEmail_GetByID",
            new { PropertyID = booking.PropertyID },
            commandType: CommandType.StoredProcedure);

        if (property is null)
        {
            throw new InvalidOperationException(
                $"Property {booking.PropertyID} does not have a pre-arrival email configuration.");
        }

        if (!property.IsEnabled)
        {
            throw new InvalidOperationException(
                $"Pre-arrival email scheduling is disabled for property {booking.PropertyID}.");
        }

        var recipient = overrideEmail ?? booking.Email;

        if (string.IsNullOrWhiteSpace(recipient))
        {
            throw new InvalidOperationException($"Booking {bookingId} does not have an email address.");
        }

        var checkIn = DateOnly.FromDateTime(booking.CheckIn);
        var checkOut = DateOnly.FromDateTime(booking.CheckOut);

        return new GuestPreArrivalEmailRequested(
            BookingId: booking.ID,
            BookingReference: booking.SessionID,
            PropertyId: booking.PropertyID,
            PropertyName: property.PropertyName,
            CheckIn: checkIn,
            CheckOut: checkOut,
            NoAdult: booking.NoAdult,
            NoChildren: booking.NoChildren,
            NoInfant: booking.NoInfant,
            GuestName: booking.Name,
            GuestEmail: recipient,
            GuestTelephone: booking.Telephone,
            GuestPostalCode: booking.PostalCode,
            GuestCountry: booking.Country,
            AmountTotal: booking.Total,
            ScheduledSendDate: checkIn.AddDays(-property.SendDaysBeforeCheckIn));
    }

    private static string GetCommand(string[] args)
    {
        if (args.Length == 0 || args[0].StartsWith("--", StringComparison.Ordinal))
        {
            return "confirmation";
        }

        return args[0].Trim().ToLowerInvariant();
    }

    private static string? GetOption(string[] args, params string[] names)
    {
        for (var index = 0; index < args.Length; index++)
        {
            if (!names.Contains(args[index], StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            if (index + 1 >= args.Length || args[index + 1].StartsWith("--", StringComparison.Ordinal))
            {
                return null;
            }

            return args[index + 1];
        }

        return null;
    }

    private static bool HasOption(string[] args, params string[] names) =>
        args.Any(arg => names.Contains(arg, StringComparer.OrdinalIgnoreCase));

    private static void PrintUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine(@"  dotnet run --project tools\send-test-email -- [confirmation] [--to you@example.com]");
        Console.WriteLine(@"  dotnet run --project tools\send-test-email -- pre-arrival --booking-id 123 [--to you@example.com]");
        Console.WriteLine();
        Console.WriteLine("Optional connection settings:");
        Console.WriteLine("  --rabbitmq-host <host>");
        Console.WriteLine("  --rabbitmq-username <username>");
        Console.WriteLine("  --rabbitmq-password <password>");
        Console.WriteLine("  --connection-string \"<sql connection string>\"");
        Console.WriteLine();
        Console.WriteLine("Environment variable fallbacks:");
        Console.WriteLine("  RabbitMQ__Host");
        Console.WriteLine("  RabbitMQ__Username");
        Console.WriteLine("  RabbitMQ__Password");
        Console.WriteLine("  ConnectionStrings__LymmHolidayLets");
    }

    private sealed class BookingRow
    {
        public int ID { get; init; }
        public string? SessionID { get; init; }
        public byte PropertyID { get; init; }
        public DateTime CheckIn { get; init; }
        public DateTime CheckOut { get; init; }
        public byte? NoAdult { get; init; }
        public byte? NoChildren { get; init; }
        public byte? NoInfant { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Email { get; init; }
        public string? Telephone { get; init; }
        public string? PostalCode { get; init; }
        public string? Country { get; init; }
        public long? Total { get; init; }
    }

    private sealed class PropertyGuestEmailConfigRow
    {
        public string PropertyName { get; init; } = string.Empty;
        public bool IsEnabled { get; init; }
        public int SendDaysBeforeCheckIn { get; init; }
    }
}
