﻿using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Model.Page.Entity;
using LymmHolidayLets.Domain.ReadModel.Page;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Domain.Repository.EF;

namespace LymmHolidayLets.Application.Query
{
    public sealed class PageQuery(
        IDapperPageDataAdapter pageDataAdapter,
        IDapperPageRepository pageRepository,
        IPageRepositoryEF pageRepositoryEf)
        : IPageQuery
    {
        public Page? GetById(int id)
        {
            return pageRepository.GetById(id);
        }

        public IEnumerable<PageSummary> GetAllPageSummary()
        {
            return pageDataAdapter.GetAllSummary();
        }

        public PageDetail? GetPageByAliasTitle(string aliasTitle)
        {
            return pageDataAdapter.GetPageByAliasTitle(aliasTitle);
        }

        public Task<PageDetail?> GetPageByAliasTitleAsync(string aliasTitle)
        {
            return pageDataAdapter.GetPageByAliasTitleAsync(aliasTitle);
        }

        public bool SiteUrlExists(string aliasTitle)
        {
            return pageDataAdapter.SiteUrlExists(aliasTitle);
        }

        public bool SiteUrlExistsExcludingPage(string aliasTitle, int pageId)
        {
            return pageDataAdapter.SiteUrlExistsExcludingPage(aliasTitle, pageId);
        }

        public bool PageExitsByTemplateId(int templateId)
        {
            return pageDataAdapter.PageExitsByTemplateId(templateId);
        }

        // EF-based query surface for GraphQL

        public IQueryable<PageEF> GetPageByIdEf(int id)
        {
            return pageRepositoryEf.GetPageById(id);
        }
    }
}
