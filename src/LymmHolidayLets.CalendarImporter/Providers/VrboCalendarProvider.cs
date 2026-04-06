using LymmHolidayLets.CalendarImporter.Interfaces;
using Ical.Net;

namespace LymmHolidayLets.CalendarImporter.Providers;

public sealed class VrboCalendarProvider(HttpClient httpClient, ILogger<VrboCalendarProvider> logger)
    : ICalendarProvider
{
    public string ProviderName => "VRBO";

    public async Task<List<CalendarBlock>> FetchCalendarBlocksAsync(string url, CancellationToken cancellationToken)
    {
        var blocks = new List<CalendarBlock>();

        try
        {
            logger.LogInformation("Fetching VRBO calendar from {Url}", url);

            var response = await httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            using var stream = new MemoryStream(content);

            var calendar = Calendar.Load(stream);

            foreach (var calEvent in calendar.Events)
            {
                // VRBO marks blocked/unavailable dates
                if (calEvent.Summary?.Contains("unavailable", StringComparison.OrdinalIgnoreCase) != true &&
                    calEvent.Summary?.Contains("blocked", StringComparison.OrdinalIgnoreCase) != true &&
                    calEvent.Summary?.Contains("reserved", StringComparison.OrdinalIgnoreCase) != true)
                {
                    continue;
                }
                
                var startDate = DateOnly.FromDateTime(calEvent.Start.Date);
                var endDate = DateOnly.FromDateTime(calEvent.End.Date);

                // Skip past dates
                if (endDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
                {
                    continue;
                }

                blocks.Add(new CalendarBlock
                {
                    StartDate = startDate,
                    EndDate = endDate
                });
            }

            logger.LogInformation("Found {BlockCount} calendar blocks from VRBO", blocks.Count);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP error fetching VRBO calendar from {Url}", url);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error parsing VRBO calendar from {Url}", url);
            throw;
        }

        return blocks;
    }
}
