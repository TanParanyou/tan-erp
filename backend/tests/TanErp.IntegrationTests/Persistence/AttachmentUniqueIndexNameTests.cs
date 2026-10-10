using Microsoft.EntityFrameworkCore;
using TanErp.Infrastructure.Persistence;
using TanErp.Infrastructure.Persistence.Attachments;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

/// <summary>The store maps unique violations by constraint name; this proves the names exist as unique indexes in the migrated schema.</summary>
public class AttachmentUniqueIndexNameTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public Task InitializeAsync() => _postgres.StartAsync();

    public async Task DisposeAsync() => await _postgres.DisposeAsync();

    [Fact]
    public async Task StoreConstraintNames_AreUniqueIndexesInMigratedSchema()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(_postgres.GetConnectionString()).Options;
        await using var db = new AppDbContext(options);
        await db.Database.MigrateAsync();

        var names = await db.Database
            .SqlQuery<string>($"SELECT indexname AS \"Value\" FROM pg_indexes WHERE schemaname = 'files' AND indexdef LIKE 'CREATE UNIQUE INDEX%'")
            .ToListAsync();

        Assert.Contains(AttachmentStore.ActiveLinkIndex, names);
        Assert.Contains(AttachmentStore.SignatureImageIndex, names);
    }
}
