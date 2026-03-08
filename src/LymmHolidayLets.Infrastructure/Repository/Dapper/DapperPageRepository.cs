using Dapper;
using LymmHolidayLets.Domain.Model.Page.Entity;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Infrastructure.Exception;
using System.Data;

namespace LymmHolidayLets.Infrastructure.Repository.Dapper
{
    public sealed class DapperPageRepository(DbSession session) : RepositoryBase<Page>(session), IPageRepository
    {
        public IEnumerable<Page> GetAll()
        {
            const string procedure = "Page_GetAll";

            try
            {
                using var connection = Session.Connection;
                var results = connection.Query(procedure,
                        commandType: CommandType.StoredProcedure);

                return results.Select(page => new Page(page.PageId, page.AliasTitle, 
                    page.MetaDescription, page.Title, page.MainImage, page.MainImageAlt, 
                    page.Description, new Template(page.TemplateId, page.TemplateDescription), page.Visible)).ToList();
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding all pages with the procedure {procedure}", ex);
            }
        }

        public Page? GetById(int id)
        {
            const string procedure = "Page_GetById";

            try
            {
                using var connection = Session.Connection;
                var result = connection.QueryFirstOrDefault(procedure, new
                {
                    PageId = id
                },
                commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                var page = new Page(result.PageId, result.AliasTitle, result.MetaDescription,
                    result.Title, result.MainImage, result.MainImageAlt, result.Description,
                    new Template(result.TemplateId, result.TemplateDescription), result.Visible);

                return page;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding a page with the procedure {procedure}", ex);
            }
        }

        public void Create(Page page)
        {
            const string procedure = "Page_Insert";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                    {
                        page.AliasTitle,
                        page.MetaDescription,
                        page.Title,
                        page.MainImage,
                        page.MainImageAlt,
                        page.Description,
                        page.Template.TemplateId,
                        page.Visible
                    },
                   commandType: CommandType.StoredProcedure);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured creating a page with the procedure {procedure}", ex);
            }
        }

        public void Update(Page page)
        {
            const string procedure = "Page_Update";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure,
                new
                {
                    page.PageId,
                    page.AliasTitle,
                    page.MetaDescription,
                    page.Title,
                    page.MainImage,
                    page.MainImageAlt,
                    page.Description,
                    page.Template.TemplateId,
                    page.Visible
                },
                commandType: CommandType.StoredProcedure);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured updating a page with the procedure {procedure}", ex);
            }
        }

        public void Delete(int id)
        {
            const string procedure = "Page_Delete";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    PageId = id
                }, commandType: CommandType.StoredProcedure);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured deleting a page with the procedure {procedure}", ex);
            }
        }
    }
}
