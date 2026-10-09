using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.Notifications;
using TanErp.Api.ErrorHandling;
using TanErp.Domain.Notifications;
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
    private Guid[] _mineIds = [];
    private Guid _orgBRowId;

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(idToken switch
            {
                "token-org-a" => TestOnlyDataSeeder.TestFirebaseUid,
                "token-org-b" => TestOnlyDataSeeder.TestFirebaseUidB,
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
}
