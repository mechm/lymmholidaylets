using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Service;
using LymmHolidayLets.Application.Service;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Dto.Email;
using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.NotificationWorker.Consumers;
using LymmHolidayLets.Infrastructure;
using LymmHolidayLets.Infrastructure.Dapper;
using LymmHolidayLets.Infrastructure.DataAdapter;
using LymmHolidayLets.Infrastructure.Emailer;
using LymmHolidayLets.Infrastructure.Repository;
using LymmHolidayLets.Infrastructure.Services;
using Dapper;
using MassTransit;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting LymmHolidayLets.NotificationWorker");

try
{
    // Register Dapper type handlers before any DB access
    SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());
    SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

    var builder = WebApplication.CreateBuilder(args);

    // Load local overrides (credentials, local connection strings). File is git-ignored.
    builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false);

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

    var config = builder.Configuration;

    // Email infrastructure — uses generic SMTP (smtp2go) via MailKit; no SendGrid dependency.
    builder.Services.AddTransient<IEmailService, EmailService>();
    builder.Services.AddTransient<IEmailTemplateBuilder, EmailTemplateBuilder>();
    builder.Services.AddTransient<IEmailGeneratorService, EmailGeneratorService>();
    builder.Services.AddTransient<ICustomerBookingConfirmationBuilder, CustomerBookingConfirmationBuilder>();

    // Data access
    builder.Services.AddTransient<IDatabaseFactory, DatabaseFactory>();
    builder.Services.AddTransient<DbSession>();
    builder.Services.AddTransient<ICustomerBookingEmailDataAdapter, DapperCustomerBookingEmailDataAdapter>();
    builder.Services.AddTransient<IDapperPriceDataAdapter, DapperPriceDataAdapter>();

    // SMS infrastructure
    builder.Services.Configure<TwilioOptions>(
        options => config.GetSection("Twilio").Bind(options));
    builder.Services.AddTransient<ITextMessageService, TextMessageService>();

    // Configuration bindings
    builder.Services.Configure<SmtpConfig>(options => config.GetSection("SmtpConfig").Bind(options));
    builder.Services.Configure<EmailOptions>(options => config.GetSection("Email").Bind(options));

    // MassTransit with RabbitMQ
    builder.Services.AddMassTransit(x =>
    {
        // Retry with exponential back-off before the message is dead-lettered.
        // Email sending is susceptible to transient failures (SendGrid rate limits,
        // brief network issues) that typically resolve within a few seconds.
        // After 3 retries the message is moved to the *_error queue for inspection.
        x.AddConsumer<EmailEnquiryConsumer>(c =>
            c.UseMessageRetry(r =>
                r.Exponential(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5))));

        // Split into two independent consumers so a failure sending one email
        // does not trigger a retry of the other, which would cause duplicate sends.
        // RabbitMQ fans out each BookingNotificationRequested event to both queues independently.
        x.AddConsumer<BookingConfirmedToCompanyConsumer>(c =>
            c.UseMessageRetry(r =>
                r.Exponential(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5))));

        x.AddConsumer<BookingConfirmedToCustomerConsumer>(c =>
            c.UseMessageRetry(r =>
                r.Exponential(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5))));

        // SMS notifications with same retry pattern as email
        x.AddConsumer<BookingNotificationSmsConsumer>(c =>
            c.UseMessageRetry(r =>
                r.Exponential(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5))));

        x.UsingRabbitMq((ctx, cfg) =>
        {
            cfg.Host(config["RabbitMQ:Host"] ?? "localhost", "/", h =>
            {
                h.Username(config["RabbitMQ:Username"] ?? "guest");
                h.Password(config["RabbitMQ:Password"] ?? "guest");
            });

            cfg.ConfigureEndpoints(ctx);
        });
    });

    var app = builder.Build();

    await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "LymmHolidayLets.NotificationWorker terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
