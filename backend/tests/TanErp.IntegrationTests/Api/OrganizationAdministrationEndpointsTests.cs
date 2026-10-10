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
using TanErp.Api.Contracts.Organization;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class OrganizationAdministrationEndpointsTests : IAsyncLifetime
{
    private const string AdminToken = "admin-token";
    private const string ViewerToken = "viewer-token";
    private const string OrgBToken = "org-b-token";
    private const string ViewerUid = "org-viewer-uid";

    private static readonly Guid ViewerUserId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f6a01");
    private static readonly Guid ViewerMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f6a02");

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private sealed class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(idToken switch
            {
                AdminToken => TestOnlyDataSeeder.TestFirebaseUid,
                ViewerToken => ViewerUid,
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
        await SeedViewerAsync(db);
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    /// <summary>A member holding only organizations.read, to prove read and write permissions are separate.</summary>
    private static async Task SeedViewerAsync(AppDbContext db)
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var read = await db.Permissions.SingleAsync(p => p.Key == "organizations.read");
        var role = new Role(Guid.NewGuid(), orgId, "Org Viewer", "read only", isActive: true);
        db.Users.Add(new User(ViewerUserId, ViewerUid, "Viewer", "org-viewer@example.test", isActive: true));
        db.Memberships.Add(new Membership(ViewerMembershipId, orgId, TestOnlyDataSeeder.TestBranchId, ViewerUserId, isActive: true));
        db.Roles.Add(role);
        db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), role.Id, orgId, read.Id, PermissionScope.Organization, orgId));
        db.MembershipRoles.Add(new MembershipRole(ViewerMembershipId, role.Id, orgId));
        await db.SaveChangesAsync();
    }

    private AppDbContext NewDb() => _factory.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();

    private Task<HttpResponseMessage> Send(
        HttpMethod method, string url, string token, object? body = null, Guid? ifMatch = null, Guid? membershipId = null, string? idempotencyKey = null)
    {
        var request = new HttpRequestMessage(method, url);
        if (idempotencyKey is not null) request.Headers.Add("Idempotency-Key", idempotencyKey);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Membership-Id", (membershipId ?? TestOnlyDataSeeder.TestMembershipId).ToString());
        if (ifMatch.HasValue) request.Headers.Add("If-Match", $"\"{ifMatch.Value}\"");
        if (body is not null) request.Content = JsonContent.Create(body);
        return _client.SendAsync(request);
    }

    private static async Task<string?> ReadCode(HttpResponseMessage response)
    {
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("code").GetString();
    }

    private async Task<OrganizationProfileResponse> GetProfileAsync()
    {
        var response = await Send(HttpMethod.Get, "/api/v1/admin/organization", AdminToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<OrganizationProfileResponse>())!;
    }

    [Fact]
    public async Task GetProfile_ReturnsOwnOrganizationWithEtag()
    {
        var response = await Send(HttpMethod.Get, "/api/v1/admin/organization", AdminToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var profile = (await response.Content.ReadFromJsonAsync<OrganizationProfileResponse>())!;
        Assert.Equal(TestOnlyDataSeeder.TestOrgId, profile.Id);
        Assert.Equal($"\"{profile.RowVersion}\"", response.Headers.ETag?.Tag);
    }

    [Fact]
    public async Task GetProfile_OrganizationBAdmin_SeesOnlyOrganizationB()
    {
        var response = await Send(HttpMethod.Get, "/api/v1/admin/organization", OrgBToken, membershipId: TestOnlyDataSeeder.TestMembershipBId);

        var profile = (await response.Content.ReadFromJsonAsync<OrganizationProfileResponse>())!;
        Assert.Equal(TestOnlyDataSeeder.TestOrgBId, profile.Id);
    }

    [Fact]
    public async Task UpdateProfile_PersistsTrimmedValuesAndWritesAuditWithoutValues()
    {
        var before = await GetProfileAsync();

        var response = await Send(HttpMethod.Put, "/api/v1/admin/organization", AdminToken,
            new UpdateOrganizationProfileRequest("  บริษัท ทดสอบ  ", "Test Co", "0105536000003", "1 ถนนทดสอบ", null, "02-000-0000"),
            ifMatch: before.RowVersion);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var after = (await response.Content.ReadFromJsonAsync<OrganizationProfileResponse>())!;
        Assert.Equal("บริษัท ทดสอบ", after.Name);
        Assert.Equal("0105536000003", after.TaxIdentifier);
        Assert.NotEqual(before.RowVersion, after.RowVersion);
        Assert.Equal($"\"{after.RowVersion}\"", response.Headers.ETag?.Tag);

        await using var db = NewDb();
        var audit = await db.AuditEvents.AsNoTracking().SingleAsync(a => a.Action == "organization.profile-updated");
        Assert.Contains("taxIdentifier", audit.ChangesJson);
        Assert.DoesNotContain("0105536000003", audit.ChangesJson);
        Assert.DoesNotContain("ถนนทดสอบ", audit.ChangesJson);
        Assert.Equal(before.RowVersion, audit.RowVersionBefore);
        Assert.Equal(after.RowVersion, audit.RowVersionAfter);
    }

    [Fact]
    public async Task UpdateProfile_StaleIfMatch_Returns409AndWithoutIfMatch_Returns428()
    {
        var before = await GetProfileAsync();
        var body = new UpdateOrganizationProfileRequest("x", null, null, null, null, null);

        var stale = await Send(HttpMethod.Put, "/api/v1/admin/organization", AdminToken, body, ifMatch: Guid.NewGuid());
        var missing = await Send(HttpMethod.Put, "/api/v1/admin/organization", AdminToken, body);

        Assert.Equal(HttpStatusCode.Conflict, stale.StatusCode);
        Assert.Equal("ADMIN_VERSION_CONFLICT", await ReadCode(stale));
        Assert.Equal(HttpStatusCode.PreconditionRequired, missing.StatusCode);
        Assert.Equal(before.RowVersion, (await GetProfileAsync()).RowVersion);
    }

    [Fact]
    public async Task UpdateProfile_InvalidTaxIdentifier_Returns422WithoutChangingTheRow()
    {
        var before = await GetProfileAsync();

        var response = await Send(HttpMethod.Put, "/api/v1/admin/organization", AdminToken,
            new UpdateOrganizationProfileRequest("x", null, "0105536000004", null, null, null), ifMatch: before.RowVersion);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("ORGANIZATION_TAX_ID_INVALID", await ReadCode(response));
        Assert.Equal(before.Name, (await GetProfileAsync()).Name);
    }

    [Fact]
    public async Task ReadOnlyMember_CanReadButNotUpdate()
    {
        var read = await Send(HttpMethod.Get, "/api/v1/admin/organization", ViewerToken, membershipId: ViewerMembershipId);
        var write = await Send(HttpMethod.Put, "/api/v1/admin/organization", ViewerToken,
            new UpdateOrganizationProfileRequest("hacked", null, null, null, null, null), ifMatch: Guid.NewGuid(), membershipId: ViewerMembershipId);

        Assert.Equal(HttpStatusCode.OK, read.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, write.StatusCode);
        Assert.Equal("PERMISSION_DENIED", await ReadCode(write));
    }
}
