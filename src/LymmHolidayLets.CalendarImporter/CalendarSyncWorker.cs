using LymmHolidayLets.CalendarImporter.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LymmHolidayLets.CalendarImporter;

public sealed class CalendarSyncWorker : BackgroundService
{
    private readonly ILogger<CalendarSyncWorker> _logger;
    private readonly ICalendarSyncService _syncService;
    private readonly CalendarSyncOptions _options;
    private readonly PeriodicTimer _timer;

    public CalendarSyncWorker(
        ILogger<CalendarSyncWorker> logger,
        ICalendarSyncService syncService,
        IOptions<CalendarSyncOptions> options)
    {
        _logger = logger;
        _syncService = syncService;
        _options = options.Value;
        _timer = new PeriodicTimer(TimeSpan.FromMinutes(_options.IntervalMinutes));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Calendar Sync Worker starting with {IntervalMinutes} minute interval",
            _options.IntervalMinutes);

        // Run immediately on startup
        await SyncCalendarsAsync(stoppingToken);

        // Then run periodically
        while (!stoppingToken.IsCancellationRequested && await _timer.WaitForNextTickAsync(stoppingToken))
        {
            await SyncCalendarsAsync(stoppingToken);
        }
    }

    private async Task SyncCalendarsAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting calendar synchronization");
            var startTime = DateTime.UtcNow;

            await _syncService.SyncAllCalendarsAsync(_options.Properties, cancellationToken);

            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("Calendar synchronization completed in {DurationSeconds}s",
                duration.TotalSeconds);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Calendar synchronization cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during calendar synchronization");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calendar Sync Worker stopping");
        _timer.Dispose();
        await base.StopAsync(cancellationToken);
    }
}
