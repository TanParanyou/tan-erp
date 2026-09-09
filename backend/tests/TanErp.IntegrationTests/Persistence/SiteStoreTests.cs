using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Models;
using TanErp.Application.Crm.Customers;
using TanErp.Application.Crm.Sites.CreateSite;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Persistence;
using TanErp.Infrastructure.Persistence.Crm;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

public class SiteStoreTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private AppDbContext _db = null!;
    private SiteStore _store = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        _db = new AppDbContext(options);
        await _db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(_db, "Test", true);

        _store = new SiteStore(_db);
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
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        // Customer in Draft state
        var customer = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท ดราฟต์", null, "th",
            new PrimaryContactInput("นาย หนึ่ง", null, "0812345678", null, "phone"), now);
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        var access = new RequestAccessContext(userId, Guid.NewGuid(), orgId, null, "sites.manage", "organization");
        var cmd = new CreateSiteCommand("uid", Guid.NewGuid(), customer.Id, "key-1", "บ้านพัก", "123", "ต", "อ", "จ", "10000", "TH", null, null, null, "trace-1");

        var result = await _store.CreateAsync(access, cmd, "key-hash-1", "payload-hash-1");

        Assert.True(result.IsFailure);
        Assert.Equal("CUSTOMER_INVALID_STATE", result.Error.Code);
    }

    [Fact]
    public async Task Create_CustomerNotInTenant_ReturnsResourceNotFound()
    {
        var org1 = TestOnlyDataSeeder.TestOrgId;
        var org2 = Guid.NewGuid();
        _db.Organizations.Add(new Organization(org2, "Other Org TEST_ONLY"));

        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var customer = Customer.CreateDraft(Guid.NewGuid(), org2, userId, "organization", "บริษัท ต่างองค์กร", null, "th",
            new PrimaryContactInput("นาย สอง", null, "0812345678", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        // access belongs to org1, customer belongs to org2
        var access = new RequestAccessContext(userId, Guid.NewGuid(), org1, null, "sites.manage", "organization");
        var cmd = new CreateSiteCommand("uid", Guid.NewGuid(), customer.Id, "key-2", "บ้านพัก", "123", "ต", "อ", "จ", "10000", "TH", null, null, null, "trace-2");

        var result = await _store.CreateAsync(access, cmd, "key-hash-2", "payload-hash-2");

        Assert.True(result.IsFailure);
        Assert.Equal("RESOURCE_NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task Create_Success_PersistsSiteAuditAndIdempotency()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var customer = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท แอคทีฟ", null, "th",
            new PrimaryContactInput("นาย สาม", null, "0812345678", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        var access = new RequestAccessContext(userId, Guid.NewGuid(), orgId, null, "sites.manage", "organization");
        var cmd = new CreateSiteCommand("uid", Guid.NewGuid(), customer.Id, "key-3", "บ้านพักตากอากาศ", "123 หมู่ 4", "บางตลาด", "ปากเกร็ด", "นนทบุรี", "11120", "TH", 13.7563m, 100.5018m, "มีที่จอดรถ", "trace-3");

        var result = await _store.CreateAsync(access, cmd, "key-hash-3", "payload-hash-3");

        Assert.True(result.IsSuccess);
        var siteProj = result.Value;
        Assert.NotNull(siteProj);
        Assert.Equal("บ้านพักตากอากาศ", siteProj.Label);
        Assert.Equal(SiteStatus.Active, siteProj.Status);

        // Verify Site persisted in DB
        var persistedSite = await _db.Sites.FirstOrDefaultAsync(s => s.Id == siteProj.Id);
        Assert.NotNull(persistedSite);
        Assert.Equal(siteProj.Code, persistedSite.Code);

        // Verify Idempotency record persisted
        var idemp = await _db.IdempotencyRecords.FirstOrDefaultAsync(r => r.KeyHash == "key-hash-3" && r.OrganizationId == orgId);
        Assert.NotNull(idemp);
        Assert.Equal(siteProj.Id.ToString(), idemp.ResourceId);

        // Verify Audit record persisted
        var audit = await _db.AuditEvents.FirstOrDefaultAsync(a => a.ResourceId == siteProj.Id.ToString() && a.Action == "site.created");
        Assert.NotNull(audit);
        Assert.Equal("Site", audit.ResourceType);
        Assert.Equal("trace-3", audit.TraceId);
    }

    [Fact]
    public async Task Create_ExactReplay_ReturnsSameProjection()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var customer = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท รีเพลย์", null, "th",
            new PrimaryContactInput("นาย สี่", null, "0812345678", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        var access = new RequestAccessContext(userId, Guid.NewGuid(), orgId, null, "sites.manage", "organization");
        var cmd = new CreateSiteCommand("uid", Guid.NewGuid(), customer.Id, "key-4", "ไซต์ 1", "123", "ต", "อ", "จ", "10000", "TH", null, null, null, "trace-4");

        var first = await _store.CreateAsync(access, cmd, "key-hash-4", "payload-hash-4");
        Assert.True(first.IsSuccess);

        var replay = await _store.CreateAsync(access, cmd, "key-hash-4", "payload-hash-4");
        Assert.True(replay.IsSuccess);
        Assert.Equal(first.Value!.Id, replay.Value!.Id);
        Assert.Equal(first.Value.Code, replay.Value.Code);
    }

    [Fact]
    public async Task Create_ReusedKeyWithDifferentPayload_ReturnsConflict()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var customer = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท คอนฟลิกต์", null, "th",
            new PrimaryContactInput("นาย ห้า", null, "0812345678", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        var access = new RequestAccessContext(userId, Guid.NewGuid(), orgId, null, "sites.manage", "organization");
        var cmd1 = new CreateSiteCommand("uid", Guid.NewGuid(), customer.Id, "key-5", "ไซต์ 1", "123", "ต", "อ", "จ", "10000", "TH", null, null, null, "trace-5");
        var first = await _store.CreateAsync(access, cmd1, "key-hash-5", "payload-hash-5");
        Assert.True(first.IsSuccess);

        var cmd2 = new CreateSiteCommand("uid", Guid.NewGuid(), customer.Id, "key-5", "ไซต์ 2", "999", "ต", "อ", "จ", "10000", "TH", null, null, null, "trace-5");
        var conflict = await _store.CreateAsync(access, cmd2, "key-hash-5", "payload-hash-DIFFERENT");
        Assert.True(conflict.IsFailure);
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", conflict.Error.Code);
    }

    [Fact]
    public async Task ListByCustomer_CustomerNotFound_ReturnsNull()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var missingCustId = Guid.NewGuid();

        var result = await _store.ListByCustomerAsync(orgId, missingCustId);

        Assert.Null(result);
    }

    [Fact]
    public async Task ListByCustomer_CustomerFound_ReturnsOrderedSites()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var customer = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท ไซต์ลิสต์", null, "th",
            new PrimaryContactInput("นาย หก", null, "0812345678", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        _db.Customers.Add(customer);

        var s1 = Site.CreateActive(Guid.NewGuid(), orgId, customer.Id, userId, "ไซต์แรก",
            new SiteAddressInput("1", "ต", "อ", "จ", "10000", "TH"), null, null, null, now.AddMinutes(-10));
        var s2 = Site.CreateActive(Guid.NewGuid(), orgId, customer.Id, userId, "ไซต์สอง",
            new SiteAddressInput("2", "ต", "อ", "จ", "10000", "TH"), null, null, null, now);
        _db.Sites.AddRange(s1, s2);
        await _db.SaveChangesAsync();

        var result = await _store.ListByCustomerAsync(orgId, customer.Id);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(s1.Id, result[0].Id);
        Assert.Equal(s2.Id, result[1].Id);
    }
}
