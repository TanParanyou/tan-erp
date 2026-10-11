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
using TanErp.Api.Contracts.IdentityAccess.RoleManagement;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class RoleManagementEndpointsTests : IAsyncLifetime
{
    private const string AdminToken = "admin-token";       // Test Admin (system role): holds every seeded permission
    private const string ViewerToken = "viewer-token";     // holds roles.assign only (NOT roles.manage)
    private const string CheckerToken = "checker-token";   // roles.manage + roles.assign-approval + a few permissions
    private const string OrgBToken = "org-b-token";
    private const string OrgCToken = "org-c-token";        // sole administrator comes from one non-system role

    private static readonly Guid ViewerMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f7a02");
    private static readonly Guid CheckerMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f7a12");
    private static readonly Guid OrgCId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f7a20");
    private static readonly Guid OrgCBranchId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f7a21");
    private static readonly Guid OrgCMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f7a23");
    private static readonly Guid OrgCRoleId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f7a24");

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private sealed class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(idToken switch
            {
                AdminToken => TestOnlyDataSeeder.TestFirebaseUid,
                ViewerToken => "rm-viewer-uid",
                CheckerToken => "rm-checker-uid",
                OrgBToken => TestOnlyDataSeeder.TestFirebaseUidB,
                OrgCToken => "rm-org-c-uid",
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
        await SeedFixturesAsync(db);
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private static async Task SeedFixturesAsync(AppDbContext db)
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var permissions = await db.Permissions.ToDictionaryAsync(p => p.Key);

        Role MakeRole(Guid org, string name, params string[] keys)
        {
            var role = new Role(Guid.NewGuid(), org, name, name, isActive: true);
            db.Roles.Add(role);
            foreach (var key in keys)
                db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), role.Id, org, permissions[key].Id, PermissionScope.Organization, org));
            return role;
        }

        void MakeMember(Guid org, Guid? branch, Guid membershipId, string uid, Role role)
        {
            var userId = Guid.NewGuid();
            db.Users.Add(new User(userId, uid, uid, $"{uid}@example.test", isActive: true));
            db.Memberships.Add(new Membership(membershipId, org, branch, userId, isActive: true));
            db.MembershipRoles.Add(new MembershipRole(membershipId, role.Id, org));
        }

        MakeMember(orgId, TestOnlyDataSeeder.TestBranchId, ViewerMembershipId, "rm-viewer-uid", MakeRole(orgId, "RM Viewer", "roles.assign"));
        MakeMember(orgId, TestOnlyDataSeeder.TestBranchId, CheckerMembershipId, "rm-checker-uid", MakeRole(orgId, "RM Checker",
            "roles.manage", "roles.assign-approval", "estimates.approve", "estimates.read", "purchase-orders.approve", "customers.read"));

        // Organization C: its only administrator (users.manage) comes from one NON-system role.
        db.Organizations.Add(new TanErp.Domain.Organization.Organization(OrgCId, "RM Org C"));
        db.Branches.Add(new Branch(OrgCBranchId, OrgCId, "C01", "สาขา C"));
        var orgCRole = new Role(OrgCRoleId, OrgCId, "Org C Admin", "Org C Admin", isActive: true);
        db.Roles.Add(orgCRole);
        foreach (var key in new[] { "users.manage", "roles.manage", "customers.read" })
            db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), orgCRole.Id, OrgCId, permissions[key].Id, PermissionScope.Organization, OrgCId));
        MakeMember(OrgCId, OrgCBranchId, OrgCMembershipId, "rm-org-c-uid", orgCRole);

        await db.SaveChangesAsync();
    }

    private AppDbContext NewDb() => _factory.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();

    private Task<HttpResponseMessage> Send(
        HttpMethod method, string url, string token, object? body = null, Guid? ifMatch = null, Guid? membershipId = null, string? idempotencyKey = null)
    {
        var request = new HttpRequestMessage(method, url);
        if (idempotencyKey is not null) request.Headers.Add("Idempotency-Key", idempotencyKey);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Membership-Id", (membershipId ?? MembershipFor(token)).ToString());
        if (ifMatch.HasValue) request.Headers.Add("If-Match", $"\"{ifMatch.Value}\"");
        if (body is not null) request.Content = JsonContent.Create(body);
        return _client.SendAsync(request);
    }

    private static Guid MembershipFor(string token) => token switch
    {
        ViewerToken => ViewerMembershipId,
        CheckerToken => CheckerMembershipId,
        OrgBToken => TestOnlyDataSeeder.TestMembershipBId,
        OrgCToken => OrgCMembershipId,
        _ => TestOnlyDataSeeder.TestMembershipId
    };

    private static async Task<string?> ReadCode(HttpResponseMessage response)
    {
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("code").GetString();
    }

    private static async Task<T> ReadAs<T>(HttpResponseMessage response) => (await response.Content.ReadFromJsonAsync<T>())!;

    /// <summary>Inserts a custom role straight into the database (the create endpoint is exercised separately).</summary>
    private async Task<Guid> SeedRoleAsync(string name, string[]? keys = null, bool active = true, Guid? orgId = null)
    {
        await using var db = NewDb();
        var org = orgId ?? TestOnlyDataSeeder.TestOrgId;
        var role = new Role(Guid.NewGuid(), org, name, "seeded " + name, active);
        db.Roles.Add(role);
        foreach (var key in keys ?? Array.Empty<string>())
        {
            var permission = await db.Permissions.SingleAsync(p => p.Key == key);
            db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), role.Id, org, permission.Id, PermissionScope.Organization, org));
        }

        await db.SaveChangesAsync();
        return role.Id;
    }

    // ------------------------------------------------------------------ catalog

    [Fact]
    public async Task Catalog_ReturnsActivePermissionsSortedWithTheApprovalFlag()
    {
        var response = await Send(HttpMethod.Get, "/api/v1/admin/permissions", AdminToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = (await ReadAs<PermissionCatalogResponse>(response)).Items;
        Assert.Equal(items.Select(i => i.Key).OrderBy(k => k, StringComparer.Ordinal), items.Select(i => i.Key));
        Assert.True(items.Single(i => i.Key == "estimates.approve").IsApprovalPermission);
        Assert.True(items.Single(i => i.Key == "purchase-orders.approve").IsApprovalPermission);
        Assert.False(items.Single(i => i.Key == "customers.read").IsApprovalPermission);
    }

    [Fact]
    public async Task EveryEndpoint_WithoutRolesManage_Returns403WithTheSameCodeForAnyId()
    {
        var anyId = Guid.NewGuid();
        var urls = new[] { "/api/v1/admin/permissions", "/api/v1/admin/role-definitions", $"/api/v1/admin/role-definitions/{anyId}",
            $"/api/v1/admin/role-definitions/{TestOnlyDataSeeder.TestOrgId}" };

        foreach (var url in urls)
        {
            var response = await Send(HttpMethod.Get, url, ViewerToken);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal("PERMISSION_DENIED", await ReadCode(response));
        }
    }

    // ------------------------------------------------------------------ list

    [Fact]
    public async Task List_SearchesFiltersByStatusPaginatesAndStaysInsideTheOrganization()
    {
        await SeedRoleAsync("Zeta Planner", new[] { "customers.read" });
        await SeedRoleAsync("Zeta Retired", active: false);
        await SeedRoleAsync("Zeta Org B", orgId: TestOnlyDataSeeder.TestOrgBId);

        var all = await ReadAs<RoleDefinitionListResponse>(await Send(HttpMethod.Get, "/api/v1/admin/role-definitions?search=ZETA", AdminToken));
        var inactive = await ReadAs<RoleDefinitionListResponse>(await Send(HttpMethod.Get, "/api/v1/admin/role-definitions?search=zeta&status=inactive", AdminToken));
        var paged = await ReadAs<RoleDefinitionListResponse>(await Send(HttpMethod.Get, "/api/v1/admin/role-definitions?search=zeta&limit=1&page=2&sortOrder=desc", AdminToken));
        var escaped = await ReadAs<RoleDefinitionListResponse>(await Send(HttpMethod.Get, "/api/v1/admin/role-definitions?search=%25", AdminToken));

        Assert.Equal(new[] { "Zeta Planner", "Zeta Retired" }, all.Items.Select(i => i.Name).ToArray());
        Assert.Equal(new[] { "Zeta Retired" }, inactive.Items.Select(i => i.Name).ToArray());
        Assert.Equal(2, paged.Pagination.TotalCount);
        Assert.Equal(2, paged.Pagination.TotalPages);
        Assert.Equal(new[] { "Zeta Planner" }, paged.Items.Select(i => i.Name).ToArray()); // desc: Retired, Planner
        Assert.Empty(escaped.Items); // a literal % is escaped, not a wildcard
        Assert.Contains(all.Items, i => i.Name == "Zeta Planner" && i.PermissionCount == 1 && !i.IsSystem);
    }

    [Fact]
    public async Task List_ShowsTheSeededSystemRoleAsSystemWithMembers()
    {
        var page = await ReadAs<RoleDefinitionListResponse>(await Send(HttpMethod.Get, "/api/v1/admin/role-definitions?search=Test%20Admin", AdminToken));

        var admin = page.Items.Single(i => i.Name == "Test Admin");
        Assert.True(admin.IsSystem);
        Assert.True(admin.RequiresApproval);
        Assert.True(admin.MemberCount >= 1);
        Assert.DoesNotContain(page.Items, i => i.Name == "Test Admin B");
    }

    [Theory]
    [InlineData("status=bogus")]
    [InlineData("sortBy=bogus")]
    [InlineData("sortOrder=sideways")]
    public async Task List_InvalidQuery_Returns400(string query)
    {
        var response = await Send(HttpMethod.Get, "/api/v1/admin/role-definitions?" + query, AdminToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("REQUEST_VALIDATION_FAILED", await ReadCode(response));
    }

    // ------------------------------------------------------------------ detail

    [Fact]
    public async Task Get_ReturnsGrantsCountsAndEtag_AndOtherOrganizationIdIs404()
    {
        var roleId = await SeedRoleAsync("Detail Role", new[] { "customers.read", "estimates.approve" });
        var otherOrgRole = await SeedRoleAsync("Other Org Role", orgId: TestOnlyDataSeeder.TestOrgBId);

        var response = await Send(HttpMethod.Get, $"/api/v1/admin/role-definitions/{roleId}", AdminToken);
        var other = await Send(HttpMethod.Get, $"/api/v1/admin/role-definitions/{otherOrgRole}", AdminToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var role = await ReadAs<RoleDefinitionResponse>(response);
        Assert.Equal(new[] { "customers.read", "estimates.approve" }, role.Permissions.Select(p => p.PermissionKey).ToArray());
        Assert.All(role.Permissions, p => Assert.Equal("organization", p.Scope));
        Assert.True(role.RequiresApproval);
        Assert.Equal(0, role.MemberCount);
        Assert.Null(role.PendingChange);
        Assert.Equal($"\"{role.RowVersion}\"", response.Headers.ETag?.Tag);
        Assert.Equal(HttpStatusCode.NotFound, other.StatusCode);
        Assert.Equal("RESOURCE_NOT_FOUND", await ReadCode(other));
    }
}
