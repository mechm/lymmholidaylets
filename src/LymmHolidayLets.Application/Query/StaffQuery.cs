using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.Model.Staff.Entity;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Query
{
    public sealed class StaffQuery(IStaffRepository staffRepository) : IStaffQuery
    {
        public IEnumerable<Staff> GetAll()
        {
            return staffRepository.GetAll();
        }

        public Staff? GetById(byte id)
        {
            return staffRepository.GetById(id);
        }
    }
}
