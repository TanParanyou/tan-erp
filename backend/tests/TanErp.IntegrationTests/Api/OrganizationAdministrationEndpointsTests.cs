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

    private static CreateBranchRequest NewBranch(string code, string? taxCode = null) =>
        new(code, "สาขา " + code, "Branch " + code, taxCode, "ที่อยู่", null, "02-111-1111");

    private Task<HttpResponseMessage> CreateBranch(CreateBranchRequest body, string? key = null, string token = AdminToken, Guid? membershipId = null) =>
        Send(HttpMethod.Post, "/api/v1/admin/branches", token, body, idempotencyKey: key ?? Guid.NewGuid().ToString(), membershipId: membershipId);

    [Fact]
    public async Task CreateBranch_PersistsDetailsWritesAuditAndReturnsEtag()
    {
        var response = await CreateBranch(NewBranch("B10", "00010"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var branch = (await response.Content.ReadFromJsonAsync<BranchResponse>())!;
        Assert.Equal("B10", branch.Code);
        Assert.Equal("00010", branch.TaxBranchCode);
        Assert.True(branch.IsActive);
        Assert.Equal($"\"{branch.RowVersion}\"", response.Headers.ETag?.Tag);

        await using var db = NewDb();
        var audit = await db.AuditEvents.AsNoTracking().SingleAsync(a => a.Action == "branches.created" && a.ResourceId == branch.Id.ToString());
        Assert.Equal(branch.Id, audit.BranchId);
        Assert.DoesNotContain("ที่อยู่", audit.ChangesJson);
    }

    [Fact]
    public async Task CreateBranch_DuplicateCodeAndDuplicateTaxCode_ReturnDifferentConflictCodes()
    {
        Assert.Equal(HttpStatusCode.Created, (await CreateBranch(NewBranch("B11", "00011"))).StatusCode);

        var dupCode = await CreateBranch(NewBranch("B11", "00099"));
        var dupTax = await CreateBranch(NewBranch("B12", "00011"));

        Assert.Equal(HttpStatusCode.Conflict, dupCode.StatusCode);
        Assert.Equal("BRANCH_CODE_ALREADY_EXISTS", await ReadCode(dupCode));
        Assert.Equal(HttpStatusCode.Conflict, dupTax.StatusCode);
        Assert.Equal("BRANCH_TAX_CODE_ALREADY_EXISTS", await ReadCode(dupTax));
    }

    [Fact]
    public async Task CreateBranch_SameCodeInAnotherOrganization_IsAllowed()
    {
        // Seeded organization A already owns "B01"; organization B may create its own "B01".
        var response = await CreateBranch(NewBranch("B01"), token: OrgBToken, membershipId: TestOnlyDataSeeder.TestMembershipBId);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateBranch_InvalidCodeOrTaxCode_Returns422()
    {
        var badCode = await CreateBranch(NewBranch("bad code"));
        var badTax = await CreateBranch(NewBranch("B13", "12"));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, badCode.StatusCode);
        Assert.Equal("BRANCH_CODE_INVALID", await ReadCode(badCode));
        Assert.Equal("BRANCH_TAX_CODE_INVALID", await ReadCode(badTax));
    }

    [Fact]
    public async Task CreateBranch_ReplayAndKeyReuseAndMissingKey()
    {
        var key = "branch-create-" + Guid.NewGuid();
        var first = await CreateBranch(NewBranch("B14"), key);
        var second = await CreateBranch(NewBranch("B14"), key);
        var reused = await CreateBranch(NewBranch("B15"), key);
        var noKey = await Send(HttpMethod.Post, "/api/v1/admin/branches", AdminToken, NewBranch("B16"));

        Assert.Equal(HttpStatusCode.Created, second.StatusCode);
        Assert.Equal((await first.Content.ReadFromJsonAsync<BranchResponse>())!.Id, (await second.Content.ReadFromJsonAsync<BranchResponse>())!.Id);
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", await ReadCode(reused));
        Assert.Equal("IDEMPOTENCY_KEY_REQUIRED", await ReadCode(noKey));
        await using var db = NewDb();
        Assert.Equal(1, await db.Branches.CountAsync(b => b.Code == "B14"));
    }

    [Fact]
    public async Task ListBranches_IncludesInactiveFiltersByStatusAndStaysInsideOrganization()
    {
        var created = (await (await CreateBranch(NewBranch("B17"))).Content.ReadFromJsonAsync<BranchResponse>())!;
        await using (var db = NewDb())
        {
            var branch = await db.Branches.SingleAsync(b => b.Id == created.Id);
            branch.Deactivate();
            await db.SaveChangesAsync();
        }

        var all = await ListAsync("", AdminToken, TestOnlyDataSeeder.TestMembershipId);
        var inactive = await ListAsync("?status=inactive", AdminToken, TestOnlyDataSeeder.TestMembershipId);
        var orgB = await ListAsync("", OrgBToken, TestOnlyDataSeeder.TestMembershipBId);
        var invalid = await Send(HttpMethod.Get, "/api/v1/admin/branches?status=bogus", AdminToken);

        Assert.Contains(all, b => b.Id == created.Id && !b.IsActive);
        Assert.Equal(new[] { created.Id }, inactive.Select(b => b.Id).ToArray());
        Assert.DoesNotContain(orgB, b => b.Id == created.Id);
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
    }

    private async Task<IReadOnlyList<BranchResponse>> ListAsync(string query, string token, Guid membershipId)
    {
        var response = await Send(HttpMethod.Get, "/api/v1/admin/branches" + query, token, membershipId: membershipId);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<IReadOnlyList<BranchResponse>>())!;
    }

    [Fact]
    public async Task GetBranch_OtherOrganizationId_Returns404ButWithoutPermissionReturns403ForAnyId()
    {
        var orgBBranch = TestOnlyDataSeeder.TestBranchBId;
        var ownBranch = TestOnlyDataSeeder.TestBranchId;

        var otherOrg = await Send(HttpMethod.Get, $"/api/v1/admin/branches/{orgBBranch}", AdminToken);
        var viewerExisting = await Send(HttpMethod.Get, $"/api/v1/admin/branches/{ownBranch}", ViewerToken, membershipId: ViewerMembershipId);
        var viewerMissing = await Send(HttpMethod.Get, $"/api/v1/admin/branches/{Guid.NewGuid()}", ViewerToken, membershipId: ViewerMembershipId);

        Assert.Equal(HttpStatusCode.NotFound, otherOrg.StatusCode);
        Assert.Equal("RESOURCE_NOT_FOUND", await ReadCode(otherOrg));
        Assert.Equal(HttpStatusCode.Forbidden, viewerExisting.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, viewerMissing.StatusCode); // identical: existence is never revealed
    }

    private async Task<BranchResponse> CreateAndReadAsync(string code, string? taxCode = null)
    {
        var response = await CreateBranch(NewBranch(code, taxCode));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<BranchResponse>())!;
    }

    [Fact]
    public async Task UpdateBranch_ChangesDetailsKeepsCodeIgnoresCodeInBodyAndAudits()
    {
        var created = await CreateAndReadAsync("B20");

        var response = await Send(HttpMethod.Put, $"/api/v1/admin/branches/{created.Id}", AdminToken,
            new { code = "HACKED", name = "ชื่อใหม่", nameEn = "New", taxBranchCode = "00020", addressTh = "x", addressEn = (string?)null, phone = "1" },
            ifMatch: created.RowVersion);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = (await response.Content.ReadFromJsonAsync<BranchResponse>())!;
        Assert.Equal("B20", updated.Code);
        Assert.Equal("ชื่อใหม่", updated.Name);
        Assert.Equal("00020", updated.TaxBranchCode);
        Assert.NotEqual(created.RowVersion, updated.RowVersion);
        Assert.Equal($"\"{updated.RowVersion}\"", response.Headers.ETag?.Tag);

        await using var db = NewDb();
        var audit = await db.AuditEvents.AsNoTracking().SingleAsync(a => a.Action == "branches.updated" && a.ResourceId == created.Id.ToString());
        Assert.Contains("taxBranchCode", audit.ChangesJson);
        Assert.DoesNotContain("00020", audit.ChangesJson);
        Assert.Equal(created.RowVersion, audit.RowVersionBefore);
    }

    [Fact]
    public async Task UpdateBranch_StaleVersion_Returns409AndMissingIfMatch_Returns428()
    {
        var created = await CreateAndReadAsync("B21");
        var body = new UpdateBranchRequest("n", null, null, null, null, null);

        var stale = await Send(HttpMethod.Put, $"/api/v1/admin/branches/{created.Id}", AdminToken, body, ifMatch: Guid.NewGuid());
        var missing = await Send(HttpMethod.Put, $"/api/v1/admin/branches/{created.Id}", AdminToken, body);

        Assert.Equal("ADMIN_VERSION_CONFLICT", await ReadCode(stale));
        Assert.Equal(HttpStatusCode.PreconditionRequired, missing.StatusCode);
    }

    [Fact]
    public async Task UpdateBranch_TaxCodeOwnedByAnotherBranch_Returns409WithTaxCodeError()
    {
        await CreateAndReadAsync("B22", "00022");
        var other = await CreateAndReadAsync("B23", "00023");

        var response = await Send(HttpMethod.Put, $"/api/v1/admin/branches/{other.Id}", AdminToken,
            new UpdateBranchRequest("n", null, "00022", null, null, null), ifMatch: other.RowVersion);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("BRANCH_TAX_CODE_ALREADY_EXISTS", await ReadCode(response));
    }

    [Fact]
    public async Task UpdateBranch_OtherOrganizationBranch_Returns404AndLeavesItUntouched()
    {
        await using var before = NewDb();
        var orgBBranch = await before.Branches.AsNoTracking().SingleAsync(b => b.Id == TestOnlyDataSeeder.TestBranchBId);

        var response = await Send(HttpMethod.Put, $"/api/v1/admin/branches/{orgBBranch.Id}", AdminToken,
            new UpdateBranchRequest("hijacked", null, null, null, null, null), ifMatch: orgBBranch.RowVersion);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await using var after = NewDb();
        Assert.Equal(orgBBranch.Name, (await after.Branches.AsNoTracking().SingleAsync(b => b.Id == orgBBranch.Id)).Name);
    }
}
