using Dapper;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.AvailabilityICal;
using LymmHolidayLets.Infrastructure.Exception;
using LymmHolidayLets.Infrastructure.Repository;
using System.Data;

namespace LymmHolidayLets.Infrastructure.DataAdapter
{
    public sealed class DapperICalDataAdapter(DbSession session) : SqlQueryBase(session), IDapperICalDataAdapter
    {
        public IEnumerable<AvailabilityICal> GetICalAvailability(byte propertyId)
        {
            const string procedure = "ICalBooking_Available_GetByPropertyID";

            try
            {
                using var connection = Session.Connection;
                var availability = connection.Query<AvailabilityICal>(procedure,
                    new { PropertyID = propertyId },
                    Session.Transaction, 
                    commandType: CommandType.StoredProcedure);

                return availability;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException(
                    $"An error occurred finding ical availability with the procedure {procedure}", ex);
            }
        }

        public async Task<IReadOnlyList<AvailabilityICal>> GetICalAvailabilityAsync(byte propertyId, CancellationToken cancellationToken = default)
        {
            const string procedure = "ICalBooking_Available_GetByPropertyID";

            try
            {
                using var connection = Session.Connection;
                var command = new CommandDefinition(
                    procedure,
                    new { PropertyID = propertyId },
                    Session.Transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                var availability = await connection.QueryAsync<AvailabilityICal>(command);

                return availability.AsList();
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException(
                    $"An error occurred finding ical availability with the procedure {procedure}", ex);
            }
        }
    }
}
