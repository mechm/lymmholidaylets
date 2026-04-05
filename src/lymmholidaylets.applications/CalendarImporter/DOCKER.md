# Docker Setup - CalendarImporter

This document describes the Docker configuration for the LymmHolidayLets CalendarImporter background service.

## Overview

The CalendarImporter is configured as a Docker service in the main `docker-compose.yml` file and runs as a background worker alongside the other application services.

## Files

### Dockerfile
Location: `src/LymmHolidayLets.Applications/CalendarImporter/Dockerfile`

Multi-stage Dockerfile that:
1. **Build stage**: Restores dependencies and builds the project
2. **Publish stage**: Publishes the release build
3. **Runtime stage**: Creates a minimal runtime container with the published app

### .dockerignore
Location: `src/LymmHolidayLets.Applications/CalendarImporter/.dockerignore`

Excludes unnecessary files from the Docker build context to improve build performance.

## Docker Compose Configuration

### Service Definition (docker-compose.yml)

```yaml
lymmholidaylets-calendarimporter:
  build:
    context: .
    dockerfile: src/LymmHolidayLets.Applications/CalendarImporter/Dockerfile
  container_name: calendar_importer
  environment:
    ConnectionStrings__LymmHolidayLets: "Server=mssql,1433;Database=LymmHolidayLets;..."
    CalendarSync__IntervalMinutes: "30"
    Logging__LogLevel__Default: "Information"
    Logging__LogLevel__LymmHolidayLets.CalendarImporter: "Information"
  depends_on:
    mssql:
      condition: service_healthy
  restart: unless-stopped
```

### Development Overrides (docker-compose.override.yml)

```yaml
lymmholidaylets-calendarimporter:
  environment:
    Logging__LogLevel__Default: "Debug"
    Logging__LogLevel__LymmHolidayLets.CalendarImporter: "Debug"
    CalendarSync__IntervalMinutes: "5"  # Shorter interval for development
```

## Environment Variables

The CalendarImporter supports configuration via environment variables using the .NET configuration system.

### Required

- `ConnectionStrings__LymmHolidayLets` - SQL Server connection string

### Optional

- `CalendarSync__IntervalMinutes` - How often to sync calendars (default: 30)
- `Logging__LogLevel__Default` - Default log level (Information, Debug, Warning, Error)
- `Logging__LogLevel__LymmHolidayLets.CalendarImporter` - CalendarImporter specific log level

### Calendar Configuration

Currently, calendar URLs are configured in `appsettings.json` inside the container. For production deployments, consider:

1. **Mount a configuration file:**
   ```yaml
   volumes:
     - ./calendar-config.json:/app/appsettings.Production.json:ro
   ```

2. **Use environment variables:** (for each property)
   ```yaml
   environment:
     CalendarSync__Properties__0__PropertyId: "1"
     CalendarSync__Properties__0__PropertyName: "Lymm Village Apartment"
     CalendarSync__Properties__0__Calendars__0__Provider: "Airbnb"
     CalendarSync__Properties__0__Calendars__0__Url: "https://..."
   ```

3. **Database-driven configuration:** (future enhancement)

## Usage

### Start All Services
```bash
docker-compose up -d
```

### Start Only CalendarImporter
```bash
docker-compose up -d lymmholidaylets-calendarimporter
```

### View Logs
```bash
# Follow logs in real-time
docker-compose logs -f lymmholidaylets-calendarimporter

# View last 100 lines
docker-compose logs --tail 100 lymmholidaylets-calendarimporter
```

### Restart Service
```bash
docker-compose restart lymmholidaylets-calendarimporter
```

### Stop Service
```bash
docker-compose stop lymmholidaylets-calendarimporter
```

### Force Immediate Sync
The service syncs immediately on startup, so restart it to trigger a sync:
```bash
docker-compose restart lymmholidaylets-calendarimporter
```

## Health & Monitoring

### Check Service Status
```bash
docker-compose ps lymmholidaylets-calendarimporter
```

### View Resource Usage
```bash
docker stats calendar_importer
```

### Inspect Container
```bash
docker inspect calendar_importer
```

## Logs

Logs are written to:
1. **Console (stdout)** - Visible via `docker-compose logs`
2. **File** - `logs/calendar-importer-{date}.txt` inside the container

To access file logs:
```bash
docker exec -it calendar_importer ls -la /app/logs
docker exec -it calendar_importer cat /app/logs/calendar-importer-20260404.txt
```

Or mount a volume to persist logs on the host:
```yaml
volumes:
  - ./logs/calendar-importer:/app/logs
```

## Troubleshooting

### Service Won't Start

1. Check logs:
   ```bash
   docker-compose logs lymmholidaylets-calendarimporter
   ```

2. Verify database is healthy:
   ```bash
   docker-compose ps mssql
   ```

3. Check network connectivity:
   ```bash
   docker-compose exec lymmholidaylets-calendarimporter ping mssql
   ```

### Calendars Not Syncing

1. Check logs for HTTP errors or parsing failures
2. Verify iCal URLs are accessible from inside the container:
   ```bash
   docker-compose exec lymmholidaylets-calendarimporter curl -I https://www.airbnb.co.uk/calendar/ical/...
   ```

3. Increase logging verbosity:
   ```bash
   docker-compose down
   # Edit docker-compose.override.yml to set Debug level
   docker-compose up -d
   ```

### High Memory Usage

1. Check resource usage:
   ```bash
   docker stats calendar_importer
   ```

2. Consider increasing sync interval to reduce memory pressure

### Connection Errors

Ensure the connection string uses the service name `mssql`, not `localhost`:
```
Server=mssql,1433;Database=LymmHolidayLets;...
```

## Production Deployment

For production:

1. **Remove override file** or set `COMPOSE_FILE` environment variable:
   ```bash
   export COMPOSE_FILE=docker-compose.yml
   docker-compose up -d
   ```

2. **Use secrets** for sensitive data instead of environment variables

3. **Set resource limits:**
   ```yaml
   lymmholidaylets-calendarimporter:
     deploy:
       resources:
         limits:
           cpus: '0.5'
           memory: 512M
         reservations:
           cpus: '0.25'
           memory: 256M
   ```

4. **Enable log rotation** or use a logging driver:
   ```yaml
   logging:
     driver: "json-file"
     options:
       max-size: "10m"
       max-file: "3"
   ```

## Integration with Solution

The CalendarImporter service:
- **Depends on**: `mssql` (SQL Server database)
- **Does not depend on**: API, UI, RabbitMQ, EmailWorker (runs independently)
- **Restart policy**: `unless-stopped` (auto-restarts on failure)
- **Network**: Shares the default Docker Compose network with other services

## Future Enhancements

- Add health check endpoint to the worker service
- Implement Prometheus metrics endpoint
- Store calendar configurations in database instead of config file
- Add configuration UI in the main application
