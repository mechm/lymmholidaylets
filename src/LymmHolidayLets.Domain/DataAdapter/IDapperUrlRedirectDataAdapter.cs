using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.Model.UrlRedirect.ValueType;

namespace LymmHolidayLets.Domain.DataAdapter
{
    public interface IDapperUrlRedirectDataAdapter : IDapperSqlQueryBase
    {
        IEnumerable<UrlRedirect> GetAll();
    }
}
