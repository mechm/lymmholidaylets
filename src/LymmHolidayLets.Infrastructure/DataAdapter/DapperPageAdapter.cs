using LymmHolidayLets.Infrastructure.Exception;
using System.Data;
using Dapper;
using LymmHolidayLets.Infrastructure.Repository;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Page;

namespace LymmHolidayLets.Infrastructure.DataAdapter
{
    public sealed class DapperPageDataAdapter(DbSession session) : SqlQueryBase(session), IDapperPageDataAdapter
    {
        public bool SiteUrlExists(string aliasTitle)
        {
            const string procedure = "Page_SiteAliasTitle_Exists";

            try
            {
                using var sqlConnection = _session.Connection;
                var pageExists = sqlConnection.ExecuteScalar<bool>(procedure, new
                    {
                        aliasTitle,
                    }, _session.Transaction,
                    commandType: CommandType.StoredProcedure);

                return pageExists;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding whether a page exists with the procedure {procedure}", ex);
            }
        }

        public bool SiteUrlExistsExcludingPage(string aliasTitle, int pageId)
        {
            const string procedure = "Page_SiteAliasTitle_Exists_ExcludePage";

            try
            {
                using var connection = _session.Connection;
                var pageExists = connection.ExecuteScalar<bool>(procedure, new
                    {
                        AliasTitle = aliasTitle,
                        PageId = pageId
                    },
                    commandType: CommandType.StoredProcedure);              

                return pageExists;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding whether a page exists exluding current page with the procedure {procedure}", ex);
            }
        }

        public IEnumerable<PageSummary> GetAllSummary()
        {
            const string procedure = "Page_GetAllSummary";

            try
            {
                using var connection = _session.Connection;
                var results = connection.Query(procedure,
                                commandType: CommandType.StoredProcedure);

                return results.Select(page => new PageSummary(page.PageId, page.AliasTitle, page.Title, page.TemplateDescription, page.Visible)).ToList();
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding all pages with the procedure {procedure}", ex);
            }
        }

        public PageDetail? GetPageByAliasTitle(string aliasTitle)
        {
            const string procedure = "Page_GetByAliasTitle";

            try
            {
                PageDetail? page = null;

                using var connection = _session.Connection;
                var result = connection.QueryFirstOrDefault(procedure, new
                {
                    AliasTitle = aliasTitle
                },
                commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    page = new PageDetail(result.AliasTitle, result.MetaDescription,
                        result.Title, result.MainImage, result.MainImageAlt, result.Description,
                        result.Template, result.Visible);
                }               

                return page;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding a page with the procedure {procedure}", ex);
            }
        }

        public async Task<PageDetail?> GetPageByAliasTitleAsync(string aliasTitle)
        {
            const string procedure = "Page_GetByAliasTitle";

            try
            {
                using var connection = _session.Connection;
                var result = await connection.QueryFirstOrDefaultAsync(procedure, new
                {
                    AliasTitle = aliasTitle
                },
                commandType: CommandType.StoredProcedure);

                if (result is null)
                    return null;

                return new PageDetail(result.AliasTitle, result.MetaDescription,
                    result.Title, result.MainImage, result.MainImageAlt, result.Description,
                    result.Template, result.Visible);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding a page with the procedure {procedure}", ex);
            }
        }

        public bool PageExitsByTemplateId(int templateId)
        {
            const string procedure = "Page_Exists_GetByTemplateId";

            try
            {
                using var connection = _session.Connection;
                var pageExists = connection.ExecuteScalar<bool>(procedure, new
                    {
                        templateId
                    },
                    commandType: CommandType.StoredProcedure);              

                return pageExists;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException(
                    $"An error occured finding whether a page exists with the procedure {procedure}", ex);
            }
        }
    }
}
