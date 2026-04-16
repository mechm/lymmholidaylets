using LymmHolidayLets.CalendarMaintenanceWorker;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/calendar-maintenance-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting Calendar Maintenance Worker");

    var builder = Host.CreateApplicationBuilder(args);

    // Load optional local overrides (not committed to source control).
    // appsettings.local.json overrides appsettings.json for local development.
    builder.Configuration
        .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

    // Add Serilog
    builder.Services.AddSerilog();

    // Register the worker
    builder.Services.AddHostedService<Worker>();

    var host = builder.Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Calendar Maintenance Worker terminated unexpectedly");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

return 0;
