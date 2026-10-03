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
using TanErp.Api.Contracts.Projects;
using TanErp.Api.ErrorHandling;
using TanErp.Domain.Commercial;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Estimates;
using TanErp.Domain.Projects;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class InventoryEndpointsTests : IAsyncLifetime
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

    private sealed record Seeded(Guid QuotationId, Guid QuotationVersion, Guid OpportunityId, Guid EstimateId, string QuotationNumber, decimal Total);

    /// <summary>An accepted quotation on a Won opportunity, built from domain transitions so no API flow is needed.</summary>
    private async Task<Seeded> SeedAcceptedQuotationAsync(bool markWon = true, bool accept = true, decimal total = 125000.50m)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTimeOffset.UtcNow;

        var customer = Customer.CreateDraft(
            Guid.NewGuid(), OrgId, UserId, CustomerType.Person, "คุณลูกค้า โครงการ TEST_ONLY", null, "th",
            new PrimaryContactInput("คุณสมชาย", null, "0811111111", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        db.Customers.Add(customer);

        var site = Site.CreateActive(
            Guid.NewGuid(), OrgId, customer.Id, UserId, "บ้านพักอาศัย",
            new SiteAddressInput("123 ถนนสุขุมวิท", "คลองเตย", "คลองเตย", "กรุงเทพมหานคร", "10110", "TH"), 13.7m, 100.5m, null, now);
        db.Sites.Add(site);

        var opp = Opportunity.CreateDraft(
            Guid.NewGuid(), OrgId, BranchId, customer.Id, site.Id, UserId, UserId,
            "งานบิลท์อินห้องนอน", "ขอบเขตงาน", new[] { "built-in" }, null, 350000m, "THB",
            new DateOnly(2026, 12, 31), now.AddDays(2), "นัดเข้าวัดพื้นที่", now);
        opp.Qualify(opp.RowVersion);
        opp.EnterSurveying(opp.RowVersion, site.Id);
        opp.EnterEstimating(opp.RowVersion);
        opp.EnterProposed(opp.RowVersion);
        if (markWon) opp.MarkWon(opp.RowVersion);
        db.Opportunities.Add(opp);

        var estimate = Estimate.CreateDraft(
            Guid.NewGuid(), OrgId, BranchId, customer.Id, opp.Id, $"EST-T-{Guid.NewGuid():N}"[..14],
            siteSurveyRevisionId: null, siteSurveySnapshotHash: null);
        db.Estimates.Add(estimate);

        var number = $"QT-T-{Guid.NewGuid():N}"[..14];
        var quotation = new Quotation(
            Guid.NewGuid(), OrgId, BranchId, customer.Id, opp.Id, estimate.Id, estimate.CurrentRevision!.Id,
            number, total, "snapshot-hash-abc", now);
        if (accept) quotation.Accept(now);
        db.Quotations.Add(quotation);

        await db.SaveChangesAsync();
        return new Seeded(quotation.Id, quotation.RowVersion, opp.Id, estimate.Id, number, total);
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

    private static async Task<ProjectControlResponse> ControlAsync(HttpResponseMessage response)
    {
        Assert.True(response.IsSuccessStatusCode, $"Unexpected {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        return (await response.Content.ReadFromJsonAsync<ProjectControlResponse>())!;
    }

    private async Task<Guid> CreateProjectAsync()
    {
        var seeded = await SeedAcceptedQuotationAsync();
        var response = await SendAsync(HttpMethod.Post, "/api/v1/projects",
            new CreateProjectFromHandoverRequest(seeded.QuotationId, seeded.QuotationVersion, UserId, null, null),
            key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<ProjectResponse>())!.Id;
    }

    private async Task<ProjectControlResponse> GetControlAsync(Guid projectId) =>
        await ControlAsync(await SendAsync(HttpMethod.Get, $"/api/v1/projects/{projectId}/control"));

    private static readonly DateOnly Start = new(2026, 11, 1);
    private static readonly DateOnly End = new(2027, 1, 31);

    private async Task<ProjectControlResponse> ActivateAsync(Guid projectId, params decimal[] amounts)
    {
        var control = await GetControlAsync(projectId);
        control = await ControlAsync(await SendAsync(HttpMethod.Put, $"/api/v1/projects/{projectId}/plan", new SetProjectPlanRequest(Start, End), ifMatch: control.RowVersion));
        var lines = amounts.Select((a, i) => new ProjectBudgetLineRequest("material", $"หมวด {i + 1}", a)).ToList();
        control = await ControlAsync(await SendAsync(HttpMethod.Put, $"/api/v1/projects/{projectId}/budget", new ReplaceProjectBudgetRequest(lines), ifMatch: control.RowVersion));
        return await ControlAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/transitions", new TransitionProjectRequest("active", null), ifMatch: control.RowVersion));
    }


    private static readonly Guid PlywoodId = TestOnlyDataSeeder.TestItemCatalogPlywoodId;
    private static readonly Guid LaminateId = TestOnlyDataSeeder.TestItemCatalogLaminateId;

    private async Task<SupplierResponse> CreateSupplierAsync(string name = "บริษัท ไม้ดี จำกัด", string? key = null)
    {
        var response = await SendAsync(HttpMethod.Post, "/api/v1/suppliers", new SupplierRequest(name, "Good Wood", "0105500000001", "คุณขาย", "021234567", "sales@example.test", 30), key: key ?? Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<SupplierResponse>())!;
    }

    private static PurchaseOrderRequest Order(Guid supplierId, Guid? projectId = null, decimal plywoodQty = 10m, decimal plywoodPrice = 1250m, bool withLaminate = true)
    {
        var lines = new List<PurchaseOrderLineRequest> { new(PlywoodId, plywoodQty, plywoodPrice) };
        if (withLaminate) lines.Add(new(LaminateId, 4m, 580m));
        return new PurchaseOrderRequest(supplierId, projectId, new DateOnly(2026, 11, 15), "ส่งหน้างาน", lines);
    }

    private async Task<PurchaseOrderResponse> OrderResult(HttpResponseMessage response, HttpStatusCode expected = HttpStatusCode.OK)
    {
        Assert.True(response.StatusCode == expected, $"Expected {expected} but got {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        return (await response.Content.ReadFromJsonAsync<PurchaseOrderResponse>())!;
    }

    private async Task<PurchaseOrderResponse> CreateOrderAsync(PurchaseOrderRequest request) =>
        await OrderResult(await SendAsync(HttpMethod.Post, "/api/v1/purchase-orders", request, key: Guid.NewGuid().ToString("N")), HttpStatusCode.Created);

    private async Task<PurchaseOrderResponse> ActAsync(PurchaseOrderResponse order, string action, string? note = null, string token = "token-org-a")
    {
        var response = await SendAsync(HttpMethod.Post, $"/api/v1/purchase-orders/{order.Id}/{action}", new PurchaseOrderActionRequest(note), token: token, ifMatch: order.RowVersion);
        return await OrderResult(response);
    }

    private async Task<PurchaseOrderResponse> ApprovedOrderAsync(PurchaseOrderRequest request)
    {
        var order = await CreateOrderAsync(request);
        order = await ActAsync(order, "submit");
        return await ActAsync(order, "approve", token: "token-approver");
    }

    private async Task<PurchaseOrderResponse> GetOrderAsync(Guid id) =>
        await OrderResult(await SendAsync(HttpMethod.Get, $"/api/v1/purchase-orders/{id}"));

    private async Task<HttpResponseMessage> ReceiveAsync(Guid orderId, string key, params (Guid LineId, decimal Qty)[] lines) =>
        await SendAsync(HttpMethod.Post, $"/api/v1/purchase-orders/{orderId}/receipts",
            new GoodsReceiptRequest(null, "รับของ", lines.Select(l => new GoodsReceiptLineRequest(l.LineId, l.Qty)).ToList()), key: key);


    private async Task<WarehouseResponse> CreateWarehouseAsync(string name = "คลังหลัก", string? key = null)
    {
        var response = await SendAsync(HttpMethod.Post, "/api/v1/warehouses", new WarehouseRequest(name, "กรุงเทพ"), key: key ?? Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<WarehouseResponse>())!;
    }

    private async Task<StockDocumentResponse> Doc(HttpResponseMessage response, HttpStatusCode expected = HttpStatusCode.Created)
    {
        Assert.True(response.StatusCode == expected, $"Expected {expected} but got {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        return (await response.Content.ReadFromJsonAsync<StockDocumentResponse>())!;
    }

    /// <summary>An approved purchase order that is fully received; returns the goods receipt id.</summary>
    private async Task<Guid> ReceivedGoodsReceiptAsync(decimal plywoodQty, decimal plywoodPrice)
    {
        var supplier = await CreateSupplierAsync();
        var order = await ApprovedOrderAsync(Order(supplier.Id, plywoodQty: plywoodQty, plywoodPrice: plywoodPrice, withLaminate: false));
        var received = await OrderResult(await ReceiveAsync(order.Id, Guid.NewGuid().ToString("N"), (order.Lines[0].Id, plywoodQty)), HttpStatusCode.Created);
        return received.Receipts.Single().Id;
    }

    private async Task<StockDocumentResponse> PutIntoStockAsync(Guid goodsReceiptId, Guid warehouseId, string? key = null) =>
        await Doc(await SendAsync(HttpMethod.Post, "/api/v1/inventory/receipts", new ReceiveGoodsReceiptRequest(goodsReceiptId, warehouseId), key: key ?? Guid.NewGuid().ToString("N")));

    private async Task<StockBalanceResponse?> BalanceAsync(Guid warehouseId, Guid itemId)
    {
        var response = await SendAsync(HttpMethod.Get, $"/api/v1/inventory/balances?warehouseId={warehouseId}&itemId={itemId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<StockBalanceListResponse>())!.Items.SingleOrDefault();
    }

    private async Task<HttpResponseMessage> IssueAsync(Guid warehouseId, decimal qty, Guid? projectId = null, string? key = null) =>
        await SendAsync(HttpMethod.Post, "/api/v1/inventory/issues", new IssueStockRequest(warehouseId, projectId, "เบิกใช้งาน", new List<StockLineRequest> { new(PlywoodId, qty) }), key: key ?? Guid.NewGuid().ToString("N"));

    private async Task AssertLedgerConsistentAsync()
    {
        var recon = (await (await SendAsync(HttpMethod.Get, "/api/v1/inventory/reconciliation")).Content.ReadFromJsonAsync<ReconciliationResponse>())!;
        Assert.Equal(0, recon.InconsistentCount);
        Assert.All(recon.Rows, r => Assert.True(r.BalanceOnHand >= 0));
    }

    [Fact]
    public async Task Warehouse_Lifecycle_Numbering_AndPermissions()
    {
        var created = await CreateWarehouseAsync(key: "warehouse-create-key-0001");
        Assert.StartsWith("WH-", created.Code);
        Assert.Equal("active", created.Status);

        var replay = await SendAsync(HttpMethod.Post, "/api/v1/warehouses", new WarehouseRequest("คลังหลัก", "กรุงเทพ"), key: "warehouse-create-key-0001");
        Assert.Equal(created.Id, (await replay.Content.ReadFromJsonAsync<WarehouseResponse>())!.Id);
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/warehouses", new WarehouseRequest("อื่น", null), key: "warehouse-create-key-0001")));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await SendAsync(HttpMethod.Post, "/api/v1/warehouses", new WarehouseRequest(" ", null), key: Guid.NewGuid().ToString("N"))).StatusCode);

        var updated = await SendAsync(HttpMethod.Put, $"/api/v1/warehouses/{created.Id}", new WarehouseRequest("คลังกลาง", null), ifMatch: created.RowVersion);
        var warehouse = (await updated.Content.ReadFromJsonAsync<WarehouseResponse>())!;
        Assert.Equal("คลังกลาง", warehouse.Name);
        Assert.Equal("WAREHOUSE_VERSION_CONFLICT", await ErrorCodeAsync(await SendAsync(HttpMethod.Put, $"/api/v1/warehouses/{created.Id}", new WarehouseRequest("x", null), ifMatch: created.RowVersion)));

        var off = (await (await SendAsync(HttpMethod.Post, $"/api/v1/warehouses/{created.Id}/deactivate", ifMatch: warehouse.RowVersion)).Content.ReadFromJsonAsync<WarehouseResponse>())!;
        Assert.Equal("inactive", off.Status);
        Assert.Equal("WAREHOUSE_INVALID_STATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/warehouses/{created.Id}/deactivate", ifMatch: off.RowVersion)));

        var list = (await (await SendAsync(HttpMethod.Get, "/api/v1/warehouses?status=inactive")).Content.ReadFromJsonAsync<WarehouseListResponse>())!;
        Assert.Single(list.Items);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await SendAsync(HttpMethod.Get, "/api/v1/warehouses?status=bogus")).StatusCode);

        Assert.Equal(HttpStatusCode.Forbidden, (await SendAsync(HttpMethod.Get, "/api/v1/warehouses", token: "token-no-perm", membership: MembershipNoPermId)).StatusCode);
        var foreign = await SendAsync(HttpMethod.Get, $"/api/v1/warehouses/{created.Id}", token: "token-org-b");
        Assert.True(foreign.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GoodsReceipt_PutIntoStock_CreatesLedgerAndBalance_OnlyOnce()
    {
        var warehouse = await CreateWarehouseAsync();
        var receiptId = await ReceivedGoodsReceiptAsync(10m, 1250m);

        var doc = await PutIntoStockAsync(receiptId, warehouse.Id, "stock-receipt-key-0001");
        Assert.StartsWith("SR", doc.Number);
        Assert.Equal("receipt", doc.DocumentType);
        Assert.Equal("goods_receipt", doc.SourceType);
        Assert.Equal(receiptId, doc.SourceId);
        var movement = Assert.Single(doc.Movements);
        Assert.Equal(10m, movement.QuantityDelta);
        Assert.Equal(1250m, movement.UnitCost);
        Assert.Equal(12500m, movement.ValueDelta);
        Assert.Equal(10m, movement.OnHandAfter);

        var balance = (await BalanceAsync(warehouse.Id, PlywoodId))!;
        Assert.Equal(10m, balance.OnHand);
        Assert.Equal(0m, balance.Reserved);
        Assert.Equal(1250m, balance.AverageCost);
        Assert.Equal(12500m, balance.TotalValue);

        // Same key replays; a different key cannot put the same receipt into stock again
        var replay = await Doc(await SendAsync(HttpMethod.Post, "/api/v1/inventory/receipts", new ReceiveGoodsReceiptRequest(receiptId, warehouse.Id), key: "stock-receipt-key-0001"));
        Assert.Equal(doc.Id, replay.Id);
        var again = await SendAsync(HttpMethod.Post, "/api/v1/inventory/receipts", new ReceiveGoodsReceiptRequest(receiptId, warehouse.Id), key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.Conflict, again.StatusCode);
        Assert.Equal("INVENTORY_RECEIPT_ALREADY_POSTED", await ErrorCodeAsync(again));
        Assert.Equal(10m, (await BalanceAsync(warehouse.Id, PlywoodId))!.OnHand);

        // The purchase order now shows which stock document took the receipt in
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var poId = (await db.GoodsReceipts.AsNoTracking().SingleAsync(r => r.Id == receiptId)).PurchaseOrderId;
            var order = await OrderResult(await SendAsync(HttpMethod.Get, $"/api/v1/purchase-orders/{poId}"));
            Assert.Equal(doc.Number, order.Receipts.Single().StockDocumentNumber);
        }

        var missing = await SendAsync(HttpMethod.Post, "/api/v1/inventory/receipts", new ReceiveGoodsReceiptRequest(Guid.NewGuid(), warehouse.Id), key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        var noPermission = await SendAsync(HttpMethod.Post, "/api/v1/inventory/receipts", new ReceiveGoodsReceiptRequest(receiptId, warehouse.Id), token: "token-no-perm", key: Guid.NewGuid().ToString("N"), membership: MembershipNoPermId);
        Assert.Equal(HttpStatusCode.Forbidden, noPermission.StatusCode);
        await AssertLedgerConsistentAsync();
    }

    [Fact]
    public async Task InactiveWarehouse_RefusesPostings_AndWarehouseWithStockCannotBeDeactivated()
    {
        var warehouse = await CreateWarehouseAsync();
        var receiptId = await ReceivedGoodsReceiptAsync(5m, 100m);
        await PutIntoStockAsync(receiptId, warehouse.Id);

        var current = (await (await SendAsync(HttpMethod.Get, $"/api/v1/warehouses/{warehouse.Id}")).Content.ReadFromJsonAsync<WarehouseResponse>())!;
        var blocked = await SendAsync(HttpMethod.Post, $"/api/v1/warehouses/{warehouse.Id}/deactivate", ifMatch: current.RowVersion);
        Assert.Equal(HttpStatusCode.Conflict, blocked.StatusCode);
        Assert.Equal("WAREHOUSE_HAS_STOCK", await ErrorCodeAsync(blocked));

        var empty = await CreateWarehouseAsync("คลังว่าง");
        var off = (await (await SendAsync(HttpMethod.Post, $"/api/v1/warehouses/{empty.Id}/deactivate", ifMatch: empty.RowVersion)).Content.ReadFromJsonAsync<WarehouseResponse>())!;
        Assert.Equal("inactive", off.Status);
        var secondReceipt = await ReceivedGoodsReceiptAsync(1m, 100m);
        var response = await SendAsync(HttpMethod.Post, "/api/v1/inventory/receipts", new ReceiveGoodsReceiptRequest(secondReceipt, empty.Id), key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("INVENTORY_WAREHOUSE_INACTIVE", await ErrorCodeAsync(response));
    }

    [Fact]
    public async Task MovingAverage_IssuesAtAverageCost_AndNeverGoNegative()
    {
        var warehouse = await CreateWarehouseAsync();
        await PutIntoStockAsync(await ReceivedGoodsReceiptAsync(10m, 1250m), warehouse.Id);
        await PutIntoStockAsync(await ReceivedGoodsReceiptAsync(10m, 1350m), warehouse.Id);

        var balance = (await BalanceAsync(warehouse.Id, PlywoodId))!;
        Assert.Equal(20m, balance.OnHand);
        Assert.Equal(26000m, balance.TotalValue);
        Assert.Equal(1300m, balance.AverageCost);

        var issue = await Doc(await IssueAsync(warehouse.Id, 5m, key: "issue-key-00000001"));
        Assert.StartsWith("SI", issue.Number);
        var movement = Assert.Single(issue.Movements);
        Assert.Equal(-5m, movement.QuantityDelta);
        Assert.Equal(1300m, movement.UnitCost);
        Assert.Equal(-6500m, movement.ValueDelta);
        Assert.Equal(15m, movement.OnHandAfter);

        // Replay does not issue twice
        var replay = await Doc(await IssueAsync(warehouse.Id, 5m, key: "issue-key-00000001"));
        Assert.Equal(issue.Id, replay.Id);
        Assert.Equal(15m, (await BalanceAsync(warehouse.Id, PlywoodId))!.OnHand);
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", await ErrorCodeAsync(await IssueAsync(warehouse.Id, 1m, key: "issue-key-00000001")));

        // More than is on hand is refused and leaves everything untouched
        var tooMuch = await IssueAsync(warehouse.Id, 16m);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, tooMuch.StatusCode);
        Assert.Equal("INVENTORY_INSUFFICIENT_STOCK", await ErrorCodeAsync(tooMuch));
        var after = (await BalanceAsync(warehouse.Id, PlywoodId))!;
        Assert.Equal(15m, after.OnHand);
        Assert.Equal(19500m, after.TotalValue);

        // The last units take the remaining value, leaving no rounding residue
        await Doc(await IssueAsync(warehouse.Id, 15m));
        var empty = (await BalanceAsync(warehouse.Id, PlywoodId))!;
        Assert.Equal(0m, empty.OnHand);
        Assert.Equal(0m, empty.TotalValue);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await IssueAsync(warehouse.Id, 0m)).StatusCode);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await SendAsync(HttpMethod.Post, "/api/v1/inventory/issues", new IssueStockRequest(warehouse.Id, null, null, new List<StockLineRequest> { new(PlywoodId, 1m), new(PlywoodId, 1m) }), key: Guid.NewGuid().ToString("N"))).StatusCode);
        await AssertLedgerConsistentAsync();
    }

    [Fact]
    public async Task Reservations_ReduceAvailableStock_AndAreConsumedByProjectIssues()
    {
        var warehouse = await CreateWarehouseAsync();
        await PutIntoStockAsync(await ReceivedGoodsReceiptAsync(10m, 100m), warehouse.Id);
        var projectId = await CreateProjectAsync();

        var reserveRequest = new ReserveStockRequest(warehouse.Id, PlywoodId, projectId, 8m, "สำหรับงานตู้");
        var reservation = (await (await SendAsync(HttpMethod.Post, "/api/v1/inventory/reservations", reserveRequest, key: "reserve-key-0001")).Content.ReadFromJsonAsync<ReservationResponse>())!;
        Assert.Equal(8m, reservation.Quantity);
        Assert.Equal("active", reservation.Status);

        var replay = (await (await SendAsync(HttpMethod.Post, "/api/v1/inventory/reservations", reserveRequest, key: "reserve-key-0001")).Content.ReadFromJsonAsync<ReservationResponse>())!;
        Assert.Equal(reservation.Id, replay.Id);
        var balance = (await BalanceAsync(warehouse.Id, PlywoodId))!;
        Assert.Equal(8m, balance.Reserved);
        Assert.Equal(2m, balance.Available);

        var overReserve = await SendAsync(HttpMethod.Post, "/api/v1/inventory/reservations", new ReserveStockRequest(warehouse.Id, PlywoodId, projectId, 3m, null), key: Guid.NewGuid().ToString("N"));
        Assert.Equal("INVENTORY_INSUFFICIENT_STOCK", await ErrorCodeAsync(overReserve));

        // Someone else can only issue what is not reserved
        var blocked = await IssueAsync(warehouse.Id, 3m);
        Assert.Equal("INVENTORY_INSUFFICIENT_STOCK", await ErrorCodeAsync(blocked));
        await Doc(await IssueAsync(warehouse.Id, 2m));

        // The project's own issue uses its reservation
        await Doc(await IssueAsync(warehouse.Id, 5m, projectId));
        var afterProjectIssue = (await BalanceAsync(warehouse.Id, PlywoodId))!;
        Assert.Equal(3m, afterProjectIssue.OnHand);
        Assert.Equal(3m, afterProjectIssue.Reserved);
        Assert.Equal(0m, afterProjectIssue.Available);

        var reservations = (await (await SendAsync(HttpMethod.Get, $"/api/v1/inventory/reservations?projectId={projectId}")).Content.ReadFromJsonAsync<ReservationListResponse>())!;
        Assert.Equal(3m, reservations.Items.Single().Quantity);

        var stale = await SendAsync(HttpMethod.Post, $"/api/v1/inventory/reservations/{reservation.Id}/release", new ReleaseReservationRequest(Guid.NewGuid()));
        Assert.Equal("INVENTORY_RESERVATION_VERSION_CONFLICT", await ErrorCodeAsync(stale));
        var current = reservations.Items.Single();
        var released = (await (await SendAsync(HttpMethod.Post, $"/api/v1/inventory/reservations/{reservation.Id}/release", new ReleaseReservationRequest(current.RowVersion))).Content.ReadFromJsonAsync<ReservationResponse>())!;
        Assert.Equal("released", released.Status);
        var final = (await BalanceAsync(warehouse.Id, PlywoodId))!;
        Assert.Equal(0m, final.Reserved);
        Assert.Equal(3m, final.Available);

        var again = await SendAsync(HttpMethod.Post, $"/api/v1/inventory/reservations/{reservation.Id}/release", new ReleaseReservationRequest(released.RowVersion));
        Assert.Equal("INVENTORY_RESERVATION_INVALID", await ErrorCodeAsync(again));
        await AssertLedgerConsistentAsync();
    }

    [Fact]
    public async Task Transfers_AreAtomic_AndKeepTheValue()
    {
        var from = await CreateWarehouseAsync("คลังต้นทาง");
        var to = await CreateWarehouseAsync("คลังปลายทาง");
        await PutIntoStockAsync(await ReceivedGoodsReceiptAsync(10m, 1250m), from.Id);
        await PutIntoStockAsync(await ReceivedGoodsReceiptAsync(3m, 1000m), from.Id); // avg = (12500+3000)/13

        var sourceBefore = (await BalanceAsync(from.Id, PlywoodId))!;
        var valueBefore = sourceBefore.TotalValue;

        var doc = await Doc(await SendAsync(HttpMethod.Post, "/api/v1/inventory/transfers",
            new TransferStockRequest(from.Id, to.Id, "ย้ายคลัง", new List<StockLineRequest> { new(PlywoodId, 4m) }), key: "transfer-key-0001"));
        Assert.StartsWith("ST", doc.Number);
        Assert.Equal(2, doc.Movements.Count);
        Assert.Contains(doc.Movements, m => m.Kind == "transfer_out" && m.QuantityDelta == -4m);
        Assert.Contains(doc.Movements, m => m.Kind == "transfer_in" && m.QuantityDelta == 4m);

        var source = (await BalanceAsync(from.Id, PlywoodId))!;
        var target = (await BalanceAsync(to.Id, PlywoodId))!;
        Assert.Equal(9m, source.OnHand);
        Assert.Equal(4m, target.OnHand);
        Assert.Equal(valueBefore, source.TotalValue + target.TotalValue);

        // Insufficient source stock changes neither side
        var tooMuch = await SendAsync(HttpMethod.Post, "/api/v1/inventory/transfers", new TransferStockRequest(from.Id, to.Id, null, new List<StockLineRequest> { new(PlywoodId, 50m) }), key: Guid.NewGuid().ToString("N"));
        Assert.Equal("INVENTORY_INSUFFICIENT_STOCK", await ErrorCodeAsync(tooMuch));
        Assert.Equal(9m, (await BalanceAsync(from.Id, PlywoodId))!.OnHand);
        Assert.Equal(4m, (await BalanceAsync(to.Id, PlywoodId))!.OnHand);

        var same = await SendAsync(HttpMethod.Post, "/api/v1/inventory/transfers", new TransferStockRequest(from.Id, from.Id, null, new List<StockLineRequest> { new(PlywoodId, 1m) }), key: Guid.NewGuid().ToString("N"));
        Assert.Equal("INVENTORY_TRANSFER_SAME_WAREHOUSE", await ErrorCodeAsync(same));
        var replay = await Doc(await SendAsync(HttpMethod.Post, "/api/v1/inventory/transfers", new TransferStockRequest(from.Id, to.Id, "ย้ายคลัง", new List<StockLineRequest> { new(PlywoodId, 4m) }), key: "transfer-key-0001"));
        Assert.Equal(doc.Id, replay.Id);
        Assert.Equal(9m, (await BalanceAsync(from.Id, PlywoodId))!.OnHand);
        await AssertLedgerConsistentAsync();
    }

    [Fact]
    public async Task Adjustments_FollowCounts_RequireReason_AndCostForEmptyBalances()
    {
        var warehouse = await CreateWarehouseAsync();
        await PutIntoStockAsync(await ReceivedGoodsReceiptAsync(10m, 200m), warehouse.Id);

        Assert.Equal("INVENTORY_REASON_REQUIRED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/inventory/adjustments", new AdjustStockRequest(warehouse.Id, " ", new List<AdjustmentLineRequest> { new(PlywoodId, 7m, null) }), key: Guid.NewGuid().ToString("N"))));
        Assert.Equal("INVENTORY_NO_CHANGE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/inventory/adjustments", new AdjustStockRequest(warehouse.Id, "นับสต็อก", new List<AdjustmentLineRequest> { new(PlywoodId, 10m, null) }), key: Guid.NewGuid().ToString("N"))));

        var down = await Doc(await SendAsync(HttpMethod.Post, "/api/v1/inventory/adjustments", new AdjustStockRequest(warehouse.Id, "ของหาย", new List<AdjustmentLineRequest> { new(PlywoodId, 7m, null) }), key: "adjust-key-00000001"));
        Assert.StartsWith("SA", down.Number);
        Assert.Equal("adjustment_out", down.Movements.Single().Kind);
        Assert.Equal(-3m, down.Movements.Single().QuantityDelta);
        Assert.Equal(1400m, (await BalanceAsync(warehouse.Id, PlywoodId))!.TotalValue);

        var up = await Doc(await SendAsync(HttpMethod.Post, "/api/v1/inventory/adjustments", new AdjustStockRequest(warehouse.Id, "พบของเพิ่ม", new List<AdjustmentLineRequest> { new(PlywoodId, 9m, null) }), key: Guid.NewGuid().ToString("N")));
        Assert.Equal("adjustment_in", up.Movements.Single().Kind);
        Assert.Equal(200m, up.Movements.Single().UnitCost);
        Assert.Equal(1800m, (await BalanceAsync(warehouse.Id, PlywoodId))!.TotalValue);

        // Count below what is reserved is refused
        var projectId = await CreateProjectAsync();
        await SendAsync(HttpMethod.Post, "/api/v1/inventory/reservations", new ReserveStockRequest(warehouse.Id, PlywoodId, projectId, 6m, null), key: Guid.NewGuid().ToString("N"));
        var belowReserved = await SendAsync(HttpMethod.Post, "/api/v1/inventory/adjustments", new AdjustStockRequest(warehouse.Id, "นับใหม่", new List<AdjustmentLineRequest> { new(PlywoodId, 2m, null) }), key: Guid.NewGuid().ToString("N"));
        Assert.Equal("INVENTORY_INSUFFICIENT_STOCK", await ErrorCodeAsync(belowReserved));

        // An empty balance has no average, so adding stock needs a cost
        var other = await CreateWarehouseAsync("คลังใหม่");
        Assert.Equal("INVENTORY_COST_REQUIRED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/inventory/adjustments", new AdjustStockRequest(other.Id, "ยกยอด", new List<AdjustmentLineRequest> { new(PlywoodId, 4m, null) }), key: Guid.NewGuid().ToString("N"))));
        var seeded = await Doc(await SendAsync(HttpMethod.Post, "/api/v1/inventory/adjustments", new AdjustStockRequest(other.Id, "ยกยอด", new List<AdjustmentLineRequest> { new(PlywoodId, 4m, 150m) }), key: Guid.NewGuid().ToString("N")));
        Assert.Equal(600m, seeded.Movements.Single().ValueDelta);
        await AssertLedgerConsistentAsync();
    }

    [Fact]
    public async Task ConcurrentIssues_NeverOversell_AndTheLedgerStaysConsistent()
    {
        var warehouse = await CreateWarehouseAsync();
        await PutIntoStockAsync(await ReceivedGoodsReceiptAsync(10m, 100m), warehouse.Id);

        // Six simultaneous issues of 3 against 10 units: at most three can win.
        var responses = await Task.WhenAll(Enumerable.Range(0, 6).Select(_ => IssueAsync(warehouse.Id, 3m)));
        var succeeded = responses.Count(r => r.StatusCode == HttpStatusCode.Created);
        var refused = responses.Where(r => r.StatusCode != HttpStatusCode.Created).ToList();

        Assert.Equal(3, succeeded);
        Assert.Equal(3, refused.Count);
        foreach (var response in refused)
        {
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            Assert.Equal("INVENTORY_INSUFFICIENT_STOCK", await ErrorCodeAsync(response));
        }

        var balance = (await BalanceAsync(warehouse.Id, PlywoodId))!;
        Assert.Equal(1m, balance.OnHand);
        Assert.Equal(100m, balance.TotalValue);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var ledger = await db.StockMovements.AsNoTracking().Where(m => m.WarehouseId == warehouse.Id && m.ItemId == PlywoodId).ToListAsync();
        Assert.Equal(1m, ledger.Sum(m => m.QuantityDelta));
        Assert.All(ledger, m => Assert.True(m.OnHandAfter >= 0));
        await AssertLedgerConsistentAsync();
    }

    [Fact]
    public async Task Movements_AreListedAndFilteredFromTheLedger()
    {
        var warehouse = await CreateWarehouseAsync();
        var receiptDoc = await PutIntoStockAsync(await ReceivedGoodsReceiptAsync(10m, 100m), warehouse.Id);
        await Doc(await IssueAsync(warehouse.Id, 4m));

        var all = (await (await SendAsync(HttpMethod.Get, $"/api/v1/inventory/movements?warehouseId={warehouse.Id}")).Content.ReadFromJsonAsync<StockMovementListResponse>())!;
        Assert.Equal(2, all.Pagination.TotalCount);
        Assert.Equal("issue", all.Items[0].Kind);

        var issues = (await (await SendAsync(HttpMethod.Get, $"/api/v1/inventory/movements?kind=issue&itemId={PlywoodId}")).Content.ReadFromJsonAsync<StockMovementListResponse>())!;
        Assert.Single(issues.Items);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await SendAsync(HttpMethod.Get, "/api/v1/inventory/movements?kind=bogus")).StatusCode);

        var document = (await (await SendAsync(HttpMethod.Get, $"/api/v1/inventory/documents/{receiptDoc.Id}")).Content.ReadFromJsonAsync<StockDocumentResponse>())!;
        Assert.Equal(receiptDoc.Number, document.Number);
        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Get, $"/api/v1/inventory/documents/{Guid.NewGuid()}")).StatusCode);

        var inStock = (await (await SendAsync(HttpMethod.Get, $"/api/v1/inventory/balances?inStockOnly=true&search={Uri.EscapeDataString("ไม้อัด")}")).Content.ReadFromJsonAsync<StockBalanceListResponse>())!;
        Assert.Single(inStock.Items);
        Assert.Equal(600m, inStock.TotalValue);
        Assert.Equal(HttpStatusCode.Forbidden, (await SendAsync(HttpMethod.Get, "/api/v1/inventory/balances", token: "token-no-perm", membership: MembershipNoPermId)).StatusCode);
    }
}
