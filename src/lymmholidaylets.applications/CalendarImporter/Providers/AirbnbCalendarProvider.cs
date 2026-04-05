using LymmHolidayLets.CalendarImporter.Interfaces;
using Ical.Net;
using Microsoft.Extensions.Logging;

namespace LymmHolidayLets.CalendarImporter.Providers;

public sealed class AirbnbCalendarProvider : ICalendarProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AirbnbCalendarProvider> _logger;

    public string ProviderName => "Airbnb";

    public AirbnbCalendarProvider(HttpClient httpClient, ILogger<AirbnbCalendarProvider> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        // Airbnb requires user-agent
        if (!_httpClient.DefaultRequestHeaders.Contains("user-agent"))
        {
            _httpClient.DefaultRequestHeaders.Add("user-agent", 
                "Mozilla/5.0 (compatible; LymmHolidayLetsCalendarSync/1.0)");
        }
    }

    public async Task<List<CalendarBlock>> FetchCalendarBlocksAsync(string url, CancellationToken cancellationToken)
    {
        var blocks = new List<CalendarBlock>();

        try
        {
            _logger.LogInformation("Fetching Airbnb calendar from {Url}", url);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            using var stream = new MemoryStream(content);

            var calendar = Calendar.Load(stream);

            foreach (var calEvent in calendar.Events)
            {
                if (calEvent.Summary != "Reserved" && calEvent.Summary != "Not available")
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

            _logger.LogInformation("Found {BlockCount} calendar blocks from Airbnb", blocks.Count);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching Airbnb calendar from {Url}", url);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing Airbnb calendar from {Url}", url);
            throw;
        }

        return blocks;
    }
}
