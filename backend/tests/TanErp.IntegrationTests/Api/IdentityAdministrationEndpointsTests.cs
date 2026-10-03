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
using TanErp.Api.Contracts.IdentityAccess;
using TanErp.Api.Contracts.IdentityAccess.Administration;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class IdentityAdministrationEndpointsTests : IAsyncLifetime
{
    private const string AdminToken = "admin-token";
    private const string SecondAdminToken = "second-admin-token";
    private const string ViewerToken = "viewer-token";
    private const string OrgBToken = "org-b-token";
    private const string InviteeToken = "invitee-token";
    private const string UnverifiedInviteeToken = "unverified-invitee-token";
    private const string OtherIdentityToken = "other-identity-token";

    private const string SecondAdminUid = "second-admin-uid";
    private const string ViewerUid = "viewer-uid";
    private const string InviteeEmail = "New.Person@Example.test";

    private static readonly Guid SecondAdminUserId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5a01");
    private static readonly Guid SecondAdminMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5a02");
    private static readonly Guid ViewerUserId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5a03");
    private static readonly Guid ViewerMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5a04");

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private Guid _viewerRoleId;
    private Guid _approverRoleId;
    private Guid _secondAdminRoleId;
    private Guid _beyondCallerRoleId;

    private sealed class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(idToken switch
            {
                AdminToken => TestOnlyDataSeeder.TestFirebaseUid,
                SecondAdminToken => SecondAdminUid,
                ViewerToken => ViewerUid,
                OrgBToken => TestOnlyDataSeeder.TestFirebaseUidB,
                InviteeToken or UnverifiedInviteeToken => "invitee-uid",
                OtherIdentityToken => "other-identity-uid",
                _ => null
            });

        public async Task<FirebaseIdentity?> VerifyIdentityAsync(string idToken, CancellationToken cancellationToken = default)
        {
            var uid = await VerifyTokenAsync(idToken, cancellationToken);
            if (uid is null) return null;

            return idToken switch
            {
                InviteeToken => new FirebaseIdentity(uid, "  new.person@EXAMPLE.test ", true),
                UnverifiedInviteeToken => new FirebaseIdentity(uid, InviteeEmail, false),
                OtherIdentityToken => new FirebaseIdentity(uid, InviteeEmail, true),
                _ => new FirebaseIdentity(uid, null, false)
            };
        }
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Database"] = _postgres.GetConnectionString(),
                    ["SeedTestData"] = "true"
                });
            });
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
        await SeedAdministrationFixturesAsync(db);
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    // ------------------------------------------------------------------ creation

    [Fact]
    public async Task CreateUser_CreatesPendingUserWithRoleAndAuditWithoutPersonalData()
    {
        var response = await Send(HttpMethod.Post, "/api/v1/admin/users", AdminToken, new CreateAdminUserRequest(
            "ผู้ใช้ใหม่", "someone@example.test", TestOnlyDataSeeder.TestBranchId, new[] { _viewerRoleId }));

        Assert.True(response.StatusCode == HttpStatusCode.Created, await response.Content.ReadAsStringAsync());
        var user = (await response.Content.ReadFromJsonAsync<AdminUserResponse>())!;
        Assert.Equal("pending", user.Status);
        Assert.Single(user.Memberships);
        Assert.Contains(user.Memberships[0].Roles, role => role.Id == _viewerRoleId);
        Assert.Empty(user.Memberships[0].PendingRoleRequests);
        Assert.Equal($"\"{user.RowVersion}\"", response.Headers.ETag?.Tag);

        await using var db = NewDb();
        var audit = await db.AuditEvents.AsNoTracking().SingleAsync(a => a.Action == "users.created" && a.ResourceId == user.Id.ToString());
        Assert.Equal(TestOnlyDataSeeder.TestUserId, audit.ActorUserId);
        Assert.DoesNotContain("someone@example.test", audit.ChangesJson, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ผู้ใช้ใหม่", audit.ChangesJson);
    }

    [Fact]
    public async Task CreateUser_DuplicateEmailIgnoringCaseAndWhitespace_Returns409()
    {
        var first = await Send(HttpMethod.Post, "/api/v1/admin/users", AdminToken, new CreateAdminUserRequest(
            "A", "dup@example.test", null, new[] { _viewerRoleId }));
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await Send(HttpMethod.Post, "/api/v1/admin/users", AdminToken, new CreateAdminUserRequest(
            "B", "  DUP@Example.TEST ", null, new[] { _viewerRoleId }));

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        Assert.Equal("USER_EMAIL_ALREADY_EXISTS", await ReadCode(second));
    }

    [Fact]
    public async Task CreateUser_WithInvalidInput_Returns400()
    {
        var noRoles = await Send(HttpMethod.Post, "/api/v1/admin/users", AdminToken, new CreateAdminUserRequest(
            "A", "a@example.test", null, Array.Empty<Guid>()));
        var badEmail = await Send(HttpMethod.Post, "/api/v1/admin/users", AdminToken, new CreateAdminUserRequest(
            "A", "not-an-email", null, new[] { _viewerRoleId }));

        Assert.Equal(HttpStatusCode.BadRequest, noRoles.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, badEmail.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WithRoleBeyondCallerPermissions_Returns403AndCreatesNothing()
    {
        var response = await Send(HttpMethod.Post, "/api/v1/admin/users", AdminToken, new CreateAdminUserRequest(
            "Escalation", "escalate@example.test", null, new[] { _beyondCallerRoleId }));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("ROLE_ESCALATION_DENIED", await ReadCode(response));
        await using var db = NewDb();
        Assert.False(await db.Users.AnyAsync(u => u.NormalizedEmail == "escalate@example.test"));
    }

    [Fact]
    public async Task CreateUser_WithApprovalRole_CreatesPendingRequestInsteadOfAssignment()
    {
        var response = await Send(HttpMethod.Post, "/api/v1/admin/users", AdminToken, new CreateAdminUserRequest(
            "Approver", "approver@example.test", null, new[] { _approverRoleId }));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var user = (await response.Content.ReadFromJsonAsync<AdminUserResponse>())!;
        Assert.Empty(user.Memberships[0].Roles);
        var pending = Assert.Single(user.Memberships[0].PendingRoleRequests);
        Assert.Equal(_approverRoleId, pending.Role.Id);
    }

    [Fact]
    public async Task CreateUser_FromOrgB_CannotUseOrgARoles()
    {
        var response = await Send(HttpMethod.Post, "/api/v1/admin/users", OrgBToken, new CreateAdminUserRequest(
            "Cross", "cross@example.test", null, new[] { _viewerRoleId }), membershipId: TestOnlyDataSeeder.TestMembershipBId);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ------------------------------------------------------------------ maker-checker

    [Fact]
    public async Task ApprovalRoleAssignment_RequiresIndependentChecker()
    {
        var created = await CreateUserAsync("maker-target@example.test", _viewerRoleId);
        var membershipId = created.Memberships[0].Id;

        var assign = await Send(HttpMethod.Post, $"/api/v1/admin/memberships/{membershipId}/roles", AdminToken,
            new AssignAdminRoleRequest(_approverRoleId));
        Assert.Equal(HttpStatusCode.Accepted, assign.StatusCode);
        var body = (await assign.Content.ReadFromJsonAsync<AdminAssignRoleResponse>())!;
        var request = body.PendingRequest!;

        var duplicate = await Send(HttpMethod.Post, $"/api/v1/admin/memberships/{membershipId}/roles", AdminToken,
            new AssignAdminRoleRequest(_approverRoleId));
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
        Assert.Equal("ROLE_ASSIGNMENT_REQUEST_PENDING", await ReadCode(duplicate));

        var selfApprove = await Send(HttpMethod.Post, $"/api/v1/admin/role-assignment-requests/{request.Id}/approve", AdminToken,
            ifMatch: request.RowVersion);
        Assert.Equal(HttpStatusCode.Forbidden, selfApprove.StatusCode);
        Assert.Equal("ROLE_ASSIGNMENT_INDEPENDENT_CHECKER_REQUIRED", await ReadCode(selfApprove));

        var approve = await Send(HttpMethod.Post, $"/api/v1/admin/role-assignment-requests/{request.Id}/approve", SecondAdminToken,
            ifMatch: request.RowVersion, membershipId: SecondAdminMembershipId);
        Assert.Equal(HttpStatusCode.OK, approve.StatusCode);
        Assert.Equal("approved", (await approve.Content.ReadFromJsonAsync<AdminRoleRequestResponse>())!.Status);

        var user = await GetUserAsync(created.Id, AdminToken);
        Assert.Contains(user.Memberships[0].Roles, role => role.Id == _approverRoleId);
        Assert.Empty(user.Memberships[0].PendingRoleRequests);

        var again = await Send(HttpMethod.Post, $"/api/v1/admin/role-assignment-requests/{request.Id}/approve", SecondAdminToken,
            ifMatch: request.RowVersion, membershipId: SecondAdminMembershipId);
        Assert.Equal(HttpStatusCode.Conflict, again.StatusCode);
    }

    [Fact]
    public async Task RoleRequest_CanBeCancelledOnlyByRequester()
    {
        var created = await CreateUserAsync("cancel-target@example.test", _viewerRoleId);
        var assign = await Send(HttpMethod.Post, $"/api/v1/admin/memberships/{created.Memberships[0].Id}/roles", AdminToken,
            new AssignAdminRoleRequest(_approverRoleId));
        var request = (await assign.Content.ReadFromJsonAsync<AdminAssignRoleResponse>())!.PendingRequest!;

        var byOther = await Send(HttpMethod.Post, $"/api/v1/admin/role-assignment-requests/{request.Id}/cancel", SecondAdminToken,
            ifMatch: request.RowVersion, membershipId: SecondAdminMembershipId);
        Assert.Equal(HttpStatusCode.Forbidden, byOther.StatusCode);

        var byRequester = await Send(HttpMethod.Post, $"/api/v1/admin/role-assignment-requests/{request.Id}/cancel", AdminToken,
            ifMatch: request.RowVersion);
        Assert.Equal(HttpStatusCode.OK, byRequester.StatusCode);
        Assert.Equal("cancelled", (await byRequester.Content.ReadFromJsonAsync<AdminRoleRequestResponse>())!.Status);
    }

    // ------------------------------------------------------------------ guards

    [Fact]
    public async Task RevokeRole_OnOwnMembership_Returns403()
    {
        var response = await Send(HttpMethod.Delete,
            $"/api/v1/admin/memberships/{TestOnlyDataSeeder.TestMembershipId}/roles/{await AdminRoleIdAsync()}", AdminToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("SELF_ROLE_CHANGE_FORBIDDEN", await ReadCode(response));
    }

    [Fact]
    public async Task LastAdministrator_CannotBeRemovedByDeactivation()
    {
        // Two administrators exist. The second one switches the first off (allowed), then cannot switch itself off.
        var secondUser = await GetUserAsync(SecondAdminUserId, SecondAdminToken, SecondAdminMembershipId);
        var firstMembership = (await GetUserAsync(TestOnlyDataSeeder.TestUserId, SecondAdminToken, SecondAdminMembershipId)).Memberships[0];

        var deactivateFirst = await Send(HttpMethod.Post, $"/api/v1/admin/memberships/{firstMembership.Id}/deactivate", SecondAdminToken,
            ifMatch: firstMembership.RowVersion, membershipId: SecondAdminMembershipId);
        Assert.Equal(HttpStatusCode.OK, deactivateFirst.StatusCode);

        var deactivateSelf = await Send(HttpMethod.Post, $"/api/v1/admin/memberships/{secondUser.Memberships[0].Id}/deactivate", SecondAdminToken,
            ifMatch: secondUser.Memberships[0].RowVersion, membershipId: SecondAdminMembershipId);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, deactivateSelf.StatusCode);
        Assert.Equal("LAST_ADMINISTRATOR_REQUIRED", await ReadCode(deactivateSelf));

        var deactivateUser = await Send(HttpMethod.Post, $"/api/v1/admin/users/{secondUser.Id}/deactivate", SecondAdminToken,
            ifMatch: secondUser.RowVersion, membershipId: SecondAdminMembershipId);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, deactivateUser.StatusCode);
        Assert.Equal("LAST_ADMINISTRATOR_REQUIRED", await ReadCode(deactivateUser));
    }

    [Fact]
    public async Task PendingAdministrator_DoesNotCountAsAnActiveAdministrator()
    {
        // The second admin is removed first, then an invited (pending) admin is added; the only real admin still cannot leave.
        var secondUser = await GetUserAsync(SecondAdminUserId, AdminToken);
        var secondMembership = secondUser.Memberships[0];
        var removeSecond = await Send(HttpMethod.Post, $"/api/v1/admin/memberships/{secondMembership.Id}/deactivate", AdminToken,
            ifMatch: secondMembership.RowVersion);
        Assert.Equal(HttpStatusCode.OK, removeSecond.StatusCode);

        var pendingAdmin = await CreateUserAsync("pending-admin@example.test", _secondAdminRoleId);
        Assert.Equal("pending", pendingAdmin.Status);

        var firstUser = await GetUserAsync(TestOnlyDataSeeder.TestUserId, AdminToken);
        var leave = await Send(HttpMethod.Post, $"/api/v1/admin/memberships/{firstUser.Memberships[0].Id}/deactivate", AdminToken,
            ifMatch: firstUser.Memberships[0].RowVersion);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, leave.StatusCode);
        Assert.Equal("LAST_ADMINISTRATOR_REQUIRED", await ReadCode(leave));
    }

    [Fact]
    public async Task UpdateWithStaleOrMissingIfMatch_IsRejected()
    {
        var created = await CreateUserAsync("concurrency@example.test", _viewerRoleId);

        var missing = await Send(HttpMethod.Patch, $"/api/v1/admin/users/{created.Id}", AdminToken, new RenameAdminUserRequest("Renamed"));
        Assert.Equal((HttpStatusCode)428, missing.StatusCode);

        var stale = await Send(HttpMethod.Patch, $"/api/v1/admin/users/{created.Id}", AdminToken, new RenameAdminUserRequest("Renamed"),
            ifMatch: Guid.NewGuid());
        Assert.Equal(HttpStatusCode.Conflict, stale.StatusCode);
        Assert.Equal("ADMIN_VERSION_CONFLICT", await ReadCode(stale));

        var ok = await Send(HttpMethod.Patch, $"/api/v1/admin/users/{created.Id}", AdminToken, new RenameAdminUserRequest("Renamed"),
            ifMatch: created.RowVersion);
        Assert.Equal(HttpStatusCode.OK, ok.StatusCode);
        Assert.Equal("Renamed", (await ok.Content.ReadFromJsonAsync<AdminUserResponse>())!.DisplayName);
    }

    // ------------------------------------------------------------------ scope and permissions

    [Fact]
    public async Task OtherOrganization_CannotReadOrChangeUsersOfThisOrganization()
    {
        var read = await Send(HttpMethod.Get, $"/api/v1/admin/users/{TestOnlyDataSeeder.TestUserId}", OrgBToken,
            membershipId: TestOnlyDataSeeder.TestMembershipBId);
        Assert.Equal(HttpStatusCode.NotFound, read.StatusCode);

        var change = await Send(HttpMethod.Post, $"/api/v1/admin/memberships/{TestOnlyDataSeeder.TestMembershipId}/deactivate", OrgBToken,
            ifMatch: Guid.NewGuid(), membershipId: TestOnlyDataSeeder.TestMembershipBId);
        Assert.Equal(HttpStatusCode.NotFound, change.StatusCode);

        var list = await Send(HttpMethod.Get, "/api/v1/admin/users", OrgBToken, membershipId: TestOnlyDataSeeder.TestMembershipBId);
        var items = (await list.Content.ReadFromJsonAsync<AdminUserListResponse>())!.Items;
        Assert.DoesNotContain(items, item => item.Id == TestOnlyDataSeeder.TestUserId);
    }

    [Fact]
    public async Task UserWithoutAdministrationPermissions_IsForbidden()
    {
        var list = await Send(HttpMethod.Get, "/api/v1/admin/users", ViewerToken, membershipId: ViewerMembershipId);
        var create = await Send(HttpMethod.Post, "/api/v1/admin/users", ViewerToken, new CreateAdminUserRequest(
            "X", "x@example.test", null, new[] { _viewerRoleId }), membershipId: ViewerMembershipId);
        var requests = await Send(HttpMethod.Get, "/api/v1/admin/role-assignment-requests", ViewerToken, membershipId: ViewerMembershipId);

        Assert.Equal(HttpStatusCode.Forbidden, list.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, create.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, requests.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await _client.GetAsync("/api/v1/admin/users")).StatusCode);
    }

    [Fact]
    public async Task ListUsers_SupportsSearchStatusAndPagination()
    {
        await CreateUserAsync("searchable-one@example.test", _viewerRoleId);
        await CreateUserAsync("searchable-two@example.test", _viewerRoleId);

        var response = await Send(HttpMethod.Get, "/api/v1/admin/users?search=searchable-&status=pending&pageSize=1&page=2", AdminToken);
        var page = (await response.Content.ReadFromJsonAsync<AdminUserListResponse>())!;

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, page.Pagination.TotalCount);
        Assert.Equal(2, page.Pagination.TotalPages);
        Assert.Single(page.Items);

        var badStatus = await Send(HttpMethod.Get, "/api/v1/admin/users?status=bogus", AdminToken);
        Assert.Equal(HttpStatusCode.BadRequest, badStatus.StatusCode);
    }

    [Fact]
    public async Task ListRoles_MarksAssignabilityAndApprovalRequirement()
    {
        var response = await Send(HttpMethod.Get, "/api/v1/admin/roles", AdminToken);
        var roles = (await response.Content.ReadFromJsonAsync<AdminRoleListResponse>())!.Items;

        Assert.True(roles.Single(r => r.Id == _viewerRoleId).Assignable);
        Assert.False(roles.Single(r => r.Id == _viewerRoleId).RequiresApproval);
        Assert.True(roles.Single(r => r.Id == _approverRoleId).RequiresApproval);
        Assert.False(roles.Single(r => r.Id == _beyondCallerRoleId).Assignable);
    }

    // ------------------------------------------------------------------ revocation takes effect immediately

    [Fact]
    public async Task RevokingRoleOrDeactivatingMembership_TakesEffectOnNextRequest()
    {
        var before = await Send(HttpMethod.Get, "/api/v1/users", ViewerToken, membershipId: ViewerMembershipId);
        Assert.Equal(HttpStatusCode.OK, before.StatusCode);

        var viewer = await GetUserAsync(ViewerUserId, AdminToken);
        var revoke = await Send(HttpMethod.Delete, $"/api/v1/admin/memberships/{ViewerMembershipId}/roles/{_viewerRoleId}", AdminToken);
        Assert.Equal(HttpStatusCode.NoContent, revoke.StatusCode);

        var afterRevoke = await Send(HttpMethod.Get, "/api/v1/users", ViewerToken, membershipId: ViewerMembershipId);
        Assert.Equal(HttpStatusCode.Forbidden, afterRevoke.StatusCode);

        var assign = await Send(HttpMethod.Post, $"/api/v1/admin/memberships/{ViewerMembershipId}/roles", AdminToken, new AssignAdminRoleRequest(_viewerRoleId));
        Assert.Equal(HttpStatusCode.Created, assign.StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await Send(HttpMethod.Get, "/api/v1/users", ViewerToken, membershipId: ViewerMembershipId)).StatusCode);

        var current = (await GetUserAsync(ViewerUserId, AdminToken)).Memberships[0];
        var deactivate = await Send(HttpMethod.Post, $"/api/v1/admin/memberships/{ViewerMembershipId}/deactivate", AdminToken, ifMatch: current.RowVersion);
        Assert.Equal(HttpStatusCode.OK, deactivate.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await Send(HttpMethod.Get, "/api/v1/users", ViewerToken, membershipId: ViewerMembershipId)).StatusCode);
        Assert.NotEqual(viewer.Memberships[0].RowVersion, current.RowVersion);
    }

    [Fact]
    public async Task DeactivatedUser_CannotUseTheirToken()
    {
        var viewer = await GetUserAsync(ViewerUserId, AdminToken);
        var deactivate = await Send(HttpMethod.Post, $"/api/v1/admin/users/{ViewerUserId}/deactivate", AdminToken, ifMatch: viewer.RowVersion);
        Assert.Equal(HttpStatusCode.OK, deactivate.StatusCode);

        Assert.Equal(HttpStatusCode.Forbidden, (await Send(HttpMethod.Get, "/api/v1/users", ViewerToken, membershipId: ViewerMembershipId)).StatusCode);
        var me = await Send(HttpMethod.Get, "/api/v1/me", ViewerToken);
        Assert.Equal(HttpStatusCode.Forbidden, me.StatusCode);
    }

    // ------------------------------------------------------------------ first-login linking

    [Fact]
    public async Task FirstVerifiedLogin_LinksPendingUserByEmailAndNeverRelinks()
    {
        var created = await CreateUserAsync(InviteeEmail, _viewerRoleId);
        Assert.Equal("pending", created.Status);

        var unverified = await Send(HttpMethod.Get, "/api/v1/me", UnverifiedInviteeToken);
        Assert.Equal(HttpStatusCode.Forbidden, unverified.StatusCode);
        Assert.Equal("pending", (await GetUserAsync(created.Id, AdminToken)).Status);

        var linked = await Send(HttpMethod.Get, "/api/v1/me", InviteeToken);
        Assert.Equal(HttpStatusCode.OK, linked.StatusCode);
        var me = (await linked.Content.ReadFromJsonAsync<CurrentUserResponse>())!;
        Assert.Equal(created.Id, me.User.Id);
        Assert.Single(me.Memberships);

        var afterLink = await GetUserAsync(created.Id, AdminToken);
        Assert.Equal("active", afterLink.Status);

        // A different Firebase identity claiming the same email cannot take over an already linked user.
        var takeover = await Send(HttpMethod.Get, "/api/v1/me", OtherIdentityToken);
        Assert.Equal(HttpStatusCode.Forbidden, takeover.StatusCode);

        await using var db = NewDb();
        Assert.Equal("invitee-uid", (await db.Users.AsNoTracking().SingleAsync(u => u.Id == created.Id)).FirebaseUid);
        Assert.True(await db.AuditEvents.AnyAsync(a => a.Action == "users.identity-linked" && a.ResourceId == created.Id.ToString()));
    }

    [Fact]
    public async Task Linking_IgnoresDeactivatedPendingUsers()
    {
        var created = await CreateUserAsync(InviteeEmail, _viewerRoleId);
        var deactivate = await Send(HttpMethod.Post, $"/api/v1/admin/users/{created.Id}/deactivate", AdminToken, ifMatch: created.RowVersion);
        Assert.Equal(HttpStatusCode.OK, deactivate.StatusCode);

        var login = await Send(HttpMethod.Get, "/api/v1/me", InviteeToken);

        Assert.Equal(HttpStatusCode.Forbidden, login.StatusCode);
        await using var db = NewDb();
        Assert.Null((await db.Users.AsNoTracking().SingleAsync(u => u.Id == created.Id)).FirebaseUid);
    }

    // ------------------------------------------------------------------ helpers

    private AppDbContext NewDb()
    {
        var scope = _factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    private async Task<Guid> AdminRoleIdAsync()
    {
        await using var db = NewDb();
        return await db.Roles.AsNoTracking()
            .Where(r => r.OrganizationId == TestOnlyDataSeeder.TestOrgId && r.Name == "Test Admin")
            .Select(r => r.Id)
            .SingleAsync();
    }

    private async Task<AdminUserResponse> CreateUserAsync(string email, Guid roleId)
    {
        var response = await Send(HttpMethod.Post, "/api/v1/admin/users", AdminToken, new CreateAdminUserRequest(
            "Test " + email, email, null, new[] { roleId }));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<AdminUserResponse>())!;
    }

    private async Task<AdminUserResponse> GetUserAsync(Guid userId, string token, Guid? membershipId = null)
    {
        var response = await Send(HttpMethod.Get, $"/api/v1/admin/users/{userId}", token, membershipId: membershipId);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<AdminUserResponse>())!;
    }

    private Task<HttpResponseMessage> Send(
        HttpMethod method, string url, string token, object? body = null, Guid? ifMatch = null, Guid? membershipId = null)
    {
        var request = new HttpRequestMessage(method, url);
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

    private async Task SeedAdministrationFixturesAsync(AppDbContext db)
    {
        var permissions = await db.Permissions.ToDictionaryAsync(p => p.Key);
        var orgId = TestOnlyDataSeeder.TestOrgId;

        // A permission the seeded administrator never receives, to prove anti-escalation.
        var beyond = Permission.Create("zz.beyond-caller", "Held by nobody in the test organization");
        db.Permissions.Add(beyond);
        permissions[beyond.Key] = beyond;

        Role MakeRole(string name, params string[] keys)
        {
            var role = new Role(Guid.NewGuid(), orgId, name, name, isActive: true);
            db.Roles.Add(role);
            foreach (var key in keys)
            {
                db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), role.Id, orgId, permissions[key].Id, PermissionScope.Organization, orgId));
            }

            return role;
        }

        var viewer = MakeRole("Test Viewer", "opportunities.read");
        var approver = MakeRole("Test Approver", "estimates.approve", "estimates.read");
        var secondAdmin = MakeRole("Test Second Admin", "users.read", "users.manage", "memberships.manage", "roles.assign", "roles.assign-approval", "estimates.approve", "estimates.read", "organizations.read");
        var beyondRole = MakeRole("Test Beyond", "zz.beyond-caller");
        _viewerRoleId = viewer.Id;
        _approverRoleId = approver.Id;
        _secondAdminRoleId = secondAdmin.Id;
        _beyondCallerRoleId = beyondRole.Id;

        db.Users.Add(new User(SecondAdminUserId, SecondAdminUid, "Second Admin", "second.admin@example.test"));
        db.Memberships.Add(new Membership(SecondAdminMembershipId, orgId, TestOnlyDataSeeder.TestBranchId, SecondAdminUserId));
        db.MembershipRoles.Add(new MembershipRole(SecondAdminMembershipId, secondAdmin.Id, orgId));

        db.Users.Add(new User(ViewerUserId, ViewerUid, "Viewer", "viewer@example.test"));
        db.Memberships.Add(new Membership(ViewerMembershipId, orgId, TestOnlyDataSeeder.TestBranchId, ViewerUserId));
        db.MembershipRoles.Add(new MembershipRole(ViewerMembershipId, viewer.Id, orgId));

        await db.SaveChangesAsync();
    }
}
