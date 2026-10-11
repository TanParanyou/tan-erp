using System.Text.RegularExpressions;
using TanErp.Application.IdentityAccess.Administration;
using Xunit;

namespace TanErp.UnitTests.IdentityAccess;

/// <summary>Every approve/publish/review style key in the seeded catalog must be a conscious decision (ADR 0020).</summary>
public class ApprovalPermissionCoverageTests
{
    /// <summary>Reviewed and deliberately NOT treated as approval authority for maker-checker.</summary>
    private static readonly HashSet<string> ReviewedNotApproval = new(StringComparer.Ordinal)
    {
        "boms.approve",
        "pricing-templates.approve",
        "quick-estimates.review"
    };

    /// <summary>Walks up from the test binary until the repository's backend/src folder is found.</summary>
    private static string BackendFile(params string[] parts)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !Directory.Exists(Path.Combine(dir.FullName, "backend", "src"))) dir = dir.Parent;
        return Path.Combine(new[] { dir!.FullName, "backend" }.Concat(parts).ToArray());
    }

    [Fact]
    public void EverySeededApproveOrPublishKey_IsInTheApprovalSetOrExplicitlyReviewed()
    {
        var seeder = File.ReadAllText(BackendFile("src", "TanErp.Infrastructure", "Persistence", "TestOnlyDataSeeder.cs"));
        var keys = Regex.Matches(seeder, "\\(\"([a-z\\-\\.]+\\.(?:approve|publish|review))\"")
            .Select(m => m.Groups[1].Value)
            .Distinct()
            .ToList();

        Assert.NotEmpty(keys);
        var undecided = keys
            .Where(k => !AdministrationPolicy.ApprovalPermissionKeys.Contains(k) && !ReviewedNotApproval.Contains(k))
            .ToArray();
        Assert.True(undecided.Length == 0,
            "Add these keys to AdministrationPolicy.ApprovalPermissionKeys or to ReviewedNotApproval (and ADR 0020): " + string.Join(", ", undecided));

        Assert.All(ReviewedNotApproval, k => Assert.Contains(k, keys));
    }
}
