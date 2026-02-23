using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Model.Page.Entity;
using LymmHolidayLets.Domain.ReadModel.Page;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Domain.Repository.EF;

namespace LymmHolidayLets.Application.Query
{
    public sealed class PageQuery : IPageQuery
    {
        private readonly IDapperPageDataAdapter _pageDataAdapter;
        private readonly IDapperPageRepository _pageRepository;
        private readonly IPageRepositoryEF _pageRepositoryEf;

        public PageQuery(IDapperPageDataAdapter pageDataAdapter,
                IDapperPageRepository pageRepository,
                IPageRepositoryEF pageRepositoryEf)
        {
            _pageDataAdapter = pageDataAdapter;
            _pageRepository = pageRepository;
            _pageRepositoryEf = pageRepositoryEf;
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

        // EF-based query surface for GraphQL

        public IQueryable<PageEF> GetPageByIdEf(int id)
        {
            return _pageRepositoryEf.GetPageById(id);
        }
    }
}
