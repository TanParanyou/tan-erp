using Microsoft.EntityFrameworkCore;
using TanErp.Application.Reporting;

namespace TanErp.Infrastructure.Persistence.Reporting;

public sealed class ReportingMaterializedViewRefresher
{
    private readonly AppDbContext _db;

    public ReportingMaterializedViewRefresher(AppDbContext db)
    {
        _db = db;
    }

    public async Task RefreshAllAsync(CancellationToken ct)
    {
        foreach (var view in ReportingMaterializedViewOptions.ViewNames)
        {
            // View names come from a fixed allowlist, never from input, so the interpolation cannot inject.
#pragma warning disable EF1002
            await _db.Database.ExecuteSqlRawAsync($"REFRESH MATERIALIZED VIEW CONCURRENTLY {view}", ct);
#pragma warning restore EF1002
        }
    }
}
