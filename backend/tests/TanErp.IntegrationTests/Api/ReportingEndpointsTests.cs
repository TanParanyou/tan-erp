using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api.Contracts.Reporting;
using TanErp.Api.ErrorHandling;
using TanErp.Domain.Commercial;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Estimates;
using TanErp.Domain.Finance;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Inventory;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;
using TanErp.Domain.Projects;
using TanErp.Domain.Service;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class ReportingEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private static readonly Guid OrgId = TestOnlyDataSeeder.TestOrgId;
    private static readonly Guid Branch1Id = TestOnlyDataSeeder.TestBranchId;
    private static readonly Guid Branch2Id = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4e22");
    private static readonly Guid UserId = TestOnlyDataSeeder.TestUserId;
    private const string UidA = TestOnlyDataSeeder.TestFirebaseUid;

    private const string UidBranch2 = "uid-branch-2-user";
    private static readonly Guid UserBranch2Id = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4e20");
    private static readonly Guid MembershipBranch2Id = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4e21");

    private const string UidOrgWide = "uid-orgwide-user";
    private static readonly Guid UserOrgWideId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4e30");
    private static readonly Guid MembershipOrgWideId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4e31");

    private const string UidNoPerm = "uid-no-reports-perm";
    private static readonly Guid UserNoPermId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4e40");
    private static readonly Guid MembershipNoPermId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4e41");

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(idToken switch
            {
                "token-org-a" => UidA,
                "token-branch-2" => UidBranch2,
                "token-orgwide" => UidOrgWide,
                "token-no-perm" => UidNoPerm,
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

        // Branch 2 in same Organization
        var branch2 = new Branch(Branch2Id, OrgId, "B02", "สาขาที่ 2");
        db.Branches.Add(branch2);

        // Admin role for permissions
        var adminRole = await db.Roles.SingleAsync(r => r.OrganizationId == OrgId && r.Name == "Test Admin");

        // User Branch 2
        var userB2 = new User(UserBranch2Id, UidBranch2, "b2@example.com", "Branch 2 User", true);
        db.Users.Add(userB2);
        var memberB2 = new Membership(MembershipBranch2Id, OrgId, Branch2Id, UserBranch2Id, isActive: true);
        db.Memberships.Add(memberB2);
        db.MembershipRoles.Add(new MembershipRole(MembershipBranch2Id, adminRole.Id, OrgId));

        // User Org-Wide (BranchId is null)
        var userOrgWide = new User(UserOrgWideId, UidOrgWide, "orgwide@example.com", "Org-wide User", true);
        db.Users.Add(userOrgWide);
        var memberOrgWide = new Membership(MembershipOrgWideId, OrgId, null, UserOrgWideId, isActive: true);
        db.Memberships.Add(memberOrgWide);
        db.MembershipRoles.Add(new MembershipRole(MembershipOrgWideId, adminRole.Id, OrgId));

        // User No Perm (active membership but zero roles/permissions)
        var userNoPerm = new User(UserNoPermId, UidNoPerm, "noperm@example.com", "No Perm User", true);
        db.Users.Add(userNoPerm);
        var memberNoPerm = new Membership(MembershipNoPermId, OrgId, Branch1Id, UserNoPermId, isActive: true);
        db.Memberships.Add(memberNoPerm);

        await db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private static HttpRequestMessage Request(
        HttpMethod method,
        string url,
        string token = "token-org-a",
        Guid? membership = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var membershipId = membership ?? token switch
        {
            "token-branch-2" => MembershipBranch2Id,
            "token-orgwide" => MembershipOrgWideId,
            "token-no-perm" => MembershipNoPermId,
            _ => TestOnlyDataSeeder.TestMembershipId
        };
        request.Headers.Add("X-Membership-Id", membershipId.ToString());
        return request;
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string url,
        string token = "token-org-a",
        Guid? membership = null)
    {
        var request = Request(method, url, token, membership);
        return await _client.SendAsync(request);
    }

    private static async Task<string> ErrorCodeAsync(HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<ApiProblemDetails>())!.Code;

    // ---------------------------------------------------------------------------------------------
    // Test 1: Hand-Calculated Fixture Verification
    // ---------------------------------------------------------------------------------------------

    [Fact]
    public async Task AllReports_And_DashboardSummary_ReturnExpectedHandCalculatedMetrics()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTimeOffset.UtcNow;
        var asOf = DateOnly.FromDateTime(DateTime.UtcNow);

        // 1. Customer & Site
        var customer = Customer.CreateDraft(
            Guid.NewGuid(), OrgId, UserId, CustomerType.Person, "ลูกค้าทดสอบรายงาน", null, "th",
            new PrimaryContactInput("สมชาย", null, "0812345678", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        db.Customers.Add(customer);

        var site = Site.CreateActive(
            Guid.NewGuid(), OrgId, customer.Id, UserId, "ไซต์ทดสอบ",
            new SiteAddressInput("100 ถ.สาทร", "สาทร", "สาทร", "กรุงเทพฯ", "10120", "TH"), 13.7m, 100.5m, null, now);
        db.Sites.Add(site);

        // 2. Opportunities (Branch 1)
        // Opp 1: won 20,000
        var opp1 = Opportunity.CreateDraft(Guid.NewGuid(), OrgId, Branch1Id, customer.Id, site.Id, UserId, UserId, "งาน A", "desc", [OpportunityWorkType.BuiltIn], null, 20000m, "THB", asOf, now, "note", now);
        opp1.Qualify(opp1.RowVersion);
        opp1.EnterSurveying(opp1.RowVersion, site.Id);
        opp1.EnterEstimating(opp1.RowVersion);
        opp1.EnterProposed(opp1.RowVersion);
        opp1.MarkWon(opp1.RowVersion);
        db.Opportunities.Add(opp1);

        // Opp 2: won 30,000
        var opp2 = Opportunity.CreateDraft(Guid.NewGuid(), OrgId, Branch1Id, customer.Id, site.Id, UserId, UserId, "งาน B", "desc", [OpportunityWorkType.BuiltIn], null, 30000m, "THB", asOf, now, "note", now);
        opp2.Qualify(opp2.RowVersion);
        opp2.EnterSurveying(opp2.RowVersion, site.Id);
        opp2.EnterEstimating(opp2.RowVersion);
        opp2.EnterProposed(opp2.RowVersion);
        opp2.MarkWon(opp2.RowVersion);
        db.Opportunities.Add(opp2);

        // Opp 3: lost 10,000
        var opp3 = Opportunity.CreateDraft(Guid.NewGuid(), OrgId, Branch1Id, customer.Id, site.Id, UserId, UserId, "งาน C", "desc", [OpportunityWorkType.BuiltIn], null, 10000m, "THB", asOf, now, "note", now);
        opp3.Qualify(opp3.RowVersion);
        opp3.Close(opp3.RowVersion, OpportunityStage.Lost, OpportunityReasonCodes.LostPriceTooHigh, "แพงเกินไป");
        db.Opportunities.Add(opp3);

        // Opp 4: proposed 15,000
        var opp4 = Opportunity.CreateDraft(Guid.NewGuid(), OrgId, Branch1Id, customer.Id, site.Id, UserId, UserId, "งาน D", "desc", [OpportunityWorkType.BuiltIn], null, 15000m, "THB", asOf, now, "note", now);
        opp4.Qualify(opp4.RowVersion);
        opp4.EnterSurveying(opp4.RowVersion, site.Id);
        opp4.EnterEstimating(opp4.RowVersion);
        opp4.EnterProposed(opp4.RowVersion);
        db.Opportunities.Add(opp4);

        // 3. Quotations (Branch 1)
        var est1 = Estimate.CreateDraft(Guid.NewGuid(), OrgId, Branch1Id, customer.Id, opp1.Id, "EST-REP-01", null, null);
        db.Estimates.Add(est1);
        var q1 = new Quotation(Guid.NewGuid(), OrgId, Branch1Id, customer.Id, opp1.Id, est1.Id, est1.CurrentRevision!.Id, "QT-REP-01", 40000m, "hash1", now);
        db.Quotations.Add(q1); // status: issued

        var est2 = Estimate.CreateDraft(Guid.NewGuid(), OrgId, Branch1Id, customer.Id, opp2.Id, "EST-REP-02", null, null);
        db.Estimates.Add(est2);
        var q2 = new Quotation(Guid.NewGuid(), OrgId, Branch1Id, customer.Id, opp2.Id, est2.Id, est2.CurrentRevision!.Id, "QT-REP-02", 60000m, "hash2", now);
        q2.Accept(now); // status: accepted
        db.Quotations.Add(q2);

        var est3 = Estimate.CreateDraft(Guid.NewGuid(), OrgId, Branch1Id, customer.Id, opp2.Id, "EST-REP-03", null, null);
        db.Estimates.Add(est3);
        var q3 = new Quotation(Guid.NewGuid(), OrgId, Branch1Id, customer.Id, opp2.Id, est3.Id, est3.CurrentRevision!.Id, "QT-REP-03", 50000m, "hash3", now);
        q3.Accept(now); // status: accepted
        db.Quotations.Add(q3);

        // 4. Projects & Budget (Branch 1)
        // Project 1: Baseline 100k, CO approved 20k -> Current 120k, Committed 30k, Actual 40k -> Consumed 70k, Variance 50k, OverBudget = false
        var proj1 = Project.CreateFromHandover(
            Guid.NewGuid(), OrgId, Branch1Id, customer.Id, site.Id, opp1.Id, q2.Id, est1.Id, est1.CurrentRevision.Id,
            null, "PRJ-REP-01", "โครงการ 1", UserId, asOf, "QT-REP-02", 120000m, "hash2", null, "base-hash-1", UserId, now);
        proj1.SetPlan(asOf, asOf.AddMonths(2), now);
        proj1.SetBaselineBudget(100000m, "budget-hash-1", now);
        proj1.TransitionTo(ProjectStatus.Active, null, 1, now);
        db.Projects.Add(proj1);

        var bl1 = new ProjectBudgetLine(Guid.NewGuid(), OrgId, proj1.Id, ProjectBudgetCategory.Material, "ค่าวัสดุ", 100000m, 1);
        db.ProjectBudgetLines.Add(bl1);

        var co1 = new ProjectChangeOrder(Guid.NewGuid(), OrgId, proj1.Id, "CO-01", "เพิ่มวัสดุ", "ความต้องการลูกค้า", 20000m, 20000m, UserId, now);
        co1.Submit(now);
        co1.Decide(true, UserBranch2Id, "อนุมัติ", now);
        db.ProjectChangeOrders.Add(co1);

        var cleCommit = new ProjectCostLedgerEntry(
            Guid.NewGuid(), OrgId, proj1.Id, null, ProjectBudgetCategory.Material,
            ProjectCostEntryType.Commitment, ProjectCostSourceType.PurchaseOrder,
            Guid.NewGuid(), "PO-01", "สั่งซื้อวัสดุ", 30000m, false, UserId, now);

        var cleActual = new ProjectCostLedgerEntry(
            Guid.NewGuid(), OrgId, proj1.Id, null, ProjectBudgetCategory.Material,
            ProjectCostEntryType.Actual, ProjectCostSourceType.GoodsReceipt,
            Guid.NewGuid(), "GR-01", "รับของเข้า", 40000m, false, UserId, now);

        db.ProjectCostLedgerEntries.AddRange(cleCommit, cleActual);

        // Project 2: Baseline 50k, Current 50k, Actual 60k -> Consumed 60k, Variance -10k, OverBudget = true
        var proj2 = Project.CreateFromHandover(
            Guid.NewGuid(), OrgId, Branch1Id, customer.Id, site.Id, opp2.Id, q3.Id, est3.Id, est3.CurrentRevision.Id,
            null, "PRJ-REP-02", "โครงการ 2", UserId, asOf, "QT-REP-03", 50000m, "hash3", null, "base-hash-2", UserId, now);
        proj2.SetPlan(asOf, asOf.AddMonths(2), now);
        proj2.SetBaselineBudget(50000m, "budget-hash-2", now);
        proj2.TransitionTo(ProjectStatus.Active, null, 1, now);
        db.Projects.Add(proj2);

        var bl2 = new ProjectBudgetLine(Guid.NewGuid(), OrgId, proj2.Id, ProjectBudgetCategory.Material, "ค่าวัสดุ", 50000m, 1);
        db.ProjectBudgetLines.Add(bl2);

        var cleActual2 = new ProjectCostLedgerEntry(
            Guid.NewGuid(), OrgId, proj2.Id, null, ProjectBudgetCategory.Material,
            ProjectCostEntryType.Actual, ProjectCostSourceType.GoodsReceipt,
            Guid.NewGuid(), "GR-02", "รับของเข้า", 60000m, false, UserId, now);

        db.ProjectCostLedgerEntries.Add(cleActual2);

        // 5. Stock Value (Branch 1)
        var wh1 = new Warehouse(Guid.NewGuid(), OrgId, Branch1Id, "WH-REP-01", "คลังหลัก", "address", UserId, now);
        db.Warehouses.Add(wh1);

        var plyItemId = TestOnlyDataSeeder.TestItemCatalogPlywoodId;
        var lamItemId = TestOnlyDataSeeder.TestItemCatalogLaminateId;
        var sheetUnitId = TestOnlyDataSeeder.TestItemCatalogSheetUnitId;

        var sb1 = new StockBalance(Guid.NewGuid(), OrgId, wh1.Id, plyItemId, sheetUnitId, now);
        sb1.Receive(10m, 100m, now);
        sb1.Reserve(2m, now);

        var sb2 = new StockBalance(Guid.NewGuid(), OrgId, wh1.Id, lamItemId, sheetUnitId, now);
        sb2.Receive(5m, 100m, now);

        db.StockBalances.AddRange(sb1, sb2);

        // 6. AR Aging (Branch 1)
        // Billing 1: Due 10 days ago, Amount 10,000, Paid 2,000 -> Outstanding 8,000 (Bucket 1_30)
        var bill1 = new BillingDocument(Guid.NewGuid(), OrgId, Branch1Id, proj1.Id, "BIL-REP-01", "milestone", "งวด 1", 10000m, asOf.AddDays(-10), UserId, now);
        bill1.ApplyPayment(2000m, now);
        db.BillingDocuments.Add(bill1);

        // Billing 2: Due in 5 days, Amount 5,000, Paid 0 -> Outstanding 5,000 (Bucket Current)
        var bill2 = new BillingDocument(Guid.NewGuid(), OrgId, Branch1Id, proj1.Id, "BIL-REP-02", "milestone", "งวด 2", 5000m, asOf.AddDays(5), UserId, now);
        db.BillingDocuments.Add(bill2);

        // Billing 3: Voided -> Outstanding 0 (Skipped)
        var bill3 = new BillingDocument(Guid.NewGuid(), OrgId, Branch1Id, proj1.Id, "BIL-REP-03", "milestone", "งวด 3 ยกเลิก", 20000m, asOf, UserId, now);
        bill3.Void("ลูกค้ายกเลิก", UserId, now);
        db.BillingDocuments.Add(bill3);

        // Warranty & Installation for Service Request 1 (In Warranty)
        var job = new InstallationJob(Guid.NewGuid(), OrgId, Branch1Id, proj1.Id, "INS-REP-01", asOf, asOf.AddDays(1), "ทีมช่าง A", "note", UserId, now);
        job.Start(now);
        job.MarkReady(now);
        job.Handover(HandoverOutcome.Accepted, "คุณสมชาย", "ตรวจรับแล้ว", 12, asOf, asOf, now);
        db.InstallationJobs.Add(job);

        var war = new Warranty(Guid.NewGuid(), OrgId, proj1.Id, job.Id, "WAR-REP-01", asOf, 12, now);
        db.Warranties.Add(war);

        // 7. Service SLA (Branch 1)
        // Request 1: urgent, in-warranty (warrantyId != null), created 1 hour ago -> elapsed 1 hr, resolve 8 hrs -> on_track
        var sr1 = new ServiceRequest(Guid.NewGuid(), OrgId, Branch1Id, proj1.Id, war.Id, "SR-REP-01", "แอร์รั่ว", "desc", ServicePriority.Urgent, UserId, now.AddHours(-1));
        sr1.Start(UserId, now.AddHours(-1));
        db.ServiceRequests.Add(sr1);

        // Billing for Service Request 2 (Fully paid so Outstanding = 0 and won't affect AR Aging)
        var billSr = new BillingDocument(Guid.NewGuid(), OrgId, Branch1Id, proj1.Id, "BIL-SR-01", "other", "ค่าบริการ", 2500m, asOf, UserId, now);
        billSr.ApplyPayment(2500m, now);
        db.BillingDocuments.Add(billSr);

        // Request 2: normal, out-of-warranty (warrantyId = null), waiting_customer, service charge 2,500
        var sr2 = new ServiceRequest(Guid.NewGuid(), OrgId, Branch1Id, proj1.Id, null, "SR-REP-02", "ไฟดับ", "desc", ServicePriority.Normal, UserId, now.AddHours(-5));
        sr2.Start(UserId, now.AddHours(-4));
        sr2.WaitCustomer("รอลูกค้าเปิดประตู", UserId, now.AddHours(-1));
        sr2.RecordServiceCharge(2500m, "ค่าบริการนอกประกัน", billSr.Id, billSr.Number, UserId, now);
        db.ServiceRequests.Add(sr2);

        await db.SaveChangesAsync();

        // -----------------------------------------------------------------------------------------
        // Assert Report 1: Pipeline & Win Rate
        // -----------------------------------------------------------------------------------------
        var pipeRes = await SendAsync(HttpMethod.Get, "/api/v1/reports/pipeline");
        Assert.Equal(HttpStatusCode.OK, pipeRes.StatusCode);
        var pipe = (await pipeRes.Content.ReadFromJsonAsync<PipelineReportResponse>())!;
        Assert.Equal(4, pipe.Summary.TotalCount);
        Assert.Equal(75000m, pipe.Summary.TotalExpectedBudget);
        Assert.Equal(2, pipe.Summary.WonCount);
        Assert.Equal(50000m, pipe.Summary.WonBudget);
        Assert.Equal(1, pipe.Summary.LostCount);
        Assert.Equal(1, pipe.Summary.OpenCount);
        Assert.Equal(15000m, pipe.Summary.OpenExpectedBudget);
        Assert.Equal(66.67m, pipe.Summary.WinRatePercent);

        // -----------------------------------------------------------------------------------------
        // Assert Report 2: Quotations
        // -----------------------------------------------------------------------------------------
        var quotRes = await SendAsync(HttpMethod.Get, "/api/v1/reports/quotations");
        Assert.Equal(HttpStatusCode.OK, quotRes.StatusCode);
        var quot = (await quotRes.Content.ReadFromJsonAsync<QuotationReportResponse>())!;
        Assert.Equal(3, quot.Summary.TotalCount);
        Assert.Equal(3, quot.Summary.IssuedCount);
        Assert.Equal(150000m, quot.Summary.IssuedAmount);
        Assert.Equal(2, quot.Summary.AcceptedCount);
        Assert.Equal(110000m, quot.Summary.AcceptedAmount);
        Assert.Equal(66.67m, quot.Summary.AcceptanceRatePercent);

        // -----------------------------------------------------------------------------------------
        // Assert Report 3: Project Budget vs Actual
        // -----------------------------------------------------------------------------------------
        var projRes = await SendAsync(HttpMethod.Get, "/api/v1/reports/projects-budget");
        Assert.Equal(HttpStatusCode.OK, projRes.StatusCode);
        var pb = (await projRes.Content.ReadFromJsonAsync<ProjectBudgetReportResponse>())!;
        Assert.Equal(2, pb.Summary.ProjectCount);
        Assert.Equal(1, pb.Summary.OverBudgetCount);
        Assert.Equal(150000m, pb.Summary.TotalBaselineBudget);
        Assert.Equal(20000m, pb.Summary.TotalApprovedChangeOrderBudget);
        Assert.Equal(170000m, pb.Summary.TotalCurrentBudget);
        Assert.Equal(30000m, pb.Summary.TotalCommittedAmount);
        Assert.Equal(100000m, pb.Summary.TotalActualAmount);
        Assert.Equal(40000m, pb.Summary.TotalVarianceAmount);

        // -----------------------------------------------------------------------------------------
        // Assert Report 4: Stock Value
        // -----------------------------------------------------------------------------------------
        var stockRes = await SendAsync(HttpMethod.Get, "/api/v1/reports/stock-value");
        Assert.Equal(HttpStatusCode.OK, stockRes.StatusCode);
        var sv = (await stockRes.Content.ReadFromJsonAsync<StockValueReportResponse>())!;
        Assert.Equal(1, sv.Summary.WarehouseCount);
        Assert.Equal(2, sv.Summary.SkuCount);
        Assert.Equal(15m, sv.Summary.TotalOnHand);
        Assert.Equal(2m, sv.Summary.TotalReserved);
        Assert.Equal(1500m, sv.Summary.TotalValue);

        // -----------------------------------------------------------------------------------------
        // Assert Report 5: AR Aging
        // -----------------------------------------------------------------------------------------
        var arRes = await SendAsync(HttpMethod.Get, $"/api/v1/reports/ar-aging?asOfDate={asOf:yyyy-MM-dd}");
        Assert.Equal(HttpStatusCode.OK, arRes.StatusCode);
        var ar = (await arRes.Content.ReadFromJsonAsync<ReportingArAgingResponse>())!;
        Assert.Equal(2, ar.Summary.InvoiceCount);
        Assert.Equal(5000m, ar.Summary.Current);
        Assert.Equal(8000m, ar.Summary.Bucket1To30);
        Assert.Equal(0m, ar.Summary.Bucket31To60);
        Assert.Equal(13000m, ar.Summary.TotalOutstanding);

        // -----------------------------------------------------------------------------------------
        // Assert Report 6: Service SLA
        // -----------------------------------------------------------------------------------------
        var srvRes = await SendAsync(HttpMethod.Get, "/api/v1/reports/service-sla");
        Assert.Equal(HttpStatusCode.OK, srvRes.StatusCode);
        var srv = (await srvRes.Content.ReadFromJsonAsync<ServiceSlaReportResponse>())!;
        Assert.Equal(2, srv.Summary.TotalCount);
        Assert.Equal(2, srv.Summary.OpenBacklogCount);
        Assert.Equal(1, srv.Summary.WaitingCustomerCount);
        Assert.Equal(2500m, srv.Summary.OutOfWarrantyChargedTotal);

        // -----------------------------------------------------------------------------------------
        // Assert Dashboard Summary: Matches all 6 summaries in a single call
        // -----------------------------------------------------------------------------------------
        var dashRes = await SendAsync(HttpMethod.Get, $"/api/v1/reports/dashboard-summary?asOfDate={asOf:yyyy-MM-dd}");
        Assert.Equal(HttpStatusCode.OK, dashRes.StatusCode);
        var dash = (await dashRes.Content.ReadFromJsonAsync<DashboardSummaryResponse>())!;
        Assert.Equal(Branch1Id, dash.EffectiveBranchId);

        Assert.True(dash.Pipeline.Permitted);
        Assert.NotNull(dash.Pipeline.Summary);
        Assert.Equal(4, dash.Pipeline.Summary.TotalCount);
        Assert.Equal(66.67m, dash.Pipeline.Summary.WinRatePercent);

        Assert.True(dash.Quotations.Permitted);
        Assert.NotNull(dash.Quotations.Summary);
        Assert.Equal(3, dash.Quotations.Summary.IssuedCount);
        Assert.Equal(2, dash.Quotations.Summary.AcceptedCount);

        Assert.True(dash.ProjectsBudget.Permitted);
        Assert.NotNull(dash.ProjectsBudget.Summary);
        Assert.Equal(2, dash.ProjectsBudget.Summary.ProjectCount);
        Assert.Equal(1, dash.ProjectsBudget.Summary.OverBudgetCount);

        Assert.True(dash.StockValue.Permitted);
        Assert.NotNull(dash.StockValue.Summary);
        Assert.Equal(1500m, dash.StockValue.Summary.TotalValue);

        Assert.True(dash.ArAging.Permitted);
        Assert.NotNull(dash.ArAging.Summary);
        Assert.Equal(13000m, dash.ArAging.Summary.TotalOutstanding);

        Assert.True(dash.ServiceSla.Permitted);
        Assert.NotNull(dash.ServiceSla.Summary);
        Assert.Equal(2, dash.ServiceSla.Summary.OpenBacklogCount);
        Assert.Equal(2500m, dash.ServiceSla.Summary.OutOfWarrantyChargedTotal);
    }

    // ---------------------------------------------------------------------------------------------
    // Test 2: Cross-Branch Isolation & Branch Clamping
    // ---------------------------------------------------------------------------------------------

    [Fact]
    public async Task CrossBranchIsolation_And_BranchClamping()
    {
        // 1. Caller bound to Branch 1 requests Branch 2 -> 403 Forbidden (PERMISSION_DENIED)
        var crossBranchRes = await SendAsync(HttpMethod.Get, $"/api/v1/reports/pipeline?branchId={Branch2Id}");
        Assert.Equal(HttpStatusCode.Forbidden, crossBranchRes.StatusCode);
        Assert.Equal("PERMISSION_DENIED", await ErrorCodeAsync(crossBranchRes));

        // 2. Caller bound to Branch 1 requests without branchId -> clamps to Branch 1, returns 200 OK
        var clampedRes = await SendAsync(HttpMethod.Get, "/api/v1/reports/pipeline");
        Assert.Equal(HttpStatusCode.OK, clampedRes.StatusCode);

        // 3. Org-wide caller requests specific Branch 2 -> 200 OK
        var orgWideB2Res = await SendAsync(HttpMethod.Get, $"/api/v1/reports/pipeline?branchId={Branch2Id}", token: "token-orgwide");
        Assert.Equal(HttpStatusCode.OK, orgWideB2Res.StatusCode);

        // 4. Org-wide caller requests all branches (no branchId) -> 200 OK
        var orgWideAllRes = await SendAsync(HttpMethod.Get, "/api/v1/reports/pipeline", token: "token-orgwide");
        Assert.Equal(HttpStatusCode.OK, orgWideAllRes.StatusCode);
    }

    // ---------------------------------------------------------------------------------------------
    // Test 3: Per-Report Permission Enforcement & Redacted Dashboard Sections
    // ---------------------------------------------------------------------------------------------

    [Fact]
    public async Task PerReportPermissionEnforcement_And_RedactedDashboardSections()
    {
        // User with no permissions (UidNoPerm) should be denied for each standalone report endpoint
        var endpoints = new[]
        {
            "/api/v1/reports/pipeline",
            "/api/v1/reports/quotations",
            "/api/v1/reports/projects-budget",
            "/api/v1/reports/stock-value",
            "/api/v1/reports/ar-aging",
            "/api/v1/reports/service-sla"
        };

        foreach (var endpoint in endpoints)
        {
            var res = await SendAsync(HttpMethod.Get, endpoint, token: "token-no-perm");
            Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
            Assert.Equal("PERMISSION_DENIED", await ErrorCodeAsync(res));
        }

        // For dashboard-summary, an authenticated member with NO module permissions should still receive 200 OK,
        // but every single section MUST be redacted (Permitted = false, Summary = null)
        var dashRes = await SendAsync(HttpMethod.Get, "/api/v1/reports/dashboard-summary", token: "token-no-perm");
        Assert.Equal(HttpStatusCode.OK, dashRes.StatusCode);
        var dash = (await dashRes.Content.ReadFromJsonAsync<DashboardSummaryResponse>())!;

        Assert.False(dash.Pipeline.Permitted);
        Assert.Null(dash.Pipeline.Summary);

        Assert.False(dash.Quotations.Permitted);
        Assert.Null(dash.Quotations.Summary);

        Assert.False(dash.ProjectsBudget.Permitted);
        Assert.Null(dash.ProjectsBudget.Summary);

        Assert.False(dash.StockValue.Permitted);
        Assert.Null(dash.StockValue.Summary);

        Assert.False(dash.ArAging.Permitted);
        Assert.Null(dash.ArAging.Summary);

        Assert.False(dash.ServiceSla.Permitted);
        Assert.Null(dash.ServiceSla.Summary);
    }
}
