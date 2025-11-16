using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.ReadModel.Homepage;

namespace LymmHolidayLets.Domain.DataAdapter
{
    public interface IDapperHomepageDataAdapter : IDapperSqlQueryBase
    {
        HomepageAggregate GetHomePageDetail();
    }
}
