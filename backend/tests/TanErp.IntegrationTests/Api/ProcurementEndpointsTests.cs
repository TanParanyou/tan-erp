using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.Finance;
using TanErp.Api.Contracts.Inventory;
using TanErp.Api.Contracts.Procurement;
using TanErp.Api.Contracts.Projects;
using TanErp.Api.ErrorHandling;
using TanErp.Application.IdentityAccess.ApprovalLimits;
using TanErp.Domain.Commercial;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Estimates;
using TanErp.Domain.Finance;
using TanErp.Domain.Notifications;
using TanErp.Domain.Projects;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using TanErp.IntegrationTests.Support;
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
    public async Task ApproveOrder_EnforcesApprovalAuthority_AndRejectNeedsNoLimit()
    {
        var supplier = await CreateSupplierAsync();
        var order = await ActAsync(await CreateOrderAsync(Order(supplier.Id)), "submit");
        var toReject = await ActAsync(await CreateOrderAsync(Order(supplier.Id)), "submit");
        Task<HttpResponseMessage> Approve() => SendAsync(HttpMethod.Post, $"/api/v1/purchase-orders/{order.Id}/approve",
            new PurchaseOrderActionRequest(null), token: "token-approver", ifMatch: order.RowVersion);
        Task SetLimitAsync(bool isUnlimited, decimal? maxAmount, bool isActive = true) =>
            ApprovalLimitTestSupport.SetLimitAsync(_factory.Services, OrgId, "Test Admin",
                ApprovalDocumentType.PurchaseOrder, isUnlimited, maxAmount, isActive);

        try
        {
            await SetLimitAsync(false, order.TotalAmount - 0.01m);
            var exceeded = await Approve();
            Assert.Equal(HttpStatusCode.Forbidden, exceeded.StatusCode);
            Assert.Equal("APPROVAL_LIMIT_EXCEEDED", await ErrorCodeAsync(exceeded));

            await SetLimitAsync(false, order.TotalAmount, isActive: false);
            Assert.Equal("APPROVAL_AUTHORITY_NOT_CONFIGURED", await ErrorCodeAsync(await Approve()));
            Assert.Equal("rejected", (await ActAsync(toReject, "reject", "ราคาสูง", "token-approver")).Status);

            await SetLimitAsync(false, order.TotalAmount);
            Assert.Equal("approved", (await ActAsync(order, "approve", token: "token-approver")).Status);
        }
        finally
        {
            // Other tests in this class approve as Test Admin, so restore its seeded unlimited limit.
            await SetLimitAsync(true, null);
        }
    }

    [Fact]
    public async Task ApprovalAuthority_UsesTheHighestCeilingAcrossActiveRoles_AndIgnoresOtherCurrencies()
    {
        Guid secondRoleId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var second = new Role(Guid.NewGuid(), OrgId, "Second Approver Role", "TEST_ONLY second approver role", isActive: true);
            db.Roles.Add(second);
            db.MembershipRoles.Add(new MembershipRole(ApproverMembershipId, second.Id, OrgId));
            db.ApprovalLimits.Add(ApprovalLimit.Create(Guid.NewGuid(), OrgId, second.Id, ApprovalDocumentType.PurchaseOrder,
                null, ApprovalLimitRules.Currency, false, 50_000m, DateTimeOffset.UtcNow));
            await db.SaveChangesAsync();
            secondRoleId = second.Id;
        }

        async Task<string?> CheckAsync(decimal amount, string currency = "THB")
        {
            using var scope = _factory.Services.CreateScope();
            var error = await scope.ServiceProvider.GetRequiredService<IApprovalAuthority>().EnsureAsync(new ApprovalAuthorityCheck(
                OrgId, ApproverMembershipId, ApprovalDocumentType.PurchaseOrder, BranchId, amount, currency), CancellationToken.None);
            return error?.Code;
        }

        try
        {
            await ApprovalLimitTestSupport.SetLimitAsync(_factory.Services, OrgId, "Test Admin",
                ApprovalDocumentType.PurchaseOrder, false, 1_000m);

            Assert.Null(await CheckAsync(20_000m));                                      // the second role's higher ceiling wins
            Assert.Equal("APPROVAL_LIMIT_EXCEEDED", await CheckAsync(50_000.01m));
            Assert.Equal("APPROVAL_AUTHORITY_NOT_CONFIGURED", await CheckAsync(1m, "USD")); // no limit in another currency

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                (await db.Roles.SingleAsync(r => r.Id == secondRoleId)).Deactivate();
                await db.SaveChangesAsync();
            }

            Assert.Equal("APPROVAL_LIMIT_EXCEEDED", await CheckAsync(20_000m));          // an inactive role grants nothing
        }
        finally
        {
            await ApprovalLimitTestSupport.SetLimitAsync(_factory.Services, OrgId, "Test Admin",
                ApprovalDocumentType.PurchaseOrder, true, null);
        }
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

    [Fact]
    public async Task PurchaseRequest_Lifecycle_MakerChecker_Notification_AndConsolidationToPurchaseOrder()
    {
        var supplier = await CreateSupplierAsync("บริษัท วัสดุรวม จำกัด");
        var projectA = await CreateProjectAsync();
        await ActivateAsync(projectA, 100_000m);
        var projectB = await CreateProjectAsync();
        await ActivateAsync(projectB, 100_000m);

        // Create PR1 on Project A -> draft -> edit draft (keeping overlapping items) -> submit -> Maker!=Checker -> approve
        var createPr1 = await SendAsync(
            HttpMethod.Post,
            "/api/v1/purchase-requests",
            new PurchaseRequestRequest(
                projectA,
                new DateOnly(2026, 11, 10),
                "ขอซื้อรอบแรก",
                new List<PurchaseRequestLineRequest>
                {
                    new(PlywoodId, 4m, 1200m, "แผ่นหลัก"),
                    new(LaminateId, 2m, 580m, "ปิดผิว")
                }),
            key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.Created, createPr1.StatusCode);
        var pr1 = (await createPr1.Content.ReadFromJsonAsync<PurchaseRequestResponse>())!;
        Assert.StartsWith("PR-", pr1.Number);
        Assert.Equal("draft", pr1.Status);

        var updatePr1 = await SendAsync(
            HttpMethod.Put,
            $"/api/v1/purchase-requests/{pr1.Id}",
            new PurchaseRequestRequest(
                projectA,
                new DateOnly(2026, 11, 12),
                "ขอซื้อรอบแรก (ปรับจำนวน)",
                new List<PurchaseRequestLineRequest>
                {
                    new(PlywoodId, 6m, 1200m, "แผ่นหลัก"),
                    new(LaminateId, 2m, 580m, "ปิดผิว")
                }),
            ifMatch: pr1.RowVersion);
        Assert.Equal(HttpStatusCode.OK, updatePr1.StatusCode);
        pr1 = (await updatePr1.Content.ReadFromJsonAsync<PurchaseRequestResponse>())!;
        Assert.Equal(6m * 1200m + 2m * 580m, pr1.TotalEstimatedAmount);

        var submitPr1 = await SendAsync(HttpMethod.Post, $"/api/v1/purchase-requests/{pr1.Id}/submit", ifMatch: pr1.RowVersion);
        Assert.Equal(HttpStatusCode.OK, submitPr1.StatusCode);
        pr1 = (await submitPr1.Content.ReadFromJsonAsync<PurchaseRequestResponse>())!;
        Assert.Equal("submitted", pr1.Status);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var rows = await db.Notifications.AsNoTracking()
                .Where(n => n.OrganizationId == OrgId && n.Type == NotificationTypes.PurchaseRequestApprovalRequested)
                .Select(n => new { n.RecipientUserId, n.PayloadJson })
                .ToListAsync();
            var recipients = rows.Where(n => n.PayloadJson.Contains(pr1.Number, StringComparison.Ordinal))
                .Select(n => n.RecipientUserId)
                .ToList();
            Assert.Contains(ApproverUserId, recipients);
            Assert.DoesNotContain(UserId, recipients);
        }

        var selfApprove = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/purchase-requests/{pr1.Id}/approve",
            new PurchaseRequestActionRequest(null),
            token: "token-org-a",
            ifMatch: pr1.RowVersion);
        Assert.Equal(HttpStatusCode.Forbidden, selfApprove.StatusCode);
        Assert.Equal("PURCHASE_REQUEST_SELF_APPROVAL", await ErrorCodeAsync(selfApprove));

        var approvePr1 = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/purchase-requests/{pr1.Id}/approve",
            new PurchaseRequestActionRequest("อนุมัติใบที่ 1"),
            token: "token-approver",
            ifMatch: pr1.RowVersion);
        Assert.Equal(HttpStatusCode.OK, approvePr1.StatusCode);
        pr1 = (await approvePr1.Content.ReadFromJsonAsync<PurchaseRequestResponse>())!;
        Assert.Equal("approved", pr1.Status);

        // Create & approve PR2 on same Project A with overlapping Plywood @ 1300
        var pr2Resp = await SendAsync(
            HttpMethod.Post,
            "/api/v1/purchase-requests",
            new PurchaseRequestRequest(
                projectA,
                new DateOnly(2026, 11, 10),
                "ขอซื้อเพิ่มเติม",
                new List<PurchaseRequestLineRequest> { new(PlywoodId, 4m, 1300m, null) }),
            key: Guid.NewGuid().ToString("N"));
        var pr2 = (await pr2Resp.Content.ReadFromJsonAsync<PurchaseRequestResponse>())!;
        pr2 = (await (await SendAsync(HttpMethod.Post, $"/api/v1/purchase-requests/{pr2.Id}/submit", ifMatch: pr2.RowVersion)).Content.ReadFromJsonAsync<PurchaseRequestResponse>())!;
        pr2 = (await (await SendAsync(HttpMethod.Post, $"/api/v1/purchase-requests/{pr2.Id}/approve", new PurchaseRequestActionRequest("อนุมัติใบที่ 2"), token: "token-approver", ifMatch: pr2.RowVersion)).Content.ReadFromJsonAsync<PurchaseRequestResponse>())!;

        // Create & approve PR3 on Project B -> cross-project consolidation must fail with 409 PURCHASE_REQUEST_CONVERSION_INCOMPATIBLE
        var pr3Resp = await SendAsync(
            HttpMethod.Post,
            "/api/v1/purchase-requests",
            new PurchaseRequestRequest(
                projectB,
                null,
                "อีกโครงการ",
                new List<PurchaseRequestLineRequest> { new(PlywoodId, 2m, 1200m, null) }),
            key: Guid.NewGuid().ToString("N"));
        var pr3 = (await pr3Resp.Content.ReadFromJsonAsync<PurchaseRequestResponse>())!;
        pr3 = (await (await SendAsync(HttpMethod.Post, $"/api/v1/purchase-requests/{pr3.Id}/submit", ifMatch: pr3.RowVersion)).Content.ReadFromJsonAsync<PurchaseRequestResponse>())!;
        pr3 = (await (await SendAsync(HttpMethod.Post, $"/api/v1/purchase-requests/{pr3.Id}/approve", new PurchaseRequestActionRequest("อนุมัติใบที่ 3"), token: "token-approver", ifMatch: pr3.RowVersion)).Content.ReadFromJsonAsync<PurchaseRequestResponse>())!;

        var incompatible = await SendAsync(
            HttpMethod.Post,
            "/api/v1/purchase-requests/convert",
            new ConvertPurchaseRequestsRequest(new List<Guid> { pr1.Id, pr3.Id }, supplier.Id, null, null, null),
            key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.Conflict, incompatible.StatusCode);
        Assert.Equal("PURCHASE_REQUEST_CONVERSION_INCOMPATIBLE", await ErrorCodeAsync(incompatible));

        // Consolidate PR1 + PR2 into one draft PO (weighted average Plywood = (6*1200 + 4*1300)/10 = 1240; override Laminate = 600)
        var convertResp = await SendAsync(
            HttpMethod.Post,
            "/api/v1/purchase-requests/convert",
            new ConvertPurchaseRequestsRequest(
                new List<Guid> { pr1.Id, pr2.Id },
                supplier.Id,
                null,
                "รวมใบขอซื้อ PR1+PR2",
                new List<PurchaseRequestUnitPriceOverrideRequest> { new(LaminateId, 600m) }),
            key: Guid.NewGuid().ToString("N"));
        var po = await OrderResult(convertResp, HttpStatusCode.Created);
        Assert.Equal("draft", po.Status);
        Assert.Equal(projectA, po.Project!.Id);
        Assert.Equal(new DateOnly(2026, 11, 10), po.ExpectedDeliveryDate);
        Assert.Equal(2, po.SourcePurchaseRequests.Count);

        var poPlywood = po.Lines.Single(l => l.ItemId == PlywoodId);
        var poLaminate = po.Lines.Single(l => l.ItemId == LaminateId);
        Assert.Equal(10m, poPlywood.Quantity);
        Assert.Equal(1240m, poPlywood.UnitPrice);
        Assert.Equal(2m, poLaminate.Quantity);
        Assert.Equal(600m, poLaminate.UnitPrice);

        var pr1After = (await (await SendAsync(HttpMethod.Get, $"/api/v1/purchase-requests/{pr1.Id}")).Content.ReadFromJsonAsync<PurchaseRequestResponse>())!;
        Assert.Equal("converted", pr1After.Status);
        Assert.Equal(po.Id, pr1After.ConvertedPurchaseOrder!.Id);

        // Re-converting an already-converted PR fails with 409 PURCHASE_REQUEST_INVALID_STATE
        var reconvert = await SendAsync(
            HttpMethod.Post,
            "/api/v1/purchase-requests/convert",
            new ConvertPurchaseRequestsRequest(new List<Guid> { pr1.Id }, supplier.Id, null, null, null),
            key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.Conflict, reconvert.StatusCode);
        Assert.Equal("PURCHASE_REQUEST_INVALID_STATE", await ErrorCodeAsync(reconvert));
    }

    [Fact]
    public async Task SupplierReturn_DeductsWarehouseStockAtomically_RestoresProjectCommitment_AndGuardsInsufficientStock()
    {
        var supplier = await CreateSupplierAsync("บริษัท คลังไม้ จำกัด");
        var projectId = await CreateProjectAsync();
        await ActivateAsync(projectId, 50_000m);

        var whResp = await SendAsync(HttpMethod.Post, "/api/v1/warehouses", new WarehouseRequest("คลังคืนสินค้า", "กรุงเทพ"), key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.Created, whResp.StatusCode);
        var warehouse = (await whResp.Content.ReadFromJsonAsync<WarehouseResponse>())!;

        var order = await ApprovedOrderAsync(Order(supplier.Id, projectId, plywoodQty: 10m, plywoodPrice: 1000m, withLaminate: false));
        var poLine = order.Lines.Single();

        // Receive all 10 units and put into warehouse stock
        var receivedPo = await OrderResult(await ReceiveAsync(order.Id, Guid.NewGuid().ToString("N"), (poLine.Id, 10m)), HttpStatusCode.Created);
        var gr = Assert.Single(receivedPo.Receipts);
        var grLine = Assert.Single(gr.Lines);

        var putResp = await SendAsync(HttpMethod.Post, "/api/v1/inventory/receipts", new ReceiveGoodsReceiptRequest(gr.Id, warehouse.Id), key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.Created, putResp.StatusCode);

        // Over-returning 11 > 10 fails with 422 SUPPLIER_RETURN_OVER_RETURNED
        var overReturn = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/purchase-orders/{order.Id}/returns",
            new SupplierReturnRequest(gr.Id, null, "คืนเกิน", new List<SupplierReturnLineRequest> { new(grLine.Id, 11m) }),
            key: Guid.NewGuid().ToString("N"),
            ifMatch: receivedPo.RowVersion);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, overReturn.StatusCode);
        Assert.Equal("SUPPLIER_RETURN_OVER_RETURNED", await ErrorCodeAsync(overReturn));

        // Return 3 defective units -> atomic return_out stock deduction + +3,000 commitment restoration
        var returnResp = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/purchase-orders/{order.Id}/returns",
            new SupplierReturnRequest(gr.Id, null, "ไม้บิ่นมุมจากการขนส่ง", new List<SupplierReturnLineRequest> { new(grLine.Id, 3m) }),
            key: Guid.NewGuid().ToString("N"),
            ifMatch: receivedPo.RowVersion);
        var afterReturn = await OrderResult(returnResp, HttpStatusCode.Created);
        Assert.Equal("partially_received", afterReturn.Status);
        Assert.Equal(7m, afterReturn.Lines.Single().ReceivedQuantity);
        Assert.Equal(3m, afterReturn.Lines.Single().RemainingQuantity);
        Assert.Equal(3m, afterReturn.Receipts.Single().Lines.Single().ReturnedQuantity);
        Assert.Equal(7m, afterReturn.Receipts.Single().Lines.Single().RemainingReturnableQuantity);

        var srn = Assert.Single(afterReturn.Returns);
        Assert.StartsWith("SRN-", srn.Number);
        Assert.Equal(3000m, srn.TotalReturnAmount);
        Assert.False(string.IsNullOrWhiteSpace(srn.StockDocumentNumber));

        var balResp = await SendAsync(HttpMethod.Get, $"/api/v1/inventory/balances?warehouseId={warehouse.Id}&itemId={PlywoodId}");
        var balance = (await balResp.Content.ReadFromJsonAsync<StockBalanceListResponse>())!.Items.Single();
        Assert.Equal(7m, balance.OnHand);

        var invRecon = (await (await SendAsync(HttpMethod.Get, "/api/v1/inventory/reconciliation")).Content.ReadFromJsonAsync<ReconciliationResponse>())!;
        Assert.Equal(0, invRecon.InconsistentCount);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var srnLedger = await db.ProjectCostLedgerEntries.AsNoTracking()
                .SingleAsync(e => e.OrganizationId == OrgId && e.ProjectId == projectId && e.SourceType == ProjectCostSourceType.SupplierReturn);
            Assert.Equal(ProjectCostEntryType.Commitment, srnLedger.EntryType);
            Assert.Equal(3000m, srnLedger.Amount);
            Assert.True(srnLedger.IsReversal);
        }

        // Issue 6 units from warehouse so OnHand = 1, then try returning 2 units -> fails with 422 INVENTORY_INSUFFICIENT_STOCK and rolls back cleanly
        var issueResp = await SendAsync(
            HttpMethod.Post,
            "/api/v1/inventory/issues",
            new IssueStockRequest(warehouse.Id, projectId, "เบิกใช้งานหน้างาน", new List<StockLineRequest> { new(PlywoodId, 6m) }),
            key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.Created, issueResp.StatusCode);

        var insufficientReturn = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/purchase-orders/{order.Id}/returns",
            new SupplierReturnRequest(gr.Id, null, "จะคืนแต่เบิกไปแล้ว", new List<SupplierReturnLineRequest> { new(grLine.Id, 2m) }),
            key: Guid.NewGuid().ToString("N"),
            ifMatch: afterReturn.RowVersion);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, insufficientReturn.StatusCode);
        Assert.Equal("INVENTORY_INSUFFICIENT_STOCK", await ErrorCodeAsync(insufficientReturn));

        var unchangedPo = await GetOrderAsync(order.Id);
        Assert.Equal(7m, unchangedPo.Lines.Single().ReceivedQuantity);
        Assert.Single(unchangedPo.Returns);
    }

    [Fact]
    public async Task SupplierInvoice_ThreeWayMatch_HoldApproval_Outbox_PriceVarianceLedger_AndVoid()
    {
        var supplier = await CreateSupplierAsync("บริษัท วางบิลทดสอบ จำกัด");
        var projectId = await CreateProjectAsync();
        await ActivateAsync(projectId, 50_000m);

        var order = await ApprovedOrderAsync(Order(supplier.Id, projectId, plywoodQty: 10m, plywoodPrice: 1000m, withLaminate: false));
        var poLine = order.Lines.Single();

        // Receive 6 units
        var receivedPo = await OrderResult(await ReceiveAsync(order.Id, Guid.NewGuid().ToString("N"), (poLine.Id, 6m)), HttpStatusCode.Created);
        var gr = Assert.Single(receivedPo.Receipts);
        var grLine = Assert.Single(gr.Lines);

        // Case B: Create SINV1 with 7 units (> 6 received) -> submit -> 422 SUPPLIER_INVOICE_OVER_RECEIVED
        var createSinv1 = await SendAsync(
            HttpMethod.Post,
            "/api/v1/supplier-invoices",
            new SupplierInvoiceRequest(
                order.Id,
                "TAX-INV-0001",
                new DateOnly(2026, 11, 20),
                new DateOnly(2026, 12, 20),
                490m,
                "ใบกำกับใบแรก",
                new List<SupplierInvoiceLineRequest> { new(poLine.Id, 7m, 1000m) }),
            key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.Created, createSinv1.StatusCode);
        var sinv1 = (await createSinv1.Content.ReadFromJsonAsync<SupplierInvoiceResponse>())!;
        Assert.StartsWith("SINV-", sinv1.Number);

        var overSubmit = await SendAsync(HttpMethod.Post, $"/api/v1/supplier-invoices/{sinv1.Id}/submit", ifMatch: sinv1.RowVersion);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, overSubmit.StatusCode);
        Assert.Equal("SUPPLIER_INVOICE_OVER_RECEIVED", await ErrorCodeAsync(overSubmit));

        // Case A: Edit SINV1 to 4 units @ 1015 (+1.5% <= 2% tolerance) -> submit -> matched + outbox + +60 variance in cost ledger
        var updateSinv1 = await SendAsync(
            HttpMethod.Put,
            $"/api/v1/supplier-invoices/{sinv1.Id}",
            new SupplierInvoiceRequest(
                order.Id,
                "TAX-INV-0001",
                new DateOnly(2026, 11, 20),
                new DateOnly(2026, 12, 20),
                284.20m,
                "แก้จำนวนตามของรับจริง",
                new List<SupplierInvoiceLineRequest> { new(poLine.Id, 4m, 1015m) }),
            ifMatch: sinv1.RowVersion);
        Assert.Equal(HttpStatusCode.OK, updateSinv1.StatusCode);
        sinv1 = (await updateSinv1.Content.ReadFromJsonAsync<SupplierInvoiceResponse>())!;

        var matchSubmit = await SendAsync(HttpMethod.Post, $"/api/v1/supplier-invoices/{sinv1.Id}/submit", ifMatch: sinv1.RowVersion);
        Assert.Equal(HttpStatusCode.OK, matchSubmit.StatusCode);
        sinv1 = (await matchSubmit.Content.ReadFromJsonAsync<SupplierInvoiceResponse>())!;
        Assert.Equal("matched", sinv1.Status);
        Assert.Equal("matched", sinv1.MatchStatus);
        Assert.Equal(60m, sinv1.TotalVarianceAmount);

        var poAfterSinv1 = await GetOrderAsync(order.Id);
        Assert.Equal(4m, poAfterSinv1.Lines.Single().InvoicedQuantity);
        Assert.Equal(2m, poAfterSinv1.Lines.Single().UninvoicedReceivedQuantity);

        // Returning 3 units when 6 received and 4 invoiced (6 - 3 = 3 < 4) must be blocked by SUPPLIER_RETURN_ALREADY_INVOICED
        var returnInvoiced = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/purchase-orders/{order.Id}/returns",
            new SupplierReturnRequest(gr.Id, null, "จะคืนของที่วางบิลแล้ว", new List<SupplierReturnLineRequest> { new(grLine.Id, 3m) }),
            key: Guid.NewGuid().ToString("N"),
            ifMatch: poAfterSinv1.RowVersion);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, returnInvoiced.StatusCode);
        Assert.Equal("SUPPLIER_RETURN_ALREADY_INVOICED", await ErrorCodeAsync(returnInvoiced));

        // Duplicate supplierInvoiceNumber for same supplier while active -> 409 SUPPLIER_INVOICE_DUPLICATE_NUMBER
        var dupInv = await SendAsync(
            HttpMethod.Post,
            "/api/v1/supplier-invoices",
            new SupplierInvoiceRequest(
                order.Id,
                "tax-inv-0001",
                new DateOnly(2026, 11, 21),
                null,
                0m,
                null,
                new List<SupplierInvoiceLineRequest> { new(poLine.Id, 1m, 1000m) }),
            key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.Conflict, dupInv.StatusCode);
        Assert.Equal("SUPPLIER_INVOICE_DUPLICATE_NUMBER", await ErrorCodeAsync(dupInv));

        // Case C: SINV2 for remaining 2 units @ 1050 (+5% > 2%) -> on_hold -> notification -> self-approve 403 -> approve -> outbox + +100 variance -> void -> reversal
        var createSinv2 = await SendAsync(
            HttpMethod.Post,
            "/api/v1/supplier-invoices",
            new SupplierInvoiceRequest(
                order.Id,
                "TAX-INV-0002",
                new DateOnly(2026, 11, 22),
                new DateOnly(2026, 12, 22),
                147m,
                "ราคาขึ้น 5%",
                new List<SupplierInvoiceLineRequest> { new(poLine.Id, 2m, 1050m) }),
            key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.Created, createSinv2.StatusCode);
        var sinv2 = (await createSinv2.Content.ReadFromJsonAsync<SupplierInvoiceResponse>())!;

        var holdSubmit = await SendAsync(HttpMethod.Post, $"/api/v1/supplier-invoices/{sinv2.Id}/submit", ifMatch: sinv2.RowVersion);
        Assert.Equal(HttpStatusCode.OK, holdSubmit.StatusCode);
        sinv2 = (await holdSubmit.Content.ReadFromJsonAsync<SupplierInvoiceResponse>())!;
        Assert.Equal("on_hold", sinv2.Status);
        Assert.Equal("price_variance", sinv2.MatchStatus);
        Assert.Equal(100m, sinv2.TotalVarianceAmount);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var rows = await db.Notifications.AsNoTracking()
                .Where(n => n.OrganizationId == OrgId && n.Type == NotificationTypes.SupplierInvoiceApprovalRequested)
                .Select(n => new { n.RecipientUserId, n.PayloadJson })
                .ToListAsync();
            var holdNotifRecipients = rows.Where(n => n.PayloadJson.Contains(sinv2.Number, StringComparison.Ordinal))
                .Select(n => n.RecipientUserId)
                .ToList();
            Assert.Contains(ApproverUserId, holdNotifRecipients);
            Assert.DoesNotContain(UserId, holdNotifRecipients);

            // While on_hold, no AP outbox message has been queued for SINV2 yet
            Assert.False(await db.AccountingOutbox.AsNoTracking()
                .AnyAsync(o => o.OrganizationId == OrgId && o.ResourceId == sinv2.Id));
        }

        var selfApproveHold = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/supplier-invoices/{sinv2.Id}/approve",
            new SupplierInvoiceActionRequest(null),
            token: "token-org-a",
            ifMatch: sinv2.RowVersion);
        Assert.Equal(HttpStatusCode.Forbidden, selfApproveHold.StatusCode);
        Assert.Equal("SUPPLIER_INVOICE_SELF_APPROVAL", await ErrorCodeAsync(selfApproveHold));

        var approveHold = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/supplier-invoices/{sinv2.Id}/approve",
            new SupplierInvoiceActionRequest("อนุมัติรับราคาใหม่"),
            token: "token-approver",
            ifMatch: sinv2.RowVersion);
        Assert.Equal(HttpStatusCode.OK, approveHold.StatusCode);
        sinv2 = (await approveHold.Content.ReadFromJsonAsync<SupplierInvoiceResponse>())!;
        Assert.Equal("approved", sinv2.Status);

        // Void SINV2 -> queues supplier-invoice.voided, releases 2 invoiced units, reverses -100m variance, and frees TAX-INV-0002
        var voidResp = await SendAsync(
            HttpMethod.Post,
            $"/api/v1/supplier-invoices/{sinv2.Id}/void",
            new SupplierInvoiceActionRequest("ผู้ขายออกใบใหม่แทน"),
            ifMatch: sinv2.RowVersion);
        Assert.Equal(HttpStatusCode.OK, voidResp.StatusCode);
        sinv2 = (await voidResp.Content.ReadFromJsonAsync<SupplierInvoiceResponse>())!;
        Assert.Equal("voided", sinv2.Status);

        var poAfterVoid = await GetOrderAsync(order.Id);
        Assert.Equal(4m, poAfterVoid.Lines.Single().InvoicedQuantity);
        Assert.Equal(2m, poAfterVoid.Lines.Single().UninvoicedReceivedQuantity);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var outboxKinds = await db.AccountingOutbox.AsNoTracking()
                .Where(o => o.OrganizationId == OrgId && (o.ResourceId == sinv1.Id || o.ResourceId == sinv2.Id))
                .OrderBy(o => o.CreatedAtUtc)
                .Select(o => new { o.ResourceId, o.Kind })
                .ToListAsync();
            Assert.Contains(outboxKinds, x => x.ResourceId == sinv1.Id && x.Kind == OutboxKind.SupplierInvoicePosted);
            Assert.Contains(outboxKinds, x => x.ResourceId == sinv2.Id && x.Kind == OutboxKind.SupplierInvoicePosted);
            Assert.Contains(outboxKinds, x => x.ResourceId == sinv2.Id && x.Kind == OutboxKind.SupplierInvoiceVoided);

            var sinvLedger = await db.ProjectCostLedgerEntries.AsNoTracking()
                .Where(e => e.OrganizationId == OrgId && e.ProjectId == projectId && e.SourceType == ProjectCostSourceType.SupplierInvoice)
                .ToListAsync();
            Assert.Equal(3, sinvLedger.Count);
            Assert.Equal(60m, sinvLedger.Sum(e => e.Amount)); // +60 (SINV1) + 100 (SINV2) - 100 (SINV2 void) = +60 net
        }

        var finReconResp = await SendAsync(HttpMethod.Get, "/api/v1/finance/reconciliation");
        Assert.Equal(HttpStatusCode.OK, finReconResp.StatusCode);
        var finRecon = (await finReconResp.Content.ReadFromJsonAsync<FinanceReconciliationResponse>())!;
        Assert.DoesNotContain(finRecon.Rows, r => r.Issue == "missing_message");

        // Reusing TAX-INV-0002 after void is now allowed
        var reuseVoidedNo = await SendAsync(
            HttpMethod.Post,
            "/api/v1/supplier-invoices",
            new SupplierInvoiceRequest(
                order.Id,
                "TAX-INV-0002",
                new DateOnly(2026, 11, 23),
                null,
                140m,
                "ออกใบใหม่แทนใบที่ Void",
                new List<SupplierInvoiceLineRequest> { new(poLine.Id, 2m, 1000m) }),
            key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.Created, reuseVoidedNo.StatusCode);
    }
}
