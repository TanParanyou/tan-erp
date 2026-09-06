using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.IdentityAccess;
using TanErp.Api.ErrorHandling;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class CurrentUserEndpointTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
        {
            var uid = idToken switch
            {
                "token-valid-active" => "uid-active",
                "token-disabled" => "uid-disabled",
                "token-no-membership" => "uid-no-membership",
                _ => null
            };

            return Task.FromResult(uid);
        }
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureServices(services =>
            {
                // Replace DbContext with Testcontainers connection string
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(_postgres.GetConnectionString()));

                // Replace IFirebaseTokenVerifier with test verifier
                var tokenDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFirebaseTokenVerifier));
                if (tokenDescriptor != null) services.Remove(tokenDescriptor);

                services.AddSingleton<IFirebaseTokenVerifier, TestFirebaseTokenVerifier>();
            });
        });

        _client = _factory.CreateClient();

        // Apply migrations and seed data
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        var userId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4a10");
        var membershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4a11");
        var orgId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4a12");
        var branchId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4a13");

        var org = new Organization(orgId, "TEST_ONLY Project ERP");
        var branch = new Branch(branchId, orgId, "B01", "สาขาทดสอบ");
        var userActive = new User(userId, "uid-active", "ผู้ใช้ TEST_ONLY", "foundation-user@example.test");
        var userDisabled = new User(Guid.NewGuid(), "uid-disabled", "ผู้ใช้ถูกระงับ", "disabled@example.test", isActive: false);
        var userNoMembership = new User(Guid.NewGuid(), "uid-no-membership", "ผู้ใช้ไม่มีสมาชิก", "nomember@example.test");

        var membership = new Membership(membershipId, orgId, branchId, userId);
        var role = new Role(Guid.NewGuid(), orgId, "Admin");
        var permission = Permission.Create("organizations.read");

        var rolePermission = new RolePermission(Guid.NewGuid(), role.Id, permission.Id, PermissionScope.Organization, orgId);
        var membershipRole = new MembershipRole(membershipId, role.Id, orgId);

        db.Organizations.Add(org);
        db.Branches.Add(branch);
        db.Users.AddRange(userActive, userDisabled, userNoMembership);
        db.Memberships.Add(membership);
        db.Roles.Add(role);
        db.Permissions.Add(permission);
        db.RolePermissions.Add(rolePermission);
        db.MembershipRoles.Add(membershipRole);

        await db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task GetCurrentUser_NoAuthorizationHeader_Returns401AuthenticationRequired()
    {
        var response = await _client.GetAsync("/api/v1/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var body = await response.Content.ReadAsStringAsync();
        var problem = JsonSerializer.Deserialize<ApiProblemDetails>(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.NotNull(problem);
        Assert.Equal("AUTHENTICATION_REQUIRED", problem.Code);
        Assert.False(string.IsNullOrWhiteSpace(problem.TraceId));
        Assert.DoesNotContain("Exception", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("SELECT", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetCurrentUser_MalformedOrExpiredToken_Returns401AuthenticationInvalid()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "malformed-expired-token");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        var problem = JsonSerializer.Deserialize<ApiProblemDetails>(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.NotNull(problem);
        Assert.Equal("AUTHENTICATION_INVALID", problem.Code);
        Assert.False(string.IsNullOrWhiteSpace(problem.TraceId));
    }

    [Fact]
    public async Task GetCurrentUser_ValidToken_DisabledUser_Returns403UserAccessDisabled()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-disabled");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        var problem = JsonSerializer.Deserialize<ApiProblemDetails>(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.NotNull(problem);
        Assert.Equal("USER_ACCESS_DISABLED", problem.Code);
        Assert.False(string.IsNullOrWhiteSpace(problem.TraceId));
    }

    [Fact]
    public async Task GetCurrentUser_ValidToken_NoMembership_Returns403ActiveMembershipRequired()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-no-membership");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        var problem = JsonSerializer.Deserialize<ApiProblemDetails>(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.NotNull(problem);
        Assert.Equal("ACTIVE_MEMBERSHIP_REQUIRED", problem.Code);
        Assert.False(string.IsNullOrWhiteSpace(problem.TraceId));
    }

    [Fact]
    public async Task GetCurrentUser_ValidToken_ActiveMembership_Returns200DocumentedResponse()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-valid-active");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CurrentUserResponse>(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.NotNull(result);
        Assert.Equal(Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4a10"), result.User.Id);
        Assert.Equal("ผู้ใช้ TEST_ONLY", result.User.DisplayName);
        Assert.Equal("foundation-user@example.test", result.User.Email);

        Assert.Single(result.Memberships);
        var member = result.Memberships[0];
        Assert.Equal(Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4a11"), member.Id);
        Assert.Equal("TEST_ONLY Project ERP", member.Organization.Name);
        Assert.Equal("สาขาทดสอบ", member.Branch?.Name);

        Assert.Single(member.Permissions);
        var perm = member.Permissions[0];
        Assert.Equal("organizations.read", perm.Key);
        Assert.Equal("organization", perm.Scope);
        Assert.Equal(Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4a12"), perm.ScopeId);

        // Assert security rules: no Firebase UID or Role names leaked
        Assert.DoesNotContain("uid-active", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Admin", body);
    }

    [Theory]
    [InlineData("th", "จำเป็นต้องมีสมาชิกภาพ")]
    [InlineData("en", "Active Membership")]
    [InlineData("fr", "จำเป็นต้องมีสมาชิกภาพ")] // French falls back to default Thai
    public async Task GetCurrentUser_Localization_HonorsAcceptLanguageHeader(string locale, string expectedTitleSubstring)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-no-membership");
        request.Headers.Add("Accept-Language", locale);

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var problem = JsonSerializer.Deserialize<ApiProblemDetails>(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.NotNull(problem);
        Assert.Equal("ACTIVE_MEMBERSHIP_REQUIRED", problem.Code);
        Assert.Contains(expectedTitleSubstring, problem.Title);
    }
}
