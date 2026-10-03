using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.Surveys;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Files;
using TanErp.Domain.Surveys;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class SiteSurveyEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private static readonly Guid OrgAId = TestOnlyDataSeeder.TestOrgId;
    private static readonly Guid BranchAId = TestOnlyDataSeeder.TestBranchId;
    private static readonly Guid MembershipAId = TestOnlyDataSeeder.TestMembershipId;
    private static readonly Guid UserAId = TestOnlyDataSeeder.TestUserId;
    private const string UidA = TestOnlyDataSeeder.TestFirebaseUid;

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<string?>(idToken == "token-org-a" ? UidA : null);
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
                    ["ConnectionStrings:Database"] = _postgres.GetConnectionString()
                });
            });
            builder.ConfigureServices(services =>
            {
                var dbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (dbDescriptor != null) services.Remove(dbDescriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(_postgres.GetConnectionString()));

                var fbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFirebaseTokenVerifier));
                if (fbDescriptor != null) services.Remove(fbDescriptor);
                services.AddSingleton<IFirebaseTokenVerifier, TestFirebaseTokenVerifier>();
            });
        });

        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true);
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private HttpRequestMessage CreateAuthenticatedRequest(HttpMethod method, string url, string token, Guid membershipId)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Membership-Id", membershipId.ToString());
        return request;
    }

    private async Task<(Guid customerId, Guid siteId, Guid opportunityId, Guid oppVersion)> SetupQualifiedOpportunityAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTimeOffset.UtcNow;

        var customer = Customer.CreateDraft(
            Guid.NewGuid(), OrgAId, UserAId, CustomerType.Person, "คุณลูกค้า ตัวอย่าง TEST_ONLY", null, "th",
            new PrimaryContactInput("คุณสมชาย", null, "0811111111", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        db.Customers.Add(customer);

        var address = new SiteAddressInput("123 ถนนสุขุมวิท", "คลองเตย", "คลองเตย", "กรุงเทพมหานคร", "10110", "TH");
        var site = Site.CreateActive(Guid.NewGuid(), OrgAId, customer.Id, UserAId, "บ้านพักอาศัย", address, 13.7m, 100.5m, null, now);
        db.Sites.Add(site);

        var opp = Opportunity.CreateDraft(
            Guid.NewGuid(), OrgAId, BranchAId, customer.Id, site.Id, UserAId, UserAId,
            "งานบิลท์อินห้องนอน", "ขอบเขตงานตู้เสื้อผ้าและเตียง", new[] { "built-in" }, null, 350000m, "THB",
            new DateOnly(2026, 12, 31), now.AddDays(2), "นัดเข้าวัดพื้นที่", now);
        opp.Qualify(opp.RowVersion);
        db.Opportunities.Add(opp);

        await db.SaveChangesAsync();

        return (customer.Id, site.Id, opp.Id, opp.RowVersion);
    }

    [Fact]
    public async Task CreateSurvey_ValidQualifiedOpportunity_CreatesSurveyAndRevisionsAndTransitionsToSurveying()
    {
        var (_, siteId, oppId, oppVersion) = await SetupQualifiedOpportunityAsync();
        var now = DateTimeOffset.UtcNow;
        var start = now.AddDays(1);
        var end = start.AddHours(2);

        var requestMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{oppId}/surveys",
            "token-org-a",
            MembershipAId);
        requestMsg.Headers.Add("Idempotency-Key", "idemp-survey-create-0001");
        requestMsg.Content = JsonContent.Create(new CreateSiteSurveyRequest(
            siteId,
            UserAId,
            start,
            end,
            oppVersion));

        var response = await _client.SendAsync(requestMsg);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var survey = await response.Content.ReadFromJsonAsync<SiteSurveyResponse>();
        Assert.NotNull(survey);
        Assert.Equal(oppId, survey.OpportunityId);
        Assert.Equal(siteId, survey.SiteId);
        Assert.Equal(UserAId, survey.AssignedSurveyorId);
        Assert.Equal(SiteSurveyStatus.Scheduled, survey.Status);
        Assert.StartsWith("SRV-", survey.SurveyNumber);
        Assert.NotNull(survey.CurrentRevision);
        Assert.Equal(1, survey.CurrentRevision.RevisionNumber);
        Assert.Equal(SurveyRevisionStatus.Draft, survey.CurrentRevision.Status);
        Assert.Equal(SurveyDefaults.CurrentTemplateVersion, survey.CurrentRevision.SurveyTemplateVersion);

        // Verify Opportunity transitioned to Surveying in DB
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dbOpp = await db.Opportunities.SingleAsync(o => o.Id == oppId);
        Assert.Equal(OpportunityStage.Surveying, dbOpp.Stage);
        Assert.NotEqual(oppVersion, dbOpp.RowVersion);

        // Verify Stage History recorded
        var stageHistory = await db.OpportunityStageHistories
            .Where(h => h.OpportunityId == oppId && h.ToStage == OpportunityStage.Surveying)
            .SingleOrDefaultAsync();
        Assert.NotNull(stageHistory);
        Assert.Equal(OpportunityStage.Qualified, stageHistory.FromStage);

        // Verify GET endpoint works
        var getReq = CreateAuthenticatedRequest(
            HttpMethod.Get,
            $"/api/v1/opportunities/{oppId}/surveys",
            "token-org-a",
            MembershipAId);
        var getResp = await _client.SendAsync(getReq);
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);
        var getSurvey = await getResp.Content.ReadFromJsonAsync<SiteSurveyResponse>();
        Assert.NotNull(getSurvey);
        Assert.Equal(survey.Id, getSurvey.Id);
        Assert.Equal(survey.SurveyNumber, getSurvey.SurveyNumber);
    }

    [Fact]
    public async Task CreateSurvey_ScheduleEndBeforeStart_Returns422()
    {
        var (_, siteId, oppId, oppVersion) = await SetupQualifiedOpportunityAsync();
        var now = DateTimeOffset.UtcNow;
        var start = now.AddDays(1);
        var end = start.AddHours(-1); // End before start

        var requestMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{oppId}/surveys",
            "token-org-a",
            MembershipAId);
        requestMsg.Headers.Add("Idempotency-Key", "idemp-survey-create-invalid-sched");
        requestMsg.Content = JsonContent.Create(new CreateSiteSurveyRequest(
            siteId,
            UserAId,
            start,
            end,
            oppVersion));

        var response = await _client.SendAsync(requestMsg);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDraftAndMarkReady_CompleteFlow_TransitionsOpportunityToEstimating()
    {
        var (_, siteId, oppId, oppVersion) = await SetupQualifiedOpportunityAsync();
        var now = DateTimeOffset.UtcNow;
        var start = now.AddDays(1);
        var end = start.AddHours(2);

        // 1. Create survey
        var createReq = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{oppId}/surveys",
            "token-org-a",
            MembershipAId);
        createReq.Headers.Add("Idempotency-Key", "idemp-survey-flow-0001");
        createReq.Content = JsonContent.Create(new CreateSiteSurveyRequest(
            siteId,
            UserAId,
            start,
            end,
            oppVersion));

        var createResp = await _client.SendAsync(createReq);
        Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);
        var survey = await createResp.Content.ReadFromJsonAsync<SiteSurveyResponse>();
        Assert.NotNull(survey);
        Assert.NotNull(survey.CurrentRevision);
        var revisionId = survey.CurrentRevision.Id;
        var revVersion = survey.CurrentRevision.RowVersion;

        // 2. Update Draft with Areas & Measurements
        var updateReq = CreateAuthenticatedRequest(
            HttpMethod.Put,
            $"/api/v1/opportunities/{oppId}/surveys/{survey.Id}/revisions/{revisionId}/draft",
            "token-org-a",
            MembershipAId);
        updateReq.Headers.Add("If-Match", $"\"{revVersion}\"");

        var areas = new List<UpdateSurveyAreaRequest>
        {
            new(
                null,
                "AREA-01",
                "ห้องนอนใหญ่",
                "ตู้เสื้อผ้า built-in",
                1,
                new List<UpdateSurveyMeasurementRequest>
                {
                    new(null, "width", 3.2m, "m", "measured", "ความกว้างผนัง", 1),
                    new(null, "height", 2.6m, "m", "measured", "ความสูงฝ้า", 2),
                    new(null, "depth", 0.6m, "m", "measured", "ความลึกตู้", 3)
                })
        };

        updateReq.Content = JsonContent.Create(new UpdateSurveyDraftRequest(
            revVersion,
            now,
            "สำรวจและวัดระยะห้องนอนใหญ่",
            new List<string> { "ผนังปูนฉาบเรียบ" },
            new List<string> { "มีเบรกเกอร์แอร์ที่ผนังด้านขวา" },
            new List<string>(),
            areas,
            FullChecklist(),
            new List<UpdateSurveyEvidenceRequest> { new(await SeedEvidenceFileAsync(oppId), EvidenceKind.SitePhoto, "ผนังด้านขวา", 1) }));

        var updateResp = await _client.SendAsync(updateReq);
        Assert.Equal(HttpStatusCode.OK, updateResp.StatusCode);

        var updatedRev = await updateResp.Content.ReadFromJsonAsync<SiteSurveyRevisionResponse>();
        Assert.NotNull(updatedRev);
        Assert.Equal("สำรวจและวัดระยะห้องนอนใหญ่", updatedRev.ScopeSummary);
        Assert.NotNull(updatedRev.Areas);
        Assert.Single(updatedRev.Areas);
        Assert.Equal("ห้องนอนใหญ่", updatedRev.Areas[0].Name);
        Assert.Equal(3, updatedRev.Areas[0].Measurements.Count);
        var newRevVersion = updatedRev.RowVersion;

        // 3. Mark Ready
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var oppInDb = await db.Opportunities.AsNoTracking().SingleAsync(o => o.Id == oppId);
            var currentOppVersion = oppInDb.RowVersion;

            var markReadyReq = CreateAuthenticatedRequest(
                HttpMethod.Post,
                $"/api/v1/opportunities/{oppId}/surveys/{survey.Id}/revisions/{revisionId}/mark-ready",
                "token-org-a",
                MembershipAId);
            markReadyReq.Headers.Add("Idempotency-Key", "idemp-survey-mark-ready-0001");
            markReadyReq.Content = JsonContent.Create(new MarkSurveyReadyRequest(
                newRevVersion,
                currentOppVersion));

            var markReadyResp = await _client.SendAsync(markReadyReq);
            Assert.Equal(HttpStatusCode.OK, markReadyResp.StatusCode);

            var readyRev = await markReadyResp.Content.ReadFromJsonAsync<SiteSurveyRevisionResponse>();
            Assert.NotNull(readyRev);
            Assert.Equal(SurveyRevisionStatus.Ready, readyRev.Status);
            Assert.Equal(SurveyReadiness.Ready, readyRev.Readiness);
            Assert.NotNull(readyRev.SnapshotHash);
            Assert.StartsWith("v3:", readyRev.SnapshotHash);

            // Verify Opportunity transitioned to Estimating
            var finalOpp = await db.Opportunities.AsNoTracking().SingleAsync(o => o.Id == oppId);
            Assert.Equal(OpportunityStage.Estimating, finalOpp.Stage);


            var estimatingHistory = await db.OpportunityStageHistories
                .Where(h => h.OpportunityId == oppId && h.ToStage == OpportunityStage.Estimating)
                .SingleOrDefaultAsync();
            Assert.NotNull(estimatingHistory);
            Assert.Equal(OpportunityStage.Surveying, estimatingHistory.FromStage);
        }
    }


    private static List<UpdateSurveyChecklistRequest> FullChecklist() => new()
    {
        new(SurveyChecklistItems.SiteAccessConfirmed, ChecklistResultValue.Pass, null),
        new(SurveyChecklistItems.UtilitiesChecked, ChecklistResultValue.NotApplicable, "ไม่มีงานระบบ"),
        new(SurveyChecklistItems.ExistingConditionsInspected, ChecklistResultValue.Pass, null),
        new(SurveyChecklistItems.CustomerRequirementsConfirmed, ChecklistResultValue.Pass, null)
    };

    private async Task<Guid> SeedEvidenceFileAsync(Guid opportunityId, string parentType = FileParentTypes.Opportunity)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTimeOffset.UtcNow;

        var session = new FileUploadSession(
            Guid.NewGuid(), OrgAId, parentType, opportunityId, null, UserAId, now, now.AddHours(1),
            Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"));
        db.FileUploadSessions.Add(session);

        var file = new UploadedFile(
            Guid.NewGuid(), OrgAId, $"{OrgAId}/{session.Id}/evidence.webp", "evidence.webp", "image/webp", 2048,
            session.Id.ToString(), UserAId, now, contentSha256: Guid.NewGuid().ToString("N"));
        db.UploadedFiles.Add(file);

        await db.SaveChangesAsync();
        return file.Id;
    }

    private async Task<HttpResponseMessage> PutDraftAsync(Guid oppId, Guid surveyId, SiteSurveyRevisionResponse revision, UpdateSurveyDraftRequest body)
    {
        var request = CreateAuthenticatedRequest(
            HttpMethod.Put,
            $"/api/v1/opportunities/{oppId}/surveys/{surveyId}/revisions/{revision.Id}/draft",
            "token-org-a",
            MembershipAId);
        request.Headers.Add("If-Match", $"\"{revision.RowVersion}\"");
        request.Content = JsonContent.Create(body);
        return await _client.SendAsync(request);
    }

    private static UpdateSurveyDraftRequest DraftBody(
        SiteSurveyRevisionResponse revision,
        List<UpdateSurveyChecklistRequest>? checklist,
        List<UpdateSurveyEvidenceRequest>? evidence) => new(
            revision.RowVersion,
            DateTimeOffset.UtcNow,
            "สำรวจห้องนอน",
            new List<string> { "ผนังปูน" },
            new List<string>(),
            new List<string>(),
            new List<UpdateSurveyAreaRequest>
            {
                new(null, "AREA-01", "ห้องนอน", null, 1, new List<UpdateSurveyMeasurementRequest>
                {
                    new(null, "width", 3.2m, "m", "measured", null, 1)
                })
            },
            checklist,
            evidence);

    private async Task<HttpResponseMessage> PostAsync(string url, string idempotencyKey, object body)
    {
        var request = CreateAuthenticatedRequest(HttpMethod.Post, url, "token-org-a", MembershipAId);
        request.Headers.Add("Idempotency-Key", idempotencyKey);
        request.Content = JsonContent.Create(body);
        return await _client.SendAsync(request);
    }

    private async Task<(Guid oppId, Guid surveyId, SiteSurveyRevisionResponse ready)> CreateReadySurveyAsync(string keyPrefix)
    {
        var (_, siteId, oppId, oppVersion) = await SetupQualifiedOpportunityAsync();
        var now = DateTimeOffset.UtcNow;

        var createResp = await PostAsync(
            $"/api/v1/opportunities/{oppId}/surveys",
            $"{keyPrefix}-create-0001",
            new CreateSiteSurveyRequest(siteId, UserAId, now.AddDays(1), now.AddDays(1).AddHours(2), oppVersion));
        Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);
        var survey = (await createResp.Content.ReadFromJsonAsync<SiteSurveyResponse>())!;
        var draft = survey.CurrentRevision!;

        var evidenceFileId = await SeedEvidenceFileAsync(oppId);
        var updateResp = await PutDraftAsync(oppId, survey.Id, draft, DraftBody(
            draft,
            FullChecklist(),
            new List<UpdateSurveyEvidenceRequest> { new(evidenceFileId, EvidenceKind.SitePhoto, "มุมห้อง", 1) }));
        Assert.Equal(HttpStatusCode.OK, updateResp.StatusCode);
        var updated = (await updateResp.Content.ReadFromJsonAsync<SiteSurveyRevisionResponse>())!;

        var ready = await MarkReadyAsync(oppId, survey.Id, updated, $"{keyPrefix}-ready-0001");
        return (oppId, survey.Id, ready);
    }

    private async Task<SiteSurveyRevisionResponse> MarkReadyAsync(Guid oppId, Guid surveyId, SiteSurveyRevisionResponse revision, string idempotencyKey)
    {
        Guid oppVersion;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            oppVersion = (await db.Opportunities.AsNoTracking().SingleAsync(o => o.Id == oppId)).RowVersion;
        }

        var response = await PostAsync(
            $"/api/v1/opportunities/{oppId}/surveys/{surveyId}/revisions/{revision.Id}/mark-ready",
            idempotencyKey,
            new MarkSurveyReadyRequest(revision.RowVersion, oppVersion));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<SiteSurveyRevisionResponse>())!;
    }

    [Fact]
    public async Task CloneVoidAndReReady_FullLifecycle_SupersedesAndKeepsHistory()
    {
        var (oppId, surveyId, ready1) = await CreateReadySurveyAsync("rev-lifecycle");
        var revisionsUrl = $"/api/v1/opportunities/{oppId}/surveys/{surveyId}/revisions";

        // Clone requires a reason
        var noReason = await PostAsync(revisionsUrl, "rev-lifecycle-clone-noreason", new CloneSurveyRevisionRequest(ready1.Id, " "));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, noReason.StatusCode);

        // Clone the ready revision into a new draft; source stays untouched
        var cloneBody = new CloneSurveyRevisionRequest(ready1.Id, "ลูกค้าแก้แบบ");
        var cloneResp = await PostAsync(revisionsUrl, "rev-lifecycle-clone-0001", cloneBody);
        Assert.Equal(HttpStatusCode.Created, cloneResp.StatusCode);
        var clone = (await cloneResp.Content.ReadFromJsonAsync<SiteSurveyRevisionResponse>())!;
        Assert.Equal(2, clone.RevisionNumber);
        Assert.Equal(SurveyRevisionStatus.Draft, clone.Status);
        Assert.Null(clone.SnapshotHash);
        Assert.Equal(ready1.ScopeSummary, clone.ScopeSummary);
        Assert.Single(clone.Areas!);
        Assert.NotEqual(ready1.Areas![0].Id, clone.Areas![0].Id);

        // Replay with same key returns the same revision; a second draft is rejected
        var replay = await PostAsync(revisionsUrl, "rev-lifecycle-clone-0001", cloneBody);
        Assert.Equal(HttpStatusCode.Created, replay.StatusCode);
        Assert.Equal(clone.Id, (await replay.Content.ReadFromJsonAsync<SiteSurveyRevisionResponse>())!.Id);

        var second = await PostAsync(revisionsUrl, "rev-lifecycle-clone-0002", cloneBody);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);

        // Mark the clone ready: previous ready revision is superseded, opportunity stays estimating
        var ready2 = await MarkReadyAsync(oppId, surveyId, clone, "rev-lifecycle-ready-0002");
        Assert.Equal(SurveyRevisionStatus.Ready, ready2.Status);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var first = await db.SiteSurveyRevisions.AsNoTracking().SingleAsync(r => r.Id == ready1.Id);
            Assert.Equal(SurveyRevisionStatus.Superseded, first.Status);
            Assert.Equal(ready1.SnapshotHash, first.SnapshotHash);
            Assert.Equal(OpportunityStage.Estimating, (await db.Opportunities.AsNoTracking().SingleAsync(o => o.Id == oppId)).Stage);
        }

        // Void needs a reason, then succeeds once and is not repeatable
        var voidUrl = $"{revisionsUrl}/{ready2.Id}/void";
        var voidNoReason = await PostAsync(voidUrl, "rev-lifecycle-void-noreason", new VoidSurveyRevisionRequest(ready2.RowVersion, ""));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, voidNoReason.StatusCode);

        var voidResp = await PostAsync(voidUrl, "rev-lifecycle-void-0001", new VoidSurveyRevisionRequest(ready2.RowVersion, "วัดผิดห้อง"));
        Assert.Equal(HttpStatusCode.OK, voidResp.StatusCode);
        var voided = (await voidResp.Content.ReadFromJsonAsync<SiteSurveyRevisionResponse>())!;
        Assert.Equal(SurveyRevisionStatus.Void, voided.Status);

        var voidAgain = await PostAsync(voidUrl, "rev-lifecycle-void-0002", new VoidSurveyRevisionRequest(voided.RowVersion, "ซ้ำ"));
        Assert.Equal(HttpStatusCode.Conflict, voidAgain.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var row = await db.SiteSurveyRevisions.AsNoTracking().SingleAsync(r => r.Id == ready2.Id);
            Assert.Equal("วัดผิดห้อง", row.VoidReason);
            Assert.NotNull(row.VoidedAtUtc);
            Assert.True(await db.AuditEvents.AnyAsync(a => a.Action == "survey.revision-voided" && a.ResourceId == ready2.Id.ToString()));
        }
    }

    [Fact]
    public async Task CloneRevision_DraftSource_Returns409()
    {
        var (oppId, surveyId, ready) = await CreateReadySurveyAsync("rev-draftsrc");
        var revisionsUrl = $"/api/v1/opportunities/{oppId}/surveys/{surveyId}/revisions";

        var cloneResp = await PostAsync(revisionsUrl, "rev-draftsrc-clone-0001", new CloneSurveyRevisionRequest(ready.Id, "แก้ไข"));
        var draft = (await cloneResp.Content.ReadFromJsonAsync<SiteSurveyRevisionResponse>())!;

        var response = await PostAsync(revisionsUrl, "rev-draftsrc-clone-0002", new CloneSurveyRevisionRequest(draft.Id, "แก้ไขอีก"));
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task MarkReady_MissingChecklistOrEvidence_Returns422AndStaysDraft()
    {
        var (_, siteId, oppId, oppVersion) = await SetupQualifiedOpportunityAsync();
        var now = DateTimeOffset.UtcNow;
        var createResp = await PostAsync(
            $"/api/v1/opportunities/{oppId}/surveys",
            "rev-gate-create-0001",
            new CreateSiteSurveyRequest(siteId, UserAId, now.AddDays(1), now.AddDays(1).AddHours(2), oppVersion));
        var survey = (await createResp.Content.ReadFromJsonAsync<SiteSurveyResponse>())!;
        var draft = survey.CurrentRevision!;

        // Measurements only: no checklist and no evidence
        var updated = (await (await PutDraftAsync(oppId, survey.Id, draft, DraftBody(draft, null, null)))
            .Content.ReadFromJsonAsync<SiteSurveyRevisionResponse>())!;

        var markUrl = $"/api/v1/opportunities/{oppId}/surveys/{survey.Id}/revisions/{updated.Id}/mark-ready";
        Guid oppVersionNow;
        using (var scope = _factory.Services.CreateScope())
        {
            oppVersionNow = (await scope.ServiceProvider.GetRequiredService<AppDbContext>().Opportunities.AsNoTracking().SingleAsync(o => o.Id == oppId)).RowVersion;
        }

        var noChecklist = await PostAsync(markUrl, "rev-gate-ready-0001", new MarkSurveyReadyRequest(updated.RowVersion, oppVersionNow));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, noChecklist.StatusCode);

        // Checklist answered but a non-pass answer without a note is still rejected
        var checklist = FullChecklist();
        checklist[1] = new(SurveyChecklistItems.UtilitiesChecked, ChecklistResultValue.Fail, null);
        var withChecklist = (await (await PutDraftAsync(oppId, survey.Id, updated, DraftBody(updated, checklist, null)))
            .Content.ReadFromJsonAsync<SiteSurveyRevisionResponse>())!;
        var noNote = await PostAsync(markUrl, "rev-gate-ready-0002", new MarkSurveyReadyRequest(withChecklist.RowVersion, oppVersionNow));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, noNote.StatusCode);

        // Complete checklist but no evidence file
        var withNote = (await (await PutDraftAsync(oppId, survey.Id, withChecklist, DraftBody(withChecklist, FullChecklist(), null)))
            .Content.ReadFromJsonAsync<SiteSurveyRevisionResponse>())!;
        var noEvidence = await PostAsync(markUrl, "rev-gate-ready-0003", new MarkSurveyReadyRequest(withNote.RowVersion, oppVersionNow));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, noEvidence.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var row = await db.SiteSurveyRevisions.AsNoTracking().SingleAsync(r => r.Id == updated.Id);
            Assert.Equal(SurveyRevisionStatus.Draft, row.Status);
            Assert.Null(row.SnapshotHash);
            Assert.Equal(OpportunityStage.Surveying, (await db.Opportunities.AsNoTracking().SingleAsync(o => o.Id == oppId)).Stage);
        }
    }

    [Fact]
    public async Task UpdateDraft_InvalidChecklistOrForeignEvidence_Returns422()
    {
        var (_, siteId, oppId, oppVersion) = await SetupQualifiedOpportunityAsync();
        var now = DateTimeOffset.UtcNow;
        var createResp = await PostAsync(
            $"/api/v1/opportunities/{oppId}/surveys",
            "rev-invalid-create-0001",
            new CreateSiteSurveyRequest(siteId, UserAId, now.AddDays(1), now.AddDays(1).AddHours(2), oppVersion));
        var survey = (await createResp.Content.ReadFromJsonAsync<SiteSurveyResponse>())!;
        var draft = survey.CurrentRevision!;

        var unknownItem = await PutDraftAsync(oppId, survey.Id, draft, DraftBody(
            draft,
            new List<UpdateSurveyChecklistRequest> { new("not_a_template_item", ChecklistResultValue.Pass, null) },
            null));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, unknownItem.StatusCode);

        // A file uploaded for a different parent type cannot be used as evidence
        var wrongParentFile = await SeedEvidenceFileAsync(oppId, FileParentTypes.Customer);
        var foreign = await PutDraftAsync(oppId, survey.Id, draft, DraftBody(
            draft,
            null,
            new List<UpdateSurveyEvidenceRequest> { new(wrongParentFile, EvidenceKind.SitePhoto, null, 1) }));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, foreign.StatusCode);

        var badKind = await PutDraftAsync(oppId, survey.Id, draft, DraftBody(
            draft,
            null,
            new List<UpdateSurveyEvidenceRequest> { new(Guid.NewGuid(), "video", null, 1) }));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, badKind.StatusCode);
    }

    [Fact]
    public async Task CloneRevision_CopiesChecklistAndEvidence_AndTemplateEndpointListsCurrentVersion()
    {
        var (oppId, surveyId, ready) = await CreateReadySurveyAsync("rev-copy");
        Assert.StartsWith("v3:", ready.SnapshotHash);

        var cloneResp = await PostAsync(
            $"/api/v1/opportunities/{oppId}/surveys/{surveyId}/revisions",
            "rev-copy-clone-0001",
            new CloneSurveyRevisionRequest(ready.Id, "แก้แบบ"));
        var clone = (await cloneResp.Content.ReadFromJsonAsync<SiteSurveyRevisionResponse>())!;

        Assert.Equal(4, clone.Checklist!.Count);
        Assert.Single(clone.Evidence!);
        Assert.Equal(ready.Evidence![0].FileId, clone.Evidence![0].FileId);
        Assert.NotEqual(ready.Evidence[0].Id, clone.Evidence[0].Id);
        Assert.Equal(ready.SurveyTemplateVersion, clone.SurveyTemplateVersion);

        var templatesReq = CreateAuthenticatedRequest(HttpMethod.Get, "/api/v1/survey-template-versions", "token-org-a", MembershipAId);
        var templatesResp = await _client.SendAsync(templatesReq);
        Assert.Equal(HttpStatusCode.OK, templatesResp.StatusCode);
        var templates = (await templatesResp.Content.ReadFromJsonAsync<SurveyTemplateVersionListResponse>())!;
        var current = Assert.Single(templates.Items, t => t.IsCurrent);
        Assert.Equal(SurveyDefaults.CurrentTemplateVersion, current.Code);
        Assert.Equal(4, current.RequiredChecklistItems.Count);
        Assert.Equal(1, current.MinimumEvidenceCount);
    }

    [Fact]
    public async Task NewReadyRevisionAndVoid_LeaveEstimateReferenceUntouched_AndAuditTheRisk()
    {
        var (oppId, surveyId, ready1) = await CreateReadySurveyAsync("rev-estimate");
        Guid estimateId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var opp = await db.Opportunities.AsNoTracking().SingleAsync(o => o.Id == oppId);
            var estimate = TanErp.Domain.Estimates.Estimate.CreateDraft(
                Guid.NewGuid(), OrgAId, BranchAId, opp.CustomerId, oppId, $"EST-{Guid.NewGuid():N}"[..16],
                ready1.Id, ready1.SnapshotHash);
            db.Estimates.Add(estimate);
            await db.SaveChangesAsync();
            estimateId = estimate.Id;
        }

        var revisionsUrl = $"/api/v1/opportunities/{oppId}/surveys/{surveyId}/revisions";
        var clone = (await (await PostAsync(revisionsUrl, "rev-estimate-clone-0001", new CloneSurveyRevisionRequest(ready1.Id, "แก้แบบ")))
            .Content.ReadFromJsonAsync<SiteSurveyRevisionResponse>())!;
        var ready2 = await MarkReadyAsync(oppId, surveyId, clone, "rev-estimate-ready-0002");

        // Void the revision the estimate references: allowed, never re-pointed, risk recorded.
        var voidResp = await PostAsync(
            $"{revisionsUrl}/{ready1.Id}/void",
            "rev-estimate-void-0001",
            new VoidSurveyRevisionRequest((await GetRevisionVersionAsync(ready1.Id)), "วัดผิด"));
        Assert.Equal(HttpStatusCode.OK, voidResp.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var estimate = await db.Estimates.AsNoTracking().SingleAsync(e => e.Id == estimateId);
            Assert.Equal(ready1.Id, estimate.SiteSurveyRevisionId);
            Assert.Equal(ready1.SnapshotHash, estimate.SiteSurveySnapshotHash);

            var original = await db.SiteSurveyRevisions.AsNoTracking().SingleAsync(r => r.Id == ready1.Id);
            Assert.Equal(SurveyRevisionStatus.Void, original.Status);
            Assert.Equal(ready1.SnapshotHash, original.SnapshotHash);

            var latest = await db.SiteSurveyRevisions.AsNoTracking().SingleAsync(r => r.Id == ready2.Id);
            Assert.Equal(SurveyRevisionStatus.Ready, latest.Status);

            var audit = await db.AuditEvents.AsNoTracking()
                .Where(a => a.Action == "survey.revision-voided" && a.ResourceId == ready1.Id.ToString())
                .SingleAsync();
            Assert.Contains("\"referencedByEstimate\":true", audit.ChangesJson.Replace(" ", string.Empty));
        }
    }

    private async Task<Guid> GetRevisionVersionAsync(Guid revisionId)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return (await db.SiteSurveyRevisions.AsNoTracking().SingleAsync(r => r.Id == revisionId)).RowVersion;
    }
}

