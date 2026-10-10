using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Domain.Notifications;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

/// <summary>Proves the migrated schema: table, indexes, FKs and check constraints that mirror (never exceed) the domain invariants.</summary>
public class NotificationSchemaTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private AppDbContext _db = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(_postgres.GetConnectionString()).Options);
        await _db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(_db, "Test", true);
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private static Notification Make(string key = "estimate.approval-requested:1", string payload = "{\"resourceId\":\"x\"}") =>
        new(Guid.NewGuid(), TestOnlyDataSeeder.TestOrgId, TestOnlyDataSeeder.TestUserId,
            NotificationTypes.EstimateApprovalRequested, payload, key, DateTimeOffset.UtcNow);

    [Fact]
    public async Task Table_LivesInTheNotificationsSchema_WithTheExpectedIndexes()
    {
        var indexes = await _db.Database
            .SqlQuery<string>($"SELECT indexname AS \"Value\" FROM pg_indexes WHERE schemaname = 'notifications' AND tablename = 'notifications'")
            .ToListAsync();

        Assert.Contains("ux_notifications_recipient_dedupe", indexes);
        Assert.Contains("ix_notifications_recipient_created", indexes);
        Assert.Contains("ix_notifications_unread", indexes);

        var unreadDef = await _db.Database
            .SqlQuery<string>($"SELECT indexdef AS \"Value\" FROM pg_indexes WHERE indexname = 'ix_notifications_unread'")
            .SingleAsync();
        Assert.Contains("read_at_utc IS NULL", unreadDef);
    }

    [Fact]
    public async Task SameRecipientAndDedupeKey_IsRejectedByTheDatabase()
    {
        _db.Set<Notification>().Add(Make());
        await _db.SaveChangesAsync();

        _db.Set<Notification>().Add(Make());
        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => _db.SaveChangesAsync());
        Assert.Equal("ux_notifications_recipient_dedupe", ((PostgresException)ex.InnerException!).ConstraintName);
    }

    [Fact]
    public async Task ARecipientOutsideUsers_IsRejectedByTheForeignKey()
    {
        _db.Set<Notification>().Add(new Notification(
            Guid.NewGuid(), TestOnlyDataSeeder.TestOrgId, Guid.NewGuid(),
            NotificationTypes.EstimateApprovalRequested, "{}", "k:1", DateTimeOffset.UtcNow));

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => _db.SaveChangesAsync());
        Assert.Equal(PostgresErrorCodes.ForeignKeyViolation, ((PostgresException)ex.InnerException!).SqlState);
    }

    [Theory]
    [InlineData("ck_notifications_type_format", "Bad Type", "k:2", "{}")]
    [InlineData("ck_notifications_dedupe_key_length", "estimate.approval-requested", "   ", "{}")]
    [InlineData("ck_notifications_payload_object", "estimate.approval-requested", "k:3", "[]")]
    public async Task ShapeChecks_RejectRowsTheDomainCouldNeverCreate(string constraint, string type, string key, string payload)
    {
        var sql = "INSERT INTO notifications.notifications (id, organization_id, recipient_user_id, type, payload, dedupe_key, created_at_utc) " +
                  "VALUES ({0}, {1}, {2}, {3}, {4}::jsonb, {5}, now())";
        var ex = await Assert.ThrowsAsync<PostgresException>(() => _db.Database.ExecuteSqlRawAsync(
            sql, Guid.NewGuid(), TestOnlyDataSeeder.TestOrgId, TestOnlyDataSeeder.TestUserId, type, payload, key));
        Assert.Equal(constraint, ex.ConstraintName);
    }
}
