using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace LymmHolidayLets.CalendarMaintenanceWorker;

/// <summary>
/// Lightweight worker that runs the Calendar_DateRange stored procedure once daily at 2 AM UTC.
/// This procedure rebuilds the calendar date range table and populates property availability.
/// </summary>
public sealed class Worker(
    ILogger<Worker> logger,
    IConfiguration configuration) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Calendar Maintenance Worker starting - runs daily at 2 AM UTC");

        // Run immediately on startup
        await RunMaintenanceAsync(stoppingToken);

        // Then run once per day at 2 AM UTC
        while (!stoppingToken.IsCancellationRequested)
        {
            var nextRun = CalculateNextRunTime();
            var delay = nextRun - DateTime.UtcNow;

            logger.LogInformation("Next maintenance scheduled for {NextRun} UTC (in {Hours:F1} hours)",
                nextRun, delay.TotalHours);

            try
            {
                await Task.Delay(delay, stoppingToken);
                await RunMaintenanceAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Calendar Maintenance Worker stopping");
                break;
            }
        }
    }

    private async Task RunMaintenanceAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Starting calendar maintenance - executing Calendar_DateRange procedure");
            var startTime = DateTime.UtcNow;

            var connectionString = configuration.GetConnectionString("LymmHolidayLets")
                ?? throw new InvalidOperationException("Connection string 'LymmHolidayLets' not found");

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            await connection.ExecuteAsync(
                "Calendar_DateRange",
                commandType: CommandType.StoredProcedure,
                commandTimeout: 120);

            var duration = DateTime.UtcNow - startTime;
            logger.LogInformation("Calendar maintenance completed in {DurationSeconds:F1}s", duration.TotalSeconds);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Calendar maintenance cancelled");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during calendar maintenance - will retry at next scheduled time");
        }
    }

    private static DateTime CalculateNextRunTime()
    {
        var now = DateTime.UtcNow;
        var today2Am = now.Date.AddHours(2);

        // If it's already past 2 AM today, schedule for 2 AM tomorrow
        return now >= today2Am ? today2Am.AddDays(1) : today2Am;
    }
}
