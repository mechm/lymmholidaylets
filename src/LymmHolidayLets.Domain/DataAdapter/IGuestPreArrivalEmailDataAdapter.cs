using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.ReadModel.Email;

namespace LymmHolidayLets.Domain.DataAdapter;

public interface IGuestPreArrivalEmailDataAdapter : IDapperSqlQueryBase
{
    IReadOnlyList<GuestPreArrivalDueBooking> GetDueBookings(DateOnly? runDate = null);
    PropertyGuestEmailTemplateData? GetTemplateByPropertyId(byte propertyId);
    bool TryReserveDispatch(int bookingId, string emailType, DateTime scheduledForUtc, DateTime reservationExpiresAtUtc);
    void UpdateDispatchStatus(int bookingId, string emailType, string status, string? failureMessage = null);
}
