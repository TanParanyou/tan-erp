using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
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

public class ProjectControlEndpointsTests : IAsyncLifetime
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
    private const string UidNoPerm = "uid-no-control-perm";
    private static readonly Guid MembershipNoPermId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b61");

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
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true);

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

    [Fact]
    public async Task Activation_RequiresPlanAndBudget_ThenFreezesBaseline()
    {
        var projectId = await CreateProjectAsync();
        var control = await GetControlAsync(projectId);

        var noPlan = await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/transitions", new TransitionProjectRequest("active", null), ifMatch: control.RowVersion);
        Assert.Equal(HttpStatusCode.Conflict, noPlan.StatusCode);
        Assert.Equal("PROJECT_NOT_READY", await ErrorCodeAsync(noPlan));

        var badPlan = await SendAsync(HttpMethod.Put, $"/api/v1/projects/{projectId}/plan", new SetProjectPlanRequest(End, Start), ifMatch: control.RowVersion);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, badPlan.StatusCode);

        control = await ControlAsync(await SendAsync(HttpMethod.Put, $"/api/v1/projects/{projectId}/plan", new SetProjectPlanRequest(Start, End), ifMatch: control.RowVersion));
        var noBudget = await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/transitions", new TransitionProjectRequest("active", null), ifMatch: control.RowVersion);
        Assert.Equal("PROJECT_NOT_READY", await ErrorCodeAsync(noBudget));

        var invalidBudget = await SendAsync(HttpMethod.Put, $"/api/v1/projects/{projectId}/budget",
            new ReplaceProjectBudgetRequest(new List<ProjectBudgetLineRequest> { new("bogus", "x", 10m) }), ifMatch: control.RowVersion);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, invalidBudget.StatusCode);

        control = await ControlAsync(await SendAsync(HttpMethod.Put, $"/api/v1/projects/{projectId}/budget",
            new ReplaceProjectBudgetRequest(new List<ProjectBudgetLineRequest> { new("material", "ไม้", 60000m), new("labor", "ค่าแรง", 40000.50m) }), ifMatch: control.RowVersion));
        Assert.Equal(100000.50m, control.Budget.BaselineTotal);
        Assert.False(control.Budget.IsFrozen);

        var stale = await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/transitions", new TransitionProjectRequest("active", null), ifMatch: Guid.NewGuid());
        Assert.Equal(HttpStatusCode.Conflict, stale.StatusCode);
        Assert.Equal("PROJECT_VERSION_CONFLICT", await ErrorCodeAsync(stale));

        var active = await ControlAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/transitions", new TransitionProjectRequest("active", null), ifMatch: control.RowVersion));
        Assert.Equal("active", active.Status);
        Assert.True(active.Budget.IsFrozen);
        Assert.NotNull(active.ActivatedAtUtc);
        Assert.Equal("planned", active.History.Single().FromStatus);

        var frozen = await SendAsync(HttpMethod.Put, $"/api/v1/projects/{projectId}/budget",
            new ReplaceProjectBudgetRequest(new List<ProjectBudgetLineRequest> { new("material", "ไม้", 1m) }), ifMatch: active.RowVersion);
        Assert.Equal(HttpStatusCode.Conflict, frozen.StatusCode);
    }

    [Fact]
    public async Task StatusTransitions_FollowTheLifecycle_AndRequireReasons()
    {
        var projectId = await CreateProjectAsync();
        var active = await ActivateAsync(projectId, 1000m);

        var noReason = await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/transitions", new TransitionProjectRequest("on_hold", " "), ifMatch: active.RowVersion);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, noReason.StatusCode);

        var hold = await ControlAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/transitions", new TransitionProjectRequest("on_hold", "รอลูกค้า"), ifMatch: active.RowVersion));
        Assert.Equal("on_hold", hold.Status);
        Assert.Equal("รอลูกค้า", hold.StatusReason);

        var illegal = await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/transitions", new TransitionProjectRequest("completed", null), ifMatch: hold.RowVersion);
        Assert.Equal(HttpStatusCode.Conflict, illegal.StatusCode);
        Assert.Equal("PROJECT_INVALID_TRANSITION", await ErrorCodeAsync(illegal));

        var resumed = await ControlAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/transitions", new TransitionProjectRequest("active", null), ifMatch: hold.RowVersion));
        var ready = await ControlAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/transitions", new TransitionProjectRequest("ready_for_handover", null), ifMatch: resumed.RowVersion));
        var done = await ControlAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/transitions", new TransitionProjectRequest("completed", null), ifMatch: ready.RowVersion));
        Assert.Equal("completed", done.Status);
        Assert.NotNull(done.CompletedAtUtc);
        Assert.Equal(5, done.History.Count);

        var afterDone = await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/transitions", new TransitionProjectRequest("cancelled", "x"), ifMatch: done.RowVersion);
        Assert.Equal(HttpStatusCode.Conflict, afterDone.StatusCode);
    }

    [Fact]
    public async Task Milestones_RollUpWeightedProgress_AndGateReadyForHandover()
    {
        var projectId = await CreateProjectAsync();
        var planning = await GetControlAsync(projectId);

        var withOne = await ControlAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/milestones", new AddProjectMilestoneRequest("เข้าวัดพื้นที่", Start, 1)));
        var withTwo = await ControlAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/milestones", new AddProjectMilestoneRequest("ติดตั้ง", End, 3)));
        Assert.Equal(2, withTwo.Milestones.Count);
        Assert.Equal(0m, withTwo.Progress.Percent);
        Assert.Equal(planning.RowVersion, withTwo.RowVersion);

        var invalid = await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/milestones", new AddProjectMilestoneRequest(" ", null, 1));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, invalid.StatusCode);

        // Completing needs an active project
        var early = await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/milestones/{withOne.Milestones[0].Id}/complete", new ProjectMilestoneVersionRequest(withOne.Milestones[0].RowVersion));
        Assert.Equal("PROJECT_INVALID_STATE", await ErrorCodeAsync(early));

        var active = await ActivateAsync(projectId, 500m);
        var first = active.Milestones.Single(m => m.Name == "เข้าวัดพื้นที่");

        var blocked = await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/transitions", new TransitionProjectRequest("ready_for_handover", null), ifMatch: active.RowVersion);
        Assert.Equal("PROJECT_NOT_READY", await ErrorCodeAsync(blocked));

        var staleComplete = await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/milestones/{first.Id}/complete", new ProjectMilestoneVersionRequest(Guid.NewGuid()));
        Assert.Equal("PROJECT_MILESTONE_VERSION_CONFLICT", await ErrorCodeAsync(staleComplete));

        var afterFirst = await ControlAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/milestones/{first.Id}/complete", new ProjectMilestoneVersionRequest(first.RowVersion)));
        Assert.Equal(25m, afterFirst.Progress.Percent);
        Assert.Equal(1, afterFirst.Progress.CompletedMilestones);

        var completedEdit = await SendAsync(HttpMethod.Put, $"/api/v1/projects/{projectId}/milestones/{first.Id}", new UpdateProjectMilestoneRequest("ใหม่", null, 2, afterFirst.Milestones.Single(m => m.Id == first.Id).RowVersion));
        Assert.Equal("PROJECT_MILESTONE_COMPLETED", await ErrorCodeAsync(completedEdit));

        var second = afterFirst.Milestones.Single(m => m.Name == "ติดตั้ง");
        var afterSecond = await ControlAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/milestones/{second.Id}/complete", new ProjectMilestoneVersionRequest(second.RowVersion)));
        Assert.Equal(100m, afterSecond.Progress.Percent);

        var ready = await ControlAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/transitions", new TransitionProjectRequest("ready_for_handover", null), ifMatch: afterSecond.RowVersion));
        Assert.Equal("ready_for_handover", ready.Status);
    }

    [Fact]
    public async Task ChangeOrders_NeedAnotherApprover_AndAdjustCurrentBudgetAndContract()
    {
        var projectId = await CreateProjectAsync();
        var active = await ActivateAsync(projectId, 1000m);
        var baselineContract = active.Contract.BaselineAmount;

        var invalid = await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/change-orders", new CreateProjectChangeOrderRequest(" ", "เหตุผล", 0m, 0m), key: "co-invalid-key-0001");
        Assert.Equal(HttpStatusCode.UnprocessableEntity, invalid.StatusCode);

        var created = await ControlAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/change-orders",
            new CreateProjectChangeOrderRequest("เพิ่มตู้", "ลูกค้าขอเพิ่ม", 300m, 450m), key: "co-create-key-0001"));
        var order = Assert.Single(created.ChangeOrders);
        Assert.StartsWith("PCO", order.Number);
        Assert.Equal("draft", order.Status);

        var replay = await ControlAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/change-orders",
            new CreateProjectChangeOrderRequest("เพิ่มตู้", "ลูกค้าขอเพิ่ม", 300m, 450m), key: "co-create-key-0001"));
        Assert.Single(replay.ChangeOrders);

        var decideDraft = await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/change-orders/{order.Id}/approve", new ProjectChangeOrderActionRequest(order.RowVersion, null), token: "token-approver");
        Assert.Equal("PROJECT_CHANGE_ORDER_INVALID_STATE", await ErrorCodeAsync(decideDraft));

        var submitted = await ControlAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/change-orders/{order.Id}/submit", new ProjectChangeOrderActionRequest(order.RowVersion, null)));
        var submittedOrder = submitted.ChangeOrders.Single();

        // Pending change orders block handover
        var blocked = await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/transitions", new TransitionProjectRequest("ready_for_handover", null), ifMatch: submitted.RowVersion);
        Assert.Equal("PROJECT_NOT_READY", await ErrorCodeAsync(blocked));

        // Maker-checker: the creator cannot decide
        var self = await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/change-orders/{order.Id}/approve", new ProjectChangeOrderActionRequest(submittedOrder.RowVersion, null));
        Assert.Equal(HttpStatusCode.Forbidden, self.StatusCode);
        Assert.Equal("PROJECT_CHANGE_ORDER_SELF_APPROVAL", await ErrorCodeAsync(self));

        var staleDecision = await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/change-orders/{order.Id}/approve", new ProjectChangeOrderActionRequest(Guid.NewGuid(), null), token: "token-approver");
        Assert.Equal("PROJECT_CHANGE_ORDER_VERSION_CONFLICT", await ErrorCodeAsync(staleDecision));

        var approved = await ControlAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/change-orders/{order.Id}/approve", new ProjectChangeOrderActionRequest(submittedOrder.RowVersion, "ตกลง"), token: "token-approver"));
        Assert.Equal("approved", approved.ChangeOrders.Single().Status);
        Assert.Equal(ApproverUserId, approved.ChangeOrders.Single().DecidedBy!.Id);
        Assert.Equal(1000m, approved.Budget.BaselineTotal);
        Assert.Equal(300m, approved.Budget.ApprovedBudgetDelta);
        Assert.Equal(1300m, approved.Budget.CurrentTotal);
        Assert.Equal(baselineContract, approved.Contract.BaselineAmount);
        Assert.Equal(baselineContract + 450m, approved.Contract.CurrentAmount);

        // Over-budget control: a reduction larger than the current budget is refused
        var shrink = await ControlAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/change-orders",
            new CreateProjectChangeOrderRequest("ลดงาน", "ตัดงาน", -5000m, -100m), key: "co-create-key-0002"));
        var shrinkOrder = shrink.ChangeOrders.Single(c => c.Title == "ลดงาน");
        var shrinkSubmitted = (await ControlAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/change-orders/{shrinkOrder.Id}/submit", new ProjectChangeOrderActionRequest(shrinkOrder.RowVersion, null))))
            .ChangeOrders.Single(c => c.Id == shrinkOrder.Id);
        var negative = await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/change-orders/{shrinkOrder.Id}/approve", new ProjectChangeOrderActionRequest(shrinkSubmitted.RowVersion, null), token: "token-approver");
        Assert.Equal(HttpStatusCode.UnprocessableEntity, negative.StatusCode);
        Assert.Equal("PROJECT_BUDGET_NEGATIVE", await ErrorCodeAsync(negative));

        var noNote = await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/change-orders/{shrinkOrder.Id}/reject", new ProjectChangeOrderActionRequest(shrinkSubmitted.RowVersion, null), token: "token-approver");
        Assert.Equal(HttpStatusCode.UnprocessableEntity, noNote.StatusCode);
        var rejected = await ControlAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/change-orders/{shrinkOrder.Id}/reject", new ProjectChangeOrderActionRequest(shrinkSubmitted.RowVersion, "เกินงบ"), token: "token-approver"));
        Assert.Equal("rejected", rejected.ChangeOrders.Single(c => c.Id == shrinkOrder.Id).Status);
        Assert.Equal(1300m, rejected.Budget.CurrentTotal);
    }

    [Fact]
    public async Task ChangeOrders_RejectedOnPlannedProject_AndControlRequiresPermissionAndScope()
    {
        var projectId = await CreateProjectAsync();

        var planned = await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/change-orders", new CreateProjectChangeOrderRequest("a", "b", 1m, 1m), key: "co-planned-key-0001");
        Assert.Equal(HttpStatusCode.Conflict, planned.StatusCode);
        Assert.Equal("PROJECT_INVALID_STATE", await ErrorCodeAsync(planned));

        var denied = await SendAsync(HttpMethod.Get, $"/api/v1/projects/{projectId}/control", token: "token-no-perm", membership: MembershipNoPermId);
        Assert.Equal(HttpStatusCode.Forbidden, denied.StatusCode);

        var foreign = await SendAsync(HttpMethod.Get, $"/api/v1/projects/{projectId}/control", token: "token-org-b");
        Assert.True(foreign.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Forbidden);

        var control = await GetControlAsync(projectId);
        var foreignWrite = await SendAsync(HttpMethod.Put, $"/api/v1/projects/{projectId}/plan", new SetProjectPlanRequest(Start, End), token: "token-org-b", ifMatch: control.RowVersion);
        Assert.True(foreignWrite.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Forbidden);

        var unchanged = await GetControlAsync(projectId);
        Assert.Null(unchanged.PlannedStartDate);
    }
}
