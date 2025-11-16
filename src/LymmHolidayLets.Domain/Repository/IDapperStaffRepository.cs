using LymmHolidayLets.Domain.Model.Staff.Entity;

namespace LymmHolidayLets.Domain.Repository
{
    public interface IDapperStaffRepository : IDapperRepository<Staff>
    {
        Staff? GetById(byte id);
        IEnumerable<Staff> GetAll();
        void Create(Staff staff);
        void Update(Staff staff);
        void Delete(byte id);
    }
}