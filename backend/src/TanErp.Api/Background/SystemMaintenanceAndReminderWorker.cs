using TanErp.Application.Notifications;

namespace TanErp.Api.Background;

public sealed class SystemMaintenanceAndReminderWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly SystemMaintenanceAndReminderOptions _options;
    private readonly ILogger<SystemMaintenanceAndReminderWorker> _logger;

    public SystemMaintenanceAndReminderWorker(
        IServiceScopeFactory scopes,
        SystemMaintenanceAndReminderOptions options,
        ILogger<SystemMaintenanceAndReminderWorker> logger)
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
        }
        catch (OperationCanceledException)
        {
            return;
        }

        using var timer = new PeriodicTimer(_options.EffectiveInterval);
        do
        {
            await RunTickAsync(stoppingToken);
        }
        while (await WaitForNextTickAsync(timer, stoppingToken));
    }

    private static async Task<bool> WaitForNextTickAsync(PeriodicTimer timer, CancellationToken ct)
    {
        try
        {
            return await timer.WaitForNextTickAsync(ct);
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    public async Task RunTickAsync(CancellationToken ct)
    {
        try
        {
            using var scope = _scopes.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ISystemMaintenanceAndReminderService>();
            var result = await service.RunOnceAsync(ct);
            _logger.LogInformation(
                "System maintenance & reminder run finished: SLA scanned {SlaScanned} (created {SlaCreated}), warranties scanned {WarrantiesScanned} (created {WarrantyCreated}), expired sessions deleted {SessionsDeleted}, orphan files deleted {OrphansDeleted}, expired audit events deleted {AuditsDeleted}, skipped (locked) {Skipped}",
                result.SlaScanned,
                result.SlaNotificationsCreated,
                result.WarrantiesScanned,
                result.WarrantyNotificationsCreated,
                result.ExpiredSessionsDeleted,
                result.OrphanFilesDeleted,
                result.ExpiredAuditEventsDeleted,
                result.SkippedBecauseLocked);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Shutting down.
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "System maintenance & reminder run failed; it will be retried on the next tick.");
        }
    }
}
