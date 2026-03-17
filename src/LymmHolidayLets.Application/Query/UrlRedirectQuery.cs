using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Model.UrlRedirect.ValueType;

namespace LymmHolidayLets.Application.Query
{
    public sealed class UrlRedirectQuery(IDapperUrlRedirectDataAdapter urlRedirectDataAdapter) : IUrlRedirectQuery
    {
        public IEnumerable<UrlRedirect> GetAll()
        {
            return urlRedirectDataAdapter.GetAll();
        }
    }
}
