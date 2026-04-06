using LymmHolidayLets.CalendarImporter.Interfaces;
using System.Data;

namespace LymmHolidayLets.CalendarImporter.Infrastructure;

public sealed class DbSession : IDisposable
{
    public IDbConnection? Connection { get; }
    public IDbTransaction? Transaction { get; set; }

    public DbSession(IDatabaseFactory databaseFactory)
    {
        Connection = databaseFactory.GetConnection;
        Connection.Open();
    }

    public void Dispose()
    {
        Transaction?.Dispose();
        Connection?.Dispose();
    }
}
