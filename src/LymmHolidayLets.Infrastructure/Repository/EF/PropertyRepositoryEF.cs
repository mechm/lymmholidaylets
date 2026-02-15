using LymmHolidayLets.Domain.Model.Property.Entity;
using LymmHolidayLets.Domain.Repository.EF;

namespace LymmHolidayLets.Infrastructure.Repository.EF
{
    public class PropertyRepositoryEF(AppDbContext context) : IPropertyRepositoryEF
    {
        private readonly AppDbContext _context = context;

        public IQueryable<PropertyEF> GetBaseQuery()
        {
            // Important: return raw IQueryable for HotChocolate to apply filters/projections
            return _context.Set<PropertyEF>();
        }

        public IQueryable<PropertyEF> GetPropertyById(byte id)
        {
            return _context.Set<PropertyEF>().Where(x => x.ID == id);
        }
    }
}
