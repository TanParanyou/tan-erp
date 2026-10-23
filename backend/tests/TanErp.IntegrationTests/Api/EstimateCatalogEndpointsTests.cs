using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TanErp.Api;
using TanErp.Api.Contracts.Estimates;
using TanErp.Api.Contracts.Items;
using TanErp.Application.Estimates;
using TanErp.Application.Items;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Estimates;
using TanErp.Domain.Items;
using TanErp.Domain.Files;
using TanErp.Domain.Organization;
using TanErp.Domain.Surveys;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using TanErp.IntegrationTests.Support;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class EstimateCatalogEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private readonly InMemoryLoggerProvider _loggerProvider = new();

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
            builder.ConfigureLogging(logging => logging.AddProvider(_loggerProvider));
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
        var solarImage = new UploadedFile(Guid.NewGuid(), OrgId, "test/solar-category-image", "solar-category.png", "image/png", 128, "test-solar-category-session", actorId, now);
        var jinkoImage = new UploadedFile(Guid.NewGuid(), OrgId, "test/jinko-brand-image", "jinko-brand.png", "image/png", 128, "test-jinko-brand-session", actorId, now);
        db.UploadedFiles.AddRange(solarImage, jinkoImage);
        var catSolar = new ItemCategory(Guid.NewGuid(), OrgId, "SOLAR", LocalizedText.Create("โซลาร์เซลล์", "Solar Cell"), null, null, [ItemType.Material], 1, actorId, now, solarImage.Id);
        var catMount = new ItemCategory(Guid.NewGuid(), OrgId, "MOUNT", LocalizedText.Create("โครงสร้างยึด", "Mounting Structure"), null, null, [ItemType.Material], 2, actorId, now);
        db.ItemCategories.AddRange(catSolar, catMount);

        // 2. Units
        var unitPiece = new UnitOfMeasure(Guid.NewGuid(), OrgId, "PCS", LocalizedText.Create("ชิ้น", "Piece"), "pcs", "count", 0, "half_up", actorId, now);
        var unitSet = new UnitOfMeasure(Guid.NewGuid(), OrgId, "SET", LocalizedText.Create("ชุด", "Set"), "set", "count", 0, "half_up", actorId, now);
        db.Units.AddRange(unitPiece, unitSet);

        // 3. Brands
        var brandJinko = new ItemBrand(Guid.NewGuid(), OrgId, "JINKO", LocalizedText.Create("จินโกะ", "Jinko Solar"), null, 1, actorId, now, jinkoImage.Id);
        db.ItemBrands.Add(brandJinko);

        // 4. Item 1: Active, AllBranches, with Published Cost
        var item1 = Item.CreateDraft(
            Guid.NewGuid(), OrgId, "SOLAR-PANEL-550W", ItemType.Material, catSolar.Id, brandJinko.Id,
            LocalizedText.Create("แผงโซลาร์ 550W", "Solar Panel 550W"), null, unitPiece.Id,
            ItemAvailabilityMode.AllBranches, new ItemCapabilities(true, true, true, true, false),
            new Dictionary<string, string> { ["fixture"] = "item_catalog_filter_test_only", ["thickness_mm"] = "10" }, 1, actorId, now);
        item1.Activate(actorId, now, false, true, true, true);
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
        item2.Activate(actorId, now, true, true, true, true);
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
        item4.Activate(actorId, now, false, true, true, true);
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

    [Fact]
    public async Task SearchCatalog_FilterByHasCost_PushesPredicateToDatabase_DoesNotReturnShortPage()
    {
        // Add 3 unpriced items with codes that sort alphabetically BEFORE and BETWEEN the priced items
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var cat = await db.ItemCategories.FirstAsync(c => c.OrganizationId == OrgId);
            var unit = await db.Units.FirstAsync(u => u.OrganizationId == OrgId);
            var now = DateTimeOffset.UtcNow;
            var actorId = Guid.NewGuid();

            var unpriced1 = Item.CreateDraft(Guid.NewGuid(), OrgId, "AAA-NO-COST", ItemType.Material, cat.Id, null, LocalizedText.Create("ของไม่มีราคา 1", null), null, unit.Id, ItemAvailabilityMode.AllBranches, new ItemCapabilities(true, true, true, true, false), null, null, actorId, now);
            unpriced1.Activate(actorId, now, false, true, true, true);
            var unpriced2 = Item.CreateDraft(Guid.NewGuid(), OrgId, "BBB-NO-COST", ItemType.Material, cat.Id, null, LocalizedText.Create("ของไม่มีราคา 2", null), null, unit.Id, ItemAvailabilityMode.AllBranches, new ItemCapabilities(true, true, true, true, false), null, null, actorId, now);
            unpriced2.Activate(actorId, now, false, true, true, true);
            var unpriced3 = Item.CreateDraft(Guid.NewGuid(), OrgId, "CCC-NO-COST", ItemType.Material, cat.Id, null, LocalizedText.Create("ของไม่มีราคา 3", null), null, unit.Id, ItemAvailabilityMode.AllBranches, new ItemCapabilities(true, true, true, true, false), null, null, actorId, now);
            unpriced3.Activate(actorId, now, false, true, true, true);

            db.Items.AddRange(unpriced1, unpriced2, unpriced3);
            await db.SaveChangesAsync();
        }

        // Query with hasCost=true and pageSize=2
        // If in-memory filter is used on pageSize+1 (3 items: AAA, BBB, CCC), they have no cost so result would be EMPTY 0 items!
        // But with database predicate pushdown, it must return the 2 priced items ("MOUNT-ROOF-01", "SOLAR-PANEL-550W")
        var req = CreateRequest($"/api/v1/estimate-catalog/items?branchId={BranchAId}&hasCost=true&pageSize=2");
        var res = await _client.SendAsync(req);

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var body = await res.Content.ReadFromJsonAsync<EstimateCatalogResponse>();
        Assert.NotNull(body);
        Assert.Equal(2, body.Items.Count);
        Assert.All(body.Items, i => Assert.NotNull(i.ResolvedCost));

        var defaultRequest = CreateRequest($"/api/v1/estimate-catalog/items?branchId={BranchAId}&pageSize=2");
        var defaultResponse = await _client.SendAsync(defaultRequest);
        var defaultBody = await defaultResponse.Content.ReadFromJsonAsync<EstimateCatalogResponse>();
        Assert.NotNull(defaultBody);
        Assert.Equal(2, defaultBody.Items.Count);
        Assert.All(defaultBody.Items, i => Assert.NotNull(i.ResolvedCost));
    }

    [Fact]
    public async Task SearchCatalog_ProductItem_MapsToMaterialCostComponent()
    {
        Guid productId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var unit = await db.Units.FirstAsync(u => u.OrganizationId == OrgId);
            var now = DateTimeOffset.UtcNow;
            var actorId = Guid.NewGuid();
            var category = new ItemCategory(Guid.NewGuid(), OrgId, "TEST-PRODUCT-CATALOG",
                LocalizedText.Create("สินค้าสำเร็จรูปทดสอบ", "Test Finished Product"), null, null,
                [ItemType.Product], 10, actorId, now);
            var product = Item.CreateDraft(Guid.NewGuid(), OrgId, "TEST-PRODUCT-CATALOG-01", ItemType.Product,
                category.Id, null, LocalizedText.Create("ชุดสินค้าสำเร็จรูป", "Finished Product Set"), null,
                unit.Id, ItemAvailabilityMode.AllBranches, new ItemCapabilities(true, true, true, true, false),
                null, null, actorId, now);
            product.Activate(actorId, now, false, true, true, true);
            db.ItemCategories.Add(category);
            db.Items.Add(product);

            var cost = CostRecord.CreateDraft(Guid.NewGuid(), OrgId, product.Id, CostScopeType.Organization,
                null, unit.Id, "THB", 1250m, 0m, null, now.AddDays(-1), null, 1, null, null, null, null,
                actorId, now);
            cost.Submit(actorId, now);
            cost.Approve(Guid.NewGuid(), now);
            cost.Publish(Guid.NewGuid(), now);
            db.CostRecords.Add(cost);
            await db.SaveChangesAsync();
            productId = product.Id;
        }

        var request = CreateRequest($"/api/v1/estimate-catalog/items?branchId={BranchAId}&hasCost=true&itemType=product&pageSize=20");
        var response = await _client.SendAsync(request);

        Assert.True(response.StatusCode == HttpStatusCode.OK,
            $"Expected 200 but received {(int)response.StatusCode}. {string.Join(Environment.NewLine, _loggerProvider.Messages)}");
        var body = await response.Content.ReadFromJsonAsync<EstimateCatalogResponse>();
        Assert.NotNull(body);
        var item = Assert.Single(body.Items);
        Assert.Equal(productId, item.Id);
        Assert.Equal(ItemType.Product, item.ItemType);
        Assert.Equal(CostComponentType.Material, item.CostComponentType);
    }

    [Fact]
    public async Task SearchCatalog_FacetsRemainStableWhenFiltersOrSearchChange()
    {
        Guid solarCategoryId;
        Guid brandId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            solarCategoryId = await db.ItemCategories.Where(c => c.OrganizationId == OrgId && c.Code == "SOLAR")
                .Select(c => c.Id).SingleAsync();
            brandId = await db.ItemBrands.Where(b => b.OrganizationId == OrgId && b.Code == "JINKO")
                .Select(b => b.Id).SingleAsync();
        }

        var baselineResponse = await _client.SendAsync(CreateRequest($"/api/v1/estimate-catalog/items?branchId={BranchAId}"));
        Assert.Equal(HttpStatusCode.OK, baselineResponse.StatusCode);
        var baseline = await baselineResponse.Content.ReadFromJsonAsync<EstimateCatalogResponse>();
        Assert.NotNull(baseline);

        var request = CreateRequest($"/api/v1/estimate-catalog/items?branchId={BranchAId}&categoryId={solarCategoryId}&brandId={brandId}&itemType=material&attributeKey=thickness_mm&attributeValue=10&search=SOLAR&pageSize=1");
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<EstimateCatalogResponse>();
        Assert.NotNull(body);
        Assert.Single(body.Items);
        Assert.Equal("SOLAR-PANEL-550W", body.Items[0].Code);
        Assert.Equal(JsonSerializer.Serialize(baseline.Facets), JsonSerializer.Serialize(body.Facets));
        var categoryFacet = body.Facets.Categories.Single(facet => facet.Id == solarCategoryId);
        var brandFacet = body.Facets.Brands.Single(facet => facet.Id == brandId);
        Assert.NotNull(categoryFacet.ImageFileId);
        Assert.NotNull(brandFacet.ImageFileId);
        var solarItem = body.Items.Single();
        Assert.Equal(categoryFacet.ImageFileId, solarItem.Category.ImageFileId);
        Assert.Equal(brandFacet.ImageFileId, solarItem.Brand?.ImageFileId);

        var emptyResponse = await _client.SendAsync(CreateRequest($"/api/v1/estimate-catalog/items?branchId={BranchAId}&search=NO-MATCH-TEST-ONLY"));
        Assert.Equal(HttpStatusCode.OK, emptyResponse.StatusCode);
        var empty = await emptyResponse.Content.ReadFromJsonAsync<EstimateCatalogResponse>();
        Assert.NotNull(empty);
        Assert.Empty(empty.Items);
        Assert.Equal(JsonSerializer.Serialize(baseline.Facets), JsonSerializer.Serialize(empty.Facets));
    }

    [Fact]
    public async Task SearchCatalog_FiltersByAttributeAndReturnsAttributeFacets()
    {
        var request = CreateRequest($"/api/v1/estimate-catalog/items?branchId={BranchAId}&attributeKey=thickness_mm&attributeValue=10");
        var response = await _client.SendAsync(request);

        Assert.True(response.StatusCode == HttpStatusCode.OK,
            $"Expected 200 but received {(int)response.StatusCode}. {string.Join(Environment.NewLine, _loggerProvider.Messages)}");
        var body = await response.Content.ReadFromJsonAsync<EstimateCatalogResponse>();
        Assert.NotNull(body);
        var item = Assert.Single(body.Items);
        Assert.Equal("SOLAR-PANEL-550W", item.Code);
        Assert.Equal("10", item.Attributes.RootElement.GetProperty("thickness_mm").GetString());
        Assert.Contains(body.Facets.Attributes, facet => facet.Key == "thickness_mm" && facet.Value == "10" && facet.Count == 1);

        var keyOnlyRequest = CreateRequest($"/api/v1/estimate-catalog/items?branchId={BranchAId}&attributeKey=fixture");
        var keyOnlyResponse = await _client.SendAsync(keyOnlyRequest);
        Assert.Equal(HttpStatusCode.OK, keyOnlyResponse.StatusCode);
        var keyOnlyBody = await keyOnlyResponse.Content.ReadFromJsonAsync<EstimateCatalogResponse>();
        Assert.NotNull(keyOnlyBody);
        Assert.Contains(keyOnlyBody.Items, result => result.Code == "SOLAR-PANEL-550W");
    }

    [Fact]
    public async Task SearchCatalog_AmbiguousCost_DoesNotOccupyPricedPageOrFacets()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var category = await db.ItemCategories.FirstAsync(c => c.OrganizationId == OrgId);
            var unit = await db.Units.FirstAsync(u => u.OrganizationId == OrgId);
            var now = DateTimeOffset.UtcNow;
            var actorId = Guid.NewGuid();
            var ambiguousItem = Item.CreateDraft(
                Guid.NewGuid(), OrgId, "AAA-AMBIGUOUS", ItemType.Material, category.Id, null,
                LocalizedText.Create("ราคากำกวม", "Ambiguous Cost"), null, unit.Id,
                ItemAvailabilityMode.AllBranches, new ItemCapabilities(true, true, true, true, false),
                null, null, actorId, now);
            ambiguousItem.Activate(actorId, now, false, true, true, true);
            db.Items.Add(ambiguousItem);

            foreach (var amount in new[] { 100m, 120m })
            {
                var cost = CostRecord.CreateDraft(
                    Guid.NewGuid(), OrgId, ambiguousItem.Id, CostScopeType.Organization, null, unit.Id, "THB",
                    amount, 0m, null, now.AddDays(-1), null, 1, null, null, null, null, actorId, now);
                cost.Submit(actorId, now);
                cost.Approve(Guid.NewGuid(), now);
                cost.Publish(Guid.NewGuid(), now);
                db.CostRecords.Add(cost);
            }

            await db.SaveChangesAsync();
        }

        var request = CreateRequest($"/api/v1/estimate-catalog/items?branchId={BranchAId}&pageSize=1");
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<EstimateCatalogResponse>();
        Assert.NotNull(body);
        Assert.Single(body.Items);
        Assert.NotEqual("AAA-AMBIGUOUS", body.Items[0].Code);
        Assert.NotNull(body.Items[0].ResolvedCost);
        Assert.Equal(2, body.Facets.ItemTypes.Single(f => f.Value == ItemType.Material).Count);
    }

    [Fact]
    public async Task SearchCatalog_TamperedCursor_ReturnsBadRequestWithCatalogCursorInvalid()
    {
        // Tampered JSON with empty code and empty Guid
        var tamperedJson = "{\"Code\":\"\",\"Id\":\"00000000-0000-0000-0000-000000000000\"}";
        var tamperedCursor = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(tamperedJson));

        var req = CreateRequest($"/api/v1/estimate-catalog/items?branchId={BranchAId}&cursor={tamperedCursor}");
        var res = await _client.SendAsync(req);

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task EstimateUnitConversion_ResolverOptionsAndFrozenSnapshot_WorkEndToEnd()
    {
        Guid solarItemId;
        Guid unitPieceId;
        Guid unitBoxId;
        Guid unitMeterId;
        Guid sharedBoxConvId;
        Guid itemBoxConvId;
        Guid estimateId;
        Guid revisionId;
        Guid revisionVersion;

        var now = DateTimeOffset.UtcNow;
        var today = DateOnly.FromDateTime(now.UtcDateTime);
        var actorId = TestOnlyDataSeeder.TestUserId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var solarItem = await db.Items.SingleAsync(i => i.OrganizationId == OrgId && i.Code == "SOLAR-PANEL-550W");
            solarItemId = solarItem.Id;
            unitPieceId = solarItem.BaseUnitId;

            var unitBox = new UnitOfMeasure(Guid.NewGuid(), OrgId, "BOX", LocalizedText.Create("กล่อง", "Box"), "box", "count", 0, "half_up", actorId, now);
            var unitPallet = new UnitOfMeasure(Guid.NewGuid(), OrgId, "PALLET", LocalizedText.Create("พาเลท", "Pallet"), "pallet", "count", 0, "half_up", actorId, now);
            var unitMeter = new UnitOfMeasure(Guid.NewGuid(), OrgId, "M", LocalizedText.Create("เมตร", "Meter"), "m", "length", 2, "half_up", actorId, now);
            unitBoxId = unitBox.Id;
            unitMeterId = unitMeter.Id;
            db.Units.AddRange(unitBox, unitPallet, unitMeter);

            // Shared conversion BOX -> PCS = 10, PALLET -> PCS = 30; Item-specific conversion BOX -> PCS = 12 (overrides shared BOX -> PCS)
            var sharedBox = new UnitConversion(Guid.NewGuid(), OrgId, unitBox.Id, unitPieceId, 10m, today.AddDays(-5), null, "มาตรฐานกล่องรวม", actorId, now);
            var sharedPallet = new UnitConversion(Guid.NewGuid(), OrgId, unitPallet.Id, unitPieceId, 30m, today.AddDays(-5), null, "มาตรฐานพาเลท", actorId, now);
            var itemBox = new ItemUnitConversion(Guid.NewGuid(), OrgId, solarItemId, unitBox.Id, unitPieceId, 12m, today.AddDays(-5), null, "กล่องเฉพาะแผง 550W บรรจุ 12 แผง", actorId, now);
            sharedBoxConvId = sharedBox.Id;
            itemBoxConvId = itemBox.Id;
            db.UnitConversions.AddRange(sharedBox, sharedPallet);
            db.ItemUnitConversions.Add(itemBox);
            await db.SaveChangesAsync();

            var resolver = scope.ServiceProvider.GetRequiredService<IUnitConversionResolver>();

            // 1. Item-specific conversion overrides shared conversion for BOX -> PCS
            var resolvedBox = await resolver.ResolveToBaseUnitAsync(OrgId, solarItemId, "BOX", today, CancellationToken.None);
            Assert.True(resolvedBox.IsSuccess);
            Assert.Equal((itemBoxConvId, UnitConversionScopes.Item, "BOX", "PCS", 12m),
                (resolvedBox.Value!.ConversionId, resolvedBox.Value.Scope, resolvedBox.Value.FromUnitCode, resolvedBox.Value.ToUnitCode, resolvedBox.Value.Factor));

            // 2. Shared conversion is used when no item-specific override exists (PALLET -> PCS)
            var resolvedPallet = await resolver.ResolveToBaseUnitAsync(OrgId, solarItemId, "PALLET", today, CancellationToken.None);
            Assert.True(resolvedPallet.IsSuccess);
            Assert.Equal((sharedPallet.Id, UnitConversionScopes.Shared, 30m),
                (resolvedPallet.Value!.ConversionId, resolvedPallet.Value.Scope, resolvedPallet.Value.Factor));

            // 3. Dimension mismatch returns ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH
            var mismatch = await resolver.ResolveToBaseUnitAsync(OrgId, solarItemId, "M", today, CancellationToken.None);
            Assert.True(mismatch.IsFailure);
            Assert.Equal("ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH", mismatch.Error.Code);

            // 4. Missing active conversion returns ESTIMATE_UNIT_CONVERSION_NOT_FOUND
            var notFound = await resolver.ResolveToBaseUnitAsync(OrgId, solarItemId, "SET", today, CancellationToken.None);
            Assert.True(notFound.IsFailure);
            Assert.Equal("ESTIMATE_UNIT_CONVERSION_NOT_FOUND", notFound.Error.Code);
        }

        // 5. GET /api/v1/estimate-catalog/items/{itemId}/units with policy mode = 'none' (default) returns ConversionsEnabled=false & base unit only
        var unitsNoneRes = await _client.SendAsync(CreateRequest($"/api/v1/estimate-catalog/items/{solarItemId}/units?branchId={BranchAId}"));
        Assert.Equal(HttpStatusCode.OK, unitsNoneRes.StatusCode);
        var unitsNone = (await unitsNoneRes.Content.ReadFromJsonAsync<EstimateUnitOptionsResponse>())!;
        Assert.False(unitsNone.ConversionsEnabled);
        Assert.Equal("PCS", unitsNone.BaseUnitCode);
        Assert.Single(unitsNone.Units);

        // Publish a branch CalculationPolicyVersion for BranchA with UnitConversionMode = 'snapshot'
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var existingPolicies = await db.CalculationPolicyVersions.Where(p => p.OrganizationId == OrgId && p.BranchId == BranchAId).ToListAsync();
            db.CalculationPolicyVersions.RemoveRange(existingPolicies);
            var snapshotPolicy = new CalculationPolicyVersion(
                Guid.NewGuid(), OrgId, BranchAId, "CALC-SNAP-B01", 1, "none", 0m, "away-from-zero",
                now.AddDays(-1), null, actorId, UnitConversionModes.Snapshot);
            snapshotPolicy.Publish(Guid.NewGuid(), now);
            db.CalculationPolicyVersions.Add(snapshotPolicy);

            // Seed a customer, site, opportunity (in Estimating) and Ready survey revision so we can create & update an Official Estimate draft
            var customer = Customer.CreateDraft(
                Guid.NewGuid(), OrgId, actorId, CustomerType.Person, "ลูกค้าทดสอบ G08", null, "th",
                new PrimaryContactInput("คุณสมชาย G08", null, "0812345678", null, "phone"), now);
            customer.Activate(customer.RowVersion);
            var siteAddress = new SiteAddressInput("123 ถนนสุขุมวิท", "คลองเตย", "คลองเตย", "กรุงเทพมหานคร", "10110", "TH");
            var site = Site.CreateActive(Guid.NewGuid(), OrgId, customer.Id, actorId, "ไซต์ G08", siteAddress, 13.7m, 100.5m, null, now);
            var opp = Opportunity.CreateDraft(
                Guid.NewGuid(), OrgId, BranchAId, customer.Id, site.Id, actorId, actorId,
                "โครงการทดสอบ G08", "งานโซลาร์เซลล์", new[] { "built-in" }, null, 500000m, "THB",
                new DateOnly(2026, 12, 31), now.AddDays(2), "เตรียมประเมินราคา", now);
            opp.Qualify(opp.RowVersion);
            opp.EnterSurveying(opp.RowVersion, site.Id);
            opp.EnterEstimating(opp.RowVersion);
            var survey = SiteSurvey.CreateAppointment(OrgId, BranchAId, opp.Id, site.Id, actorId, actorId, now, now.AddHours(2), now);
            var surveyRev = SiteSurveyRevision.CreateBaseline(OrgId, survey.Id, actorId, now);
            var area = new SiteSurveyArea(Guid.NewGuid(), OrgId, surveyRev.Id, "AREA-01", "หลังคา", null, 1);
            area.AddMeasurement(new SiteSurveyMeasurement(Guid.NewGuid(), OrgId, area.Id, "width", 10.0m, "m", "measured", null, 1));
            surveyRev.AddArea(area);
            surveyRev.UpdateDraft(now, "สำรวจหลังคา", null, null, null);
            surveyRev.MarkReady(actorId, now, "aaaa1111bbbb2222cccc3333dddd4444eeee5555ffff6666aaaa1111bbbb2222");
            db.Customers.Add(customer);
            db.Sites.Add(site);
            db.Opportunities.Add(opp);
            db.SiteSurveys.Add(survey);
            db.SiteSurveyRevisions.Add(surveyRev);
            await db.SaveChangesAsync();

            var estimateStore = scope.ServiceProvider.GetRequiredService<IEstimateStore>();
            var draftRes = await estimateStore.CreateDraftAsync(OrgId, opp.Id, surveyRev.Id, "THB", actorId, "g08-create-key", "g08-create-hash", CancellationToken.None);
            Assert.True(draftRes.IsSuccess);
            estimateId = draftRes.Value!.Id;
            revisionId = draftRes.Value.CurrentRevision!.Id;
            revisionVersion = draftRes.Value.CurrentRevision.RowVersion;
        }

        // 6. Now GET /api/v1/estimate-catalog/items/{itemId}/units returns ConversionsEnabled=true with PCS, BOX (item factor 12), and PALLET (shared factor 30)
        var unitsSnapRes = await _client.SendAsync(CreateRequest($"/api/v1/estimate-catalog/items/{solarItemId}/units?branchId={BranchAId}"));
        Assert.Equal(HttpStatusCode.OK, unitsSnapRes.StatusCode);
        var unitsSnap = (await unitsSnapRes.Content.ReadFromJsonAsync<EstimateUnitOptionsResponse>())!;
        Assert.True(unitsSnap.ConversionsEnabled);
        Assert.Equal(3, unitsSnap.Units.Count);
        var boxOption = unitsSnap.Units.Single(u => u.UnitCode == "BOX");
        Assert.Equal(itemBoxConvId, boxOption.ConversionId);
        Assert.Equal(UnitConversionScopes.Item, boxOption.ConversionScope);
        Assert.Equal("12", boxOption.Factor);

        // 7. Save estimate draft with a cost component in BOX (Quantity = 2 BOX = 24 PCS @ 3,200 THB/PCS = 76,800 THB)
        Guid savedComponentId;
        using (var scope = _factory.Services.CreateScope())
        {
            var estimateStore = scope.ServiceProvider.GetRequiredService<IEstimateStore>();
            var sections = new[]
            {
                new EstimateSectionDraftDto(
                    null, "SEC-01", "งานแผงโซลาร์", "Solar Array", 1,
                    new[]
                    {
                        new EstimateWorkItemDraftDto(
                            null, "WI-01", "ติดตั้งแผงโซลาร์", "Install Solar Panels", 1m, "SET", SellingRuleType.Margin, 0.20m, 1,
                            new[]
                            {
                                new EstimateCostComponentDraftDto(
                                    null, CostComponentType.Material, "แผงโซลาร์ 550W (ยกกล่อง)", 2m, "BOX", 3200m, "THB", 1,
                                    ItemId: solarItemId, ProvisionalReasonCode: "LEGACY_IMPORT")
                            },
                            ItemId: solarItemId)
                    })
            };

            var updated = await estimateStore.UpdateDraftAsync(OrgId, estimateId, revisionId, revisionVersion, sections, actorId, CancellationToken.None);
            revisionVersion = updated.RowVersion;
            var comp = Assert.Single(updated.Sections.Single().WorkItems.Single().CostComponents);
            savedComponentId = comp.Id;
            Assert.NotNull(comp.ConversionSnapshot);
            Assert.Equal((itemBoxConvId, UnitConversionScopes.Item, "BOX", "PCS", 12m),
                (comp.ConversionSnapshot!.ConversionId, comp.ConversionSnapshot.Scope, comp.ConversionSnapshot.FromUnit, comp.ConversionSnapshot.ToUnit, comp.ConversionSnapshot.Factor));
            Assert.Equal(38400m, comp.UnitCost); // 12 * 3,200
            Assert.Equal(76800m, comp.TotalCost); // 2 * 12 * 3,200
        }

        // 8. Replace the item conversion in DB with a new factor (15 PCS per BOX) and re-save the draft echoing the original Component Id & ConversionId:
        //    the frozen conversion snapshot (factor 12) must remain untouched!
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var oldConv = await db.ItemUnitConversions.SingleAsync(c => c.Id == itemBoxConvId);
            db.ItemUnitConversions.Remove(oldConv);
            db.ItemUnitConversions.Add(new ItemUnitConversion(Guid.NewGuid(), OrgId, solarItemId, unitBoxId, unitPieceId, 15m, today.AddDays(-1), null, "เปลี่ยนขนาดกล่องใหม่ 15 แผง", actorId, now));
            await db.SaveChangesAsync();

            var estimateStore = scope.ServiceProvider.GetRequiredService<IEstimateStore>();
            var sectionsEcho = new[]
            {
                new EstimateSectionDraftDto(
                    null, "SEC-01", "งานแผงโซลาร์ (อัปเดตชื่อ)", "Solar Array Updated", 1,
                    new[]
                    {
                        new EstimateWorkItemDraftDto(
                            null, "WI-01", "ติดตั้งแผงโซลาร์", "Install Solar Panels", 1m, "SET", SellingRuleType.Margin, 0.20m, 1,
                            new[]
                            {
                                new EstimateCostComponentDraftDto(
                                    savedComponentId, CostComponentType.Material, "แผงโซลาร์ 550W (ยกกล่อง)", 2m, "BOX", 38400m, "THB", 1,
                                    ItemId: solarItemId, ProvisionalReasonCode: "LEGACY_IMPORT", ConversionId: itemBoxConvId)
                            },
                            ItemId: solarItemId)
                    })
            };

            var reSaved = await estimateStore.UpdateDraftAsync(OrgId, estimateId, revisionId, revisionVersion, sectionsEcho, actorId, CancellationToken.None);
            var compAfter = Assert.Single(reSaved.Sections.Single().WorkItems.Single().CostComponents);
            Assert.NotNull(compAfter.ConversionSnapshot);
            Assert.Equal(itemBoxConvId, compAfter.ConversionSnapshot!.ConversionId);
            Assert.Equal(12m, compAfter.ConversionSnapshot.Factor);
            Assert.Equal(76800m, compAfter.TotalCost);
        }
    }
}
