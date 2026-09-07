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
using TanErp.Api.Contracts.Crm.Customers;
using TanErp.Api.ErrorHandling;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class CustomerEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private static readonly Guid OrgAId = TestOnlyDataSeeder.TestOrgId;
    private static readonly Guid MembershipAId = TestOnlyDataSeeder.TestMembershipId;
    private const string UidA = TestOnlyDataSeeder.TestFirebaseUid;

    private static readonly Guid OrgBId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b20");
    private static readonly Guid MembershipBId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b21");
    private const string UidB = "foundation-user-org-b-test-only";

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
        {
            var uid = idToken switch
            {
                "token-org-a" => UidA,
                "token-org-b" => UidB,
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
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Database"] = _postgres.GetConnectionString()
                });
            });
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(_postgres.GetConnectionString()));

                var authDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFirebaseTokenVerifier));
                if (authDescriptor != null) services.Remove(authDescriptor);
                services.AddSingleton<IFirebaseTokenVerifier, TestFirebaseTokenVerifier>();
            });
        });

        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true);

        // Seed CRM permissions for Org A
        var readPerm = Permission.Create("customers.read", "Read Customers");
        var createPerm = Permission.Create("customers.create", "Create Customers");
        var manageContactPerm = Permission.Create("customer-contacts.manage", "Manage Customer Contacts");
        db.Permissions.AddRange(readPerm, createPerm, manageContactPerm);

        var adminRole = await db.Roles.FirstAsync(r => r.OrganizationId == OrgAId);
        db.RolePermissions.AddRange(
            new RolePermission(Guid.NewGuid(), adminRole.Id, OrgAId, readPerm.Id, PermissionScope.Organization, OrgAId),
            new RolePermission(Guid.NewGuid(), adminRole.Id, OrgAId, createPerm.Id, PermissionScope.Organization, OrgAId),
            new RolePermission(Guid.NewGuid(), adminRole.Id, OrgAId, manageContactPerm.Id, PermissionScope.Organization, OrgAId)
        );

        // Seed Org B with user, membership and separate roles
        var orgB = new Organization(OrgBId, "TEST_ONLY Organization B");
        db.Organizations.Add(orgB);

        var userB = new User(Guid.NewGuid(), UidB, "User Org B", "userb@example.test", isActive: true);
        db.Users.Add(userB);

        var membershipB = new Membership(MembershipBId, OrgBId, null, userB.Id, isActive: true);
        db.Memberships.Add(membershipB);

        var roleB = new Role(Guid.NewGuid(), OrgBId, "Org B Admin", "Admin for Org B", isActive: true);
        db.Roles.Add(roleB);

        db.RolePermissions.AddRange(
            new RolePermission(Guid.NewGuid(), roleB.Id, OrgBId, readPerm.Id, PermissionScope.Organization, OrgBId),
            new RolePermission(Guid.NewGuid(), roleB.Id, OrgBId, createPerm.Id, PermissionScope.Organization, OrgBId),
            new RolePermission(Guid.NewGuid(), roleB.Id, OrgBId, manageContactPerm.Id, PermissionScope.Organization, OrgBId)
        );

        db.MembershipRoles.Add(new MembershipRole(MembershipBId, roleB.Id, OrgBId));

        await db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task CreateCustomer_WithoutAuth_Returns401()
    {
        var request = new CreateCustomerRequest("organization", "บริษัท ก", null, "th",
            new CreatePrimaryContactRequest("นาย ก", null, "0812345678", null, "phone"));

        var response = await _client.PostAsJsonAsync("/api/v1/customers", request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateCustomer_WithoutMembershipHeader_Returns400()
    {
        var request = new CreateCustomerRequest("organization", "บริษัท ก", null, "th",
            new CreatePrimaryContactRequest("นาย ก", null, "0812345678", null, "phone"));

        var msg = new HttpRequestMessage(HttpMethod.Post, "/api/v1/customers");
        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-org-a");
        msg.Headers.Add("Idempotency-Key", "valid-idempotency-key-001");
        msg.Content = JsonContent.Create(request);

        var response = await _client.SendAsync(msg);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal("MEMBERSHIP_CONTEXT_REQUIRED", problem.Code);
    }

    [Fact]
    public async Task CreateCustomer_WithoutIdempotencyKey_Returns400()
    {
        var request = new CreateCustomerRequest("organization", "บริษัท ก", null, "th",
            new CreatePrimaryContactRequest("นาย ก", null, "0812345678", null, "phone"));

        var msg = new HttpRequestMessage(HttpMethod.Post, "/api/v1/customers");
        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-org-a");
        msg.Headers.Add("X-Membership-Id", MembershipAId.ToString());
        msg.Content = JsonContent.Create(request);

        var response = await _client.SendAsync(msg);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal("IDEMPOTENCY_KEY_REQUIRED", problem.Code);
    }

    [Fact]
    public async Task CreateCustomer_WhenValid_Returns201WithLocationAndETag()
    {
        var request = new CreateCustomerRequest("organization", "บริษัท ก จำกัด TEST_ONLY", "Company A TEST_ONLY", "th",
            new CreatePrimaryContactRequest("คุณ สมชาย", "กรรมการ", "+66812345678", "somchai@example.test", "phone"));

        var key = "valid-create-key-10001";
        var msg = new HttpRequestMessage(HttpMethod.Post, "/api/v1/customers");
        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-org-a");
        msg.Headers.Add("X-Membership-Id", MembershipAId.ToString());
        msg.Headers.Add("Idempotency-Key", key);
        msg.Content = JsonContent.Create(request);

        var response = await _client.SendAsync(msg);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        Assert.NotNull(response.Headers.Location);
        Assert.NotNull(response.Headers.ETag);

        var customer = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        Assert.NotNull(customer);
        Assert.Equal("draft", customer.Status);
        Assert.Equal("บริษัท ก จำกัด TEST_ONLY", customer.DisplayNameTh);
        Assert.StartsWith("CUS-", customer.Code);
        Assert.NotNull(customer.PrimaryContact);
        Assert.False(customer.PrimaryContact.IsMasked);
        Assert.Equal("+66812345678", customer.PrimaryContact.Phone);

        // Retry same key and payload returns same customer
        var retryMsg = new HttpRequestMessage(HttpMethod.Post, "/api/v1/customers");
        retryMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-org-a");
        retryMsg.Headers.Add("X-Membership-Id", MembershipAId.ToString());
        retryMsg.Headers.Add("Idempotency-Key", key);
        retryMsg.Content = JsonContent.Create(request);

        var retryResponse = await _client.SendAsync(retryMsg);
        Assert.Equal(HttpStatusCode.Created, retryResponse.StatusCode);

        var replayedCustomer = await retryResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        Assert.NotNull(replayedCustomer);
        Assert.Equal(customer.Id, replayedCustomer.Id);

        // Same key with different payload returns 409
        var diffRequest = new CreateCustomerRequest("person", "นาย ข TEST_ONLY", null, "th",
            new CreatePrimaryContactRequest("นาย ข", null, "+66899999999", null, "phone"));

        var conflictMsg = new HttpRequestMessage(HttpMethod.Post, "/api/v1/customers");
        conflictMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-org-a");
        conflictMsg.Headers.Add("X-Membership-Id", MembershipAId.ToString());
        conflictMsg.Headers.Add("Idempotency-Key", key);
        conflictMsg.Content = JsonContent.Create(diffRequest);

        var conflictResponse = await _client.SendAsync(conflictMsg);
        Assert.Equal(HttpStatusCode.Conflict, conflictResponse.StatusCode);
        var conflictProblem = await conflictResponse.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.NotNull(conflictProblem);
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", conflictProblem.Code);
    }

    [Fact]
    public async Task CustomerTenantIsolation_OrgBCannotAccessOrgACustomer()
    {
        // 1. Org A creates customer
        var request = new CreateCustomerRequest("organization", "บริษัท ลับเฉพาะ ก", null, "th",
            new CreatePrimaryContactRequest("คุณลับ", null, "+66811112222", null, "phone"));

        var createMsg = new HttpRequestMessage(HttpMethod.Post, "/api/v1/customers");
        createMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-org-a");
        createMsg.Headers.Add("X-Membership-Id", MembershipAId.ToString());
        createMsg.Headers.Add("Idempotency-Key", "key-isolation-test-01");
        createMsg.Content = JsonContent.Create(request);

        var createResponse = await _client.SendAsync(createMsg);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        Assert.NotNull(created);

        // 2. Org B tries to GET Org A's customer
        var getMsg = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/customers/{created.Id}");
        getMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-org-b");
        getMsg.Headers.Add("X-Membership-Id", MembershipBId.ToString());

        var getResponse = await _client.SendAsync(getMsg);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        var problem = await getResponse.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal("RESOURCE_NOT_FOUND", problem.Code);

        // 3. Org B lists customers: Org A's customer must not appear
        var listMsg = new HttpRequestMessage(HttpMethod.Get, "/api/v1/customers");
        listMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-org-b");
        listMsg.Headers.Add("X-Membership-Id", MembershipBId.ToString());

        var listResponse = await _client.SendAsync(listMsg);
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var list = await listResponse.Content.ReadFromJsonAsync<CustomerListResponse>();
        Assert.NotNull(list);
        Assert.DoesNotContain(list.Items, i => i.Id == created.Id);
    }
}
