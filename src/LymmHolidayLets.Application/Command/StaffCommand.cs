using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class StaffCommand : IStaffCommand
    {
        private readonly IDapperStaffRepository _staffRepository;

        public StaffCommand(IDapperStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }
        public void Create(Staff staff)
        {
            _staffRepository.Create(new Domain.Model.Staff.Entity.Staff(
             staff.Name, staff.YearsExperience, staff.JobTitle,
             staff.ProfileBio, staff.LinkedInLink, staff.ImagePath,
             staff.Visible));
        }

        public void Update(Staff staff)
        {
            _staffRepository.Update(new Domain.Model.Staff.Entity.Staff(
             staff.ID, staff.Name, staff.YearsExperience, staff.JobTitle,
             staff.ProfileBio, staff.LinkedInLink, staff.ImagePath, 
             staff.Visible));
        }

        public void Delete(byte id)
        {
            _staffRepository.Delete(id);
        }
    }
}