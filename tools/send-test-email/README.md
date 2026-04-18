# SendTestEmail

## Description

`SendTestEmail` is a small .NET 10 command-line utility for publishing test email messages to RabbitMQ in the LymmHolidayLets environment. It supports both booking confirmation messages and guest pre-arrival email messages so you can exercise the `LymmHolidayLets.NotificationWorker` without waiting for a real booking flow or scheduler run.

## Installation

### Prerequisites

- .NET 10 SDK
- RabbitMQ running locally or reachable from your machine
- SQL Server access for the `pre-arrival` command

### Restore dependencies

From the repository root:

```sh
dotnet restore tools\send-test-email\SendTestEmail.csproj
```

The tool depends on:

- `LymmHolidayLets.Contracts`
- `MassTransit.RabbitMQ`
- `Dapper`
- `Microsoft.Data.SqlClient`

## Usage

### Show help

```sh
dotnet run --project tools\send-test-email -- --help
```

### Publish a booking confirmation test message

```sh
dotnet run --project tools\send-test-email -- confirmation --to you@example.com
```

If no `--to` value is provided, the tool uses its built-in sample recipient.

### Publish a guest pre-arrival email test message

```sh
dotnet run --project tools\send-test-email -- pre-arrival --booking-id 123 --to you@example.com --connection-string "Server=localhost,1433;Database=LymmHolidayLets;User Id=sa;Password=YourStrong!Password123;TrustServerCertificate=True;Encrypt=False;"
```

For `pre-arrival`, the tool:

1. Loads the booking with `Booking_GetByID`
2. Loads the property pre-arrival template configuration with `Property_GuestPreArrivalEmail_GetByID`
3. Builds a `GuestPreArrivalEmailRequested` message
4. Publishes it to RabbitMQ for `LymmHolidayLets.NotificationWorker` to consume

### Optional connection settings

You can provide these as command-line options:

```text
--rabbitmq-host <host>
--rabbitmq-username <username>
--rabbitmq-password <password>
--connection-string "<sql connection string>"
```

Or via environment variables:

```text
RabbitMQ__Host
RabbitMQ__Username
RabbitMQ__Password
ConnectionStrings__LymmHolidayLets
```

## Features

- Publishes `BookingNotificationRequested` test messages
- Publishes `GuestPreArrivalEmailRequested` messages from real booking data
- Supports recipient overrides with `--to`
- Uses the same RabbitMQ contract flow as the application workers
- Reuses the live database configuration for pre-arrival email tests

## Contributing

- Keep this tool lightweight and focused on manual message publishing
- Prefer reusing existing contracts and stored procedures over duplicating business logic
- Preserve compatibility with the NotificationWorker and scheduler message flow
- Build the tool after changes:

```sh
dotnet build tools\send-test-email\SendTestEmail.csproj
```

## License

MIT License
