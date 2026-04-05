using LymmHolidayLets.CalendarImporter.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace LymmHolidayLets.CalendarImporter.Infrastructure;

public sealed class DatabaseFactory(IConfiguration configuration) : IDatabaseFactory
{
    private readonly string _connectionString = configuration.GetConnectionString("LymmHolidayLets")
                                                ?? throw new InvalidOperationException("Connection string 'LymmHolidayLets' not found");

    public IDbConnection GetConnection => new SqlConnection(_connectionString);
}
