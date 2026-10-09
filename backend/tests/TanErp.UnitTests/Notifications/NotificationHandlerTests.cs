using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Notifications;
using TanErp.Domain.Notifications;
using Xunit;

namespace TanErp.UnitTests.Notifications;

public class NotificationHandlerTests
{
    private static readonly Guid Org = Guid.NewGuid();
    private static readonly Guid User = Guid.NewGuid();
    private static readonly Guid ResourceId = Guid.NewGuid();
    private static readonly NotificationCaller Caller = new("uid-1", Guid.NewGuid(), "trace");

    private sealed class FakeAccess : IRequestAccessResolver
    {
        public bool MembershipActive { get; set; } = true;
        public HashSet<string> Granted { get; } = new(StringComparer.Ordinal);
        public List<string> Requested { get; } = new();

        public Task<Result<RequestAccessContext>> ResolveMembershipAsync(string firebaseUid, Guid membershipId, CancellationToken cancellationToken = default) =>
            Task.FromResult(MembershipActive
                ? Result<RequestAccessContext>.Success(new RequestAccessContext(User, membershipId, Org, null, string.Empty, "organization"))
                : Result<RequestAccessContext>.Failure(new Error("ACTIVE_MEMBERSHIP_REQUIRED", "inactive")));

        public Task<Result<RequestAccessContext>> ResolveAsync(string firebaseUid, Guid membershipId, string permissionKey, CancellationToken cancellationToken = default)
        {
            Requested.Add(permissionKey);
            return Task.FromResult(Granted.Contains(permissionKey)
                ? Result<RequestAccessContext>.Success(new RequestAccessContext(User, membershipId, Org, null, permissionKey, "organization"))
                : Result<RequestAccessContext>.Failure(new Error("PERMISSION_DENIED", "denied")));
        }
    }

    private sealed class FakeStore : INotificationStore
    {
        public List<NotificationRow> Rows { get; } = new();
        public int Calls { get; private set; }
        public (Guid Org, Guid User)? LastOwner { get; private set; }
        public NotificationListQuery? LastQuery { get; private set; }

        // Honors the requested page like the real store does, so clamping is observable through the handler.
        public Task<NotificationRowPage> ListAsync(Guid organizationId, Guid userId, NotificationListQuery query, CancellationToken ct = default)
        {
            Calls++; LastOwner = (organizationId, userId); LastQuery = query;
            var items = Rows.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToList();
            return Task.FromResult(new NotificationRowPage(items, Rows.Count + 40));
        }

        public Task<int> CountUnreadAsync(Guid organizationId, Guid userId, CancellationToken ct = default)
        {
            Calls++; LastOwner = (organizationId, userId);
            return Task.FromResult(3);
        }

        public Task<NotificationRow?> MarkReadAsync(Guid organizationId, Guid userId, Guid notificationId, CancellationToken ct = default)
        {
            Calls++; LastOwner = (organizationId, userId);
            return Task.FromResult(Rows.FirstOrDefault(r => r.Id == notificationId));
        }

        public Task<int> MarkAllReadAsync(Guid organizationId, Guid userId, CancellationToken ct = default)
        {
            Calls++; LastOwner = (organizationId, userId);
            return Task.FromResult(7);
        }
    }

    private static NotificationRow Row(string type, string payload) => new(Guid.NewGuid(), type, payload, DateTimeOffset.UtcNow, null);

    private static string PoPayload => $"{{\"resourceId\":\"{ResourceId}\",\"documentNumber\":\"PO-1\",\"actorDisplayName\":\"A\"}}";

    private static (NotificationHandler Handler, FakeAccess Access, FakeStore Store) Build()
    {
        var access = new FakeAccess();
        var store = new FakeStore();
        return (new NotificationHandler(access, store), access, store);
    }

    [Fact]
    public async Task EveryOperation_RequiresAnActiveMembership_BeforeTouchingTheStore()
    {
        var (handler, access, store) = Build();
        access.MembershipActive = false;

        Assert.Equal("ACTIVE_MEMBERSHIP_REQUIRED", (await handler.ListAsync(Caller, false, 1, 20)).Error.Code);
        Assert.Equal("ACTIVE_MEMBERSHIP_REQUIRED", (await handler.CountUnreadAsync(Caller)).Error.Code);
        Assert.Equal("ACTIVE_MEMBERSHIP_REQUIRED", (await handler.MarkReadAsync(Caller, Guid.NewGuid())).Error.Code);
        Assert.Equal("ACTIVE_MEMBERSHIP_REQUIRED", (await handler.MarkAllReadAsync(Caller)).Error.Code);
        Assert.Equal(0, store.Calls);
    }

    [Fact]
    public async Task List_UsesTheCallersOwnOrganizationAndUser_AndClampsPaging()
    {
        var (handler, _, store) = Build();

        var result = await handler.ListAsync(Caller, true, 0, 500);

        Assert.True(result.IsSuccess);
        Assert.Equal((Org, User), store.LastOwner);
        Assert.Equal(new NotificationListQuery(true, 1, NotificationLimits.MaxPageSize), store.LastQuery);
        Assert.Equal(1, result.Value!.Page);
        Assert.Equal(NotificationLimits.MaxPageSize, result.Value.PageSize);
        Assert.Equal(40, result.Value.TotalCount);
        Assert.Equal(1, result.Value.TotalPages);

        await handler.ListAsync(Caller, false, 3, 0);
        Assert.Equal(new NotificationListQuery(false, 3, NotificationLimits.DefaultPageSize), store.LastQuery);
    }

    [Fact]
    public async Task List_PageSizeAboveTheMaximum_ReturnsAtMostTheMaximumItems_AndReportsTheClampedSize()
    {
        var (handler, _, store) = Build();
        for (var i = 0; i < 60; i++) store.Rows.Add(Row(NotificationTypes.PurchaseOrderApprovalRequested, PoPayload));

        var result = await handler.ListAsync(Caller, false, 1, 1000);

        Assert.True(result.IsSuccess);
        Assert.Equal(NotificationLimits.MaxPageSize, store.LastQuery!.PageSize);
        Assert.Equal(NotificationLimits.MaxPageSize, result.Value!.PageSize);
        Assert.True(result.Value.Items.Count <= NotificationLimits.MaxPageSize);
        Assert.Equal(NotificationLimits.MaxPageSize, result.Value.Items.Count);
        Assert.Equal(2, result.Value.TotalPages);
    }

    [Fact]
    public async Task DeepLink_FollowsTheReadersCurrentPermission()
    {
        var (handler, access, store) = Build();
        store.Rows.Add(Row(NotificationTypes.PurchaseOrderApprovalRequested, PoPayload));
        store.Rows.Add(Row(NotificationTypes.PurchaseOrderApprovalRequested, PoPayload));

        var withoutPermission = await handler.ListAsync(Caller, false, 1, 20);
        Assert.All(withoutPermission.Value!.Items, i => Assert.Null(i.DeepLink));

        access.Granted.Add("purchase-orders.approve");
        access.Requested.Clear();
        var withPermission = await handler.ListAsync(Caller, false, 1, 20);

        Assert.All(withPermission.Value!.Items, i => Assert.Equal($"/procurement/purchase-orders/{ResourceId}", i.DeepLink));
        Assert.Single(access.Requested);                       // one permission lookup per distinct type, not per row
        Assert.Equal("PO-1", withPermission.Value.Items[0].Payload["documentNumber"]);
    }

    [Fact]
    public async Task ARowWhoseTypeIsNoLongerRegistered_IsStillListed_WithoutALink()
    {
        var (handler, _, store) = Build();
        store.Rows.Add(Row("retired.type", "{\"resourceId\":\"x\"}"));

        var result = await handler.ListAsync(Caller, false, 1, 20);

        Assert.Null(Assert.Single(result.Value!.Items).DeepLink);
    }

    [Fact]
    public async Task MarkRead_OfARowThatIsNotTheCallers_IsNotFound()
    {
        var (handler, _, store) = Build();

        var result = await handler.MarkReadAsync(Caller, Guid.NewGuid());

        Assert.Equal("NOTIFICATION_NOT_FOUND", result.Error.Code);
        Assert.Equal((Org, User), store.LastOwner);
    }

    [Fact]
    public async Task MarkRead_ReturnsTheProjection_AndCountsAreScopedToTheCaller()
    {
        var (handler, _, store) = Build();
        var row = Row(NotificationTypes.MrpRunApprovalRequested, $"{{\"resourceId\":\"{ResourceId}\",\"documentNumber\":\"M-1\",\"actorDisplayName\":\"A\"}}");
        store.Rows.Add(row);

        var read = await handler.MarkReadAsync(Caller, row.Id);
        Assert.Equal(row.Id, read.Value!.Id);
        Assert.Equal(3, (await handler.CountUnreadAsync(Caller)).Value);
        Assert.Equal(7, (await handler.MarkAllReadAsync(Caller)).Value);
        Assert.Equal((Org, User), store.LastOwner);
    }
}
