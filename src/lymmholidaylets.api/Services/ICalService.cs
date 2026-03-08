using Microsoft.AspNetCore.Mvc;

namespace LymmHolidayLets.Api.Services
{
    public interface ICalService
    {
        Task<FileContentResult?> GetCalendarAsync(byte propertyId, Guid identifier, CancellationToken cancellationToken = default);
    }
}
