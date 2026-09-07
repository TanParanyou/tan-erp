using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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

    private static readonly Guid OrgBId = TestOnlyDataSeeder.TestOrgBId;
    private static readonly Guid MembershipBId = TestOnlyDataSeeder.TestMembershipBId;
    private const string UidB = TestOnlyDataSeeder.TestFirebaseUidB;

    private const string UidReadOnly = "uid-read-only-test";
    private static readonly Guid MembershipReadOnlyId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b31");

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
        {
            var uid = idToken switch
            {
                "token-org-a" => UidA,
                "token-org-b" => UidB,
                "token-read-only" => UidReadOnly,
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

        // Seed read-only user (has customers.read, but lacks customer-contacts.manage)
        var userReadOnly = new User(Guid.NewGuid(), UidReadOnly, "Read Only User", "readonly@example.test", isActive: true);
        db.Users.Add(userReadOnly);

        var membershipReadOnly = new Membership(MembershipReadOnlyId, OrgAId, null, userReadOnly.Id, isActive: true);
        db.Memberships.Add(membershipReadOnly);

        var roleReadOnly = new Role(Guid.NewGuid(), OrgAId, "Read Only Role", "Read only role", isActive: true);
        db.Roles.Add(roleReadOnly);

        var readPerm = await db.Permissions.FirstAsync(p => p.Key == "customers.read");
        db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), roleReadOnly.Id, OrgAId, readPerm.Id, PermissionScope.Organization, OrgAId));
        db.MembershipRoles.Add(new MembershipRole(MembershipReadOnlyId, roleReadOnly.Id, OrgAId));

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

        // 2. Org B tries to GET Org A's customer -> 404
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

    [Fact]
    public async Task DuplicateSearch_NeverReportsCandidateFromAnotherOrg()
    {
        // 1. Org A creates customer with distinct name and phone
        var requestA = new CreateCustomerRequest("organization", "บริษัท ช่างทอง จำกัด", null, "th",
            new CreatePrimaryContactRequest("นายช่าง", null, "+66819998877", null, "phone"));

        var msgA = new HttpRequestMessage(HttpMethod.Post, "/api/v1/customers");
        msgA.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-org-a");
        msgA.Headers.Add("X-Membership-Id", MembershipAId.ToString());
        msgA.Headers.Add("Idempotency-Key", "key-duplicate-org-a-1");
        msgA.Content = JsonContent.Create(requestA);

        var responseA = await _client.SendAsync(msgA);
        Assert.Equal(HttpStatusCode.Created, responseA.StatusCode);

        // 2. Org B creates customer with the EXACT SAME name and phone
        var requestB = new CreateCustomerRequest("organization", "บริษัท ช่างทอง จำกัด", null, "th",
            new CreatePrimaryContactRequest("นายช่าง", null, "+66819998877", null, "phone"));

        var msgB = new HttpRequestMessage(HttpMethod.Post, "/api/v1/customers");
        msgB.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-org-b");
        msgB.Headers.Add("X-Membership-Id", MembershipBId.ToString());
        msgB.Headers.Add("Idempotency-Key", "key-duplicate-org-b-1");
        msgB.Content = JsonContent.Create(requestB);

        var responseB = await _client.SendAsync(msgB);
        Assert.Equal(HttpStatusCode.Created, responseB.StatusCode);

        var resultB = await responseB.Content.ReadFromJsonAsync<CustomerResponse>();
        Assert.NotNull(resultB);
        // Org B must NOT see Org A's customer in duplicate candidates
        Assert.True(resultB.DuplicateCandidates == null || resultB.DuplicateCandidates.Count == 0);
    }

    [Fact]
    public async Task ListAndGet_WhenUserLacksManageContactPermission_MasksContactPii()
    {
        // 1. Org A creates customer with full phone & email
        var request = new CreateCustomerRequest("organization", "บริษัท โทรศัพท์เต็ม จำกัด", null, "th",
            new CreatePrimaryContactRequest("คุณประเสริฐ", "ผู้จัดการ", "+66812345678", "prasert@example.test", "phone"));

        var createMsg = new HttpRequestMessage(HttpMethod.Post, "/api/v1/customers");
        createMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-org-a");
        createMsg.Headers.Add("X-Membership-Id", MembershipAId.ToString());
        createMsg.Headers.Add("Idempotency-Key", "key-masking-test-01");
        createMsg.Content = JsonContent.Create(request);

        var createResponse = await _client.SendAsync(createMsg);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        Assert.NotNull(created);

        // 2. Read-only user (without customer-contacts.manage) lists customers
        var listMsg = new HttpRequestMessage(HttpMethod.Get, "/api/v1/customers");
        listMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-read-only");
        listMsg.Headers.Add("X-Membership-Id", MembershipReadOnlyId.ToString());

        var listResponse = await _client.SendAsync(listMsg);
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var list = await listResponse.Content.ReadFromJsonAsync<CustomerListResponse>();
        Assert.NotNull(list);
        var item = list.Items.First(i => i.Id == created.Id);
        Assert.True(item.PrimaryContact.IsMasked);
        Assert.DoesNotContain("2345", item.PrimaryContact.Phone ?? "");
        Assert.Contains("******", item.PrimaryContact.Phone ?? "");

        // 3. Read-only user GETs detail
        var getMsg = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/customers/{created.Id}");
        getMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-read-only");
        getMsg.Headers.Add("X-Membership-Id", MembershipReadOnlyId.ToString());

        var getResponse = await _client.SendAsync(getMsg);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var detail = await getResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        Assert.NotNull(detail);
        Assert.True(detail.PrimaryContact.IsMasked);
        Assert.Contains("******", detail.PrimaryContact.Phone ?? "");
        Assert.Contains("***@", detail.PrimaryContact.Email ?? "");
    }
}
