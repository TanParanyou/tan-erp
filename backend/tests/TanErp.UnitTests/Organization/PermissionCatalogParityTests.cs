using Xunit;

namespace TanErp.UnitTests.OrganizationPermissions;

public class PermissionCatalogParityTests
{
    private static string RepoFile(params string[] parts)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "docs", "03-contracts", "permission-catalog.md"))) dir = dir.Parent;
        return Path.Combine(new[] { dir!.FullName }.Concat(parts).ToArray());
    }

    [Theory]
    [InlineData("organizations.read")]
    [InlineData("organizations.manage")]
    [InlineData("branches.manage")]
    public void GovernanceKey_IsInDocsSeederAndFrontend(string key)
    {
        Assert.Contains($"`{key}`", File.ReadAllText(RepoFile("docs", "03-contracts", "permission-catalog.md")));
        Assert.Contains($"(\"{key}\"", File.ReadAllText(RepoFile("backend", "src", "TanErp.Infrastructure", "Persistence", "TestOnlyDataSeeder.cs")));
        Assert.Contains($"\"{key}\"", File.ReadAllText(RepoFile("frontend", "src", "lib", "permissions", "permissions.ts")));
    }
}
