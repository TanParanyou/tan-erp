using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
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

public class ProcurementEndpointsTests : IAsyncLifetime
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
    private const string UidNoPerm = "uid-no-procurement-perm";
    private static readonly Guid MembershipNoPermId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b62");

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

    [Fact]
    public async Task Supplier_CreateUpdateDeactivate_Lifecycle_WithIdempotencyAndConcurrency()
    {
        var created = await CreateSupplierAsync(key: "supplier-create-key-0001");
        Assert.StartsWith("SUP-", created.Code);
        Assert.Equal("active", created.Status);

        var replay = await SendAsync(HttpMethod.Post, "/api/v1/suppliers", new SupplierRequest("บริษัท ไม้ดี จำกัด", "Good Wood", "0105500000001", "คุณขาย", "021234567", "sales@example.test", 30), key: "supplier-create-key-0001");
        Assert.Equal(created.Id, (await replay.Content.ReadFromJsonAsync<SupplierResponse>())!.Id);
        var reused = await SendAsync(HttpMethod.Post, "/api/v1/suppliers", new SupplierRequest("อื่น", null, null, null, null, null, 0), key: "supplier-create-key-0001");
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", await ErrorCodeAsync(reused));

        var invalid = await SendAsync(HttpMethod.Post, "/api/v1/suppliers", new SupplierRequest(" ", null, null, null, null, null, 30), key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, invalid.StatusCode);
        var badTerm = await SendAsync(HttpMethod.Post, "/api/v1/suppliers", new SupplierRequest("ก", null, null, null, null, null, 400), key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, badTerm.StatusCode);

        var updated = await SendAsync(HttpMethod.Put, $"/api/v1/suppliers/{created.Id}", new SupplierRequest("บริษัท ไม้ดีมาก จำกัด", null, null, null, null, null, 45), ifMatch: created.RowVersion);
        var supplier = (await updated.Content.ReadFromJsonAsync<SupplierResponse>())!;
        Assert.Equal(45, supplier.PaymentTermDays);

        var stale = await SendAsync(HttpMethod.Put, $"/api/v1/suppliers/{created.Id}", new SupplierRequest("ก", null, null, null, null, null, 1), ifMatch: created.RowVersion);
        Assert.Equal("SUPPLIER_VERSION_CONFLICT", await ErrorCodeAsync(stale));

        var off = await SendAsync(HttpMethod.Post, $"/api/v1/suppliers/{created.Id}/deactivate", ifMatch: supplier.RowVersion);
        var inactive = (await off.Content.ReadFromJsonAsync<SupplierResponse>())!;
        Assert.Equal("inactive", inactive.Status);
        var again = await SendAsync(HttpMethod.Post, $"/api/v1/suppliers/{created.Id}/deactivate", ifMatch: inactive.RowVersion);
        Assert.Equal("SUPPLIER_INVALID_STATE", await ErrorCodeAsync(again));

        var list = (await (await SendAsync(HttpMethod.Get, $"/api/v1/suppliers?status=inactive&search={Uri.EscapeDataString("ไม้ดี")}")).Content.ReadFromJsonAsync<SupplierListResponse>())!;
        Assert.Single(list.Items);
        var badFilter = await SendAsync(HttpMethod.Get, "/api/v1/suppliers?status=bogus");
        Assert.Equal(HttpStatusCode.UnprocessableEntity, badFilter.StatusCode);

        var denied = await SendAsync(HttpMethod.Get, "/api/v1/suppliers", token: "token-no-perm", membership: MembershipNoPermId);
        Assert.Equal(HttpStatusCode.Forbidden, denied.StatusCode);
        var foreign = await SendAsync(HttpMethod.Get, $"/api/v1/suppliers/{created.Id}", token: "token-org-b");
        Assert.True(foreign.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PurchaseOrder_Validation_DraftEditing_AndMakerCheckerApproval()
    {
        var supplier = await CreateSupplierAsync();

        var noLines = await SendAsync(HttpMethod.Post, "/api/v1/purchase-orders", new PurchaseOrderRequest(supplier.Id, null, null, null, new List<PurchaseOrderLineRequest>()), key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, noLines.StatusCode);
        var duplicate = await SendAsync(HttpMethod.Post, "/api/v1/purchase-orders", new PurchaseOrderRequest(supplier.Id, null, null, null, new List<PurchaseOrderLineRequest> { new(PlywoodId, 1m, 1m), new(PlywoodId, 2m, 1m) }), key: Guid.NewGuid().ToString("N"));
        Assert.Equal("PURCHASE_ORDER_LINE_INVALID", await ErrorCodeAsync(duplicate));
        var zeroQty = await SendAsync(HttpMethod.Post, "/api/v1/purchase-orders", new PurchaseOrderRequest(supplier.Id, null, null, null, new List<PurchaseOrderLineRequest> { new(PlywoodId, 0m, 1m) }), key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, zeroQty.StatusCode);
        var unknownItem = await SendAsync(HttpMethod.Post, "/api/v1/purchase-orders", new PurchaseOrderRequest(supplier.Id, null, null, null, new List<PurchaseOrderLineRequest> { new(Guid.NewGuid(), 1m, 1m) }), key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.NotFound, unknownItem.StatusCode);

        var order = await CreateOrderAsync(Order(supplier.Id));
        Assert.StartsWith("PO", order.Number);
        Assert.Equal("draft", order.Status);
        Assert.Equal(10m * 1250m + 4m * 580m, order.TotalAmount);
        Assert.Equal(2, order.Lines.Count);
        Assert.Equal("TEST-MAT-PLY-MR18", order.Lines[0].ItemCode);

        var edited = await OrderResult(await SendAsync(HttpMethod.Put, $"/api/v1/purchase-orders/{order.Id}", Order(supplier.Id, plywoodQty: 2m, withLaminate: false), ifMatch: order.RowVersion));
        Assert.Single(edited.Lines);
        Assert.Equal(2500m, edited.TotalAmount);

        var stale = await SendAsync(HttpMethod.Put, $"/api/v1/purchase-orders/{order.Id}", Order(supplier.Id), ifMatch: order.RowVersion);
        Assert.Equal("PURCHASE_ORDER_VERSION_CONFLICT", await ErrorCodeAsync(stale));

        var submitted = await ActAsync(edited, "submit");
        var editSubmitted = await SendAsync(HttpMethod.Put, $"/api/v1/purchase-orders/{order.Id}", Order(supplier.Id), ifMatch: submitted.RowVersion);
        Assert.Equal("PURCHASE_ORDER_INVALID_STATE", await ErrorCodeAsync(editSubmitted));

        var self = await SendAsync(HttpMethod.Post, $"/api/v1/purchase-orders/{order.Id}/approve", new PurchaseOrderActionRequest(null), ifMatch: submitted.RowVersion);
        Assert.Equal(HttpStatusCode.Forbidden, self.StatusCode);
        Assert.Equal("PURCHASE_ORDER_SELF_APPROVAL", await ErrorCodeAsync(self));

        var noNote = await SendAsync(HttpMethod.Post, $"/api/v1/purchase-orders/{order.Id}/reject", new PurchaseOrderActionRequest(" "), token: "token-approver", ifMatch: submitted.RowVersion);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, noNote.StatusCode);

        var approved = await ActAsync(submitted, "approve", "ตกลง", "token-approver");
        Assert.Equal("approved", approved.Status);
        Assert.Equal(ApproverUserId, approved.DecidedBy!.Id);

        var rejected = await CreateOrderAsync(Order(supplier.Id));
        rejected = await ActAsync(await ActAsync(rejected, "submit"), "reject", "ราคาสูง", "token-approver");
        Assert.Equal("rejected", rejected.Status);

        var noReason = await SendAsync(HttpMethod.Post, $"/api/v1/purchase-orders/{approved.Id}/cancel", new PurchaseOrderActionRequest(null), ifMatch: approved.RowVersion);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, noReason.StatusCode);
        var cancelled = await ActAsync(approved, "cancel", "ยกเลิกงาน");
        Assert.Equal("cancelled", cancelled.Status);
        Assert.Equal("ยกเลิกงาน", cancelled.CancelReason);
    }

    [Fact]
    public async Task InactiveSupplier_BlocksNewAndApprovedOrders()
    {
        var supplier = await CreateSupplierAsync();
        var order = await ActAsync(await CreateOrderAsync(Order(supplier.Id)), "submit");

        var off = await SendAsync(HttpMethod.Post, $"/api/v1/suppliers/{supplier.Id}/deactivate", ifMatch: supplier.RowVersion);
        Assert.Equal(HttpStatusCode.OK, off.StatusCode);

        var create = await SendAsync(HttpMethod.Post, "/api/v1/purchase-orders", Order(supplier.Id), key: Guid.NewGuid().ToString("N"));
        Assert.Equal("PURCHASE_ORDER_SUPPLIER_INACTIVE", await ErrorCodeAsync(create));
        var approve = await SendAsync(HttpMethod.Post, $"/api/v1/purchase-orders/{order.Id}/approve", new PurchaseOrderActionRequest(null), token: "token-approver", ifMatch: order.RowVersion);
        Assert.Equal("PURCHASE_ORDER_SUPPLIER_INACTIVE", await ErrorCodeAsync(approve));
    }

    [Fact]
    public async Task GoodsReceipts_AllowPartialReceiving_RefuseOverReceipt_AndStayIdempotent()
    {
        var supplier = await CreateSupplierAsync();

        var draft = await CreateOrderAsync(Order(supplier.Id));
        var early = await ReceiveAsync(draft.Id, "receipt-draft-key-0001", (draft.Lines[0].Id, 1m));
        Assert.Equal("PURCHASE_ORDER_INVALID_STATE", await ErrorCodeAsync(early));

        var order = await ApprovedOrderAsync(Order(supplier.Id));
        var plywood = order.Lines.Single(l => l.ItemCode == "TEST-MAT-PLY-MR18");
        var laminate = order.Lines.Single(l => l.ItemCode == "TEST-MAT-LAM-WHITE");

        var partial = await OrderResult(await ReceiveAsync(order.Id, "receipt-key-0001", (plywood.Id, 6m)), HttpStatusCode.Created);
        Assert.Equal("partially_received", partial.Status);
        Assert.Equal(6m, partial.Lines.Single(l => l.Id == plywood.Id).ReceivedQuantity);
        Assert.Equal(4m, partial.Lines.Single(l => l.Id == plywood.Id).RemainingQuantity);
        var receipt = Assert.Single(partial.Receipts);
        Assert.StartsWith("GR", receipt.Number);

        // Same key + payload replays without receiving twice
        var replay = await OrderResult(await ReceiveAsync(order.Id, "receipt-key-0001", (plywood.Id, 6m)), HttpStatusCode.Created);
        Assert.Equal(6m, replay.Lines.Single(l => l.Id == plywood.Id).ReceivedQuantity);
        Assert.Single(replay.Receipts);
        var reused = await ReceiveAsync(order.Id, "receipt-key-0001", (plywood.Id, 1m));
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", await ErrorCodeAsync(reused));

        // Over-receipt is refused and nothing is written
        var over = await ReceiveAsync(order.Id, "receipt-key-0002", (plywood.Id, 4.5m), (laminate.Id, 1m));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, over.StatusCode);
        Assert.Equal("GOODS_RECEIPT_OVER_RECEIVED", await ErrorCodeAsync(over));
        var afterOver = await GetOrderAsync(order.Id);
        Assert.Equal(6m, afterOver.Lines.Single(l => l.Id == plywood.Id).ReceivedQuantity);
        Assert.Equal(0m, afterOver.Lines.Single(l => l.Id == laminate.Id).ReceivedQuantity);
        Assert.Single(afterOver.Receipts);

        var invalidLine = await ReceiveAsync(order.Id, "receipt-key-0003", (Guid.NewGuid(), 1m));
        Assert.Equal("GOODS_RECEIPT_INVALID", await ErrorCodeAsync(invalidLine));
        var zero = await ReceiveAsync(order.Id, "receipt-key-0004", (plywood.Id, 0m));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, zero.StatusCode);

        var cancelAfterReceipt = await SendAsync(HttpMethod.Post, $"/api/v1/purchase-orders/{order.Id}/cancel", new PurchaseOrderActionRequest("เลิก"), ifMatch: afterOver.RowVersion);
        Assert.Equal("PURCHASE_ORDER_HAS_RECEIPTS", await ErrorCodeAsync(cancelAfterReceipt));

        var complete = await OrderResult(await ReceiveAsync(order.Id, "receipt-key-0005", (plywood.Id, 4m), (laminate.Id, 4m)), HttpStatusCode.Created);
        Assert.Equal("received", complete.Status);
        Assert.Equal(2, complete.Receipts.Count);
        Assert.All(complete.Lines, l => Assert.Equal(0m, l.RemainingQuantity));

        var afterComplete = await ReceiveAsync(order.Id, "receipt-key-0006", (plywood.Id, 1m));
        Assert.Equal("PURCHASE_ORDER_INVALID_STATE", await ErrorCodeAsync(afterComplete));

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(2, await db.GoodsReceipts.CountAsync(r => r.PurchaseOrderId == order.Id));
        Assert.Equal(3, await db.GoodsReceiptLines.CountAsync(l => l.OrganizationId == OrgId));
    }

    [Fact]
    public async Task ProjectOrders_AreBudgetControlled_AndCommitProjectBudget()
    {
        var supplier = await CreateSupplierAsync();

        // A planned project has no budget commitment capacity
        var plannedProjectId = await CreateProjectAsync();
        var plannedOrder = await ActAsync(await CreateOrderAsync(Order(supplier.Id, plannedProjectId)), "submit");
        var notActive = await SendAsync(HttpMethod.Post, $"/api/v1/purchase-orders/{plannedOrder.Id}/approve", new PurchaseOrderActionRequest(null), token: "token-approver", ifMatch: plannedOrder.RowVersion);
        Assert.Equal("PURCHASE_ORDER_PROJECT_NOT_ACTIVE", await ErrorCodeAsync(notActive));

        var projectId = await CreateProjectAsync();
        var active = await ActivateAsync(projectId, 20000m);
        Assert.Equal(0m, active.Budget.CommittedAmount);

        var firstOrder = await CreateOrderAsync(Order(supplier.Id, projectId, plywoodQty: 10m, withLaminate: false)); // 12,500
        var approved = await ActAsync(await ActAsync(firstOrder, "submit"), "approve", token: "token-approver");
        Assert.Equal("approved", approved.Status);
        Assert.Equal(projectId, approved.Project!.Id);

        var control = await GetControlAsync(projectId);
        Assert.Equal(12500m, control.Budget.CommittedAmount);
        Assert.Equal(7500m, control.Budget.AvailableBudget);

        // 12,500 + 8,750 would exceed 20,000
        var tooBig = await ActAsync(await CreateOrderAsync(Order(supplier.Id, projectId, plywoodQty: 7m, withLaminate: false)), "submit");
        var over = await SendAsync(HttpMethod.Post, $"/api/v1/purchase-orders/{tooBig.Id}/approve", new PurchaseOrderActionRequest(null), token: "token-approver", ifMatch: tooBig.RowVersion);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, over.StatusCode);
        Assert.Equal("PURCHASE_ORDER_OVER_BUDGET", await ErrorCodeAsync(over));

        // Cancelling the first order releases its commitment; an approved change order raises the budget
        await ActAsync(approved, "cancel", "เปลี่ยนแผน");
        Assert.Equal(0m, (await GetControlAsync(projectId)).Budget.CommittedAmount);

        var fits = await ActAsync(tooBig, "approve", token: "token-approver");
        Assert.Equal("approved", fits.Status);
        Assert.Equal(8750m, (await GetControlAsync(projectId)).Budget.CommittedAmount);

        var listed = (await (await SendAsync(HttpMethod.Get, $"/api/v1/purchase-orders?projectId={projectId}&status=approved")).Content.ReadFromJsonAsync<PurchaseOrderListResponse>())!;
        Assert.Single(listed.Items);
        Assert.Equal(approved.Project.Code, listed.Items[0].ProjectCode);
    }

    [Fact]
    public async Task Procurement_RequiresPermission_AndIsolatesOrganizations()
    {
        var supplier = await CreateSupplierAsync();
        var order = await CreateOrderAsync(Order(supplier.Id));

        var denied = await SendAsync(HttpMethod.Post, "/api/v1/purchase-orders", Order(supplier.Id), token: "token-no-perm", key: Guid.NewGuid().ToString("N"), membership: MembershipNoPermId);
        Assert.Equal(HttpStatusCode.Forbidden, denied.StatusCode);
        var deniedReceipt = await SendAsync(HttpMethod.Post, $"/api/v1/purchase-orders/{order.Id}/receipts", new GoodsReceiptRequest(null, null, new List<GoodsReceiptLineRequest> { new(order.Lines[0].Id, 1m) }), token: "token-no-perm", key: Guid.NewGuid().ToString("N"), membership: MembershipNoPermId);
        Assert.Equal(HttpStatusCode.Forbidden, deniedReceipt.StatusCode);

        var foreign = await SendAsync(HttpMethod.Get, $"/api/v1/purchase-orders/{order.Id}", token: "token-org-b");
        Assert.True(foreign.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Forbidden);
        var foreignList = await SendAsync(HttpMethod.Get, "/api/v1/purchase-orders", token: "token-org-b");
        if (foreignList.StatusCode == HttpStatusCode.OK)
        {
            Assert.Empty((await foreignList.Content.ReadFromJsonAsync<PurchaseOrderListResponse>())!.Items);
        }
    }
}
