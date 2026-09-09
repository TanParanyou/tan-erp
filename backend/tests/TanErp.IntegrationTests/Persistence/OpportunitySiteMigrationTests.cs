using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

public class OpportunitySiteMigrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private AppDbContext _db = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        _db = new AppDbContext(options);
        await _db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(_db, "Test", true);
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task Migration_CanRollbackAndReapply()
    {
        // Rehearsal rollback to the migration prior to OpportunitySiteSlice
        await _db.Database.MigrateAsync("20260908160415_AddCustomerQuickIntakeFields");

        // Verify sites and opportunities tables are gone
        var siteExists = await _db.Database
            .SqlQueryRaw<bool>(@"SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'crm' AND table_name = 'sites') AS ""Value""")
            .FirstAsync();
        Assert.False(siteExists);

        var oppExists = await _db.Database
            .SqlQueryRaw<bool>(@"SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'crm' AND table_name = 'opportunities') AS ""Value""")
            .FirstAsync();
        Assert.False(oppExists);

        // Reapply all migrations
        await _db.Database.MigrateAsync();

        var siteReapplyExists = await _db.Database
            .SqlQueryRaw<bool>(@"SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'crm' AND table_name = 'sites') AS ""Value""")
            .FirstAsync();
        Assert.True(siteReapplyExists);

        var oppReapplyExists = await _db.Database
            .SqlQueryRaw<bool>(@"SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'crm' AND table_name = 'opportunities') AS ""Value""")
            .FirstAsync();
        Assert.True(oppReapplyExists);
    }

    [Fact]
    public async Task Migration_ExposesExactColumns()
    {
        async Task<bool> ColumnExists(string schema, string table, string column) =>
            await _db.Database
                .SqlQueryRaw<bool>(@"SELECT EXISTS (SELECT FROM information_schema.columns WHERE table_schema = {0} AND table_name = {1} AND column_name = {2}) AS ""Value""", schema, table, column)
                .FirstAsync();

        // Sites columns
        Assert.True(await ColumnExists("crm", "sites", "id"));
        Assert.True(await ColumnExists("crm", "sites", "organization_id"));
        Assert.True(await ColumnExists("crm", "sites", "customer_id"));
        Assert.True(await ColumnExists("crm", "sites", "code"));
        Assert.True(await ColumnExists("crm", "sites", "label"));
        Assert.True(await ColumnExists("crm", "sites", "normalized_label"));
        Assert.True(await ColumnExists("crm", "sites", "address_line1"));
        Assert.True(await ColumnExists("crm", "sites", "subdistrict"));
        Assert.True(await ColumnExists("crm", "sites", "district"));
        Assert.True(await ColumnExists("crm", "sites", "province"));
        Assert.True(await ColumnExists("crm", "sites", "postal_code"));
        Assert.True(await ColumnExists("crm", "sites", "country_code"));
        Assert.True(await ColumnExists("crm", "sites", "latitude"));
        Assert.True(await ColumnExists("crm", "sites", "longitude"));
        Assert.True(await ColumnExists("crm", "sites", "access_note"));
        Assert.True(await ColumnExists("crm", "sites", "status"));
        Assert.True(await ColumnExists("crm", "sites", "row_version"));
        Assert.True(await ColumnExists("crm", "sites", "created_at_utc"));
        Assert.True(await ColumnExists("crm", "sites", "created_by_user_id"));

        // Opportunities columns
        Assert.True(await ColumnExists("crm", "opportunities", "id"));
        Assert.True(await ColumnExists("crm", "opportunities", "organization_id"));
        Assert.True(await ColumnExists("crm", "opportunities", "branch_id"));
        Assert.True(await ColumnExists("crm", "opportunities", "customer_id"));
        Assert.True(await ColumnExists("crm", "opportunities", "primary_site_id"));
        Assert.True(await ColumnExists("crm", "opportunities", "owner_user_id"));
        Assert.True(await ColumnExists("crm", "opportunities", "code"));
        Assert.True(await ColumnExists("crm", "opportunities", "title"));
        Assert.True(await ColumnExists("crm", "opportunities", "normalized_title"));
        Assert.True(await ColumnExists("crm", "opportunities", "scope_summary"));
        Assert.True(await ColumnExists("crm", "opportunities", "work_types"));
        Assert.True(await ColumnExists("crm", "opportunities", "source_code"));
        Assert.True(await ColumnExists("crm", "opportunities", "expected_budget"));
        Assert.True(await ColumnExists("crm", "opportunities", "currency_code"));
        Assert.True(await ColumnExists("crm", "opportunities", "target_decision_date"));
        Assert.True(await ColumnExists("crm", "opportunities", "next_action_at_utc"));
        Assert.True(await ColumnExists("crm", "opportunities", "next_action_note"));
        Assert.True(await ColumnExists("crm", "opportunities", "stage"));
        Assert.True(await ColumnExists("crm", "opportunities", "row_version"));
        Assert.True(await ColumnExists("crm", "opportunities", "created_at_utc"));
        Assert.True(await ColumnExists("crm", "opportunities", "created_by_user_id"));
    }

    [Fact]
    public async Task Insert_DuplicateSiteCodeInSameOrg_ThrowsUniqueViolation()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var customer = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท ไซต์ จำกัด", null, "th",
            new PrimaryContactInput("นาย ช่าง", null, "0812345678", null, "phone"), now);
        _db.Customers.Add(customer);

        var site1 = Site.CreateActive(Guid.NewGuid(), orgId, customer.Id, userId, "บ้านพักอาศัย",
            new SiteAddressInput("123 หมู่ 1", "บางตลาด", "ปากเกร็ด", "นนทบุรี", "11120", "TH"),
            null, null, null, now);
        _db.Sites.Add(site1);
        await _db.SaveChangesAsync();

        var ex = await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await _db.Database.ExecuteSqlRawAsync(
                @"INSERT INTO crm.sites (id, organization_id, customer_id, code, label, normalized_label, address_line1, subdistrict, district, province, postal_code, country_code, status, row_version, created_at_utc, created_by_user_id)
                  VALUES ({0}, {1}, {2}, {3}, 'บ้านหลังที่สอง', 'บ้านหลังที่สอง', '456', 'ต', 'อ', 'จ', '10000', 'TH', 'active', {4}, {5}, {6})",
                Guid.NewGuid(), orgId, customer.Id, site1.Code, Guid.NewGuid(), now, userId);
        });

        var pgEx = ex as PostgresException ?? ex.InnerException as PostgresException;
        Assert.NotNull(pgEx);
        Assert.Equal("23505", pgEx.SqlState);
    }

    [Fact]
    public async Task Insert_SiteWithCrossTenantCustomer_ThrowsForeignKeyViolation()
    {
        var org1 = TestOnlyDataSeeder.TestOrgId;
        var org2 = Guid.NewGuid();
        _db.Organizations.Add(new Organization(org2, "Second Org for Site TEST_ONLY"));
        await _db.SaveChangesAsync();

        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var customerInOrg1 = Customer.CreateDraft(Guid.NewGuid(), org1, userId, "organization", "บริษัท ข้ามองค์กร", null, "th",
            new PrimaryContactInput("นาย สอง", null, "0812345678", null, "phone"), now);
        _db.Customers.Add(customerInOrg1);
        await _db.SaveChangesAsync();

        // Attempt to insert site pointing to customer in Org 1, but with organization_id = Org 2
        var ex = await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await _db.Database.ExecuteSqlRawAsync(
                @"INSERT INTO crm.sites (id, organization_id, customer_id, code, label, normalized_label, address_line1, subdistrict, district, province, postal_code, country_code, status, row_version, created_at_utc, created_by_user_id)
                  VALUES ({0}, {1}, {2}, 'SITE-CROSS-01', 'ไซต์แฮก', 'ไซต์แฮก', '456', 'ต', 'อ', 'จ', '10000', 'TH', 'active', {3}, {4}, {5})",
                Guid.NewGuid(), org2, customerInOrg1.Id, Guid.NewGuid(), now, userId);
        });

        var pgEx = ex as PostgresException ?? ex.InnerException as PostgresException;
        Assert.NotNull(pgEx);
        Assert.Equal("23503", pgEx.SqlState);
    }

    [Fact]
    public async Task Insert_OpportunityWithCrossTenantCustomer_ThrowsForeignKeyViolation()
    {
        var org1 = TestOnlyDataSeeder.TestOrgId;
        var org2 = Guid.NewGuid();
        _db.Organizations.Add(new Organization(org2, "Second Org for Opp TEST_ONLY"));

        var branchInOrg2 = new Branch(Guid.NewGuid(), org2, "BR-02", "สาขา 2", true, DateTimeOffset.UtcNow);
        _db.Branches.Add(branchInOrg2);
        await _db.SaveChangesAsync();

        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var customerInOrg1 = Customer.CreateDraft(Guid.NewGuid(), org1, userId, "organization", "บริษัท ลูกค้า 1", null, "th",
            new PrimaryContactInput("นาย สาม", null, "0812345678", null, "phone"), now);
        _db.Customers.Add(customerInOrg1);
        await _db.SaveChangesAsync();

        // Attempt to insert opportunity pointing to customer in Org 1, but with organization_id = Org 2
        var ex = await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await _db.Database.ExecuteSqlRawAsync(
                @"INSERT INTO crm.opportunities (id, organization_id, branch_id, customer_id, owner_user_id, code, title, normalized_title, work_types, stage, row_version, created_at_utc, created_by_user_id)
                  VALUES ({0}, {1}, {2}, {3}, {4}, 'OPP-CROSS-01', 'งานแฮก', 'งานแฮก', '{{built-in}}', 'draft', {5}, {6}, {7})",
                Guid.NewGuid(), org2, branchInOrg2.Id, customerInOrg1.Id, userId, Guid.NewGuid(), now, userId);
        });

        var pgEx = ex as PostgresException ?? ex.InnerException as PostgresException;
        Assert.NotNull(pgEx);
        Assert.Equal("23503", pgEx.SqlState);
    }

    [Fact]
    public async Task Insert_OpportunityWithCrossTenantBranch_ThrowsForeignKeyViolation()
    {
        var org1 = TestOnlyDataSeeder.TestOrgId;
        var org2 = Guid.NewGuid();
        _db.Organizations.Add(new Organization(org2, "Third Org for Opp TEST_ONLY"));

        var branchInOrg2 = new Branch(Guid.NewGuid(), org2, "BR-03", "สาขา 3", true, DateTimeOffset.UtcNow);
        _db.Branches.Add(branchInOrg2);

        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var customerInOrg1 = Customer.CreateDraft(Guid.NewGuid(), org1, userId, "organization", "บริษัท ลูกค้า สาขาต่าง", null, "th",
            new PrimaryContactInput("นาย สี่", null, "0812345678", null, "phone"), now);
        _db.Customers.Add(customerInOrg1);
        await _db.SaveChangesAsync();

        // Attempt to insert opportunity with organization_id = Org 1, but branch_id in Org 2
        var ex = await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await _db.Database.ExecuteSqlRawAsync(
                @"INSERT INTO crm.opportunities (id, organization_id, branch_id, customer_id, owner_user_id, code, title, normalized_title, work_types, stage, row_version, created_at_utc, created_by_user_id)
                  VALUES ({0}, {1}, {2}, {3}, {4}, 'OPP-CROSS-02', 'งานแฮกสาขา', 'งานแฮกสาขา', '{{built-in}}', 'draft', {5}, {6}, {7})",
                Guid.NewGuid(), org1, branchInOrg2.Id, customerInOrg1.Id, userId, Guid.NewGuid(), now, userId);
        });

        var pgEx = ex as PostgresException ?? ex.InnerException as PostgresException;
        Assert.NotNull(pgEx);
        Assert.Equal("23503", pgEx.SqlState);
    }

    [Fact]
    public async Task Insert_OpportunityWithCrossTenantOrCustomerSite_ThrowsForeignKeyViolation()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var branchId = TestOnlyDataSeeder.TestBranchId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var c1 = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "ลูกค้าคนที่ 1", null, "th",
            new PrimaryContactInput("นาย ก", null, "0812345678", null, "phone"), now);
        var c2 = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "ลูกค้าคนที่ 2", null, "th",
            new PrimaryContactInput("นาย ข", null, "0812345679", null, "phone"), now);
        _db.Customers.AddRange(c1, c2);

        var siteForC2 = Site.CreateActive(Guid.NewGuid(), orgId, c2.Id, userId, "ไซต์ของลูกค้าคนที่ 2",
            new SiteAddressInput("123", "ต", "อ", "จ", "10000", "TH"), null, null, null, now);
        _db.Sites.Add(siteForC2);
        await _db.SaveChangesAsync();

        // Attempt to insert opportunity for Customer 1 pointing to primary_site_id belonging to Customer 2
        var ex = await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await _db.Database.ExecuteSqlRawAsync(
                @"INSERT INTO crm.opportunities (id, organization_id, branch_id, customer_id, primary_site_id, owner_user_id, code, title, normalized_title, work_types, stage, row_version, created_at_utc, created_by_user_id)
                  VALUES ({0}, {1}, {2}, {3}, {4}, {5}, 'OPP-CROSS-SITE', 'งานไซต์ข้ามลูกค้า', 'งานไซต์ข้ามลูกค้า', '{{built-in}}', 'draft', {6}, {7}, {8})",
                Guid.NewGuid(), orgId, branchId, c1.Id, siteForC2.Id, userId, Guid.NewGuid(), now, userId);
        });

        var pgEx = ex as PostgresException ?? ex.InnerException as PostgresException;
        Assert.NotNull(pgEx);
        Assert.Equal("23503", pgEx.SqlState);
    }
}
