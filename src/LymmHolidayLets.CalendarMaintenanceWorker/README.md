# LymmHolidayLets.CalendarMaintenanceWorker

Lightweight background worker that maintains the calendar date range infrastructure.

## Purpose

Executes the `Calendar_DateRange` stored procedure once daily at **2 AM UTC** to:
- Rebuild the `CalendarRange` table with dates for the next 2 years
- Populate `Calendar` table with default property availability
- Clean up old calendar entries (older than 13 months)

## Configuration

### Connection String
Set in `appsettings.json` or environment variables:

```json
{
  "ConnectionStrings": {
    "LymmHolidayLets": "Server=localhost,1433;Database=LymmHolidayLets;..."
  }
}
```

### Docker Environment Variables
```yaml
environment:
  ConnectionStrings__LymmHolidayLets: "Server=mssql,1433;Database=LymmHolidayLets;User Id=sa;Password=YourStrong!Password123;TrustServerCertificate=True;Encrypt=False;"
```

## Behavior

- **Startup**: Runs immediately when the worker starts
- **Schedule**: Runs once per day at 2 AM UTC
- **Error Handling**: Logs errors and retries at the next scheduled time
- **Timeout**: Stored procedure has a 120-second timeout

## Running Locally

```bash
cd src/LymmHolidayLets.CalendarMaintenanceWorker
dotnet run
```

## Docker

Included in `docker-compose.yml` as `lymmholidaylets-calendarmaintenance` service.

```bash
docker-compose up lymmholidaylets-calendarmaintenance
```

## Logs

- Console output (stdout)
- File: `logs/calendar-maintenance-YYYYMMDD.txt` (rolling daily)

## Monitoring

Check logs for:
- `"Starting calendar maintenance"` - execution started
- `"Calendar maintenance completed in Xs"` - successful completion
- `"Error during calendar maintenance"` - execution failed
- `"Next maintenance scheduled for..."` - next run time

## Dependencies

- .NET 10.0 Runtime
- SQL Server (accessible via connection string)
- `Calendar_DateRange` stored procedure must exist in database
