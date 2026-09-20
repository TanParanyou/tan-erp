using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.Estimates;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Estimates;
using TanErp.Domain.Surveys;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class EstimateEndpointsTests : IAsyncLifetime
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
            var uid = idToken switch
            {
                "token-org-a" => UidA,
                "token-org-b" => TestOnlyDataSeeder.TestFirebaseUidB,
                _ => null
            };
            return Task.FromResult(uid);
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

    private async Task<(Guid customerId, Guid siteId, Guid opportunityId, Guid oppVersion, Guid surveyRevisionId, string snapshotHash)> SetupEstimatingOpportunityAsync(
        string revisionStatus = SurveyRevisionStatus.Ready,
        string stage = OpportunityStage.Estimating,
        Guid? overrideOrgId = null,
        Guid? overrideBranchId = null,
        Guid? overrideUserId = null)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTimeOffset.UtcNow;
        var orgId = overrideOrgId ?? OrgAId;
        var branchId = overrideBranchId ?? (overrideOrgId == TestOnlyDataSeeder.TestOrgBId ? TestOnlyDataSeeder.TestBranchBId : BranchAId);
        var userId = overrideUserId ?? (overrideOrgId == TestOnlyDataSeeder.TestOrgBId ? TestOnlyDataSeeder.TestUserIdB : UserAId);

        var customer = Customer.CreateDraft(
            Guid.NewGuid(), orgId, userId, CustomerType.Person, "คุณลูกค้า สำหรับ Estimate", null, "th",
            new PrimaryContactInput("คุณสมชาย", null, "0812345678", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        db.Customers.Add(customer);

        var address = new SiteAddressInput("123 ถนนสุขุมวิท", "คลองเตย", "คลองเตย", "กรุงเทพมหานคร", "10110", "TH");
        var site = Site.CreateActive(Guid.NewGuid(), orgId, customer.Id, userId, "บ้านพักอาศัย", address, 13.7m, 100.5m, null, now);
        db.Sites.Add(site);

        var opp = Opportunity.CreateDraft(
            Guid.NewGuid(), orgId, branchId, customer.Id, site.Id, userId, userId,
            "งานประเมินราคาบิลท์อิน", "ขอบเขตงานตู้เสื้อผ้า", new[] { "built-in" }, null, 500000m, "THB",
            new DateOnly(2026, 12, 31), now.AddDays(2), "เตรียมประเมินราคา", now);

        if (stage == OpportunityStage.Qualified || stage == OpportunityStage.Surveying || stage == OpportunityStage.Estimating)
        {
            opp.Qualify(opp.RowVersion);
        }
        if (stage == OpportunityStage.Surveying || stage == OpportunityStage.Estimating)
        {
            opp.EnterSurveying(opp.RowVersion, site.Id);
        }
        if (stage == OpportunityStage.Estimating)
        {
            opp.EnterEstimating(opp.RowVersion);
        }
        db.Opportunities.Add(opp);

        var survey = SiteSurvey.CreateAppointment(
            orgId, branchId, opp.Id, site.Id, userId, userId, now, now.AddHours(2), now);
        var revision = SiteSurveyRevision.CreateBaseline(orgId, survey.Id, userId, now);
        var area = new SiteSurveyArea(Guid.NewGuid(), orgId, revision.Id, "AREA-01", "ห้องนอน", null, 1);
        area.AddMeasurement(new SiteSurveyMeasurement(Guid.NewGuid(), orgId, area.Id, "width", 3.0m, "m", "measured", null, 1));
        revision.AddArea(area);
        revision.UpdateDraft(now, "สำรวจห้องนอน", null, null, null);

        var snapshotHash = "hash-ready-survey-rev-001";
        if (revisionStatus == SurveyRevisionStatus.Ready)
        {
            revision.MarkReady(userId, now, snapshotHash);
        }

        db.SiteSurveys.Add(survey);
        db.SiteSurveyRevisions.Add(revision);

        await db.SaveChangesAsync();

        return (customer.Id, site.Id, opp.Id, opp.RowVersion, revision.Id, snapshotHash);
    }

    [Fact]
    public async Task CreateEstimate_ReadySurvey_DerivesCustomerBranchAndSnapshot()
    {
        var (customerId, _, oppId, _, surveyRevId, snapshotHash) = await SetupEstimatingOpportunityAsync();

        var requestMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/v1/estimates",
            "token-org-a",
            MembershipAId);
        requestMsg.Headers.Add("Idempotency-Key", "idemp-estimate-derive-0001");
        requestMsg.Content = JsonContent.Create(new CreateEstimateDraftRequest(
            oppId,
            surveyRevId,
            Currency: "THB"));

        var response = await _client.SendAsync(requestMsg);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var estimate = await response.Content.ReadFromJsonAsync<EstimateDetailResponse>();
        Assert.NotNull(estimate);
        Assert.Equal(oppId, estimate.OpportunityId);
        Assert.Equal(customerId, estimate.CustomerId);
        Assert.Equal(BranchAId, estimate.BranchId);
        Assert.Equal(surveyRevId, estimate.SiteSurveyRevisionId);
        Assert.Equal(snapshotHash, estimate.SiteSurveySnapshotHash);
        Assert.Equal(EstimateStatus.Draft, estimate.Status);
        Assert.StartsWith("EST-", estimate.Number);
        Assert.Equal(1, estimate.CurrentRevisionNo);
        Assert.NotNull(estimate.CurrentRevision);
        Assert.Equal(1, estimate.CurrentRevision.RevisionNo);
        Assert.Equal(EstimateRevisionStatus.Draft, estimate.CurrentRevision.Status);
    }

    [Theory]
    [InlineData("cross-org")]
    [InlineData("wrong-opp")]
    [InlineData("draft-revision")]
    [InlineData("non-estimating-stage")]
    public async Task CreateEstimate_ForgedOrUnreadyRelationship_IsRejected(string scenario)
    {
        Guid oppId;
        Guid surveyRevId;
        HttpStatusCode expectedStatus;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var initialCounter = (await db.DocumentSequenceCounters.AsNoTracking().FirstOrDefaultAsync(c => c.OrganizationId == OrgAId && c.DocumentType == "estimates"))?.CurrentValue ?? 0;
            var initialAuditCount = await db.AuditEvents.CountAsync(a => a.OrganizationId == OrgAId && a.Action == "estimates.created");
            var initialEstimateCount = await db.Estimates.CountAsync(e => e.OrganizationId == OrgAId);

            switch (scenario)
            {
                case "cross-org":
                {
                    // Survey revision belongs to OrgB
                    var (_, _, _, _, otherRevId, _) = await SetupEstimatingOpportunityAsync(overrideOrgId: TestOnlyDataSeeder.TestOrgBId);
                    var (_, _, currentOppId, _, _, _) = await SetupEstimatingOpportunityAsync();
                    oppId = currentOppId;
                    surveyRevId = otherRevId;
                    expectedStatus = HttpStatusCode.NotFound;
                    break;
                }
                case "wrong-opp":
                {
                    // Survey revision belongs to oppB, requested with oppA
                    var (_, _, oppA, _, _, _) = await SetupEstimatingOpportunityAsync();
                    var (_, _, _, _, revB, _) = await SetupEstimatingOpportunityAsync();
                    oppId = oppA;
                    surveyRevId = revB;
                    expectedStatus = HttpStatusCode.NotFound;
                    break;
                }
                case "draft-revision":
                {
                    // Survey revision is Draft, not Ready
                    var (_, _, opp, _, draftRev, _) = await SetupEstimatingOpportunityAsync(revisionStatus: SurveyRevisionStatus.Draft);
                    oppId = opp;
                    surveyRevId = draftRev;
                    expectedStatus = HttpStatusCode.UnprocessableEntity;
                    break;
                }
                case "non-estimating-stage":
                {
                    // Opportunity is in Qualified stage, not Estimating
                    var (_, _, opp, _, rev, _) = await SetupEstimatingOpportunityAsync(stage: OpportunityStage.Qualified);
                    oppId = opp;
                    surveyRevId = rev;
                    expectedStatus = HttpStatusCode.Conflict;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(scenario));
            }

            var requestMsg = CreateAuthenticatedRequest(
                HttpMethod.Post,
                "/api/v1/estimates",
                "token-org-a",
                MembershipAId);
            requestMsg.Headers.Add("Idempotency-Key", $"idemp-forged-{scenario}");
            requestMsg.Content = JsonContent.Create(new CreateEstimateDraftRequest(
                oppId,
                surveyRevId,
                Currency: "THB"));

            var response = await _client.SendAsync(requestMsg);
            Assert.Equal(expectedStatus, response.StatusCode);

            // Assert no number / Estimate / audit consumed
            using var verifyScope = _factory.Services.CreateScope();
            var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();

            var finalCounter = (await verifyDb.DocumentSequenceCounters.AsNoTracking().FirstOrDefaultAsync(c => c.OrganizationId == OrgAId && c.DocumentType == "estimates"))?.CurrentValue ?? 0;
            var finalAuditCount = await verifyDb.AuditEvents.CountAsync(a => a.OrganizationId == OrgAId && a.Action == "estimates.created");
            var finalEstimateCount = await verifyDb.Estimates.CountAsync(e => e.OrganizationId == OrgAId);

            Assert.Equal(initialCounter, finalCounter);
            Assert.Equal(initialAuditCount, finalAuditCount);
            Assert.Equal(initialEstimateCount, finalEstimateCount);
        }
    }

    [Fact]
    public async Task UpdateEstimateDraft_ValidSections_UpdatesAndRotatesRowVersion()
    {
        var (_, _, oppId, _, surveyRevId, _) = await SetupEstimatingOpportunityAsync();

        // 1. Create estimate
        var createMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/v1/estimates",
            "token-org-a",
            MembershipAId);
        createMsg.Headers.Add("Idempotency-Key", "idemp-estimate-create-0002");
        createMsg.Content = JsonContent.Create(new CreateEstimateDraftRequest(
            oppId,
            surveyRevId,
            "THB"));

        var createRes = await _client.SendAsync(createMsg);
        Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);
        var estimate = (await createRes.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        var revision = estimate.CurrentRevision!;

        // 2. Update draft with sections, items, and cost components
        var updateMsg = CreateAuthenticatedRequest(
            HttpMethod.Put,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/draft",
            "token-org-a",
            MembershipAId);
        updateMsg.Headers.Add("If-Match", $"\"{revision.RowVersion}\"");

        var sections = new List<UpdateEstimateSectionDto>
        {
            new(
                null,
                "SEC-01",
                "หมวดงานบิลท์อินห้องนอน",
                "Bedroom Built-in",
                1,
                new List<UpdateEstimateWorkItemDto>
                {
                    new(
                        null,
                        "WI-01",
                        "ตู้เสื้อผ้า 3 บานเปิด",
                        "3-Door Wardrobe",
                        2m,
                        "ตู้",
                        SellingRuleType.Margin,
                        30m, // 30% margin
                        1,
                        new List<UpdateEstimateCostComponentDto>
                        {
                            new(null, CostComponentType.Material, "ไม้โครงและไม้อัด", 10m, "แผ่น", 1500m, "THB", 1),
                            new(null, CostComponentType.Labor, "ค่าช่างประกอบและทำสี", 3m, "วัน", 1200m, "THB", 2)
                        }
                    )
                }
            )
        };

        updateMsg.Content = JsonContent.Create(new UpdateEstimateDraftRequest(
            revision.RowVersion,
            sections));

        var updateRes = await _client.SendAsync(updateMsg);
        Assert.Equal(HttpStatusCode.OK, updateRes.StatusCode);

        var updatedRev = await updateRes.Content.ReadFromJsonAsync<EstimateRevisionResponse>();
        Assert.NotNull(updatedRev);
        Assert.NotEqual(revision.RowVersion, updatedRev.RowVersion);
        Assert.Single(updatedRev.Sections);
        Assert.Equal("SEC-01", updatedRev.Sections[0].Code);
        Assert.Single(updatedRev.Sections[0].WorkItems);
        Assert.Equal("WI-01", updatedRev.Sections[0].WorkItems[0].Code);
        Assert.Equal(2, updatedRev.Sections[0].WorkItems[0].CostComponents.Count);
    }

    [Fact]
    public async Task CalculateEstimate_WithDiscount_ProducesAccurateFinancialSnapshot()
    {
        var (_, _, oppId, _, surveyRevId, _) = await SetupEstimatingOpportunityAsync();

        // 1. Create estimate
        var createMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/v1/estimates",
            "token-org-a",
            MembershipAId);
        createMsg.Headers.Add("Idempotency-Key", "idemp-estimate-create-0003");
        createMsg.Content = JsonContent.Create(new CreateEstimateDraftRequest(
            oppId,
            surveyRevId,
            "THB"));

        var createRes = await _client.SendAsync(createMsg);
        var estimate = (await createRes.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        var revision = estimate.CurrentRevision!;

        // 2. Add Work Item:
        // Quantity: 1
        // Cost: Material = 20,000 THB
        // Margin: 50% => Selling = 20,000 / (1 - 0.5) = 40,000 THB
        var updateMsg = CreateAuthenticatedRequest(
            HttpMethod.Put,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/draft",
            "token-org-a",
            MembershipAId);
        updateMsg.Headers.Add("If-Match", $"\"{revision.RowVersion}\"");

        var sections = new List<UpdateEstimateSectionDto>
        {
            new(
                null,
                "SEC-01",
                "งานห้องรับแขก",
                null,
                1,
                new List<UpdateEstimateWorkItemDto>
                {
                    new(
                        null,
                        "WI-01",
                        "ชั้นวางทีวี",
                        null,
                        1m,
                        "ชุด",
                        SellingRuleType.Margin,
                        50m,
                        1,
                        new List<UpdateEstimateCostComponentDto>
                        {
                            new(null, CostComponentType.Material, "วัสดุปิดผิวลามิเนต", 1m, "ชุด", 20000m, "THB", 1)
                        }
                    )
                }
            )
        };

        updateMsg.Content = JsonContent.Create(new UpdateEstimateDraftRequest(
            revision.RowVersion,
            sections));

        var updateRes = await _client.SendAsync(updateMsg);
        var updatedRev = (await updateRes.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;

        // 3. Calculate with 5,000 THB discount
        // Net Cost = 20,000 THB
        // Selling Before Discount = 40,000 THB
        // Discount = 5,000 THB
        // Net Before Tax = 35,000 THB
        // Tax 7% = 35,000 * 0.07 = 2,450 THB
        // Grand Total = 35,000 + 2,450 = 37,450 THB
        // Margin Amount = 35,000 - 20,000 = 15,000 THB
        // Margin Rate = 15,000 / 35,000 = 0.4286 (42.86%)
        var calcMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/revisions/{updatedRev.Id}/calculate",
            "token-org-a",
            MembershipAId);
        calcMsg.Headers.Add("Idempotency-Key", "idemp-estimate-calc-0001");
        calcMsg.Content = JsonContent.Create(new CalculateEstimateRequest(
            updatedRev.RowVersion,
            DiscountAmount: 5000m));

        var calcRes = await _client.SendAsync(calcMsg);
        Assert.Equal(HttpStatusCode.OK, calcRes.StatusCode);

        var calculatedRev = await calcRes.Content.ReadFromJsonAsync<EstimateRevisionResponse>();
        Assert.NotNull(calculatedRev);
        Assert.Equal(20000m, calculatedRev.NetCost);
        Assert.Equal(40000m, calculatedRev.SellingBeforeDiscount);
        Assert.Equal(5000m, calculatedRev.DiscountAmount);
        Assert.Equal(35000m, calculatedRev.NetBeforeTax);
        Assert.Equal(2450m, calculatedRev.TaxAmount);
        Assert.Equal(37450m, calculatedRev.GrandTotal);
        Assert.Equal(15000m, calculatedRev.MarginAmount);
        Assert.Equal(0.4286m, calculatedRev.MarginRate);
        Assert.Equal(2, calculatedRev.CalculationVersion);
        Assert.NotNull(calculatedRev.CalculationSnapshotJson);
    }

    [Fact]
    public async Task UpdateDraft_OutdatedVersion_Returns409Conflict()
    {
        var (_, _, oppId, _, surveyRevId, _) = await SetupEstimatingOpportunityAsync();

        // 1. Create estimate
        var createMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/v1/estimates",
            "token-org-a",
            MembershipAId);
        createMsg.Headers.Add("Idempotency-Key", "idemp-estimate-create-0004");
        createMsg.Content = JsonContent.Create(new CreateEstimateDraftRequest(
            oppId,
            surveyRevId,
            "THB"));

        var createRes = await _client.SendAsync(createMsg);
        var estimate = (await createRes.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        var revision = estimate.CurrentRevision!;

        // 2. Try to update using an outdated version
        var staleVersion = Guid.NewGuid();
        var updateMsg = CreateAuthenticatedRequest(
            HttpMethod.Put,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/draft",
            "token-org-a",
            MembershipAId);
        updateMsg.Headers.Add("If-Match", $"\"{staleVersion}\"");
        updateMsg.Content = JsonContent.Create(new UpdateEstimateDraftRequest(
            staleVersion,
            new List<UpdateEstimateSectionDto>()));

        var updateRes = await _client.SendAsync(updateMsg);
        Assert.Equal(HttpStatusCode.Conflict, updateRes.StatusCode);
    }

    [Fact]
    public async Task IssueQuotation_And_AcceptQuotation_FullCommercialCycle_AdvancesOpportunityToProposedThenWon()
    {
        var (_, _, oppId, _, surveyRevId, _) = await SetupEstimatingOpportunityAsync();

        // 1. Create estimate
        var createMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/v1/estimates",
            "token-org-a",
            MembershipAId);
        createMsg.Headers.Add("Idempotency-Key", "idemp-commercial-0001");
        createMsg.Content = JsonContent.Create(new CreateEstimateDraftRequest(
            oppId,
            surveyRevId,
            "THB"));

        var createRes = await _client.SendAsync(createMsg);
        Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);
        var estimate = (await createRes.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        var revision = estimate.CurrentRevision!;

        // 2. Update draft with sections, items, and cost components
        var updateMsg = CreateAuthenticatedRequest(
            HttpMethod.Put,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/draft",
            "token-org-a",
            MembershipAId);
        updateMsg.Headers.Add("If-Match", $"\"{revision.RowVersion}\"");
        updateMsg.Content = JsonContent.Create(new UpdateEstimateDraftRequest(
            revision.RowVersion,
            new List<UpdateEstimateSectionDto>
            {
                new(
                    null,
                    "SEC-01",
                    "งาน Built-in ตู้",
                    "Built-in Section",
                    1,
                    new List<UpdateEstimateWorkItemDto>
                    {
                        new(
                            null,
                            "WI-01",
                            "ตู้เสื้อผ้า",
                            "Wardrobe",
                            1,
                            "ชุด",
                            SellingRuleType.Margin,
                            0.20m,
                            1,
                            new List<UpdateEstimateCostComponentDto>
                            {
                                new(null, CostComponentType.Material, "ไม้", 10, "แผ่น", 1000m, "THB", 1)
                            })
                    })
            }));

        var updateRes = await _client.SendAsync(updateMsg);
        Assert.Equal(HttpStatusCode.OK, updateRes.StatusCode);
        var updatedRevision = (await updateRes.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;

        // 3. Calculate with discount
        var calcMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/calculate",
            "token-org-a",
            MembershipAId);
        calcMsg.Headers.Add("Idempotency-Key", "idemp-commercial-calc-0001");
        calcMsg.Content = JsonContent.Create(new CalculateEstimateRequest(updatedRevision.RowVersion, 500m));

        var calcRes = await _client.SendAsync(calcMsg);
        Assert.Equal(HttpStatusCode.OK, calcRes.StatusCode);
        var calculatedRev = (await calcRes.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;
        Assert.True(calculatedRev.GrandTotal > 0);

        // Fetch latest estimate rowVersion
        var getEstimateMsg = CreateAuthenticatedRequest(
            HttpMethod.Get,
            $"/api/v1/estimates/{estimate.Id}",
            "token-org-a",
            MembershipAId);
        var getEstimateRes = await _client.SendAsync(getEstimateMsg);
        var latestEstimate = (await getEstimateRes.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;

        // Fetch latest opportunity rowVersion
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var oppInDb = await db.Opportunities.FirstAsync(o => o.Id == oppId);
        Assert.Equal(OpportunityStage.Estimating, oppInDb.Stage);

        var expectedOppRowVersion = oppInDb.RowVersion;

        // 4. Issue Quotation
        var issueQuoteMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/quotation",
            "token-org-a",
            MembershipAId);
        issueQuoteMsg.Headers.Add("Idempotency-Key", "idemp-commercial-quote-0001");
        issueQuoteMsg.Content = JsonContent.Create(new IssueQuotationRequest(
            latestEstimate.RowVersion,
            expectedOppRowVersion));

        var issueQuoteRes = await _client.SendAsync(issueQuoteMsg);
        var errContent = await issueQuoteRes.Content.ReadAsStringAsync();
        Assert.True(issueQuoteRes.StatusCode == HttpStatusCode.Created, $"Failed with: {issueQuoteRes.StatusCode} - {errContent}");
        var quotation = (await issueQuoteRes.Content.ReadFromJsonAsync<QuotationResponse>())!;

        Assert.StartsWith("QT-", quotation.Number);
        Assert.Equal("issued", quotation.Status);
        Assert.Equal(OpportunityStage.Proposed, quotation.OpportunityStage);
        Assert.Equal(calculatedRev.GrandTotal, quotation.GrandTotal);

        // Verify DB stage and history
        await db.Entry(oppInDb).ReloadAsync();
        Assert.Equal(OpportunityStage.Proposed, oppInDb.Stage);

        var historyList = await db.OpportunityStageHistories
            .Where(h => h.OpportunityId == oppId)
            .OrderBy(h => h.OccurredAtUtc)
            .ToListAsync();
        Assert.Contains(historyList, h => h.FromStage == OpportunityStage.Estimating && h.ToStage == OpportunityStage.Proposed);

        // Verify Idempotency replay returns same quotation
        var replayMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/quotation",
            "token-org-a",
            MembershipAId);
        replayMsg.Headers.Add("Idempotency-Key", "idemp-commercial-quote-0001");
        replayMsg.Content = JsonContent.Create(new IssueQuotationRequest(
            latestEstimate.RowVersion,
            expectedOppRowVersion));

        var replayRes = await _client.SendAsync(replayMsg);
        Assert.Equal(HttpStatusCode.Created, replayRes.StatusCode);
        var replayQuotation = (await replayRes.Content.ReadFromJsonAsync<QuotationResponse>())!;
        Assert.Equal(quotation.QuotationId, replayQuotation.QuotationId);
        Assert.Equal(quotation.Number, replayQuotation.Number);

        // 5. Accept Quotation (Customer Acceptance) -> Won
        var acceptMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/quotation/accept",
            "token-org-a",
            MembershipAId);
        acceptMsg.Headers.Add("Idempotency-Key", "idemp-commercial-accept-0001");
        acceptMsg.Content = JsonContent.Create(new AcceptQuotationRequest(
            quotation.OpportunityRowVersion,
            "ลูกค้ายืนยันตกลงสั่งซื้อตามใบเสนอราคา"));

        var acceptRes = await _client.SendAsync(acceptMsg);
        Assert.Equal(HttpStatusCode.OK, acceptRes.StatusCode);
        var acceptResponse = (await acceptRes.Content.ReadFromJsonAsync<AcceptQuotationResponse>())!;

        Assert.Equal(quotation.QuotationId, acceptResponse.QuotationId);
        Assert.Equal(OpportunityStage.Won, acceptResponse.OpportunityStage);

        // Verify DB stage and history
        await db.Entry(oppInDb).ReloadAsync();
        Assert.Equal(OpportunityStage.Won, oppInDb.Stage);

        var updatedHistories = await db.OpportunityStageHistories
            .Where(h => h.OpportunityId == oppId)
            .OrderBy(h => h.OccurredAtUtc)
            .ToListAsync();
        Assert.Contains(updatedHistories, h => h.FromStage == OpportunityStage.Proposed && h.ToStage == OpportunityStage.Won);
    }

    private async Task<(EstimateDetailResponse estimate, Opportunity opp, EstimateRevisionResponse calculatedRev)> SetupCalculatedEstimateAsync(
        string keyPrefix = "calc-est")
    {
        var (_, _, oppId, _, surveyRevId, _) = await SetupEstimatingOpportunityAsync();

        var createMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/v1/estimates",
            "token-org-a",
            MembershipAId);
        createMsg.Headers.Add("Idempotency-Key", $"{keyPrefix}-create");
        createMsg.Content = JsonContent.Create(new CreateEstimateDraftRequest(
            oppId,
            surveyRevId,
            "THB"));

        var createRes = await _client.SendAsync(createMsg);
        Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);
        var estimate = (await createRes.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        var revision = estimate.CurrentRevision!;

        var updateMsg = CreateAuthenticatedRequest(
            HttpMethod.Put,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/draft",
            "token-org-a",
            MembershipAId);
        updateMsg.Headers.Add("If-Match", $"\"{revision.RowVersion}\"");
        updateMsg.Content = JsonContent.Create(new UpdateEstimateDraftRequest(
            revision.RowVersion,
            new List<UpdateEstimateSectionDto>
            {
                new(
                    null,
                    "SEC-01",
                    "งาน Built-in ตู้",
                    "Built-in Section",
                    1,
                    new List<UpdateEstimateWorkItemDto>
                    {
                        new(
                            null,
                            "WI-01",
                            "ตู้เสื้อผ้า",
                            "Wardrobe",
                            1,
                            "ชุด",
                            SellingRuleType.Margin,
                            0.20m,
                            1,
                            new List<UpdateEstimateCostComponentDto>
                            {
                                new(null, CostComponentType.Material, "ไม้", 10, "แผ่น", 1000m, "THB", 1)
                            })
                    })
            }));

        var updateRes = await _client.SendAsync(updateMsg);
        Assert.Equal(HttpStatusCode.OK, updateRes.StatusCode);
        var updatedRevision = (await updateRes.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;

        var calcMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/calculate",
            "token-org-a",
            MembershipAId);
        calcMsg.Headers.Add("Idempotency-Key", $"{keyPrefix}-calc");
        calcMsg.Content = JsonContent.Create(new CalculateEstimateRequest(updatedRevision.RowVersion, 500m));

        var calcRes = await _client.SendAsync(calcMsg);
        Assert.Equal(HttpStatusCode.OK, calcRes.StatusCode);
        var calculatedRev = (await calcRes.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;

        var getEstimateMsg = CreateAuthenticatedRequest(
            HttpMethod.Get,
            $"/api/v1/estimates/{estimate.Id}",
            "token-org-a",
            MembershipAId);
        var getEstimateRes = await _client.SendAsync(getEstimateMsg);
        var latestEstimate = (await getEstimateRes.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var oppInDb = await db.Opportunities.FirstAsync(o => o.Id == oppId);

        return (latestEstimate, oppInDb, calculatedRev);
    }

    [Fact]
    public async Task IssueQuotation_ReplaySameIntent_ReturnsSameQuotationWithoutDuplicateEffects()
    {
        var (estimate, opp, _) = await SetupCalculatedEstimateAsync($"replay-{Guid.NewGuid():N}");

        var idempotencyKey = $"idemp-quote-replay-{Guid.NewGuid():N}";

        // Initial Issue
        var issueMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/quotation",
            "token-org-a",
            MembershipAId);
        issueMsg.Headers.Add("Idempotency-Key", idempotencyKey);
        issueMsg.Content = JsonContent.Create(new IssueQuotationRequest(
            estimate.RowVersion,
            opp.RowVersion));

        var issueRes = await _client.SendAsync(issueMsg);
        Assert.Equal(HttpStatusCode.Created, issueRes.StatusCode);
        var quotation = (await issueRes.Content.ReadFromJsonAsync<QuotationResponse>())!;

        // Replay same intent (same key, same payload)
        var replayMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/quotation",
            "token-org-a",
            MembershipAId);
        replayMsg.Headers.Add("Idempotency-Key", idempotencyKey);
        replayMsg.Content = JsonContent.Create(new IssueQuotationRequest(
            estimate.RowVersion,
            opp.RowVersion));

        var replayRes = await _client.SendAsync(replayMsg);
        Assert.Equal(HttpStatusCode.Created, replayRes.StatusCode);
        var replayQuotation = (await replayRes.Content.ReadFromJsonAsync<QuotationResponse>())!;

        Assert.Equal(quotation.QuotationId, replayQuotation.QuotationId);
        Assert.Equal(quotation.Number, replayQuotation.Number);

        // Assert exactly one Quotation in DB for this estimate
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var quotationsCount = await db.Quotations.CountAsync(q => q.EstimateId == estimate.Id);
        Assert.Equal(1, quotationsCount);

        var stageHistoryCount = await db.OpportunityStageHistories
            .CountAsync(h => h.OpportunityId == opp.Id && h.ToStage == OpportunityStage.Proposed);
        Assert.Equal(1, stageHistoryCount);

        var auditCount = await db.AuditEvents
            .CountAsync(a => a.Action == "quotations.issued" && a.ResourceId == quotation.QuotationId.ToString());
        Assert.Equal(1, auditCount);

        var idempRecordCount = await db.IdempotencyRecords
            .CountAsync(r => r.ResourceId == quotation.QuotationId.ToString() && r.Operation == "quotations.issue");
        Assert.Equal(1, idempRecordCount);

        // Replay with DIFFERENT payload must be rejected with IDEMPOTENCY_KEY_REUSED (409)
        var conflictMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/quotation",
            "token-org-a",
            MembershipAId);
        conflictMsg.Headers.Add("Idempotency-Key", idempotencyKey);
        conflictMsg.Content = JsonContent.Create(new IssueQuotationRequest(
            Guid.NewGuid(), // different version
            opp.RowVersion));

        var conflictRes = await _client.SendAsync(conflictMsg);
        Assert.Equal(HttpStatusCode.Conflict, conflictRes.StatusCode);
    }

    [Fact]
    public async Task IssueQuotation_TwoEstimates_AllocatesDistinctAtomicNumbers()
    {
        var (estimate1, opp1, _) = await SetupCalculatedEstimateAsync($"atomic1-{Guid.NewGuid():N}");
        var (estimate2, opp2, _) = await SetupCalculatedEstimateAsync($"atomic2-{Guid.NewGuid():N}");

        var issueTask1 = Task.Run(async () =>
        {
            var msg = CreateAuthenticatedRequest(
                HttpMethod.Post,
                $"/api/v1/estimates/{estimate1.Id}/quotation",
                "token-org-a",
                MembershipAId);
            msg.Headers.Add("Idempotency-Key", $"idemp-quote-atomic-1-{Guid.NewGuid():N}");
            msg.Content = JsonContent.Create(new IssueQuotationRequest(
                estimate1.RowVersion,
                opp1.RowVersion));
            return await _client.SendAsync(msg);
        });

        var issueTask2 = Task.Run(async () =>
        {
            var msg = CreateAuthenticatedRequest(
                HttpMethod.Post,
                $"/api/v1/estimates/{estimate2.Id}/quotation",
                "token-org-a",
                MembershipAId);
            msg.Headers.Add("Idempotency-Key", $"idemp-quote-atomic-2-{Guid.NewGuid():N}");
            msg.Content = JsonContent.Create(new IssueQuotationRequest(
                estimate2.RowVersion,
                opp2.RowVersion));
            return await _client.SendAsync(msg);
        });

        var responses = await Task.WhenAll(issueTask1, issueTask2);

        Assert.Equal(HttpStatusCode.Created, responses[0].StatusCode);
        Assert.Equal(HttpStatusCode.Created, responses[1].StatusCode);

        var q1 = (await responses[0].Content.ReadFromJsonAsync<QuotationResponse>())!;
        var q2 = (await responses[1].Content.ReadFromJsonAsync<QuotationResponse>())!;

        Assert.NotEmpty(q1.Number);
        Assert.NotEmpty(q2.Number);
        Assert.NotEqual(q1.Number, q2.Number);
        Assert.NotEqual(q1.QuotationId, q2.QuotationId);
    }

    [Fact]
    public async Task AcceptQuotation_ReplaySameIntent_ReturnsSameWonResultWithoutDuplicateEffects()
    {
        var (estimate, opp, _) = await SetupCalculatedEstimateAsync($"accept-{Guid.NewGuid():N}");

        // 1. Issue quotation to get into Proposed stage
        var issueMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/quotation",
            "token-org-a",
            MembershipAId);
        issueMsg.Headers.Add("Idempotency-Key", $"idemp-quote-pre-accept-{Guid.NewGuid():N}");
        issueMsg.Content = JsonContent.Create(new IssueQuotationRequest(
            estimate.RowVersion,
            opp.RowVersion));

        var issueRes = await _client.SendAsync(issueMsg);
        Assert.Equal(HttpStatusCode.Created, issueRes.StatusCode);
        var quotation = (await issueRes.Content.ReadFromJsonAsync<QuotationResponse>())!;

        var idempotencyKey = $"idemp-accept-replay-{Guid.NewGuid():N}";

        // 2. Initial Acceptance
        var acceptMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/quotation/accept",
            "token-org-a",
            MembershipAId);
        acceptMsg.Headers.Add("Idempotency-Key", idempotencyKey);
        acceptMsg.Content = JsonContent.Create(new AcceptQuotationRequest(
            quotation.OpportunityRowVersion,
            "ลูกค้ายืนยันตกลงสั่งซื้อตามใบเสนอราคา"));

        var acceptRes = await _client.SendAsync(acceptMsg);
        Assert.Equal(HttpStatusCode.OK, acceptRes.StatusCode);
        var accepted = (await acceptRes.Content.ReadFromJsonAsync<AcceptQuotationResponse>())!;
        Assert.Equal(OpportunityStage.Won, accepted.OpportunityStage);
        Assert.Equal(quotation.QuotationId, accepted.QuotationId);

        // 3. Replay with SAME idempotency key and same payload
        var replayMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/quotation/accept",
            "token-org-a",
            MembershipAId);
        replayMsg.Headers.Add("Idempotency-Key", idempotencyKey);
        replayMsg.Content = JsonContent.Create(new AcceptQuotationRequest(
            quotation.OpportunityRowVersion,
            "ลูกค้ายืนยันตกลงสั่งซื้อตามใบเสนอราคา"));

        var replayRes = await _client.SendAsync(replayMsg);
        Assert.Equal(HttpStatusCode.OK, replayRes.StatusCode);
        var replayed = (await replayRes.Content.ReadFromJsonAsync<AcceptQuotationResponse>())!;
        Assert.Equal(accepted.QuotationId, replayed.QuotationId);
        Assert.Equal(OpportunityStage.Won, replayed.OpportunityStage);

        // Verify database: Exactly 1 Won history, 1 quotations.accepted audit, 1 idempotency record
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var wonHistoryCount = await db.OpportunityStageHistories
            .CountAsync(h => h.OpportunityId == opp.Id && h.ToStage == OpportunityStage.Won);
        Assert.Equal(1, wonHistoryCount);

        var quoteAuditCount = await db.AuditEvents
            .CountAsync(a => a.Action == "quotations.accepted" && a.ResourceId == quotation.QuotationId.ToString());
        Assert.Equal(1, quoteAuditCount);

        var idempRecordCount = await db.IdempotencyRecords
            .CountAsync(r => r.ResourceId == quotation.QuotationId.ToString() && r.Operation == "quotations.accept");
        Assert.Equal(1, idempRecordCount);

        // 4. Replay with DIFFERENT payload under same key must be rejected (409 Conflict)
        var diffPayloadMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/quotation/accept",
            "token-org-a",
            MembershipAId);
        diffPayloadMsg.Headers.Add("Idempotency-Key", idempotencyKey);
        diffPayloadMsg.Content = JsonContent.Create(new AcceptQuotationRequest(
            Guid.NewGuid(), // different version
            "หมายเหตุอื่น"));

        var diffPayloadRes = await _client.SendAsync(diffPayloadMsg);
        Assert.Equal(HttpStatusCode.Conflict, diffPayloadRes.StatusCode);

        // 5. Different key after Won must NOT masquerade as replay (must be rejected with 409 Conflict)
        var diffKeyMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/quotation/accept",
            "token-org-a",
            MembershipAId);
        diffKeyMsg.Headers.Add("Idempotency-Key", $"idemp-different-key-{Guid.NewGuid():N}");
        diffKeyMsg.Content = JsonContent.Create(new AcceptQuotationRequest(
            quotation.OpportunityRowVersion,
            "สั่งซื้อใหม่อีกรอบ"));

        var diffKeyRes = await _client.SendAsync(diffKeyMsg);
        Assert.Equal(HttpStatusCode.Conflict, diffKeyRes.StatusCode);
    }
}
