using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.Inventory;
using TanErp.Api.Contracts.Procurement;
using TanErp.Api.Contracts.Production;
using TanErp.Api.Contracts.Projects;
using TanErp.Api.ErrorHandling;
using TanErp.Domain.Commercial;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Estimates;
using TanErp.Domain.Items;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Domain.Procurement;
using TanErp.Domain.Projects;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class ProductionEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private static readonly Guid OrgId = TestOnlyDataSeeder.TestOrgId;
    private static readonly Guid BranchId = TestOnlyDataSeeder.TestBranchId;
    private static readonly Guid UserId = TestOnlyDataSeeder.TestUserId;
    private const string UidA = TestOnlyDataSeeder.TestFirebaseUid;
    private const string UidB = TestOnlyDataSeeder.TestFirebaseUidB;
    private const string UidApprover = "uid-project-approver";
    private static readonly Guid ApproverUserId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b70");
    private static readonly Guid ApproverMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b71");
    private const string UidNoPerm = "uid-no-inventory-perm";
    private static readonly Guid MembershipNoPermId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b63");

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(idToken switch
            {
                "token-org-a" => UidA,
                "token-org-b" => UidB,
                "token-no-perm" => UidNoPerm,
                "token-approver" => UidApprover,
                _ => null
            });
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
                if (descriptor != null) services.Remove(descriptor);
                services.AddSingleton<IFirebaseTokenVerifier, TestFirebaseTokenVerifier>();
            });
        });

        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true, seedItemCatalogDemoData: true);

        var noPermUser = new TanErp.Domain.IdentityAccess.User(Guid.NewGuid(), UidNoPerm, "noperm@example.com", "No Perm", true);
        db.Users.Add(noPermUser);
        db.Memberships.Add(new TanErp.Domain.Organization.Membership(MembershipNoPermId, OrgId, BranchId, noPermUser.Id, isActive: true));

        // A second administrator, so change orders can be decided by someone other than their creator.
        db.Users.Add(new User(ApproverUserId, UidApprover, "approver@example.com", "Approver", true));
        db.Memberships.Add(new Membership(ApproverMembershipId, OrgId, BranchId, ApproverUserId, isActive: true));
        var adminRole = await db.Roles.SingleAsync(r => r.OrganizationId == OrgId && r.Name == "Test Admin");
        db.MembershipRoles.Add(new MembershipRole(ApproverMembershipId, adminRole.Id, OrgId));
        await db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private static HttpRequestMessage Request(HttpMethod method, string url, string token = "token-org-a", string? key = null, Guid? membership = null, Guid? ifMatch = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var membershipId = membership ?? token switch
        {
            "token-org-b" => TestOnlyDataSeeder.TestMembershipBId,
            "token-approver" => ApproverMembershipId,
            _ => TestOnlyDataSeeder.TestMembershipId
        };
        request.Headers.Add("X-Membership-Id", membershipId.ToString());
        if (key is not null) request.Headers.Add("Idempotency-Key", key);
        if (ifMatch.HasValue) request.Headers.Add("If-Match", $"\"{ifMatch.Value}\"");
        return request;
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, object? body = null, string token = "token-org-a", string? key = null, Guid? ifMatch = null, Guid? membership = null)
    {
        var request = Request(method, url, token, key, membership, ifMatch);
        if (body is not null) request.Content = JsonContent.Create(body);
        return await _client.SendAsync(request);
    }

    private static async Task<string> ErrorCodeAsync(HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<ApiProblemDetails>())!.Code;

    private static string Key() => Guid.NewGuid().ToString("N");

    private static readonly Guid PlywoodId = TestOnlyDataSeeder.TestItemCatalogPlywoodId;
    private static readonly Guid LaminateId = TestOnlyDataSeeder.TestItemCatalogLaminateId;

    private async Task<Guid> CreateProducibleItemAsync(string code)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTimeOffset.UtcNow;
        var template = await db.Items.AsNoTracking().SingleAsync(i => i.Id == PlywoodId);
        var item = Item.CreateDraft(
            Guid.NewGuid(), OrgId, code, ItemType.Product, template.CategoryId, template.BrandId,
            LocalizedText.Create($"สินค้าสำเร็จรูป {code}", $"Finished {code}"), null, template.BaseUnitId, ItemAvailabilityMode.AllBranches,
            new ItemCapabilities(CanSell: true, CanCost: true, CanPurchase: false, CanStock: true, CanProduce: true), null, null, UserId, now, "TEST-MATERIAL");
        item.Activate(UserId, now, false, true, true, true);
        db.Items.Add(item);
        await db.SaveChangesAsync();
        return item.Id;
    }

    private async Task<WarehouseResponse> CreateWarehouseAsync()
    {
        var response = await SendAsync(HttpMethod.Post, "/api/v1/warehouses", new WarehouseRequest("คลังผลิต", "กรุงเทพ"), key: Key());
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<WarehouseResponse>())!;
    }

    private async Task StockAsync(Guid warehouseId, Guid itemId, decimal quantity, decimal unitCost)
    {
        var response = await SendAsync(HttpMethod.Post, "/api/v1/inventory/adjustments",
            new AdjustStockRequest(warehouseId, "ยกยอด", new List<AdjustmentLineRequest> { new(itemId, quantity, unitCost) }), key: Key());
        Assert.True(response.StatusCode == HttpStatusCode.Created, await response.Content.ReadAsStringAsync());
    }

    private async Task<StockBalanceResponse?> BalanceAsync(Guid warehouseId, Guid itemId)
    {
        var response = await SendAsync(HttpMethod.Get, $"/api/v1/inventory/balances?warehouseId={warehouseId}&itemId={itemId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<StockBalanceListResponse>())!.Items.SingleOrDefault();
    }

    private async Task AssertLedgerConsistentAsync()
    {
        var recon = (await (await SendAsync(HttpMethod.Get, "/api/v1/inventory/reconciliation")).Content.ReadFromJsonAsync<ReconciliationResponse>())!;
        Assert.Equal(0, recon.InconsistentCount);
        Assert.All(recon.Rows, r => Assert.True(r.BalanceOnHand >= 0));
    }

    private static async Task<T> Ok<T>(HttpResponseMessage response, HttpStatusCode expected = HttpStatusCode.OK)
    {
        Assert.True(response.StatusCode == expected, $"Expected {expected} but got {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private static List<BomLineRequest> Lines(decimal plywood = 2m, decimal laminate = 1m, decimal laminateScrap = 10m) =>
        new() { new(PlywoodId, plywood, 0m), new(LaminateId, laminate, laminateScrap) };

    private async Task<BomResponse> CreateBomAsync(Guid itemId, List<BomLineRequest>? lines = null) =>
        await Ok<BomResponse>(await SendAsync(HttpMethod.Post, "/api/v1/boms", new BomRequest(itemId, 1m, "BOM ทดสอบ", lines ?? Lines()), key: Key()), HttpStatusCode.Created);

    private async Task<BomResponse> ApproveAsync(BomResponse bom, int revisionNo, string token = "token-approver")
    {
        var revision = bom.Revisions.Single(r => r.RevisionNo == revisionNo);
        return await Ok<BomResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/boms/{bom.Id}/revisions/{revision.Id}/approve", token: token, ifMatch: revision.RowVersion));
    }

    private async Task<WorkOrderResponse> CreateWorkOrderAsync(Guid itemId, Guid warehouseId, decimal planned = 4m) =>
        await Ok<WorkOrderResponse>(await SendAsync(HttpMethod.Post, "/api/v1/work-orders", new WorkOrderRequest(itemId, warehouseId, null, planned, "ผลิตทดสอบ"), key: Key()), HttpStatusCode.Created);

    private async Task<WorkOrderResponse> ReleaseAsync(WorkOrderResponse wo) =>
        await Ok<WorkOrderResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/work-orders/{wo.Id}/release", ifMatch: wo.RowVersion));

    private async Task<WorkOrderResponse> GetWorkOrderAsync(Guid id) =>
        await Ok<WorkOrderResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/work-orders/{id}"));

    private Task<HttpResponseMessage> IssueAsync(Guid woId, decimal plywood, decimal laminate, string? key = null) =>
        SendAsync(HttpMethod.Post, $"/api/v1/work-orders/{woId}/issues",
            new WorkOrderMaterialsRequest(new List<WorkOrderMaterialLineRequest> { new(PlywoodId, plywood), new(LaminateId, laminate) }), key: key ?? Key());

    private Task<HttpResponseMessage> CompleteAsync(Guid woId, decimal quantity, string? key = null) =>
        SendAsync(HttpMethod.Post, $"/api/v1/work-orders/{woId}/completions", new WorkOrderCompleteRequest(quantity), key: key ?? Key());

    [Fact]
    public async Task Bom_Lifecycle_MakerChecker_Revisions_AndCycleDetection()
    {
        var fg = await CreateProducibleItemAsync("FG-A");
        var sub = await CreateProducibleItemAsync("FG-SUB");

        Assert.Equal("BOM_ITEM_NOT_PRODUCIBLE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/boms", new BomRequest(PlywoodId, 1m, null, Lines()), key: Key())));
        Assert.Equal("BOM_LINE_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/boms", new BomRequest(fg, 1m, null, new List<BomLineRequest> { new(PlywoodId, 1m, 0m), new(PlywoodId, 1m, 0m) }), key: Key())));
        Assert.Equal("BOM_CYCLE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/boms", new BomRequest(fg, 1m, null, new List<BomLineRequest> { new(fg, 1m, 0m) }), key: Key())));

        var key = Key();
        var bom = await Ok<BomResponse>(await SendAsync(HttpMethod.Post, "/api/v1/boms", new BomRequest(fg, 1m, "x", Lines()), key: key), HttpStatusCode.Created);
        Assert.StartsWith("BOM-", bom.Code);
        var replay = await Ok<BomResponse>(await SendAsync(HttpMethod.Post, "/api/v1/boms", new BomRequest(fg, 1m, "x", Lines()), key: key), HttpStatusCode.Created);
        Assert.Equal(bom.Id, replay.Id);
        Assert.Equal("BOM_ALREADY_EXISTS", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/boms", new BomRequest(fg, 1m, null, Lines()), key: Key())));

        var draft = bom.Revisions.Single();
        Assert.Equal("draft", draft.Status);
        Assert.Equal(1.1m, draft.Lines.Single(l => l.Component.Id == LaminateId).GrossQuantity);

        // The author cannot approve; a second user can.
        Assert.Equal("BOM_SELF_APPROVAL", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/boms/{bom.Id}/revisions/{draft.Id}/approve", ifMatch: draft.RowVersion)));
        Assert.Equal("BOM_VERSION_CONFLICT", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/boms/{bom.Id}/revisions/{draft.Id}/approve", token: "token-approver", ifMatch: Guid.NewGuid())));
        var edited = await Ok<BomResponse>(await SendAsync(HttpMethod.Put, $"/api/v1/boms/{bom.Id}/revisions/{draft.Id}", new BomDraftRequest(1m, "แก้ไข", Lines(plywood: 3m)), ifMatch: draft.RowVersion));
        Assert.Equal(3m, edited.Revisions.Single().Lines.Single(l => l.Component.Id == PlywoodId).Quantity);
        var approved = await ApproveAsync(edited, 1);
        Assert.Equal("approved", approved.Revisions.Single().Status);
        Assert.Equal("BOM_INVALID_STATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Put, $"/api/v1/boms/{bom.Id}/revisions/{draft.Id}", new BomDraftRequest(1m, null, Lines()), ifMatch: approved.Revisions.Single().RowVersion)));

        // A new revision supersedes the approved one once approved.
        var rev2 = await Ok<BomResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/boms/{bom.Id}/revisions", new BomDraftRequest(1m, "rev2", Lines(plywood: 2m))), HttpStatusCode.Created);
        Assert.Equal("BOM_INVALID_STATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/boms/{bom.Id}/revisions", new BomDraftRequest(1m, null, Lines()))));
        var after = await ApproveAsync(rev2, 2);
        Assert.Equal("approved", after.Revisions.Single(r => r.RevisionNo == 2).Status);
        Assert.Equal("obsolete", after.Revisions.Single(r => r.RevisionNo == 1).Status);

        // FG uses SUB would be fine, but SUB using FG afterwards forms a cycle.
        var parent = await CreateProducibleItemAsync("FG-PARENT");
        var parentUsesSub = await CreateBomAsync(parent, new List<BomLineRequest> { new(sub, 1m, 0m) });
        await ApproveAsync(parentUsesSub, 1);
        Assert.Equal("BOM_CYCLE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/boms", new BomRequest(sub, 1m, null, new List<BomLineRequest> { new(parent, 1m, 0m) }), key: Key())));

        var list = await Ok<BomListResponse>(await SendAsync(HttpMethod.Get, "/api/v1/boms?search=FG-A"));
        Assert.Single(list.Items);
        Assert.Equal(2, list.Items[0].ApprovedRevisionNo);

        Assert.Equal(HttpStatusCode.Forbidden, (await SendAsync(HttpMethod.Get, "/api/v1/boms", token: "token-no-perm", membership: MembershipNoPermId)).StatusCode);
    }

    [Fact]
    public async Task WorkOrder_FullFlow_IssueReturnComplete_ValuesStockAndCost()
    {
        var fg = await CreateProducibleItemAsync("FG-WO");
        var warehouse = await CreateWarehouseAsync();
        await StockAsync(warehouse.Id, PlywoodId, 20m, 1000m);
        await StockAsync(warehouse.Id, LaminateId, 10m, 500m);

        Assert.Equal("PRODUCTION_BOM_NOT_APPROVED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/work-orders", new WorkOrderRequest(fg, warehouse.Id, null, 4m, null), key: Key())));
        await ApproveAsync(await CreateBomAsync(fg), 1);

        var wo = await CreateWorkOrderAsync(fg, warehouse.Id, 4m);
        Assert.StartsWith("WO", wo.Number);
        Assert.Equal("draft", wo.Status);
        Assert.Equal(8m, wo.Materials.Single(m => m.Item.Id == PlywoodId).RequiredQuantity);
        Assert.Equal(4.4m, wo.Materials.Single(m => m.Item.Id == LaminateId).RequiredQuantity);

        Assert.Equal("PRODUCTION_INVALID_STATE", await ErrorCodeAsync(await IssueAsync(wo.Id, 1m, 1m)));
        wo = await ReleaseAsync(wo);
        Assert.Equal("PRODUCTION_INVALID_STATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/work-orders/{wo.Id}/release", ifMatch: wo.RowVersion)));

        Assert.Equal("PRODUCTION_OVER_ISSUED", await ErrorCodeAsync(await IssueAsync(wo.Id, 9m, 1m)));
        var issueKey = Key();
        wo = await Ok<WorkOrderResponse>(await IssueAsync(wo.Id, 8m, 4.4m, issueKey), HttpStatusCode.Created);
        Assert.Equal("in_progress", wo.Status);
        Assert.Single(wo.Transactions);
        var replay = await Ok<WorkOrderResponse>(await IssueAsync(wo.Id, 8m, 4.4m, issueKey), HttpStatusCode.Created);
        Assert.Single(replay.Transactions);
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", await ErrorCodeAsync(await IssueAsync(wo.Id, 1m, 1m, issueKey)));
        Assert.Equal(12m, (await BalanceAsync(warehouse.Id, PlywoodId))!.OnHand);
        Assert.Equal(5.6m, (await BalanceAsync(warehouse.Id, LaminateId))!.OnHand);

        // Completion needs materials issued for the cumulative share; half done consumes half the materials.
        wo = await Ok<WorkOrderResponse>(await CompleteAsync(wo.Id, 2m), HttpStatusCode.Created);
        Assert.Equal(2m, wo.CompletedQuantity);
        Assert.Equal(5100m, wo.CostAllocated);
        var balance = (await BalanceAsync(warehouse.Id, fg))!;
        Assert.Equal(2m, balance.OnHand);
        Assert.Equal(2550m, balance.AverageCost);

        // Only the part not yet consumed can go back.
        var tooMuch = new WorkOrderMaterialsRequest(new List<WorkOrderMaterialLineRequest> { new(PlywoodId, 5m) });
        Assert.Equal("PRODUCTION_RETURN_EXCEEDS", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/work-orders/{wo.Id}/returns", tooMuch, key: Key())));
        wo = await Ok<WorkOrderResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/work-orders/{wo.Id}/returns", new WorkOrderMaterialsRequest(new List<WorkOrderMaterialLineRequest> { new(PlywoodId, 1m) }), key: Key()), HttpStatusCode.Created);
        Assert.Equal(13m, (await BalanceAsync(warehouse.Id, PlywoodId))!.OnHand);

        Assert.Equal("PRODUCTION_MATERIAL_SHORTAGE", await ErrorCodeAsync(await CompleteAsync(wo.Id, 2m)));
        Assert.Equal("PRODUCTION_OVER_COMPLETED", await ErrorCodeAsync(await CompleteAsync(wo.Id, 3m)));
        wo = await Ok<WorkOrderResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/work-orders/{wo.Id}/issues",
            new WorkOrderMaterialsRequest(new List<WorkOrderMaterialLineRequest> { new(PlywoodId, 1m) }), key: Key()), HttpStatusCode.Created);
        wo = await Ok<WorkOrderResponse>(await CompleteAsync(wo.Id, 2m), HttpStatusCode.Created);
        Assert.Equal("completed", wo.Status);
        Assert.Equal(4m, wo.CompletedQuantity);
        Assert.Equal(4m, (await BalanceAsync(warehouse.Id, fg))!.OnHand);
        Assert.Equal(wo.Transactions.Where(t => t.Kind == "issue").Sum(t => t.Value) - wo.Transactions.Where(t => t.Kind == "return").Sum(t => t.Value),
            (await BalanceAsync(warehouse.Id, fg))!.TotalValue);
        Assert.Equal("PRODUCTION_INVALID_STATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/work-orders/{wo.Id}/cancel", new WorkOrderActionRequest("x"), ifMatch: wo.RowVersion)));
        await AssertLedgerConsistentAsync();
    }

    [Fact]
    public async Task WorkOrder_Cancel_Rules_InsufficientStock_AndPermissions()
    {
        var fg = await CreateProducibleItemAsync("FG-CAN");
        var warehouse = await CreateWarehouseAsync();
        await StockAsync(warehouse.Id, PlywoodId, 5m, 1000m);
        await StockAsync(warehouse.Id, LaminateId, 10m, 500m);
        await ApproveAsync(await CreateBomAsync(fg), 1);

        var wo = await ReleaseAsync(await CreateWorkOrderAsync(fg, warehouse.Id, 4m));

        // 8 plywood required but only 5 on hand: nothing is issued and the order is unchanged.
        Assert.Equal("INVENTORY_INSUFFICIENT_STOCK", await ErrorCodeAsync(await IssueAsync(wo.Id, 8m, 4m)));
        var unchanged = await GetWorkOrderAsync(wo.Id);
        Assert.Equal("released", unchanged.Status);
        Assert.Empty(unchanged.Transactions);
        Assert.Equal(5m, (await BalanceAsync(warehouse.Id, PlywoodId))!.OnHand);

        wo = await Ok<WorkOrderResponse>(await IssueAsync(wo.Id, 4m, 2m), HttpStatusCode.Created);
        Assert.Equal("PRODUCTION_HAS_ISSUED_MATERIALS", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/work-orders/{wo.Id}/cancel", new WorkOrderActionRequest("ยกเลิก"), ifMatch: wo.RowVersion)));
        wo = await Ok<WorkOrderResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/work-orders/{wo.Id}/returns",
            new WorkOrderMaterialsRequest(wo.Materials.Select(m => new WorkOrderMaterialLineRequest(m.Item.Id, m.NetIssuedQuantity)).ToList()), key: Key()), HttpStatusCode.Created);
        Assert.Equal(5m, (await BalanceAsync(warehouse.Id, PlywoodId))!.OnHand);
        Assert.Equal("PRODUCTION_REASON_REQUIRED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/work-orders/{wo.Id}/cancel", new WorkOrderActionRequest(" "), ifMatch: wo.RowVersion)));
        var cancelled = await Ok<WorkOrderResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/work-orders/{wo.Id}/cancel", new WorkOrderActionRequest("ยกเลิกงาน"), ifMatch: wo.RowVersion));
        Assert.Equal("cancelled", cancelled.Status);

        var list = await Ok<WorkOrderListResponse>(await SendAsync(HttpMethod.Get, "/api/v1/work-orders?status=cancelled"));
        Assert.Single(list.Items);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await SendAsync(HttpMethod.Get, "/api/v1/work-orders?status=bogus")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await SendAsync(HttpMethod.Get, "/api/v1/work-orders", token: "token-no-perm", membership: MembershipNoPermId)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await SendAsync(HttpMethod.Post, "/api/v1/work-orders", new WorkOrderRequest(fg, warehouse.Id, null, 1m, null), token: "token-no-perm", membership: MembershipNoPermId, key: Key())).StatusCode);
        await AssertLedgerConsistentAsync();
    }

    [Fact]
    public async Task WorkCenter_RoutingSnapshot_Immutability_AndDailyCapacityView()
    {
        var fg = await CreateProducibleItemAsync("FG-RT");
        var warehouse = await CreateWarehouseAsync();

        // 1. Create Work Center and verify validation / conflict rules
        Assert.Equal("PRODUCTION_WORK_CENTER_INVALID", await ErrorCodeAsync(
            await SendAsync(HttpMethod.Post, "/api/v1/work-centers", new CreateWorkCenterRequest("WC-BAD", "สถานีผิด", 30m, 0m), key: Key())));

        var wc = await Ok<WorkCenterResponse>(
            await SendAsync(HttpMethod.Post, "/api/v1/work-centers", new CreateWorkCenterRequest("WC-SAW", "เครื่องเลื่อย CNC", 8m, 250m), key: Key()),
            HttpStatusCode.Created);
        Assert.Equal("WC-SAW", wc.Code);
        Assert.Equal(8m, wc.DailyCapacityHours);
        Assert.Equal("active", wc.Status);

        Assert.Equal("PRODUCTION_WORK_CENTER_CODE_CONFLICT", await ErrorCodeAsync(
            await SendAsync(HttpMethod.Post, "/api/v1/work-centers", new CreateWorkCenterRequest("wc-saw", "ซ้ำ", 8m, 100m), key: Key())));

        // 2. Create BOM with Routing Steps (1 internal on WC-SAW, 1 subcontracted)
        var routing = new List<BomRoutingStepRequest>
        {
            new(10, "CUT", "ตัดแผ่นไม้", wc.Id, SetupHours: 1m, RunHours: 2m, IsSubcontracted: false, StdSubcontractCost: 0m, Note: "ตัดตามแบบ"),
            new(20, "COAT", "พ่นสีภายนอก", null, SetupHours: 0m, RunHours: 0m, IsSubcontracted: true, StdSubcontractCost: 500m, Note: "ส่งโรงพ่น")
        };
        var bom = await Ok<BomResponse>(
            await SendAsync(HttpMethod.Post, "/api/v1/boms", new BomRequest(fg, 1m, "BOM พร้อม Routing", Lines(), routing), key: Key()),
            HttpStatusCode.Created);
        Assert.Equal(2, bom.Revisions.Single().RoutingSteps.Count);
        bom = await ApproveAsync(bom, 1);

        // Cannot deactivate Work Center while referenced by an approved BOM
        Assert.Equal("PRODUCTION_WORK_CENTER_IN_USE", await ErrorCodeAsync(
            await SendAsync(HttpMethod.Post, $"/api/v1/work-centers/{wc.Id}/deactivate", ifMatch: wc.RowVersion)));

        // 3. Create Work Order (plannedQuantity = 4, single-day window) -> Routing is snapshotted
        var day = new DateOnly(2026, 10, 30);
        var wo = await Ok<WorkOrderResponse>(
            await SendAsync(HttpMethod.Post, "/api/v1/work-orders", new WorkOrderRequest(fg, warehouse.Id, null, 4m, "ทดสอบ Snapshot", day, day), key: Key()),
            HttpStatusCode.Created);
        Assert.Equal(2, wo.Operations.Count);
        var cutOp = wo.Operations.Single(o => o.Sequence == 10);
        Assert.Equal(1m, cutOp.PlannedSetupHours);
        Assert.Equal(8m, cutOp.PlannedRunHours); // 2m runHours * 4 planned / 1 output = 8m

        // 4. Create & approve BOM Revision 2 with changed RunHours (5m) -> existing WO snapshot must NOT change
        var routingRev2 = new List<BomRoutingStepRequest>
        {
            new(10, "CUT", "ตัดแผ่นไม้ปรับใหม่", wc.Id, SetupHours: 2m, RunHours: 5m, IsSubcontracted: false, StdSubcontractCost: 0m)
        };
        var rev2 = await Ok<BomResponse>(
            await SendAsync(HttpMethod.Post, $"/api/v1/boms/{bom.Id}/revisions", new BomDraftRequest(1m, "rev2", Lines(), routingRev2)),
            HttpStatusCode.Created);
        await ApproveAsync(rev2, 2);

        var reloadedWo = await GetWorkOrderAsync(wo.Id);
        Assert.Equal(2, reloadedWo.Operations.Count);
        Assert.Equal(8m, reloadedWo.Operations.Single(o => o.Sequence == 10).PlannedRunHours);

        // 5. Release WO and verify Daily Capacity View reflects 9.0 planned hours on 8.0 capacity (112.5% overloaded)
        wo = await ReleaseAsync(wo);
        var dayIso = day.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        var capView = await Ok<WorkCenterCapacityViewResponse>(
            await SendAsync(HttpMethod.Get, $"/api/v1/work-centers/capacity?fromDate={dayIso}&toDate={dayIso}"));
        var wcRow = capView.WorkCenters.Single(w => w.WorkCenterId == wc.Id);
        var dayPoint = wcRow.Days.Single(d => d.Date == day);
        Assert.Equal(8m, dayPoint.CapacityHours);
        Assert.Equal(9m, dayPoint.PlannedHours);
        Assert.True(dayPoint.IsOverloaded);
        Assert.Equal(112.5m, dayPoint.UtilizationPercent);
    }

    [Fact]
    public async Task WorkOrder_TotalManufacturingCost_CombinesMaterial_TimesheetLabor_AndSubcontractPo()
    {
        var fg = await CreateProducibleItemAsync("FG-COST");
        var warehouse = await CreateWarehouseAsync();
        await StockAsync(warehouse.Id, PlywoodId, 20m, 1000m);
        await StockAsync(warehouse.Id, LaminateId, 10m, 500m);

        // Seed an active Project and an approved subcontract PurchaseOrder
        Guid projectId;
        Guid poId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var now = DateTimeOffset.UtcNow;

            var customer = Customer.CreateDraft(
                Guid.NewGuid(), OrgId, UserId, CustomerType.Person, "ลูกค้าผลิต G-13", null, "th",
                new PrimaryContactInput("คุณสมศรี", null, "0899999999", null, "phone"), now);
            customer.Activate(customer.RowVersion);
            db.Customers.Add(customer);

            var site = Site.CreateActive(
                Guid.NewGuid(), OrgId, customer.Id, UserId, "ไซต์งาน G-13",
                new SiteAddressInput("99 สุขุมวิท", "คลองเตย", "คลองเตย", "กรุงเทพมหานคร", "10110", "TH"), 13.7m, 100.5m, null, now);
            db.Sites.Add(site);

            var opp = Opportunity.CreateDraft(
                Guid.NewGuid(), OrgId, BranchId, customer.Id, site.Id, UserId, UserId,
                "งานผลิตเฟอร์นิเจอร์", "ขอบเขต", new[] { "built-in" }, null, 200000m, "THB",
                new DateOnly(2026, 12, 31), now.AddDays(1), "นัดวัด", now);
            opp.Qualify(opp.RowVersion);
            opp.EnterSurveying(opp.RowVersion, site.Id);
            opp.EnterEstimating(opp.RowVersion);
            opp.EnterProposed(opp.RowVersion);
            opp.MarkWon(opp.RowVersion);
            db.Opportunities.Add(opp);

            var estimate = Estimate.CreateDraft(Guid.NewGuid(), OrgId, BranchId, customer.Id, opp.Id, "EST-G13-01", null, null);
            db.Estimates.Add(estimate);

            var quotation = new Quotation(Guid.NewGuid(), OrgId, BranchId, customer.Id, opp.Id, estimate.Id, estimate.CurrentRevision!.Id, "QT-G13-01", 200000m, "hash-g13", now);
            quotation.Accept(now);
            db.Quotations.Add(quotation);

            var project = Project.CreateFromHandover(
                Guid.NewGuid(), OrgId, BranchId, customer.Id, site.Id, opp.Id, quotation.Id, estimate.Id, estimate.CurrentRevision.Id,
                null, "PRJ-G13-01", "โครงการทดสอบต้นทุนการผลิต", UserId, new DateOnly(2026, 10, 1), "QT-G13-01", 200000m, "hash-g13", null, "base-g13", UserId, now);
            project.SetPlan(new DateOnly(2026, 10, 1), new DateOnly(2026, 12, 31), now);
            var budgetLine = new ProjectBudgetLine(Guid.NewGuid(), OrgId, project.Id, ProjectBudgetCategory.Labor, "งบแรงงานและผลิต", 100000m, 1);
            db.ProjectBudgetLines.Add(budgetLine);
            project.SetBaselineBudget(100000m, "budget-hash", now);
            project.TransitionTo(ProjectStatus.Active, null, 1, now);
            db.Projects.Add(project);
            projectId = project.Id;

            var plywoodItem = await db.Items.AsNoTracking().Include(i => i.BaseUnit).SingleAsync(i => i.Id == PlywoodId);
            var supplier = new Supplier(Guid.NewGuid(), OrgId, "SUP-SUB-01", "ผู้รับจ้างพ่นสี", "คุณช่าง", "0812223333", "sub@example.com", "1234567890123", "กรุงเทพ", 30, UserId, now);
            db.Suppliers.Add(supplier);

            var po = new PurchaseOrder(Guid.NewGuid(), OrgId, BranchId, supplier.Id, project.Id, "PO-SUB-0001", new DateOnly(2026, 10, 30), "จ้างพ่นสี", UserId, now);
            po.AddLine(new PurchaseOrderLine(
                Guid.NewGuid(), OrgId, po.Id, 1, plywoodItem.Id, plywoodItem.Code, "งานจ้างพ่นสีไฮกลอส",
                plywoodItem.BaseUnitId, plywoodItem.BaseUnit?.Code ?? "PCS", 2m, 600m));
            po.Submit(now);
            po.Decide(true, ApproverUserId, "อนุมัติ PO จ้างผลิต", now);
            db.PurchaseOrders.Add(po);
            poId = po.Id;

            await db.SaveChangesAsync();
        }

        var wc = await Ok<WorkCenterResponse>(
            await SendAsync(HttpMethod.Post, "/api/v1/work-centers", new CreateWorkCenterRequest(null, "ห้องประกอบ", 8m, 200m), key: Key()),
            HttpStatusCode.Created);
        Assert.StartsWith("WC-", wc.Code);

        var routing = new List<BomRoutingStepRequest>
        {
            new(10, "ASSY", "ประกอบโครง", wc.Id, SetupHours: 0.5m, RunHours: 1.5m, IsSubcontracted: false, StdSubcontractCost: 0m),
            new(20, "PAINT", "จ้างพ่นสีภายนอก", null, SetupHours: 0m, RunHours: 0m, IsSubcontracted: true, StdSubcontractCost: 600m)
        };
        var bom = await ApproveAsync(await Ok<BomResponse>(
            await SendAsync(HttpMethod.Post, "/api/v1/boms", new BomRequest(fg, 1m, "BOM ต้นทุนรวม", Lines(), routing), key: Key()),
            HttpStatusCode.Created), 1);

        var wo = await Ok<WorkOrderResponse>(
            await SendAsync(HttpMethod.Post, "/api/v1/work-orders", new WorkOrderRequest(fg, warehouse.Id, projectId, 2m, "WO ต้นทุนครบ"), key: Key()),
            HttpStatusCode.Created);
        wo = await ReleaseAsync(wo);

        // 1. Issue materials: 4 Plywood @ 1000 (4000) + 2.2 Laminate @ 500 (1100) = 5100 THB
        wo = await Ok<WorkOrderResponse>(await IssueAsync(wo.Id, 4m, 2.2m), HttpStatusCode.Created);
        Assert.Equal(5100m, wo.NetMaterialValue);

        // 2. Log, submit, and approve a G-20 Labor Timesheet linked to this WorkOrder (4h @ 150 default rate = 600 THB)
        var projectCtrl = await Ok<ProjectControlResponse>(
            await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/timesheets",
                new CreateProjectTimesheetRequest(UserId, new DateOnly(2026, 10, 30), 4m, null, wo.Id, "ประกอบโครงไม้ตาม WO"), key: Key()),
            HttpStatusCode.Created);
        var ts = projectCtrl.Timesheets.Single();
        projectCtrl = await Ok<ProjectControlResponse>(
            await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/timesheets/{ts.Id}/submit",
                new ProjectTimesheetActionRequest(ts.RowVersion, null)));
        ts = projectCtrl.Timesheets.Single();
        await Ok<ProjectControlResponse>(
            await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/timesheets/{ts.Id}/approve",
                new ProjectTimesheetActionRequest(ts.RowVersion, "อนุมัติชั่วโมงประกอบ"), token: "token-approver"));

        // 3. Complete internal operation & record subcontract PO operation (1200 THB)
        var internalOp = wo.Operations.Single(o => !o.IsSubcontracted);
        var subOp = wo.Operations.Single(o => o.IsSubcontracted);

        Assert.Equal("PRODUCTION_OPERATION_NOT_SUBCONTRACTED", await ErrorCodeAsync(
            await SendAsync(HttpMethod.Post, $"/api/v1/work-orders/{wo.Id}/operations/{internalOp.Id}/subcontract",
                new WorkOrderSubcontractOperationRequest(poId, 500m), key: Key())));

        wo = await Ok<WorkOrderResponse>(
            await SendAsync(HttpMethod.Post, $"/api/v1/work-orders/{wo.Id}/operations/{internalOp.Id}/complete",
                new WorkOrderCompleteOperationRequest(4m, "ประกอบเสร็จ"), key: Key()));
        Assert.Equal("completed", wo.Operations.Single(o => o.Id == internalOp.Id).Status);

        Assert.Equal("PRODUCTION_SUBCONTRACT_COST_EXCEEDS_PO", await ErrorCodeAsync(
            await SendAsync(HttpMethod.Post, $"/api/v1/work-orders/{wo.Id}/operations/{subOp.Id}/subcontract",
                new WorkOrderSubcontractOperationRequest(poId, 1500m), key: Key())));

        wo = await Ok<WorkOrderResponse>(
            await SendAsync(HttpMethod.Post, $"/api/v1/work-orders/{wo.Id}/operations/{subOp.Id}/subcontract",
                new WorkOrderSubcontractOperationRequest(poId, 1200m, "รับงานพ่นสีตาม PO"), key: Key()));

        Assert.Equal(5100m, wo.NetMaterialValue);
        Assert.Equal(4m, wo.LaborHours);
        Assert.Equal(600m, wo.LaborCost);
        Assert.Single(wo.LaborEntries);
        Assert.Equal(1200m, wo.SubcontractCost);
        Assert.Equal(6900m, wo.TotalManufacturingCost);

        // 4. Complete WorkOrder (2 units) -> Finished goods stock receives full 6900 THB (3450 THB/unit)
        wo = await Ok<WorkOrderResponse>(await CompleteAsync(wo.Id, 2m), HttpStatusCode.Created);
        Assert.Equal("completed", wo.Status);
        Assert.Equal(6900m, wo.CostAllocated);

        var fgBalance = (await BalanceAsync(warehouse.Id, fg))!;
        Assert.Equal(2m, fgBalance.OnHand);
        Assert.Equal(6900m, fgBalance.TotalValue);
        Assert.Equal(3450m, fgBalance.AverageCost);
        await AssertLedgerConsistentAsync();
    }
}
