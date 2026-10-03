using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.Service;
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

public class ServiceEndpointsTests : IAsyncLifetime
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


    private static string Key() => Guid.NewGuid().ToString("N");

    private static async Task<T> Ok<T>(HttpResponseMessage response, HttpStatusCode expected = HttpStatusCode.OK)
    {
        Assert.True(response.StatusCode == expected, $"Expected {expected} but got {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);

    private async Task<InstallationResponse> CreateInstallationAsync(Guid projectId, string? key = null) =>
        await Ok<InstallationResponse>(await SendAsync(HttpMethod.Post, "/api/v1/installations",
            new InstallationRequest(projectId, Start, Start.AddDays(5), "ทีมติดตั้ง A", "ติดตั้งห้องนอน",
                new List<ChecklistItemRequest> { new("ตรวจวัดพื้นที่", true), new("ทำความสะอาดหน้างาน", true), new("ถ่ายรูปงาน", false) }), key: key ?? Key()), HttpStatusCode.Created);

    private Task<HttpResponseMessage> Command(InstallationResponse job, string path, object? body = null, string token = "token-org-a", Guid? version = null) =>
        SendAsync(HttpMethod.Post, $"/api/v1/installations/{job.Id}/{path}", body, token: token, ifMatch: version ?? job.RowVersion);

    private async Task<InstallationResponse> Step(InstallationResponse job, string path, object? body = null, string token = "token-org-a") =>
        await Ok<InstallationResponse>(await Command(job, path, body, token));

    private async Task<InstallationResponse> GetJobAsync(Guid id) => await Ok<InstallationResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/installations/{id}"));

    private async Task<InstallationResponse> SetDoneAsync(InstallationResponse job, Guid itemId, bool done = true) =>
        await Ok<InstallationResponse>(await SendAsync(HttpMethod.Put, $"/api/v1/installations/{job.Id}/checklist/{itemId}", new ChecklistDoneRequest(done), ifMatch: job.RowVersion));

    [Fact]
    public async Task Installation_Lifecycle_ChecklistDefectsHandoverAndWarranty()
    {
        var projectId = await CreateProjectAsync();
        await ActivateAsync(projectId, 100000m);

        var key = Key();
        var job = await CreateInstallationAsync(projectId, key);
        Assert.StartsWith("INS", job.Number);
        Assert.Equal(("planned", 3), (job.Status, job.Checklist.Count));
        Assert.Equal(job.Id, (await Ok<InstallationResponse>(await SendAsync(HttpMethod.Post, "/api/v1/installations", new InstallationRequest(projectId, Start, Start.AddDays(5), "ทีมติดตั้ง A", "ติดตั้งห้องนอน",
            new List<ChecklistItemRequest> { new("ตรวจวัดพื้นที่", true), new("ทำความสะอาดหน้างาน", true), new("ถ่ายรูปงาน", false) }), key: key), HttpStatusCode.Created)).Id);

        // Closing the project is blocked while an installation is open.
        var control = await GetControlAsync(projectId);
        Assert.Equal("PROJECT_OPEN_INSTALLATION", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/transitions", new TransitionProjectRequest("completed", null), ifMatch: control.RowVersion)));

        Assert.Equal("INSTALLATION_INVALID_STATE", await ErrorCodeAsync(await Command(job, "ready")));
        Assert.Equal("INSTALLATION_INVALID_STATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Put, $"/api/v1/installations/{job.Id}/checklist/{job.Checklist[0].Id}", new ChecklistDoneRequest(true), ifMatch: job.RowVersion)));
        Assert.Equal("INSTALLATION_VERSION_CONFLICT", await ErrorCodeAsync(await Command(job, "start", version: Guid.NewGuid())));
        job = await Step(job, "start");
        Assert.Equal("in_progress", job.Status);

        // Required checklist items and defects gate the move to ready.
        job = await Ok<InstallationResponse>(await Command(job, "defects", new DefectRequest("รอยขีดข่วนที่บานตู้", "major")));
        Assert.Equal(("open", 1), (job.Defects.Single().Status, job.Defects.Single().No));
        job = await SetDoneAsync(job, job.Checklist[0].Id);
        Assert.Equal("INSTALLATION_CHECKLIST_INCOMPLETE", await ErrorCodeAsync(await Command(job, "ready")));
        job = await SetDoneAsync(job, job.Checklist[1].Id);
        Assert.Equal("INSTALLATION_DEFECTS_OPEN", await ErrorCodeAsync(await Command(job, "ready")));

        var defectId = job.Defects.Single().Id;
        Assert.Equal("INSTALLATION_REASON_REQUIRED", await ErrorCodeAsync(await Command(job, $"defects/{defectId}/resolve", new NoteRequest(" "))));
        Assert.Equal("INSTALLATION_DEFECT_INVALID_STATE", await ErrorCodeAsync(await Command(job, $"defects/{defectId}/verify")));
        job = await Step(job, $"defects/{defectId}/resolve", new NoteRequest("ขัดและทาสีซ่อม"));
        Assert.Equal("INSTALLATION_SELF_VERIFICATION", await ErrorCodeAsync(await Command(job, $"defects/{defectId}/verify")));
        job = await Step(job, $"defects/{defectId}/verify", null, "token-approver");
        Assert.Equal("verified", job.Defects.Single().Status);

        job = await Step(job, "ready");
        Assert.Equal("ready_for_handover", job.Status);

        // A defect found at handover sends the job back; reopening a verified defect does too.
        job = await Ok<InstallationResponse>(await Command(job, "defects", new DefectRequest("บานพับหลวม", "minor")));
        Assert.Equal("in_progress", job.Status);
        var second = job.Defects.Single(d => d.No == 2).Id;
        job = await Step(job, $"defects/{second}/resolve", new NoteRequest("ขันแน่น"));
        job = await Step(job, $"defects/{second}/verify", null, "token-approver");
        job = await Step(job, "ready");
        job = await Step(job, $"defects/{defectId}/reopen", new ReasonRequest("ลูกค้าพบรอยซ้ำ"));
        Assert.Equal(("in_progress", "reopened", 1), (job.Status, job.Defects.Single(d => d.Id == defectId).Status, job.Defects.Single(d => d.Id == defectId).ReopenCount));
        job = await Step(job, $"defects/{defectId}/resolve", new NoteRequest("เปลี่ยนบาน"));
        job = await Step(job, $"defects/{defectId}/verify", null, "token-approver");
        job = await Step(job, "ready");

        // Handover: dispute needs a reason and returns to work; acceptance needs an explicit warranty term.
        Assert.Equal("INSTALLATION_REASON_REQUIRED", await ErrorCodeAsync(await Command(job, "handover", new HandoverRequest("disputed", "คุณสมชาย", null, null, null))));
        job = await Step(job, "handover", new HandoverRequest("disputed", "คุณสมชาย", "สีไม่ตรงตัวอย่าง", null, null));
        Assert.Equal(("in_progress", 1, "สีไม่ตรงตัวอย่าง"), (job.Status, job.DisputeCount, job.LastDisputeNote));
        job = await Step(job, "ready");
        Assert.Equal("INSTALLATION_FIELD_INVALID", await ErrorCodeAsync(await Command(job, "handover", new HandoverRequest("accepted", "คุณสมชาย", null, null, null))));
        Assert.Equal("INSTALLATION_FIELD_INVALID", await ErrorCodeAsync(await Command(job, "handover", new HandoverRequest("accepted", "คุณสมชาย", null, 12, Today.AddDays(2)))));
        Assert.Equal("INSTALLATION_FIELD_INVALID", await ErrorCodeAsync(await Command(job, "handover", new HandoverRequest("accepted", "คุณสมชาย", null, 121, null))));
        job = await Step(job, "handover", new HandoverRequest("accepted", "คุณสมชาย", "ตรวจรับแล้ว", 12, null));
        Assert.Equal("handed_over", job.Status);
        Assert.Equal(("คุณสมชาย", 12, Today), (job.Handover!.SignerName, job.Handover.WarrantyMonths, job.Handover.Date));
        Assert.StartsWith("WAR", job.Warranty!.Number);
        Assert.Equal((Today, Today.AddMonths(12).AddDays(-1)), (job.Warranty.StartDate, job.Warranty.EndDate));
        Assert.Equal("INSTALLATION_INVALID_STATE", await ErrorCodeAsync(await Command(job, "cancel", new ReasonRequest("x"))));

        var warranty = await Ok<WarrantyResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/warranties/{job.Warranty.Id}"));
        Assert.Equal((true, 0, job.Number), (warranty.IsActive, warranty.ServiceRequestCount, warranty.InstallationNumber));

        // With the installation handed over the close gate no longer applies.
        control = await GetControlAsync(projectId);
        Assert.NotEqual("PROJECT_OPEN_INSTALLATION", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/transitions", new TransitionProjectRequest("completed", null), ifMatch: control.RowVersion)));

        var list = await Ok<InstallationsListResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/installations?status=handed_over&projectId={projectId}"));
        Assert.Equal((1, 3, 1), (list.Items.Count, list.Items[0].ChecklistTotal, 0 + (list.Items[0].OpenDefects == 0 ? 1 : 0)));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await SendAsync(HttpMethod.Get, "/api/v1/installations?status=bogus")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await SendAsync(HttpMethod.Get, "/api/v1/installations", token: "token-no-perm", membership: MembershipNoPermId)).StatusCode);
    }

    [Fact]
    public async Task Installation_Cancel_AndInvalidInputs()
    {
        var projectId = await CreateProjectAsync();
        Assert.Equal("INSTALLATION_FIELD_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/installations", new InstallationRequest(projectId, Start, Start.AddDays(-1), null, null, null), key: Key())));
        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Post, "/api/v1/installations", new InstallationRequest(Guid.NewGuid(), Start, Start, null, null, null), key: Key())).StatusCode);

        var job = await CreateInstallationAsync(projectId);
        Assert.Equal("INSTALLATION_REASON_REQUIRED", await ErrorCodeAsync(await Command(job, "cancel", new ReasonRequest(" "))));
        job = await Step(job, "cancel", new ReasonRequest("ลูกค้าเลื่อนงาน"));
        Assert.Equal(("cancelled", "ลูกค้าเลื่อนงาน"), (job.Status, job.CancelReason));
        Assert.Equal("INSTALLATION_INVALID_STATE", await ErrorCodeAsync(await Command(job, "start")));

        // A cancelled job does not block closing the project.
        var control = await GetControlAsync(projectId);
        Assert.NotEqual("PROJECT_OPEN_INSTALLATION", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{projectId}/transitions", new TransitionProjectRequest("completed", null), ifMatch: control.RowVersion)));
    }

    [Fact]
    public async Task ServiceRequests_FlagWarranty_AndKeepStatusHistory()
    {
        var projectId = await CreateProjectAsync();
        await ActivateAsync(projectId, 100000m);

        // No warranty yet: the request is accepted but flagged out of warranty.
        var key = Key();
        var first = await Ok<ServiceRequestResponse>(await SendAsync(HttpMethod.Post, "/api/v1/service-requests", new ServiceRequestRequest(projectId, "ประตูตู้ปิดไม่สนิท", "ประตูตู้เสื้อผ้าปิดไม่สนิท", "high"), key: key), HttpStatusCode.Created);
        Assert.StartsWith("SRV", first.Number);
        Assert.Equal((false, null), (first.InWarranty, first.Warranty));
        Assert.Equal(first.Id, (await Ok<ServiceRequestResponse>(await SendAsync(HttpMethod.Post, "/api/v1/service-requests", new ServiceRequestRequest(projectId, "ประตูตู้ปิดไม่สนิท", "ประตูตู้เสื้อผ้าปิดไม่สนิท", "high"), key: key), HttpStatusCode.Created)).Id);
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/service-requests", new ServiceRequestRequest(projectId, "อื่น", "อื่น", "low"), key: key)));
        Assert.Equal("SERVICE_FIELD_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/service-requests", new ServiceRequestRequest(projectId, "x", "y", "bogus"), key: Key())));

        // A handover with a warranty makes later requests in-warranty.
        var job = await CreateInstallationAsync(projectId);
        job = await Step(job, "start");
        foreach (var item in job.Checklist.Where(i => i.Required)) job = await SetDoneAsync(job, item.Id);
        job = await Step(job, "ready");
        job = await Step(job, "handover", new HandoverRequest("accepted", "คุณสมชาย", null, 6, null));
        var covered = await Ok<ServiceRequestResponse>(await SendAsync(HttpMethod.Post, "/api/v1/service-requests", new ServiceRequestRequest(projectId, "ลิ้นชักตก", "ลิ้นชักรางหลุด", "normal"), key: Key()), HttpStatusCode.Created);
        Assert.Equal((true, job.Warranty!.Id), (covered.InWarranty, covered.Warranty!.Id));

        // Status flow with history; each step needs the current version.
        Task<HttpResponseMessage> Move(ServiceRequestResponse r, string path, object? body = null, Guid? version = null) =>
            SendAsync(HttpMethod.Post, $"/api/v1/service-requests/{r.Id}/{path}", body, ifMatch: version ?? r.RowVersion);
        Assert.Equal("SERVICE_FIELD_INVALID", await ErrorCodeAsync(await Move(covered, "schedule", new ScheduleRequest(Today.AddDays(-1)))));
        Assert.Equal("SERVICE_INVALID_STATE", await ErrorCodeAsync(await Move(covered, "resolve", new NoteRequest("x"))));
        Assert.Equal("SERVICE_VERSION_CONFLICT", await ErrorCodeAsync(await Move(covered, "start", null, Guid.NewGuid())));
        var scheduled = await Ok<ServiceRequestResponse>(await Move(covered, "schedule", new ScheduleRequest(Today.AddDays(2))));
        Assert.Equal(("scheduled", Today.AddDays(2)), (scheduled.Status, scheduled.ScheduledDate));
        var started = await Ok<ServiceRequestResponse>(await Move(scheduled, "start"));
        Assert.Equal("SERVICE_REASON_REQUIRED", await ErrorCodeAsync(await Move(started, "resolve", new NoteRequest(" "))));
        var resolved = await Ok<ServiceRequestResponse>(await Move(started, "resolve", new NoteRequest("เปลี่ยนรางลิ้นชัก")));
        var closed = await Ok<ServiceRequestResponse>(await Move(resolved, "close"));
        Assert.Equal("SERVICE_REASON_REQUIRED", await ErrorCodeAsync(await Move(closed, "reopen", new ReasonRequest(""))));
        var reopened = await Ok<ServiceRequestResponse>(await Move(closed, "reopen", new ReasonRequest("ปัญหากลับมา")));
        Assert.Equal(("in_progress", 1), (reopened.Status, reopened.ReopenCount));
        Assert.Equal(new[] { "open", "scheduled", "in_progress", "resolved", "closed", "in_progress" }, reopened.Events.Select(e => e.ToStatus).ToArray());
        Assert.Equal("SERVICE_INVALID_STATE", await ErrorCodeAsync(await Move(reopened, "close")));

        var warranty = await Ok<WarrantyResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/warranties/{job.Warranty.Id}"));
        Assert.Equal(1, warranty.ServiceRequestCount);
        var inWarranty = await Ok<ServiceRequestsListResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/service-requests?projectId={projectId}&inWarranty=true"));
        Assert.Single(inWarranty.Items);
        Assert.Equal(2, (await Ok<ServiceRequestsListResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/service-requests?projectId={projectId}"))).Items.Count);

        // Once the warranty has ended, new requests are flagged out of warranty again.
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE service.warranties SET start_date = '2020-01-01', end_date = '2020-06-30' WHERE id = {job.Warranty.Id}");
        }

        var late = await Ok<ServiceRequestResponse>(await SendAsync(HttpMethod.Post, "/api/v1/service-requests", new ServiceRequestRequest(projectId, "หลังหมดประกัน", "แจ้งหลังหมดประกัน", "low"), key: Key()), HttpStatusCode.Created);
        Assert.False(late.InWarranty);
        Assert.Single((await Ok<WarrantiesListResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/warranties?state=expired&projectId={projectId}"))).Items);
        Assert.Empty((await Ok<WarrantiesListResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/warranties?state=active&projectId={projectId}"))).Items);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await SendAsync(HttpMethod.Get, "/api/v1/warranties?state=bogus")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await SendAsync(HttpMethod.Get, "/api/v1/service-requests", token: "token-no-perm", membership: MembershipNoPermId)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await SendAsync(HttpMethod.Get, $"/api/v1/warranties/{job.Warranty.Id}", token: "token-no-perm", membership: MembershipNoPermId)).StatusCode);
    }
}
