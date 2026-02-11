using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Model.ICal.Entity;
using LymmHolidayLets.Domain.ReadModel.AvailabilityICal;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Query
{
    public sealed class CalQuery(IDapperICalRepository icalRepository, IDapperICalDataAdapter icalDataAdapter)
        : ICalQuery
    {
        public IList<ICal> GetAll()
        {
            return icalRepository.GetAll();
        }

        public IEnumerable<AvailabilityICal> GetICalAvailability(byte propertyId)
        {
            return icalDataAdapter.GetICalAvailability(propertyId);
        }
    }
}
