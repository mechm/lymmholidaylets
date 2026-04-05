using LymmHolidayLets.CalendarImporter.Interfaces;
using Dapper;
using System.Data;

namespace LymmHolidayLets.CalendarImporter.Infrastructure;

public sealed class CalendarDataAdapter(DbSession session) : ICalendarDataAdapter
{
    public void BlockCalendarByPropertyForDate(int propertyId, DateOnly startDate, DateOnly endDate)
    {
        const string procedure = "Calendar_Update_Property_Date";

        try
        {
            using var connection = session.Connection;
            connection?.Execute(procedure, new
            {
                propertyId,
                startDate,
                endDate
            },
            session.Transaction,
            commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"An error occurred updating calendar by property id {propertyId} and dates {startDate} to {endDate}",
                ex);
        }
    }
}
