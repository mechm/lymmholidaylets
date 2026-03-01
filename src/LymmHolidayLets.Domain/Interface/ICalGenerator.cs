namespace LymmHolidayLets.Domain.Interface
{
    public interface ICalGenerator
    {
        Task<string> GenerateCalendarAsync(byte propertyId);
    }
}
