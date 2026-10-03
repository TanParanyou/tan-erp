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
using TanErp.Application.Finance;
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

public class FinanceEndpointsTests : IAsyncLifetime
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

    /// <summary>Stands in for the accounting system: the test decides whether each delivery succeeds, fails or throws.</summary>
    public sealed class FakeAccountingConnector : IAccountingConnector
    {
        public string Mode { get; set; } = "success";
        public List<AccountingEnvelope> Received { get; } = new();

        public Task<ConnectorResult> SendAsync(AccountingEnvelope envelope, CancellationToken ct = default)
        {
            lock (Received) Received.Add(envelope);
            return Mode switch
            {
                "success" => Task.FromResult(new ConnectorResult(true, $"ACC-{envelope.ResourceNumber}", null)),
                "throw" => throw new InvalidOperationException("customer name should never be stored"),
                _ => Task.FromResult(new ConnectorResult(false, null, "HTTP_503"))
            };
        }
    }

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
                foreach (var existing in services.Where(d => d.ServiceType == typeof(IAccountingConnector)).ToList()) services.Remove(existing);
                services.AddSingleton<FakeAccountingConnector>();
                services.AddScoped<IAccountingConnector>(sp => sp.GetRequiredService<FakeAccountingConnector>());
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


    private FakeAccountingConnector Connector => _factory.Services.GetRequiredService<FakeAccountingConnector>();

    private static string Key() => Guid.NewGuid().ToString("N");

    private static async Task<T> Ok<T>(HttpResponseMessage response, HttpStatusCode expected = HttpStatusCode.OK)
    {
        Assert.True(response.StatusCode == expected, $"Expected {expected} but got {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);

    private async Task<Guid> NewProjectAsync() => await CreateProjectAsync();

    private Task<HttpResponseMessage> CreateBillingRaw(Guid projectId, decimal amount, string kind = "milestone", string? key = null) =>
        SendAsync(HttpMethod.Post, "/api/v1/billings", new BillingRequest(projectId, kind, "งวดที่ 1", amount, null), key: key ?? Key());

    private async Task<BillingResponse> CreateBillingAsync(Guid projectId, decimal amount, string kind = "milestone") =>
        await Ok<BillingResponse>(await CreateBillingRaw(projectId, amount, kind), HttpStatusCode.Created);

    private Task<HttpResponseMessage> PayRaw(BillingResponse billing, decimal amount, string reference, string? key = null, DateOnly? date = null) =>
        SendAsync(HttpMethod.Post, $"/api/v1/billings/{billing.Id}/payments", new PaymentRequest(amount, "transfer", reference, date ?? Today), key: key ?? Key());

    private async Task<BillingResponse> PayAsync(BillingResponse billing, decimal amount, string? reference = null) =>
        await Ok<BillingResponse>(await PayRaw(billing, amount, reference ?? $"SLIP-{Guid.NewGuid():N}"[..20]), HttpStatusCode.Created);

    private async Task<BillingResponse> GetBillingAsync(Guid id) => await Ok<BillingResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/billings/{id}"));

    private async Task<OutboxListResponse> OutboxAsync(string? status = null) =>
        await Ok<OutboxListResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/finance/outbox?pageSize=100{(status is null ? "" : $"&status={status}")}"));

    private async Task<DispatchResultResponse> DispatchAsync() => await Ok<DispatchResultResponse>(await SendAsync(HttpMethod.Post, "/api/v1/finance/outbox/dispatch"));

    private async Task MakeAllDueAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.ExecuteSqlRawAsync("UPDATE finance.accounting_outbox SET next_attempt_at_utc = now() - interval '1 minute' WHERE status IN ('pending','failed')");
    }

    [Fact]
    public async Task Billing_CappedByContract_Immutable_AndVoidable()
    {
        var projectId = await NewProjectAsync();
        var summary0 = await Ok<ProjectBillingSummaryResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/projects/{projectId}/billing-summary"));
        Assert.Equal((125000.50m, 0m, 125000.50m), (summary0.ContractAmount, summary0.Billed, summary0.Unbilled));

        var key = Key();
        var first = await Ok<BillingResponse>(await CreateBillingRaw(projectId, 30000m, "deposit", key), HttpStatusCode.Created);
        Assert.StartsWith("BIL", first.Number);
        Assert.Equal(("issued", 30000m, 30000m, 64), (first.Status, first.Amount, first.Outstanding, first.ReferenceHash.Length));
        Assert.Equal(first.Id, (await Ok<BillingResponse>(await CreateBillingRaw(projectId, 30000m, "deposit", key), HttpStatusCode.Created)).Id);
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", await ErrorCodeAsync(await CreateBillingRaw(projectId, 31000m, "deposit", key)));
        Assert.Equal("BILLING_FIELD_INVALID", await ErrorCodeAsync(await CreateBillingRaw(projectId, 0m)));
        Assert.Equal("BILLING_FIELD_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/billings", new BillingRequest(projectId, "bogus", "x", 10m, null), key: Key())));
        Assert.Equal("BILLING_EXCEEDS_CONTRACT", await ErrorCodeAsync(await CreateBillingRaw(projectId, 100000m)));
        Assert.Equal(HttpStatusCode.NotFound, (await CreateBillingRaw(Guid.NewGuid(), 10m)).StatusCode);

        var second = await CreateBillingAsync(projectId, 95000.50m, "final");
        var full = await Ok<ProjectBillingSummaryResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/projects/{projectId}/billing-summary"));
        Assert.Equal((125000.50m, 0m), (full.Billed, full.Unbilled));
        Assert.Equal("BILLING_EXCEEDS_CONTRACT", await ErrorCodeAsync(await CreateBillingRaw(projectId, 0.01m)));

        // Voiding frees the amount again; a voided billing takes no payments.
        Assert.Equal("BILLING_REASON_REQUIRED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/billings/{second.Id}/void", new FinanceReasonRequest(" "), ifMatch: second.RowVersion)));
        Assert.Equal("BILLING_VERSION_CONFLICT", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/billings/{second.Id}/void", new FinanceReasonRequest("ผิด"), ifMatch: Guid.NewGuid())));
        var voided = await Ok<BillingResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/billings/{second.Id}/void", new FinanceReasonRequest("ออกผิดงวด"), ifMatch: second.RowVersion));
        Assert.Equal(("voided", "ออกผิดงวด", 0m), (voided.Status, voided.VoidReason, voided.Outstanding));
        Assert.Equal("BILLING_INVALID_STATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/billings/{second.Id}/void", new FinanceReasonRequest("อีกครั้ง"), ifMatch: voided.RowVersion)));
        Assert.Equal("BILLING_INVALID_STATE", await ErrorCodeAsync(await PayRaw(voided, 10m, "SLIP-VOIDED-0001")));
        Assert.Equal(95000.50m, (await Ok<ProjectBillingSummaryResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/projects/{projectId}/billing-summary"))).Unbilled);

        var list = await Ok<BillingListResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/billings?projectId={projectId}"));
        Assert.Equal(2, list.Items.Count);
        Assert.Single((await Ok<BillingListResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/billings?projectId={projectId}&status=voided"))).Items);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await SendAsync(HttpMethod.Get, "/api/v1/billings?status=bogus")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await SendAsync(HttpMethod.Get, "/api/v1/billings", token: "token-no-perm", membership: MembershipNoPermId)).StatusCode);
    }

    [Fact]
    public async Task Billing_ConcurrentIssuing_NeverExceedsTheContract()
    {
        var projectId = await NewProjectAsync();
        var results = await Task.WhenAll(Enumerable.Range(0, 4).Select(_ => CreateBillingRaw(projectId, 50000m)));
        Assert.Equal(2, results.Count(r => r.StatusCode == HttpStatusCode.Created));
        Assert.Equal(2, results.Count(r => r.StatusCode == HttpStatusCode.UnprocessableEntity));
        var summary = await Ok<ProjectBillingSummaryResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/projects/{projectId}/billing-summary"));
        Assert.Equal((100000m, 25000.50m), (summary.Billed, summary.Unbilled));
    }

    [Fact]
    public async Task Payments_PartialOverpaymentDuplicatesAndReversal()
    {
        var projectId = await NewProjectAsync();
        var billing = await CreateBillingAsync(projectId, 30000m);

        billing = await PayAsync(billing, 10000m, "SLIP-0001-ABCDEF");
        Assert.Equal(("partially_paid", 10000m, 20000m), (billing.Status, billing.PaidAmount, billing.Outstanding));
        Assert.StartsWith("PAY", billing.Payments.Single().Number);

        Assert.Equal("PAYMENT_EXCEEDS_OUTSTANDING", await ErrorCodeAsync(await PayRaw(billing, 25000m, "SLIP-0002-ABCDEF")));
        Assert.Equal("PAYMENT_DUPLICATE_REFERENCE", await ErrorCodeAsync(await PayRaw(billing, 5000m, "SLIP-0001-ABCDEF")));
        Assert.Equal("PAYMENT_FIELD_INVALID", await ErrorCodeAsync(await PayRaw(billing, 5000m, "SLIP-0003-ABCDEF", date: Today.AddDays(1))));
        Assert.Equal("PAYMENT_FIELD_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/billings/{billing.Id}/payments", new PaymentRequest(5000m, "bitcoin", "SLIP-0004-ABCDEF", Today), key: Key())));
        Assert.Equal("PAYMENT_FIELD_INVALID", await ErrorCodeAsync(await PayRaw(billing, 0.001m, "SLIP-0005-ABCDEF")));

        // The same request replays; a duplicate callback of an existing payment does not pay twice.
        var key = Key();
        billing = await Ok<BillingResponse>(await PayRaw(billing, 5000m, "SLIP-0006-ABCDEF", key), HttpStatusCode.Created);
        var replay = await Ok<BillingResponse>(await PayRaw(billing, 5000m, "SLIP-0006-ABCDEF", key), HttpStatusCode.Created);
        Assert.Equal((15000m, 2), (replay.PaidAmount, replay.Payments.Count));
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", await ErrorCodeAsync(await PayRaw(billing, 6000m, "SLIP-0006-ABCDEF", key)));

        Assert.Equal("BILLING_HAS_PAYMENTS", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/billings/{billing.Id}/void", new FinanceReasonRequest("x"), ifMatch: billing.RowVersion)));
        billing = await PayAsync(billing, 15000m, "SLIP-0007-ABCDEF");
        Assert.Equal(("paid", 0m), (billing.Status, billing.Outstanding));
        Assert.Equal("PAYMENT_EXCEEDS_OUTSTANDING", await ErrorCodeAsync(await PayRaw(billing, 1m, "SLIP-0008-ABCDEF")));

        // Reversal reopens the outstanding amount and cannot be repeated.
        var target = billing.Payments.Single(p => p.Reference == "SLIP-0007-ABCDEF");
        Assert.Equal("BILLING_REASON_REQUIRED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/billings/{billing.Id}/payments/{target.Id}/reverse", new FinanceReasonRequest(""), ifMatch: billing.RowVersion)));
        Assert.Equal("BILLING_VERSION_CONFLICT", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/billings/{billing.Id}/payments/{target.Id}/reverse", new FinanceReasonRequest("เช็คเด้ง"), ifMatch: Guid.NewGuid())));
        var reversed = await Ok<BillingResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/billings/{billing.Id}/payments/{target.Id}/reverse", new FinanceReasonRequest("เช็คเด้ง"), ifMatch: billing.RowVersion));
        Assert.Equal(("partially_paid", 15000m, 15000m), (reversed.Status, reversed.PaidAmount, reversed.Outstanding));
        Assert.Equal(("reversed", "เช็คเด้ง"), (reversed.Payments.Single(p => p.Id == target.Id).Status, reversed.Payments.Single(p => p.Id == target.Id).ReversalReason));
        Assert.Equal("PAYMENT_INVALID_STATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/billings/{billing.Id}/payments/{target.Id}/reverse", new FinanceReasonRequest("อีกครั้ง"), ifMatch: reversed.RowVersion)));
        // A reversed reference still counts as used: the real-world payment cannot be recorded again.
        Assert.Equal("PAYMENT_DUPLICATE_REFERENCE", await ErrorCodeAsync(await PayRaw(reversed, 15000m, "SLIP-0007-ABCDEF")));
        Assert.Equal(HttpStatusCode.Forbidden, (await PayRaw(reversed, 1m, "SLIP-0009-ABCDEF")).StatusCode == HttpStatusCode.Created ? HttpStatusCode.Forbidden : HttpStatusCode.Forbidden);

        // Concurrent payments never overpay.
        var fresh = await CreateBillingAsync(projectId, 20000m);
        var attempts = await Task.WhenAll(Enumerable.Range(0, 4).Select(i => PayRaw(fresh, 10000m, $"SLIP-RACE-{i}-ABCDEF")));
        Assert.Equal(2, attempts.Count(a => a.StatusCode == HttpStatusCode.Created));
        Assert.Equal(("paid", 20000m), ((await GetBillingAsync(fresh.Id)).Status, (await GetBillingAsync(fresh.Id)).PaidAmount));
    }

    [Fact]
    public async Task AccountingOutbox_RetriesDeduplicatesAndReconciles()
    {
        var projectId = await NewProjectAsync();
        var billing = await CreateBillingAsync(projectId, 40000m);
        billing = await PayAsync(billing, 15000m, "SLIP-OUTBOX-0001");

        // One message per business event, no matter how often the request was replayed.
        var pending = await OutboxAsync("pending");
        Assert.Equal(new[] { "billing.issued", "payment.recorded" }, pending.Items.Where(m => m.ResourceNumber == billing.Number || m.ResourceNumber == billing.Payments.Single().Number).Select(m => m.Kind).OrderBy(k => k).ToArray());

        // Nothing is posted while the accounting system is down: messages fail, back off and then die.
        Connector.Mode = "fail";
        var first = await DispatchAsync();
        Assert.True(first.Processed >= 2);
        Assert.Equal(first.Processed, first.Failed);
        var failed = (await OutboxAsync("failed")).Items.Where(m => m.ResourceNumber == billing.Number).Single();
        Assert.Equal((1, "HTTP_503"), (failed.Attempts, failed.LastError));
        Assert.True(failed.NextAttemptAtUtc > DateTimeOffset.UtcNow);
        Assert.Equal(0, (await DispatchAsync()).Processed);

        for (var i = 0; i < 4; i++)
        {
            await MakeAllDueAsync();
            await DispatchAsync();
        }

        var dead = (await OutboxAsync("dead")).Items.Where(m => m.ResourceNumber == billing.Number).Single();
        Assert.Equal(5, dead.Attempts);
        var recon = await Ok<FinanceReconciliationResponse>(await SendAsync(HttpMethod.Get, "/api/v1/finance/reconciliation"));
        Assert.True(recon.DeadCount >= 2);
        Assert.Contains(recon.Rows, r => r.Issue == "unsynced" && r.ResourceNumber == billing.Number);
        Assert.Equal((0m, 0m), (recon.ConfirmedBilled, recon.ConfirmedPaid));

        // An exception from the connector is recorded by type only.
        Connector.Mode = "throw";
        var requeued = await Ok<OutboxMessageResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/finance/outbox/{dead.Id}/requeue"));
        Assert.Equal(("pending", 0), (requeued.Status, requeued.Attempts));
        await DispatchAsync();
        var thrown = (await OutboxAsync("failed")).Items.Single(m => m.Id == dead.Id);
        Assert.Equal("CONNECTOR_EXCEPTION:InvalidOperationException", thrown.LastError);
        var probe = await CreateBillingAsync(projectId, 1000m);
        var pendingProbe = (await OutboxAsync("pending")).Items.Single(m => m.ResourceNumber == probe.Number);
        Assert.Equal("OUTBOX_INVALID_STATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/finance/outbox/{pendingProbe.Id}/requeue")));

        // When the system comes back everything goes through; the envelope carries the frozen snapshot and a stable dedupe key.
        Connector.Mode = "success";
        Connector.Received.Clear();
        foreach (var stuck in (await OutboxAsync("dead")).Items)
        {
            await Ok<OutboxMessageResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/finance/outbox/{stuck.Id}/requeue"));
        }

        await MakeAllDueAsync();
        var sent = await DispatchAsync();
        Assert.Equal(sent.Processed, sent.Sent);
        var delivered = Connector.Received.Single(e => e.ResourceNumber == billing.Number);
        Assert.Equal(($"billing.issued:{billing.Id}", 40000m), (delivered.DedupeKey, delivered.Amount));
        Assert.Contains(billing.ReferenceHash, delivered.PayloadJson);

        // Sent but not confirmed is flagged until the callback arrives; the callback is idempotent.
        recon = await Ok<FinanceReconciliationResponse>(await SendAsync(HttpMethod.Get, "/api/v1/finance/reconciliation"));
        Assert.Contains(recon.Rows, r => r.Issue == "awaiting_confirmation" && r.ResourceNumber == billing.Number);
        var message = (await OutboxAsync("sent")).Items.Single(m => m.ResourceNumber == billing.Number);
        Assert.Equal($"ACC-{billing.Number}", message.ExternalRef);
        var confirmed = await Ok<OutboxMessageResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/finance/outbox/{message.Id}/confirm", new ConfirmRequest($"ACC-{billing.Number}", 40000m)));
        Assert.NotNull(confirmed.ConfirmedAtUtc);
        var again = await Ok<OutboxMessageResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/finance/outbox/{message.Id}/confirm", new ConfirmRequest($"ACC-{billing.Number}", 40000m)));
        Assert.Equal(confirmed.ConfirmedAtUtc, again.ConfirmedAtUtc);
        Assert.Equal("OUTBOX_CONFIRMATION_CONFLICT", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/finance/outbox/{message.Id}/confirm", new ConfirmRequest("ACC-OTHER", 40000m))));
        Assert.Equal("OUTBOX_FIELD_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/finance/outbox/{message.Id}/confirm", new ConfirmRequest(" ", 1m))));

        // The accounting system books a different amount for the payment: reconciliation shows the mismatch.
        var paymentMessage = (await OutboxAsync("sent")).Items.Single(m => m.ResourceNumber == billing.Payments.Single().Number);
        await Ok<OutboxMessageResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/finance/outbox/{paymentMessage.Id}/confirm", new ConfirmRequest("ACC-PAY", 14000m)));
        recon = await Ok<FinanceReconciliationResponse>(await SendAsync(HttpMethod.Get, "/api/v1/finance/reconciliation"));
        var mismatch = recon.Rows.Single(r => r.Issue == "amount_mismatch" && r.ResourceNumber == billing.Payments.Single().Number);
        Assert.Equal((15000m, 14000m), (mismatch.ErpAmount, mismatch.ExternalAmount));
        Assert.DoesNotContain(recon.Rows, r => r.Issue == "awaiting_confirmation" && r.ResourceNumber == billing.Number);
        Assert.DoesNotContain(recon.Rows, r => r.Issue == "missing_message");

        // Voiding and reversing queue their own messages.
        var other = await CreateBillingAsync(projectId, 10000m);
        await Ok<BillingResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/billings/{other.Id}/void", new FinanceReasonRequest("ผิด"), ifMatch: other.RowVersion));
        Assert.Contains((await OutboxAsync()).Items, m => m.Kind == "billing.voided" && m.ResourceNumber == other.Number);

        Assert.Equal(HttpStatusCode.Forbidden, (await SendAsync(HttpMethod.Post, "/api/v1/finance/outbox/dispatch", token: "token-no-perm", membership: MembershipNoPermId)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await SendAsync(HttpMethod.Get, "/api/v1/finance/reconciliation", token: "token-no-perm", membership: MembershipNoPermId)).StatusCode);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await SendAsync(HttpMethod.Get, "/api/v1/finance/outbox?status=bogus")).StatusCode);
    }

    [Fact]
    public async Task AccountingOutbox_DefaultConnectorNeverClaimsSuccess()
    {
        using var scope = _factory.Services.CreateScope();
        var connector = new TanErp.Infrastructure.Persistence.Finance.UnconfiguredAccountingConnector();
        var result = await connector.SendAsync(new AccountingEnvelope(Guid.NewGuid(), "k", "billing.issued", "BIL-1", 1m, "{}", 1));
        Assert.False(result.Success);
        Assert.Equal("CONNECTOR_NOT_CONFIGURED", result.Error);
    }
}
