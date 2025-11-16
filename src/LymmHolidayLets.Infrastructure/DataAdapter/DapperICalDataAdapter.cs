using Dapper;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.AvailabilityICal;
using LymmHolidayLets.Infrastructure.Exception;
using LymmHolidayLets.Infrastructure.Repository;
using System.Data;

namespace LymmHolidayLets.Infrastructure.DataAdapter
{
    public sealed class DapperICalDataAdapter : SqlQueryBase, IDapperICalDataAdapter
    {
        public DapperICalDataAdapter(DbSession session) : base(session)
        {
        }

        public IEnumerable<AvailabilityICal> GetICalAvailability(byte propertyId)
        {
            const string procedure = "ICalBooking_Available_GetByPropertyID";

            try
            {
                using var connection = _session.Connection;
                IEnumerable<AvailabilityICal> availability = connection.Query<AvailabilityICal>(procedure,
                    new { PropertyID = propertyId },
                    _session.Transaction, 
                    commandType: CommandType.StoredProcedure);

                return availability;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException(
                    $"An error occurred finding ical availability with the procedure {procedure}", ex);
            }
        }
    }
}
