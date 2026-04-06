namespace LymmHolidayLets.Application.Interface.Service
{
    public interface IPropertyCacheInvalidator
    {
        void Invalidate(byte propertyId);
    }
}
