using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Domain.ReadModel.Page;
using Microsoft.Extensions.Logging;

namespace LymmHolidayLets.Application.Service;

public sealed class PageQueryService(
    IApplicationCache cache,
    IPageQuery pageQuery,
    ILogger<PageQueryService> logger) : IPageQueryService
{
    public async Task<PageDetail?> GetVisiblePageByAliasAsync(string alias, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"page-detail-{alias}";

        if (cache.TryGetValue(cacheKey, out PageDetail? page))
        {
            return page is { Visible: true } ? page : null;
        }

        logger.LogInformation("Page cache miss for AliasTitle={AliasTitle}. Fetching from database.", alias);
        page = await pageQuery.GetPageByAliasTitleAsync(alias);

        if (page is not null && page.Visible)
        {
            cache.SetAbsolute(cacheKey, page, TimeSpan.FromHours(24));
        }

        return page is { Visible: true } ? page : null;
    }
}
