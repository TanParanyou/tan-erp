using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.QuickEstimates;
using TanErp.Api.ErrorHandling;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class QuickEstimateEndpointsTests : IAsyncLifetime
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

    private static async Task<T> Ok<T>(HttpResponseMessage response, HttpStatusCode expected = HttpStatusCode.OK)
    {
        Assert.True(response.StatusCode == expected, $"Expected {expected} but got {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    // TEST_ONLY values: they exercise the rules and are not business rates.
    private static PricingTemplateRequest TemplateBody(string code = "QE-TEST-WARDROBE", decimal rate = 8000m, decimal directShareLimit = 200000m, decimal minimum = 20000m) => new(
        code, "built-in", "ตู้เสื้อผ้า TEST_ONLY", "area", "m2", rate, minimum, 0.10m, 0.25m, 1000m, 7, 0.07m, "exclusive", directShareLimit, null, null,
        new List<GradeRequest> { new("standard", "Standard", 1.0m), new("premium", "Premium", 1.3m) },
        new List<ComplexityRequest> { new("curved", "งานโค้ง", 1.2m, 0.03m), new("hidden", "ระบบซ่อน", 1.1m, 0.05m) },
        new List<AddOnRequest> { new("delivery", "ค่าขนส่ง", 5000m, false), new("hardware", "อุปกรณ์ต่อชุด", 2000m, true) },
        0.08m, 0.02m, 0.06m, new List<string> { "ขนาดเป็นค่าประมาณ" }, new List<string> { "งานรื้อถอน" });

    private async Task<PricingTemplateResponse> TemplateStepAsync(PricingTemplateResponse t, string path, string token = "token-org-a", object? body = null) =>
        await Ok<PricingTemplateResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/pricing-templates/{t.Id}/{path}", body, token: token, ifMatch: t.RowVersion));

    /// <summary>A template walked through the whole lifecycle: written by A, approved by the second administrator.</summary>
    private async Task<PricingTemplateResponse> ActiveTemplateAsync(string code = "QE-TEST-WARDROBE", string stop = "active", decimal directShareLimit = 200000m)
    {
        var t = await Ok<PricingTemplateResponse>(await SendAsync(HttpMethod.Post, "/api/v1/pricing-templates", TemplateBody(code, directShareLimit: directShareLimit), key: Key()), HttpStatusCode.Created);
        t = await TemplateStepAsync(t, "submit");
        t = await TemplateStepAsync(t, "decision", "token-approver", new TemplateDecisionRequest("approved", null));
        if (stop == "approved") return t;
        t = await TemplateStepAsync(t, "calibration");
        if (stop == "calibration") return t;
        return await TemplateStepAsync(t, "activate");
    }

    private async Task<QuickEstimateResponse> NewEstimateAsync(Guid? opportunityId = null) =>
        await Ok<QuickEstimateResponse>(await SendAsync(HttpMethod.Post, "/api/v1/quick-estimates", new CreateQuickEstimateRequest(null, opportunityId), key: Key()), HttpStatusCode.Created);

    private static QuickEstimateDraftRequest FullDraft(Guid templateId, string grade = "premium", string confidence = "medium", bool custom = false, List<string>? complexity = null) => new(
        templateId, "house", "ห้องนอนใหญ่", grade, complexity ?? new List<string>(), new List<string> { "delivery" }, confidence, custom,
        new List<MeasurementRequest> { new(Guid.Parse("00000000-0000-0000-0000-000000000001"), "wardrobe", 3.00m, 2.60m, null, 1) });

    private async Task<QuickEstimateResponse> PatchAsync(QuickEstimateResponse e, QuickEstimateDraftRequest body, string token = "token-org-a") =>
        await Ok<QuickEstimateResponse>(await SendAsync(HttpMethod.Patch, $"/api/v1/quick-estimates/{e.Id}/draft", body, token: token, ifMatch: e.RowVersion));

    private async Task<QuickEstimateResponse> CalculateAsync(QuickEstimateResponse e) =>
        await Ok<QuickEstimateResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/quick-estimates/{e.Id}/calculate", ifMatch: e.RowVersion));

    private async Task<QuickEstimateResponse> GetEstimateAsync(Guid id) => await Ok<QuickEstimateResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/quick-estimates/{id}"));

    private Task<HttpResponseMessage> ShareAsync(QuickEstimateResponse e, int version, string? key = null, string token = "token-org-a") =>
        SendAsync(HttpMethod.Post, $"/api/v1/quick-estimates/{e.Id}/shares", new ShareRequest(version, "onscreen", null, "th"), token: token, key: key ?? Key());

    [Fact]
    public async Task PricingTemplate_Lifecycle_MakerChecker_Versions_AndEffectiveQuery()
    {
        var created = await Ok<PricingTemplateResponse>(await SendAsync(HttpMethod.Post, "/api/v1/pricing-templates", TemplateBody(), key: "tpl-create-key-000001"), HttpStatusCode.Created);
        Assert.Equal(("QE-TEST-WARDROBE", 1, "draft"), (created.Code, created.Version, created.Status));
        Assert.Equal(created.Id, (await Ok<PricingTemplateResponse>(await SendAsync(HttpMethod.Post, "/api/v1/pricing-templates", TemplateBody(), key: "tpl-create-key-000001"), HttpStatusCode.Created)).Id);
        Assert.Equal("PRICING_TEMPLATE_CODE_EXISTS", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/pricing-templates", TemplateBody(), key: Key())));
        Assert.Equal("PRICING_TEMPLATE_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/pricing-templates", TemplateBody("QE-BAD-1", rate: 0m), key: Key())));
        var badFactor = TemplateBody("QE-BAD-2") with { Grades = new List<GradeRequest> { new("x", "x", 99m) } };
        Assert.Equal("PRICING_TEMPLATE_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, "/api/v1/pricing-templates", badFactor, key: Key())));

        var edited = await Ok<PricingTemplateResponse>(await SendAsync(HttpMethod.Put, $"/api/v1/pricing-templates/{created.Id}", TemplateBody(rate: 8500m), ifMatch: created.RowVersion));
        Assert.Equal(8500m, edited.ReferenceRate);
        Assert.Equal("PRICING_TEMPLATE_VERSION_CONFLICT", await ErrorCodeAsync(await SendAsync(HttpMethod.Put, $"/api/v1/pricing-templates/{created.Id}", TemplateBody(), ifMatch: created.RowVersion)));
        Assert.Equal("PRICING_TEMPLATE_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Put, $"/api/v1/pricing-templates/{created.Id}", TemplateBody("OTHER-CODE"), ifMatch: edited.RowVersion)));

        // Maker–checker: the author cannot decide; a returned template needs a reason and becomes editable again.
        var submitted = await TemplateStepAsync(edited, "submit");
        Assert.Equal("PRICING_TEMPLATE_SELF_APPROVAL", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/pricing-templates/{created.Id}/decision", new TemplateDecisionRequest("approved", null), ifMatch: submitted.RowVersion)));
        Assert.Equal("QUICK_ESTIMATE_REASON_REQUIRED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/pricing-templates/{created.Id}/decision", new TemplateDecisionRequest("returned", " "), token: "token-approver", ifMatch: submitted.RowVersion)));
        Assert.Equal("QUICK_ESTIMATE_FIELD_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/pricing-templates/{created.Id}/decision", new TemplateDecisionRequest("maybe", null), token: "token-approver", ifMatch: submitted.RowVersion)));
        var returned = await TemplateStepAsync(submitted, "decision", "token-approver", new TemplateDecisionRequest("returned", "ปรับ rate"));
        Assert.Equal(("draft", "ปรับ rate"), (returned.Status, returned.DecisionNote));
        returned = await TemplateStepAsync(returned, "submit");
        var approved = await TemplateStepAsync(returned, "decision", "token-approver", new TemplateDecisionRequest("approved", null));
        Assert.Equal("approved", approved.Status);
        Assert.Equal("PRICING_TEMPLATE_INVALID_STATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Put, $"/api/v1/pricing-templates/{created.Id}", TemplateBody(), ifMatch: approved.RowVersion)));

        // Not usable until calibration; the capture form never sees rates or factors.
        Assert.Empty(await Ok<List<EffectiveTemplateResponse>>(await SendAsync(HttpMethod.Get, "/api/v1/pricing-templates/effective?workType=built-in")));
        var calibrating = await TemplateStepAsync(approved, "calibration");
        var effectiveBody = await (await SendAsync(HttpMethod.Get, "/api/v1/pricing-templates/effective?workType=built-in")).Content.ReadAsStringAsync();
        Assert.Contains("QE-TEST-WARDROBE", effectiveBody);
        Assert.Contains("calibration", effectiveBody);
        foreach (var secret in new[] { "referenceRate", "factor", "riskModifier", "minimumCharge", "directShareLimit", "amount" }) Assert.DoesNotContain(secret, effectiveBody, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await SendAsync(HttpMethod.Get, "/api/v1/pricing-templates/effective?workType=bogus")).StatusCode);

        var active = await TemplateStepAsync(calibrating, "activate");
        Assert.Equal("active", active.Status);

        // A new version is a copy; activating it retires the old one, and only one version stays active.
        var v2 = await Ok<PricingTemplateResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/pricing-templates/{active.Id}/versions"), HttpStatusCode.Created);
        Assert.Equal((2, "draft", active.ReferenceRate), (v2.Version, v2.Status, v2.ReferenceRate));
        Assert.Equal("PRICING_TEMPLATE_DRAFT_EXISTS", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/pricing-templates/{active.Id}/versions")));
        v2 = await Ok<PricingTemplateResponse>(await SendAsync(HttpMethod.Put, $"/api/v1/pricing-templates/{v2.Id}", TemplateBody(rate: 9000m), ifMatch: v2.RowVersion));
        v2 = await TemplateStepAsync(v2, "submit");
        v2 = await TemplateStepAsync(v2, "decision", "token-approver", new TemplateDecisionRequest("approved", null));
        v2 = await TemplateStepAsync(v2, "activate");
        Assert.Equal("superseded", (await Ok<PricingTemplateResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/pricing-templates/{active.Id}"))).Status);
        var list = await Ok<PricingTemplateListResponse>(await SendAsync(HttpMethod.Get, "/api/v1/pricing-templates?search=QE-TEST"));
        Assert.Equal(new[] { "active", "superseded" }, list.Items.OrderBy(i => i.Status).Select(i => i.Status).ToArray());

        var disabled = await TemplateStepAsync(v2, "disable");
        Assert.Equal("disabled", disabled.Status);
        Assert.Equal("PRICING_TEMPLATE_INVALID_STATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/pricing-templates/{v2.Id}/activate", ifMatch: disabled.RowVersion)));
        Assert.Equal(HttpStatusCode.Forbidden, (await SendAsync(HttpMethod.Get, "/api/v1/pricing-templates", token: "token-no-perm", membership: MembershipNoPermId)).StatusCode);
    }

    [Fact]
    public async Task QuickEstimate_CalculatesDeterministicRange_VersionsOnChange_AndSharesCustomerSafeSummary()
    {
        var template = await ActiveTemplateAsync();
        var estimate = await NewEstimateAsync();
        Assert.StartsWith("QE", estimate.Number);
        Assert.Equal("draft", estimate.Status);

        // Incomplete input is refused with the stable code and nothing is created.
        estimate = await PatchAsync(estimate, new QuickEstimateDraftRequest(template.Id, "house", "ห้องนอนใหญ่", "premium", null, new List<string> { "delivery" }, "medium", false,
            new List<MeasurementRequest> { new(Guid.NewGuid(), "wardrobe", 3.00m, null, null, 1) }));
        Assert.Equal("QUICK_ESTIMATE_INPUT_INCOMPLETE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/quick-estimates/{estimate.Id}/calculate", ifMatch: estimate.RowVersion)));
        Assert.Equal("QUICK_ESTIMATE_VERSION_CONFLICT", await ErrorCodeAsync(await SendAsync(HttpMethod.Patch, $"/api/v1/quick-estimates/{estimate.Id}/draft", FullDraft(template.Id), ifMatch: Guid.NewGuid())));
        var gold = await PatchAsync(estimate, FullDraft(template.Id, "gold"));
        Assert.Equal("QUICK_ESTIMATE_OPTION_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/quick-estimates/{estimate.Id}/calculate", ifMatch: gold.RowVersion)));
        estimate = gold;

        estimate = await PatchAsync(estimate, FullDraft(template.Id));
        var calculated = await CalculateAsync(estimate);
        var calc = calculated.CurrentCalculation!;
        // 3.0 × 2.6 m² × 8,000 × 1.3 + 5,000 delivery = 86,120; range ±12% rounded outward to 1,000.
        Assert.Equal(("calculated", 1, 86120m, 75000m, 97000m, "shareable"), (calculated.Status, calc.Version, calc.NetAmount, calc.DisplayedLower, calc.DisplayedUpper, calc.ShareDecision));
        Assert.Equal(7.8m, calc.Lines.Single().BillableQuantity);
        Assert.Equal(64, calc.InputHash.Length);

        // Calculating again with nothing changed keeps version 1; a priced change makes version 2 and the old one stays readable.
        calculated = await CalculateAsync(calculated);
        Assert.Equal(1, calculated.CurrentCalculation!.Version);
        var changed = await PatchAsync(calculated, FullDraft(template.Id, "standard"));
        Assert.Equal("draft", changed.Status);
        var second = await CalculateAsync(changed);
        Assert.Equal((2, 67400m), (second.CurrentCalculation!.Version, second.CurrentCalculation.NetAmount));
        var snapshot1 = await Ok<CalculationSnapshotResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/quick-estimates/{estimate.Id}/calculations/1/snapshot"));
        Assert.Contains("86120", snapshot1.SnapshotJson);
        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Get, $"/api/v1/quick-estimates/{estimate.Id}/calculations/9/snapshot")).StatusCode);

        // Share only the current version, once per key, with a summary that carries no internal factors.
        Assert.Equal("QUICK_ESTIMATE_STALE_VERSION", await ErrorCodeAsync(await ShareAsync(second, 1)));
        var key = Key();
        var shared = await Ok<QuickEstimateResponse>(await ShareAsync(second, 2, key), HttpStatusCode.Created);
        var summary = shared.Shares.Single().SummaryJson;
        Assert.Contains("\"lower\":", summary);
        foreach (var internalTerm in new[] { "factor", "referenceRate", "risk", "netAmount", "minimumCharge", "reasons", "hash" }) Assert.DoesNotContain(internalTerm, summary, StringComparison.OrdinalIgnoreCase);
        Assert.Single((await Ok<QuickEstimateResponse>(await ShareAsync(second, 2, key), HttpStatusCode.Created)).Shares);
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/quick-estimates/{estimate.Id}/shares", new ShareRequest(2, "pdf", null, "en"), key: key)));
        Assert.Equal("QUICK_ESTIMATE_FIELD_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/quick-estimates/{estimate.Id}/shares", new ShareRequest(2, "carrier-pigeon", null, "th"), key: Key())));

        // Without a template the draft cannot be priced; a disabled template stops new calculations.
        var bare = await NewEstimateAsync();
        Assert.Equal("QUICK_ESTIMATE_TEMPLATE_REQUIRED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/quick-estimates/{bare.Id}/calculate", ifMatch: bare.RowVersion)));
        var forDisable = await GetEstimateAsync(second.Id);
        await TemplateStepAsync(template, "disable");
        var again = await PatchAsync(forDisable, new QuickEstimateDraftRequest(null, null, "ห้องนอนเล็ก", null, null, null, null, null, null));
        Assert.Equal("PRICING_TEMPLATE_NOT_USABLE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/quick-estimates/{again.Id}/calculate", ifMatch: again.RowVersion)));
        Assert.Equal("PRICING_TEMPLATE_NOT_USABLE", await ErrorCodeAsync(await SendAsync(HttpMethod.Patch, $"/api/v1/quick-estimates/{again.Id}/draft", new QuickEstimateDraftRequest(template.Id, null, null, null, null, null, null, null, null), ifMatch: again.RowVersion)));

        var list = await Ok<QuickEstimateListResponse>(await SendAsync(HttpMethod.Get, "/api/v1/quick-estimates?search=QE"));
        Assert.True(list.Items.Count >= 2);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await SendAsync(HttpMethod.Get, "/api/v1/quick-estimates?status=bogus")).StatusCode);
    }

    [Fact]
    public async Task QuickEstimate_RiskyEstimates_NeedAnIndependentReview_BeforeSharing()
    {
        var template = await ActiveTemplateAsync();
        // Custom material, low confidence and complex work push the range to its maximum: a reviewer must approve it.
        var estimate = await PatchAsync(await NewEstimateAsync(), FullDraft(template.Id, confidence: "low", custom: true, complexity: new List<string> { "curved", "hidden" }));
        var calculated = await CalculateAsync(estimate);
        Assert.Equal("pending_review", calculated.CurrentCalculation!.ShareDecision);
        Assert.Contains("RANGE_AT_MAXIMUM", calculated.CurrentCalculation.ReasonCodes);
        Assert.Equal("QUICK_ESTIMATE_REVIEW_REQUIRED", await ErrorCodeAsync(await ShareAsync(calculated, 1)));

        // Retried submit does not create a second review; edits are locked while it waits.
        var submitted = await Ok<QuickEstimateResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/quick-estimates/{estimate.Id}/submit-review", new SubmitReviewRequest(1, "ขอตรวจวัสดุพิเศษ")));
        Assert.Equal(("pending_review", 1), (submitted.Status, submitted.Reviews.Count));
        Assert.Single((await Ok<QuickEstimateResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/quick-estimates/{estimate.Id}/submit-review", new SubmitReviewRequest(1, "ซ้ำ")))).Reviews);
        Assert.Equal("QUICK_ESTIMATE_INVALID_STATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Patch, $"/api/v1/quick-estimates/{estimate.Id}/draft", FullDraft(template.Id), ifMatch: submitted.RowVersion)));
        Assert.Equal("QUICK_ESTIMATE_REVIEW_REQUIRED", await ErrorCodeAsync(await ShareAsync(submitted, 1)));

        // Maker–checker, then return → edit → recalculate → approve with a second reviewer.
        Assert.Equal("QUICK_ESTIMATE_SELF_REVIEW", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/quick-estimates/{estimate.Id}/review-decisions", new ReviewDecisionRequest(1, "approved", "EVIDENCE_VERIFIED", null))));
        Assert.Equal("QUICK_ESTIMATE_REASON_REQUIRED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/quick-estimates/{estimate.Id}/review-decisions", new ReviewDecisionRequest(1, "returned", " ", null), token: "token-approver")));
        var returned = await Ok<QuickEstimateResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/quick-estimates/{estimate.Id}/review-decisions", new ReviewDecisionRequest(1, "returned", "NEED_PHOTOS", "แนบรูปวัสดุ"), token: "token-approver"));
        Assert.Equal(("returned", "returned"), (returned.Status, returned.Reviews.Single().Status));
        Assert.Equal("QUICK_ESTIMATE_INVALID_STATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, $"/api/v1/quick-estimates/{estimate.Id}/review-decisions", new ReviewDecisionRequest(1, "approved", "OK", null), token: "token-approver")));

        var fixedUp = await PatchAsync(returned, FullDraft(template.Id, confidence: "low", custom: true, complexity: new List<string> { "curved", "hidden" }) with { RoomOrArea = "ห้องนอนใหญ่ (แก้ไข)" });
        var recalculated = await CalculateAsync(fixedUp);
        // The room name is part of what was priced, so the corrected draft is calculation version 2 and needs its own review.
        Assert.Equal(2, recalculated.CurrentCalculation!.Version);
        await Ok<QuickEstimateResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/quick-estimates/{estimate.Id}/submit-review", new SubmitReviewRequest(2, null)));
        var approved = await Ok<QuickEstimateResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/quick-estimates/{estimate.Id}/review-decisions", new ReviewDecisionRequest(2, "approved", "EVIDENCE_VERIFIED", "ตรวจแล้ว"), token: "token-approver"));
        Assert.Equal(("approved", 2), (approved.Status, approved.Reviews.Count));
        Assert.Equal("QUICK_ESTIMATE_STALE_VERSION", await ErrorCodeAsync(await ShareAsync(approved, 1)));
        Assert.Single((await Ok<QuickEstimateResponse>(await ShareAsync(approved, 2), HttpStatusCode.Created)).Shares);

        // A change after approval drops the approval: nothing can be shared from numbers that no longer match.
        var edited = await PatchAsync(approved, FullDraft(template.Id, "standard", "low", true, new List<string> { "curved", "hidden" }));
        Assert.Equal("draft", edited.Status);
        Assert.Equal("QUICK_ESTIMATE_STALE_VERSION", await ErrorCodeAsync(await ShareAsync(edited, 1)));
    }

    [Fact]
    public async Task QuickEstimate_CalibrationTemplates_AlwaysNeedReview_AndAccessIsScoped()
    {
        var calibration = await ActiveTemplateAsync("QE-TEST-CALIB", stop: "calibration");
        var estimate = await CalculateAsync(await PatchAsync(await NewEstimateAsync(), FullDraft(calibration.Id)));
        Assert.Equal("pending_review", estimate.CurrentCalculation!.ShareDecision);
        Assert.Equal(new[] { "TEMPLATE_IN_CALIBRATION" }, estimate.CurrentCalculation.ReasonCodes.ToArray());

        // Above the template's direct-share limit also needs review.
        var strict = await ActiveTemplateAsync("QE-TEST-STRICT", directShareLimit: 90000m);
        var big = await CalculateAsync(await PatchAsync(await NewEstimateAsync(), FullDraft(strict.Id)));
        Assert.Equal(("pending_review", "ABOVE_DIRECT_SHARE_LIMIT"), (big.CurrentCalculation!.ShareDecision, big.CurrentCalculation.ReasonCodes.Single()));

        // Scope and permissions.
        var foreign = await SendAsync(HttpMethod.Get, $"/api/v1/quick-estimates/{estimate.Id}", token: "token-org-b");
        Assert.True(foreign.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Forbidden);
        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Post, "/api/v1/quick-estimates", new CreateQuickEstimateRequest(Guid.NewGuid(), null), key: Key())).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Post, "/api/v1/quick-estimates", new CreateQuickEstimateRequest(null, Guid.NewGuid()), key: Key())).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await SendAsync(HttpMethod.Get, "/api/v1/quick-estimates", token: "token-no-perm", membership: MembershipNoPermId)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await SendAsync(HttpMethod.Post, "/api/v1/quick-estimates", new CreateQuickEstimateRequest(null, null), token: "token-no-perm", membership: MembershipNoPermId, key: Key())).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Get, $"/api/v1/quick-estimates/{Guid.NewGuid()}")).StatusCode);

        // Idempotent creation.
        var key = Key();
        var first = await Ok<QuickEstimateResponse>(await SendAsync(HttpMethod.Post, "/api/v1/quick-estimates", new CreateQuickEstimateRequest(null, null), key: key), HttpStatusCode.Created);
        Assert.Equal(first.Id, (await Ok<QuickEstimateResponse>(await SendAsync(HttpMethod.Post, "/api/v1/quick-estimates", new CreateQuickEstimateRequest(null, null), key: key), HttpStatusCode.Created)).Id);
    }
}
