using LymmHolidayLets.Domain.ReadModel.Homepage;

namespace LymmHolidayLets.Application.Interface.Query
{
    public interface IHomepageQuery
    {
        HomepageAggregate GetHomePageDetail();
        Task<HomepageAggregate> GetHomePageDetailAsync();
    }
}
