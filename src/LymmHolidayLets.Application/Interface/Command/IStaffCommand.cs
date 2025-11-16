using LymmHolidayLets.Application.Model.Command;

namespace LymmHolidayLets.Application.Interface.Command
{
    public interface IStaffCommand
    {
        void Create(Staff staff);
        void Update(Staff staff);
        void Delete(byte id);
    }
}
