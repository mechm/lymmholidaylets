# LymmHolidayLets.NotificationWorker

A .NET 10 background worker service that handles all outbound notifications for the LymmHolidayLets platform. It listens to a RabbitMQ message bus via [MassTransit](https://masstransit.io/) and dispatches email and SMS notifications in response to domain events.

## Overview

The NotificationWorker is a headless ASP.NET Core web application (no HTTP endpoints) that runs as a long-lived background process. It subscribes to events published by the main API and delivers notifications through two channels:

- **Email** — via [SendGrid](https://sendgrid.com/)
- **SMS** — via [Twilio](https://www.twilio.com/)

Each consumer is registered independently so that a failure in one notification type does not trigger retries in another, preventing duplicate sends.

## Features

- **Booking confirmation emails** — sends separate confirmation emails to the customer and to the company when a booking is confirmed
- **Booking SMS alerts** — sends an SMS summary to one or more configured recipients when a booking is made
- **Enquiry emails** — forwards contact form submissions to the company inbox, with optional CC recipients
- **Exponential back-off retry** — all consumers retry up to 3 times (2 s → 30 s) before dead-lettering the message
- **Structured logging** — Serilog with console sink; configurable via `appsettings.json`

## Consumers

| Consumer | Event | Channel |
|---|---|---|
| `BookingConfirmedToCustomerConsumer` | `BookingConfirmedEvent` | Email → Customer |
| `BookingConfirmedToCompanyConsumer` | `BookingConfirmedEvent` | Email → Company |
| `BookingNotificationSmsConsumer` | `BookingNotificationRequested` | SMS → Configured recipients |
| `EmailEnquiryConsumer` | `EmailEnquirySubmittedEvent` | Email → Company |

## Project Structure

```
LymmHolidayLets.NotificationWorker/
├── Consumers/               # MassTransit IConsumer<T> implementations
├── Services/                # Twilio SMS service + options
├── Program.cs               # Startup, DI registration, MassTransit config
├── appsettings.json         # Default configuration (no secrets)
└── Dockerfile               # Multi-stage Docker build
```

## Dependencies

| Package | Version | Purpose |
|---|---|---|
| `MassTransit.RabbitMQ` | 8.4.0 | Message bus / consumer host |
| `SendGrid` | 9.29.3 | Transactional email delivery |
| `Twilio` | 7.13.6 | SMS delivery |
| `Serilog.Extensions.Hosting` | 9.0.0 | Structured logging |

Internal project references: `LymmHolidayLets.Contracts`, `LymmHolidayLets.Application`, `LymmHolidayLets.Application.Model`, `LymmHolidayLets.Infrastructure`

## Configuration

All secrets must be provided via environment variables, user secrets, or a secrets manager — **never commit credentials to `appsettings.json`**.

```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  },
  "SendGrid": {
    "ApiKey": "<your-sendgrid-api-key>"
  },
  "Twilio": {
    "AccountSid": "<your-account-sid>",
    "AuthToken": "<your-auth-token>",
    "FromNumber": "+447897031197"
  },
  "Email": {
    "CompanyName": "<company display name>",
    "CompanyEmail": "<company inbox address>",
    "CcEmails": {
      "Name": "email@example.com"
    }
  }
}
```

For local development, use [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets):

```bash
dotnet user-secrets set "SendGrid:ApiKey" "your-key" --project src/LymmHolidayLets.NotificationWorker
dotnet user-secrets set "Twilio:AccountSid" "your-sid" --project src/LymmHolidayLets.NotificationWorker
dotnet user-secrets set "Twilio:AuthToken" "your-token" --project src/LymmHolidayLets.NotificationWorker
```

## Installation & Running

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- RabbitMQ instance (local or remote)

### Run locally

```bash
dotnet run --project src/LymmHolidayLets.NotificationWorker/LymmHolidayLets.NotificationWorker.csproj
```

### Run with Docker

```bash
# From the repository root
docker build -f src/LymmHolidayLets.NotificationWorker/Dockerfile -t lymmholidaylets-notificationworker .
docker run --rm lymmholidaylets-notificationworker
```

### Run as part of the full stack

```bash
docker-compose up
```

## Contributing

Follow the conventions in [`AGENTS.md`](../../AGENTS.md) at the repository root. Key points:

- No business logic in consumers — delegate to application services
- Use `ILogger<T>` structured logging throughout
- Add unit tests in `LymmHolidayLets.UnitTests` for any new consumer or service logic
- Run `dotnet build lymmholidaylets.slnx` and `dotnet test lymmholidaylets.slnx` before submitting changes
