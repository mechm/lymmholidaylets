using LymmHolidayLets.Domain.Model.Property.Entity;

namespace LymmHolidayLets.Domain.Repository.EF
{
    public interface IPropertyRepositoryEF
    {
        IQueryable<PropertyEF> GetBaseQuery();
        IQueryable<PropertyEF> GetPropertyById(byte id);
    }
}
