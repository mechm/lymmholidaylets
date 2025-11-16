using Dapper;
using LymmHolidayLets.Domain.Model.ICal.Entity;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Infrastructure.Exception;
using System.Data;

namespace LymmHolidayLets.Infrastructure.Repository.Dapper
{
    public sealed class DapperICalRepository : RepositoryBase<ICal>, IDapperICalRepository
    {
        public DapperICalRepository(DbSession session) : base(session)
        {
        }

        public IList<ICal> GetAll()
        {
            const string procedure = "ICal_GetAll";

            try
            {
                using var connection = Session.Connection;
                var results = connection.Query(procedure, Session.Transaction, commandType: CommandType.StoredProcedure);

                return results.Select(cal => new ICal(cal.ID, cal.PropertyID, cal.FriendlyName, cal.Identifier)).ToList();
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occurred finding all icals with the procedure {procedure}", ex);
            }
        }
    }
}
