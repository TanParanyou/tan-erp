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
using TanErp.Api.Contracts.Mrp;
using TanErp.Api.Contracts.Procurement;
using TanErp.Api.Contracts.Production;
using TanErp.Api.ErrorHandling;
using TanErp.Domain.Common;
using TanErp.Domain.Items;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class MrpEndpointsTests : IAsyncLifetime
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

    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);
    private static readonly DateOnly Due = Today.AddDays(20);

    private async Task<Guid> ApprovedFinishedGoodAsync(string code)
    {
        var fg = await CreateProducibleItemAsync(code);
        await ApproveAsync(await CreateBomAsync(fg), 1);
        return fg;
    }

    private static MrpRunRequest Run(Guid fg, decimal quantity = 4m, bool includeOpenWorkOrders = false) =>
        new(Today, 7, 3, includeOpenWorkOrders, quantity > 0 ? new List<MrpDemandRequest> { new(fg, quantity, Due, "SO-TEST") } : new List<MrpDemandRequest>());

    private async Task<MrpRunResponse> RunAsync(MrpRunRequest request, string? key = null) =>
        await Ok<MrpRunResponse>(await SendAsync(HttpMethod.Post, "/api/v1/mrp/runs", request, key: key ?? Key()), HttpStatusCode.Created);

    private async Task<MrpRunResponse> GetRunAsync(Guid id) =>
        await Ok<MrpRunResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/mrp/runs/{id}"));

    private Task<HttpResponseMessage> DecideAsync(MrpRunResponse run, MrpRecommendationResponse rec, string action, string token = "token-approver") =>
        SendAsync(HttpMethod.Post, $"/api/v1/mrp/runs/{run.Id}/recommendations/{rec.Id}/{action}", token: token, ifMatch: rec.RowVersion);

    [Fact]
    public async Task Run_ExplodesBom_NetsStock_AndStoresAFrozenSnapshot()
    {
        var fg = await ApprovedFinishedGoodAsync("FG-MRP1");
        var warehouse = await CreateWarehouseAsync();
        await StockAsync(warehouse.Id, PlywoodId, 5m, 1000m);

        var key = Key();
        var run = await RunAsync(Run(fg), key);
        Assert.StartsWith("MRP", run.Number);
        Assert.Equal(64, run.InputHash.Length);
        Assert.Equal(1, run.Snapshot.DemandCount);

        var make = run.Recommendations.Single(r => r.Item.Id == fg);
        Assert.Equal(("make", 4m, Due, Due.AddDays(-3), 0), (make.Action, make.Quantity, make.NeedBy, make.OrderBy, make.Level));
        Assert.Equal("SO-TEST", make.Reasons.Single().SourceRef);

        // 4 cabinets need 8 plywood (5 in stock → buy 3) and 4.4 laminate (none in stock → buy 4.4), released at the make date.
        var plywood = run.Recommendations.Single(r => r.Item.Id == PlywoodId);
        Assert.Equal(("buy", 3m, 5m, 1), (plywood.Action, plywood.Quantity, plywood.StockUsed, plywood.Level));
        Assert.Equal(Due.AddDays(-3), plywood.NeedBy);
        Assert.Equal(Due.AddDays(-10), plywood.OrderBy);
        Assert.Equal(4.4m, run.Recommendations.Single(r => r.Item.Id == LaminateId).Quantity);

        // Same key + payload replays; same key with another payload is rejected.
        Assert.Equal(run.Id, (await RunAsync(Run(fg), key)).Id);
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/mrp/runs", Run(fg, 9m), key: key)));

        // Stock arrives after the run: the stored plan does not move, a new run does.
        await StockAsync(warehouse.Id, PlywoodId, 20m, 1000m);
        var unchanged = await GetRunAsync(run.Id);
        Assert.Equal(run.InputHash, unchanged.InputHash);
        Assert.Equal(3m, unchanged.Recommendations.Single(r => r.Item.Id == PlywoodId).Quantity);

        var second = await RunAsync(Run(fg));
        Assert.NotEqual(run.InputHash, second.InputHash);
        Assert.DoesNotContain(second.Recommendations, r => r.Item.Id == PlywoodId);
        var third = await RunAsync(Run(fg));
        Assert.Equal(second.InputHash, third.InputHash);
        Assert.Equal(second.Recommendations.Select(r => (r.Item.Id, r.Quantity, r.NeedBy)), third.Recommendations.Select(r => (r.Item.Id, r.Quantity, r.NeedBy)));

        var list = await Ok<MrpRunListResponse>(await SendAsync(HttpMethod.Get, "/api/v1/mrp/runs"));
        Assert.Equal(3, list.Items.Count);
    }

    [Fact]
    public async Task Run_CountsOpenPurchaseOrdersAndOpenWorkOrders()
    {
        var fg = await ApprovedFinishedGoodAsync("FG-MRP2");
        var warehouse = await CreateWarehouseAsync();
        await StockAsync(warehouse.Id, PlywoodId, 6m, 1000m);
        await StockAsync(warehouse.Id, LaminateId, 10m, 500m);

        // An open work order for 4 units needs 8 plywood; 6 in stock leaves 2 to buy.
        var wo = await ReleaseAsync(await CreateWorkOrderAsync(fg, warehouse.Id, 4m));
        var run = await RunAsync(Run(fg, 0m, includeOpenWorkOrders: true));
        Assert.Equal(2m, run.Recommendations.Single(r => r.Item.Id == PlywoodId).Quantity);
        Assert.Equal(wo.Number, run.Recommendations.Single(r => r.Item.Id == PlywoodId).Reasons.Single().SourceRef);
        Assert.DoesNotContain(run.Recommendations, r => r.Item.Id == LaminateId);

        // Work order output counts as supply for a finished-good demand that is due later.
        var withDemand = await RunAsync(Run(fg, 4m, includeOpenWorkOrders: true));
        Assert.DoesNotContain(withDemand.Recommendations, r => r.Item.Id == fg);

        Assert.Equal("MRP_DEMAND_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/mrp/runs", Run(fg, 0m), key: Key())));
        Assert.Equal("MRP_DEMAND_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/mrp/runs", new MrpRunRequest(Today, 7, 3, false, new List<MrpDemandRequest> { new(fg, 1m, Today.AddDays(-1), null) }), key: Key())));
        Assert.Equal("MRP_FIELD_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/mrp/runs", new MrpRunRequest(Today, -1, 3, false, new List<MrpDemandRequest> { new(fg, 1m, Due, null) }), key: Key())));
    }

    [Fact]
    public async Task Recommendations_RequireIndependentApproval_AndConvertToDraftDocuments()
    {
        var fg = await ApprovedFinishedGoodAsync("FG-MRP3");
        var warehouse = await CreateWarehouseAsync();
        var run = await RunAsync(Run(fg));
        var make = run.Recommendations.Single(r => r.Item.Id == fg);
        var buy = run.Recommendations.Single(r => r.Item.Id == PlywoodId);

        Assert.Equal("MRP_SELF_APPROVAL", await ErrorCodeAsync(await DecideAsync(run, make, "approve", "token-org-a")));
        Assert.Equal("MRP_VERSION_CONFLICT", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/mrp/runs/{run.Id}/recommendations/{make.Id}/approve", token: "token-approver", ifMatch: Guid.NewGuid())));
        Assert.Equal("MRP_INVALID_STATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/mrp/runs/{run.Id}/recommendations/{make.Id}/convert", new MrpConvertRequest(null, null, warehouse.Id), ifMatch: make.RowVersion)));

        var approved = await Ok<MrpRunResponse>(await DecideAsync(run, make, "approve"));
        make = approved.Recommendations.Single(r => r.Id == make.Id);
        Assert.Equal("approved", make.Status);
        Assert.Equal("MRP_CONVERT_INPUT_REQUIRED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/mrp/runs/{run.Id}/recommendations/{make.Id}/convert", new MrpConvertRequest(null, null, null), ifMatch: make.RowVersion)));
        var converted = await Ok<MrpRunResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/mrp/runs/{run.Id}/recommendations/{make.Id}/convert", new MrpConvertRequest(null, null, warehouse.Id), ifMatch: make.RowVersion));
        var link = converted.Recommendations.Single(r => r.Id == make.Id).Converted!;
        Assert.Equal("work_order", link.Type);
        var order = await GetWorkOrderAsync(link.Id);
        Assert.Equal(("draft", 4m), (order.Status, order.PlannedQuantity));
        Assert.Contains(run.Number, order.Note);

        // A buy recommendation becomes a draft purchase order for the chosen supplier and price.
        var supplier = await Ok<SupplierResponse>(await SendAsync(HttpMethod.Post, "/api/v1/suppliers", new SupplierRequest("บริษัท ไม้ดี", null, null, null, null, null, 30), key: Key()), HttpStatusCode.Created);
        var buyApproved = (await Ok<MrpRunResponse>(await DecideAsync(converted, buy, "approve"))).Recommendations.Single(r => r.Id == buy.Id);
        Assert.Equal("MRP_CONVERT_INPUT_REQUIRED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/mrp/runs/{run.Id}/recommendations/{buy.Id}/convert", new MrpConvertRequest(null, null, null), ifMatch: buyApproved.RowVersion)));
        var po = await Ok<MrpRunResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/mrp/runs/{run.Id}/recommendations/{buy.Id}/convert", new MrpConvertRequest(supplier.Id, 1250m, null), ifMatch: buyApproved.RowVersion));
        var poLink = po.Recommendations.Single(r => r.Id == buy.Id).Converted!;
        Assert.Equal("purchase_order", poLink.Type);
        var purchase = await Ok<PurchaseOrderResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/purchase-orders/{poLink.Id}"));
        Assert.Equal("draft", purchase.Status);
        Assert.Equal(buy.Quantity, purchase.Lines.Single().Quantity);
        Assert.Equal(buy.NeedBy, purchase.ExpectedDeliveryDate);

        // A rejected recommendation can never be converted.
        var laminate = po.Recommendations.Single(r => r.Item.Id == LaminateId);
        var rejected = (await Ok<MrpRunResponse>(await DecideAsync(po, laminate, "reject"))).Recommendations.Single(r => r.Id == laminate.Id);
        Assert.Equal("rejected", rejected.Status);
        Assert.Equal("MRP_INVALID_STATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/mrp/runs/{run.Id}/recommendations/{laminate.Id}/convert", new MrpConvertRequest(supplier.Id, 1m, null), ifMatch: rejected.RowVersion)));
    }

    [Fact]
    public async Task Mrp_Permissions_AndCrossOrganizationIsolation()
    {
        var fg = await ApprovedFinishedGoodAsync("FG-MRP4");
        Assert.Equal(HttpStatusCode.Forbidden, (await SendAsync(HttpMethod.Get, "/api/v1/mrp/runs", token: "token-no-perm", membership: MembershipNoPermId)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await SendAsync(HttpMethod.Post, "/api/v1/mrp/runs", Run(fg), token: "token-no-perm", membership: MembershipNoPermId, key: Key())).StatusCode);
        var run = await RunAsync(Run(fg));
        var foreign = await SendAsync(HttpMethod.Get, $"/api/v1/mrp/runs/{run.Id}", token: "token-org-b");
        Assert.True(foreign.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Forbidden);
    }
}
