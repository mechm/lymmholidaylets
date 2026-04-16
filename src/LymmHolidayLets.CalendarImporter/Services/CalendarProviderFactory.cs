using LymmHolidayLets.CalendarImporter.Interfaces;
using LymmHolidayLets.CalendarImporter.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace LymmHolidayLets.CalendarImporter.Services;

public sealed class CalendarProviderFactory(IServiceProvider serviceProvider) : ICalendarProviderFactory
{
    public ICalendarProvider? GetProvider(string providerName)
    {
        return providerName?.ToLowerInvariant() switch
        {
            "airbnb" => serviceProvider.GetRequiredService<AirbnbCalendarProvider>(),
            "booking" or "booking.com" => serviceProvider.GetRequiredService<BookingCalendarProvider>(),
            "vrbo" => serviceProvider.GetRequiredService<VrboCalendarProvider>(),
            _ => null
        };
    }
}
