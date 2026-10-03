using Microsoft.Extensions.Diagnostics.HealthChecks;
using TanErp.Infrastructure.Persistence;

namespace TanErp.Api.Health;

/// <summary>Readiness check: PostgreSQL must accept a connection within a short timeout.</summary>
public sealed class DatabaseHealthCheck : IHealthCheck
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(3);

    private readonly AppDbContext _db;

    public DatabaseHealthCheck(AppDbContext db)
    {
        _db = db;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(Timeout);

        try
        {
            // The result carries no description or exception: connection strings and errors must not reach anonymous callers.
            return await _db.Database.CanConnectAsync(timeout.Token)
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy();
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return HealthCheckResult.Unhealthy();
        }
    }
}
