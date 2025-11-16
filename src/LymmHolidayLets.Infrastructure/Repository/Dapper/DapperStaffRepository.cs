using Dapper;
using LymmHolidayLets.Domain.Model.Staff.Entity;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Infrastructure.Exception;
using System.Data;

namespace LymmHolidayLets.Infrastructure.Repository.Dapper
{
    public sealed class DapperStaffRepository : RepositoryBase<Staff>, IDapperStaffRepository
    {
        public DapperStaffRepository(DbSession session) : base(session)
        {
        }
        public IEnumerable<Staff> GetAll()
        {
            const string procedure = "Staff_GetAll";

            try
            {
                IList<Staff> staffs = new List<Staff>();

                using var connection = Session.Connection;
                var results = connection.Query(procedure,
                    commandType: CommandType.StoredProcedure);

                foreach (var staff in results)
                {
                    staffs.Add(new Staff(staff.ID, staff.Name, staff.YearsExperience, staff.JobTitle, 
                        staff.ProfileBio, staff.LinkedInLink, staff.ImagePath, staff.Visible));
                }

                return staffs;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding all staff with the procedure {procedure}", ex);
            }
        }

        public Staff? GetById(byte id)
        {
            const string procedure = "Staff_GetById";

            try
            {
                Staff staff;

                using var connection = Session.Connection;
                var result = connection.QueryFirstOrDefault(procedure, new
                {
                    id
                },
                commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                staff = new Staff(result.ID, result.Name, result.YearsExperience, result.JobTitle,
                        result.ProfileBio, result.LinkedInLink, result.ImagePath, result.Visible);

                return staff;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding staff with the procedure {procedure}", ex);
            }
        }

        public void Create(Staff staff)
        {
            const string procedure = "Staff_Insert";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    staff.Name,
                    staff.YearsExperience,
                    staff.JobTitle,
                    staff.ProfileBio,
                    staff.LinkedInLink,
                    staff.ImagePath,
                    staff.Visible
                },
                commandType: CommandType.StoredProcedure);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured creating staff with the procedure {procedure}", ex);
            }
        }

        public void Update(Staff staff)
        {
            const string procedure = "Staff_Update";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure,
                new
                {
                    staff.ID,
                    staff.Name,
                    staff.YearsExperience,
                    staff.JobTitle,
                    staff.ProfileBio,
                    staff.LinkedInLink,
                    staff.ImagePath,
                    staff.Visible
                },
                commandType: CommandType.StoredProcedure);

            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured updating staff with the procedure {procedure}", ex);
            }
        }

        public void Delete(byte id)
        {
            const string procedure = "Staff_Delete";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    id
                }, commandType: CommandType.StoredProcedure);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured deleting staff with the procedure {procedure}", ex);
            }
        }
    }
}