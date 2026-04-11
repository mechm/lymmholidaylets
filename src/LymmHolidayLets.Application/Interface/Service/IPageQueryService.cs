using LymmHolidayLets.Domain.ReadModel.Page;

namespace LymmHolidayLets.Application.Interface.Service;

public interface IPageQueryService
{
    Task<PageDetail?> GetVisiblePageByAliasAsync(string alias, CancellationToken cancellationToken = default);
}
