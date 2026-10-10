using Microsoft.EntityFrameworkCore;
using TanErp.Infrastructure.Persistence;
using TanErp.Infrastructure.Persistence.OrganizationAdministration;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

/// <summary>
/// Model-only check (no database). Every entity that carries a BranchId must be either counted as open work or
/// excluded with a written reason, so a new branch-scoped document cannot silently bypass the deactivation guard.
/// </summary>
public class BranchDependencyInspectorCoverageTests
{
    [Fact]
    public void EveryEntityWithBranchId_IsEitherCountedOrExcludedWithAReason()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql("Host=localhost;Database=model-only").Options;
        using var db = new AppDbContext(options);

        var withBranch = db.Model.GetEntityTypes()
            .Where(e => e.FindProperty("BranchId") is not null)
            .Select(e => e.ClrType)
            .ToHashSet();

        var classified = BranchDependencyInspector.CountedEntities.Keys
            .Concat(BranchDependencyInspector.ExcludedEntities.Keys)
            .ToHashSet();

        var unclassified = withBranch.Except(classified).Select(t => t.Name).OrderBy(n => n).ToArray();
        Assert.True(unclassified.Length == 0,
            "Entities with BranchId not classified in BranchDependencyInspector (count them or exclude with a reason): " + string.Join(", ", unclassified));

        var stale = classified.Except(withBranch).Select(t => t.Name).OrderBy(n => n).ToArray();
        Assert.True(stale.Length == 0, "Classified types that no longer have BranchId: " + string.Join(", ", stale));
        Assert.All(BranchDependencyInspector.ExcludedEntities.Values, reason => Assert.False(string.IsNullOrWhiteSpace(reason)));
    }
}
