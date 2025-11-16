using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Model.UrlRedirect.ValueType;

namespace LymmHolidayLets.Application.Query
{
    public sealed class UrlRedirectQuery : IUrlRedirectQuery
    {
        private readonly IDapperUrlRedirectDataAdapter _urlRedirectDataAdapter;

        public UrlRedirectQuery(IDapperUrlRedirectDataAdapter urlRedirectDataAdapter)
        {
            _urlRedirectDataAdapter = urlRedirectDataAdapter;
        }

        public IEnumerable<UrlRedirect> GetAll()
        {
            return _urlRedirectDataAdapter.GetAll();
        }
    }
}
