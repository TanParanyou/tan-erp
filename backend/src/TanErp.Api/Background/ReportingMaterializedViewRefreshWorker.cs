using TanErp.Application.Reporting;
using TanErp.Infrastructure.Persistence.Reporting;

namespace TanErp.Api.Background;

/// <summary>
/// Refreshes the reporting materialized views on a fixed interval (VQ-RPT-01). A failed refresh is logged and retried on the next tick; the
/// previous snapshot keeps serving because REFRESH ... CONCURRENTLY never blocks readers. Not registered in Test or when disabled.
/// </summary>
public sealed class ReportingMaterializedViewRefreshWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly ReportingMaterializedViewOptions _options;
    private readonly ILogger<ReportingMaterializedViewRefreshWorker> _logger;

    public ReportingMaterializedViewRefreshWorker(
        IServiceScopeFactory scopes, ReportingMaterializedViewOptions options, ILogger<ReportingMaterializedViewRefreshWorker> logger)
    {
        _scopes = scopes;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(_options.EffectiveStartupDelay, stoppingToken);
            using var timer = new PeriodicTimer(_options.EffectiveInterval);
            do
            {
                await RunTickAsync(stoppingToken);
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException)
        {
            // Shutting down.
        }
    }

    public async Task RunTickAsync(CancellationToken ct)
    {
        try
        {
            using var scope = _scopes.CreateScope();
            var refresher = scope.ServiceProvider.GetRequiredService<ReportingMaterializedViewRefresher>();
            await refresher.RefreshAllAsync(ct);
            _logger.LogInformation("Reporting materialized views refreshed.");
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Shutting down.
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Reporting materialized view refresh failed; the previous snapshot stays in use.");
        }
    }
}
