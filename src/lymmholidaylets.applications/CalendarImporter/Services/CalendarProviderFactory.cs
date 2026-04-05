using LymmHolidayLets.CalendarImporter.Interfaces;
using LymmHolidayLets.CalendarImporter.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace LymmHolidayLets.CalendarImporter.Services;

public sealed class CalendarProviderFactory : ICalendarProviderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CalendarProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ICalendarProvider? GetProvider(string providerName)
    {
        return providerName?.ToLowerInvariant() switch
        {
            "airbnb" => _serviceProvider.GetRequiredService<AirbnbCalendarProvider>(),
            "booking" or "booking.com" => _serviceProvider.GetRequiredService<BookingCalendarProvider>(),
            "vrbo" => _serviceProvider.GetRequiredService<VrboCalendarProvider>(),
            _ => null
        };
    }
}
