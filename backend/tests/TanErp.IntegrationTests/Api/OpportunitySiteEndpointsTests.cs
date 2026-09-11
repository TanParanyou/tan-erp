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
using TanErp.Api.Contracts.Crm.Opportunities;
using TanErp.Api.Contracts.Crm.Sites;
using TanErp.Api.ErrorHandling;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using TanErp.IntegrationTests.Support;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class OpportunitySiteEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private static readonly Guid OrgAId = TestOnlyDataSeeder.TestOrgId;
    private static readonly Guid BranchAId = TestOnlyDataSeeder.TestBranchId;
    private static readonly Guid MembershipAId = TestOnlyDataSeeder.TestMembershipId;
    private const string UidA = TestOnlyDataSeeder.TestFirebaseUid;

    private static readonly Guid OrgBId = TestOnlyDataSeeder.TestOrgBId;
    private static readonly Guid MembershipBId = TestOnlyDataSeeder.TestMembershipBId;
    private const string UidB = TestOnlyDataSeeder.TestFirebaseUidB;

    private const string UidNoBranch = "uid-no-branch-test";
    private static readonly Guid MembershipNoBranchId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4c01");

    private const string UidNoPerm = "uid-no-perm-test";
    private static readonly Guid MembershipNoPermId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4c02");

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
        {
            var uid = idToken switch
            {
                "token-org-a" => UidA,
                "token-org-b" => UidB,
                "token-no-branch" => UidNoBranch,
                "token-no-perm" => UidNoPerm,
                _ => null
            };

            return Task.FromResult(uid);
        }
    }

    private readonly InMemoryLoggerProvider _loggerProvider = new();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureLogging(logging => logging.AddProvider(_loggerProvider));
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

        // Seed user with no branch
        var userNoBranch = new User(Guid.NewGuid(), UidNoBranch, "No Branch User", "nobranch@example.test", true);
        db.Users.Add(userNoBranch);
        var membershipNoBranch = new Membership(MembershipNoBranchId, OrgAId, null, userNoBranch.Id, true);
        db.Memberships.Add(membershipNoBranch);

        // Give no-branch user all CRM permissions in Org A
        var roleAll = await db.Roles.FirstAsync(r => r.OrganizationId == OrgAId && r.Name == "Test Admin");
        db.MembershipRoles.Add(new MembershipRole(MembershipNoBranchId, roleAll.Id, OrgAId));

        // Seed user with no CRM permissions
        var userNoPerm = new User(Guid.NewGuid(), UidNoPerm, "No Perm User", "noperm@example.test", true);
        db.Users.Add(userNoPerm);
        var membershipNoPerm = new Membership(MembershipNoPermId, OrgAId, BranchAId, userNoPerm.Id, true);
        db.Memberships.Add(membershipNoPerm);

        await db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private HttpRequestMessage CreateRequest(
        HttpMethod method,
        string url,
        string token,
        Guid membershipId,
        string? idempotencyKey = null)
    {
        var msg = new HttpRequestMessage(method, url);
        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        msg.Headers.Add("X-Membership-Id", membershipId.ToString());
        if (idempotencyKey != null)
        {
            msg.Headers.Add("Idempotency-Key", idempotencyKey);
        }
        return msg;
    }

    private async Task<Customer> SeedActiveCustomerAsync(Guid orgId)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTimeOffset.UtcNow;
        var cust = Customer.CreateDraft(Guid.NewGuid(), orgId, TestOnlyDataSeeder.TestUserId, "organization", "ลูกค้าทดสอบ", null, "th",
            new PrimaryContactInput("นาย ช่าง", null, "0812345678", null, "phone"), now);
        cust.Activate(cust.RowVersion);
        db.Customers.Add(cust);
        await db.SaveChangesAsync();
        return cust;
    }

    // ==========================================
    // SITES ENDPOINTS TESTS
    // ==========================================

    [Fact]
    public async Task CreateSite_MissingPermissions_Returns403()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var body = new CreateSiteRequest("บ้าน", "123", "ต", "อ", "จ", "10000", "TH");
        var msg = CreateRequest(HttpMethod.Post, $"/api/v1/customers/{customer.Id}/sites", "token-no-perm", MembershipNoPermId, "site-key-no-perm-1");
        msg.Content = JsonContent.Create(body);

        var res = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        var problem = await res.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("PERMISSION_DENIED", problem?.Code);
    }

    [Fact]
    public async Task CreateSite_MissingFields_Returns422()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var body = new CreateSiteRequest("", "123", "ต", "อ", "จ", "10000", "TH");
        var msg = CreateRequest(HttpMethod.Post, $"/api/v1/customers/{customer.Id}/sites", "token-org-a", MembershipAId, "site-key-invalid-1");
        msg.Content = JsonContent.Create(body);

        var res = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, res.StatusCode);
        var problem = await res.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("SITE_FIELD_REQUIRED", problem?.Code);
    }

    [Fact]
    public async Task CreateSite_InactiveCustomer_Returns409()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTimeOffset.UtcNow;
        var draftCust = Customer.CreateDraft(Guid.NewGuid(), OrgAId, TestOnlyDataSeeder.TestUserId, "organization", "ลูกค้าดราฟต์", null, "th",
            new PrimaryContactInput("นาย สอง", null, "0812345678", null, "phone"), now);
        db.Customers.Add(draftCust);
        await db.SaveChangesAsync();

        var body = new CreateSiteRequest("บ้าน", "123", "ต", "อ", "จ", "10000", "TH");
        var msg = CreateRequest(HttpMethod.Post, $"/api/v1/customers/{draftCust.Id}/sites", "token-org-a", MembershipAId, "site-key-draft-cust-1");
        msg.Content = JsonContent.Create(body);

        var res = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.Conflict, res.StatusCode);
        var problem = await res.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("CUSTOMER_INVALID_STATE", problem?.Code);
    }

    [Fact]
    public async Task CreateSite_Success_Returns201WithLocationAndETag()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var body = new CreateSiteRequest("บ้านพัก", "123 หมู่ 1", "บางตลาด", "ปากเกร็ด", "นนทบุรี", "11120", "TH", 13.7563m, 100.5018m, "มีที่จอดรถ");
        var msg = CreateRequest(HttpMethod.Post, $"/api/v1/customers/{customer.Id}/sites", "token-org-a", MembershipAId, "site-key-success-1");
        msg.Content = JsonContent.Create(body);

        var res = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        Assert.NotNull(res.Headers.ETag);

        var site = await res.Content.ReadFromJsonAsync<SiteResponse>();
        Assert.NotNull(site);
        Assert.Equal("บ้านพัก", site.Label);
        Assert.Equal(customer.Id, site.CustomerId);
    }

    [Fact]
    public async Task CreateSite_ExactReplay_Returns201WithSameId()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var body = new CreateSiteRequest("บ้านพัก", "123", "ต", "อ", "จ", "10000", "TH");

        var msg1 = CreateRequest(HttpMethod.Post, $"/api/v1/customers/{customer.Id}/sites", "token-org-a", MembershipAId, "site-key-replay-1");
        msg1.Content = JsonContent.Create(body);
        var res1 = await _client.SendAsync(msg1);
        var site1 = await res1.Content.ReadFromJsonAsync<SiteResponse>();

        var msg2 = CreateRequest(HttpMethod.Post, $"/api/v1/customers/{customer.Id}/sites", "token-org-a", MembershipAId, "site-key-replay-1");
        msg2.Content = JsonContent.Create(body);
        var res2 = await _client.SendAsync(msg2);
        var site2 = await res2.Content.ReadFromJsonAsync<SiteResponse>();

        Assert.Equal(site1?.Id, site2?.Id);
    }

    [Fact]
    public async Task CreateSite_ReusedKeyDifferentPayload_Returns409()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var body1 = new CreateSiteRequest("บ้าน 1", "123", "ต", "อ", "จ", "10000", "TH");
        var msg1 = CreateRequest(HttpMethod.Post, $"/api/v1/customers/{customer.Id}/sites", "token-org-a", MembershipAId, "site-key-reused-1");
        msg1.Content = JsonContent.Create(body1);
        await _client.SendAsync(msg1);

        var body2 = new CreateSiteRequest("บ้าน 2", "456", "ต", "อ", "จ", "10000", "TH");
        var msg2 = CreateRequest(HttpMethod.Post, $"/api/v1/customers/{customer.Id}/sites", "token-org-a", MembershipAId, "site-key-reused-1");
        msg2.Content = JsonContent.Create(body2);
        var res2 = await _client.SendAsync(msg2);

        Assert.Equal(HttpStatusCode.Conflict, res2.StatusCode);
        var problem = await res2.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", problem?.Code);
    }

    [Fact]
    public async Task ListSites_CrossTenant_Returns404()
    {
        // Customer belongs to Org A, query sent by Org B
        var customerInOrgA = await SeedActiveCustomerAsync(OrgAId);
        var msg = CreateRequest(HttpMethod.Get, $"/api/v1/customers/{customerInOrgA.Id}/sites", "token-org-b", MembershipBId);

        var res = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    // ==========================================
    // OPPORTUNITIES ENDPOINTS TESTS
    // ==========================================

    [Fact]
    public async Task CreateOpportunity_MissingPermissions_Returns403()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var body = new CreateOpportunityRequest(customer.Id, null, "งาน 1", null, new[] { "built-in" }, null, null, null, null, null, null);
        var msg = CreateRequest(HttpMethod.Post, "/api/v1/opportunities", "token-no-perm", MembershipNoPermId, "opp-key-no-perm-1");
        msg.Content = JsonContent.Create(body);

        var res = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        var problem = await res.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("PERMISSION_DENIED", problem?.Code);
    }

    [Fact]
    public async Task CreateOpportunity_NoActiveBranch_Returns422()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var body = new CreateOpportunityRequest(customer.Id, null, "งาน 1", null, new[] { "built-in" }, null, null, null, null, null, null);
        var msg = CreateRequest(HttpMethod.Post, "/api/v1/opportunities", "token-no-branch", MembershipNoBranchId, "opp-key-no-branch-1");
        msg.Content = JsonContent.Create(body);

        var res = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, res.StatusCode);
        var problem = await res.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("ACTIVE_BRANCH_REQUIRED", problem?.Code);
    }

    [Fact]
    public async Task CreateOpportunity_SiteBelongsToAnotherCustomer_Returns404()
    {
        var c1 = await SeedActiveCustomerAsync(OrgAId);
        var c2 = await SeedActiveCustomerAsync(OrgAId);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var siteForC2 = Site.CreateActive(Guid.NewGuid(), OrgAId, c2.Id, TestOnlyDataSeeder.TestUserId, "ไซต์ ข",
            new SiteAddressInput("123", "ต", "อ", "จ", "10000", "TH"), null, null, null, DateTimeOffset.UtcNow);
        db.Sites.Add(siteForC2);
        await db.SaveChangesAsync();

        var body = new CreateOpportunityRequest(c1.Id, siteForC2.Id, "งานข้ามไซต์", null, new[] { "built-in" }, null, null, null, null, null, null);
        var msg = CreateRequest(HttpMethod.Post, "/api/v1/opportunities", "token-org-a", MembershipAId, "opp-key-cross-site-1");
        msg.Content = JsonContent.Create(body);

        var res = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task CreateOpportunity_Success_Returns201WithLocationAndETag()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var body = new CreateOpportunityRequest(
            customer.Id, null, "Built-in ห้องนอนใหญ่", "สำรวจตู้เสื้อผ้า",
            new[] { "built-in" }, null, 250000m, "THB", new DateOnly(2026, 10, 15),
            DateTimeOffset.UtcNow.AddDays(1), "นัดยืนยันเวลา");

        var msg = CreateRequest(HttpMethod.Post, "/api/v1/opportunities", "token-org-a", MembershipAId, "opp-key-success-1");
        msg.Content = JsonContent.Create(body);

        var res = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        Assert.NotNull(res.Headers.ETag);
        Assert.NotNull(res.Headers.Location);

        var opp = await res.Content.ReadFromJsonAsync<OpportunityResponse>();
        Assert.NotNull(opp);
        Assert.Equal("Built-in ห้องนอนใหญ่", opp.Title);
        Assert.Equal(OpportunityStage.Draft, opp.Stage);
        Assert.Equal(BranchAId, opp.BranchId);
        Assert.Equal(TestOnlyDataSeeder.TestUserId, opp.OwnerUserId);
    }

    [Fact]
    public async Task ListAndGetOpportunity_CrossTenant_Returns404()
    {
        // Create opportunity in Org A
        var customerInOrgA = await SeedActiveCustomerAsync(OrgAId);
        var body = new CreateOpportunityRequest(customerInOrgA.Id, null, "งาน Org A", null, new[] { "built-in" }, null, null, null, null, null, null);
        var msg = CreateRequest(HttpMethod.Post, "/api/v1/opportunities", "token-org-a", MembershipAId, "opp-key-cross-tenant-1");
        msg.Content = JsonContent.Create(body);
        var createRes = await _client.SendAsync(msg);
        var opp = await createRes.Content.ReadFromJsonAsync<OpportunityResponse>();
        Assert.NotNull(opp);

        // Org B attempts to GET opportunity created in Org A -> 404
        var getMsg = CreateRequest(HttpMethod.Get, $"/api/v1/opportunities/{opp.Id}", "token-org-b", MembershipBId);
        var getRes = await _client.SendAsync(getMsg);
        Assert.Equal(HttpStatusCode.NotFound, getRes.StatusCode);

        // Org B lists opportunities with customerId of Org A -> empty items (isolated)
        var listMsg = CreateRequest(HttpMethod.Get, $"/api/v1/opportunities?customerId={customerInOrgA.Id}", "token-org-b", MembershipBId);
        var listRes = await _client.SendAsync(listMsg);
        Assert.Equal(HttpStatusCode.OK, listRes.StatusCode);
        var listData = await listRes.Content.ReadFromJsonAsync<OpportunityListResponse>();
        Assert.NotNull(listData);
        Assert.Empty(listData.Items);
    }

    [Fact]
    public async Task CreateSite_PrivacyAuditAndLogs_DoesNotLeakAddressOrNotes()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var addressSentinel = $"SENTINEL_ADDR_{Guid.NewGuid():N}";
        var accessNoteSentinel = $"SENTINEL_NOTE_{Guid.NewGuid():N}";

        var body = new CreateSiteRequest(
            "ไซต์ทดสอบความเป็นส่วนตัว",
            addressSentinel,
            "บางโฉลง",
            "บางพลี",
            "สมุทรปราการ",
            "10540",
            "TH",
            null,
            null,
            accessNoteSentinel);

        var msg = CreateRequest(HttpMethod.Post, $"/api/v1/customers/{customer.Id}/sites", "token-org-a", MembershipAId, $"site-key-privacy-{Guid.NewGuid():N}");
        msg.Content = JsonContent.Create(body);

        var res = await _client.SendAsync(msg);
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);

        var site = await res.Content.ReadFromJsonAsync<SiteResponse>();
        Assert.NotNull(site);

        // Verify AuditEvent does not leak plain sentinel text in ChangesJson, but contains property names
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var audit = await db.AuditEvents.FirstOrDefaultAsync(a =>
            a.OrganizationId == OrgAId &&
            a.Action == "site.created" &&
            a.ResourceId == site.Id.ToString());

        Assert.NotNull(audit);
        Assert.NotNull(audit.ChangesJson);
        Assert.DoesNotContain(addressSentinel, audit.ChangesJson, StringComparison.Ordinal);
        Assert.DoesNotContain(accessNoteSentinel, audit.ChangesJson, StringComparison.Ordinal);
        Assert.Contains("addressLine1", audit.ChangesJson, StringComparison.Ordinal);
        Assert.Contains("accessNote", audit.ChangesJson, StringComparison.Ordinal);

        // Verify captured logs do not contain the sentinels
        var allLogs = string.Join("\n", _loggerProvider.Messages);
        Assert.DoesNotContain(addressSentinel, allLogs, StringComparison.Ordinal);
        Assert.DoesNotContain(accessNoteSentinel, allLogs, StringComparison.Ordinal);
    }

    private async Task<Opportunity> SeedDraftOpportunityAsync(Guid orgId, Guid branchId, Guid customerId)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTimeOffset.UtcNow;
        var opp = Opportunity.CreateDraft(
            Guid.NewGuid(),
            orgId,
            branchId,
            customerId,
            null,
            TestOnlyDataSeeder.TestUserId,
            TestOnlyDataSeeder.TestUserId,
            "งานระบบบิวต์อิน TEST_ONLY",
            "รายละเอียดงาน",
            new[] { "built-in" },
            null,
            250000m,
            "THB",
            new DateOnly(2026, 12, 1),
            now.AddDays(1),
            "โทรติดตามผล",
            now);
        db.Opportunities.Add(opp);
        await db.SaveChangesAsync();
        return opp;
    }

    [Fact]
    public async Task QualifyOpportunity_ValidRequest_ReturnsQualifiedWithNewETagAndPersistsHistory()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var opp = await SeedDraftOpportunityAsync(OrgAId, BranchAId, customer.Id);

        var request = new TransitionOpportunityStageRequest("qualified", opp.RowVersion);
        var msg = CreateRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{opp.Id}/stage-transitions",
            "token-org-a",
            MembershipAId,
            $"idem-qualify-{Guid.NewGuid():N}");
        msg.Content = JsonContent.Create(request);

        var res = await _client.SendAsync(msg);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var qualified = await res.Content.ReadFromJsonAsync<OpportunityResponse>();
        Assert.NotNull(qualified);
        Assert.Equal("qualified", qualified.Stage);
        Assert.NotEqual(opp.RowVersion, qualified.RowVersion);
        Assert.Equal($"\"{qualified.RowVersion}\"", res.Headers.ETag?.Tag);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var history = await db.OpportunityStageHistories
            .Where(h => h.OpportunityId == opp.Id)
            .ToListAsync();
        Assert.Single(history);
        Assert.Equal("draft", history[0].FromStage);
        Assert.Equal("qualified", history[0].ToStage);

        var audit = await db.AuditEvents
            .Where(a => a.ResourceId == opp.Id.ToString() && a.Action == "opportunity.stage-changed")
            .ToListAsync();
        Assert.Single(audit);
    }
}
