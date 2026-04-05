using Dapper;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Page;
using LymmHolidayLets.Infrastructure.Exception;
using LymmHolidayLets.Infrastructure.Repository;
using System.Data;

namespace LymmHolidayLets.Infrastructure.DataAdapter
{
    public sealed class DapperTemplateDataAdapter(DbSession session) : SqlQueryBase(session), IDapperTemplateDataAdapter
    {
        public IEnumerable<Template> GetAllTemplateSummary()
        {
            try
            {
                return GetAll<Template>("Template");
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException(
                    "An error occured finding Template", ex);
            }
        }

        public bool TemplateItemExists(string description)
        {
            const string procedure = "Template_Description_Exists";

            try
            {
                using var connection = Session.Connection;
                var templateExists = connection.ExecuteScalar<bool>(procedure, new
                    {
                        description
                    },
                    commandType: CommandType.StoredProcedure);              

                return templateExists;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding whether a template item exists with the procedure {procedure}", ex);
            }
        }

        public bool TemplateItemExistsExcludingDescription(string description, int templateId)
        {
            const string procedure = "Template_Description_Exists_ExcludeDescription";

            try
            {
                using var connection = Session.Connection;
                var templateExists = connection.ExecuteScalar<bool>(procedure, new
                    {
                        description,
                        templateId
                    },
                    commandType: CommandType.StoredProcedure);               

                return templateExists;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding whether a template item exists excluding current template item with the procedure {procedure}", ex);
            }
        }
    }
}
