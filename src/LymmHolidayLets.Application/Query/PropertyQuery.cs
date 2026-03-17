using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Property;
using LymmHolidayLets.Domain.Repository.EF;
using LymmHolidayLets.Domain.Model.Property.Entity;

namespace LymmHolidayLets.Application.Query
{
    public sealed class PropertyQuery(
        IDapperPropertyDataAdapter propertyDataAdapter,
        IPropertyRepositoryEF propertyRepositoryEf)
        : IPropertyQuery
    {
        public PropertyBooking GetPropertyBookingById(byte propertyId)
        {
            return propertyDataAdapter.GetPropertyBookingById(propertyId);
        }

        public PropertyDetailAggregate? GetPropertyDetailById(byte propertyId) 
        {
            return propertyDataAdapter.GetPropertyDetailById(propertyId);
        }

        public PropertyCheckInCheckOutTime? GetPropertyCheckInCheckOutTime(byte propertyId)
        {
            return propertyDataAdapter.GetPropertyCheckInCheckOutTime(propertyId);
        }

        // EF-based surface for GraphQL

        public IQueryable<PropertyEF> GetPropertyByIdEf(byte id)
        {
            return propertyRepositoryEf.GetPropertyById(id);
        }
    }
}
