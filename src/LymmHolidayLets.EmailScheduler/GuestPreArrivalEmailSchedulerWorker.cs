using Microsoft.Extensions.Options;

namespace LymmHolidayLets.EmailScheduler;

public sealed class GuestPreArrivalEmailSchedulerWorker(
    ILogger<GuestPreArrivalEmailSchedulerWorker> logger,
    IServiceScopeFactory scopeFactory,
    IOptions<GuestPreArrivalEmailSchedulerOptions> options) : BackgroundService
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(options.Value.IntervalMinutes));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Guest pre-arrival email scheduler starting with {IntervalMinutes} minute interval",
            options.Value.IntervalMinutes);

        await RunCycleAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested && await _timer.WaitForNextTickAsync(stoppingToken))
        {
            await RunCycleAsync(stoppingToken);
        }
    }

    private async Task RunCycleAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var schedulerService = scope.ServiceProvider.GetRequiredService<GuestPreArrivalEmailSchedulerService>();

            await schedulerService.ProcessDueEmailsAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Guest pre-arrival email scheduler cycle cancelled");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while processing guest pre-arrival email schedule");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Guest pre-arrival email scheduler stopping");
        _timer.Dispose();
        await base.StopAsync(cancellationToken);
    }
}
