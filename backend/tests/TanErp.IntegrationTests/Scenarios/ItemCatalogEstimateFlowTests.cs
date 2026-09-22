using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.Estimates;
using TanErp.Api.Contracts.Items;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Estimates;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;
using TanErp.Domain.Surveys;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Scenarios;

public class ItemCatalogEstimateFlowTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private static readonly Guid OrgId = TestOnlyDataSeeder.TestOrgId;
    private static readonly Guid BranchAId = TestOnlyDataSeeder.TestBranchId;
    private static readonly Guid MembershipId = TestOnlyDataSeeder.TestMembershipId;
    private static readonly Guid MakerUserId = TestOnlyDataSeeder.TestUserId;
    private static readonly Guid CheckerUserId = Guid.NewGuid();
    private const string Uid = TestOnlyDataSeeder.TestFirebaseUid;

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
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
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
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private HttpRequestMessage CreateAuthenticatedRequest(
        HttpMethod method,
        string url,
        string token = "test-token",
        Guid? membershipId = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Membership-Id", (membershipId ?? MembershipId).ToString());
        return request;
    }

    [Fact]
    public async Task CompleteE2EFlow_ItemMaster_CostMakerChecker_BranchResolution_EstimateSnapshot_And_ConflictRejection()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTimeOffset.UtcNow;

        // 0. Setup Branch B for OrgId
        var branchB = new Branch(Guid.NewGuid(), OrgId, "B02", "สาขาขอนแก่น");
        db.Branches.Add(branchB);
        var branchBId = branchB.Id;

        // 1. Setup Taxonomy: Category & Unit
        var category = new ItemCategory(
            Guid.NewGuid(), OrgId, "E2E-CAT-01",
            LocalizedText.Create("หมวดหมู่งานไม้ E2E", "E2E Wood Category"),
            null, null, [ItemType.Material], 1, MakerUserId, now);
        var unit = new UnitOfMeasure(
            Guid.NewGuid(), OrgId, "SHEET",
            LocalizedText.Create("แผ่น", "Sheet"),
            "sheet", "area", 0, "half_up", MakerUserId, now);
        db.ItemCategories.Add(category);
        db.Units.Add(unit);

        // 2. Setup Item Master & Activate
        var item = Item.CreateDraft(
            Guid.NewGuid(), OrgId, "WOOD-HMR-E2E", ItemType.Material, category.Id, null,
            LocalizedText.Create("ไม้ HMR 18 มม. E2E", "HMR Board 18mm E2E"),
            LocalizedText.Create("ไม้อัดทนชื้นเกรดพรีเมียม", "Premium moisture resistant board"),
            unit.Id, ItemAvailabilityMode.AllBranches,
            new ItemCapabilities(CanSell: true, CanCost: true, CanPurchase: true, CanStock: true, CanProduce: false),
            null, null, MakerUserId, now);
        item.Activate(MakerUserId, now, hasActiveSelectedBranch: false);
        db.Items.Add(item);

        // 3. Attach Primary Image
        var primaryImageFileId = Guid.NewGuid();
        var uploadedFile = new TanErp.Domain.Files.UploadedFile(
            primaryImageFileId,
            OrgId,
            $"items/{item.Id}/{primaryImageFileId}.jpg",
            "hmr-board.jpg",
            "image/jpeg",
            204800,
            $"session-{Guid.NewGuid():N}",
            MakerUserId,
            now,
            scanStatus: "clean",
            verifiedAtUtc: now,
            width: 800,
            height: 600);
        db.UploadedFiles.Add(uploadedFile);

        var image = new ItemImage(
            Guid.NewGuid(), OrgId, item.Id, primaryImageFileId,
            ItemImageRole.Primary, true, 1,
            LocalizedText.Create("รูปภาพด้านหน้า", "Front view"),
            null, MakerUserId, now);
        db.ItemImages.Add(image);

        // 4. Create Org Standard Cost (1000 THB) - Maker Submit & Checker Approve/Publish
        var orgCost = CostRecord.CreateDraft(
            Guid.NewGuid(), OrgId, item.Id, CostScopeType.Organization, null, unit.Id, "THB",
            1000m, 0m, null, now.AddDays(-1), null, 1, null, null, null, null, MakerUserId, now);
        orgCost.Submit(MakerUserId, now);
        orgCost.Approve(CheckerUserId, now);
        orgCost.Publish(CheckerUserId, now);
        db.CostRecords.Add(orgCost);

        // 5. Create Branch B Specific Cost (1200 THB) - Maker Submit & Checker Approve/Publish
        var branchCost = CostRecord.CreateDraft(
            Guid.NewGuid(), OrgId, item.Id, CostScopeType.Branch, branchBId, unit.Id, "THB",
            1200m, 0m, null, now.AddDays(-1), null, 1, null, null, null, null, MakerUserId, now);
        branchCost.Submit(MakerUserId, now);
        branchCost.Approve(CheckerUserId, now);
        branchCost.Publish(CheckerUserId, now);
        db.CostRecords.Add(branchCost);

        // 6. Setup Opportunity & Ready Site Survey for Branch B
        var customer = Customer.CreateDraft(
            Guid.NewGuid(), OrgId, MakerUserId, CustomerType.Person, "คุณลูกค้า สาขา B", null, "th",
            new PrimaryContactInput("คุณสมพร", null, "0891112233", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        db.Customers.Add(customer);

        var address = new SiteAddressInput("99/9 ถ.มิตรภาพ", "ในเมือง", "เมือง", "ขอนแก่น", "40000", "TH");
        var site = Site.CreateActive(Guid.NewGuid(), OrgId, customer.Id, MakerUserId, "บ้านพักขอนแก่น", address, 16.4m, 102.8m, null, now);
        db.Sites.Add(site);

        var opp = Opportunity.CreateDraft(
            Guid.NewGuid(), OrgId, branchBId, customer.Id, site.Id, MakerUserId, MakerUserId,
            "งานบิวท์อิน สาขาขอนแก่น", "ขอบเขตตู้เสื้อผ้า", ["built-in"], null, 300000m, "THB",
            new DateOnly(2026, 12, 31), now.AddDays(2), "เตรียมประเมินราคา", now);
        opp.Qualify(opp.RowVersion);
        opp.EnterSurveying(opp.RowVersion, site.Id);
        opp.EnterEstimating(opp.RowVersion);
        db.Opportunities.Add(opp);

        var survey = SiteSurvey.CreateAppointment(OrgId, branchBId, opp.Id, site.Id, MakerUserId, MakerUserId, now, now.AddHours(2), now);
        var revision = SiteSurveyRevision.CreateBaseline(OrgId, survey.Id, MakerUserId, now);
        var area = new SiteSurveyArea(Guid.NewGuid(), OrgId, revision.Id, "AREA-01", "ห้องแต่งตัว", null, 1);
        area.AddMeasurement(new SiteSurveyMeasurement(Guid.NewGuid(), OrgId, area.Id, "width", 4.0m, "m", "measured", null, 1));
        revision.AddArea(area);
        revision.UpdateDraft(now, "สำรวจพื้นที่พร้อมประเมิน", null, null, null);
        var snapshotHash = "hash-ready-survey-branch-b";
        revision.MarkReady(MakerUserId, now, snapshotHash);

        db.SiteSurveys.Add(survey);
        db.SiteSurveyRevisions.Add(revision);
        await db.SaveChangesAsync();

        // 7. Verify Authoritative Catalog Endpoint Precedence:
        // Branch A gets Org Cost (1000 THB), Branch B gets Branch Cost (1200 THB)
        var catalogReqA = CreateAuthenticatedRequest(HttpMethod.Get, $"/api/v1/estimate-catalog/items?branchId={BranchAId}&search=WOOD-HMR-E2E");
        var catalogResA = await _client.SendAsync(catalogReqA);
        Assert.Equal(HttpStatusCode.OK, catalogResA.StatusCode);
        var catalogDataA = await catalogResA.Content.ReadFromJsonAsync<EstimateCatalogResponse>();
        Assert.NotNull(catalogDataA);
        var catalogItemA = Assert.Single(catalogDataA.Items);
        Assert.Equal(1000m, catalogItemA.ResolvedCost?.Amount);
        Assert.Equal(CostScopeType.Organization, catalogItemA.ResolvedCost?.Scope);
        Assert.Equal(primaryImageFileId, catalogItemA.PrimaryImage?.FileId);

        var catalogReqB = CreateAuthenticatedRequest(HttpMethod.Get, $"/api/v1/estimate-catalog/items?branchId={branchBId}&search=WOOD-HMR-E2E");
        var catalogResB = await _client.SendAsync(catalogReqB);
        Assert.Equal(HttpStatusCode.OK, catalogResB.StatusCode);
        var catalogDataB = await catalogResB.Content.ReadFromJsonAsync<EstimateCatalogResponse>();
        Assert.NotNull(catalogDataB);
        var catalogItemB = Assert.Single(catalogDataB.Items);
        Assert.Equal(1200m, catalogItemB.ResolvedCost?.Amount);
        Assert.Equal(CostScopeType.Branch, catalogItemB.ResolvedCost?.Scope);
        Assert.Equal(primaryImageFileId, catalogItemB.PrimaryImage?.FileId);

        // 8. Create Estimate Draft from Opportunity in Branch B
        var createEstReq = CreateAuthenticatedRequest(HttpMethod.Post, "/api/v1/estimates");
        createEstReq.Headers.Add("Idempotency-Key", $"idemp-e2e-estimate-{Guid.NewGuid():N}");
        createEstReq.Content = JsonContent.Create(new CreateEstimateDraftRequest(opp.Id, revision.Id, Currency: "THB"));
        var createEstRes = await _client.SendAsync(createEstReq);
        Assert.Equal(HttpStatusCode.Created, createEstRes.StatusCode);
        var estimate = (await createEstRes.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        Assert.Equal(branchBId, estimate.BranchId);

        // 9. Update Draft with Work Item containing the Catalog Item with Branch B Cost (1200 THB, Version 1)
        var currentRev = estimate.CurrentRevision!;
        var updateReq = CreateAuthenticatedRequest(HttpMethod.Put, $"/api/v1/estimates/{estimate.Id}/revisions/{currentRev.Id}/draft");
        updateReq.Headers.Add("If-Match", $"\"{currentRev.RowVersion}\"");
        updateReq.Content = JsonContent.Create(new UpdateEstimateDraftRequest(
            ExpectedRevisionVersion: currentRev.RowVersion,
            Sections: new[]
            {
                new UpdateEstimateSectionDto(
                    Id: null,
                    Code: "SEC-01",
                    NameTh: "งานโครงสร้างตู้เสื้อผ้า",
                    NameEn: "Wardrobe Structure",
                    SortOrder: 1,
                    WorkItems: new[]
                    {
                        new UpdateEstimateWorkItemDto(
                            Id: null,
                            Code: "WI-WARDROBE-01",
                            DescriptionTh: "ตู้เสื้อผ้าไม้ HMR 18 มม.",
                            DescriptionEn: "HMR Wardrobe 18mm",
                            Quantity: 1m,
                            UnitCode: "set",
                            SellingRuleType: "margin",
                            SellingRuleValue: 0.20m,
                            SortOrder: 1,
                            CostComponents: new[]
                            {
                                new UpdateEstimateCostComponentDto(
                                    Id: null,
                                    Type: "material",
                                    Description: "ไม้ HMR 18 มม. E2E",
                                    Quantity: 5m,
                                    UnitCode: "SHEET",
                                    UnitCost: 1200m,
                                    Currency: "THB",
                                    SortOrder: 1,
                                    ItemId: item.Id,
                                    CostRecordId: branchCost.Id,
                                    CostRecordVersion: 1)
                            })
                    })
            }));

        var updateRes = await _client.SendAsync(updateReq);
        Assert.Equal(HttpStatusCode.OK, updateRes.StatusCode);
        var updatedRevision = (await updateRes.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;

        // Verify Snapshots in Response & Database
        var costComp = updatedRevision.Sections[0].WorkItems[0].CostComponents[0];
        Assert.Equal(item.Id, costComp.ItemId);
        Assert.Equal(branchCost.Id, costComp.CostRecordId);
        Assert.Equal(1, costComp.CostRecordVersion);
        Assert.Equal("WOOD-HMR-E2E", costComp.ItemCodeSnapshot);
        Assert.Equal("ไม้ HMR 18 มม. E2E", costComp.ItemNameSnapshot?.Thai);
        Assert.Equal("SHEET", costComp.UnitSnapshot);
        Assert.Equal(1200m, costComp.UnitCostSnapshot);
        Assert.Equal(CostScopeType.Branch, costComp.CostScopeSnapshot);
        Assert.NotNull(costComp.ResolvedAtUtc);

        // 10. Simulate Price Change: Branch B Cost Record published as Version 2 with new price (1350 THB)
        using var changeScope = _factory.Services.CreateScope();
        var changeDb = changeScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var newCost = CostRecord.CreateDraft(
            Guid.NewGuid(), OrgId, item.Id, CostScopeType.Branch, branchBId, unit.Id, "THB",
            1350m, 0m, null, now, null, 2, null, null, null, null, MakerUserId, now);
        newCost.Submit(MakerUserId, now);
        newCost.Approve(CheckerUserId, now);
        newCost.Publish(CheckerUserId, now);
        changeDb.CostRecords.Add(newCost);
        await changeDb.SaveChangesAsync();

        // 11. Attempt to Update Draft using Stale Price / Stale Version (1200 THB, Version 1)
        // Expected: 409 Conflict with ITEM_COST_VERSION_CONFLICT
        var staleReq = CreateAuthenticatedRequest(HttpMethod.Put, $"/api/v1/estimates/{estimate.Id}/revisions/{currentRev.Id}/draft");
        staleReq.Headers.Add("If-Match", $"\"{updatedRevision.RowVersion}\"");
        staleReq.Content = JsonContent.Create(new UpdateEstimateDraftRequest(
            ExpectedRevisionVersion: updatedRevision.RowVersion,
            Sections: new[]
            {
                new UpdateEstimateSectionDto(
                    Id: null,
                    Code: "SEC-01",
                    NameTh: "งานโครงสร้างตู้เสื้อผ้า",
                    NameEn: "Wardrobe Structure",
                    SortOrder: 1,
                    WorkItems: new[]
                    {
                        new UpdateEstimateWorkItemDto(
                            Id: null,
                            Code: "WI-WARDROBE-01",
                            DescriptionTh: "ตู้เสื้อผ้าไม้ HMR 18 มม.",
                            DescriptionEn: "HMR Wardrobe 18mm",
                            Quantity: 1m,
                            UnitCode: "set",
                            SellingRuleType: "margin",
                            SellingRuleValue: 0.20m,
                            SortOrder: 1,
                            CostComponents: new[]
                            {
                                new UpdateEstimateCostComponentDto(
                                    Id: null,
                                    Type: "material",
                                    Description: "ไม้ HMR 18 มม. E2E",
                                    Quantity: 5m,
                                    UnitCode: "SHEET",
                                    UnitCost: 1200m, // Stale price!
                                    Currency: "THB",
                                    SortOrder: 1,
                                    ItemId: item.Id,
                                    CostRecordId: branchCost.Id, // Stale cost record!
                                    CostRecordVersion: 1) // Stale version!
                            })
                    })
            }));

        var staleRes = await _client.SendAsync(staleReq);
        Assert.Equal(HttpStatusCode.Conflict, staleRes.StatusCode);
        var problemDetails = await staleRes.Content.ReadAsStringAsync();
        Assert.Contains("ITEM_COST_VERSION_CONFLICT", problemDetails);

        // 12. Update Draft using Authoritative Active Price (1350 THB, Version 2)
        // Expected: 200 OK with updated Version 2 snapshots
        var freshReq = CreateAuthenticatedRequest(HttpMethod.Put, $"/api/v1/estimates/{estimate.Id}/revisions/{currentRev.Id}/draft");
        freshReq.Headers.Add("If-Match", $"\"{updatedRevision.RowVersion}\"");
        freshReq.Content = JsonContent.Create(new UpdateEstimateDraftRequest(
            ExpectedRevisionVersion: updatedRevision.RowVersion,
            Sections: new[]
            {
                new UpdateEstimateSectionDto(
                    Id: null,
                    Code: "SEC-01",
                    NameTh: "งานโครงสร้างตู้เสื้อผ้า",
                    NameEn: "Wardrobe Structure",
                    SortOrder: 1,
                    WorkItems: new[]
                    {
                        new UpdateEstimateWorkItemDto(
                            Id: null,
                            Code: "WI-WARDROBE-01",
                            DescriptionTh: "ตู้เสื้อผ้าไม้ HMR 18 มม.",
                            DescriptionEn: "HMR Wardrobe 18mm",
                            Quantity: 1m,
                            UnitCode: "set",
                            SellingRuleType: "margin",
                            SellingRuleValue: 0.20m,
                            SortOrder: 1,
                            CostComponents: new[]
                            {
                                new UpdateEstimateCostComponentDto(
                                    Id: null,
                                    Type: "material",
                                    Description: "ไม้ HMR 18 มม. E2E",
                                    Quantity: 5m,
                                    UnitCode: "SHEET",
                                    UnitCost: 1350m, // Fresh price
                                    Currency: "THB",
                                    SortOrder: 1,
                                    ItemId: item.Id,
                                    CostRecordId: newCost.Id,
                                    CostRecordVersion: 2)
                            })
                    })
            }));

        var freshRes = await _client.SendAsync(freshReq);
        Assert.Equal(HttpStatusCode.OK, freshRes.StatusCode);
        var freshRevision = (await freshRes.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;
        var freshCostComp = freshRevision.Sections[0].WorkItems[0].CostComponents[0];
        Assert.Equal(newCost.Id, freshCostComp.CostRecordId);
        Assert.Equal(2, freshCostComp.CostRecordVersion);
        Assert.Equal(1350m, freshCostComp.UnitCostSnapshot);
        Assert.Equal(1350m, freshCostComp.UnitCost);
        Assert.Equal(6750m, freshCostComp.TotalCost);
    }
}
