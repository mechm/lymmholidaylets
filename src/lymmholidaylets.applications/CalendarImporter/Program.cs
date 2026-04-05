using LymmHolidayLets.CalendarImporter;
using LymmHolidayLets.CalendarImporter.Infrastructure;
using LymmHolidayLets.CalendarImporter.Interfaces;
using LymmHolidayLets.CalendarImporter.Providers;
using LymmHolidayLets.CalendarImporter.Services;
using Dapper;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using System.Data;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/calendar-importer-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting Calendar Importer Worker Service");

    var builder = Host.CreateApplicationBuilder(args);

    // Configure Serilog
    builder.Services.AddSerilog();

    // Register Dapper type handlers
    SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());

    // Configure HTTP client with retry policy
    builder.Services.AddHttpClient<ICalendarProvider, AirbnbCalendarProvider>()
        .AddPolicyHandler(GetRetryPolicy());

    builder.Services.AddHttpClient<ICalendarProvider, BookingCalendarProvider>()
        .AddPolicyHandler(GetRetryPolicy());

    builder.Services.AddHttpClient<ICalendarProvider, VrboCalendarProvider>()
        .AddPolicyHandler(GetRetryPolicy());

    // Register services
    builder.Services.AddTransient<DbSession>();
    builder.Services.AddTransient<IDatabaseFactory, DatabaseFactory>();
    builder.Services.AddTransient<ICalendarDataAdapter, CalendarDataAdapter>();
    builder.Services.AddSingleton<ICalendarSyncService, CalendarSyncService>();
    builder.Services.AddSingleton<ICalendarProviderFactory, CalendarProviderFactory>();

    // Register providers
    builder.Services.AddTransient<AirbnbCalendarProvider>();
    builder.Services.AddTransient<BookingCalendarProvider>();
    builder.Services.AddTransient<VrboCalendarProvider>();

    // Configure options
    builder.Services.Configure<CalendarSyncOptions>(
        builder.Configuration.GetSection("CalendarSync"));

    // Register the worker
    builder.Services.AddHostedService<CalendarSyncWorker>();

    // Add health checks
    builder.Services.AddHealthChecks();

    var host = builder.Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

return 0;

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryCount, context) =>
            {
                Log.Warning("Retry {RetryCount} after {Delay}s due to {Result}",
                    retryCount, timespan.TotalSeconds, outcome.Result?.StatusCode);
            });
}

sealed class SqlDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value.ToDateTime(TimeOnly.MinValue);
    }

    public override DateOnly Parse(object value)
    {
        return DateOnly.FromDateTime((DateTime)value);
    }
}
