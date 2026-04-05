# LymmHolidayLets Calendar Importer Worker Service

A .NET 10 background worker service that synchronizes calendar availability from external booking platforms (Airbnb, Booking.com, VRBO) into the LymmHolidayLets database.

## Features

- ✅ **.NET 10 Worker Service** - Modern hosted service pattern with `BackgroundService`
- ✅ **Serilog** - Structured logging aligned with the main application
- ✅ **Polly Resilience** - Automatic retry with exponential backoff for HTTP failures
- ✅ **IHttpClientFactory** - Proper HttpClient lifecycle management
- ✅ **Options Pattern** - Strongly-typed configuration via `appsettings.json`
- ✅ **Async/Await** - Fully asynchronous throughout
- ✅ **Graceful Shutdown** - Proper cancellation token handling
- ✅ **Health Checks** - Monitor service health
- ✅ **Deduplication** - Prevents duplicate calendar blocks in a single sync

## Architecture

```
LymmHolidayLets.CalendarImporter/
├── Program.cs                     # Entry point with DI setup
├── CalendarSyncWorker.cs          # Background service (runs periodically)
├── CalendarSyncOptions.cs         # Configuration models
├── Interfaces/                    # Abstractions
│   ├── ICalendarProvider.cs
│   ├── ICalendarSyncService.cs
│   ├── ICalendarProviderFactory.cs
│   ├── ICalendarDataAdapter.cs
│   └── IDatabaseFactory.cs
├── Providers/                     # iCal feed parsers
│   ├── AirbnbCalendarProvider.cs
│   ├── BookingCalendarProvider.cs
│   └── VrboCalendarProvider.cs
├── Services/                      # Business logic
│   ├── CalendarSyncService.cs
│   └── CalendarProviderFactory.cs
└── Infrastructure/                # Data access
    ├── DatabaseFactory.cs
    ├── DbSession.cs
    └── CalendarDataAdapter.cs
```

## Configuration

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "LymmHolidayLets": "Server=localhost,1433;Database=LymmHolidayLets;..."
  },
  "CalendarSync": {
    "IntervalMinutes": 30,
    "Properties": [
      {
        "PropertyId": 1,
        "PropertyName": "Lymm Village Apartment",
        "Calendars": [
          {
            "Provider": "Airbnb",
            "Url": "https://www.airbnb.co.uk/calendar/ical/..."
          }
        ]
      }
    ]
  }
}
```

### Configuration Options

- **IntervalMinutes**: How often to sync calendars (default: 30)
- **Properties**: Array of properties with their calendar feed URLs
  - **PropertyId**: Database ID of the property
  - **PropertyName**: Human-readable name (for logging)
  - **Calendars**: Array of iCal feed configurations
    - **Provider**: `Airbnb`, `Booking`, or `VRBO`
    - **Url**: iCal feed URL from the booking platform

## Running the Service

### Development (Console)

```bash
cd src/LymmHolidayLets.Applications/CalendarImporter
dotnet run
```

### Production (Windows Service)

```bash
# Build
dotnet publish -c Release

# Install as Windows Service
sc create "LymmHolidayLets.CalendarImporter" binPath="path\to\LymmHolidayLets.CalendarImporter.exe"
sc start "LymmHolidayLets.CalendarImporter"
```

### Docker (Recommended for Production)

The service is included in `docker-compose.yml` as `lymmholidaylets-calendarimporter`.

**Run all services:**
```bash
docker-compose up
```

**Run only CalendarImporter:**
```bash
docker-compose up lymmholidaylets-calendarimporter
```

**View logs:**
```bash
docker-compose logs -f lymmholidaylets-calendarimporter
```

**Configuration:**
Calendar URLs and sync interval can be configured via environment variables in `docker-compose.yml`:
- `CalendarSync__IntervalMinutes` - Sync interval (default: 30 minutes)
- `ConnectionStrings__LymmHolidayLets` - Database connection string

For development, `docker-compose.override.yml` sets a shorter interval (5 minutes) and debug logging.


## How It Works

1. **Startup**: The worker starts and runs an initial sync immediately
2. **Periodic Sync**: Every N minutes (configurable), the worker:
   - Fetches iCal feeds from all configured providers
   - Parses calendar events marked as "Reserved" or "Unavailable"
   - Filters out past dates
   - Calls the `Calendar_Update_Property_Date` stored procedure to block dates
   - Deduplicates blocks within the same sync run
3. **Error Handling**: If a provider fails, it logs the error and continues with other providers
4. **Retry Logic**: HTTP failures are retried 3 times with exponential backoff

## Database Integration

The service calls the `Calendar_Update_Property_Date` stored procedure:

```sql
EXEC Calendar_Update_Property_Date 
    @propertyId = 1, 
    @startDate = '2026-04-15', 
    @endDate = '2026-04-20'
```

This procedure should be idempotent (safe to call multiple times with the same dates).

## Logging

Logs are written to:
- **Console** - Structured output
- **File** - `logs/calendar-importer-{date}.txt` (rolling daily)

Example log output:

```
[INF] Starting calendar synchronization
[INF] Syncing calendars for property 1 - Lymm Village Apartment
[INF] Fetching Airbnb calendar from https://...
[INF] Found 12 calendar blocks from Airbnb
[INF] Blocking calendar for Lymm Village Apartment (ID: 1) from 2026-04-15 to 2026-04-20 via Airbnb
[INF] Calendar synchronization completed in 3.2s
```

## Improvements from Original Version

| Aspect | Old Version | New Version |
|--------|-------------|-------------|
| Framework | Topshelf (unmaintained) | .NET Worker Service |
| .NET Version | .NET 7 | .NET 10 |
| Logging | NLog | Serilog (aligned with main app) |
| SQL Client | System.Data.SqlClient (deprecated) | Microsoft.Data.SqlClient |
| Async | Synchronous `.Wait()` | Proper async/await |
| HTTP | Manual HttpClient | IHttpClientFactory + Polly |
| Configuration | XML file | appsettings.json |
| Error Handling | Crash on failure | Graceful error handling + retries |
| Shutdown | Abrupt | Graceful with CancellationToken |
| Code Structure | "Infastructure" typo | Fixed + cleaner architecture |

## Troubleshooting

### Service won't start
- Check connection string in `appsettings.json`
- Verify SQL Server is accessible
- Check logs in `logs/` directory

### Calendars not syncing
- Verify iCal URLs are accessible (test in browser)
- Check provider names match exactly: `Airbnb`, `Booking`, or `VRBO`
- Review logs for HTTP errors or parsing failures

### High memory usage
- Consider reducing sync interval
- Check for memory leaks in provider code

## Future Enhancements

- [ ] Store calendar URLs in database instead of config file
- [ ] Add metrics (Prometheus/StatsD)
- [ ] Circuit breaker for consistently failing providers
- [ ] Support for additional providers (TripAdvisor, etc.)
- [ ] Webhook support for real-time updates
- [ ] Admin UI for managing calendar feed URLs
