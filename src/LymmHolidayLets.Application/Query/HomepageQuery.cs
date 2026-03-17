using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Homepage;

namespace LymmHolidayLets.Application.Query
{
    public sealed class HomepageQuery(IDapperHomepageDataAdapter homepageDataAdapter) : IHomepageQuery
    {
        public HomepageAggregate GetHomePageDetail() 
        { 
            return homepageDataAdapter.GetHomePageDetail();
        }

        public Task<HomepageAggregate> GetHomePageDetailAsync()
        {
            return homepageDataAdapter.GetHomePageDetailAsync();
        }
    }
}
