using Dapper;
using LymmHolidayLets.Domain.Model.Template;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Infrastructure.Exception;
using System.Data;

namespace LymmHolidayLets.Infrastructure.Repository.Dapper
{
    public sealed class DapperTemplateRepository : RepositoryBase<Template>, IDapperTemplateRepository
    {
        public DapperTemplateRepository(DbSession session) : base(session)
        {
        }

        public IEnumerable<Template> GetAll()
        {
            const string procedure = "Template_GetAll";

            try
            {
                IList<Template> templates = new List<Template>();

                using var connection = Session.Connection;
                var results = connection.Query(procedure,
                        commandType: CommandType.StoredProcedure);

                foreach (var page in results)
                {
                    templates.Add(new Template(page.TemplateId, page.Description));
                }               

                return templates;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding all templates with the procedure {procedure}", ex);
            }
        }

        public Template? GetById(int id)
        {
            const string procedure = "Template_GetById";

            try
            {
                Template template;

                using var connection = Session.Connection;
                var result = connection.QueryFirstOrDefault(procedure, new
                    {
                        TemplateId = id
                    },commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                template = new Template(result.TemplateId, result.Description);               

                return template;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding a template with the procedure {procedure}", ex);
            }
        }

        public void Create(Template template)
        {
            const string procedure = "Template_Insert";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    template.Description
                },commandType: CommandType.StoredProcedure);
               
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured creating a template with the procedure {procedure}", ex);
            }
        }

        public void Update(Template template)
        {
            const string procedure = "Template_Update";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure,
                new
                {
                    template.TemplateId,
                    template.Description
                },
                commandType: CommandType.StoredProcedure);
               
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured updating a template with the procedure {procedure}", ex);
            }
        }

        public void Delete(int id)
        {
            const string procedure = "Template_Delete";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    TemplateId = id
                }, commandType: CommandType.StoredProcedure);               
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured deleting a template with the procedure {procedure}", ex);
            }
        }
    }
}
