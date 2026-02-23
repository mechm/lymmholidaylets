using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Property;
using LymmHolidayLets.Domain.Repository.EF;
using LymmHolidayLets.Domain.Model.Property.Entity;

namespace LymmHolidayLets.Application.Query
{
    public sealed class PropertyQuery : IPropertyQuery
    {
        private readonly IDapperPropertyDataAdapter _propertyDataAdapter;
        private readonly IPropertyRepositoryEF _propertyRepositoryEf;

        public PropertyQuery(IDapperPropertyDataAdapter propertyDataAdapter, IPropertyRepositoryEF propertyRepositoryEf)
        {
            _propertyDataAdapter = propertyDataAdapter;
            _propertyRepositoryEf = propertyRepositoryEf;
        }

        public PropertyBooking GetPropertyBookingById(byte propertyId)
        {
            return _propertyDataAdapter.GetPropertyBookingById(propertyId);
        }

        public PropertyDetailAggregate? GetPropertyDetailById(byte propertyId) 
        {
            return _propertyDataAdapter.GetPropertyDetailById(propertyId);
        }

        public PropertyCheckInCheckOutTime? GetPropertyCheckInCheckOutTime(byte propertyId)
        {
            return _propertyDataAdapter.GetPropertyCheckInCheckOutTime(propertyId);
        }

        // EF-based surface for GraphQL

        public IQueryable<PropertyEF> GetPropertyByIdEf(byte id)
        {
            return _propertyRepositoryEf.GetPropertyById(id);
        }
    }
}