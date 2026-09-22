using Microsoft.EntityFrameworkCore;
using TanErp.Application.Items;
using TanErp.Domain.Items;
using TanErp.Infrastructure.Persistence;
using TanErp.Infrastructure.Persistence.Items;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

public class CostResolverTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private AppDbContext _db = null!;
    private CostResolver _resolver = null!;

    private readonly Guid _orgId = TestOnlyDataSeeder.TestOrgId;
    private readonly Guid _branchA = TestOnlyDataSeeder.TestBranchId;
    private readonly Guid _branchB = TestOnlyDataSeeder.TestBranchBId;
    private readonly Guid _actorId = Guid.NewGuid();

    private Guid _itemId;
    private Guid _unitId;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        _db = new AppDbContext(options);
        await _db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(_db, "Test", true);

        _resolver = new CostResolver(_db);

        // Seed Category and Unit
        var now = DateTimeOffset.UtcNow;
        var cat = new ItemCategory(
            Guid.NewGuid(),
            _orgId,
            "CAT-COST-01",
            LocalizedText.Create("ทดสอบต้นทุน", "Cost Test"),
            null,
            null,
            [ItemType.Material],
            0,
            _actorId,
            now);
        _db.ItemCategories.Add(cat);

        var unit = new UnitOfMeasure(
            Guid.NewGuid(),
            _orgId,
            "PCS",
            LocalizedText.Create("ชิ้น", "Piece"),
            "pcs",
            "count",
            0,
            "half_up",
            _actorId,
            now);
        _db.Units.Add(unit);

        var item = Item.CreateDraft(
            Guid.NewGuid(),
            _orgId,
            "COST-ITEM-01",
            ItemType.Material,
            cat.Id,
            null,
            LocalizedText.Create("สินค้าทดสอบต้นทุน", "Cost Item"),
            null,
            unit.Id,
            ItemAvailabilityMode.AllBranches,
            new ItemCapabilities(false, true, true, true, false),
            null,
            null,
            _actorId,
            DateTimeOffset.UtcNow);
        _db.Items.Add(item);

        await _db.SaveChangesAsync();

        _itemId = item.Id;
        _unitId = unit.Id;
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task Resolve_BranchScopedRecord_TakesPrecedenceOverOrganizationDefault()
    {
        var now = DateTimeOffset.UtcNow;

        // Org default: 100 THB
        var orgCost = CostRecord.CreateDraft(
            Guid.NewGuid(), _orgId, _itemId, CostScopeType.Organization, null, _unitId, "THB",
            100m, 0m, null, now.AddDays(-10), null, 1, null, null, null, null, _actorId, now);
        orgCost.Submit(_actorId, now);
        orgCost.Approve(Guid.NewGuid(), now);
        orgCost.Publish(Guid.NewGuid(), now);

        // Branch A override: 120 THB
        var branchCost = CostRecord.CreateDraft(
            Guid.NewGuid(), _orgId, _itemId, CostScopeType.Branch, _branchA, _unitId, "THB",
            120m, 0m, null, now.AddDays(-10), null, 2, null, null, null, null, _actorId, now);
        branchCost.Submit(_actorId, now);
        branchCost.Approve(Guid.NewGuid(), now);
        branchCost.Publish(Guid.NewGuid(), now);

        _db.CostRecords.AddRange(orgCost, branchCost);
        await _db.SaveChangesAsync();

        // Query for Branch A should return 120 THB
        var reqA = new ResolveCostRequest(_orgId, _branchA, _itemId, _unitId, "THB", 1, now);
        var resA = await _resolver.ResolveAsync(reqA, CancellationToken.None);
        Assert.True(resA.IsSuccess);
        Assert.Equal(120m, resA.Value!.Amount);
        Assert.Equal(CostScopeType.Branch, resA.Value.Scope);
        Assert.Equal(_branchA, resA.Value.BranchId);

        // Query for Branch B (no override) should return 100 THB
        var reqB = new ResolveCostRequest(_orgId, _branchB, _itemId, _unitId, "THB", 1, now);
        var resB = await _resolver.ResolveAsync(reqB, CancellationToken.None);
        Assert.True(resB.IsSuccess);
        Assert.Equal(100m, resB.Value!.Amount);
        Assert.Equal(CostScopeType.Organization, resB.Value.Scope);
    }

    [Fact]
    public async Task Resolve_HigherSourcePriority_TakesPrecedence()
    {
        var now = DateTimeOffset.UtcNow;

        var sourceLow = new CostSource(Guid.NewGuid(), _orgId, "SRC_LOW", LocalizedText.Create("ต่ำ", "Low"), priority: 10, isActive: true, now);
        var sourceHigh = new CostSource(Guid.NewGuid(), _orgId, "SRC_HIGH", LocalizedText.Create("สูง", "High"), priority: 50, isActive: true, now);
        _db.CostSources.AddRange(sourceLow, sourceHigh);

        var costLow = CostRecord.CreateDraft(
            Guid.NewGuid(), _orgId, _itemId, CostScopeType.Organization, null, _unitId, "THB",
            150m, 0m, null, now.AddDays(-5), null, 1, sourceLow.Id, null, null, null, _actorId, now);
        costLow.Submit(_actorId, now);
        costLow.Approve(Guid.NewGuid(), now);
        costLow.Publish(Guid.NewGuid(), now);

        var costHigh = CostRecord.CreateDraft(
            Guid.NewGuid(), _orgId, _itemId, CostScopeType.Organization, null, _unitId, "THB",
            175m, 0m, null, now.AddDays(-5), null, 2, sourceHigh.Id, null, null, null, _actorId, now);
        costHigh.Submit(_actorId, now);
        costHigh.Approve(Guid.NewGuid(), now);
        costHigh.Publish(Guid.NewGuid(), now);

        _db.CostRecords.AddRange(costLow, costHigh);
        await _db.SaveChangesAsync();

        var req = new ResolveCostRequest(_orgId, _branchA, _itemId, _unitId, "THB", 1, now);
        var res = await _resolver.ResolveAsync(req, CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.Equal(175m, res.Value!.Amount);
        Assert.Equal("SRC_HIGH", res.Value.CostSourceCode);
    }

    [Fact]
    public async Task Resolve_LatestEffectiveFrom_TakesPrecedence()
    {
        var now = DateTimeOffset.UtcNow;

        var olderCost = CostRecord.CreateDraft(
            Guid.NewGuid(), _orgId, _itemId, CostScopeType.Organization, null, _unitId, "THB",
            200m, 0m, null, now.AddDays(-30), null, 1, null, null, null, null, _actorId, now);
        olderCost.Submit(_actorId, now);
        olderCost.Approve(Guid.NewGuid(), now);
        olderCost.Publish(Guid.NewGuid(), now);

        var newerCost = CostRecord.CreateDraft(
            Guid.NewGuid(), _orgId, _itemId, CostScopeType.Organization, null, _unitId, "THB",
            250m, 0m, null, now.AddDays(-2), null, 2, null, null, null, null, _actorId, now);
        newerCost.Submit(_actorId, now);
        newerCost.Approve(Guid.NewGuid(), now);
        newerCost.Publish(Guid.NewGuid(), now);

        _db.CostRecords.AddRange(olderCost, newerCost);
        await _db.SaveChangesAsync();

        var req = new ResolveCostRequest(_orgId, _branchA, _itemId, _unitId, "THB", 1, now);
        var res = await _resolver.ResolveAsync(req, CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.Equal(250m, res.Value!.Amount);
    }

    [Fact]
    public async Task Resolve_AmbiguousIdenticalPrecedenceDifferentAmount_ReturnsAmbiguousError()
    {
        var now = DateTimeOffset.UtcNow;
        var fixedDate = now.AddDays(-10);

        // Two published records with same scope, same effective from date, but different amounts
        var cost1 = CostRecord.CreateDraft(
            Guid.NewGuid(), _orgId, _itemId, CostScopeType.Organization, null, _unitId, "THB",
            300m, 0m, null, fixedDate, null, 1, null, null, null, null, _actorId, now);
        cost1.Submit(_actorId, now);
        cost1.Approve(Guid.NewGuid(), now);
        cost1.Publish(Guid.NewGuid(), now);

        var cost2 = CostRecord.CreateDraft(
            Guid.NewGuid(), _orgId, _itemId, CostScopeType.Organization, null, _unitId, "THB",
            350m, 0m, null, fixedDate, null, 1, null, null, null, null, _actorId, now);
        cost2.Submit(_actorId, now);
        cost2.Approve(Guid.NewGuid(), now);
        cost2.Publish(Guid.NewGuid(), now);

        _db.CostRecords.AddRange(cost1, cost2);
        await _db.SaveChangesAsync();

        var req = new ResolveCostRequest(_orgId, _branchA, _itemId, _unitId, "THB", 1, now);
        var res = await _resolver.ResolveAsync(req, CancellationToken.None);

        Assert.False(res.IsSuccess);
        Assert.Equal("ITEM_COST_AMBIGUOUS", res.Error.Code);
    }

    [Fact]
    public async Task Resolve_NoMatchingPublishedCost_ReturnsNotFoundError()
    {
        var now = DateTimeOffset.UtcNow;
        var req = new ResolveCostRequest(_orgId, _branchA, _itemId, _unitId, "THB", 1, now);
        var res = await _resolver.ResolveAsync(req, CancellationToken.None);

        Assert.False(res.IsSuccess);
        Assert.Equal("ITEM_COST_NOT_FOUND", res.Error.Code);
    }

    [Fact]
    public async Task Resolve_DraftOrSubmittedRecords_AreIgnored()
    {
        var now = DateTimeOffset.UtcNow;

        var draft = CostRecord.CreateDraft(
            Guid.NewGuid(), _orgId, _itemId, CostScopeType.Organization, null, _unitId, "THB",
            100m, 0m, null, now.AddDays(-5), null, 1, null, null, null, null, _actorId, now);

        var submitted = CostRecord.CreateDraft(
            Guid.NewGuid(), _orgId, _itemId, CostScopeType.Organization, null, _unitId, "THB",
            150m, 0m, null, now.AddDays(-5), null, 2, null, null, null, null, _actorId, now);
        submitted.Submit(_actorId, now);

        _db.CostRecords.AddRange(draft, submitted);
        await _db.SaveChangesAsync();

        var req = new ResolveCostRequest(_orgId, _branchA, _itemId, _unitId, "THB", 1, now);
        var res = await _resolver.ResolveAsync(req, CancellationToken.None);

        Assert.False(res.IsSuccess);
        Assert.Equal("ITEM_COST_NOT_FOUND", res.Error.Code);
    }
}
