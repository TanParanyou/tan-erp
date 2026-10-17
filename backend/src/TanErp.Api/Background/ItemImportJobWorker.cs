using TanErp.Application.Items;
using TanErp.Application.Items.Import;

namespace TanErp.Api.Background;

/// <summary>
/// Polls for queued Item/Cost import jobs and runs them one at a time through <see cref="IItemImportJobRunner"/> (which owns every rule).
/// A failed run is logged and retried on the next poll; it never stops the host. Not registered in the Test environment or when
/// ItemImportJobs:Enabled is false.
/// </summary>
public sealed class ItemImportJobWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly ItemImportJobOptions _options;
    private readonly ILogger<ItemImportJobWorker> _logger;

    public ItemImportJobWorker(IServiceScopeFactory scopes, ItemImportJobOptions options, ILogger<ItemImportJobWorker> logger)
    {
        _scopes = scopes;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var handledJob = false;
            try
            {
                using var scope = _scopes.CreateScope();
                var runner = scope.ServiceProvider.GetRequiredService<IItemImportJobRunner>();
                handledJob = await runner.RunNextAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Item import worker tick failed; it will be retried on the next poll.");
            }

            if (handledJob) continue;

            try
            {
                await Task.Delay(_options.EffectivePollInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }
}
