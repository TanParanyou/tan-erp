using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Models;
using TanErp.Application.Crm.Customers;
using TanErp.Application.Crm.Opportunities;
using TanErp.Application.Crm.Opportunities.CreateOpportunity;
using TanErp.Application.Crm.Opportunities.UpdateDraftQGate;
using TanErp.Application.Crm.Opportunities.UpdateOpenOpportunity;
using TanErp.Application.Crm.Opportunities.ReassignOpportunityOwner;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Persistence;
using TanErp.Infrastructure.Persistence.Crm;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

public class OpportunityStoreTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private AppDbContext _db = null!;
    private OpportunityStore _store = null!;
    private static readonly DateTimeOffset FixedTime = DateTimeOffset.Parse("2026-09-09T04:30:00Z");

    private sealed class FixedClock : TanErp.Application.Common.Abstractions.IClock
    {
        public DateTimeOffset UtcNow { get; } = FixedTime;
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        _db = new AppDbContext(options);
        await _db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(_db, "Test", true);

        _store = new OpportunityStore(_db, new FixedClock());
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task Create_InactiveCustomer_ReturnsCustomerInvalidState()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var branchId = TestOnlyDataSeeder.TestBranchId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var customer = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท ร่าง", null, "th",
            new PrimaryContactInput("นาย หนึ่ง", null, "0812345678", null, "phone"), now);
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        var access = new RequestAccessContext(userId, Guid.NewGuid(), orgId, branchId, "opportunities.create", "branch");
        var cmd = new CreateOpportunityCommand("uid", Guid.NewGuid(), customer.Id, null, "งาน 1", null, new[] { "built-in" }, null, null, null, null, null, null, "key-opp-1", "trace-1");

        var result = await _store.CreateAsync(access, cmd, "key-h-1", "payload-h-1");

        Assert.True(result.IsFailure);
        Assert.Equal("CUSTOMER_INVALID_STATE", result.Error.Code);
    }

    [Fact]
    public async Task Create_SiteBelongsToAnotherCustomer_ReturnsResourceNotFound()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var branchId = TestOnlyDataSeeder.TestBranchId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var c1 = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท ก", null, "th",
            new PrimaryContactInput("นาย ก", null, "0812345678", null, "phone"), now);
        c1.Activate(c1.RowVersion);

        var c2 = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท ข", null, "th",
            new PrimaryContactInput("นาย ข", null, "0812345679", null, "phone"), now);
        c2.Activate(c2.RowVersion);
        _db.Customers.AddRange(c1, c2);

        var siteForC2 = Site.CreateActive(Guid.NewGuid(), orgId, c2.Id, userId, "ไซต์ของ ข",
            new SiteAddressInput("123", "ต", "อ", "จ", "10000", "TH"), null, null, null, now);
        _db.Sites.Add(siteForC2);
        await _db.SaveChangesAsync();

        var access = new RequestAccessContext(userId, Guid.NewGuid(), orgId, branchId, "opportunities.create", "branch");
        // Create opportunity for c1, but specifying siteForC2
        var cmd = new CreateOpportunityCommand("uid", Guid.NewGuid(), c1.Id, siteForC2.Id, "งาน 2", null, new[] { "built-in" }, null, null, null, null, null, null, "key-opp-2", "trace-2");

        var result = await _store.CreateAsync(access, cmd, "key-h-2", "payload-h-2");

        Assert.True(result.IsFailure);
        Assert.Equal("RESOURCE_NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task Create_Success_PersistsOpportunityAuditAndIdempotency()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var branchId = TestOnlyDataSeeder.TestBranchId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var c = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท ค", null, "th",
            new PrimaryContactInput("นาย ค", null, "0812345678", null, "phone"), now);
        c.Activate(c.RowVersion);
        _db.Customers.Add(c);

        var site = Site.CreateActive(Guid.NewGuid(), orgId, c.Id, userId, "ไซต์ของ ค",
            new SiteAddressInput("123", "ต", "อ", "จ", "10000", "TH"), null, null, null, now);
        _db.Sites.Add(site);
        await _db.SaveChangesAsync();

        var access = new RequestAccessContext(userId, Guid.NewGuid(), orgId, branchId, "opportunities.create", "branch");
        var cmd = new CreateOpportunityCommand(
            "uid", Guid.NewGuid(), c.Id, site.Id, "Built-in คอนโด", "สำรวจตู้เสื้อผ้า",
            new[] { "built-in" }, null, 250000m, "THB", new DateOnly(2026, 10, 15),
            now.AddDays(1), "นัดยืนยันเวลา", "key-opp-3", "trace-3");

        var result = await _store.CreateAsync(access, cmd, "key-h-3", "payload-h-3");

        Assert.True(result.IsSuccess);
        var opp = result.Value!;
        Assert.NotNull(opp);
        Assert.Equal("Built-in คอนโด", opp.Title);
        Assert.Equal(OpportunityStage.Draft, opp.Stage);
        Assert.Equal(site.Id, opp.PrimarySiteId);
        Assert.Equal(branchId, opp.BranchId);
        Assert.Equal(userId, opp.OwnerUserId);

        // Verify DB entity
        var dbOpp = await _db.Opportunities.FirstOrDefaultAsync(o => o.Id == opp.Id);
        Assert.NotNull(dbOpp);
        Assert.Equal(opp.Code, dbOpp.Code);
        Assert.Equal(FixedTime, dbOpp.CreatedAtUtc);

        // Verify idempotency record
        var idemp = await _db.IdempotencyRecords.FirstOrDefaultAsync(r => r.KeyHash == "key-h-3" && r.OrganizationId == orgId);
        Assert.NotNull(idemp);
        Assert.Equal(opp.Id.ToString(), idemp.ResourceId);
        Assert.Equal(FixedTime, idemp.CreatedAtUtc);

        // Verify audit event
        var audit = await _db.AuditEvents.FirstOrDefaultAsync(a => a.ResourceId == opp.Id.ToString() && a.Action == "opportunity.created");
        Assert.NotNull(audit);
        Assert.Equal("Opportunity", audit.ResourceType);
        Assert.Equal("trace-3", audit.TraceId);
        Assert.Equal(FixedTime, audit.OccurredAtUtc);
    }

    [Fact]
    public async Task Create_ExactReplay_ReturnsSameProjection()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var branchId = TestOnlyDataSeeder.TestBranchId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var c = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท ง", null, "th",
            new PrimaryContactInput("นาย ง", null, "0812345678", null, "phone"), now);
        c.Activate(c.RowVersion);
        _db.Customers.Add(c);
        await _db.SaveChangesAsync();

        var access = new RequestAccessContext(userId, Guid.NewGuid(), orgId, branchId, "opportunities.create", "branch");
        var cmd = new CreateOpportunityCommand("uid", Guid.NewGuid(), c.Id, null, "งานรีเพลย์", null, new[] { "interior" }, null, null, null, null, null, null, "key-opp-4", "trace-4");

        var first = await _store.CreateAsync(access, cmd, "key-h-4", "payload-h-4");
        Assert.True(first.IsSuccess);

        var replay = await _store.CreateAsync(access, cmd, "key-h-4", "payload-h-4");
        Assert.True(replay.IsSuccess);
        Assert.Equal(first.Value!.Id, replay.Value!.Id);
        Assert.Equal(first.Value.Code, replay.Value.Code);
    }

    [Fact]
    public async Task Create_ReusedKeyDifferentPayload_ReturnsConflict()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var branchId = TestOnlyDataSeeder.TestBranchId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var c = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท จ", null, "th",
            new PrimaryContactInput("นาย จ", null, "0812345678", null, "phone"), now);
        c.Activate(c.RowVersion);
        _db.Customers.Add(c);
        await _db.SaveChangesAsync();

        var access = new RequestAccessContext(userId, Guid.NewGuid(), orgId, branchId, "opportunities.create", "branch");
        var cmd1 = new CreateOpportunityCommand("uid", Guid.NewGuid(), c.Id, null, "งาน 1", null, new[] { "interior" }, null, null, null, null, null, null, "key-opp-5", "trace-5");
        var first = await _store.CreateAsync(access, cmd1, "key-h-5", "payload-h-5");
        Assert.True(first.IsSuccess);

        var cmd2 = new CreateOpportunityCommand("uid", Guid.NewGuid(), c.Id, null, "งาน 2", null, new[] { "curtain" }, null, null, null, null, null, null, "key-opp-5", "trace-5");
        var conflict = await _store.CreateAsync(access, cmd2, "key-h-5", "payload-h-DIFFERENT");
        Assert.True(conflict.IsFailure);
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", conflict.Error.Code);
    }

    [Fact]
    public async Task List_PaginationAndOrdering_FollowsContract()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var branchId = TestOnlyDataSeeder.TestBranchId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var c = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท ฉ", null, "th",
            new PrimaryContactInput("นาย ฉ", null, "0812345678", null, "phone"), now);
        c.Activate(c.RowVersion);
        _db.Customers.Add(c);

        // Create 3 opportunities with different nextAction dates:
        // opp1: tomorrow
        // opp2: today
        // opp3: null (should be last)
        var opp1 = Opportunity.CreateDraft(Guid.NewGuid(), orgId, branchId, c.Id, null, userId, userId, "งาน พรุ่งนี้", null, new[] { "built-in" }, null, null, null, null, now.AddDays(1), null, now);
        var opp2 = Opportunity.CreateDraft(Guid.NewGuid(), orgId, branchId, c.Id, null, userId, userId, "งาน วันนี้", null, new[] { "built-in" }, null, null, null, null, now, null, now);
        var opp3 = Opportunity.CreateDraft(Guid.NewGuid(), orgId, branchId, c.Id, null, userId, userId, "งาน ไม่มีวัน", null, new[] { "built-in" }, null, null, null, null, null, null, now);

        _db.Opportunities.AddRange(opp1, opp2, opp3);
        await _db.SaveChangesAsync();

        // Page 1 with Limit 2: should return opp2 (today), opp1 (tomorrow) and a nextCursor
        var page1 = await _store.ListAsync(orgId, new OpportunityListFilter(null, c.Id, null, 2, null));
        Assert.Equal(2, page1.Items.Count);
        Assert.Equal(opp2.Id, page1.Items[0].Id);
        Assert.Equal(opp1.Id, page1.Items[1].Id);
        Assert.NotNull(page1.NextCursor);

        // Page 2 using cursor: should return opp3 (null next action) and no nextCursor
        var page2 = await _store.ListAsync(orgId, new OpportunityListFilter(null, c.Id, null, 2, page1.NextCursor));
        Assert.Single(page2.Items);
        Assert.Equal(opp3.Id, page2.Items[0].Id);
        Assert.Null(page2.NextCursor);
    }

    [Fact]
    public async Task Get_CrossTenantOpportunity_ReturnsNull()
    {
        var org1 = TestOnlyDataSeeder.TestOrgId;
        var org2 = Guid.NewGuid();
        _db.Organizations.Add(new Organization(org2, "Second Org TEST_ONLY"));

        var branch2 = new Branch(Guid.NewGuid(), org2, "BR-02", "สาขา 2", true, DateTimeOffset.UtcNow);
        _db.Branches.Add(branch2);

        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var cInOrg2 = Customer.CreateDraft(Guid.NewGuid(), org2, userId, "organization", "ลูกค้า Org2", null, "th",
            new PrimaryContactInput("นาย สอง", null, "0812345678", null, "phone"), now);
        cInOrg2.Activate(cInOrg2.RowVersion);
        _db.Customers.Add(cInOrg2);

        var oppInOrg2 = Opportunity.CreateDraft(Guid.NewGuid(), org2, branch2.Id, cInOrg2.Id, null, userId, userId, "งาน Org 2", null, new[] { "built-in" }, null, null, null, null, null, null, now);
        _db.Opportunities.Add(oppInOrg2);
        await _db.SaveChangesAsync();

        // Querying with org1 scope for opp in org2
        var result = await _store.GetAsync(org1, oppInOrg2.Id);

        Assert.Null(result);
    }

    private sealed class CoordinatedSaveChangesInterceptor : Microsoft.EntityFrameworkCore.Diagnostics.SaveChangesInterceptor
    {
        private readonly Barrier _barrier = new(2);
        public bool Enabled { get; set; }

        public override async ValueTask<Microsoft.EntityFrameworkCore.Diagnostics.InterceptionResult<int>> SavingChangesAsync(
            Microsoft.EntityFrameworkCore.Diagnostics.DbContextEventData eventData,
            Microsoft.EntityFrameworkCore.Diagnostics.InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (Enabled)
            {
                await Task.Run(() => _barrier.SignalAndWait(cancellationToken), cancellationToken);
            }
            return result;
        }
    }

    [Fact]
    public async Task Create_ConcurrentSamePayload_BothSucceedAndShareSingleResource()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var branchId = TestOnlyDataSeeder.TestBranchId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var customer = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท แข่งออป", null, "th",
            new PrimaryContactInput("นาย แข่งออป", null, "0812345678", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        var interceptor = new CoordinatedSaveChangesInterceptor();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .AddInterceptors(interceptor)
            .Options;

        await using var db1 = new AppDbContext(options);
        await using var db2 = new AppDbContext(options);
        var store1 = new OpportunityStore(db1, new FixedClock());
        var store2 = new OpportunityStore(db2, new FixedClock());

        var access = new RequestAccessContext(userId, Guid.NewGuid(), orgId, branchId, "opportunities.create", "branch");
        var cmd = new CreateOpportunityCommand("uid", Guid.NewGuid(), customer.Id, null, "ออปแข่ง", null, new[] { "built-in" }, null, null, null, null, null, null, "key-opp-conc-1", "trace-opp-conc-1");
        const string keyHash = "opp-conc-key-hash-1";
        const string payloadHash = "opp-conc-payload-hash-1";

        interceptor.Enabled = true;

        var task1 = store1.CreateAsync(access, cmd, keyHash, payloadHash);
        var task2 = store2.CreateAsync(access, cmd, keyHash, payloadHash);

        var results = await Task.WhenAll(task1, task2);

        Assert.All(results, result => Assert.True(result.IsSuccess));
        Assert.Single(results.Select(result => result.Value!.Id).Distinct());

        await using var verificationDb = new AppDbContext(options);
        Assert.Equal(1, await verificationDb.IdempotencyRecords.CountAsync(
            row => row.OrganizationId == orgId && row.Operation == "opportunities.create" && row.KeyHash == keyHash));
        Assert.Equal(1, await verificationDb.AuditEvents.CountAsync(
            row => row.OrganizationId == orgId && row.Action == "opportunity.created"));
        Assert.Equal(1, await verificationDb.Opportunities.CountAsync(
            row => row.Id == results[0].Value!.Id));
    }

    [Fact]
    public async Task Create_ConcurrentDifferentPayload_OneSucceedsOneFailsWithConflict()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var branchId = TestOnlyDataSeeder.TestBranchId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var customer = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท แข่งออปต่าง", null, "th",
            new PrimaryContactInput("นาย แข่งออปสอง", null, "0812345678", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        var interceptor = new CoordinatedSaveChangesInterceptor();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .AddInterceptors(interceptor)
            .Options;

        await using var db1 = new AppDbContext(options);
        await using var db2 = new AppDbContext(options);
        var store1 = new OpportunityStore(db1, new FixedClock());
        var store2 = new OpportunityStore(db2, new FixedClock());

        var access = new RequestAccessContext(userId, Guid.NewGuid(), orgId, branchId, "opportunities.create", "branch");
        var cmd1 = new CreateOpportunityCommand("uid", Guid.NewGuid(), customer.Id, null, "ออปแข่ง 1", null, new[] { "built-in" }, null, null, null, null, null, null, "key-opp-conc-diff", "trace-opp-diff-1");
        var cmd2 = new CreateOpportunityCommand("uid", Guid.NewGuid(), customer.Id, null, "ออปแข่ง 2", null, new[] { "built-in" }, null, null, null, null, null, null, "key-opp-conc-diff", "trace-opp-diff-2");
        const string keyHash = "opp-conc-diff-key-hash";
        const string payloadHash1 = "opp-conc-diff-payload-hash-1";
        const string payloadHash2 = "opp-conc-diff-payload-hash-2";

        interceptor.Enabled = true;

        var task1 = store1.CreateAsync(access, cmd1, keyHash, payloadHash1);
        var task2 = store2.CreateAsync(access, cmd2, keyHash, payloadHash2);

        var results = await Task.WhenAll(task1, task2);

        var successes = results.Where(r => r.IsSuccess).ToList();
        var failures = results.Where(r => r.IsFailure).ToList();

        Assert.Single(successes);
        Assert.Single(failures);
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", failures[0].Error.Code);

        await using var verificationDb = new AppDbContext(options);
        Assert.Equal(1, await verificationDb.Opportunities.CountAsync(o => o.Id == successes[0].Value!.Id));
    }

    [Fact]
    public async Task UpdateDraftQGate_ValidDraft_PersistsFieldsVersionAuditAndReplay()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var branchId = TestOnlyDataSeeder.TestBranchId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        // 1. Create active customer and draft opportunity
        var customer = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท ลูกค้าทดสอบแก้ไขดราฟต์", null, "th",
            new PrimaryContactInput("คุณ สมชาย ทดสอบ", null, "0819998877", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        _db.Customers.Add(customer);

        var opp = Opportunity.CreateDraft(
            Guid.NewGuid(), orgId, branchId, customer.Id, null, userId, userId,
            "โครงการ ปรับปรุงห้องนอน TEST_ONLY", null, new[] { "built-in" },
            null, null, null, null, null, null, now);
        _db.Opportunities.Add(opp);
        await _db.SaveChangesAsync();

        var initialVersion = opp.RowVersion;
        var access = new RequestAccessContext(userId, Guid.NewGuid(), orgId, branchId, "opportunities.update", "organization");
        var nextActionTime = DateTimeOffset.UtcNow.AddDays(3);
        var cmd = new UpdateDraftQGateCommand(
            "uid-update",
            access.MembershipId,
            opp.Id,
            initialVersion,
            "สรุปขอบเขตงานตู้เสื้อผ้าและเตียงนอนบิวต์อิน",
            new[] { "built-in", "interior" },
            nextActionTime,
            "นัดหมายเข้าไปวัดขนาดพื้นที่จริง",
            "key-update-draft-q-gate-001",
            "trace-update-draft-001");

        const string keyHash = "key-hash-update-draft-001";
        const string payloadHash = "payload-hash-update-draft-001";

        // 2. Execute update
        var result = await _store.UpdateDraftQGateAsync(access, cmd, keyHash, payloadHash);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(initialVersion, result.Value!.RowVersion);
        Assert.Equal("draft", result.Value.Stage);
        Assert.Equal("สรุปขอบเขตงานตู้เสื้อผ้าและเตียงนอนบิวต์อิน", result.Value.ScopeSummary);
        Assert.Equal(new[] { "built-in", "interior" }, result.Value.WorkTypes);
        Assert.Equal(nextActionTime, result.Value.NextActionAtUtc);
        Assert.Equal("นัดหมายเข้าไปวัดขนาดพื้นที่จริง", result.Value.NextActionNote);

        // 3. Verify database state
        _db.ChangeTracker.Clear();
        var reloadedOpp = await _db.Opportunities.FindAsync(opp.Id);
        Assert.NotNull(reloadedOpp);
        Assert.Equal("draft", reloadedOpp.Stage);
        Assert.Equal(result.Value.RowVersion, reloadedOpp.RowVersion);
        Assert.Equal("สรุปขอบเขตงานตู้เสื้อผ้าและเตียงนอนบิวต์อิน", reloadedOpp.ScopeSummary);

        // 4. Verify no stage history created
        var historyCount = await _db.OpportunityStageHistories
            .CountAsync(h => h.OpportunityId == opp.Id);
        Assert.Equal(0, historyCount);

        // 5. Verify privacy-safe audit event
        var audit = await _db.AuditEvents
            .Where(a => a.ResourceId == opp.Id.ToString() && a.Action == "opportunity.updated")
            .ToListAsync();
        Assert.Single(audit);
        Assert.DoesNotContain("ตู้เสื้อผ้าและเตียงนอน", audit[0].ChangesJson);
        Assert.DoesNotContain("วัดขนาดพื้นที่จริง", audit[0].ChangesJson);
        using (var doc = System.Text.Json.JsonDocument.Parse(audit[0].ChangesJson))
        {
            var changedFields = doc.RootElement.GetProperty("changedFields");
            Assert.True(changedFields.GetArrayLength() > 0);
        }

        // 6. Verify replay returns same projection and no additional audit
        var replay = await _store.UpdateDraftQGateAsync(access, cmd, keyHash, payloadHash);
        Assert.True(replay.IsSuccess);
        Assert.Equal(result.Value.RowVersion, replay.Value!.RowVersion);

        var auditCountAfterReplay = await _db.AuditEvents
            .CountAsync(a => a.ResourceId == opp.Id.ToString() && a.Action == "opportunity.updated");
        Assert.Equal(1, auditCountAfterReplay);
    }

    [Fact]
    public async Task UpdateOpenOpportunity_ValidRequest_PersistsAudit()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var branchId = TestOnlyDataSeeder.TestBranchId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var c = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท ทดสอบ Open", null, "th",
            new PrimaryContactInput("นาย โอเพ่น", null, "0812345678", null, "phone"), now);
        c.Activate(c.RowVersion);
        _db.Customers.Add(c);

        var opp = Opportunity.CreateDraft(
            Guid.NewGuid(), orgId, branchId, c.Id, null, userId, userId,
            "งานเดิม", "สรุปขอบเขตเริ่มต้น", new[] { "built-in" }, null, null, null, null, now, "นัดหมายเริ่มต้น", now);
        opp.Qualify(opp.RowVersion);
        _db.Opportunities.Add(opp);
        await _db.SaveChangesAsync();

        var initialVersion = opp.RowVersion;
        var access = new RequestAccessContext(userId, Guid.NewGuid(), orgId, branchId, "opportunities.update", "organization");
        var nextActionTime = now.AddDays(7);

        var cmd = new UpdateOpenOpportunityCommand(
            "test-uid",
            Guid.NewGuid(),
            opp.Id,
            initialVersion,
            "งานปรับปรุงห้องรับรองแขก VIP",
            null,
            "สรุปงานออกแบบตกแต่งครบวงจร",
            new[] { "built-in", "interior" },
            "referral",
            500000m,
            "THB",
            new DateOnly(2026, 11, 20),
            nextActionTime,
            "นัดส่งแบบร่างรอบแรก",
            "key-open-001",
            "trace-open-001");

        const string keyHash = "key-hash-open-001";
        const string payloadHash = "payload-hash-open-001";

        var result = await _store.UpdateOpenAsync(access, cmd, keyHash, payloadHash);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(initialVersion, result.Value!.RowVersion);
        Assert.Equal("qualified", result.Value.Stage);
        Assert.Equal("งานปรับปรุงห้องรับรองแขก VIP", result.Value.Title);
        Assert.Equal(500000m, result.Value.ExpectedBudget);

        // Verify privacy-safe audit event
        var audit = await _db.AuditEvents
            .Where(a => a.ResourceId == opp.Id.ToString() && a.Action == "opportunity.updated")
            .OrderByDescending(a => a.OccurredAtUtc)
            .FirstOrDefaultAsync();
        Assert.NotNull(audit);
        Assert.DoesNotContain("VIP", audit.ChangesJson);
        Assert.DoesNotContain("500000", audit.ChangesJson);
    }

    [Fact]
    public async Task ReassignOwner_ActiveSameBranch_ChangesOwner()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var branchId = TestOnlyDataSeeder.TestBranchId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var c = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท ทดสอบ Reassign", null, "th",
            new PrimaryContactInput("นาย รีแอสไซน์", null, "0812345678", null, "phone"), now);
        c.Activate(c.RowVersion);
        _db.Customers.Add(c);

        // Create new user & membership in same branch
        var newOwnerUser = new User(Guid.NewGuid(), "new-owner-uid", "พนักงานขาย สอง", "owner2@example.test", isActive: true);
        var newOwnerMembership = new Membership(Guid.NewGuid(), orgId, branchId, newOwnerUser.Id, isActive: true);
        _db.Users.Add(newOwnerUser);
        _db.Memberships.Add(newOwnerMembership);

        var opp = Opportunity.CreateDraft(
            Guid.NewGuid(), orgId, branchId, c.Id, null, userId, userId,
            "งานขายที่จะเปลี่ยนเจ้าของ", null, new[] { "built-in" }, null, null, null, null, null, null, now);
        _db.Opportunities.Add(opp);
        await _db.SaveChangesAsync();

        var initialVersion = opp.RowVersion;
        var access = new RequestAccessContext(userId, Guid.NewGuid(), orgId, branchId, "opportunities.update", "organization");

        var cmd = new ReassignOpportunityOwnerCommand(
            "test-uid",
            Guid.NewGuid(),
            opp.Id,
            initialVersion,
            newOwnerUser.Id,
            "key-reassign-001",
            "trace-reassign-001");

        const string keyHash = "key-hash-reassign-001";
        const string payloadHash = "payload-hash-reassign-001";

        var result = await _store.ReassignOwnerAsync(access, cmd, keyHash, payloadHash);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(initialVersion, result.Value!.RowVersion);
        Assert.Equal(newOwnerUser.Id, result.Value.OwnerUserId);

        // Verify audit log has IDs only and no PII
        var audit = await _db.AuditEvents
            .Where(a => a.ResourceId == opp.Id.ToString() && a.Action == "opportunity.owner-changed")
            .FirstOrDefaultAsync();
        Assert.NotNull(audit);
        Assert.Contains(newOwnerUser.Id.ToString("D"), audit.ChangesJson);
        Assert.Contains(userId.ToString("D"), audit.ChangesJson);
        Assert.DoesNotContain("พนักงานขาย สอง", audit.ChangesJson);
    }
}

