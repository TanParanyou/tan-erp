using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.Notifications;
using TanErp.Api.Contracts.Procurement;
using TanErp.Api.ErrorHandling;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Notifications;
using TanErp.Domain.Organization;
using TanErp.Domain.Procurement;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class NotificationEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private static readonly Guid OrgId = TestOnlyDataSeeder.TestOrgId;
    private static readonly Guid UserA = TestOnlyDataSeeder.TestUserId;
    private static readonly Guid UserB = TestOnlyDataSeeder.TestUserIdB;
    private static readonly Guid PoId = Guid.NewGuid();
    private const string UidApprover = "uid-notif-approver";
    private const string UidNoPerm = "uid-notif-noperm";
    private const string UidInactive = "uid-notif-inactive";
    private static readonly Guid ApproverUserId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5b01");
    private static readonly Guid ApproverMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5b02");
    private static readonly Guid NoPermUserId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5b03");
    private static readonly Guid NoPermMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5b04");
    private static readonly Guid InactiveUserId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5b05");
    private static readonly Guid InactiveMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5b06");
    private Guid[] _mineIds = [];
    private Guid _orgBRowId;

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(idToken switch
            {
                "token-org-a" => TestOnlyDataSeeder.TestFirebaseUid,
                "token-org-b" => TestOnlyDataSeeder.TestFirebaseUidB,
                "token-approver" => UidApprover,
                "token-noperm" => UidNoPerm,
                "token-inactive" => UidInactive,
                _ => null
            });
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = _postgres.GetConnectionString(),
                ["Storage:BasePath"] = Path.Combine(Path.GetTempPath(), $"tan-erp-notif-{Guid.NewGuid():N}"),
                ["SeedTestData"] = "true"
            }));
            builder.ConfigureServices(services =>
            {
                // EF Core does not pick up IInterceptor singletons from DI for this context, so attach it to the options directly.
                services.ConfigureDbContext<AppDbContext>(options => options.AddInterceptors(new FailNextNotificationSaveInterceptor()));
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFirebaseTokenVerifier));
                if (descriptor != null) services.Remove(descriptor);
                services.AddSingleton<IFirebaseTokenVerifier, TestFirebaseTokenVerifier>();
            });
        });
        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true, seedItemCatalogDemoData: true);

        var adminRole = await db.Roles.SingleAsync(r => r.OrganizationId == OrgId && r.Name == "Test Admin");
        db.Users.Add(new User(ApproverUserId, UidApprover, "Notif Approver", "notif-approver@example.test", true));
        db.Memberships.Add(new Membership(ApproverMembershipId, OrgId, TestOnlyDataSeeder.TestBranchId, ApproverUserId, isActive: true));
        db.MembershipRoles.Add(new MembershipRole(ApproverMembershipId, adminRole.Id, OrgId));
        // Member with no role at all: reachable by the API, holds no approve permission.
        db.Users.Add(new User(NoPermUserId, UidNoPerm, "Notif NoPerm", "notif-noperm@example.test", true));
        db.Memberships.Add(new Membership(NoPermMembershipId, OrgId, TestOnlyDataSeeder.TestBranchId, NoPermUserId, isActive: true));
        // Holds the admin role but the membership is inactive: must never be a recipient.
        db.Users.Add(new User(InactiveUserId, UidInactive, "Notif Inactive", "notif-inactive@example.test", true));
        db.Memberships.Add(new Membership(InactiveMembershipId, OrgId, TestOnlyDataSeeder.TestBranchId, InactiveUserId, isActive: false));
        db.MembershipRoles.Add(new MembershipRole(InactiveMembershipId, adminRole.Id, OrgId));
        await db.SaveChangesAsync();

        var t = DateTimeOffset.UtcNow;
        string Payload() => $"{{\"resourceId\":\"{PoId}\",\"documentNumber\":\"PO-1\",\"actorDisplayName\":\"Maker\"}}";
        var mine = Enumerable.Range(0, 3).Select(i => new Notification(
            Guid.NewGuid(), OrgId, UserA, NotificationTypes.PurchaseOrderApprovalRequested, Payload(), $"po:{i}", t.AddMinutes(i))).ToArray();
        var theirs = new Notification(Guid.NewGuid(), OrgId, UserB, NotificationTypes.PurchaseOrderApprovalRequested, Payload(), "po:other", t);
        var orgB = new Notification(Guid.NewGuid(), TestOnlyDataSeeder.TestOrgBId, UserB, NotificationTypes.PurchaseOrderApprovalRequested, Payload(), "po:orgb", t);
        db.Notifications.AddRange(mine);
        db.Notifications.AddRange(theirs, orgB);
        await db.SaveChangesAsync();
        _mineIds = mine.OrderByDescending(n => n.CreatedAtUtc).Select(n => n.Id).ToArray();
        _orgBRowId = orgB.Id;
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, string? token = "token-org-a", Guid? membership = null)
    {
        var request = new HttpRequestMessage(method, url);
        if (token is not null) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var membershipId = membership ?? (token == "token-org-b" ? TestOnlyDataSeeder.TestMembershipBId : TestOnlyDataSeeder.TestMembershipId);
        request.Headers.Add("X-Membership-Id", membershipId.ToString());
        return await _client.SendAsync(request);
    }

    private static async Task<string> CodeAsync(HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<ApiProblemDetails>())!.Code;

    private async Task<HttpResponseMessage> SendJsonAsync(
        HttpMethod method, string url, object? body, string token, Guid membership, string? key = null, Guid? ifMatch = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Membership-Id", membership.ToString());
        if (key is not null) request.Headers.Add("Idempotency-Key", key);
        if (ifMatch.HasValue) request.Headers.Add("If-Match", $"\"{ifMatch.Value}\"");
        if (body is not null) request.Content = JsonContent.Create(body);
        return await _client.SendAsync(request);
    }

    private static readonly Guid MembershipA = TestOnlyDataSeeder.TestMembershipId;

    private async Task<PurchaseOrderResponse> CreateDraftPurchaseOrderAsync()
    {
        var supplier = await SendJsonAsync(HttpMethod.Post, "/api/v1/suppliers",
            new SupplierRequest("บริษัท ไม้ดี จำกัด", "Good Wood", "0105500000001", "คุณขาย", "021234567", "sales@example.test", 30),
            "token-org-a", MembershipA, key: Guid.NewGuid().ToString("N"));
        var supplierId = (await supplier.Content.ReadFromJsonAsync<SupplierResponse>())!.Id;
        var created = await SendJsonAsync(HttpMethod.Post, "/api/v1/purchase-orders",
            new PurchaseOrderRequest(supplierId, null, new DateOnly(2026, 11, 15), "ส่งหน้างาน",
                new List<PurchaseOrderLineRequest> { new(TestOnlyDataSeeder.TestItemCatalogPlywoodId, 10m, 1250m) }),
            "token-org-a", MembershipA, key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        return (await created.Content.ReadFromJsonAsync<PurchaseOrderResponse>())!;
    }

    private Task<HttpResponseMessage> SubmitAsync(PurchaseOrderResponse order) =>
        SendJsonAsync(HttpMethod.Post, $"/api/v1/purchase-orders/{order.Id}/submit", new PurchaseOrderActionRequest(null),
            "token-org-a", MembershipA, ifMatch: order.RowVersion);

    private async Task<NotificationListResponse> ListAsync(string token, Guid membership, string query = "")
    {
        var response = await SendJsonAsync(HttpMethod.Get, $"/api/v1/notifications{query}", null, token, membership);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<NotificationListResponse>())!;
    }

    [Fact]
    public async Task Requests_WithoutAuthentication_OrWithAnotherUsersMembership_AreRejected()
    {
        Assert.Equal(HttpStatusCode.Unauthorized, (await SendAsync(HttpMethod.Get, "/api/v1/notifications", token: null)).StatusCode);

        var foreign = await SendAsync(HttpMethod.Get, "/api/v1/notifications", membership: TestOnlyDataSeeder.TestMembershipBId);
        Assert.Equal(HttpStatusCode.Forbidden, foreign.StatusCode);
        Assert.Equal("ACTIVE_MEMBERSHIP_REQUIRED", await CodeAsync(foreign));
    }

    [Fact]
    public async Task List_ReturnsOnlyTheCallersRows_NewestFirst_WithDeepLinkAndNoRawIds()
    {
        var response = await SendAsync(HttpMethod.Get, "/api/v1/notifications?pageSize=500");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = (await response.Content.ReadFromJsonAsync<NotificationListResponse>())!;

        Assert.Equal(_mineIds, body.Items.Select(i => i.Id).ToArray());
        Assert.Equal(3, body.Pagination.TotalCount);
        Assert.Equal(50, body.Pagination.PageSize);
        Assert.All(body.Items, i => Assert.Equal($"/procurement/purchase-orders/{PoId}", i.DeepLink));
        Assert.DoesNotContain("recipient", await (await SendAsync(HttpMethod.Get, "/api/v1/notifications")).Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task List_PageSizeAboveTheMaximum_IsClampedToFifty()
    {
        var response = await SendAsync(HttpMethod.Get, "/api/v1/notifications?pageSize=1000");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = (await response.Content.ReadFromJsonAsync<NotificationListResponse>())!;

        Assert.True(body.Items.Count <= 50);
        Assert.Equal(50, body.Pagination.PageSize);
        Assert.Equal(1, body.Pagination.Page);
    }

    [Fact]
    public async Task List_NonPositivePageAndPageSize_FallBackToTheFirstPageAndDefaultSize()
    {
        var body = (await (await SendAsync(HttpMethod.Get, "/api/v1/notifications?page=-4&pageSize=0")).Content.ReadFromJsonAsync<NotificationListResponse>())!;

        Assert.Equal(1, body.Pagination.Page);
        Assert.Equal(20, body.Pagination.PageSize);
    }

    [Fact]
    public async Task Paging_AndUnreadFilter_Work()
    {
        var page2 = (await (await SendAsync(HttpMethod.Get, "/api/v1/notifications?page=2&pageSize=2")).Content.ReadFromJsonAsync<NotificationListResponse>())!;
        Assert.Single(page2.Items);
        Assert.Equal(2, page2.Pagination.TotalPages);

        await SendAsync(HttpMethod.Post, $"/api/v1/notifications/{_mineIds[0]}/read");
        var unread = (await (await SendAsync(HttpMethod.Get, "/api/v1/notifications?unreadOnly=true")).Content.ReadFromJsonAsync<NotificationListResponse>())!;
        Assert.Equal(2, unread.Items.Count);
    }

    [Fact]
    public async Task MarkRead_WithoutIdempotencyKey_Works_AndIsIdempotent()
    {
        var first = await SendAsync(HttpMethod.Post, $"/api/v1/notifications/{_mineIds[0]}/read");
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        var a = (await first.Content.ReadFromJsonAsync<NotificationResponse>())!;
        var b = (await (await SendAsync(HttpMethod.Post, $"/api/v1/notifications/{_mineIds[0]}/read")).Content.ReadFromJsonAsync<NotificationResponse>())!;

        Assert.NotNull(a.ReadAtUtc);
        Assert.Equal(a.ReadAtUtc, b.ReadAtUtc);
        Assert.Equal(2, (await (await SendAsync(HttpMethod.Get, "/api/v1/notifications/unread-count")).Content.ReadFromJsonAsync<UnreadCountResponse>())!.UnreadCount);
    }

    [Fact]
    public async Task MarkRead_OfAnotherUsersOrOrganizationsOrUnknownRow_IsTheSame404()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var otherUsersRow = await db.Notifications.Where(n => n.DedupeKey == "po:other").Select(n => n.Id).SingleAsync();

        foreach (var id in new[] { otherUsersRow, _orgBRowId, Guid.NewGuid() })
        {
            var response = await SendAsync(HttpMethod.Post, $"/api/v1/notifications/{id}/read");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("NOTIFICATION_NOT_FOUND", await CodeAsync(response));
        }

        Assert.Null((await db.Notifications.AsNoTracking().SingleAsync(n => n.Id == otherUsersRow)).ReadAtUtc);
    }

    [Fact]
    public async Task ReadAll_OnlyTouchesTheCallersRowsInTheCurrentOrganization()
    {
        var response = await SendAsync(HttpMethod.Post, "/api/v1/notifications/read-all");
        Assert.Equal(3, (await response.Content.ReadFromJsonAsync<MarkAllReadResponse>())!.UpdatedCount);

        var b = await SendAsync(HttpMethod.Get, "/api/v1/notifications/unread-count", "token-org-b");
        Assert.Equal(1, (await b.Content.ReadFromJsonAsync<UnreadCountResponse>())!.UnreadCount);   // user B's Org B row only
        Assert.Equal(0, (await (await SendAsync(HttpMethod.Get, "/api/v1/notifications/unread-count")).Content.ReadFromJsonAsync<UnreadCountResponse>())!.UnreadCount);
    }

    [Fact]
    public async Task SubmitPurchaseOrder_NotifiesTheOtherApproverOnce_WithPayloadFreeOfFiguresAndPii_AndNotTheMaker()
    {
        var order = await CreateDraftPurchaseOrderAsync();
        Assert.Equal(HttpStatusCode.OK, (await SubmitAsync(order)).StatusCode);

        var approver = await ListAsync("token-approver", ApproverMembershipId);
        var item = Assert.Single(approver.Items);
        Assert.Equal(NotificationTypes.PurchaseOrderApprovalRequested, item.Type);
        Assert.Equal($"/procurement/purchase-orders/{order.Id}", item.DeepLink);
        Assert.Equal(
            new[] { "actorDisplayName", "documentNumber", "resourceId" },
            item.Payload.Keys.OrderBy(k => k, StringComparer.Ordinal).ToArray());
        Assert.Equal(order.Number, item.Payload["documentNumber"]);
        Assert.Equal(TestOnlyDataSeeder.TestUserDisplayName, item.Payload["actorDisplayName"]);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var stored = await db.Notifications.AsNoTracking()
            .Where(n => n.Type == NotificationTypes.PurchaseOrderApprovalRequested && n.DedupeKey.StartsWith("purchase-order.approval-requested:"))
            .ToListAsync();
        var row = Assert.Single(stored);
        Assert.Equal(ApproverUserId, row.RecipientUserId);
        foreach (var forbidden in new[] { "1250", "12500", "sales@example.test", "0105500000001", "021234567", TestOnlyDataSeeder.TestUserEmail })
        {
            Assert.DoesNotContain(forbidden, row.PayloadJson.Replace(order.Id.ToString(), string.Empty), StringComparison.Ordinal);
        }

        // Maker, a member without the permission and an inactive holder of the permission get nothing.
        Assert.DoesNotContain(
            (await ListAsync("token-org-a", MembershipA, "?unreadOnly=true")).Items,
            i => i.Type == NotificationTypes.PurchaseOrderApprovalRequested && i.Payload["resourceId"] == order.Id.ToString());
        Assert.Empty((await ListAsync("token-noperm", NoPermMembershipId)).Items);
        Assert.False(await db.Notifications.AnyAsync(n => n.RecipientUserId == InactiveUserId));
    }

    [Fact]
    public async Task SubmitPurchaseOrder_WhenTheSaveFails_LeavesNoNotificationAndNoStatusChange_AndRetryNotifiesOnce()
    {
        var order = await CreateDraftPurchaseOrderAsync();

        FailNextNotificationSaveInterceptor.Arm();
        var failed = await SubmitAsync(order);
        Assert.Equal(HttpStatusCode.Conflict, failed.StatusCode);
        Assert.Equal("PURCHASE_ORDER_VERSION_CONFLICT", await CodeAsync(failed));

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Assert.Equal(PurchaseOrderStatus.Draft, (await db.PurchaseOrders.AsNoTracking().SingleAsync(o => o.Id == order.Id)).Status);
            Assert.False(await db.Notifications.AnyAsync(n => n.RecipientUserId == ApproverUserId));
        }

        Assert.Equal(HttpStatusCode.OK, (await SubmitAsync(order)).StatusCode);
        Assert.Single((await ListAsync("token-approver", ApproverMembershipId)).Items);
    }

    [Fact]
    public async Task OwnOnly_TheMakerNeverSeesTheApproversRow_AndAnotherOrganizationGets404()
    {
        var order = await CreateDraftPurchaseOrderAsync();
        await SubmitAsync(order);
        var approverRowId = Assert.Single((await ListAsync("token-approver", ApproverMembershipId)).Items).Id;

        Assert.DoesNotContain(approverRowId, (await ListAsync("token-org-a", MembershipA, "?pageSize=50")).Items.Select(i => i.Id));

        foreach (var (token, membership) in new[] { ("token-org-a", MembershipA), ("token-org-b", TestOnlyDataSeeder.TestMembershipBId) })
        {
            var response = await SendJsonAsync(HttpMethod.Post, $"/api/v1/notifications/{approverRowId}/read", null, token, membership);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("NOTIFICATION_NOT_FOUND", await CodeAsync(response));
        }

        Assert.Null(Assert.Single((await ListAsync("token-approver", ApproverMembershipId)).Items).ReadAtUtc);
    }

    [Fact]
    public async Task UnreadCount_DecrementsOnMarkRead_AndReadAllClearsOnlyTheCallersRows()
    {
        foreach (var _ in Enumerable.Range(0, 2))
        {
            await SubmitAsync(await CreateDraftPurchaseOrderAsync());
        }

        async Task<int> UnreadAsync(string token, Guid membership) =>
            (await (await SendJsonAsync(HttpMethod.Get, "/api/v1/notifications/unread-count", null, token, membership))
                .Content.ReadFromJsonAsync<UnreadCountResponse>())!.UnreadCount;

        Assert.Equal(2, await UnreadAsync("token-approver", ApproverMembershipId));
        var first = (await ListAsync("token-approver", ApproverMembershipId)).Items[0].Id;

        Assert.Equal(HttpStatusCode.OK, (await SendJsonAsync(HttpMethod.Post, $"/api/v1/notifications/{first}/read", null, "token-approver", ApproverMembershipId)).StatusCode);
        Assert.Equal(1, await UnreadAsync("token-approver", ApproverMembershipId));

        var readAll = await SendJsonAsync(HttpMethod.Post, "/api/v1/notifications/read-all", null, "token-approver", ApproverMembershipId);
        Assert.Equal(1, (await readAll.Content.ReadFromJsonAsync<MarkAllReadResponse>())!.UpdatedCount);
        Assert.Equal(0, await UnreadAsync("token-approver", ApproverMembershipId));
        // UserA's three seeded rows (Task 6 fixture) are untouched by the approver's read-all.
        Assert.Equal(3, await UnreadAsync("token-org-a", MembershipA));
    }
}

/// <summary>Fails the next save that contains a staged notification, so the business change must roll back with it.</summary>
public sealed class FailNextNotificationSaveInterceptor : SaveChangesInterceptor
{
    private static int _armed;

    public static void Arm() => Interlocked.Exchange(ref _armed, 1);

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var staged = eventData.Context!.ChangeTracker.Entries<Notification>().Any(e => e.State == EntityState.Added);
        if (staged && Interlocked.Exchange(ref _armed, 0) == 1)
            throw new DbUpdateConcurrencyException("Forced failure after notifications were staged.");
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
