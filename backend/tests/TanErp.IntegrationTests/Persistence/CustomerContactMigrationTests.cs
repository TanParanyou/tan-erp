using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

public class CustomerContactMigrationTests : IAsyncLifetime
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
        // Rehearsal rollback to the previous migration
        await _db.Database.MigrateAsync("20260906171716_EnforceRolePermissionOrganizationBoundaries");

        // Verify crm schema table is gone
        var exists = await _db.Database
            .SqlQueryRaw<bool>(@"SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'crm' AND table_name = 'customers') AS ""Value""")
            .FirstAsync();
        Assert.False(exists);

        // Reapply
        await _db.Database.MigrateAsync();

        var reapplyExists = await _db.Database
            .SqlQueryRaw<bool>(@"SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'crm' AND table_name = 'customers') AS ""Value""")
            .FirstAsync();
        Assert.True(reapplyExists);
    }

    [Fact]
    public async Task Insert_DuplicateCustomerCodeInSameOrg_ThrowsUniqueViolation()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var c1 = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท ก", null, "th",
            new PrimaryContactInput("นาย ก", null, "0812345678", null, "phone"), now);
        _db.Customers.Add(c1);
        await _db.SaveChangesAsync();

        // Raw insert duplicate code in same org
        var ex = await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await _db.Database.ExecuteSqlRawAsync(
                @"INSERT INTO crm.customers (id, organization_id, code, customer_type, display_name_th, normalized_display_name, status, preferred_locale, row_version, created_at_utc, created_by_user_id)
                  VALUES ({0}, {1}, {2}, 'organization', 'บริษัท ข', 'บริษัท ข', 'draft', 'th', {3}, {4}, {5})",
                Guid.NewGuid(), orgId, c1.Code, Guid.NewGuid(), now, userId);
        });

        var pgEx = ex as PostgresException ?? ex.InnerException as PostgresException;
        Assert.NotNull(pgEx);
        Assert.Equal("23505", pgEx.SqlState);
    }

    [Fact]
    public async Task Insert_ContactWithCrossTenantCustomer_ThrowsForeignKeyViolation()
    {
        var org1 = TestOnlyDataSeeder.TestOrgId;
        var org2 = Guid.NewGuid();
        _db.Organizations.Add(new Organization(org2, "Second Org TEST_ONLY"));
        await _db.SaveChangesAsync();

        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var c1 = Customer.CreateDraft(Guid.NewGuid(), org1, userId, "organization", "บริษัท ก", null, "th",
            new PrimaryContactInput("นาย ก", null, "0812345678", null, "phone"), now);
        _db.Customers.Add(c1);
        await _db.SaveChangesAsync();

        // Attempt to insert contact pointing to Customer of Org 1, but with OrganizationId of Org 2
        var ex = await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await _db.Database.ExecuteSqlRawAsync(
                @"INSERT INTO crm.customer_contacts (id, customer_id, organization_id, name, normalized_phone, preferred_channel, is_primary, status, created_at_utc, created_by_user_id)
                  VALUES ({0}, {1}, {2}, 'นาย แฮกเกอร์', '0899999999', 'phone', false, 'active', {3}, {4})",
                Guid.NewGuid(), c1.Id, org2, now, userId);
        });

        var pgEx = ex as PostgresException ?? ex.InnerException as PostgresException;
        Assert.NotNull(pgEx);
        Assert.Equal("23503", pgEx.SqlState);
    }

    [Fact]
    public async Task Insert_ContactWithoutPhoneAndEmail_ViolatesCheckConstraint()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var c1 = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท ก", null, "th",
            new PrimaryContactInput("นาย ก", null, "0812345678", null, "phone"), now);
        _db.Customers.Add(c1);
        await _db.SaveChangesAsync();

        var ex = await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await _db.Database.ExecuteSqlRawAsync(
                @"INSERT INTO crm.customer_contacts (id, customer_id, organization_id, name, preferred_channel, is_primary, status, created_at_utc, created_by_user_id)
                  VALUES ({0}, {1}, {2}, 'นาย ไร้เบอร์ไร้อีเมล', 'phone', false, 'active', {3}, {4})",
                Guid.NewGuid(), c1.Id, orgId, now, userId);
        });

        var pgEx = ex as PostgresException ?? ex.InnerException as PostgresException;
        Assert.NotNull(pgEx);
        Assert.Equal("23514", pgEx.SqlState);
    }

    [Fact]
    public async Task Insert_MultipleActivePrimaryContacts_ThrowsUniqueViolation()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var c1 = Customer.CreateDraft(Guid.NewGuid(), orgId, userId, "organization", "บริษัท ก", null, "th",
            new PrimaryContactInput("นาย ก", null, "0812345678", null, "phone"), now);
        _db.Customers.Add(c1);
        await _db.SaveChangesAsync();

        // Try inserting a second active primary contact for the same customer
        var ex = await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await _db.Database.ExecuteSqlRawAsync(
                @"INSERT INTO crm.customer_contacts (id, customer_id, organization_id, name, normalized_phone, preferred_channel, is_primary, status, created_at_utc, created_by_user_id)
                  VALUES ({0}, {1}, {2}, 'นาย คนที่สอง', '0898765432', 'phone', true, 'active', {3}, {4})",
                Guid.NewGuid(), c1.Id, orgId, now, userId);
        });

        var pgEx = ex as PostgresException ?? ex.InnerException as PostgresException;
        Assert.NotNull(pgEx);
        Assert.Equal("23505", pgEx.SqlState);
    }

    [Fact]
    public async Task Insert_DuplicateIdempotencyRecord_ThrowsUniqueViolation()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var now = DateTimeOffset.UtcNow;

        var r1 = new IdempotencyRecord(Guid.NewGuid(), orgId, "customers.create", "hash1", "payload1", "res1", now);
        _db.IdempotencyRecords.Add(r1);
        await _db.SaveChangesAsync();

        var ex = await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            var r2 = new IdempotencyRecord(Guid.NewGuid(), orgId, "customers.create", "hash1", "payload2", "res2", now);
            _db.IdempotencyRecords.Add(r2);
            await _db.SaveChangesAsync();
        });

        var pgEx = ex as PostgresException ?? ex.InnerException as PostgresException;
        Assert.NotNull(pgEx);
        Assert.Equal("23505", pgEx.SqlState);
    }
}
