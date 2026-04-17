using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.ReadModel.Email;

namespace LymmHolidayLets.Domain.DataAdapter
{
    public interface ICustomerBookingEmailDataAdapter : IDapperSqlQueryBase
    {
        CustomerBookingEmailData? GetByPropertyId(byte propertyId);
    }
}
