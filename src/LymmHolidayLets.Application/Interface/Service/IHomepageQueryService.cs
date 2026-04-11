using LymmHolidayLets.Application.Model.Service;

namespace LymmHolidayLets.Application.Interface.Service;

public interface IHomepageQueryService
{
    Task<HomepageResult?> GetHomepageDataAsync(CancellationToken cancellationToken = default);
}
