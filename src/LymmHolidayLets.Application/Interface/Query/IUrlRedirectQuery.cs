using LymmHolidayLets.Domain.Model.UrlRedirect.ValueType;

namespace LymmHolidayLets.Application.Interface.Query
{
    public interface IUrlRedirectQuery
    {
        IEnumerable<UrlRedirect> GetAll();
    }
}
