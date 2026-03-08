using LymmHolidayLets.Domain.Model.ICal.Entity;
using LymmHolidayLets.Domain.ReadModel.AvailabilityICal;

namespace LymmHolidayLets.Application.Interface.Query
{
    public interface ICalQuery
    {
        IList<ICal> GetAll();
        Task<IReadOnlyList<ICal>> GetAllAsync(CancellationToken cancellationToken = default);
        IEnumerable<AvailabilityICal> GetICalAvailability(byte propertyId);
        Task<IReadOnlyList<AvailabilityICal>> GetICalAvailabilityAsync(byte propertyId, CancellationToken cancellationToken = default);
    }
}