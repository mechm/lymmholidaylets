# LymmHolidayLets.EmailScheduler

## Description

`LymmHolidayLets.EmailScheduler` is a .NET 10 background worker that polls the database for due guest pre-arrival emails and publishes `GuestPreArrivalEmailRequested` messages to RabbitMQ. It is the scheduling part of the pre-arrival email flow; the actual rendering and sending is handled downstream by `LymmHolidayLets.NotificationWorker`.

## Installation

### Prerequisites

- .NET 10 SDK
- SQL Server with the LymmHolidayLets schema deployed
- RabbitMQ
- The main solution dependencies restored

### Local setup

From the repository root:

```sh
dotnet restore src\LymmHolidayLets.EmailScheduler\LymmHolidayLets.EmailScheduler.csproj
```

The worker reads configuration from:

- `appsettings.json`
- `appsettings.local.json` (optional, local overrides)
- environment variables

Required settings:

```json
{
  "ConnectionStrings": {
    "LymmHolidayLets": "Server=localhost,1433;Database=LymmHolidayLets;User Id=sa;Password=YourStrong!Password123;TrustServerCertificate=True;Encrypt=False;"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  },
  "GuestPreArrivalEmailScheduler": {
    "IntervalMinutes": 30,
    "ReservationTimeoutMinutes": 15
  }
}
```

## Usage

### Run locally

```sh
dotnet run --project src\LymmHolidayLets.EmailScheduler\LymmHolidayLets.EmailScheduler.csproj
```

### Run in Docker Compose

```sh
docker-compose up --build lymmholidaylets-emailscheduler
```

### What it does

On each cycle, the worker:

1. Loads due bookings from the database.
2. Reserves a dispatch row to avoid duplicate publishes.
3. Publishes a `GuestPreArrivalEmailRequested` message to RabbitMQ.
4. Marks the dispatch as `Published` or `Failed`.

## Features

- Periodic polling using `BackgroundService` and `PeriodicTimer`
- Dapper-based reads and dispatch reservation updates
- RabbitMQ publishing via MassTransit
- Serilog console logging
- Configurable polling and reservation timeouts
- Docker support for running alongside the rest of the platform

## Contributing

Follow the repository conventions when changing this worker:

- Keep scheduling/orchestration logic in this worker and email delivery in `LymmHolidayLets.NotificationWorker`.
- Preserve the existing Clean Architecture dependency flow.
- Use the repository build and test commands before finishing changes.
- Add or update tests when scheduling behavior changes.
