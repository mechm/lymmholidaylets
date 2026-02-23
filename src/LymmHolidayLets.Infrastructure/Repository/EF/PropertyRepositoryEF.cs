using LymmHolidayLets.Domain.Model.Property.Entity;
using LymmHolidayLets.Domain.Repository.EF;

namespace LymmHolidayLets.Infrastructure.Repository.EF
{
    public class PropertyRepositoryEF(AppDbContext context) : IPropertyRepositoryEF
    {
        private readonly AppDbContext _context = context;

        public IQueryable<PropertyEF> GetPropertyById(byte id)
        {
            return _context.Property.Where(x => x.ID == id).OrderBy(x => x.ID);;
        }
    }
}
