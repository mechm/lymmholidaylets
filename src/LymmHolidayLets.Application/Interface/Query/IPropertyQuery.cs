using LymmHolidayLets.Domain.ReadModel.Property;

namespace LymmHolidayLets.Application.Interface.Query
{
    public interface IPropertyQuery
    {
        PropertyBooking GetPropertyBookingById(byte propertyId);
        PropertyDetailAggregate? GetPropertyDetailById(byte propertyId);
        PropertyCheckInCheckOutTime? GetPropertyCheckInCheckOutTime(byte propertyId);
    }
}