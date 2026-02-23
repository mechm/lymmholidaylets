using LymmHolidayLets.Domain.ReadModel.Property;
using LymmHolidayLets.Domain.Model.Property.Entity;

namespace LymmHolidayLets.Application.Interface.Query
{
    public interface IPropertyQuery
    {
        PropertyBooking GetPropertyBookingById(byte propertyId);
        PropertyDetailAggregate? GetPropertyDetailById(byte propertyId);
        PropertyCheckInCheckOutTime? GetPropertyCheckInCheckOutTime(byte propertyId);

        // EF-based surface for GraphQL
        IQueryable<PropertyEF> GetPropertyByIdEf(byte id);
    }
}