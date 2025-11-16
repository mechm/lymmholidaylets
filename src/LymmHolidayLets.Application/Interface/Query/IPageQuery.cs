using LymmHolidayLets.Domain.Model.Page.Entity;
using LymmHolidayLets.Domain.ReadModel.Page;

namespace LymmHolidayLets.Application.Interface.Query
{
    public interface IPageQuery
    {
        Page? GetById(int id);
        IEnumerable<PageSummary> GetAllPageSummary();
        PageDetail? GetPageByAliasTitle(string aliasTitle);
        bool SiteUrlExists(string aliasTitle);
        bool SiteUrlExistsExcludingPage(string aliasTitle, int pageId);
        bool PageExitsByTemplateId(int templateId);
    }
}
