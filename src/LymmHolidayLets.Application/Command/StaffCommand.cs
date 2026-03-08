using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class StaffCommand(IStaffRepository staffRepository) : IStaffCommand
    {
        public void Create(Staff staff)
        {
            staffRepository.Create(new Domain.Model.Staff.Entity.Staff(
             staff.Name, staff.YearsExperience, staff.JobTitle,
             staff.ProfileBio, staff.LinkedInLink, staff.ImagePath,
             staff.Visible));
        }

        public void Update(Staff staff)
        {
            staffRepository.Update(new Domain.Model.Staff.Entity.Staff(
             staff.ID, staff.Name, staff.YearsExperience, staff.JobTitle,
             staff.ProfileBio, staff.LinkedInLink, staff.ImagePath, 
             staff.Visible));
        }

        public void Delete(byte id)
        {
            staffRepository.Delete(id);
        }
    }
}
