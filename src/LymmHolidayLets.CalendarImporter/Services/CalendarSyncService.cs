using LymmHolidayLets.CalendarImporter.Interfaces;
using Microsoft.Extensions.Logging;

namespace LymmHolidayLets.CalendarImporter.Services;

public sealed class CalendarSyncService : ICalendarSyncService
{
    private readonly ILogger<CalendarSyncService> _logger;
    private readonly ICalendarProviderFactory _providerFactory;
    private readonly ICalendarDataAdapter _dataAdapter;
    private readonly HashSet<CalendarBlockKey> _processedBlocks = new();

    public CalendarSyncService(
        ILogger<CalendarSyncService> logger,
        ICalendarProviderFactory providerFactory,
        ICalendarDataAdapter dataAdapter)
    {
        _logger = logger;
        _providerFactory = providerFactory;
        _dataAdapter = dataAdapter;
    }

    public async Task SyncAllCalendarsAsync(List<PropertyCalendarConfig> properties, CancellationToken cancellationToken)
    {
        var totalBlocks = 0;
        var totalErrors = 0;

        foreach (var property in properties)
        {
            _logger.LogInformation("Syncing calendars for property {PropertyId} - {PropertyName}",
                property.PropertyId, property.PropertyName);

            foreach (var calendar in property.Calendars)
            {
                try
                {
                    var blocksAdded = await SyncCalendarAsync(
                        property.PropertyId,
                        property.PropertyName,
                        calendar,
                        cancellationToken);

                    totalBlocks += blocksAdded;
                }
                catch (Exception ex)
                {
                    totalErrors++;
                    _logger.LogError(ex,
                        "Failed to sync {Provider} calendar for property {PropertyId}",
                        calendar.Provider, property.PropertyId);
                    // Continue with next calendar
                }
            }
        }

        _logger.LogInformation(
            "Calendar sync completed: {TotalBlocks} blocks processed, {TotalErrors} errors",
            totalBlocks, totalErrors);
    }

    private async Task<int> SyncCalendarAsync(
        int propertyId,
        string propertyName,
        CalendarFeedConfig calendarConfig,
        CancellationToken cancellationToken)
    {
        var provider = _providerFactory.GetProvider(calendarConfig.Provider);
        if (provider == null)
        {
            _logger.LogWarning("Unknown calendar provider: {Provider}", calendarConfig.Provider);
            return 0;
        }

        _logger.LogDebug("Fetching {Provider} calendar for property {PropertyId}",
            provider.ProviderName, propertyId);

        var blocks = await provider.FetchCalendarBlocksAsync(calendarConfig.Url, cancellationToken);

        var blocksAdded = 0;
        foreach (var block in blocks)
        {
            var key = new CalendarBlockKey(propertyId, block.StartDate, block.EndDate);

            // Skip if already processed in this sync
            if (_processedBlocks.Contains(key))
            {
                continue;
            }

            try
            {
                _logger.LogInformation(
                    "Blocking calendar for {PropertyName} (ID: {PropertyId}) from {StartDate} to {EndDate} via {Provider}",
                    propertyName, propertyId, block.StartDate, block.EndDate, provider.ProviderName);

                _dataAdapter.BlockCalendarByPropertyForDate(propertyId, block.StartDate, block.EndDate);
                _processedBlocks.Add(key);
                blocksAdded++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to block calendar for property {PropertyId} from {StartDate} to {EndDate}",
                    propertyId, block.StartDate, block.EndDate);
            }
        }

        return blocksAdded;
    }

    private sealed record CalendarBlockKey(int PropertyId, DateOnly StartDate, DateOnly EndDate);
}
