using LymmHolidayLets.Domain.Model.ICal.Entity;
using LymmHolidayLets.Domain.ReadModel.AvailabilityICal;

namespace LymmHolidayLets.Application.Interface.Query
{
    public interface ICalQuery
    {
        IList<ICal> GetAll();
        IEnumerable<AvailabilityICal> GetICalAvailability(byte propertyId);
    }
}