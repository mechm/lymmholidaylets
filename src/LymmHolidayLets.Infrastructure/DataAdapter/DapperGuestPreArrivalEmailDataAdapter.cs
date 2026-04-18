using System.Data;
using Dapper;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Email;
using LymmHolidayLets.Infrastructure.Exception;
using LymmHolidayLets.Infrastructure.Repository;

namespace LymmHolidayLets.Infrastructure.DataAdapter;

public sealed class DapperGuestPreArrivalEmailDataAdapter(DbSession session)
    : SqlQueryBase(session), IGuestPreArrivalEmailDataAdapter
{
    public IReadOnlyList<GuestPreArrivalDueBooking> GetDueBookings(DateOnly? runDate = null)
    {
        const string procedure = "Booking_GuestPreArrivalEmail_GetDue";

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("RunDate", runDate);

            return QueryProcedure<GuestPreArrivalDueBooking>(procedure, parameters).ToList();
        }
        catch (System.Exception ex)
        {
            throw new DataAccessException(
                $"An error occurred finding due guest pre-arrival emails with the procedure {procedure}", ex);
        }
    }

    public PropertyGuestEmailTemplateData? GetTemplateByPropertyId(byte propertyId)
    {
        const string procedure = "Property_GuestPreArrivalEmail_GetByID";

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("PropertyID", propertyId);

            return QuerySingleOrDefaultProcedure<PropertyGuestEmailTemplateData>(procedure, parameters);
        }
        catch (System.Exception ex)
        {
            throw new DataAccessException(
                $"An error occurred finding guest pre-arrival email template data with the procedure {procedure}", ex);
        }
    }

    public bool TryReserveDispatch(int bookingId, string emailType, DateTime scheduledForUtc, DateTime reservationExpiresAtUtc)
    {
        const string procedure = "GuestEmailDispatch_Reserve";

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("BookingID", bookingId);
            parameters.Add("EmailType", emailType);
            parameters.Add("ScheduledForUtc", scheduledForUtc);
            parameters.Add("ReservationExpiresAtUtc", reservationExpiresAtUtc);

            return QuerySingleProcedure<bool>(procedure, parameters);
        }
        catch (System.Exception ex)
        {
            throw new DataAccessException(
                $"An error occurred reserving guest email dispatch with the procedure {procedure}", ex);
        }
    }

    public void UpdateDispatchStatus(int bookingId, string emailType, string status, string? failureMessage = null)
    {
        const string procedure = "GuestEmailDispatch_UpdateStatus";

        try
        {
            using var connection = Session.Connection;
            connection.Open();
            connection.Execute(
                procedure,
                new
                {
                    BookingID = bookingId,
                    EmailType = emailType,
                    Status = status,
                    FailureMessage = failureMessage
                },
                commandType: CommandType.StoredProcedure);
        }
        catch (System.Exception ex)
        {
            throw new DataAccessException(
                $"An error occurred updating guest email dispatch status with the procedure {procedure}", ex);
        }
    }
}
