using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Models;
using TanErp.Application.Crm.Opportunities;
using TanErp.Application.Crm.Opportunities.QualifyOpportunity;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Infrastructure.Persistence;
using TanErp.Infrastructure.Persistence.Crm;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

public class OpportunityQualificationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private AppDbContext _db = null!;
    private OpportunityStore _store = null!;
    private static readonly DateTimeOffset FixedTime = DateTimeOffset.Parse("2026-09-10T10:00:00Z");

    private sealed class FixedClock : TanErp.Application.Common.Abstractions.IClock
    {
        public DateTimeOffset UtcNow { get; } = FixedTime;
    }

    private static TanErp.Application.Files.IFileStore CreateFileStore(AppDbContext db, TanErp.Application.Common.Abstractions.IClock clock)
    {
        var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder().Build();
        var storage = new TanErp.Infrastructure.Files.LocalFileStorageProvider(config);
        var resolver = new TanErp.Infrastructure.Files.FileParentAccessResolver(db, clock);
        return new TanErp.Infrastructure.Files.FileStore(db, storage, resolver);
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

        var clock = new FixedClock();
        _store = new OpportunityStore(_db, clock, CreateFileStore(_db, clock));
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task Qualify_WritesOpportunityHistoryAuditAndIdempotencyAtomically()
    {
        var orgId = TestOnlyDataSeeder.TestOrgId;
        var branchId = TestOnlyDataSeeder.TestBranchId;
        var userId = TestOnlyDataSeeder.TestUserId;
        var membershipId = TestOnlyDataSeeder.TestMembershipId;

        // 1. Create and seed active Customer
        var customer = Customer.CreateDraft(
            Guid.NewGuid(), orgId, userId, "organization", "ลูกค้าตัวอย่าง TEST_ONLY",
            null, "th", new PrimaryContactInput("ลูกค้าตัวอย่าง TEST_ONLY", null, "0812345678", null, "phone"), FixedTime);
        customer.Activate(customer.RowVersion);
        _db.Customers.Add(customer);

        // 2. Create Opportunity Draft satisfying Q gate
        var opp = Opportunity.CreateDraft(
            Guid.NewGuid(), orgId, branchId, customer.Id, null, userId, userId,
            "งานบิวต์อิน TEST_ONLY",
            "สำรวจและประเมินงาน",
            new[] { "built-in" },
            null, 500000m, "THB",
            new DateOnly(2026, 12, 1),
            FixedTime.AddDays(1),
            "นัดประเมินแบบ",
            FixedTime);
        _db.Opportunities.Add(opp);
        await _db.SaveChangesAsync();

        var initialVersion = opp.RowVersion;

        // 3. Command & hashes
        var command = new QualifyOpportunityCommand(
            "test-firebase-uid",
            membershipId,
            opp.Id,
            "qualified",
            initialVersion,
            "idem-qualify-test-12345",
            "trace-qualify-001");

        var keyHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(command.IdempotencyKey))).ToLowerInvariant();
        var canonicalPayload = $"{opp.Id:D}|qualified|{initialVersion:D}";
        var payloadHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonicalPayload))).ToLowerInvariant();

        var access = new RequestAccessContext(
            userId,
            membershipId,
            orgId,
            branchId,
            "opportunities.transition",
            "organization");

        // 4. Execute QualifyAsync
        var result = await _store.QualifyAsync(access, command, keyHash, payloadHash);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("qualified", result.Value!.Stage);
        Assert.NotEqual(initialVersion, result.Value.RowVersion);

        // 5. Verify database state
        _db.ChangeTracker.Clear();
        var reloadedOpp = await _db.Opportunities.FindAsync(opp.Id);
        Assert.NotNull(reloadedOpp);
        Assert.Equal("qualified", reloadedOpp.Stage);
        Assert.Equal(result.Value.RowVersion, reloadedOpp.RowVersion);

        // 6. Verify stage history
        var history = await _db.OpportunityStageHistories
            .Where(h => h.OpportunityId == opp.Id)
            .ToListAsync();
        Assert.Single(history);
        var entry = history[0];
        Assert.Equal(orgId, entry.OrganizationId);
        Assert.Equal(opp.Id, entry.OpportunityId);
        Assert.Equal("draft", entry.FromStage);
        Assert.Equal("qualified", entry.ToStage);
        Assert.Null(entry.ReasonCode);
        Assert.Null(entry.Note);
        Assert.Equal(userId, entry.ActorUserId);
        Assert.Equal(FixedTime, entry.OccurredAtUtc);
        Assert.Equal("opportunity-stage-v1", entry.PolicyVersion);
        Assert.Equal("trace-qualify-001", entry.TraceId);

        // 7. Verify audit event
        var audit = await _db.AuditEvents
            .Where(a => a.ResourceId == opp.Id.ToString() && a.Action == "opportunity.stage-changed")
            .ToListAsync();
        Assert.Single(audit);
        using (var doc = System.Text.Json.JsonDocument.Parse(audit[0].ChangesJson))
        {
            var root = doc.RootElement;
            Assert.Equal("draft", root.GetProperty("fromStage").GetString());
            Assert.Equal("qualified", root.GetProperty("toStage").GetString());
            Assert.Equal("stage", root.GetProperty("changedFields")[0].GetString());
        }
        Assert.DoesNotContain("งานบิวต์อิน", audit[0].ChangesJson);
        Assert.DoesNotContain("สำรวจและประเมินงาน", audit[0].ChangesJson);
        Assert.DoesNotContain("นัดประเมินแบบ", audit[0].ChangesJson);

        // 8. Verify idempotency record
        var idem = await _db.IdempotencyRecords
            .Where(r => r.OrganizationId == orgId && r.Operation == "opportunities.qualify" && r.KeyHash == keyHash)
            .ToListAsync();
        Assert.Single(idem);
        Assert.Equal(opp.Id.ToString(), idem[0].ResourceId);
    }
}
