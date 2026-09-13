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

    [Fact]
    public async Task PatchOpportunity_ValidDraft_ReturnsUpdatedResponseAndETag()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var opp = await SeedDraftOpportunityAsync(OrgAId, BranchAId, customer.Id);

        var nextActionTime = DateTimeOffset.UtcNow.AddDays(5);
        var patchBody = new UpdateDraftQGateRequest(
            "ปรับปรุงขอบเขตงาน Walk-in Closet",
            new[] { "built-in", "interior" },
            nextActionTime,
            "ส่งรายละเอียดแบบเบื้องต้นให้ลูกค้า");

        var msg = CreateRequest(
            HttpMethod.Patch,
            $"/api/v1/opportunities/{opp.Id}",
            "token-org-a",
            MembershipAId,
            $"idem-patch-{Guid.NewGuid():N}");
        msg.Headers.IfMatch.Add(new EntityTagHeaderValue($"\"{opp.RowVersion}\""));
        msg.Content = JsonContent.Create(patchBody);

        var res = await _client.SendAsync(msg);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var updated = await res.Content.ReadFromJsonAsync<OpportunityResponse>();
        Assert.NotNull(updated);
        Assert.Equal("draft", updated.Stage);
        Assert.NotEqual(opp.RowVersion, updated.RowVersion);
        Assert.Equal($"\"{updated.RowVersion}\"", res.Headers.ETag?.Tag);
        Assert.Equal("ปรับปรุงขอบเขตงาน Walk-in Closet", updated.ScopeSummary);
        Assert.Equal(new[] { "built-in", "interior" }, updated.WorkTypes);
        Assert.Equal("ส่งรายละเอียดแบบเบื้องต้นให้ลูกค้า", updated.NextActionNote);

        // Verify GET confirms persistence
        var getMsg = CreateRequest(
            HttpMethod.Get,
            $"/api/v1/opportunities/{opp.Id}",
            "token-org-a",
            MembershipAId);
        var getRes = await _client.SendAsync(getMsg);
        Assert.Equal(HttpStatusCode.OK, getRes.StatusCode);
        var reloaded = await getRes.Content.ReadFromJsonAsync<OpportunityResponse>();
        Assert.NotNull(reloaded);
        Assert.Equal(updated.RowVersion, reloaded.RowVersion);
        Assert.Equal("ปรับปรุงขอบเขตงาน Walk-in Closet", reloaded.ScopeSummary);
    }

    [Fact]
    public async Task PutOpenOpportunity_ValidRequest_ReturnsETag()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var opp = await SeedDraftOpportunityAsync(OrgAId, BranchAId, customer.Id);

        var updateReq = new UpdateOpenOpportunityRequest(
            "Kitchen Renovation Open Update",
            null,
            "Renovate pantry and kitchen island",
            new[] { "built-in", "interior" },
            "referral",
            550000m,
            "THB",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
            DateTimeOffset.UtcNow.AddDays(7),
            "Call client for design confirmation");

        var msg = CreateRequest(
            HttpMethod.Put,
            $"/api/v1/opportunities/{opp.Id}",
            "token-org-a",
            MembershipAId,
            $"idem-put-{Guid.NewGuid():N}");
        msg.Headers.IfMatch.Add(new EntityTagHeaderValue($"\"{opp.RowVersion}\""));
        msg.Content = JsonContent.Create(updateReq);

        var res = await _client.SendAsync(msg);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var updated = await res.Content.ReadFromJsonAsync<OpportunityResponse>();
        Assert.NotNull(updated);
        Assert.NotEqual(opp.RowVersion, updated.RowVersion);
        Assert.Equal($"\"{updated.RowVersion}\"", res.Headers.ETag?.Tag);
        Assert.Equal("Kitchen Renovation Open Update", updated.Title);
        Assert.Equal(550000m, updated.ExpectedBudget);
        Assert.Equal("Renovate pantry and kitchen island", updated.ScopeSummary);
    }

    [Fact]
    public async Task PutOpenOpportunity_WithLocalOffsetNextActionAtUtc_Succeeds()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var opp = await SeedDraftOpportunityAsync(OrgAId, BranchAId, customer.Id);

        var json = """{"title":"ำกไำกไำ","primarySiteId":null,"scopeSummary":"ก","workTypes":["interior","other"],"sourceCode":"architect_partner","expectedBudget":500,"currencyCode":"THB","targetDecisionDate":"2026-09-12","nextActionAtUtc":"2026-09-30T09:00","nextActionNote":"dw - นัดพบบอร์ดบริหาร/ผู้มีอำนาจตัดสินใจ - นัดพบบอร์ดบริหาร/ผู้มีอำนาจตัดสินใจ - นัดพบบอร์ดบริหาร/ผู้มีอำนาจตัดสินใจ"}""";

        var msg = CreateRequest(
            HttpMethod.Put,
            $"/api/v1/opportunities/{opp.Id}",
            "token-org-a",
            MembershipAId,
            $"idem-put-{Guid.NewGuid():N}");
        msg.Headers.IfMatch.Add(new EntityTagHeaderValue($"\"{opp.RowVersion}\""));
        msg.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var res = await _client.SendAsync(msg);
        var body = await res.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task PostReassignOwner_ValidRequest_ReturnsETag()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var opp = await SeedDraftOpportunityAsync(OrgAId, BranchAId, customer.Id);

        // Seed a target owner user and membership in same branch
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var newOwnerUser = new User(Guid.NewGuid(), "target-owner-uid", "พนักงานขาย สอง", "targetowner@example.test", isActive: true);
            var newOwnerMembership = new Membership(Guid.NewGuid(), OrgAId, BranchAId, newOwnerUser.Id, isActive: true);
            db.Users.Add(newOwnerUser);
            db.Memberships.Add(newOwnerMembership);
            await db.SaveChangesAsync();

            var reassignReq = new ReassignOpportunityOwnerRequest(
                newOwnerUser.Id,
                opp.RowVersion);

            var msg = CreateRequest(
                HttpMethod.Post,
                $"/api/v1/opportunities/{opp.Id}/owner-changes",
                "token-org-a",
                MembershipAId,
                $"idem-reassign-{Guid.NewGuid():N}");
            msg.Headers.IfMatch.Add(new EntityTagHeaderValue($"\"{opp.RowVersion}\""));
            msg.Content = JsonContent.Create(reassignReq);

            var res = await _client.SendAsync(msg);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var updated = await res.Content.ReadFromJsonAsync<OpportunityResponse>();
            Assert.NotNull(updated);
            Assert.NotEqual(opp.RowVersion, updated.RowVersion);
            Assert.Equal($"\"{updated.RowVersion}\"", res.Headers.ETag?.Tag);
            Assert.Equal(newOwnerUser.Id, updated.OwnerUserId);
        }
    }

    [Fact]
    public async Task PostStageTransitions_CloseLost_ValidReason_AppendsHistory()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var opp = await SeedDraftOpportunityAsync(OrgAId, BranchAId, customer.Id);

        var req = new TransitionOpportunityStageRequest(
            "lost",
            opp.RowVersion,
            OpportunityReasonCodes.LostPriceTooHigh,
            "ลูกค้างบประมาณไม่พอ");

        var msg = CreateRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{opp.Id}/stage-transitions",
            "token-org-a",
            MembershipAId,
            $"idem-lost-{Guid.NewGuid():N}");
        msg.Content = JsonContent.Create(req);

        var res = await _client.SendAsync(msg);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var updated = await res.Content.ReadFromJsonAsync<OpportunityResponse>();
        Assert.NotNull(updated);
        Assert.Equal("lost", updated.Stage);
        Assert.NotEqual(opp.RowVersion, updated.RowVersion);

        // Verify stage history
        var histMsg = CreateRequest(
            HttpMethod.Get,
            $"/api/v1/opportunities/{opp.Id}/stage-history",
            "token-org-a",
            MembershipAId);
        var histRes = await _client.SendAsync(histMsg);
        Assert.Equal(HttpStatusCode.OK, histRes.StatusCode);

        var historyList = await histRes.Content.ReadFromJsonAsync<OpportunityStageHistoryListResponse>();
        Assert.NotNull(historyList);
        Assert.NotEmpty(historyList.Items);
        var latest = historyList.Items.First();
        Assert.Equal("draft", latest.FromStage);
        Assert.Equal("lost", latest.ToStage);
        Assert.Equal(OpportunityReasonCodes.LostPriceTooHigh, latest.ReasonCode);
    }

    [Fact]
    public async Task PostStageTransitions_CloseCancelled_ValidReason_AppendsHistory()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var opp = await SeedDraftOpportunityAsync(OrgAId, BranchAId, customer.Id);

        var req = new TransitionOpportunityStageRequest(
            "cancelled",
            opp.RowVersion,
            OpportunityReasonCodes.CancelledCustomerAbandoned);

        var msg = CreateRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{opp.Id}/stage-transitions",
            "token-org-a",
            MembershipAId,
            $"idem-cancel-{Guid.NewGuid():N}");
        msg.Content = JsonContent.Create(req);

        var res = await _client.SendAsync(msg);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var updated = await res.Content.ReadFromJsonAsync<OpportunityResponse>();
        Assert.NotNull(updated);
        Assert.Equal("cancelled", updated.Stage);
        Assert.NotEqual(opp.RowVersion, updated.RowVersion);
    }

    [Fact]
    public async Task PostStageTransitions_Reopen_ApprovedTarget_AppendsHistory()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var opp = await SeedDraftOpportunityAsync(OrgAId, BranchAId, customer.Id);

        // First close as lost
        var closeReq = new TransitionOpportunityStageRequest(
            "lost",
            opp.RowVersion,
            OpportunityReasonCodes.LostPriceTooHigh);

        var closeMsg = CreateRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{opp.Id}/stage-transitions",
            "token-org-a",
            MembershipAId,
            $"idem-close-{Guid.NewGuid():N}");
        closeMsg.Content = JsonContent.Create(closeReq);

        var closeRes = await _client.SendAsync(closeMsg);
        Assert.Equal(HttpStatusCode.OK, closeRes.StatusCode);
        var closedOpp = await closeRes.Content.ReadFromJsonAsync<OpportunityResponse>();
        Assert.NotNull(closedOpp);

        // Now reopen to draft
        var reopenReq = new TransitionOpportunityStageRequest(
            "draft",
            closedOpp.RowVersion,
            OpportunityReasonCodes.ReopenBudgetAdjusted,
            "ลูกค้าได้รับงบเพิ่ม");

        var reopenMsg = CreateRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{opp.Id}/stage-transitions",
            "token-org-a",
            MembershipAId,
            $"idem-reopen-{Guid.NewGuid():N}");
        reopenMsg.Content = JsonContent.Create(reopenReq);

        var reopenRes = await _client.SendAsync(reopenMsg);
        Assert.Equal(HttpStatusCode.OK, reopenRes.StatusCode);
        var reopenedOpp = await reopenRes.Content.ReadFromJsonAsync<OpportunityResponse>();
        Assert.NotNull(reopenedOpp);
        Assert.Equal("draft", reopenedOpp.Stage);
        Assert.NotEqual(closedOpp.RowVersion, reopenedOpp.RowVersion);

        // Verify history has 2 entries (lost, then draft)
        var histMsg = CreateRequest(
            HttpMethod.Get,
            $"/api/v1/opportunities/{opp.Id}/stage-history",
            "token-org-a",
            MembershipAId);
        var histRes = await _client.SendAsync(histMsg);
        var historyList = await histRes.Content.ReadFromJsonAsync<OpportunityStageHistoryListResponse>();
        Assert.NotNull(historyList);
        Assert.Equal(2, historyList.Items.Count);
        Assert.Equal("draft", historyList.Items[0].ToStage);
        Assert.Equal(OpportunityReasonCodes.ReopenBudgetAdjusted, historyList.Items[0].ReasonCode);
        Assert.Equal("lost", historyList.Items[1].ToStage);
    }

    [Fact]
    public async Task Hardening_CrossOrgIsolation_AllMutationsAndQueriesReturn404()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var opp = await SeedDraftOpportunityAsync(OrgAId, BranchAId, customer.Id);

        // Org B attempts to transition stage of Org A -> 404
        var transReq = new TransitionOpportunityStageRequest("qualified", opp.RowVersion);
        var transMsg = CreateRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{opp.Id}/stage-transitions",
            "token-org-b",
            MembershipBId,
            $"idem-cross-trans-{Guid.NewGuid():N}");
        transMsg.Content = JsonContent.Create(transReq);
        var transRes = await _client.SendAsync(transMsg);
        Assert.Equal(HttpStatusCode.NotFound, transRes.StatusCode);

        // Org B attempts to read stage history of Org A -> 404
        var histMsg = CreateRequest(
            HttpMethod.Get,
            $"/api/v1/opportunities/{opp.Id}/stage-history",
            "token-org-b",
            MembershipBId);
        var histRes = await _client.SendAsync(histMsg);
        Assert.Equal(HttpStatusCode.NotFound, histRes.StatusCode);

        // Org B attempts to PATCH draft of Org A -> 404
        var patchReq = new UpdateDraftQGateRequest("Cross Org Scope", new[] { "built-in" }, null, null);
        var patchMsg = CreateRequest(
            HttpMethod.Patch,
            $"/api/v1/opportunities/{opp.Id}",
            "token-org-b",
            MembershipBId,
            $"idem-cross-patch-{Guid.NewGuid():N}");
        patchMsg.Headers.IfMatch.Add(new EntityTagHeaderValue($"\"{opp.RowVersion}\""));
        patchMsg.Content = JsonContent.Create(patchReq);
        var patchRes = await _client.SendAsync(patchMsg);
        Assert.Equal(HttpStatusCode.NotFound, patchRes.StatusCode);

        // Org B attempts to reassign owner of Org A -> 404
        var reassignReq = new ReassignOpportunityOwnerRequest(opp.RowVersion, Guid.NewGuid());
        var reassignMsg = CreateRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{opp.Id}/owner-changes",
            "token-org-b",
            MembershipBId,
            $"idem-cross-owner-{Guid.NewGuid():N}");
        reassignMsg.Headers.IfMatch.Add(new EntityTagHeaderValue($"\"{opp.RowVersion}\""));
        reassignMsg.Content = JsonContent.Create(reassignReq);
        var reassignRes = await _client.SendAsync(reassignMsg);
        Assert.Equal(HttpStatusCode.NotFound, reassignRes.StatusCode);
    }

    [Fact]
    public async Task Hardening_PermissionDenial_NoPermUserGets403()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var opp = await SeedDraftOpportunityAsync(OrgAId, BranchAId, customer.Id);

        // User without opportunities.transition attempts qualify -> 403
        var transReq = new TransitionOpportunityStageRequest("qualified", opp.RowVersion);
        var transMsg = CreateRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{opp.Id}/stage-transitions",
            "token-no-perm",
            MembershipNoPermId,
            $"idem-noperm-trans-{Guid.NewGuid():N}");
        transMsg.Content = JsonContent.Create(transReq);
        var transRes = await _client.SendAsync(transMsg);
        Assert.Equal(HttpStatusCode.Forbidden, transRes.StatusCode);

        // User without opportunities.update attempts PATCH draft -> 403
        var patchReq = new UpdateDraftQGateRequest("No Perm Scope", new[] { "built-in" }, null, null);
        var patchMsg = CreateRequest(
            HttpMethod.Patch,
            $"/api/v1/opportunities/{opp.Id}",
            "token-no-perm",
            MembershipNoPermId,
            $"idem-noperm-patch-{Guid.NewGuid():N}");
        patchMsg.Headers.IfMatch.Add(new EntityTagHeaderValue($"\"{opp.RowVersion}\""));
        patchMsg.Content = JsonContent.Create(patchReq);
        var patchRes = await _client.SendAsync(patchMsg);
        Assert.Equal(HttpStatusCode.Forbidden, patchRes.StatusCode);
    }

    [Fact]
    public async Task Hardening_StaleVersion_Returns409Conflict()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var opp = await SeedDraftOpportunityAsync(OrgAId, BranchAId, customer.Id);

        var staleVersion = Guid.NewGuid();

        // Stale expectedVersion in stage transition -> 409
        var transReq = new TransitionOpportunityStageRequest("qualified", staleVersion);
        var transMsg = CreateRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{opp.Id}/stage-transitions",
            "token-org-a",
            MembershipAId,
            $"idem-stale-trans-{Guid.NewGuid():N}");
        transMsg.Content = JsonContent.Create(transReq);
        var transRes = await _client.SendAsync(transMsg);
        Assert.Equal(HttpStatusCode.Conflict, transRes.StatusCode);

        // Stale If-Match in PATCH draft -> 409
        var patchReq = new UpdateDraftQGateRequest("Stale Version Update", new[] { "built-in" }, null, null);
        var patchMsg = CreateRequest(
            HttpMethod.Patch,
            $"/api/v1/opportunities/{opp.Id}",
            "token-org-a",
            MembershipAId,
            $"idem-stale-patch-{Guid.NewGuid():N}");
        patchMsg.Headers.IfMatch.Add(new EntityTagHeaderValue($"\"{staleVersion}\""));
        patchMsg.Content = JsonContent.Create(patchReq);
        var patchRes = await _client.SendAsync(patchMsg);
        Assert.Equal(HttpStatusCode.Conflict, patchRes.StatusCode);
    }

    [Fact]
    public async Task Hardening_MissingIfMatch_PatchReturns428PreconditionRequired()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var opp = await SeedDraftOpportunityAsync(OrgAId, BranchAId, customer.Id);

        var patchReq = new UpdateDraftQGateRequest("Missing IfMatch", new[] { "built-in" }, null, null);
        var patchMsg = CreateRequest(
            HttpMethod.Patch,
            $"/api/v1/opportunities/{opp.Id}",
            "token-org-a",
            MembershipAId,
            $"idem-noifmatch-{Guid.NewGuid():N}");
        // Do NOT add If-Match header
        patchMsg.Content = JsonContent.Create(patchReq);
        var patchRes = await _client.SendAsync(patchMsg);
        Assert.Equal((HttpStatusCode)428, patchRes.StatusCode);
    }

    [Fact]
    public async Task Hardening_IdempotencyReplayAndConflict_BehavesDeterministically()
    {
        var customer = await SeedActiveCustomerAsync(OrgAId);
        var opp = await SeedDraftOpportunityAsync(OrgAId, BranchAId, customer.Id);

        var idempotencyKey = $"idem-test-replay-{Guid.NewGuid():N}";
        var req = new TransitionOpportunityStageRequest("cancelled", opp.RowVersion, OpportunityReasonCodes.CancelledDuplicate);

        // First call -> 200 OK
        var firstMsg = CreateRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{opp.Id}/stage-transitions",
            "token-org-a",
            MembershipAId,
            idempotencyKey);
        firstMsg.Content = JsonContent.Create(req);
        var firstRes = await _client.SendAsync(firstMsg);
        Assert.Equal(HttpStatusCode.OK, firstRes.StatusCode);
        var firstBody = await firstRes.Content.ReadFromJsonAsync<OpportunityResponse>();
        Assert.NotNull(firstBody);

        // Second call with same idempotency key and identical payload -> 200 OK replay
        var replayMsg = CreateRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{opp.Id}/stage-transitions",
            "token-org-a",
            MembershipAId,
            idempotencyKey);
        replayMsg.Content = JsonContent.Create(req);
        var replayRes = await _client.SendAsync(replayMsg);
        Assert.Equal(HttpStatusCode.OK, replayRes.StatusCode);
        var replayBody = await replayRes.Content.ReadFromJsonAsync<OpportunityResponse>();
        Assert.NotNull(replayBody);
        Assert.Equal(firstBody.RowVersion, replayBody.RowVersion);

        // Verify history has only 1 entry (not duplicated)
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var historyCount = await db.OpportunityStageHistories
            .CountAsync(h => h.OpportunityId == opp.Id);
        Assert.Equal(1, historyCount);

        // Third call with same idempotency key but different payload -> 409 IDEMPOTENCY_KEY_REUSED
        var conflictReq = new TransitionOpportunityStageRequest("lost", opp.RowVersion, OpportunityReasonCodes.LostOther);
        var conflictMsg = CreateRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{opp.Id}/stage-transitions",
            "token-org-a",
            MembershipAId,
            idempotencyKey);
        conflictMsg.Content = JsonContent.Create(conflictReq);
        var conflictRes = await _client.SendAsync(conflictMsg);
        Assert.Equal(HttpStatusCode.Conflict, conflictRes.StatusCode);
    }
}

