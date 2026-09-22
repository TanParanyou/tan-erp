using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.Items;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class EstimateCatalogEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private const string Uid = TestOnlyDataSeeder.TestFirebaseUid;
    private static readonly Guid OrgId = TestOnlyDataSeeder.TestOrgId;
    private static readonly Guid BranchAId = TestOnlyDataSeeder.TestBranchId;
    private static readonly Guid BranchBId = Guid.NewGuid();
    private static readonly Guid MembershipId = TestOnlyDataSeeder.TestMembershipId;

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<string?>(idToken == "test-token" ? Uid : null);
        }
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Database"] = _postgres.GetConnectionString(),
                    ["SeedTestData"] = "true"
                });
            });
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFirebaseTokenVerifier));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                services.AddSingleton<IFirebaseTokenVerifier, TestFirebaseTokenVerifier>();
            });
        });

        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true);

        await SeedCatalogTestDataAsync(db);
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private HttpRequestMessage CreateRequest(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "test-token");
        request.Headers.Add("X-Membership-Id", MembershipId.ToString());
        return request;
    }

    private static async Task SeedCatalogTestDataAsync(AppDbContext db)
    {
        var now = DateTimeOffset.UtcNow;
        var actorId = Guid.NewGuid();

        // 0. Branch B in Org
        var branchB = new Branch(BranchBId, OrgId, "B02", "สาขา 2", true, now);
        db.Branches.Add(branchB);

        // 1. Categories
        var catSolar = new ItemCategory(Guid.NewGuid(), OrgId, "SOLAR", LocalizedText.Create("โซลาร์เซลล์", "Solar Cell"), null, null, [ItemType.Material], 1, actorId, now);
        var catMount = new ItemCategory(Guid.NewGuid(), OrgId, "MOUNT", LocalizedText.Create("โครงสร้างยึด", "Mounting Structure"), null, null, [ItemType.Material], 2, actorId, now);
        db.ItemCategories.AddRange(catSolar, catMount);

        // 2. Units
        var unitPiece = new UnitOfMeasure(Guid.NewGuid(), OrgId, "PCS", LocalizedText.Create("ชิ้น", "Piece"), "pcs", "count", 0, "half_up", actorId, now);
        var unitSet = new UnitOfMeasure(Guid.NewGuid(), OrgId, "SET", LocalizedText.Create("ชุด", "Set"), "set", "count", 0, "half_up", actorId, now);
        db.Units.AddRange(unitPiece, unitSet);

        // 3. Brands
        var brandJinko = new ItemBrand(Guid.NewGuid(), OrgId, "JINKO", LocalizedText.Create("จินโกะ", "Jinko Solar"), null, 1, actorId, now);
        db.ItemBrands.Add(brandJinko);

        // 4. Item 1: Active, AllBranches, with Published Cost
        var item1 = Item.CreateDraft(
            Guid.NewGuid(), OrgId, "SOLAR-PANEL-550W", ItemType.Material, catSolar.Id, brandJinko.Id,
            LocalizedText.Create("แผงโซลาร์ 550W", "Solar Panel 550W"), null, unitPiece.Id,
            ItemAvailabilityMode.AllBranches, new ItemCapabilities(true, true, true, true, false),
            null, null, actorId, now);
        item1.Activate(actorId, now, false);
        db.Items.Add(item1);

        var cost1 = CostRecord.CreateDraft(
            Guid.NewGuid(), OrgId, item1.Id, CostScopeType.Organization, null, unitPiece.Id, "THB",
            3200m, 0m, null, now.AddDays(-10), null, 1, null, null, null, null, actorId, now);
        cost1.Submit(actorId, now);
        cost1.Approve(Guid.NewGuid(), now);
        cost1.Publish(Guid.NewGuid(), now);
        db.CostRecords.Add(cost1);

        // 5. Item 2: Active, SelectedBranches (Branch A only), with Published Cost
        var item2 = Item.CreateDraft(
            Guid.NewGuid(), OrgId, "MOUNT-ROOF-01", ItemType.Material, catMount.Id, null,
            LocalizedText.Create("ขายึดหลังคาซีแพค", "CPAC Roof Hook"), null, unitSet.Id,
            ItemAvailabilityMode.SelectedBranches, new ItemCapabilities(true, true, true, true, false),
            null, null, actorId, now);
        var branchAvail = new ItemBranchAvailability(Guid.NewGuid(), OrgId, item2.Id, BranchAId, null, null, actorId, now);
        db.ItemBranchAvailabilities.Add(branchAvail);
        item2.Activate(actorId, now, true);
        db.Items.Add(item2);

        var cost2 = CostRecord.CreateDraft(
            Guid.NewGuid(), OrgId, item2.Id, CostScopeType.Branch, BranchAId, unitSet.Id, "THB",
            450m, 0m, null, now.AddDays(-10), null, 1, null, null, null, null, actorId, now);
        cost2.Submit(actorId, now);
        cost2.Approve(Guid.NewGuid(), now);
        cost2.Publish(Guid.NewGuid(), now);
        db.CostRecords.Add(cost2);

        // 6. Item 3: Draft (should NOT appear in catalog)
        var item3 = Item.CreateDraft(
            Guid.NewGuid(), OrgId, "DRAFT-ITEM", ItemType.Material, catSolar.Id, null,
            LocalizedText.Create("สินค้าดราฟต์", "Draft Item"), null, unitPiece.Id,
            ItemAvailabilityMode.AllBranches, new ItemCapabilities(true, true, true, true, false),
            null, null, actorId, now);
        db.Items.Add(item3);

        // 7. Item 4: CanCost = false (should NOT appear in catalog)
        var item4 = Item.CreateDraft(
            Guid.NewGuid(), OrgId, "NO-COST-ITEM", ItemType.Material, catSolar.Id, null,
            LocalizedText.Create("สินค้าไม่มีต้นทุน", "No Cost Item"), null, unitPiece.Id,
            ItemAvailabilityMode.AllBranches, new ItemCapabilities(true, false, true, true, false),
            null, null, actorId, now);
        item4.Activate(actorId, now, false);
        db.Items.Add(item4);

        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task SearchCatalog_BranchA_ReturnsAvailableItemsAndCosts()
    {
        var req = CreateRequest($"/api/v1/estimate-catalog/items?branchId={BranchAId}");
        var res = await _client.SendAsync(req);

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var body = await res.Content.ReadFromJsonAsync<EstimateCatalogResponse>();
        Assert.NotNull(body);

        // Item 1 (all branches) and Item 2 (Branch A) should be returned
        Assert.Equal(2, body.Items.Count);
        Assert.Contains(body.Items, i => i.Code == "SOLAR-PANEL-550W");
        Assert.Contains(body.Items, i => i.Code == "MOUNT-ROOF-01");

        // Verify resolved cost is present
        var solar = body.Items.First(i => i.Code == "SOLAR-PANEL-550W");
        Assert.NotNull(solar.ResolvedCost);
        Assert.Equal(3200m, solar.ResolvedCost.Amount);

        // Check facets
        Assert.NotNull(body.Facets);
        Assert.NotEmpty(body.Facets.Categories);
    }

    [Fact]
    public async Task SearchCatalog_BranchB_ExcludesBranchAOnlyItem()
    {
        var req = CreateRequest($"/api/v1/estimate-catalog/items?branchId={BranchBId}");
        var res = await _client.SendAsync(req);

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var body = await res.Content.ReadFromJsonAsync<EstimateCatalogResponse>();
        Assert.NotNull(body);

        // Only Item 1 (all branches) should be visible in Branch B; Item 2 (Branch A only) must NOT appear
        Assert.Single(body.Items);
        Assert.Equal("SOLAR-PANEL-550W", body.Items[0].Code);
    }

    [Fact]
    public async Task SearchCatalog_InvalidCursor_ReturnsBadRequest()
    {
        var req = CreateRequest($"/api/v1/estimate-catalog/items?branchId={BranchAId}&cursor=invalid-non-base64!!");
        var res = await _client.SendAsync(req);

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task SearchCatalog_FilterBySearchQuery_MatchesCodeOrName()
    {
        var req = CreateRequest($"/api/v1/estimate-catalog/items?branchId={BranchAId}&search={Uri.EscapeDataString("ซีแพค")}");
        var res = await _client.SendAsync(req);

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var body = await res.Content.ReadFromJsonAsync<EstimateCatalogResponse>();
        Assert.NotNull(body);
        Assert.Single(body.Items);
        Assert.Equal("MOUNT-ROOF-01", body.Items[0].Code);
    }

    [Fact]
    public async Task SearchCatalog_CrossOrgBranch_ReturnsNotFound()
    {
        var crossOrgBranchId = TestOnlyDataSeeder.TestBranchBId; // Belongs to Org B!
        var req = CreateRequest($"/api/v1/estimate-catalog/items?branchId={crossOrgBranchId}");
        var res = await _client.SendAsync(req);

        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task SearchCatalog_InactiveBranch_ReturnsUnprocessableEntity()
    {
        var inactiveBranchId = Guid.NewGuid();
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var inactiveBranch = new Branch(inactiveBranchId, OrgId, "INACT", "Inactive Branch", false, DateTimeOffset.UtcNow);
            db.Branches.Add(inactiveBranch);
            await db.SaveChangesAsync();
        }

        var req = CreateRequest($"/api/v1/estimate-catalog/items?branchId={inactiveBranchId}");
        var res = await _client.SendAsync(req);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, res.StatusCode);
    }
}
