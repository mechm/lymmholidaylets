using Dapper;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.EmailScheduler;
using LymmHolidayLets.Infrastructure;
using LymmHolidayLets.Infrastructure.Dapper;
using LymmHolidayLets.Infrastructure.DataAdapter;
using LymmHolidayLets.Infrastructure.Repository;
using MassTransit;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting LymmHolidayLets.EmailScheduler");

    var builder = Host.CreateApplicationBuilder(args);

    builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false);

    builder.Services.AddSerilog();

    SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
    SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());

    builder.Services.Configure<GuestPreArrivalEmailSchedulerOptions>(
        builder.Configuration.GetSection("GuestPreArrivalEmailScheduler"));

    builder.Services.AddTransient<IDatabaseFactory, DatabaseFactory>();
    builder.Services.AddTransient<DbSession>();
    builder.Services.AddTransient<IGuestPreArrivalEmailDataAdapter, DapperGuestPreArrivalEmailDataAdapter>();
    builder.Services.AddTransient<GuestPreArrivalEmailSchedulerService>();

    builder.Services.AddMassTransit(x =>
    {
        x.UsingRabbitMq((_, cfg) =>
        {
            cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
            {
                h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
                h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
            });
        });
    });

    builder.Services.AddHostedService<GuestPreArrivalEmailSchedulerWorker>();

    var host = builder.Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "LymmHolidayLets.EmailScheduler terminated unexpectedly");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

return 0;
