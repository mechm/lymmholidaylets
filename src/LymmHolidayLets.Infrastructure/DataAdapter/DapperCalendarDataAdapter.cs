using Dapper;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Infrastructure.Exception;
using LymmHolidayLets.Infrastructure.Repository;
using System.Data;

namespace LymmHolidayLets.Infrastructure.DataAdapter
{
    public sealed class DapperCalendarDataAdapter : SqlQueryBase, IDapperCalendarDataAdapter
    {
        public DapperCalendarDataAdapter(DbSession session) : base(session)
        {
        }

        public bool GetPropertyAvailableForDate(byte propertyId, DateOnly date)
        {
            const string procedure = "Available_GetByPropertyID";

            try
            {
                using var sqlConnection = _session.Connection;
                var available = sqlConnection.ExecuteScalar<bool>(procedure, new
                {
                    propertyId,
                    date = date.ToDateTime(new TimeOnly(0, 0)),
                }, _session.Transaction, commandType: CommandType.StoredProcedure);
                return available;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException(
                    $"An error occurred finding date available with the procedure {procedure}", ex);
            }
        }

        public IEnumerable<Domain.ReadModel.Calendar.Calendar> GetPropertyAvailableBetweenDates(byte propertyId, DateOnly checkIn, DateOnly checkOut, bool available)
        {
            const string procedure = "Calendar_GetByPropertyID_Date";

            try
            {
                using var sqlConnection = _session.Connection;
                sqlConnection.Open();

                IEnumerable<Domain.ReadModel.Calendar.Calendar> availability = sqlConnection.Query<Domain.ReadModel.Calendar.Calendar>(procedure,
                    new {
                        propertyID = propertyId,
                        checkIn,
                        checkOut,
                        available
                    },
                    _session.Transaction,
                    commandType: CommandType.StoredProcedure);
                return availability;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException(
                    $"An error occurred finding dates available with the procedure {procedure}", ex);
            }
        }
    }
}
