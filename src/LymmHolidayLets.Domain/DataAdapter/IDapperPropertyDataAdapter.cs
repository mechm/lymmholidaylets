using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.ReadModel.Property;

namespace LymmHolidayLets.Domain.DataAdapter
{
    public interface IDapperPropertyDataAdapter : IDapperSqlQueryBase
    {
        PropertyBooking GetPropertyBookingById(byte propertyId);
        PropertyDetailAggregate? GetPropertyDetailById(byte propertyId);
        Task<PropertyDetailAggregate?> GetPropertyDetailByIdAsync(byte propertyId);
        PropertyCheckInCheckOutTime? GetPropertyCheckInCheckOutTime(byte propertyId);
    }
}
