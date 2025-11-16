using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.Model.Staff.Entity;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Query
{
    public sealed class StaffQuery : IStaffQuery
    {
        private IDapperStaffRepository _staffRepository;

        public StaffQuery(IDapperStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        public IEnumerable<Staff> GetAll()
        {
            return _staffRepository.GetAll();
        }

        public Staff? GetById(byte id)
        {
            return _staffRepository.GetById(id);
        }
    }
}