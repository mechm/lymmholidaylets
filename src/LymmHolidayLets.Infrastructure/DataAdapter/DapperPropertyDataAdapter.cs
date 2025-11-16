using Dapper;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Property;
using LymmHolidayLets.Infrastructure.Exception;
using LymmHolidayLets.Infrastructure.Repository;
using System.Data;

namespace LymmHolidayLets.Infrastructure.DataAdapter
{
    public sealed class DapperPropertyDataAdapter : SqlQueryBase, IDapperPropertyDataAdapter
    {
        public DapperPropertyDataAdapter(DbSession session) : base(session)
        {
        }

        public PropertyBooking GetPropertyBookingById(byte propertyId)
        {
            const string procedure = "Property_Booking_GetByID";

            try
            {
                using var connection = _session.Connection;
                var propertyBooking = connection.QueryFirst<PropertyBooking>(procedure,
                    new { PropertyID = propertyId },
                    _session.Transaction,
                    commandType: CommandType.StoredProcedure);

                return propertyBooking;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException(
                    $"An error occurred finding property booking with the procedure {procedure}", ex);
            }
        }

        public PropertyDetailAggregate? GetPropertyDetailById(byte propertyId)
        {
            const string procedure = "Property_Detail_GetByID";

            try
            {
                PropertyDetailAggregate propertyDetailAggregate;

                using var sqlConnection = _session.Connection;
                using (var result = sqlConnection.QueryMultiple(procedure, new
                {
                    propertyId,
                }, _session.Transaction,
                  commandType: CommandType.StoredProcedure))
                {
                    PropertyBooking? propertyBooking = result.ReadSingleOrDefault<PropertyBooking>();
                   
                    if (propertyBooking == null) 
                    {
                        return null;
                    }

                    IEnumerable<DateOnly> datesBooked = result.Read<DateOnly>();
                    IEnumerable<FAQ> faqs = result.Read<FAQ>();
                    IEnumerable<Review> review = result.Read<Review>();
                    propertyDetailAggregate = new PropertyDetailAggregate(propertyBooking, datesBooked, faqs, review);
                }
                return propertyDetailAggregate;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException(
                    $"An error occurred finding property details with the procedure {procedure}", ex);
            }
        }

        public PropertyCheckInCheckOutTime? GetPropertyCheckInCheckOutTime(byte propertyId)
        {
            const string procedure = "Property_CheckInCheckOutTime";

            try
            {
                PropertyCheckInCheckOutTime? propertyCheckInCheckOutTime;

                using var sqlConnection = _session.Connection;
                using (var result = sqlConnection.QueryMultiple(procedure, new
                {
                    propertyId,
                }, _session.Transaction,
                  commandType: CommandType.StoredProcedure))
                {
                    propertyCheckInCheckOutTime = result.ReadSingleOrDefault<PropertyCheckInCheckOutTime>();              
                }
                return propertyCheckInCheckOutTime;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException(
                    $"An error occurred finding property checkin/ checkout times with the procedure {procedure}", ex);
            }
        }
    }
}