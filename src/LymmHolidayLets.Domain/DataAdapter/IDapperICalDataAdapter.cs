using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.ReadModel.AvailabilityICal;

namespace LymmHolidayLets.Domain.DataAdapter
{
    public interface IDapperICalDataAdapter : IDapperSqlQueryBase
    {
        IEnumerable<AvailabilityICal> GetICalAvailability(byte propertyId);
    }
}