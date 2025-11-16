using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Property;

namespace LymmHolidayLets.Application.Query
{
    public sealed class PropertyQuery : IPropertyQuery
    {
        private readonly IDapperPropertyDataAdapter _propertyDataAdapter;
        public PropertyQuery(IDapperPropertyDataAdapter propertyDataAdapter)
        {
            _propertyDataAdapter = propertyDataAdapter;
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
    }
}