using Dapper;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.ReadModel.Email;
using LymmHolidayLets.Infrastructure.Exception;
using LymmHolidayLets.Infrastructure.Repository;
using System.Data;

namespace LymmHolidayLets.Infrastructure.DataAdapter
{
    public sealed class DapperCustomerBookingEmailDataAdapter(DbSession session)
        : SqlQueryBase(session), ICustomerBookingEmailDataAdapter
    {
        public CustomerBookingEmailData? GetByPropertyId(byte propertyId)
        {
            const string procedure = "Property_BookingConfirmationCustomer_GetByID";

            try
            {
                using var sqlConnection = Session.Connection;
                using var result = sqlConnection.QueryMultiple(
                    procedure,
                    new { PropertyID = propertyId },
                    Session.Transaction,
                    commandType: CommandType.StoredProcedure);

                var property = result.ReadSingleOrDefault<CustomerBookingEmailData>();
                if (property is null)
                {
                    return null;
                }

                return new CustomerBookingEmailData
                {
                    PropertyId = property.PropertyId,
                    PropertyName = property.PropertyName,
                    Bedroom = property.Bedroom,
                    Bathroom = property.Bathroom,
                    CheckInTimeAfter = property.CheckInTimeAfter,
                    CheckOutTimeBefore = property.CheckOutTimeBefore,
                    AddressLineOne = property.AddressLineOne,
                    AddressLineTwo = property.AddressLineTwo,
                    TownOrCity = property.TownOrCity,
                    County = property.County,
                    Postcode = property.Postcode,
                    Country = property.Country,
                    DirectionsUrl = property.DirectionsUrl,
                    ArrivalInstructions = property.ArrivalInstructions,
                    HeroImagePath = property.HeroImagePath,
                    HeroImageAltText = property.HeroImageAltText,
                    HouseRules = result.Read<CustomerBookingEmailTextItem>().ToList(),
                    SafetyItems = result.Read<CustomerBookingEmailTextItem>().ToList(),
                    CancellationPolicies = result.Read<CustomerBookingEmailCancellationPolicy>().ToList()
                };
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException(
                    $"An error occurred finding booking confirmation email content with the procedure {procedure}", ex);
            }
        }
    }
}
