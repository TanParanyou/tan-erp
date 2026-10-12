using TanErp.Application.Notifications;

namespace TanErp.Api.Background;

/// <summary>
/// The schedule only: waits a startup delay, then runs one scan per interval. All rules live in IMembershipExpiryReminderService
/// (which tests call directly with a fake clock). A failed run is logged and retried on the next tick; it never stops the host.
/// Not registered in the Test environment or when MembershipExpiryReminder:Enabled is false.
/// </summary>
public sealed class MembershipExpiryReminderWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly MembershipExpiryReminderOptions _options;
    private readonly ILogger<MembershipExpiryReminderWorker> _logger;

    public MembershipExpiryReminderWorker(
        IServiceScopeFactory scopes, MembershipExpiryReminderOptions options, ILogger<MembershipExpiryReminderWorker> logger)
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

    /// <summary>One scan in its own DI scope. Public so a test can drive a tick without waiting for the timer.</summary>
    public async Task RunTickAsync(CancellationToken ct)
    {
        try
        {
            using var scope = _scopes.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IMembershipExpiryReminderService>();
            var result = await service.RunOnceAsync(ct);
            _logger.LogInformation(
                "Membership expiry reminder run finished: scanned {Scanned}, created {Created}, failed {Failed}, skipped (locked) {Skipped}",
                result.Scanned, result.Created, result.Failed, result.SkippedBecauseLocked);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Shutting down.
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Membership expiry reminder run failed; it will be retried on the next tick.");
        }
    }
}
