using LymmHolidayLets.Domain.Model.Staff.Entity;

namespace LymmHolidayLets.Application.Interface.Query
{
    public interface IStaffQuery
    {
        Staff? GetById(byte id);
        IEnumerable<Staff> GetAll();
    }
}
