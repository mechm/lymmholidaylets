using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Model.Page.Entity;
using LymmHolidayLets.Domain.ReadModel.Page;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Query
{
    public sealed class PageQuery : IPageQuery
    {
        private readonly IDapperPageDataAdapter _pageDataAdapter;
        private readonly IDapperPageRepository _pageRepository;

        public PageQuery(IDapperPageDataAdapter pageDataAdapter,
                IDapperPageRepository pageRepository)
        {
            _pageDataAdapter = pageDataAdapter;
            _pageRepository = pageRepository;
        }

        public Page? GetById(int id)
        {
            return _pageRepository.GetById(id);
        }

        public IEnumerable<PageSummary> GetAllPageSummary()
        {
            return _pageDataAdapter.GetAllSummary();
        }

        public PageDetail? GetPageByAliasTitle(string aliasTitle)
        {
            return _pageDataAdapter.GetPageByAliasTitle(aliasTitle);
        }

        public bool SiteUrlExists(string aliasTitle)
        {
            return _pageDataAdapter.SiteUrlExists(aliasTitle);
        }

        public bool SiteUrlExistsExcludingPage(string aliasTitle, int pageId)
        {
            return _pageDataAdapter.SiteUrlExistsExcludingPage(aliasTitle, pageId);
        }

        public bool PageExitsByTemplateId(int templateId)
        {
            return _pageDataAdapter.PageExitsByTemplateId(templateId);
        }
    }
}
