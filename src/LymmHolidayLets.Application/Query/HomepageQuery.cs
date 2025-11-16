using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Homepage;

namespace LymmHolidayLets.Application.Query
{
    public sealed class HomepageQuery : IHomepageQuery
    {
        private readonly IDapperHomepageDataAdapter _homepageDataAdapter;

        public HomepageQuery(IDapperHomepageDataAdapter homepageDataAdapter)
        {
            _homepageDataAdapter = homepageDataAdapter;
        }

        public HomepageAggregate GetHomePageDetail() 
        { 
            return _homepageDataAdapter.GetHomePageDetail();
        }
    }
}
