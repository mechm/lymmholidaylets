using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Model.ICal.Entity;
using LymmHolidayLets.Domain.ReadModel.AvailabilityICal;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Query
{
    public sealed class CalQuery : ICalQuery
    {
        private readonly IDapperICalRepository _icalRepository;
        private readonly IDapperICalDataAdapter _icalDataAdapter;

        public CalQuery(IDapperICalRepository icalRepository, IDapperICalDataAdapter icalDataAdapter)
        {
            _icalRepository = icalRepository;
            _icalDataAdapter = icalDataAdapter;
        }

        public IList<ICal> GetAll()
        {
            return _icalRepository.GetAll();
        }

        public IEnumerable<AvailabilityICal> GetICalAvailability(byte propertyId)
        {
            return _icalDataAdapter.GetICalAvailability(propertyId);
        }
    }
}
