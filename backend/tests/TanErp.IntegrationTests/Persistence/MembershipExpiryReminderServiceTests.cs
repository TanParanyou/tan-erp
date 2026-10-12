using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Notifications;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Notifications;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Persistence;
using TanErp.Infrastructure.Persistence.Notifications;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

public class MembershipExpiryReminderServiceTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private AppDbContext _db = null!;
    private MembershipExpiryReminderService _service = null!;

    private static readonly Guid Org = TestOnlyDataSeeder.TestOrgId;
    private static readonly Guid Admin = TestOnlyDataSeeder.TestUserId;   // Test Admin: holds memberships.manage at organization scope
    private static readonly DateTimeOffset T0 = new(2026, 10, 12, 8, 0, 0, TimeSpan.Zero);

    private sealed class FakeClock : IClock
    {
        public DateTimeOffset UtcNow { get; set; } = T0;
    }

    private readonly FakeClock _clock = new();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(_postgres.GetConnectionString()).Options);
        await _db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(_db, "Test", true);
        var publisher = new NotificationPublisher(_db, new NotificationRecipientResolver(_db), _clock);
        _service = new MembershipExpiryReminderService(
            _db, publisher, _clock, new MembershipExpiryReminderOptions(), NullLogger<MembershipExpiryReminderService>.Instance);
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private async Task<(Guid UserId, Guid MembershipId)> AddMemberAsync(
        DateTimeOffset? expiresAt, bool membershipActive = true, bool userActive = true, DateTimeOffset? startsAt = null, string? name = null)
    {
        var userId = Guid.NewGuid();
        var membershipId = Guid.NewGuid();
        _db.Users.Add(new User(userId, $"uid-{userId:N}", name ?? "Expiring " + userId.ToString("N")[..6], $"{userId:N}@example.test", userActive));
        _db.Memberships.Add(new Membership(membershipId, Org, null, userId, membershipActive, startsAt, expiresAt));
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
        return (userId, membershipId);
    }

    /// <summary>A second recipient whose only grant is <paramref name="permissionKey"/> at organization scope.</summary>
    private async Task<Guid> AddHolderAsync(string permissionKey)
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var membershipId = Guid.NewGuid();
        var permission = await _db.Permissions.SingleAsync(p => p.Key == permissionKey);
        _db.Users.Add(new User(userId, $"uid-{userId:N}", "Holder " + userId.ToString("N")[..6], $"{userId:N}@example.test"));
        _db.Roles.Add(new Role(roleId, Org, "Holder " + userId.ToString("N")[..6]));
        _db.Add(new RolePermission(Guid.NewGuid(), roleId, Org, permission.Id, PermissionScope.Organization, Org));
        _db.Memberships.Add(new Membership(membershipId, Org, null, userId));
        _db.Add(new MembershipRole(membershipId, roleId, Org));
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
        return userId;
    }

    private Task<List<Notification>> ReminderRowsAsync(Guid recipient) =>
        _db.Notifications.AsNoTracking()
            .Where(n => n.RecipientUserId == recipient && n.Type == NotificationTypes.MembershipExpiring)
            .OrderBy(n => n.CreatedAtUtc).ThenBy(n => n.Id)
            .ToListAsync();

    private static string Field(Notification row, string name)
    {
        using var document = JsonDocument.Parse(row.PayloadJson);
        return document.RootElement.GetProperty(name).GetString()!;
    }

    [Fact]
    public async Task FirstScan_NotifiesTheAdministratorOnce_WithTheSmallestMatchingThreshold()
    {
        var subject = await AddMemberAsync(T0.AddDays(20), name: "สมหญิง ใจดี");

        var first = await _service.RunOnceAsync();
        var second = await _service.RunOnceAsync();

        var rows = await ReminderRowsAsync(Admin);
        var row = Assert.Single(rows);
        Assert.Equal("30", Field(row, NotificationFields.DaysRemaining));
        Assert.Equal(subject.UserId.ToString("D"), Field(row, NotificationFields.ParentId));
        Assert.Equal(subject.MembershipId.ToString("D"), Field(row, NotificationFields.ResourceId));
        Assert.Equal("สมหญิง ใจดี", Field(row, NotificationFields.SubjectDisplayName));
        Assert.Equal(1, first.Created);
        Assert.Equal(0, second.Created);
        Assert.False(first.SkippedBecauseLocked);
        Assert.Empty(await ReminderRowsAsync(subject.UserId));   // the subject holds no memberships.manage
    }

    [Fact]
    public async Task AsTimePasses_EachThresholdFiresExactlyOnce()
    {
        await AddMemberAsync(T0.AddDays(20));

        await _service.RunOnceAsync();
        _clock.UtcNow = T0.AddDays(14);                       // 6 days left -> 7
        await _service.RunOnceAsync();
        await _service.RunOnceAsync();
        _clock.UtcNow = T0.AddDays(19).AddHours(12);          // 12 hours left -> 1
        await _service.RunOnceAsync();
        await _service.RunOnceAsync();

        var rows = await ReminderRowsAsync(Admin);
        Assert.Equal(["30", "7", "1"], rows.Select(r => Field(r, NotificationFields.DaysRemaining)).ToArray());
    }

    [Fact]
    public async Task ChangingTheExpiry_ReArmsTheReminder()
    {
        var subject = await AddMemberAsync(T0.AddDays(20));
        await _service.RunOnceAsync();

        var membership = await _db.Memberships.SingleAsync(m => m.Id == subject.MembershipId);
        membership.UpdateAssignment(null, null, T0.AddDays(25));
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
        await _service.RunOnceAsync();

        Assert.Equal(2, (await ReminderRowsAsync(Admin)).Count);
    }

    [Fact]
    public async Task OnlyUsableMembershipsInsideTheHorizonAreReminded()
    {
        await AddMemberAsync(T0.AddDays(45));                                   // beyond the largest threshold
        await AddMemberAsync(T0.AddHours(-1));                                  // already expired: no "expired" notice
        await AddMemberAsync(null);                                             // never expires
        await AddMemberAsync(T0.AddDays(5), membershipActive: false);
        await AddMemberAsync(T0.AddDays(5), userActive: false);
        await AddMemberAsync(T0.AddDays(5), startsAt: T0.AddDays(1));           // not started yet

        var result = await _service.RunOnceAsync();

        Assert.Equal(0, result.Created);
        Assert.Empty(await ReminderRowsAsync(Admin));
    }

    [Fact]
    public async Task OnlyHoldersOfMembershipsManage_AreNotified()
    {
        var holder = await AddHolderAsync("memberships.manage");
        var other = await AddHolderAsync("estimates.read");
        await AddMemberAsync(T0.AddDays(3));

        await _service.RunOnceAsync();

        Assert.Single(await ReminderRowsAsync(holder));
        Assert.Empty(await ReminderRowsAsync(other));
    }

    [Fact]
    public async Task ALongDisplayName_DoesNotBreakTheScan()
    {
        await AddMemberAsync(T0.AddDays(3), name: new string('ก', 255));

        var result = await _service.RunOnceAsync();

        Assert.Equal(0, result.Failed);
        Assert.Single(await ReminderRowsAsync(Admin));
    }

    [Fact]
    public async Task WhenAnotherInstanceHoldsTheLock_TheRunIsSkipped()
    {
        await AddMemberAsync(T0.AddDays(3));

        await using var other = new NpgsqlConnection(_postgres.GetConnectionString());
        await other.OpenAsync();
        await using var transaction = await other.BeginTransactionAsync();
        await using (var command = new NpgsqlCommand($"SELECT pg_advisory_xact_lock({MembershipExpiryReminderService.AdvisoryLockKey})", other, transaction))
        {
            await command.ExecuteScalarAsync();
        }

        var result = await _service.RunOnceAsync();

        Assert.True(result.SkippedBecauseLocked);
        Assert.Empty(await ReminderRowsAsync(Admin));
    }
}
