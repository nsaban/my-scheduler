using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MyScheduler.Infrastructure.Outbox;

public sealed class NotificationDispatcherBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<NotificationDispatcherBackgroundService> logger) : BackgroundService
{
    private const int BatchSize = 20;
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // AppDbContext (and everything built on it) is scoped, so a fresh scope is created
                // per poll tick rather than resolving those dependencies once for this singleton service.
                using var scope = scopeFactory.CreateScope();
                var dispatcher = scope.ServiceProvider.GetRequiredService<NotificationDispatcher>();
                await dispatcher.DispatchPendingBatchAsync(BatchSize, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Notification dispatch batch failed.");
            }

            await Task.Delay(PollInterval, stoppingToken);
        }
    }
}
