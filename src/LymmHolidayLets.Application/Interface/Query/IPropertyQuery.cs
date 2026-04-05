using LymmHolidayLets.Application.Model.Property;
using LymmHolidayLets.Domain.Model.Property.Entity;
using LymmHolidayLets.Domain.ReadModel.Property;

namespace LymmHolidayLets.Application.Interface.Query
{
    public interface IPropertyQuery
    {
        PropertyBooking GetPropertyBookingById(byte propertyId);
        PropertyDetailAggregate? GetPropertyDetailById(byte propertyId);
        Task<PropertyDetailResult?> GetPropertyDetailByIdAsync(byte propertyId);
        PropertyCheckInCheckOutTime? GetPropertyCheckInCheckOutTime(byte propertyId);

        // EF-based surface for GraphQL
        IQueryable<PropertyEF> GetPropertyByIdEf(byte id);
    }
}