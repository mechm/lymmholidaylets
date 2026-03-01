using Microsoft.AspNetCore.Mvc;

namespace LymmHolidayLets.Api.Services
{
    public interface ICalService
    {
        Task<FileContentResult?> GetCalendarAsync(int id, Guid s);
    }
}
