using System.Data;

namespace LymmHolidayLets.CalendarImporter.Interfaces;

public interface IDatabaseFactory
{
    IDbConnection GetConnection { get; }
}
