using Microsoft.EntityFrameworkCore;
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

public class NotificationInfrastructureTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private AppDbContext _db = null!;
    private NotificationRecipientResolver _resolver = null!;
    private NotificationPublisher _publisher = null!;
    private NotificationStore _store = null!;

    private static readonly Guid Org = TestOnlyDataSeeder.TestOrgId;
    private static readonly Guid Maker = TestOnlyDataSeeder.TestUserId;       // Test Admin: holds every *.approve at organization scope

    private sealed class SystemClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(_postgres.GetConnectionString()).Options);
        await _db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(_db, "Test", true);
        var clock = new SystemClock();
        _resolver = new NotificationRecipientResolver(_db);
        _publisher = new NotificationPublisher(_db, _resolver, clock);
        _store = new NotificationStore(_db, clock);
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    /// <summary>A second user whose only grant is <paramref name="permissionKey"/> at organization scope.</summary>
    private async Task<Guid> AddCheckerAsync(string permissionKey, Guid? membershipBranchId = null, bool active = true, DateTimeOffset? expiresAtUtc = null)
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var membershipId = Guid.NewGuid();
        var permission = await _db.Permissions.SingleAsync(p => p.Key == permissionKey);
        _db.Users.Add(new User(userId, $"uid-{userId:N}", "Checker " + userId.ToString("N")[..6], $"{userId:N}@example.test"));
        _db.Roles.Add(new Role(roleId, Org, "Checker " + userId.ToString("N")[..6]));
        _db.Add(new RolePermission(Guid.NewGuid(), roleId, Org, permission.Id, PermissionScope.Organization, Org));
        _db.Memberships.Add(new Membership(membershipId, Org, membershipBranchId, userId, active, null, expiresAtUtc));
        _db.Add(new MembershipRole(membershipId, roleId, Org));
        await _db.SaveChangesAsync();
        return userId;
    }

    private static NotificationEvent MrpEvent(Guid? transition = null) =>
        NotificationEvents.MrpRunCreated(Org, TestOnlyDataSeeder.TestBranchId, transition ?? Guid.NewGuid(), Maker, "MRP-0001");

    [Fact]
    public async Task Resolver_ReturnsHoldersOfThePermission_AndDropsExcludedInactiveExpiredAndOtherBranchUsers()
    {
        var holder = await AddCheckerAsync("mrp.approve");
        await AddCheckerAsync("mrp.approve", active: false);
        await AddCheckerAsync("mrp.approve", expiresAtUtc: DateTimeOffset.UtcNow.AddDays(-1));
        var otherBranch = new Branch(Guid.NewGuid(), Org, "NOTIF-B2", "Other branch");
        _db.Branches.Add(otherBranch);
        await _db.SaveChangesAsync();
        await AddCheckerAsync("mrp.approve", membershipBranchId: otherBranch.Id);                     // another branch of the same organization: never matches
        await AddCheckerAsync("estimates.read");                                                       // wrong permission

        var ids = await _resolver.ResolveAsync(Org, TestOnlyDataSeeder.TestBranchId, "mrp.approve", [Maker], DateTimeOffset.UtcNow);

        Assert.Equal([holder], ids);
    }

    [Fact]
    public async Task Resolver_WithoutABranch_ReturnsBranchScopedMembersToo()
    {
        var branchHolder = await AddCheckerAsync("roles.assign-approval", membershipBranchId: TestOnlyDataSeeder.TestBranchId);

        var ids = await _resolver.ResolveAsync(Org, null, "roles.assign-approval", [Maker], DateTimeOffset.UtcNow);

        Assert.Contains(branchHolder, ids);
        Assert.DoesNotContain(Maker, ids);
    }

    [Fact]
    public async Task Publish_StagesRowsWithoutSaving_AndTheCallersSaveCommitsThem_ExcludingTheMaker()
    {
        var checker = await AddCheckerAsync("mrp.approve");

        await _publisher.PublishAsync(MrpEvent());

        Assert.Equal(0, await CountRowsAsync());            // nothing is stored until the module saves
        await _db.SaveChangesAsync();
        var rows = await _db.Notifications.AsNoTracking().ToListAsync();
        Assert.Contains(rows, r => r.RecipientUserId == checker && r.Type == NotificationTypes.MrpRunApprovalRequested);
        Assert.DoesNotContain(rows, r => r.RecipientUserId == Maker);
        Assert.Contains("MRP-0001", rows[0].PayloadJson);
    }

    [Fact]
    public async Task Publish_ThenDiscardingTheUnitOfWork_LeavesNoNotification()
    {
        await AddCheckerAsync("mrp.approve");
        await _publisher.PublishAsync(MrpEvent());

        _db.ChangeTracker.Clear();                           // what a failed/rolled-back business change does to the staged rows
        await _db.SaveChangesAsync();

        Assert.Equal(0, await CountRowsAsync());
    }

    [Fact]
    public async Task Publish_TheSameTransitionTwice_NotifiesOnce_ButANewTransitionNotifiesAgain()
    {
        var checker = await AddCheckerAsync("mrp.approve");
        var transition = Guid.NewGuid();

        await _publisher.PublishAsync(MrpEvent(transition));
        await _publisher.PublishAsync(MrpEvent(transition));   // staged but unsaved: still deduped
        await _db.SaveChangesAsync();
        await _publisher.PublishAsync(MrpEvent(transition));   // already stored: deduped
        await _db.SaveChangesAsync();
        Assert.Equal(1, await _db.Notifications.CountAsync(n => n.RecipientUserId == checker));

        await _publisher.PublishAsync(MrpEvent());
        await _db.SaveChangesAsync();
        Assert.Equal(2, await _db.Notifications.CountAsync(n => n.RecipientUserId == checker));
    }

    [Fact]
    public async Task Publish_WithExplicitRecipients_NotifiesOnlyThem()
    {
        // The seed does not create the dedicated estimate reviewer, so the route reviewer is a checker created here.
        var reviewer = await AddCheckerAsync("estimates.approve");
        await AddCheckerAsync("estimates.approve");            // holds the permission but is not on the route

        await _publisher.PublishAsync(NotificationEvents.EstimateSubmitted(
            Org, TestOnlyDataSeeder.TestBranchId, Guid.NewGuid(), Maker, Guid.NewGuid(), "EST-0001", reviewer));
        await _db.SaveChangesAsync();

        Assert.Equal([reviewer], await _db.Notifications.Select(n => n.RecipientUserId).ToListAsync());
    }

    [Fact]
    public async Task Publish_WithNoEligibleRecipient_StagesNothing_AndAnUnknownActorFailsLoudly()
    {
        await _publisher.PublishAsync(MrpEvent());             // seed has only the maker holding mrp.approve
        await _db.SaveChangesAsync();
        Assert.Equal(0, await CountRowsAsync());

        var ex = await Assert.ThrowsAsync<NotificationDomainException>(() => _publisher.PublishAsync(
            NotificationEvents.MrpRunCreated(Org, TestOnlyDataSeeder.TestBranchId, Guid.NewGuid(), Guid.NewGuid(), "MRP-0002")));
        Assert.Equal("NOTIFICATION_FIELD_INVALID", ex.Code);
    }

    [Fact]
    public async Task Store_OnlyEverSeesTheCallersOwnRows_AndMarkReadIsIdempotent()
    {
        var mine = await AddCheckerAsync("mrp.approve");
        var theirs = await AddCheckerAsync("mrp.approve");
        await _publisher.PublishAsync(MrpEvent());
        await _publisher.PublishAsync(MrpEvent());
        await _db.SaveChangesAsync();

        var page = await _store.ListAsync(Org, mine, new NotificationListQuery(false, 1, 20));
        Assert.Equal(2, page.TotalCount);
        Assert.True(page.Items[0].CreatedAtUtc >= page.Items[1].CreatedAtUtc);
        Assert.Equal(2, await _store.CountUnreadAsync(Org, mine));

        var otherId = await _db.Notifications.Where(n => n.RecipientUserId == theirs).Select(n => n.Id).FirstAsync();
        Assert.Null(await _store.MarkReadAsync(Org, mine, otherId));                       // someone else's row
        Assert.Null(await _store.MarkReadAsync(Guid.NewGuid(), mine, page.Items[0].Id));    // another organization
        Assert.Null(await _store.MarkReadAsync(Org, mine, Guid.NewGuid()));                // does not exist

        var first = await _store.MarkReadAsync(Org, mine, page.Items[0].Id);
        var again = await _store.MarkReadAsync(Org, mine, page.Items[0].Id);
        Assert.NotNull(first!.ReadAtUtc);
        Assert.Equal(first.ReadAtUtc, again!.ReadAtUtc);                                   // first read time is kept
        Assert.Equal(1, await _store.CountUnreadAsync(Org, mine));
        Assert.Equal(2, await _store.CountUnreadAsync(Org, theirs));                       // untouched

        var unreadOnly = await _store.ListAsync(Org, mine, new NotificationListQuery(true, 1, 20));
        Assert.Single(unreadOnly.Items);

        Assert.Equal(1, await _store.MarkAllReadAsync(Org, mine));
        Assert.Equal(0, await _store.MarkAllReadAsync(Org, mine));
        Assert.Equal(2, await _store.CountUnreadAsync(Org, theirs));
    }

    [Fact]
    public async Task Publish_ExplicitRecipientsWithoutActiveMembershipInTheOrganization_AreDropped()
    {
        // A user whose only membership is in another organization (TestOrgBId).
        var otherOrgUserId = Guid.NewGuid();
        _db.Users.Add(new User(otherOrgUserId, $"uid-{otherOrgUserId:N}", "Other org " + otherOrgUserId.ToString("N")[..6], $"{otherOrgUserId:N}@example.test"));
        _db.Memberships.Add(new Membership(Guid.NewGuid(), TestOnlyDataSeeder.TestOrgBId, TestOnlyDataSeeder.TestBranchBId, otherOrgUserId, isActive: true));
        var inactiveUserId = await AddCheckerAsync("estimates.approve", active: false);
        var expiredUserId = await AddCheckerAsync("estimates.approve", expiresAtUtc: DateTimeOffset.UtcNow.AddDays(-1));
        await _db.SaveChangesAsync();

        foreach (var explicitRecipient in new[] { otherOrgUserId, inactiveUserId, expiredUserId })
        {
            await _publisher.PublishAsync(NotificationEvents.EstimateSubmitted(
                Org, TestOnlyDataSeeder.TestBranchId, Guid.NewGuid(), Maker, Guid.NewGuid(), "EST-0003", explicitRecipient));
        }
        await _db.SaveChangesAsync();

        Assert.Equal(0, await CountRowsAsync());
    }

    private Task<int> CountRowsAsync() => _db.Notifications.AsNoTracking().CountAsync();
}
