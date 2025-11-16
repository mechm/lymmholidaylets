using LymmHolidayLets.Domain.ReadModel.Page;

namespace LymmHolidayLets.Domain.DataAdapter
{
    public interface IDapperPageDataAdapter
    {
        bool SiteUrlExists(string aliasTitle);
        bool SiteUrlExistsExcludingPage(string aliasTitle, int pageId);
        IEnumerable<PageSummary> GetAllSummary();
        PageDetail? GetPageByAliasTitle(string aliasTitle);
        bool PageExitsByTemplateId(int templateId);
    }
}
