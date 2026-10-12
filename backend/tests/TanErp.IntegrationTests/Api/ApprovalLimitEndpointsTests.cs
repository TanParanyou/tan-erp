using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.IdentityAccess.ApprovalLimits;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class ApprovalLimitEndpointsTests : IAsyncLifetime
{
    private const string AdminToken = "al-admin-token";     // Test Admin (system role): holds approval-limits.manage
    private const string ViewerToken = "al-viewer-token";   // holds roles.manage only (NOT approval-limits.manage)
    private const string OrgBToken = "al-org-b-token";
    private const string Url = "/api/v1/admin/approval-limits";

    private static readonly Guid ViewerMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f7b02");

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private sealed class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(idToken switch
            {
                AdminToken => TestOnlyDataSeeder.TestFirebaseUid,
                ViewerToken => "al-viewer-uid",
                OrgBToken => TestOnlyDataSeeder.TestFirebaseUidB,
                _ => null
            });

        public async Task<FirebaseIdentity?> VerifyIdentityAsync(string idToken, CancellationToken cancellationToken = default)
        {
            var uid = await VerifyTokenAsync(idToken, cancellationToken);
            return uid is null ? null : new FirebaseIdentity(uid, null, false);
        }
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
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true);

        var org = TestOnlyDataSeeder.TestOrgId;
        var permission = await db.Permissions.SingleAsync(p => p.Key == "roles.manage");
        var viewerRole = new Role(Guid.NewGuid(), org, "AL Viewer", "AL Viewer", isActive: true);
        var userId = Guid.NewGuid();
        db.Roles.Add(viewerRole);
        db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), viewerRole.Id, org, permission.Id, PermissionScope.Organization, org));
        db.Users.Add(new User(userId, "al-viewer-uid", "al-viewer", "al-viewer@example.test", isActive: true));
        db.Memberships.Add(new Membership(ViewerMembershipId, org, TestOnlyDataSeeder.TestBranchId, userId, isActive: true));
        db.MembershipRoles.Add(new MembershipRole(ViewerMembershipId, viewerRole.Id, org));
        await db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private AppDbContext NewDb() => _factory.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();

    private Task<HttpResponseMessage> Send(
        HttpMethod method, string url, string token, object? body = null, Guid? ifMatch = null, string? idempotencyKey = null)
    {
        var request = new HttpRequestMessage(method, url);
        if (idempotencyKey is not null) request.Headers.Add("Idempotency-Key", idempotencyKey);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var membershipId = token switch
        {
            ViewerToken => ViewerMembershipId,
            OrgBToken => TestOnlyDataSeeder.TestMembershipBId,
            _ => TestOnlyDataSeeder.TestMembershipId
        };
        request.Headers.Add("X-Membership-Id", membershipId.ToString());
        if (ifMatch.HasValue) request.Headers.Add("If-Match", $"\"{ifMatch.Value}\"");
        if (body is not null) request.Content = JsonContent.Create(body);
        return _client.SendAsync(request);
    }

    private static async Task<string?> ReadCode(HttpResponseMessage response)
    {
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("code").GetString();
    }

    private static async Task<T> ReadAs<T>(HttpResponseMessage response) => (await response.Content.ReadFromJsonAsync<T>())!;

    private static string NewKey() => Guid.NewGuid().ToString("N");

    private async Task<Guid> SeedRoleAsync(string name, bool active = true, Guid? orgId = null)
    {
        await using var db = NewDb();
        var role = new Role(Guid.NewGuid(), orgId ?? TestOnlyDataSeeder.TestOrgId, name, name, active);
        db.Roles.Add(role);
        await db.SaveChangesAsync();
        return role.Id;
    }

    private static object Body(Guid roleId, string type = "estimate", Guid? branchId = null, bool unlimited = false, decimal? max = 100_000m, string currency = "THB") =>
        new { roleId, documentType = type, branchId, currency, isUnlimited = unlimited, maxAmount = max };

    private async Task<ApprovalLimitResponse> CreateAsync(object body) =>
        await ReadAs<ApprovalLimitResponse>(await Send(HttpMethod.Post, Url, AdminToken, body, idempotencyKey: NewKey()));

    private async Task ExpectPostAsync(HttpStatusCode status, string code, object body)
    {
        var response = await Send(HttpMethod.Post, Url, AdminToken, body, idempotencyKey: NewKey());
        Assert.Equal(status, response.StatusCode);
        Assert.Equal(code, await ReadCode(response));
    }

    [Fact]
    public async Task EveryEndpoint_WithoutApprovalLimitsManage_Returns403WithTheSameCode()
    {
        var anyId = Guid.NewGuid();
        var responses = new[]
        {
            await Send(HttpMethod.Get, Url, ViewerToken),
            await Send(HttpMethod.Get, $"{Url}/options", ViewerToken),
            await Send(HttpMethod.Get, $"{Url}/{anyId}", ViewerToken),
            await Send(HttpMethod.Post, Url, ViewerToken, Body(anyId), idempotencyKey: NewKey()),
            await Send(HttpMethod.Put, $"{Url}/{anyId}", ViewerToken, new { isUnlimited = true, isActive = true }, ifMatch: Guid.NewGuid())
        };

        foreach (var response in responses)
        {
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal("PERMISSION_DENIED", await ReadCode(response));
        }
    }

    [Fact]
    public async Task Options_ListActiveRolesAndBranchesOfTheOrganizationOnly()
    {
        var inactive = await SeedRoleAsync("AL Retired", active: false);
        await SeedRoleAsync("AL Other Org", orgId: TestOnlyDataSeeder.TestOrgBId);

        var options = await ReadAs<ApprovalLimitOptionsResponse>(await Send(HttpMethod.Get, $"{Url}/options", AdminToken));

        Assert.Contains(options.Roles, r => r.Name == "Test Admin");
        Assert.DoesNotContain(options.Roles, r => r.Id == inactive || r.Name == "AL Other Org");
        Assert.Contains(options.Branches, b => b.Id == TestOnlyDataSeeder.TestBranchId);
        Assert.Equal(new[] { "estimate", "purchase_order", "change_order" }, options.DocumentTypes);
        Assert.Equal(new[] { "THB" }, options.Currencies);
    }

    [Fact]
    public async Task Create_ReturnsEtag_Audits_Replays_AndRejectsKeyReuseMissingKeyAndDuplicates()
    {
        var roleId = await SeedRoleAsync("AL Estimator");
        var key = NewKey();

        var response = await Send(HttpMethod.Post, Url, AdminToken, Body(roleId), idempotencyKey: key);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await ReadAs<ApprovalLimitResponse>(response);
        Assert.Equal($"\"{created.RowVersion}\"", response.Headers.ETag!.Tag);
        Assert.Null(created.Branch);

        var replay = await ReadAs<ApprovalLimitResponse>(await Send(HttpMethod.Post, Url, AdminToken, Body(roleId), idempotencyKey: key));
        Assert.Equal(created.Id, replay.Id);
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", await ReadCode(await Send(HttpMethod.Post, Url, AdminToken, Body(roleId, max: 1m), idempotencyKey: key)));
        Assert.Equal("IDEMPOTENCY_KEY_REQUIRED", await ReadCode(await Send(HttpMethod.Post, Url, AdminToken, Body(roleId, "change_order"))));

        var branchRow = await CreateAsync(Body(roleId, branchId: TestOnlyDataSeeder.TestBranchId, max: 5_000m));
        Assert.Equal(TestOnlyDataSeeder.TestBranchId, branchRow.Branch!.Id);
        var unlimited = await CreateAsync(Body(roleId, "purchase_order", unlimited: true, max: null));
        Assert.True(unlimited.IsUnlimited);
        Assert.Null(unlimited.MaxAmount);

        await ExpectPostAsync(HttpStatusCode.Conflict, "APPROVAL_LIMIT_ALREADY_EXISTS", Body(roleId, max: 1m));
        await ExpectPostAsync(HttpStatusCode.Conflict, "APPROVAL_LIMIT_ALREADY_EXISTS", Body(roleId, branchId: TestOnlyDataSeeder.TestBranchId, max: 1m));

        await using var db = NewDb();
        Assert.Equal(3, await db.ApprovalLimits.CountAsync(l => l.RoleId == roleId));
        Assert.Equal(new[] { "approval-limits.created" },
            await db.AuditEvents.Where(a => a.ResourceType == "ApprovalLimit" && a.ResourceId == created.Id.ToString()).Select(a => a.Action).ToArrayAsync());
    }

    [Fact]
    public async Task Create_ValidatesFieldsAndReferences()
    {
        var roleId = await SeedRoleAsync("AL Validation");
        var inactiveRole = await SeedRoleAsync("AL Inactive Ref", active: false);
        var orgBRole = await SeedRoleAsync("AL Org B Ref", orgId: TestOnlyDataSeeder.TestOrgBId);
        const string Invalid = "REQUEST_VALIDATION_FAILED";

        await ExpectPostAsync(HttpStatusCode.BadRequest, Invalid, Body(roleId, "credit_note"));
        await ExpectPostAsync(HttpStatusCode.BadRequest, Invalid, Body(roleId, "invoice"));
        await ExpectPostAsync(HttpStatusCode.BadRequest, Invalid, Body(roleId, currency: "USD"));
        await ExpectPostAsync(HttpStatusCode.BadRequest, Invalid, Body(roleId, unlimited: true, max: 5m));
        await ExpectPostAsync(HttpStatusCode.BadRequest, Invalid, Body(roleId, max: null));
        await ExpectPostAsync(HttpStatusCode.BadRequest, Invalid, Body(roleId, max: -1m));
        await ExpectPostAsync(HttpStatusCode.BadRequest, Invalid, Body(roleId, max: 1.005m));
        await ExpectPostAsync(HttpStatusCode.BadRequest, Invalid, Body(Guid.Empty));
        await ExpectPostAsync(HttpStatusCode.NotFound, "APPROVAL_LIMIT_ROLE_NOT_FOUND", Body(Guid.NewGuid()));
        await ExpectPostAsync(HttpStatusCode.NotFound, "APPROVAL_LIMIT_ROLE_NOT_FOUND", Body(inactiveRole));
        await ExpectPostAsync(HttpStatusCode.NotFound, "APPROVAL_LIMIT_ROLE_NOT_FOUND", Body(orgBRole));
        await ExpectPostAsync(HttpStatusCode.NotFound, "BRANCH_NOT_FOUND", Body(roleId, branchId: Guid.NewGuid()));
        await ExpectPostAsync(HttpStatusCode.NotFound, "BRANCH_NOT_FOUND", Body(roleId, branchId: TestOnlyDataSeeder.TestBranchBId));
    }

    [Fact]
    public async Task Update_ChangesTheRowWithIfMatch_AndRejectsStaleMissingOrInvalidInput()
    {
        var created = await CreateAsync(Body(await SeedRoleAsync("AL Update")));

        var updated = await Send(HttpMethod.Put, $"{Url}/{created.Id}", AdminToken, new { isUnlimited = false, maxAmount = 250_000.5m, isActive = true }, ifMatch: created.RowVersion);
        Assert.Equal(HttpStatusCode.OK, updated.StatusCode);
        var row = await ReadAs<ApprovalLimitResponse>(updated);
        Assert.Equal(250_000.5m, row.MaxAmount);
        Assert.Equal($"\"{row.RowVersion}\"", updated.Headers.ETag!.Tag);

        Assert.Equal("ADMIN_VERSION_CONFLICT", await ReadCode(await Send(HttpMethod.Put, $"{Url}/{created.Id}", AdminToken, new { isUnlimited = true, isActive = true }, ifMatch: created.RowVersion)));
        Assert.Equal("IF_MATCH_REQUIRED", await ReadCode(await Send(HttpMethod.Put, $"{Url}/{created.Id}", AdminToken, new { isUnlimited = true, isActive = true })));
        Assert.Equal("REQUEST_VALIDATION_FAILED", await ReadCode(await Send(HttpMethod.Put, $"{Url}/{created.Id}", AdminToken, new { isUnlimited = true, maxAmount = 5m, isActive = true }, ifMatch: row.RowVersion)));

        var off = await ReadAs<ApprovalLimitResponse>(await Send(HttpMethod.Put, $"{Url}/{created.Id}", AdminToken, new { isUnlimited = false, maxAmount = 250_000.5m, isActive = false }, ifMatch: row.RowVersion));
        Assert.False(off.IsActive);
        await using var db = NewDb();
        Assert.Equal(2, await db.AuditEvents.CountAsync(a => a.Action == "approval-limits.updated" && a.ResourceId == created.Id.ToString()));
    }

    [Fact]
    public async Task List_FiltersPaginatesIncludesSeededRows_AndOtherOrganizationsRowsAreNotFound()
    {
        var roleId = await SeedRoleAsync("AL Zeta");
        await CreateAsync(Body(roleId));
        await CreateAsync(Body(roleId, "purchase_order", max: 10m));
        var inactive = await CreateAsync(Body(roleId, "change_order", max: 20m));
        await Send(HttpMethod.Put, $"{Url}/{inactive.Id}", AdminToken, new { isUnlimited = false, maxAmount = 20m, isActive = false }, ifMatch: inactive.RowVersion);

        var forRole = await ReadAs<ApprovalLimitListResponse>(await Send(HttpMethod.Get, $"{Url}?roleId={roleId}", AdminToken));
        var estimates = await ReadAs<ApprovalLimitListResponse>(await Send(HttpMethod.Get, $"{Url}?documentType=estimate", AdminToken));
        var inactiveOnly = await ReadAs<ApprovalLimitListResponse>(await Send(HttpMethod.Get, $"{Url}?roleId={roleId}&status=inactive", AdminToken));
        var paged = await ReadAs<ApprovalLimitListResponse>(await Send(HttpMethod.Get, $"{Url}?roleId={roleId}&limit=2&page=2", AdminToken));

        Assert.Equal(3, forRole.Pagination.TotalCount);
        Assert.Contains(estimates.Items, i => i.Role.Name == "Test Admin" && i.IsUnlimited);   // seeded by TestOnlyDataSeeder
        Assert.DoesNotContain(estimates.Items, i => i.Role.Name == "Test Admin B");            // other organization
        Assert.Equal(new[] { inactive.Id }, inactiveOnly.Items.Select(i => i.Id).ToArray());
        Assert.Equal(2, paged.Pagination.TotalPages);
        Assert.Single(paged.Items);
        Assert.Equal("REQUEST_VALIDATION_FAILED", await ReadCode(await Send(HttpMethod.Get, $"{Url}?documentType=credit_note", AdminToken)));

        var foreign = (await ReadAs<ApprovalLimitListResponse>(await Send(HttpMethod.Get, Url, OrgBToken))).Items.First();
        Assert.Equal(HttpStatusCode.NotFound, (await Send(HttpMethod.Get, $"{Url}/{foreign.Id}", AdminToken)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound,
            (await Send(HttpMethod.Put, $"{Url}/{foreign.Id}", AdminToken, new { isUnlimited = true, isActive = true }, ifMatch: foreign.RowVersion)).StatusCode);
    }
}
