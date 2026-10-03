using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TanErp.Api;
using TanErp.Api.Contracts.Estimates;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Estimates;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;
using TanErp.Domain.Surveys;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class EstimateEndpointsTests : IAsyncLifetime
{
    private PostgreSqlContainer? _postgres;
    private string _connectionString = string.Empty;
    private string? _externalAdminConnectionString;
    private string? _externalDatabaseName;

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
                "token-estimate-reviewer" => TestOnlyDataSeeder.TestEstimateReviewerFirebaseUid,
                "token-test-manager" => "test-only-approval-0",
                "token-branch-outsider" => "test-only-branch-outsider",
                _ => null
            };
            return Task.FromResult(uid);
        }
    }

    public async Task InitializeAsync()
    {
        var externalConnectionString = Environment.GetEnvironmentVariable("TANERP_TEST_POSTGRES_CONNECTION");
        if (!string.IsNullOrWhiteSpace(externalConnectionString))
        {
            _externalAdminConnectionString = externalConnectionString;
            _externalDatabaseName = $"tan_erp_estimate_test_{Guid.NewGuid():N}";
            _connectionString = await CreateExternalTestDatabaseAsync(
                externalConnectionString,
                _externalDatabaseName);
        }
        else
        {
            _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
            await _postgres.StartAsync();
            _connectionString = _postgres.GetConnectionString();
        }

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Database"] = _connectionString
                });
            });
            builder.ConfigureServices(services =>
            {
                var dbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (dbDescriptor != null) services.Remove(dbDescriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(_connectionString));

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
        if (_postgres is not null)
            await _postgres.DisposeAsync();
        if (_externalAdminConnectionString is not null && _externalDatabaseName is not null)
            await DropExternalTestDatabaseAsync(_externalAdminConnectionString, _externalDatabaseName);
    }

    private static async Task<string> CreateExternalTestDatabaseAsync(string adminConnectionString, string databaseName)
    {
        var adminConnectionBuilder = new NpgsqlConnectionStringBuilder(adminConnectionString);
        var databaseConnectionBuilder = new NpgsqlConnectionStringBuilder(adminConnectionString)
        {
            Database = databaseName
        };

        await using var connection = new NpgsqlConnection(adminConnectionBuilder.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand($"CREATE DATABASE \"{databaseName}\"", connection);
        await command.ExecuteNonQueryAsync();
        return databaseConnectionBuilder.ConnectionString;
    }

    private static async Task DropExternalTestDatabaseAsync(string adminConnectionString, string databaseName)
    {
        var databaseConnectionBuilder = new NpgsqlConnectionStringBuilder(adminConnectionString)
        {
            Database = databaseName
        };
        await using var databaseConnection = new NpgsqlConnection(databaseConnectionBuilder.ConnectionString);
        NpgsqlConnection.ClearPool(databaseConnection);

        await using var connection = new NpgsqlConnection(adminConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{databaseName}\" WITH (FORCE)", connection);
        await command.ExecuteNonQueryAsync();
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

        db.CustomerAddresses.Add(new CustomerAddress(
            Guid.NewGuid(), customer.Id, orgId, userId, "billing", "สำนักงาน",
            "123 ถนนสุขุมวิท", "คลองเตย", "คลองเตย", "กรุงเทพมหานคร", "10110", "TH", true, now));

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
    public async Task TestOnlyDataSeeder_SeedsEstimateDemoWorkspaceOnlyWhenEnabledAndIsIdempotent()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await TestOnlyDataSeeder.SeedAsync(db, "Development", true, seedEstimateDemoData: true);
        Assert.False(await db.Estimates.AnyAsync(estimate => estimate.Id == TestOnlyDataSeeder.TestEstimateDemoId));

        await TestOnlyDataSeeder.SeedAsync(db, "Test", true, seedEstimateDemoData: true);
        var estimate = await db.Estimates
            .Include(row => row.Revisions)
            .ThenInclude(row => row.Sections)
            .ThenInclude(row => row.WorkItems)
            .ThenInclude(row => row.CostComponents)
            .SingleAsync(row => row.Id == TestOnlyDataSeeder.TestEstimateDemoId);

        Assert.Equal("TEST-ONLY-ESTIMATE-0001", estimate.Number);
        Assert.Equal(EstimateStatus.Draft, estimate.Status);
        Assert.Equal(EstimateRevisionStatus.Draft, estimate.CurrentRevision!.Status);
        Assert.Equal(2, estimate.CurrentRevision.Sections.Single().WorkItems.Count);
        Assert.All(estimate.CurrentRevision.Sections.Single().WorkItems.SelectMany(item => item.CostComponents),
            component => Assert.Equal("TEST_ONLY_DEMO_COST", component.ProvisionalReasonCode));

        await TestOnlyDataSeeder.SeedAsync(db, "Test", true, seedEstimateDemoData: true);
        Assert.Equal(1, await db.Estimates.CountAsync(row => row.Id == TestOnlyDataSeeder.TestEstimateDemoId));
        Assert.Equal(3, await db.OpportunityStageHistories.CountAsync(row => row.OpportunityId == TestOnlyDataSeeder.TestEstimateDemoOpportunityId));
    }

    [Fact]
    public async Task TestOnlyDataSeeder_SeedsItemCatalogMaterialsAndPublishedCostsOnlyWhenEnabled()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await TestOnlyDataSeeder.SeedAsync(db, "Development", true, seedItemCatalogDemoData: true);
        Assert.False(await db.Items.AnyAsync(item => item.Id == TestOnlyDataSeeder.TestItemCatalogPlywoodId));

        await TestOnlyDataSeeder.SeedAsync(db, "Test", true, seedItemCatalogDemoData: true);
        var canonicalItems = await db.Items
            .Where(item => item.Id == TestOnlyDataSeeder.TestItemCatalogPlywoodId ||
                           item.Id == TestOnlyDataSeeder.TestItemCatalogLaminateId ||
                           item.Id == TestOnlyDataSeeder.TestItemCatalogEdgebandId)
            .OrderBy(item => item.Code)
            .ToListAsync();

        Assert.Equal(3, canonicalItems.Count);
        Assert.All(canonicalItems, item =>
        {
            Assert.Equal(ItemType.Material, item.ItemType);
            Assert.Equal(ItemStatus.Active, item.Status);
            Assert.Equal(1, item.AttributesSchemaVersion);
            Assert.Contains("TEST_ONLY", item.Name.English, StringComparison.Ordinal);
        });
        Assert.Equal("18", canonicalItems.Single(item => item.Id == TestOnlyDataSeeder.TestItemCatalogPlywoodId).Attributes!["thickness_mm"]);

        var filterItems = await db.Items
            .Where(item => item.Code.Contains("-FILTER-"))
            .ToListAsync();
        Assert.Equal(72, filterItems.Count);
        Assert.Equal(75, canonicalItems.Count + filterItems.Count);
        Assert.Equal(6, filterItems.Select(item => item.CategoryId).Distinct().Count());
        Assert.Equal(3, filterItems.Select(item => item.BrandId).Distinct().Count());
        Assert.All(filterItems.Where(item => item.ItemType == ItemType.Material).GroupBy(item => item.BrandId), group => Assert.Equal(4, group.Count()));

        var expectedTypeCodes = new[] { "MAT", "LAB", "SVC", "PRD", "SUB", "OTH" };
        foreach (var typeCode in expectedTypeCodes)
        {
            var typeItems = filterItems.Where(item => item.Code.StartsWith($"TEST-{typeCode}-FILTER-", StringComparison.Ordinal)).ToArray();
            Assert.Equal(12, typeItems.Length);
            Assert.Equal(4, typeItems.Count(item => item.Status == ItemStatus.Active));
            Assert.Equal(4, typeItems.Count(item => item.Status == ItemStatus.Draft));
            Assert.Equal(4, typeItems.Count(item => item.Status == ItemStatus.Inactive));
        }

        var catalogFixtureIds = canonicalItems.Concat(filterItems).Select(item => item.Id).ToArray();
        Assert.Equal(27, await db.CostRecords.CountAsync(cost => catalogFixtureIds.Contains(cost.ItemId)));
        Assert.All(await db.CostRecords
                .Where(cost => catalogFixtureIds.Contains(cost.ItemId))
                .ToListAsync(),
            cost =>
            {
                Assert.Equal(CostRecordStatus.Published, cost.Status);
                Assert.StartsWith("TEST_ONLY", cost.SourceReference);
            });
        Assert.Equal(75, await db.ItemBarcodes.CountAsync(barcode => catalogFixtureIds.Contains(barcode.ItemId)));

        await TestOnlyDataSeeder.SeedAsync(db, "Test", true, seedItemCatalogDemoData: true);
        Assert.Equal(72, await db.Items.CountAsync(item => item.Code.Contains("-FILTER-")));
        Assert.Equal(75, await db.Items.CountAsync(item => catalogFixtureIds.Contains(item.Id)));
        Assert.Equal(27, await db.CostRecords.CountAsync(cost => catalogFixtureIds.Contains(cost.ItemId)));
    }

    [Fact]
    public async Task TestOnlyDataSeeder_SeedsEstimateAndCatalogDemoDataTogetherIdempotently()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await TestOnlyDataSeeder.SeedAsync(
            db,
            "Test",
            true,
            seedEstimateDemoData: true,
            seedItemCatalogDemoData: true);

        Assert.True(await db.Estimates.AnyAsync(estimate => estimate.Id == TestOnlyDataSeeder.TestEstimateDemoId));
        Assert.Equal(3, await db.OpportunityStageHistories.CountAsync(history =>
            history.OpportunityId == TestOnlyDataSeeder.TestEstimateDemoOpportunityId));
        Assert.Equal(75, await db.Items.CountAsync(item => item.Code.Contains("-FILTER-") ||
            item.Id == TestOnlyDataSeeder.TestItemCatalogPlywoodId ||
            item.Id == TestOnlyDataSeeder.TestItemCatalogLaminateId ||
            item.Id == TestOnlyDataSeeder.TestItemCatalogEdgebandId));
        Assert.Equal(27, await db.CostRecords.CountAsync(cost =>
            cost.SourceReference != null && cost.SourceReference.StartsWith("TEST_ONLY")));

        await TestOnlyDataSeeder.SeedAsync(
            db,
            "Test",
            true,
            seedEstimateDemoData: true,
            seedItemCatalogDemoData: true);

        Assert.Equal(1, await db.Estimates.CountAsync(estimate => estimate.Id == TestOnlyDataSeeder.TestEstimateDemoId));
        Assert.Equal(3, await db.OpportunityStageHistories.CountAsync(history =>
            history.OpportunityId == TestOnlyDataSeeder.TestEstimateDemoOpportunityId));
        Assert.Equal(75, await db.Items.CountAsync(item => item.Code.Contains("-FILTER-") ||
            item.Id == TestOnlyDataSeeder.TestItemCatalogPlywoodId ||
            item.Id == TestOnlyDataSeeder.TestItemCatalogLaminateId ||
            item.Id == TestOnlyDataSeeder.TestItemCatalogEdgebandId));
        Assert.Equal(27, await db.CostRecords.CountAsync(cost =>
            cost.SourceReference != null && cost.SourceReference.StartsWith("TEST_ONLY")));
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
        var (_, _, oppId, oppVersion, surveyRevId, _) = await SetupEstimatingOpportunityAsync();

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
                        0.30m, // 30% margin rate as a ratio
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
    public async Task UpdateEstimateDraft_FixedPriceWithoutReason_IsRejectedWithoutWriting()
    {
        var estimate = await CreateEstimateDraftAsync();
        var revision = estimate.CurrentRevision!;
        var request = CreateAuthenticatedRequest(HttpMethod.Put,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/draft", "token-org-a", MembershipAId);
        request.Headers.Add("If-Match", $"\"{revision.RowVersion}\"");
        request.Content = JsonContent.Create(new UpdateEstimateDraftRequest(revision.RowVersion,
            CreateFixedPriceSections(null)));

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("ESTIMATE_FIXED_PRICE_REASON_REQUIRED", await response.Content.ReadAsStringAsync());
        var reloaded = await GetEstimateAsync(estimate.Id);
        Assert.Equal(revision.RowVersion, reloaded.CurrentRevision!.RowVersion);
        Assert.Empty(reloaded.CurrentRevision.Sections);
    }

    [Fact]
    public async Task UpdateEstimateDraft_FixedPriceWithoutOverridePermission_IsRejectedWithoutWriting()
    {
        var estimate = await CreateEstimateDraftAsync();
        var revision = estimate.CurrentRevision!;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var permissionId = await db.Permissions.Where(permission => permission.Key == "estimates.override-price")
                .Select(permission => permission.Id).SingleAsync();
            var assignments = await db.RolePermissions.Where(rolePermission =>
                rolePermission.OrganizationId == OrgAId && rolePermission.PermissionId == permissionId).ToListAsync();
            db.RolePermissions.RemoveRange(assignments);
            await db.SaveChangesAsync();
        }

        var request = CreateAuthenticatedRequest(HttpMethod.Put,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/draft", "token-org-a", MembershipAId);
        request.Headers.Add("If-Match", $"\"{revision.RowVersion}\"");
        request.Content = JsonContent.Create(new UpdateEstimateDraftRequest(revision.RowVersion,
            CreateFixedPriceSections("CUSTOMER_AGREED_PRICE")));

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Contains("PERMISSION_DENIED", await response.Content.ReadAsStringAsync());
        var reloaded = await GetEstimateAsync(estimate.Id);
        Assert.Equal(revision.RowVersion, reloaded.CurrentRevision!.RowVersion);
        Assert.Empty(reloaded.CurrentRevision.Sections);
    }

    [Fact]
    public async Task UpdateEstimateDraft_WithFixedPricePermission_PersistsReasonAndRequiresApproval()
    {
        var estimate = await CreateEstimateDraftAsync();
        var editorMembershipId = await CreateBranchScopedEstimateEditorAsync();
        var revision = estimate.CurrentRevision!;
        const string reasonCode = "CUSTOMER_AGREED_PRICE";
        var updateRequest = CreateAuthenticatedRequest(HttpMethod.Put,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/draft", "token-org-a", editorMembershipId);
        updateRequest.Headers.Add("If-Match", $"\"{revision.RowVersion}\"");
        updateRequest.Content = JsonContent.Create(new UpdateEstimateDraftRequest(revision.RowVersion,
            CreateFixedPriceSections(reasonCode)));

        var updateResponse = await _client.SendAsync(updateRequest);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updatedRevision = (await updateResponse.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;
        Assert.Equal(reasonCode, updatedRevision.Sections.Single().WorkItems.Single().SellingRuleReasonCode);

        var calculateRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/calculate", "token-org-a", MembershipAId);
        calculateRequest.Headers.Add("Idempotency-Key", "idemp-estimate-fixed-price-approve-0001");
        calculateRequest.Content = JsonContent.Create(new CalculateEstimateRequest(updatedRevision.RowVersion, 0m));
        var calculateResponse = await _client.SendAsync(calculateRequest);

        Assert.Equal(HttpStatusCode.OK, calculateResponse.StatusCode);
        var calculatedRevision = (await calculateResponse.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;
        Assert.Equal(EstimateReadinessStatus.RequiresAttention, calculatedRevision.Readiness);
        Assert.Contains(calculatedRevision.ReadinessReasons, reason =>
            reason.Code == "ESTIMATE_FIXED_PRICE_OVERRIDE" &&
            reason.TargetId == updatedRevision.Sections.Single().WorkItems.Single().Id);
        Assert.Contains(reasonCode, calculatedRevision.CalculationSnapshotJson!, StringComparison.Ordinal);
    }

    [Fact]
    public async Task FinancialEdit_MarksCalculationOutdatedAndPreservesPriorSnapshot()
    {
        var estimate = await CreateEstimateDraftAsync();
        var revision = estimate.CurrentRevision!;
        var updateRequest = CreateAuthenticatedRequest(HttpMethod.Put,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/draft", "token-org-a", MembershipAId);
        updateRequest.Headers.Add("If-Match", $"\"{revision.RowVersion}\"");
        updateRequest.Content = JsonContent.Create(new UpdateEstimateDraftRequest(
            revision.RowVersion,
            CreateFinancialInputSections(100m)));

        var updateResponse = await _client.SendAsync(updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updatedRevision = (await updateResponse.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;

        var calculateRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/calculate", "token-org-a", MembershipAId);
        calculateRequest.Headers.Add("Idempotency-Key", "idemp-estimate-financial-edit-calculate-0001");
        calculateRequest.Content = JsonContent.Create(new CalculateEstimateRequest(updatedRevision.RowVersion, 0m));
        var calculateResponse = await _client.SendAsync(calculateRequest);
        Assert.Equal(HttpStatusCode.OK, calculateResponse.StatusCode);
        var calculatedRevision = (await calculateResponse.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;
        var savedSnapshotJson = calculatedRevision.CalculationSnapshotJson;
        var savedGrandTotal = calculatedRevision.GrandTotal;
        Assert.False(string.IsNullOrWhiteSpace(savedSnapshotJson));

        var financialEditRequest = CreateAuthenticatedRequest(HttpMethod.Put,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/draft", "token-org-a", MembershipAId);
        financialEditRequest.Headers.Add("If-Match", $"\"{calculatedRevision.RowVersion}\"");
        financialEditRequest.Content = JsonContent.Create(new UpdateEstimateDraftRequest(
            calculatedRevision.RowVersion,
            CreateFinancialInputSections(120m)));
        var financialEditResponse = await _client.SendAsync(financialEditRequest);

        Assert.Equal(HttpStatusCode.OK, financialEditResponse.StatusCode);
        var editedRevision = (await financialEditResponse.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;
        Assert.True(editedRevision.CalculationOutdated);
        Assert.Equal(1, editedRevision.CalculationVersion);
        Assert.Equal(savedSnapshotJson, editedRevision.CalculationSnapshotJson);
        Assert.Equal(savedGrandTotal, editedRevision.GrandTotal);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(1, await db.EstimateCalculationSnapshots.CountAsync(snapshot =>
            snapshot.EstimateRevisionId == revision.Id));
    }

    private async Task<EstimateDetailResponse> CreateEstimateDraftAsync()
    {
        var (_, _, opportunityId, _, surveyRevisionId, _) = await SetupEstimatingOpportunityAsync();
        var request = CreateAuthenticatedRequest(HttpMethod.Post, "/api/v1/estimates", "token-org-a", MembershipAId);
        request.Headers.Add("Idempotency-Key", $"idemp-estimate-create-{Guid.NewGuid():N}");
        request.Content = JsonContent.Create(new CreateEstimateDraftRequest(opportunityId, surveyRevisionId, "THB"));
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
    }

    private async Task<EstimateDetailResponse> GetEstimateAsync(Guid estimateId)
    {
        var request = CreateAuthenticatedRequest(HttpMethod.Get, $"/api/v1/estimates/{estimateId}", "token-org-a", MembershipAId);
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
    }

    private async Task<Guid> CreateBranchScopedEstimateEditorAsync()
    {
        var membershipId = Guid.NewGuid();
        var role = new Role(Guid.NewGuid(), OrgAId, "Branch Estimate Editor", "Branch-scoped estimate editor", isActive: true);
        var membership = new Membership(membershipId, OrgAId, BranchAId, UserAId);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var updatePermissionId = await db.Permissions.Where(permission => permission.Key == "estimates.update")
            .Select(permission => permission.Id).SingleAsync();
        var overridePermissionId = await db.Permissions.Where(permission => permission.Key == "estimates.override-price")
            .Select(permission => permission.Id).SingleAsync();
        db.Roles.Add(role);
        db.Memberships.Add(membership);
        db.MembershipRoles.Add(new MembershipRole(membershipId, role.Id, OrgAId));
        db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), role.Id, OrgAId,
            updatePermissionId, PermissionScope.Organization, OrgAId));
        db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), role.Id, OrgAId,
            overridePermissionId, PermissionScope.Branch, BranchAId, BranchAId));
        await db.SaveChangesAsync();
        return membershipId;
    }

    private static IReadOnlyList<UpdateEstimateSectionDto> CreateFixedPriceSections(string? reasonCode)
        =>
        [
            new(null, "SEC-FIX", "ราคาทดสอบ", null, 1,
            [
                new(null, "WI-FIX", "งานราคาคงที่", null, 1m, "unit", SellingRuleType.FixedPrice, 125m, 1,
                    [new(null, CostComponentType.Material, "TEST_ONLY fixed-price cost", 1m, "unit", 100m,
                        "THB", 1, ProvisionalReasonCode: "TEST_ONLY_COST", ProvisionalNote: "Synthetic integration fixture")],
                    reasonCode,
                    OverrideReasonCode: "TEST_ONLY",
                    OverrideReason: "Synthetic custom work item fixture")
            ])
        ];

    private static IReadOnlyList<UpdateEstimateSectionDto> CreateFinancialInputSections(decimal unitCost) =>
    [
        new(
            null,
            "SEC-FINANCIAL-EDIT",
            "งานทดสอบการแก้ไขราคา",
            "Financial edit test section",
            1,
            [
                new(
                    null,
                    "WI-FINANCIAL-EDIT",
                    "รายการต้นทุนทดสอบ",
                    "Test cost item",
                    1m,
                    "unit",
                    SellingRuleType.Margin,
                    0.20m,
                    1,
                    [new(null, CostComponentType.Material, "ต้นทุนทดสอบ", 1m, "unit", unitCost, "THB", 1,
                        ProvisionalReasonCode: "market-benchmark", ProvisionalNote: "Synthetic integration fixture")],
                    OverrideReasonCode: "TEST_ONLY",
                    OverrideReason: "Synthetic custom work item fixture")
            ])
    ];

    [Fact]
    public async Task CalculateEstimate_WithDiscount_ProducesAccurateFinancialSnapshot()
    {
        var (_, _, oppId, oppVersion, surveyRevId, _) = await SetupEstimatingOpportunityAsync();

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
                        SellingRuleType.FixedPrice,
                        40000m,
                        1,
                        new List<UpdateEstimateCostComponentDto>
                        {
                            new(null, CostComponentType.Material, "วัสดุปิดผิวลามิเนต", 1m, "ชุด", 20000m, "THB", 1,
                                ProvisionalReasonCode: "market-benchmark", ProvisionalNote: "Synthetic integration fixture")
                        },
                        "CUSTOMER_AGREED_PRICE",
                        OverrideReasonCode: "TEST_ONLY",
                        OverrideReason: "Synthetic custom work item fixture")
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
            DiscountType: EstimateDiscount.FixedAmount,
            DiscountValue: 5000m,
            DiscountReasonCode: "TEST_ONLY_DISCOUNT"));

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
        Assert.Equal(1, calculatedRev.CalculationVersion);
        Assert.NotNull(calculatedRev.CalculationSnapshotJson);

        using var snapshotScope = _factory.Services.CreateScope();
        var snapshotDb = snapshotScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var savedSnapshots = await snapshotDb.EstimateCalculationSnapshots
            .Where(snapshot => snapshot.OrganizationId == estimate.OrganizationId &&
                               snapshot.EstimateRevisionId == revision.Id)
            .ToListAsync();
        var savedSnapshot = Assert.Single(savedSnapshots);
        Assert.Equal(1, savedSnapshot.CalculationVersion);
        Assert.Equal(64, savedSnapshot.InputHash.Length);
        using var persistedSnapshotJson = System.Text.Json.JsonDocument.Parse(savedSnapshot.SnapshotJson);
        Assert.Equal(37450m, persistedSnapshotJson.RootElement.GetProperty("grandTotal").GetDecimal());

        var historyMessage = CreateAuthenticatedRequest(
            HttpMethod.Get,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/calculations",
            "token-org-a",
            MembershipAId);
        var historyResponse = await _client.SendAsync(historyMessage);
        Assert.Equal(HttpStatusCode.OK, historyResponse.StatusCode);
        var history = (await historyResponse.Content.ReadFromJsonAsync<List<EstimateCalculationSnapshotResponse>>())!;
        Assert.Single(history);
        Assert.Equal(savedSnapshot.InputHash, history[0].InputHash);
        using var historySnapshotJson = System.Text.Json.JsonDocument.Parse(history[0].SnapshotJson);
        Assert.Equal(37450m, historySnapshotJson.RootElement.GetProperty("grandTotal").GetDecimal());

        var futureEffectiveFrom = DateTimeOffset.UtcNow.AddDays(1);
        var futureCalculationPolicy = new CalculationPolicyVersion(
            Guid.NewGuid(), OrgAId, BranchAId, "TEST_ONLY_CALC", 2,
            "fixed-amount", 10000m, "away-from-zero", futureEffectiveFrom, null, UserAId);
        futureCalculationPolicy.Publish(TestOnlyDataSeeder.TestUserIdB, DateTimeOffset.UtcNow);
        var futureTaxPolicy = new TaxPolicyVersion(
            Guid.NewGuid(), OrgAId, BranchAId, "TEST_ONLY_TAX", 2,
            "exclusive", 0.15m, "TEST_ONLY_FUTURE_VAT", futureEffectiveFrom, null, UserAId);
        futureTaxPolicy.Publish(TestOnlyDataSeeder.TestUserIdB, DateTimeOffset.UtcNow);
        snapshotDb.CalculationPolicyVersions.Add(futureCalculationPolicy);
        snapshotDb.TaxPolicyVersions.Add(futureTaxPolicy);
        await snapshotDb.SaveChangesAsync();

        var reproduceRequest = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/revisions/{updatedRev.Id}/calculate",
            "token-org-a",
            MembershipAId);
        reproduceRequest.Headers.Add("Idempotency-Key", "idemp-estimate-calc-historical-reproduction-0002");
        reproduceRequest.Content = JsonContent.Create(new CalculateEstimateRequest(
            calculatedRev.RowVersion,
            DiscountType: EstimateDiscount.FixedAmount,
            DiscountValue: 5000m,
            DiscountReasonCode: "TEST_ONLY_DISCOUNT"));
        var reproduceResponse = await _client.SendAsync(reproduceRequest);
        Assert.Equal(HttpStatusCode.OK, reproduceResponse.StatusCode);
        var reproducedRevision = (await reproduceResponse.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;
        Assert.Equal(2, reproducedRevision.CalculationVersion);
        Assert.Equal(37450m, reproducedRevision.GrandTotal);
        Assert.Equal("TEST_ONLY_CALC-v1", reproducedRevision.CalculationPolicyVersion);
        Assert.Equal("TEST_ONLY_TAX-v1", reproducedRevision.TaxPolicyVersion);

        var reproducedHistoryRequest = CreateAuthenticatedRequest(
            HttpMethod.Get,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/calculations",
            "token-org-a",
            MembershipAId);
        var reproducedHistoryResponse = await _client.SendAsync(reproducedHistoryRequest);
        Assert.Equal(HttpStatusCode.OK, reproducedHistoryResponse.StatusCode);
        var reproducedHistory = (await reproducedHistoryResponse.Content
            .ReadFromJsonAsync<List<EstimateCalculationSnapshotResponse>>())!;
        Assert.Equal(2, reproducedHistory.Count);
        Assert.Equal(history[0].InputHash, reproducedHistory[0].InputHash);
        Assert.Equal(history[0].SnapshotJson, reproducedHistory[0].SnapshotJson);
        Assert.Equal(history[0].CalculationPolicyVersionId, reproducedHistory[1].CalculationPolicyVersionId);
        Assert.Equal(history[0].TaxPolicyVersionId, reproducedHistory[1].TaxPolicyVersionId);
        using var secondSnapshotJson = System.Text.Json.JsonDocument.Parse(reproducedHistory[1].SnapshotJson);
        Assert.Equal(37450m, secondSnapshotJson.RootElement.GetProperty("grandTotal").GetDecimal());

        var submitRequest = CreateAuthenticatedRequest(
            HttpMethod.Post, $"/api/v1/estimates/{estimate.Id}/submit", "token-org-a", MembershipAId);
        submitRequest.Headers.Add("If-Match", $"\"{estimate.RowVersion}\"");
        submitRequest.Headers.Add("Idempotency-Key", "idemp-estimate-submit-0001");
        submitRequest.Content = JsonContent.Create(new SubmitEstimateRequest(
            1, reproducedRevision.CalculationVersion, "TEST_ONLY approval route"));
        var submitResponse = await _client.SendAsync(submitRequest);
        Assert.True(submitResponse.StatusCode == HttpStatusCode.OK,
            $"Submit failed with {(int)submitResponse.StatusCode}: {await submitResponse.Content.ReadAsStringAsync()}");
        var submittedEstimate = (await submitResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        Assert.Equal(EstimateRevisionStatus.Submitted, submittedEstimate.CurrentRevision!.Status);

        var reviewerQueueRequest = CreateAuthenticatedRequest(
            HttpMethod.Get, "/api/v1/estimates/review-queue?pageNumber=1&pageSize=10",
            "token-org-b", TestOnlyDataSeeder.TestCostReviewerMembershipId);
        var reviewerQueueResponse = await _client.SendAsync(reviewerQueueRequest);
        Assert.Equal(HttpStatusCode.OK, reviewerQueueResponse.StatusCode);
        var reviewerQueue = (await reviewerQueueResponse.Content.ReadFromJsonAsync<EstimateReviewQueueResponse>())!;
        var queuedEstimate = Assert.Single(reviewerQueue.Items);
        Assert.Equal(estimate.Id, queuedEstimate.EstimateId);
        Assert.Equal(revision.Id, queuedEstimate.RevisionId);
        Assert.Equal("SYSTEM_BOOTSTRAP_INDEPENDENT_CHECKER", queuedEstimate.FrozenRoute.PolicyCode);
        Assert.Equal(TestOnlyDataSeeder.TestCostReviewerMembershipId, queuedEstimate.FrozenRoute.Reviewer.MembershipId);
        Assert.Equal("estimates.approve", queuedEstimate.FrozenRoute.PermissionKey);
        var queuedPriceOverride = Assert.Single(queuedEstimate.PriceOverrides);
        Assert.Equal("WI-01", queuedPriceOverride.WorkItemCode);
        Assert.Equal("CUSTOMER_AGREED_PRICE", queuedPriceOverride.ReasonCode);
        Assert.Equal(64, queuedEstimate.CalculationSnapshotHash.Length);
        Assert.Single(queuedEstimate.CostEvidence);
        Assert.True(queuedEstimate.CostEvidence[0].IsProvisional);
        Assert.Null(queuedEstimate.RevisionDiff);

        var unassignedQueueRequest = CreateAuthenticatedRequest(
            HttpMethod.Get, "/api/v1/estimates/review-queue?pageNumber=1&pageSize=10",
            "token-org-a", MembershipAId);
        var unassignedQueueResponse = await _client.SendAsync(unassignedQueueRequest);
        Assert.Equal(HttpStatusCode.OK, unassignedQueueResponse.StatusCode);
        var unassignedQueue = (await unassignedQueueResponse.Content.ReadFromJsonAsync<EstimateReviewQueueResponse>())!;
        Assert.Empty(unassignedQueue.Items);

        var returnRequest = CreateAuthenticatedRequest(
            HttpMethod.Post, $"/api/v1/estimates/{estimate.Id}/review-decisions",
            "token-org-b", TestOnlyDataSeeder.TestCostReviewerMembershipId);
        returnRequest.Headers.Add("If-Match", $"\"{submittedEstimate.RowVersion}\"");
        returnRequest.Headers.Add("Idempotency-Key", "idemp-estimate-review-return-0001");
        returnRequest.Content = JsonContent.Create(new ReviewEstimateRequest(1, "returned", "MISSING_LABOR_COST", "เพิ่มข้อมูลค่าแรงทดสอบ"));
        var returnResponse = await _client.SendAsync(returnRequest);
        Assert.Equal(HttpStatusCode.OK, returnResponse.StatusCode);
        var returnedEstimate = (await returnResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        Assert.Equal(EstimateRevisionStatus.Returned, returnedEstimate.CurrentRevision!.Status);

        var recalculateRequest = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/calculate",
            "token-org-a", MembershipAId);
        recalculateRequest.Headers.Add("Idempotency-Key", "idemp-estimate-return-recalculate-01");
        recalculateRequest.Content = JsonContent.Create(new CalculateEstimateRequest(
            returnedEstimate.CurrentRevision.RowVersion,
            DiscountType: EstimateDiscount.FixedAmount,
            DiscountValue: 5000m,
            DiscountReasonCode: "TEST_ONLY_DISCOUNT"));
        var recalculateResponse = await _client.SendAsync(recalculateRequest);
        Assert.Equal(HttpStatusCode.OK, recalculateResponse.StatusCode);
        var recalculatedRevision = (await recalculateResponse.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;
        Assert.Equal(3, recalculatedRevision.CalculationVersion);

        var resubmitRequest = CreateAuthenticatedRequest(
            HttpMethod.Post, $"/api/v1/estimates/{estimate.Id}/submit", "token-org-a", MembershipAId);
        resubmitRequest.Headers.Add("If-Match", $"\"{returnedEstimate.RowVersion}\"");
        resubmitRequest.Headers.Add("Idempotency-Key", "idemp-estimate-resubmit-0001");
        resubmitRequest.Content = JsonContent.Create(new SubmitEstimateRequest(
            1, recalculatedRevision.CalculationVersion, "TEST_ONLY resubmission"));
        var resubmitResponse = await _client.SendAsync(resubmitRequest);
        Assert.Equal(HttpStatusCode.OK, resubmitResponse.StatusCode);
        var resubmittedEstimate = (await resubmitResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;

        var selfReviewRequest = CreateAuthenticatedRequest(
            HttpMethod.Post, $"/api/v1/estimates/{estimate.Id}/review-decisions", "token-org-a", MembershipAId);
        selfReviewRequest.Headers.Add("If-Match", $"\"{resubmittedEstimate.RowVersion}\"");
        selfReviewRequest.Headers.Add("Idempotency-Key", "idemp-estimate-self-review-0001");
        selfReviewRequest.Content = JsonContent.Create(new ReviewEstimateRequest(1, "approved"));
        var selfReviewResponse = await _client.SendAsync(selfReviewRequest);
        Assert.Equal(HttpStatusCode.Forbidden, selfReviewResponse.StatusCode);

        var checkerReviewRequest = CreateAuthenticatedRequest(
            HttpMethod.Post, $"/api/v1/estimates/{estimate.Id}/review-decisions",
            "token-org-b", TestOnlyDataSeeder.TestCostReviewerMembershipId);
        checkerReviewRequest.Headers.Add("If-Match", $"\"{resubmittedEstimate.RowVersion}\"");
        checkerReviewRequest.Headers.Add("Idempotency-Key", "idemp-estimate-independent-approve-0001");
        checkerReviewRequest.Content = JsonContent.Create(new ReviewEstimateRequest(1, "approved"));
        var checkerReviewResponse = await _client.SendAsync(checkerReviewRequest);
        Assert.Equal(HttpStatusCode.OK, checkerReviewResponse.StatusCode);
        var approvedEstimate = (await checkerReviewResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        Assert.Equal(EstimateRevisionStatus.Approved, approvedEstimate.CurrentRevision!.Status);
        Assert.Equal(1, await snapshotDb.EstimateApprovalSnapshots.CountAsync(snapshot => snapshot.EstimateRevisionId == revision.Id));
        Assert.Equal(2, await snapshotDb.EstimateApprovalRequests.CountAsync(request => request.EstimateRevisionId == revision.Id));
        Assert.Equal(2, await snapshotDb.EstimateApprovalDecisions.CountAsync(decision => decision.EstimateApprovalStepId != Guid.Empty));

        var approvedSnapshot = approvedEstimate.CurrentRevision.CalculationSnapshotJson;
        var approvedDraftWrite = CreateAuthenticatedRequest(HttpMethod.Put,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/draft", "token-org-a", MembershipAId);
        approvedDraftWrite.Headers.Add("If-Match", $"\"{approvedEstimate.CurrentRevision.RowVersion}\"");
        approvedDraftWrite.Content = JsonContent.Create(new UpdateEstimateDraftRequest(
            approvedEstimate.CurrentRevision.RowVersion, []));
        var approvedDraftWriteResponse = await _client.SendAsync(approvedDraftWrite);
        Assert.Equal(HttpStatusCode.Conflict, approvedDraftWriteResponse.StatusCode);
        using (var approvedVerifyScope = _factory.Services.CreateScope())
        {
            var approvedVerifyDb = approvedVerifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var approvedRevision = await approvedVerifyDb.EstimateRevisions.SingleAsync(row => row.Id == revision.Id);
            Assert.Equal(EstimateRevisionStatus.Approved, approvedRevision.Status);
            Assert.Equal(approvedSnapshot, approvedRevision.CalculationSnapshotJson);
        }

        var approvedSnapshotRow = await snapshotDb.EstimateApprovalSnapshots
            .SingleAsync(snapshot => snapshot.EstimateRevisionId == revision.Id);
        var originalApprovalHash = approvedSnapshotRow.CalculationSnapshotHash;
        var inconsistentApprovalHash = originalApprovalHash == new string('0', 64)
            ? new string('1', 64)
            : new string('0', 64);
        var quotationSequenceBeforeMismatch = await snapshotDb.DocumentSequenceCounters
            .Where(counter => counter.OrganizationId == estimate.OrganizationId &&
                              counter.DocumentType == DocumentTypes.Quotations)
            .SumAsync(counter => (long?)counter.CurrentValue) ?? 0;

        await snapshotDb.EstimateApprovalSnapshots
            .Where(snapshot => snapshot.Id == approvedSnapshotRow.Id)
            .ExecuteUpdateAsync(update => update.SetProperty(
                snapshot => snapshot.CalculationSnapshotHash, inconsistentApprovalHash));

        var mismatchedApprovalQuoteRequest = CreateAuthenticatedRequest(
            HttpMethod.Post, $"/api/v1/estimates/{estimate.Id}/quotation", "token-org-a", MembershipAId);
        mismatchedApprovalQuoteRequest.Headers.Add("Idempotency-Key", "idemp-approved-estimate-quote-0001");
        mismatchedApprovalQuoteRequest.Content = JsonContent.Create(new IssueQuotationRequest(
            approvedEstimate.RowVersion,
            oppVersion));
        var mismatchedApprovalQuoteResponse = await _client.SendAsync(mismatchedApprovalQuoteRequest);
        Assert.Equal(HttpStatusCode.Conflict, mismatchedApprovalQuoteResponse.StatusCode);
        using (var problem = System.Text.Json.JsonDocument.Parse(
                   await mismatchedApprovalQuoteResponse.Content.ReadAsStringAsync()))
            Assert.Equal("ESTIMATE_INVALID_STATE", problem.RootElement.GetProperty("code").GetString());
        Assert.NotEqual(originalApprovalHash, inconsistentApprovalHash);
        Assert.Empty(await snapshotDb.Quotations.Where(quotation => quotation.EstimateId == estimate.Id).ToListAsync());
        var opportunityBeforeValidQuote = await snapshotDb.Opportunities.AsNoTracking()
            .SingleAsync(opportunity => opportunity.Id == oppId);
        Assert.Equal(OpportunityStage.Estimating, opportunityBeforeValidQuote.Stage);
        Assert.Equal(quotationSequenceBeforeMismatch, await snapshotDb.DocumentSequenceCounters
            .Where(counter => counter.OrganizationId == estimate.OrganizationId &&
                              counter.DocumentType == DocumentTypes.Quotations)
            .SumAsync(counter => (long?)counter.CurrentValue) ?? 0);

        await snapshotDb.EstimateApprovalSnapshots
            .Where(snapshot => snapshot.Id == approvedSnapshotRow.Id)
            .ExecuteUpdateAsync(update => update.SetProperty(
                snapshot => snapshot.CalculationSnapshotHash, originalApprovalHash));

        var issueQuotationRequest = CreateAuthenticatedRequest(
            HttpMethod.Post, $"/api/v1/estimates/{estimate.Id}/quotation", "token-org-a", MembershipAId);
        issueQuotationRequest.Headers.Add("Idempotency-Key", "idemp-approved-estimate-quote-0001");
        issueQuotationRequest.Content = JsonContent.Create(new IssueQuotationRequest(
            approvedEstimate.RowVersion,
            oppVersion));
        var issueQuotationResponse = await _client.SendAsync(issueQuotationRequest);
        Assert.Equal(HttpStatusCode.Created, issueQuotationResponse.StatusCode);
        using (var quotationJson = System.Text.Json.JsonDocument.Parse(
                   await issueQuotationResponse.Content.ReadAsStringAsync()))
        {
            var responseProperties = quotationJson.RootElement.EnumerateObject()
                .Select(property => property.Name)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();
            var allowedProperties = new[]
            {
                "estimateId",
                "estimateRevisionId",
                "estimateRowVersion",
                "grandTotal",
                "issuedAtUtc",
                "number",
                "opportunityId",
                "opportunityRowVersion",
                "opportunityStage",
                "quotationId",
                "revisionNo",
                "status"
            };

            Assert.Equal(allowedProperties, responseProperties);
        }

        var quotation = (await issueQuotationResponse.Content.ReadFromJsonAsync<QuotationResponse>())!;
        Assert.Equal(estimate.Id, quotation.EstimateId);
        Assert.Equal(revision.Id, quotation.EstimateRevisionId);
        Assert.Equal(37450m, quotation.GrandTotal);

        var quotationSequenceAfterIssue = await snapshotDb.DocumentSequenceCounters
            .Where(counter => counter.OrganizationId == estimate.OrganizationId &&
                              counter.DocumentType == DocumentTypes.Quotations)
            .SumAsync(counter => (long?)counter.CurrentValue) ?? 0;
        var issueQuotationReplayRequest = CreateAuthenticatedRequest(
            HttpMethod.Post, $"/api/v1/estimates/{estimate.Id}/quotation", "token-org-a", MembershipAId);
        issueQuotationReplayRequest.Headers.Add("Idempotency-Key", "idemp-approved-estimate-quote-0001");
        issueQuotationReplayRequest.Content = JsonContent.Create(new IssueQuotationRequest(
            approvedEstimate.RowVersion,
            oppVersion));
        var issueQuotationReplayResponse = await _client.SendAsync(issueQuotationReplayRequest);
        Assert.Equal(HttpStatusCode.Created, issueQuotationReplayResponse.StatusCode);
        var replayedQuotation = (await issueQuotationReplayResponse.Content.ReadFromJsonAsync<QuotationResponse>())!;
        Assert.Equal(quotation.QuotationId, replayedQuotation.QuotationId);
        Assert.Equal(quotation.Number, replayedQuotation.Number);
        Assert.Equal(1, await snapshotDb.Quotations.CountAsync(row => row.EstimateId == estimate.Id));
        Assert.Equal(quotationSequenceAfterIssue, await snapshotDb.DocumentSequenceCounters
            .Where(counter => counter.OrganizationId == estimate.OrganizationId &&
                              counter.DocumentType == DocumentTypes.Quotations)
            .SumAsync(counter => (long?)counter.CurrentValue) ?? 0);
        Assert.Equal(1, await snapshotDb.OpportunityStageHistories.CountAsync(history =>
            history.OpportunityId == oppId && history.FromStage == OpportunityStage.Estimating &&
            history.ToStage == OpportunityStage.Proposed));

        // --- Task 3: Quotation Document Integration Tests ---
        // 1. Property Allowlist & Negative scan
        var docReq = CreateAuthenticatedRequest(
            HttpMethod.Get, $"/api/v1/estimates/{estimate.Id}/quotation/document", "token-org-a", MembershipAId);
        var docRes = await _client.SendAsync(docReq);
        Assert.Equal(HttpStatusCode.OK, docRes.StatusCode);

        var rawDocJson = await docRes.Content.ReadAsStringAsync();
        using (var docParsed = System.Text.Json.JsonDocument.Parse(rawDocJson))
        {
            var rootProps = docParsed.RootElement.EnumerateObject().Select(p => p.Name).OrderBy(n => n).ToArray();
            var expectedRootProps = new[]
            {
                "currency",
                "customer",
                "hasIncompleteTranslations",
                "issuedAtUtc",
                "locale",
                "number",
                "sections",
                "totals"
            }.OrderBy(n => n).ToArray();
            Assert.Equal(expectedRootProps, rootProps);

            var customerProps = docParsed.RootElement.GetProperty("customer").EnumerateObject().Select(p => p.Name).OrderBy(n => n).ToArray();
            var expectedCustomerProps = new[]
            {
                "address",
                "branchCode",
                "customerType",
                "displayName",
                "displayNameEn",
                "displayNameTh",
                "legalName",
                "taxIdentifier"
            }.OrderBy(n => n).ToArray();
            Assert.Equal(expectedCustomerProps, customerProps);

            var sectionsElem = docParsed.RootElement.GetProperty("sections");
            Assert.True(sectionsElem.GetArrayLength() > 0);
            var firstSection = sectionsElem[0];
            var sectionProps = firstSection.EnumerateObject().Select(p => p.Name).OrderBy(n => n).ToArray();
            var expectedSectionProps = new[]
            {
                "code",
                "name",
                "nameEn",
                "nameTh",
                "subtotal",
                "workItems"
            }.OrderBy(n => n).ToArray();
            Assert.Equal(expectedSectionProps, sectionProps);

            var workItemsElem = firstSection.GetProperty("workItems");
            Assert.True(workItemsElem.GetArrayLength() > 0);
            var firstItem = workItemsElem[0];
            var itemProps = firstItem.EnumerateObject().Select(p => p.Name).OrderBy(n => n).ToArray();
            var expectedItemProps = new[]
            {
                "code",
                "description",
                "descriptionEn",
                "descriptionTh",
                "lineTotal",
                "quantity",
                "unitCode",
                "unitPrice"
            }.OrderBy(n => n).ToArray();
            Assert.Equal(expectedItemProps, itemProps);

            var totalsProps = docParsed.RootElement.GetProperty("totals").EnumerateObject().Select(p => p.Name).OrderBy(n => n).ToArray();
            var expectedTotalsProps = new[]
            {
                "discountAmount",
                "discountType",
                "discountValue",
                "grandTotal",
                "netBeforeTax",
                "subtotal",
                "taxAmount"
            }.OrderBy(n => n).ToArray();
            Assert.Equal(expectedTotalsProps, totalsProps);
        }

        // Negative scan: Forbidden fields and internal fixture constants MUST NOT appear in payload
        var forbiddenTerms = new[]
        {
            "unitCost",
            "totalCost",
            "marginAmount",
            "marginRate",
            "markupRate",
            "sellingRuleType",
            "sellingRuleValue",
            "sellingRuleReasonCode",
            "internalNote",
            "overrideReasonCode",
            "overrideReason",
            "costRecordId",
            "snapshotHash",
            "rowVersion",
            "capturedByUserId",
            "customerId",
            "Synthetic custom work item fixture",
            "Synthetic integration fixture"
        };
        foreach (var forbidden in forbiddenTerms)
        {
            Assert.DoesNotContain(forbidden, rawDocJson, StringComparison.OrdinalIgnoreCase);
        }

        // 2. Totals match calculation snapshot and quotation
        var doc = (await docRes.Content.ReadFromJsonAsync<QuotationDocumentResponse>())!;
        Assert.Equal(quotation.Number, doc.Number);
        Assert.Equal("THB", doc.Currency);
        Assert.Equal("th", doc.Locale);
        Assert.False(doc.HasIncompleteTranslations);
        Assert.Equal(quotation.GrandTotal, doc.Totals.GrandTotal);
        Assert.Equal(40000m, doc.Totals.Subtotal);
        Assert.Equal(5000m, doc.Totals.DiscountAmount);
        Assert.Equal(35000m, doc.Totals.NetBeforeTax);
        Assert.Equal(2450m, doc.Totals.TaxAmount);

        // 3. Locale=en when descriptionEn is missing: does NOT fall back silently, flags incomplete translation
        var docEnReq = CreateAuthenticatedRequest(
            HttpMethod.Get, $"/api/v1/estimates/{estimate.Id}/quotation/document?locale=en", "token-org-a", MembershipAId);
        var docEnRes = await _client.SendAsync(docEnReq);
        Assert.Equal(HttpStatusCode.OK, docEnRes.StatusCode);
        var docEn = (await docEnRes.Content.ReadFromJsonAsync<QuotationDocumentResponse>())!;
        Assert.Equal("en", docEn.Locale);
        Assert.True(docEn.HasIncompleteTranslations);
        Assert.Null(docEn.Sections[0].WorkItems[0].Description);
        Assert.Equal("ชั้นวางทีวี", docEn.Sections[0].WorkItems[0].DescriptionTh);

        // 4. Immutability: Mutate Customer master in DB after quotation issue, document payload remains unchanged
        var customerBeforeMutation = doc.Customer.DisplayNameTh;
        await snapshotDb.Customers
            .Where(c => c.Id == estimate.CustomerId)
            .ExecuteUpdateAsync(u => u
                .SetProperty(c => c.DisplayNameTh, "ชื่อลูกค้าที่ถูกแก้ไขในภายหลัง")
                .SetProperty(c => c.LegalName, "บริษัท ถูกแก้ไข จำกัด"));

        var docAfterMutationReq = CreateAuthenticatedRequest(
            HttpMethod.Get, $"/api/v1/estimates/{estimate.Id}/quotation/document", "token-org-a", MembershipAId);
        var docAfterMutationRes = await _client.SendAsync(docAfterMutationReq);
        Assert.Equal(HttpStatusCode.OK, docAfterMutationRes.StatusCode);
        var docAfterMutation = (await docAfterMutationRes.Content.ReadFromJsonAsync<QuotationDocumentResponse>())!;
        Assert.Equal(customerBeforeMutation, docAfterMutation.Customer.DisplayNameTh);
        Assert.NotEqual("ชื่อลูกค้าที่ถูกแก้ไขในภายหลัง", docAfterMutation.Customer.DisplayNameTh);

        // 4b. Document is internally consistent with the stored snapshot values (no FE/BE recomputation drift)
        Assert.All(doc.Sections, section =>
            Assert.Equal(section.Subtotal, section.WorkItems.Sum(w => w.LineTotal)));
        Assert.Equal(doc.Totals.Subtotal, doc.Sections.Sum(s => s.Subtotal));

        // 4c. Multiple quotations for one estimate: the latest issued quotation is returned deterministically
        var issuedQuotationRow = await snapshotDb.Quotations.AsNoTracking()
            .SingleAsync(row => row.Id == quotation.QuotationId);
        const string laterQuotationNumber = "QT-TEST-LATER-0001";
        snapshotDb.Quotations.Add(new TanErp.Domain.Commercial.Quotation(
            Guid.NewGuid(),
            issuedQuotationRow.OrganizationId,
            issuedQuotationRow.BranchId,
            issuedQuotationRow.CustomerId,
            issuedQuotationRow.OpportunityId,
            issuedQuotationRow.EstimateId,
            issuedQuotationRow.EstimateRevisionId,
            laterQuotationNumber,
            issuedQuotationRow.TotalAmount,
            issuedQuotationRow.SnapshotHash,
            issuedQuotationRow.IssuedAtUtc.AddDays(1),
            issuedQuotationRow.CustomerBillingSnapshotJson,
            issuedQuotationRow.CustomerBillingSnapshotHash));
        await snapshotDb.SaveChangesAsync();
        var latestDocRes = await _client.SendAsync(CreateAuthenticatedRequest(
            HttpMethod.Get, $"/api/v1/estimates/{estimate.Id}/quotation/document", "token-org-a", MembershipAId));
        Assert.Equal(HttpStatusCode.OK, latestDocRes.StatusCode);
        var latestDoc = (await latestDocRes.Content.ReadFromJsonAsync<QuotationDocumentResponse>())!;
        Assert.Equal(laterQuotationNumber, latestDoc.Number);
        await snapshotDb.Quotations
            .Where(row => row.Number == laterQuotationNumber)
            .ExecuteDeleteAsync();

        // 5. Cross organization -> 404
        var crossOrgDocReq = CreateAuthenticatedRequest(
            HttpMethod.Get, $"/api/v1/estimates/{estimate.Id}/quotation/document", "token-org-b", TestOnlyDataSeeder.TestMembershipBId);
        var crossOrgDocRes = await _client.SendAsync(crossOrgDocReq);
        Assert.Equal(HttpStatusCode.NotFound, crossOrgDocRes.StatusCode);

        // 6. Estimate without quotation -> 404
        var nonQuotedEstimateId = Guid.NewGuid();
        var unquotedDocReq = CreateAuthenticatedRequest(
            HttpMethod.Get, $"/api/v1/estimates/{nonQuotedEstimateId}/quotation/document", "token-org-a", MembershipAId);
        var unquotedDocRes = await _client.SendAsync(unquotedDocReq);
        Assert.Equal(HttpStatusCode.NotFound, unquotedDocRes.StatusCode);

        // 7. No permission -> 403
        var noPermDocReq = CreateAuthenticatedRequest(
            HttpMethod.Get, $"/api/v1/estimates/{estimate.Id}/quotation/document",
            "token-estimate-reviewer", TestOnlyDataSeeder.TestEstimateReviewerMembershipId);
        var noPermDocRes = await _client.SendAsync(noPermDocReq);
        Assert.Equal(HttpStatusCode.Forbidden, noPermDocRes.StatusCode);

        var crossOrganizationRevisionRequest = CreateAuthenticatedRequest(
            HttpMethod.Post, $"/api/v1/estimates/{estimate.Id}/revisions", "token-org-b", TestOnlyDataSeeder.TestMembershipBId);
        crossOrganizationRevisionRequest.Headers.Add("If-Match", $"\"{quotation.EstimateRowVersion}\"");
        crossOrganizationRevisionRequest.Headers.Add("Idempotency-Key", $"cross-org-revision-{Guid.NewGuid():N}");
        crossOrganizationRevisionRequest.Content = JsonContent.Create(new CreateEstimateRevisionRequest("TEST_ONLY cross-org attempt"));
        var crossOrganizationRevisionResponse = await _client.SendAsync(crossOrganizationRevisionRequest);
        Assert.Equal(HttpStatusCode.NotFound, crossOrganizationRevisionResponse.StatusCode);

        var createRevisionRequest = CreateAuthenticatedRequest(
            HttpMethod.Post, $"/api/v1/estimates/{estimate.Id}/revisions", "token-org-a", MembershipAId);
        createRevisionRequest.Headers.Add("If-Match", $"\"{quotation.EstimateRowVersion}\"");
        createRevisionRequest.Headers.Add("Idempotency-Key", "idemp-approved-estimate-revision-0001");
        createRevisionRequest.Content = JsonContent.Create(new CreateEstimateRevisionRequest("TEST_ONLY customer change"));
        var createRevisionResponse = await _client.SendAsync(createRevisionRequest);
        Assert.Equal(HttpStatusCode.Created, createRevisionResponse.StatusCode);
        var revisedEstimate = (await createRevisionResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        Assert.Equal(2, revisedEstimate.CurrentRevision!.RevisionNo);
        Assert.Equal(EstimateRevisionStatus.Draft, revisedEstimate.CurrentRevision.Status);
        Assert.True(revisedEstimate.CurrentRevision.CalculationOutdated);
        Assert.Null(revisedEstimate.CurrentRevision.CalculationSnapshotJson);
        Assert.Equal("WI-01", Assert.Single(Assert.Single(revisedEstimate.CurrentRevision.Sections).WorkItems).Code);
        Assert.NotEqual(revision.Id, revisedEstimate.CurrentRevision.Id);

        var revisionReplayRequest = CreateAuthenticatedRequest(
            HttpMethod.Post, $"/api/v1/estimates/{estimate.Id}/revisions", "token-org-a", MembershipAId);
        revisionReplayRequest.Headers.Add("If-Match", $"\"{quotation.EstimateRowVersion}\"");
        revisionReplayRequest.Headers.Add("Idempotency-Key", "idemp-approved-estimate-revision-0001");
        revisionReplayRequest.Content = JsonContent.Create(new CreateEstimateRevisionRequest("TEST_ONLY customer change"));
        var revisionReplayResponse = await _client.SendAsync(revisionReplayRequest);
        Assert.Equal(HttpStatusCode.Created, revisionReplayResponse.StatusCode);
        var replayedRevision = (await revisionReplayResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        Assert.Equal(revisedEstimate.CurrentRevision.Id, replayedRevision.CurrentRevision!.Id);
    }

    [Theory]
    [InlineData("missing")]
    [InlineData("expired")]
    [InlineData("ambiguous")]
    public async Task CalculateEstimate_WithoutSingleEffectivePolicy_FailsClosed(string scenario)
    {
        var (_, _, opportunityId, _, surveyRevisionId, _) = await SetupEstimatingOpportunityAsync();
        var createRequest = CreateAuthenticatedRequest(HttpMethod.Post, "/api/v1/estimates", "token-org-a", MembershipAId);
        createRequest.Headers.Add("Idempotency-Key", "idemp-estimate-policy-missing-create");
        createRequest.Content = JsonContent.Create(new CreateEstimateDraftRequest(opportunityId, surveyRevisionId, "THB"));
        var createResponse = await _client.SendAsync(createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var estimate = (await createResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        var revision = estimate.CurrentRevision!;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.CalculationPolicyVersions.RemoveRange(db.CalculationPolicyVersions.Where(policy => policy.OrganizationId == OrgAId));
            db.TaxPolicyVersions.RemoveRange(db.TaxPolicyVersions.Where(policy => policy.OrganizationId == OrgAId));
            await db.SaveChangesAsync();

            if (scenario is "expired" or "ambiguous")
            {
                var start = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
                var calculationPolicies = scenario == "expired"
                    ? new[] { new CalculationPolicyVersion(Guid.NewGuid(), OrgAId, BranchAId, "TEST_ONLY_CALC_EXPIRED", 1,
                        "none", 0m, "away-from-zero", start, start.AddYears(1), UserAId) }
                    : new[]
                    {
                        new CalculationPolicyVersion(Guid.NewGuid(), OrgAId, BranchAId, "TEST_ONLY_CALC_OVERLAP", 1,
                            "none", 0m, "away-from-zero", start, null, UserAId),
                        new CalculationPolicyVersion(Guid.NewGuid(), OrgAId, BranchAId, "TEST_ONLY_CALC_OVERLAP", 2,
                            "none", 0m, "away-from-zero", start, null, UserAId)
                    };
                foreach (var policy in calculationPolicies)
                {
                    policy.Publish(TestOnlyDataSeeder.TestUserIdB, start);
                    db.CalculationPolicyVersions.Add(policy);
                }

                var taxPolicy = new TaxPolicyVersion(Guid.NewGuid(), OrgAId, BranchAId,
                    scenario == "expired" ? "TEST_ONLY_TAX_EXPIRED" : "TEST_ONLY_TAX_VALID", 1,
                    "exclusive", 0.07m, "TEST_ONLY_VAT", start,
                    scenario == "expired" ? start.AddYears(1) : null, UserAId);
                taxPolicy.Publish(TestOnlyDataSeeder.TestUserIdB, start);
                db.TaxPolicyVersions.Add(taxPolicy);
                await db.SaveChangesAsync();
            }
        }

        var calculateRequest = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/calculate",
            "token-org-a",
            MembershipAId);
        calculateRequest.Headers.Add("Idempotency-Key", "idemp-estimate-policy-missing-calculate");
        calculateRequest.Content = JsonContent.Create(new CalculateEstimateRequest(revision.RowVersion, 0m));
        var calculateResponse = await _client.SendAsync(calculateRequest);

        Assert.Equal(HttpStatusCode.Conflict, calculateResponse.StatusCode);
        using var problemJson = System.Text.Json.JsonDocument.Parse(await calculateResponse.Content.ReadAsStringAsync());
        Assert.Equal("ESTIMATE_POLICY_UNAVAILABLE", problemJson.RootElement.GetProperty("code").GetString());
        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(0, await verifyDb.EstimateCalculationSnapshots.CountAsync(snapshot => snapshot.EstimateRevisionId == revision.Id));
    }

    [Fact]
    public async Task CalculationPolicy_BranchMustBelongToPolicyOrganization()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var start = DateTimeOffset.UtcNow.AddDays(-1);
        var policy = new CalculationPolicyVersion(Guid.NewGuid(), OrgAId, TestOnlyDataSeeder.TestBranchBId,
            "TEST_ONLY_CROSS_ORG_BRANCH", 1, "none", 0m, "away-from-zero", start, null, UserAId);
        policy.Publish(TestOnlyDataSeeder.TestUserIdB, start);
        db.CalculationPolicyVersions.Add(policy);

        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();
        Assert.False(await db.CalculationPolicyVersions.AnyAsync(row => row.Id == policy.Id));
    }

    [Fact]
    public async Task CalculateEstimate_PercentDiscountPersistsTypedInputAndRequiresReason()
    {
        var (estimate, _, revision) = await SetupCalculatedEstimateAsync($"percent-discount-{Guid.NewGuid():N}");
        var missingReasonRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/calculate", "token-org-a", MembershipAId);
        missingReasonRequest.Headers.Add("Idempotency-Key", $"percent-discount-missing-reason-{Guid.NewGuid():N}");
        missingReasonRequest.Content = JsonContent.Create(new CalculateEstimateRequest(
            revision.RowVersion,
            DiscountType: EstimateDiscount.Percent,
            DiscountValue: 0.10m));
        var missingReasonResponse = await _client.SendAsync(missingReasonRequest);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, missingReasonResponse.StatusCode);
        using (var problem = System.Text.Json.JsonDocument.Parse(await missingReasonResponse.Content.ReadAsStringAsync()))
            Assert.Equal("ESTIMATE_DISCOUNT_REASON_REQUIRED", problem.RootElement.GetProperty("code").GetString());

        var calculateRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/calculate", "token-org-a", MembershipAId);
        var calculateIdempotencyKey = $"percent-discount-valid-{Guid.NewGuid():N}";
        calculateRequest.Headers.Add("Idempotency-Key", calculateIdempotencyKey);
        calculateRequest.Content = JsonContent.Create(new
        {
            expectedRevisionVersion = revision.RowVersion,
            discountType = EstimateDiscount.Percent,
            discountValue = 0.10m,
            discountReasonCode = "TEST_ONLY_DISCOUNT",
            netCost = 0m,
            grandTotal = 0m
        });
        var calculateResponse = await _client.SendAsync(calculateRequest);
        Assert.Equal(HttpStatusCode.OK, calculateResponse.StatusCode);
        var calculatedRevision = (await calculateResponse.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;
        Assert.Equal(EstimateDiscount.Percent, calculatedRevision.DiscountType);
        Assert.Equal(0.10m, calculatedRevision.DiscountValue);
        Assert.Equal("TEST_ONLY_DISCOUNT", calculatedRevision.DiscountReasonCode);
        Assert.Equal(decimal.Round(calculatedRevision.SellingBeforeDiscount * 0.10m, 2, MidpointRounding.AwayFromZero), calculatedRevision.DiscountAmount);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var persisted = await db.EstimateRevisions.SingleAsync(row => row.Id == revision.Id);
        Assert.Equal(EstimateDiscount.Percent, persisted.DiscountType);
        Assert.Equal(0.10m, persisted.DiscountValue);
        Assert.Equal("TEST_ONLY_DISCOUNT", persisted.DiscountReasonCode);

        var replayRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/calculate", "token-org-a", MembershipAId);
        replayRequest.Headers.Add("Idempotency-Key", calculateIdempotencyKey);
        replayRequest.Content = JsonContent.Create(new CalculateEstimateRequest(
            revision.RowVersion,
            DiscountType: EstimateDiscount.Percent,
            DiscountValue: 0.10m,
            DiscountReasonCode: "TEST_ONLY_DISCOUNT"));
        var replayResponse = await _client.SendAsync(replayRequest);
        Assert.Equal(HttpStatusCode.OK, replayResponse.StatusCode);
        var replayedRevision = (await replayResponse.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;
        Assert.Equal(calculatedRevision.CalculationVersion, replayedRevision.CalculationVersion);
        Assert.Equal(calculatedRevision.RowVersion, replayedRevision.RowVersion);
        Assert.Equal(calculatedRevision.GrandTotal, replayedRevision.GrandTotal);

        var reusedKeyRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/calculate", "token-org-a", MembershipAId);
        reusedKeyRequest.Headers.Add("Idempotency-Key", calculateIdempotencyKey);
        reusedKeyRequest.Content = JsonContent.Create(new CalculateEstimateRequest(
            revision.RowVersion,
            DiscountType: EstimateDiscount.Percent,
            DiscountValue: 0.05m,
            DiscountReasonCode: "TEST_ONLY_DISCOUNT"));
        var reusedKeyResponse = await _client.SendAsync(reusedKeyRequest);
        Assert.Equal(HttpStatusCode.Conflict, reusedKeyResponse.StatusCode);
        using (var problem = System.Text.Json.JsonDocument.Parse(await reusedKeyResponse.Content.ReadAsStringAsync()))
            Assert.Equal("IDEMPOTENCY_KEY_REUSED", problem.RootElement.GetProperty("code").GetString());

        using var snapshotScope = _factory.Services.CreateScope();
        var snapshotDb = snapshotScope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(calculatedRevision.CalculationVersion,
            await snapshotDb.EstimateCalculationSnapshots.CountAsync(snapshot => snapshot.EstimateRevisionId == revision.Id));
        Assert.Equal(1, await snapshotDb.IdempotencyRecords.CountAsync(record => record.OrganizationId == OrgAId &&
            record.Operation == "estimates.calculate" &&
            record.ResourceId == $"{estimate.Id:N}|{revision.Id:N}|{calculatedRevision.CalculationVersion}"));
    }

    [Fact]
    public async Task SubmitEstimate_WithMissingBoq_CalculatesBlockedReadinessAndRejectsSubmit()
    {
        var (_, _, opportunityId, _, surveyRevisionId, _) = await SetupEstimatingOpportunityAsync();
        var createRequest = CreateAuthenticatedRequest(HttpMethod.Post, "/api/v1/estimates", "token-org-a", MembershipAId);
        createRequest.Headers.Add("Idempotency-Key", $"readiness-create-{Guid.NewGuid():N}");
        createRequest.Content = JsonContent.Create(new CreateEstimateDraftRequest(opportunityId, surveyRevisionId, "THB"));
        var createResponse = await _client.SendAsync(createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var estimate = (await createResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        var revision = estimate.CurrentRevision!;

        var calculateRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/calculate", "token-org-a", MembershipAId);
        calculateRequest.Headers.Add("Idempotency-Key", $"readiness-calculate-{Guid.NewGuid():N}");
        calculateRequest.Content = JsonContent.Create(new CalculateEstimateRequest(revision.RowVersion, 0m));
        var calculateResponse = await _client.SendAsync(calculateRequest);
        Assert.Equal(HttpStatusCode.OK, calculateResponse.StatusCode);
        var calculatedRevision = (await calculateResponse.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;
        Assert.Equal("blocked", calculatedRevision.Readiness);
        Assert.Contains(calculatedRevision.ReadinessReasons, reason => reason.Code == "ESTIMATE_FIELD_REQUIRED");

        var submitRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/submit", "token-org-a", MembershipAId);
        submitRequest.Headers.Add("If-Match", $"\"{estimate.RowVersion}\"");
        submitRequest.Headers.Add("Idempotency-Key", $"readiness-submit-{Guid.NewGuid():N}");
        submitRequest.Content = JsonContent.Create(new SubmitEstimateRequest(1, calculatedRevision.CalculationVersion));
        var submitResponse = await _client.SendAsync(submitRequest);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, submitResponse.StatusCode);
        using var problem = System.Text.Json.JsonDocument.Parse(await submitResponse.Content.ReadAsStringAsync());
        Assert.Equal("ESTIMATE_FIELD_REQUIRED", problem.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task SubmitEstimate_WithoutIndependentChecker_FailsWithoutStateChange()
    {
        var (estimate, _, _) = await SetupCalculatedEstimateAsync($"no-checker-{Guid.NewGuid():N}");
        var revision = estimate.CurrentRevision!;

        using (var permissionScope = _factory.Services.CreateScope())
        {
            var db = permissionScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var approvalPermission = await db.Permissions.SingleAsync(permission => permission.Key == "estimates.approve");
            var checkerRole = await db.Roles.SingleAsync(role => role.OrganizationId == OrgAId && role.Name == "Test Cost Reviewer");
            var assignment = await db.RolePermissions.SingleAsync(row => row.RoleId == checkerRole.Id && row.PermissionId == approvalPermission.Id);
            db.RolePermissions.Remove(assignment);
            await db.SaveChangesAsync();
        }

        var submitRequest = CreateAuthenticatedRequest(HttpMethod.Post, $"/api/v1/estimates/{estimate.Id}/submit", "token-org-a", MembershipAId);
        submitRequest.Headers.Add("If-Match", $"\"{estimate.RowVersion}\"");
        submitRequest.Headers.Add("Idempotency-Key", "idemp-estimate-no-checker-submit");
        submitRequest.Content = JsonContent.Create(new SubmitEstimateRequest(revision.RevisionNo, revision.CalculationVersion));
        var submitResponse = await _client.SendAsync(submitRequest);
        Assert.Equal(HttpStatusCode.Conflict, submitResponse.StatusCode);
        using var problem = System.Text.Json.JsonDocument.Parse(await submitResponse.Content.ReadAsStringAsync());
        Assert.Equal("ESTIMATE_POLICY_UNAVAILABLE", problem.RootElement.GetProperty("code").GetString());

        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(EstimateRevisionStatus.Draft, await verifyDb.EstimateRevisions.Where(row => row.Id == revision.Id).Select(row => row.Status).SingleAsync());
        Assert.Equal(0, await verifyDb.EstimateApprovalRequests.CountAsync(row => row.EstimateRevisionId == revision.Id));
    }

    [Fact]
    public async Task ReviewEstimate_RevokedApprovalPermissionIsRecheckedAtDecisionTime()
    {
        var (estimate, _, _) = await SetupCalculatedEstimateAsync($"revoked-reviewer-{Guid.NewGuid():N}");
        var submitRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/submit", "token-org-a", MembershipAId);
        submitRequest.Headers.Add("If-Match", $"\"{estimate.RowVersion}\"");
        submitRequest.Headers.Add("Idempotency-Key", $"revoked-submit-{Guid.NewGuid():N}");
        submitRequest.Content = JsonContent.Create(new SubmitEstimateRequest(1, 1, "TEST_ONLY reviewer revoke"));
        var submitResponse = await _client.SendAsync(submitRequest);
        Assert.Equal(HttpStatusCode.OK, submitResponse.StatusCode);
        var submitted = (await submitResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;

        using (var permissionScope = _factory.Services.CreateScope())
        {
            var db = permissionScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var approvalPermission = await db.Permissions.SingleAsync(permission => permission.Key == "estimates.approve");
            var reviewerRoleId = await db.MembershipRoles
                .Where(row => row.OrganizationId == OrgAId && row.MembershipId == TestOnlyDataSeeder.TestCostReviewerMembershipId)
                .Select(row => row.RoleId)
                .SingleAsync();
            var permissionAssignment = await db.RolePermissions.SingleAsync(row =>
                row.OrganizationId == OrgAId && row.RoleId == reviewerRoleId && row.PermissionId == approvalPermission.Id);
            db.RolePermissions.Remove(permissionAssignment);
            await db.SaveChangesAsync();
        }

        var reviewRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/review-decisions", "token-org-b", TestOnlyDataSeeder.TestCostReviewerMembershipId);
        reviewRequest.Headers.Add("If-Match", $"\"{submitted.RowVersion}\"");
        reviewRequest.Headers.Add("Idempotency-Key", $"revoked-review-{Guid.NewGuid():N}");
        reviewRequest.Content = JsonContent.Create(new ReviewEstimateRequest(1, "approved"));
        var response = await _client.SendAsync(reviewRequest);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var revisionId = estimate.CurrentRevision!.Id;
        Assert.Equal(EstimateRevisionStatus.Submitted,
            await verifyDb.EstimateRevisions.Where(row => row.Id == revisionId).Select(row => row.Status).SingleAsync());
        var requestId = await verifyDb.EstimateApprovalRequests
            .Where(request => request.EstimateRevisionId == revisionId)
            .Select(request => request.Id).SingleAsync();
        Assert.Equal(0, await verifyDb.EstimateApprovalDecisions.CountAsync(row => row.EstimateApprovalRequestId == requestId));
    }

    [Theory]
    [InlineData("approved", "returned")]
    [InlineData("returned", "approved")]
    public async Task ReviewEstimate_ConcurrentDecisionsWithSameVersionOnlyOneSucceeds(
        string firstDecision, string secondDecision)
    {
        var (estimate, _, _) = await SetupCalculatedEstimateAsync($"concurrent-review-{Guid.NewGuid():N}");
        var submitRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/submit", "token-org-a", MembershipAId);
        submitRequest.Headers.Add("If-Match", $"\"{estimate.RowVersion}\"");
        submitRequest.Headers.Add("Idempotency-Key", $"concurrent-review-submit-{Guid.NewGuid():N}");
        submitRequest.Content = JsonContent.Create(new SubmitEstimateRequest(1, 1, "TEST_ONLY concurrent review"));
        var submitResponse = await _client.SendAsync(submitRequest);
        Assert.Equal(HttpStatusCode.OK, submitResponse.StatusCode);
        var submitted = (await submitResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;

        HttpRequestMessage BuildReviewRequest(string decision, string key)
        {
            var request = CreateAuthenticatedRequest(HttpMethod.Post,
                $"/api/v1/estimates/{estimate.Id}/review-decisions", "token-org-b", TestOnlyDataSeeder.TestCostReviewerMembershipId);
            request.Headers.Add("If-Match", $"\"{submitted.RowVersion}\"");
            request.Headers.Add("Idempotency-Key", key);
            request.Content = JsonContent.Create(new ReviewEstimateRequest(1, decision,
                decision == "returned" ? "TEST_ONLY_RETURN" : null,
                decision == "returned" ? "TEST_ONLY concurrent return" : null));
            return request;
        }

        var responses = await Task.WhenAll(
            _client.SendAsync(BuildReviewRequest(firstDecision, $"concurrent-first-{Guid.NewGuid():N}")),
            _client.SendAsync(BuildReviewRequest(secondDecision, $"concurrent-second-{Guid.NewGuid():N}")));

        var responseDetails = await Task.WhenAll(responses.Select(async response =>
            $"{(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}"));
        Assert.True(responses.Count(response => response.StatusCode == HttpStatusCode.OK) == 1, string.Join(Environment.NewLine, responseDetails));
        Assert.True(responses.Count(response => response.StatusCode == HttpStatusCode.Conflict) == 1, string.Join(Environment.NewLine, responseDetails));
        using var conflict = System.Text.Json.JsonDocument.Parse(
            await responses.Single(response => response.StatusCode == HttpStatusCode.Conflict).Content.ReadAsStringAsync());
        Assert.Equal("ESTIMATE_VERSION_CONFLICT", conflict.RootElement.GetProperty("code").GetString());
        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var requestId = await verifyDb.EstimateApprovalRequests
            .Where(request => request.EstimateRevisionId == estimate.CurrentRevision!.Id)
            .Select(request => request.Id).SingleAsync();
        Assert.Equal(1, await verifyDb.EstimateApprovalDecisions.CountAsync(row => row.EstimateApprovalRequestId == requestId));
    }

    [Fact]
    public async Task CreateEstimateRevision_ConcurrentRequestsWithSameVersionOnlyOneSucceeds()
    {
        var (estimate, opportunity, revision) = await SetupCalculatedEstimateAsync($"concurrent-revision-{Guid.NewGuid():N}");

        var submitRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/submit", "token-org-a", MembershipAId);
        submitRequest.Headers.Add("If-Match", $"\"{estimate.RowVersion}\"");
        submitRequest.Headers.Add("Idempotency-Key", $"concurrent-revision-submit-{Guid.NewGuid():N}");
        submitRequest.Content = JsonContent.Create(new SubmitEstimateRequest(1, revision.CalculationVersion, "TEST_ONLY revision concurrency"));
        var submitResponse = await _client.SendAsync(submitRequest);
        Assert.Equal(HttpStatusCode.OK, submitResponse.StatusCode);
        var submitted = (await submitResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;

        var reviewRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/review-decisions", "token-org-b", TestOnlyDataSeeder.TestCostReviewerMembershipId);
        reviewRequest.Headers.Add("If-Match", $"\"{submitted.RowVersion}\"");
        reviewRequest.Headers.Add("Idempotency-Key", $"concurrent-revision-review-{Guid.NewGuid():N}");
        reviewRequest.Content = JsonContent.Create(new ReviewEstimateRequest(1, "approved"));
        var reviewResponse = await _client.SendAsync(reviewRequest);
        Assert.Equal(HttpStatusCode.OK, reviewResponse.StatusCode);
        var approved = (await reviewResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;

        var quotationRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/quotation", "token-org-a", MembershipAId);
        quotationRequest.Headers.Add("Idempotency-Key", $"concurrent-revision-quote-{Guid.NewGuid():N}");
        quotationRequest.Content = JsonContent.Create(new IssueQuotationRequest(approved.RowVersion, opportunity.RowVersion));
        var quotationResponse = await _client.SendAsync(quotationRequest);
        Assert.Equal(HttpStatusCode.Created, quotationResponse.StatusCode);
        var quotation = (await quotationResponse.Content.ReadFromJsonAsync<QuotationResponse>())!;

        HttpRequestMessage BuildRevisionRequest(string key)
        {
            var request = CreateAuthenticatedRequest(HttpMethod.Post,
                $"/api/v1/estimates/{estimate.Id}/revisions", "token-org-a", MembershipAId);
            request.Headers.Add("If-Match", $"\"{quotation.EstimateRowVersion}\"");
            request.Headers.Add("Idempotency-Key", key);
            request.Content = JsonContent.Create(new CreateEstimateRevisionRequest("TEST_ONLY concurrent revision"));
            return request;
        }

        var responses = await Task.WhenAll(
            _client.SendAsync(BuildRevisionRequest($"revision-race-a-{Guid.NewGuid():N}")),
            _client.SendAsync(BuildRevisionRequest($"revision-race-b-{Guid.NewGuid():N}")));

        var responseDetails = await Task.WhenAll(responses.Select(async response =>
            $"{(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}"));
        Assert.True(responses.Count(response => response.StatusCode == HttpStatusCode.Created) == 1, string.Join(Environment.NewLine, responseDetails));
        Assert.True(responses.Count(response => response.StatusCode == HttpStatusCode.Conflict) == 1, string.Join(Environment.NewLine, responseDetails));
        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(2, await verifyDb.EstimateRevisions.CountAsync(row => row.EstimateId == estimate.Id));
    }

    [Fact]
    public async Task ReviewEstimate_SequentialApprovalRequiresEachAssignedStep()
    {
        var (estimate, _, calculatedRevision) = await SetupCalculatedEstimateAsync($"approval-sequence-{Guid.NewGuid():N}");
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true, seedDedicatedEstimateReviewer: true);

        var submitRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/submit", "token-org-a", MembershipAId);
        submitRequest.Headers.Add("If-Match", $"\"{estimate.RowVersion}\"");
        submitRequest.Headers.Add("Idempotency-Key", $"approval-sequence-submit-{Guid.NewGuid():N}");
        submitRequest.Content = JsonContent.Create(new SubmitEstimateRequest(
            1, calculatedRevision.CalculationVersion, "TEST_ONLY sequential approval"));
        var submitResponse = await _client.SendAsync(submitRequest);
        Assert.Equal(HttpStatusCode.OK, submitResponse.StatusCode);
        var submitted = (await submitResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        Assert.Equal(EstimateRevisionStatus.Submitted, submitted.CurrentRevision!.Status);

        var approvalRequest = await db.EstimateApprovalRequests.SingleAsync(row => row.EstimateId == estimate.Id);
        var firstStep = await db.EstimateApprovalSteps.SingleAsync(row => row.EstimateApprovalRequestId == approvalRequest.Id);
        Assert.Equal(TestOnlyDataSeeder.TestEstimateReviewerMembershipId, firstStep.ReviewerMembershipId);
        var secondStep = new EstimateApprovalStep(Guid.NewGuid(), OrgAId, approvalRequest.Id, 2,
            TestOnlyDataSeeder.TestUserIdB, TestOnlyDataSeeder.TestCostReviewerMembershipId, "organization", OrgAId);
        db.EstimateApprovalSteps.Add(secondStep);

        var routeSnapshot = System.Text.Json.JsonSerializer.Serialize(new
        {
            policyCode = "TEST_ONLY-TH-EST-V1",
            policyVersion = 1,
            permissionKey = "estimates.approve",
            triggers = new[] { "AMOUNT_AUTHORITY_EXCEEDED", "MARGIN_BELOW_MINIMUM" },
            steps = new[]
            {
                new { sequence = 1, reviewerMembershipId = firstStep.ReviewerMembershipId },
                new { sequence = 2, reviewerMembershipId = secondStep.ReviewerMembershipId }
            },
            thresholds = new { amountLimit = 100000m, minimumMarginRate = 0.30m }
        });
        var routeHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(routeSnapshot))).ToLowerInvariant();
        await db.EstimateApprovalRequests
            .Where(row => row.Id == approvalRequest.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(row => row.PolicyCode, "TEST_ONLY-TH-EST-V1")
                .SetProperty(row => row.RouteSnapshotJson, routeSnapshot)
                .SetProperty(row => row.RouteHash, routeHash));
        await db.SaveChangesAsync();

        var secondReviewerQueue = CreateAuthenticatedRequest(HttpMethod.Get,
            "/api/v1/estimates/review-queue?pageNumber=1&pageSize=10",
            "token-org-b", TestOnlyDataSeeder.TestCostReviewerMembershipId);
        var beforeFirstDecision = (await (await _client.SendAsync(secondReviewerQueue))
            .Content.ReadFromJsonAsync<EstimateReviewQueueResponse>())!;
        Assert.Empty(beforeFirstDecision.Items);
        var earlySecondReview = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/review-decisions", "token-org-b",
            TestOnlyDataSeeder.TestCostReviewerMembershipId);
        earlySecondReview.Headers.Add("If-Match", $"\"{submitted.RowVersion}\"");
        earlySecondReview.Headers.Add("Idempotency-Key", $"approval-sequence-early-{Guid.NewGuid():N}");
        earlySecondReview.Content = JsonContent.Create(new ReviewEstimateRequest(1, "approved"));
        Assert.Equal(HttpStatusCode.Forbidden, (await _client.SendAsync(earlySecondReview)).StatusCode);
        Assert.Empty(await db.EstimateApprovalDecisions.Where(row => row.EstimateApprovalRequestId == approvalRequest.Id).ToListAsync());

        var firstReview = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/review-decisions", "token-estimate-reviewer",
            TestOnlyDataSeeder.TestEstimateReviewerMembershipId);
        firstReview.Headers.Add("If-Match", $"\"{submitted.RowVersion}\"");
        firstReview.Headers.Add("Idempotency-Key", $"approval-sequence-first-{Guid.NewGuid():N}");
        firstReview.Content = JsonContent.Create(new ReviewEstimateRequest(1, "approved"));
        var firstReviewResponse = await _client.SendAsync(firstReview);
        Assert.Equal(HttpStatusCode.OK, firstReviewResponse.StatusCode);
        var afterFirstApproval = (await firstReviewResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        Assert.Equal(EstimateRevisionStatus.Submitted, afterFirstApproval.CurrentRevision!.Status);
        Assert.Equal(EstimateApprovalRequestStatus.Open,
            await db.EstimateApprovalRequests.Where(row => row.Id == approvalRequest.Id).Select(row => row.Status).SingleAsync());
        Assert.Empty(await db.EstimateApprovalSnapshots.Where(row => row.EstimateApprovalRequestId == approvalRequest.Id).ToListAsync());

        var nextReviewerQueue = CreateAuthenticatedRequest(HttpMethod.Get,
            "/api/v1/estimates/review-queue?pageNumber=1&pageSize=10",
            "token-org-b", TestOnlyDataSeeder.TestCostReviewerMembershipId);
        var afterFirstDecisionQueue = (await (await _client.SendAsync(nextReviewerQueue))
            .Content.ReadFromJsonAsync<EstimateReviewQueueResponse>())!;
        Assert.Equal(estimate.Id, Assert.Single(afterFirstDecisionQueue.Items).EstimateId);
        var firstReviewerQueue = CreateAuthenticatedRequest(HttpMethod.Get,
            "/api/v1/estimates/review-queue?pageNumber=1&pageSize=10",
            "token-estimate-reviewer", TestOnlyDataSeeder.TestEstimateReviewerMembershipId);
        Assert.Empty((await (await _client.SendAsync(firstReviewerQueue))
            .Content.ReadFromJsonAsync<EstimateReviewQueueResponse>())!.Items);

        var secondReview = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/review-decisions", "token-org-b",
            TestOnlyDataSeeder.TestCostReviewerMembershipId);
        secondReview.Headers.Add("If-Match", $"\"{afterFirstApproval.RowVersion}\"");
        secondReview.Headers.Add("Idempotency-Key", $"approval-sequence-second-{Guid.NewGuid():N}");
        secondReview.Content = JsonContent.Create(new ReviewEstimateRequest(1, "approved"));
        var secondReviewResponse = await _client.SendAsync(secondReview);
        Assert.Equal(HttpStatusCode.OK, secondReviewResponse.StatusCode);
        var approved = (await secondReviewResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        Assert.Equal(EstimateRevisionStatus.Approved, approved.CurrentRevision!.Status);
        var finalSnapshot = await db.EstimateApprovalSnapshots.SingleAsync(row => row.EstimateApprovalRequestId == approvalRequest.Id);
        using var snapshotJson = System.Text.Json.JsonDocument.Parse(finalSnapshot.SnapshotJson);
        Assert.Equal(2, snapshotJson.RootElement.GetProperty("decisions").GetArrayLength());
    }

    [Fact]
    public async Task CancelSubmittedEstimate_RequiresAssignedCheckerAndClosesApprovalRoute()
    {
        var (estimate, _, _) = await SetupCalculatedEstimateAsync($"cancel-submitted-{Guid.NewGuid():N}");
        var estimateRevisionId = estimate.CurrentRevision!.Id;
        var crossOrganizationCancel = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/cancel", "token-org-b", TestOnlyDataSeeder.TestMembershipBId);
        crossOrganizationCancel.Headers.Add("If-Match", $"\"{estimate.RowVersion}\"");
        crossOrganizationCancel.Headers.Add("Idempotency-Key", $"cancel-cross-org-{Guid.NewGuid():N}");
        crossOrganizationCancel.Content = JsonContent.Create(new CancelEstimateRequest("TEST_ONLY cross-org attempt"));
        var crossOrganizationResponse = await _client.SendAsync(crossOrganizationCancel);
        Assert.Equal(HttpStatusCode.NotFound, crossOrganizationResponse.StatusCode);

        var submitRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/submit", "token-org-a", MembershipAId);
        submitRequest.Headers.Add("If-Match", $"\"{estimate.RowVersion}\"");
        submitRequest.Headers.Add("Idempotency-Key", $"cancel-submitted-submit-{Guid.NewGuid():N}");
        submitRequest.Content = JsonContent.Create(new SubmitEstimateRequest(1, 1, "TEST_ONLY cancel route"));
        var submitResponse = await _client.SendAsync(submitRequest);
        Assert.Equal(HttpStatusCode.OK, submitResponse.StatusCode);
        var submittedEstimate = (await submitResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;

        var makerCancelRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/cancel", "token-org-a", MembershipAId);
        makerCancelRequest.Headers.Add("If-Match", $"\"{submittedEstimate.RowVersion}\"");
        makerCancelRequest.Headers.Add("Idempotency-Key", $"cancel-submitted-maker-{Guid.NewGuid():N}");
        makerCancelRequest.Content = JsonContent.Create(new CancelEstimateRequest("TEST_ONLY maker attempt"));
        var makerCancelResponse = await _client.SendAsync(makerCancelRequest);
        Assert.Equal(HttpStatusCode.Forbidden, makerCancelResponse.StatusCode);

        var checkerCancelRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/cancel", "token-org-b", TestOnlyDataSeeder.TestCostReviewerMembershipId);
        checkerCancelRequest.Headers.Add("If-Match", $"\"{submittedEstimate.RowVersion}\"");
        var checkerCancelKey = $"cancel-submitted-checker-{Guid.NewGuid():N}";
        checkerCancelRequest.Headers.Add("Idempotency-Key", checkerCancelKey);
        checkerCancelRequest.Content = JsonContent.Create(new CancelEstimateRequest("TEST_ONLY customer withdrew"));
        var checkerCancelResponse = await _client.SendAsync(checkerCancelRequest);
        Assert.Equal(HttpStatusCode.OK, checkerCancelResponse.StatusCode);
        var cancelledEstimate = (await checkerCancelResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        Assert.Equal(EstimateRevisionStatus.Cancelled, cancelledEstimate.CurrentRevision!.Status);
        Assert.Equal(EstimateStatus.Cancelled, cancelledEstimate.Status);

        var replayRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/cancel", "token-org-b", TestOnlyDataSeeder.TestCostReviewerMembershipId);
        replayRequest.Headers.Add("If-Match", $"\"{submittedEstimate.RowVersion}\"");
        replayRequest.Headers.Add("Idempotency-Key", checkerCancelKey);
        replayRequest.Content = JsonContent.Create(new CancelEstimateRequest("TEST_ONLY customer withdrew"));
        var replayResponse = await _client.SendAsync(replayRequest);
        Assert.Equal(HttpStatusCode.OK, replayResponse.StatusCode);
        var replayedEstimate = (await replayResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        Assert.Equal(cancelledEstimate.RowVersion, replayedEstimate.RowVersion);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var approvalRequest = await db.EstimateApprovalRequests.SingleAsync(row => row.EstimateRevisionId == estimateRevisionId);
        Assert.Equal(EstimateApprovalRequestStatus.Cancelled, approvalRequest.Status);
        Assert.NotNull(approvalRequest.ClosedAtUtc);
        Assert.Equal(0, await db.EstimateApprovalRequests.CountAsync(row => row.EstimateRevisionId == estimateRevisionId && row.Status == EstimateApprovalRequestStatus.Open));
    }

    [Fact]
    public async Task CancelEstimate_ConcurrentRequestsWithSameVersion_OnlyOneSucceeds()
    {
        var (estimate, _, _) = await SetupCalculatedEstimateAsync($"cancel-concurrent-{Guid.NewGuid():N}");

        HttpRequestMessage BuildRequest(string idempotencyKey)
        {
            var request = CreateAuthenticatedRequest(HttpMethod.Post,
                $"/api/v1/estimates/{estimate.Id}/cancel", "token-org-a", MembershipAId);
            request.Headers.Add("If-Match", $"\"{estimate.RowVersion}\"");
            request.Headers.Add("Idempotency-Key", idempotencyKey);
            request.Content = JsonContent.Create(new CancelEstimateRequest("TEST_ONLY concurrent cancellation"));
            return request;
        }

        var responses = await Task.WhenAll(
            _client.SendAsync(BuildRequest($"cancel-concurrent-a-{Guid.NewGuid():N}")),
            _client.SendAsync(BuildRequest($"cancel-concurrent-b-{Guid.NewGuid():N}")));

        Assert.Single(responses, response => response.StatusCode == HttpStatusCode.OK);
        Assert.Single(responses, response => response.StatusCode == HttpStatusCode.Conflict);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(EstimateStatus.Cancelled, await db.Estimates.Where(row => row.Id == estimate.Id).Select(row => row.Status).SingleAsync());
        Assert.Equal(1, await db.AuditEvents.CountAsync(row => row.ResourceId == estimate.Id.ToString() && row.Action == "estimates.cancelled"));
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
    public async Task UpdateDraft_WithCatalogItem_ResolvesAndSnapshotsCostAuthoritatively()
    {
        var (_, _, oppId, _, surveyRevId, _) = await SetupEstimatingOpportunityAsync();

        // 1. Seed active item and published cost
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTimeOffset.UtcNow;
        var actorId = UserAId;

        var cat = new ItemCategory(Guid.NewGuid(), OrgAId, "TEST-CAT", LocalizedText.Create("หมวดหมู่ทดสอบ", "Test Category"), null, null, [ItemType.Material], 1, actorId, now);
        var unit = new UnitOfMeasure(Guid.NewGuid(), OrgAId, "PCS", LocalizedText.Create("ชิ้น", "Piece"), "pcs", "count", 0, "half_up", actorId, now);
        db.ItemCategories.Add(cat);
        db.Units.Add(unit);

        var item = Item.CreateDraft(
            Guid.NewGuid(), OrgAId, "CATALOG-SOLAR-01", ItemType.Material, cat.Id, null,
            LocalizedText.Create("แผงโซลาร์เซลล์", "Solar Panel"), null, unit.Id,
            ItemAvailabilityMode.AllBranches, new ItemCapabilities(true, true, true, true, false),
            null, null, actorId, now);
        item.Activate(actorId, now, false, true, true, true);
        db.Items.Add(item);

        var cost = CostRecord.CreateDraft(
            Guid.NewGuid(), OrgAId, item.Id, CostScopeType.Organization, null, unit.Id, "THB",
            2500m, 0m, null, now.AddDays(-1), null, 1, null, null, null, null, actorId, now);
        var approverId = Guid.NewGuid();
        cost.Submit(actorId, now);
        cost.Approve(approverId, now);
        cost.Publish(approverId, now);
        db.CostRecords.Add(cost);
        await db.SaveChangesAsync();

        // 2. Create estimate draft
        var createMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/v1/estimates",
            "token-org-a",
            MembershipAId);
        createMsg.Headers.Add("Idempotency-Key", "idemp-estimate-catalog-0001");
        createMsg.Content = JsonContent.Create(new CreateEstimateDraftRequest(
            oppId,
            surveyRevId,
            "THB"));

        var createRes = await _client.SendAsync(createMsg);
        Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);
        var estimate = (await createRes.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        var revision = estimate.CurrentRevision!;

        // 3. Update draft with catalog component matching published cost
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
                    Id: null,
                    Code: "SEC-01",
                    NameTh: "หมวดงานโซลาร์",
                    NameEn: "Solar Section",
                    SortOrder: 1,
                    WorkItems: new List<UpdateEstimateWorkItemDto>
                    {
                        new(
                            Id: null,
                            Code: "WI-01",
                            DescriptionTh: "ติดตั้งแผงโซลาร์",
                            DescriptionEn: "Install Solar",
                            Quantity: 4m,
                            UnitCode: "ชุด",
                            SellingRuleType: SellingRuleType.Margin,
                            SellingRuleValue: 0.20m,
                            SortOrder: 1,
                            CostComponents: new List<UpdateEstimateCostComponentDto>
                            {
                                new(
                                    Id: null,
                                    Type: CostComponentType.Material,
                                    Description: "แผงโซลาร์เซลล์",
                                    Quantity: 4m,
                                    UnitCode: "PCS",
                                    UnitCost: 2500m,
                                    Currency: "THB",
                                    SortOrder: 1,
                                    ItemId: item.Id,
                                    CostRecordId: cost.Id,
                                    CostRecordVersion: 1)
                            })
                    })
            }));

        var updateRes = await _client.SendAsync(updateMsg);
        Assert.Equal(HttpStatusCode.OK, updateRes.StatusCode);

        var updatedRev = await updateRes.Content.ReadFromJsonAsync<EstimateRevisionResponse>();
        Assert.NotNull(updatedRev);
        var comp = updatedRev.Sections[0].WorkItems[0].CostComponents[0];
        Assert.Equal(item.Id, comp.ItemId);
        Assert.Equal(cost.Id, comp.CostRecordId);
        Assert.Equal(1, comp.CostRecordVersion);
        Assert.Equal("CATALOG-SOLAR-01", comp.ItemCodeSnapshot);
        Assert.NotNull(comp.ItemNameSnapshot);
        Assert.Equal("แผงโซลาร์เซลล์", comp.ItemNameSnapshot.Thai);
        Assert.Equal("PCS", comp.UnitSnapshot);
        Assert.Equal(2500m, comp.UnitCostSnapshot);
        Assert.Equal(2500m, comp.UnitCost);
        Assert.Equal(10000m, comp.TotalCost);
        Assert.NotNull(comp.ResolvedAtUtc);
    }

    [Theory]
    [InlineData("mismatched", "ITEM_COST_VERSION_CONFLICT")]
    [InlineData("stale", "ITEM_COST_STALE")]
    [InlineData("ambiguous", "ITEM_COST_AMBIGUOUS")]
    [InlineData("unit-mismatch", "ESTIMATE_UNIT_INVALID")]
    public async Task UpdateDraft_WithUnresolvableCatalogCost_ReturnsStructuredConflictWithoutWriting(
        string scenario,
        string expectedCode)
    {
        var (_, _, oppId, _, surveyRevId, _) = await SetupEstimatingOpportunityAsync();

        // 1. Seed active item and published cost
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTimeOffset.UtcNow;
        var actorId = UserAId;

        var cat = new ItemCategory(Guid.NewGuid(), OrgAId, "TEST-CAT2", LocalizedText.Create("หมวดหมู่ทดสอบ2", "Test Category 2"), null, null, [ItemType.Material], 1, actorId, now);
        var unit = new UnitOfMeasure(Guid.NewGuid(), OrgAId, "PCS2", LocalizedText.Create("ชิ้น", "Piece"), "pcs", "count", 0, "half_up", actorId, now);
        db.ItemCategories.Add(cat);
        db.Units.Add(unit);

        var item = Item.CreateDraft(
            Guid.NewGuid(), OrgAId, "CATALOG-SOLAR-02", ItemType.Material, cat.Id, null,
            LocalizedText.Create("แผงโซลาร์เซลล์ 2", "Solar Panel 2"), null, unit.Id,
            ItemAvailabilityMode.AllBranches, new ItemCapabilities(true, true, true, true, false),
            null, null, actorId, now);
        item.Activate(actorId, now, false, true, true, true);
        db.Items.Add(item);

        var effectiveFrom = now.AddDays(scenario == "stale" ? -30 : -1);
        DateTimeOffset? effectiveTo = scenario == "stale" ? now.AddDays(-1) : null;
        var cost = CostRecord.CreateDraft(
            Guid.NewGuid(), OrgAId, item.Id, CostScopeType.Organization, null, unit.Id, "THB",
            2500m, 0m, null, effectiveFrom, effectiveTo, 1, null, null, null, null, actorId, now);
        var approverId2 = Guid.NewGuid();
        cost.Submit(actorId, now);
        cost.Approve(approverId2, now);
        cost.Publish(approverId2, now);
        db.CostRecords.Add(cost);

        if (scenario == "ambiguous")
        {
            var conflictingCost = CostRecord.CreateDraft(
                Guid.NewGuid(), OrgAId, item.Id, CostScopeType.Organization, null, unit.Id, "THB",
                2750m, 0m, null, effectiveFrom, effectiveTo, 2, null, null, null, null, actorId, now);
            conflictingCost.Submit(actorId, now);
            conflictingCost.Approve(approverId2, now);
            conflictingCost.Publish(approverId2, now);
            db.CostRecords.Add(conflictingCost);
        }

        await db.SaveChangesAsync();

        // 2. Create estimate draft
        var createMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/v1/estimates",
            "token-org-a",
            MembershipAId);
        createMsg.Headers.Add("Idempotency-Key", "idemp-estimate-catalog-0002");
        createMsg.Content = JsonContent.Create(new CreateEstimateDraftRequest(
            oppId,
            surveyRevId,
            "THB"));

        var createRes = await _client.SendAsync(createMsg);
        var estimate = (await createRes.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        var revision = estimate.CurrentRevision!;

        // Update with a mismatched input; stale and ambiguous cases must fail during resolution first.
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
                    Id: null,
                    Code: "SEC-01",
                    NameTh: "หมวดงานโซลาร์",
                    NameEn: "Solar Section",
                    SortOrder: 1,
                    WorkItems: new List<UpdateEstimateWorkItemDto>
                    {
                        new(
                            Id: null,
                            Code: "WI-01",
                            DescriptionTh: "ติดตั้งแผงโซลาร์",
                            DescriptionEn: "Install Solar",
                            Quantity: 1m,
                            UnitCode: "ชุด",
                            SellingRuleType: SellingRuleType.Margin,
                            SellingRuleValue: 0.20m,
                            SortOrder: 1,
                            CostComponents: new List<UpdateEstimateCostComponentDto>
                            {
                                new(
                                    Id: null,
                                    Type: CostComponentType.Material,
                                    Description: "แผงโซลาร์เซลล์",
                                    Quantity: 1m,
                                    UnitCode: scenario == "unit-mismatch" ? "BOX" : "PCS2",
                                    UnitCost: 2000m,
                                    Currency: "THB",
                                    SortOrder: 1,
                                    ItemId: item.Id,
                                    CostRecordId: cost.Id,
                                    CostRecordVersion: 1)
                            })
                    })
            }));

        var updateRes = await _client.SendAsync(updateMsg);
        Assert.Equal(expectedCode == "ESTIMATE_UNIT_INVALID" ? HttpStatusCode.UnprocessableEntity : HttpStatusCode.Conflict, updateRes.StatusCode);
        using (var problem = System.Text.Json.JsonDocument.Parse(await updateRes.Content.ReadAsStringAsync()))
        {
            Assert.Equal(expectedCode, problem.RootElement.GetProperty("code").GetString());
        }

        var reloadRequest = CreateAuthenticatedRequest(
            HttpMethod.Get,
            $"/api/v1/estimates/{estimate.Id}",
            "token-org-a",
            MembershipAId);
        var reloadResponse = await _client.SendAsync(reloadRequest);
        Assert.Equal(HttpStatusCode.OK, reloadResponse.StatusCode);
        var reloaded = (await reloadResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;
        Assert.Equal(revision.RowVersion, reloaded.CurrentRevision!.RowVersion);
        Assert.Empty(reloaded.CurrentRevision.Sections);
        using var verificationScope = _factory.Services.CreateScope();
        var verificationDb = verificationScope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(0, await verificationDb.EstimateCalculationSnapshots.CountAsync(snapshot =>
            snapshot.EstimateRevisionId == revision.Id));
    }

    [Fact]
    public async Task IssueQuotation_DraftEstimate_CannotAdvanceOpportunityToProposed()
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
                                new(null, CostComponentType.Material, "ไม้", 10, "แผ่น", 1000m, "THB", 1,
                                    ProvisionalReasonCode: "market-benchmark", ProvisionalNote: "Synthetic integration fixture")
                            })
                    })
            }));

        var updateRes = await _client.SendAsync(updateMsg);
        Assert.Equal(HttpStatusCode.OK, updateRes.StatusCode);
        var updatedRevision = (await updateRes.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;
        Assert.True(updatedRevision.CalculationOutdated);

        // 3. Calculate with discount
        var calcMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/revisions/{revision.Id}/calculate",
            "token-org-a",
            MembershipAId);
        calcMsg.Headers.Add("Idempotency-Key", "idemp-commercial-calc-0001");
        calcMsg.Content = JsonContent.Create(new CalculateEstimateRequest(
            updatedRevision.RowVersion,
            DiscountType: EstimateDiscount.FixedAmount,
            DiscountValue: 500m,
            DiscountReasonCode: "TEST_ONLY_DISCOUNT"));

        var calcRes = await _client.SendAsync(calcMsg);
        Assert.Equal(HttpStatusCode.OK, calcRes.StatusCode);
        var calculatedRev = (await calcRes.Content.ReadFromJsonAsync<EstimateRevisionResponse>())!;
        Assert.False(calculatedRev.CalculationOutdated);
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
        Assert.Equal(HttpStatusCode.Conflict, issueQuoteRes.StatusCode);
        using var problem = System.Text.Json.JsonDocument.Parse(await issueQuoteRes.Content.ReadAsStringAsync());
        Assert.Equal("ESTIMATE_INVALID_STATE", problem.RootElement.GetProperty("code").GetString());

        await db.Entry(oppInDb).ReloadAsync();
        Assert.Equal(OpportunityStage.Estimating, oppInDb.Stage);
        Assert.Empty(await db.Quotations.Where(q => q.EstimateId == estimate.Id).ToListAsync());
        Assert.DoesNotContain(await db.OpportunityStageHistories
                .Where(h => h.OpportunityId == oppId)
                .ToListAsync(),
            h => h.FromStage == OpportunityStage.Estimating && h.ToStage == OpportunityStage.Proposed);
    }

    [Fact]
    public async Task IssueQuotation_ApprovedEstimateWithoutPrimaryBillingAddress_ReturnsCustomerBillingNotReady()
    {
        var (estimate, opportunity, revision) = await SetupCalculatedEstimateAsync($"missing-billing-{Guid.NewGuid():N}");

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var billingAddress = await db.CustomerAddresses
            .SingleAsync(address => address.CustomerId == estimate.CustomerId && address.IsPrimary);
        billingAddress.Deactivate();
        await db.SaveChangesAsync();

        var submitRequest = CreateAuthenticatedRequest(
            HttpMethod.Post, $"/api/v1/estimates/{estimate.Id}/submit", "token-org-a", MembershipAId);
        submitRequest.Headers.Add("If-Match", $"\"{estimate.RowVersion}\"");
        submitRequest.Headers.Add("Idempotency-Key", $"missing-billing-submit-{Guid.NewGuid():N}");
        submitRequest.Content = JsonContent.Create(new SubmitEstimateRequest(
            1, revision.CalculationVersion, "TEST_ONLY billing readiness"));
        var submitResponse = await _client.SendAsync(submitRequest);
        Assert.Equal(HttpStatusCode.OK, submitResponse.StatusCode);
        var submitted = (await submitResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;

        var reviewRequest = CreateAuthenticatedRequest(
            HttpMethod.Post, $"/api/v1/estimates/{estimate.Id}/review-decisions",
            "token-org-b", TestOnlyDataSeeder.TestCostReviewerMembershipId);
        reviewRequest.Headers.Add("If-Match", $"\"{submitted.RowVersion}\"");
        reviewRequest.Headers.Add("Idempotency-Key", $"missing-billing-review-{Guid.NewGuid():N}");
        reviewRequest.Content = JsonContent.Create(new ReviewEstimateRequest(1, "approved"));
        var reviewResponse = await _client.SendAsync(reviewRequest);
        Assert.Equal(HttpStatusCode.OK, reviewResponse.StatusCode);
        var approved = (await reviewResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;

        var issueRequest = CreateAuthenticatedRequest(
            HttpMethod.Post, $"/api/v1/estimates/{estimate.Id}/quotation", "token-org-a", MembershipAId);
        issueRequest.Headers.Add("Idempotency-Key", $"missing-billing-quote-{Guid.NewGuid():N}");
        issueRequest.Content = JsonContent.Create(new IssueQuotationRequest(approved.RowVersion, opportunity.RowVersion));
        var issueResponse = await _client.SendAsync(issueRequest);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, issueResponse.StatusCode);
        using var problem = System.Text.Json.JsonDocument.Parse(await issueResponse.Content.ReadAsStringAsync());
        Assert.Equal("CUSTOMER_QUOTATION_BILLING_NOT_READY", problem.RootElement.GetProperty("code").GetString());
        Assert.Empty(await db.Quotations.Where(quotation => quotation.EstimateId == estimate.Id).ToListAsync());
    }

    [Fact]
    public async Task Submit_TestOnlyApprovalProfileFreezesCombinedTriggersAndProjectsThemToQueue()
    {
        var (estimate, _, calculatedRevision) = await SetupCalculatedEstimateAsync(
            $"test-only-policy-{Guid.NewGuid():N}", sellingRuleValue: 0.15m, costUnitAmount: 200_000m);
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await SeedTestOnlyApprovalReviewersAsync(db);
        }

        using var profileFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ASPNETCORE_ENVIRONMENT"] = "Test",
                    ["Estimates:ApprovalPolicy"] = "TEST_ONLY-TH-EST-V1",
                    ["ConnectionStrings:Database"] = _connectionString
                });
            });
            builder.ConfigureServices(services =>
            {
                var dbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (dbDescriptor is not null) services.Remove(dbDescriptor);
                services.AddDbContext<AppDbContext>(options => options.UseNpgsql(_connectionString));

                var firebaseDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFirebaseTokenVerifier));
                if (firebaseDescriptor is not null) services.Remove(firebaseDescriptor);
                services.AddSingleton<IFirebaseTokenVerifier, TestFirebaseTokenVerifier>();
            });
        });
        using var profileClient = profileFactory.CreateClient();

        var submitRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/submit", "token-org-a", MembershipAId);
        submitRequest.Headers.Add("If-Match", $"\"{estimate.RowVersion}\"");
        submitRequest.Headers.Add("Idempotency-Key", $"test-only-policy-submit-{Guid.NewGuid():N}");
        submitRequest.Content = JsonContent.Create(new SubmitEstimateRequest(
            1, calculatedRevision.CalculationVersion, "TEST_ONLY combined route"));
        var submitResponse = await profileClient.SendAsync(submitRequest);
        Assert.Equal(HttpStatusCode.OK, submitResponse.StatusCode);

        using var scopeAfterSubmit = _factory.Services.CreateScope();
        var verifyDb = scopeAfterSubmit.ServiceProvider.GetRequiredService<AppDbContext>();
        var request = await verifyDb.EstimateApprovalRequests.SingleAsync(row => row.EstimateId == estimate.Id);
        Assert.Equal("TEST_ONLY-TH-EST-V1", request.PolicyCode);
        using var routeJson = System.Text.Json.JsonDocument.Parse(request.RouteSnapshotJson);
        var route = routeJson.RootElement;
        Assert.Equal("TEST_ONLY-TH-EST-V1", route.GetProperty("policyCode").GetString());
        Assert.Equal(100_000m, route.GetProperty("thresholdSnapshot").GetProperty("managerAmountLimit").GetDecimal());
        var triggerCodes = route.GetProperty("triggerSnapshot").EnumerateArray()
            .Select(trigger => trigger.GetProperty("code").GetString()).ToArray();
        Assert.Contains("AMOUNT_AUTHORITY_EXCEEDED", triggerCodes);
        Assert.Contains("MARGIN_BELOW_MINIMUM", triggerCodes);
        Assert.Contains("PROVISIONAL_COST", triggerCodes);
        Assert.Contains("CUSTOM_WORK_ITEM", triggerCodes);
        var steps = route.GetProperty("steps").EnumerateArray().ToArray();
        Assert.Equal(
            ["TEST ONLY MANAGER", "TEST ONLY FINANCIAL APPROVER", "TEST ONLY DIRECTOR", "TEST ONLY SPECIALIST CHECKER"],
            steps.Select(step => step.GetProperty("requiredRole").GetString()));
        Assert.Equal(steps.Length, steps.Select(step => step.GetProperty("userId").GetGuid()).Distinct().Count());

        var managerMembershipId = steps[0].GetProperty("membershipId").GetGuid();
        var queueRequest = CreateAuthenticatedRequest(HttpMethod.Get,
            "/api/v1/estimates/review-queue?pageNumber=1&pageSize=10", "token-test-manager", managerMembershipId);
        var queueResponse = await profileClient.SendAsync(queueRequest);
        Assert.Equal(HttpStatusCode.OK, queueResponse.StatusCode);
        var queue = (await queueResponse.Content.ReadFromJsonAsync<EstimateReviewQueueResponse>())!;
        var queuedEstimate = Assert.Single(queue.Items);
        Assert.Contains(queuedEstimate.FrozenRoute.Triggers, trigger => trigger.Code == "AMOUNT_AUTHORITY_EXCEEDED");
        Assert.Contains(queuedEstimate.FrozenRoute.Triggers, trigger => trigger.Code == "MARGIN_BELOW_MINIMUM");
        Assert.Contains(queuedEstimate.FrozenRoute.Triggers, trigger => trigger.Code == "PROVISIONAL_COST");
        Assert.Equal(100_000m, queuedEstimate.FrozenRoute.Thresholds!.ManagerAmountLimit);
    }

    [Fact]
    public async Task EstimateActions_CrossOrganizationAndBranchAccessReturnNotFoundWithoutDisclosure()
    {
        var (estimate, _, calculatedRevision) = await SetupCalculatedEstimateAsync($"scope-matrix-{Guid.NewGuid():N}");
        Guid branchOutsiderMembershipId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            branchOutsiderMembershipId = await SeedBranchScopedEstimateOutsiderAsync(db);
        }

        var draftRevision = estimate.CurrentRevision!;
        var baselineSnapshotCount = await GetCalculationSnapshotCountAsync(estimate.Id, draftRevision.Id);
        var updateBody = new UpdateEstimateDraftRequest(draftRevision.RowVersion, []);
        var attempts = new[]
        {
            (Token: "token-org-b", MembershipId: TestOnlyDataSeeder.TestMembershipBId, Action: "read"),
            (Token: "token-branch-outsider", MembershipId: branchOutsiderMembershipId, Action: "read"),
            (Token: "token-org-b", MembershipId: TestOnlyDataSeeder.TestMembershipBId, Action: "update"),
            (Token: "token-branch-outsider", MembershipId: branchOutsiderMembershipId, Action: "update"),
            (Token: "token-org-b", MembershipId: TestOnlyDataSeeder.TestMembershipBId, Action: "calculate"),
            (Token: "token-branch-outsider", MembershipId: branchOutsiderMembershipId, Action: "calculate")
        };

        foreach (var attempt in attempts)
        {
            using var request = attempt.Action switch
            {
                "read" => CreateAuthenticatedRequest(HttpMethod.Get,
                    $"/api/v1/estimates/{estimate.Id}", attempt.Token, attempt.MembershipId),
                "update" => CreateAuthenticatedRequest(HttpMethod.Put,
                    $"/api/v1/estimates/{estimate.Id}/revisions/{draftRevision.Id}/draft", attempt.Token, attempt.MembershipId),
                _ => CreateAuthenticatedRequest(HttpMethod.Post,
                    $"/api/v1/estimates/{estimate.Id}/revisions/{draftRevision.Id}/calculate", attempt.Token, attempt.MembershipId)
            };
            if (attempt.Action == "update")
            {
                request.Headers.Add("If-Match", $"\"{draftRevision.RowVersion}\"");
                request.Content = JsonContent.Create(updateBody);
            }
            else if (attempt.Action == "calculate")
            {
                request.Headers.Add("Idempotency-Key", $"scope-matrix-{Guid.NewGuid():N}");
                request.Content = JsonContent.Create(new CalculateEstimateRequest(draftRevision.RowVersion));
            }

            using var response = await _client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.NotFound,
                $"{attempt.Token} {attempt.Action} returned {(int)response.StatusCode}: {responseBody}");
            Assert.DoesNotContain(estimate.Number, responseBody, StringComparison.Ordinal);
            Assert.DoesNotContain("grandTotal", responseBody, StringComparison.OrdinalIgnoreCase);
        }

        var postDraftRevision = await GetEstimateRevisionAsync(draftRevision.Id);
        Assert.Equal(draftRevision.RowVersion, postDraftRevision.RowVersion);
        Assert.Equal(baselineSnapshotCount, await GetCalculationSnapshotCountAsync(estimate.Id, draftRevision.Id));

        var submitRequest = CreateAuthenticatedRequest(HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/submit", "token-org-a", MembershipAId);
        submitRequest.Headers.Add("If-Match", $"\"{estimate.RowVersion}\"");
        submitRequest.Headers.Add("Idempotency-Key", $"scope-matrix-submit-{Guid.NewGuid():N}");
        submitRequest.Content = JsonContent.Create(new SubmitEstimateRequest(
            1, calculatedRevision.CalculationVersion, "TEST_ONLY scope matrix"));
        var submitResponse = await _client.SendAsync(submitRequest);
        Assert.Equal(HttpStatusCode.OK, submitResponse.StatusCode);
        var submitted = (await submitResponse.Content.ReadFromJsonAsync<EstimateDetailResponse>())!;

        foreach (var reviewer in new[]
                 {
                     (Token: "token-org-b", MembershipId: TestOnlyDataSeeder.TestMembershipBId),
                     (Token: "token-branch-outsider", MembershipId: branchOutsiderMembershipId)
                 })
        {
            var approveRequest = CreateAuthenticatedRequest(HttpMethod.Post,
                $"/api/v1/estimates/{estimate.Id}/review-decisions", reviewer.Token, reviewer.MembershipId);
            approveRequest.Headers.Add("If-Match", $"\"{submitted.RowVersion}\"");
            approveRequest.Headers.Add("Idempotency-Key", $"scope-matrix-approve-{Guid.NewGuid():N}");
            approveRequest.Content = JsonContent.Create(new ReviewEstimateRequest(1, "approved"));
            using var approveResponse = await _client.SendAsync(approveRequest);
            Assert.Equal(HttpStatusCode.NotFound, approveResponse.StatusCode);
            var responseBody = await approveResponse.Content.ReadAsStringAsync();
            Assert.DoesNotContain(estimate.Number, responseBody, StringComparison.Ordinal);
            Assert.DoesNotContain("grandTotal", responseBody, StringComparison.OrdinalIgnoreCase);
        }

        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(EstimateRevisionStatus.Submitted,
            await verifyDb.EstimateRevisions.Where(row => row.Id == draftRevision.Id).Select(row => row.Status).SingleAsync());
        async Task<int> GetCalculationSnapshotCountAsync(Guid estimateId, Guid revisionId)
        {
            using var countScope = _factory.Services.CreateScope();
            var countDb = countScope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await countDb.EstimateCalculationSnapshots.CountAsync(row =>
                row.OrganizationId == OrgAId && row.EstimateRevisionId == revisionId);
        }

        async Task<EstimateRevision> GetEstimateRevisionAsync(Guid revisionId)
        {
            using var revisionScope = _factory.Services.CreateScope();
            var revisionDb = revisionScope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await revisionDb.EstimateRevisions.SingleAsync(row => row.Id == revisionId);
        }

        Assert.Empty(await verifyDb.EstimateApprovalDecisions.Where(decision =>
            verifyDb.EstimateApprovalRequests.Any(request => request.Id == decision.EstimateApprovalRequestId && request.EstimateId == estimate.Id))
            .ToListAsync());
    }

    private static async Task<Guid> SeedBranchScopedEstimateOutsiderAsync(AppDbContext db)
    {
        var branchId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var membershipId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        db.Branches.Add(new Branch(branchId, OrgAId, $"OUT-{branchId:N}"[..12], "Outside Branch"));
        db.Users.Add(new User(userId, "test-only-branch-outsider", "Branch Outsider", "branch-outsider@example.test"));
        db.Memberships.Add(new Membership(membershipId, OrgAId, branchId, userId));
        db.Roles.Add(new Role(roleId, OrgAId, "TEST ONLY BRANCH OUTSIDER", "TEST_ONLY branch scoped access"));
        db.MembershipRoles.Add(new MembershipRole(membershipId, roleId, OrgAId));
        var permissionRows = await db.Permissions.Where(row => row.Key == "estimates.read" ||
            row.Key == "estimates.update" || row.Key == "estimates.calculate" || row.Key == "estimates.approve").ToListAsync();
        foreach (var permission in permissionRows)
        {
            db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), roleId, OrgAId, permission.Id,
                PermissionScope.Branch, branchId, branchId));
        }
        await db.SaveChangesAsync();
        return membershipId;
    }

    private static async Task SeedTestOnlyApprovalReviewersAsync(AppDbContext db)
    {
        var roles = new[]
        {
            "TEST ONLY MANAGER",
            "TEST ONLY FINANCIAL APPROVER",
            "TEST ONLY DIRECTOR",
            "TEST ONLY SPECIALIST CHECKER"
        };
        var permission = await db.Permissions.SingleAsync(row => row.Key == "estimates.approve");
        for (var index = 0; index < roles.Length; index++)
        {
            var roleName = roles[index];
            var userId = Guid.NewGuid();
            var membershipId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var user = new User(userId, $"test-only-approval-{index}", roleName, $"test-approval-{index}@example.test");
            var membership = new Membership(membershipId, OrgAId, BranchAId, userId, isActive: true);
            var role = new Role(roleId, OrgAId, roleName, "TEST_ONLY approval role", isActive: true);
            db.Users.Add(user);
            db.Memberships.Add(membership);
            db.Roles.Add(role);
            db.MembershipRoles.Add(new MembershipRole(membershipId, roleId, OrgAId));
            db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), roleId, OrgAId, permission.Id,
                PermissionScope.Organization, OrgAId));
        }
        await db.SaveChangesAsync();
    }

    private async Task<(EstimateDetailResponse estimate, Opportunity opp, EstimateRevisionResponse calculatedRev)> SetupCalculatedEstimateAsync(
        string keyPrefix = "calc-est",
        decimal sellingRuleValue = 0.20m,
        decimal costUnitAmount = 1000m)
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
                            sellingRuleValue,
                            1,
                            new List<UpdateEstimateCostComponentDto>
                            {
                                new(null, CostComponentType.Material, "ไม้", 10, "แผ่น", costUnitAmount, "THB", 1,
                                    ProvisionalReasonCode: "market-benchmark", ProvisionalNote: "Synthetic integration fixture")
                            },
                            OverrideReasonCode: "TEST_ONLY",
                            OverrideReason: "Synthetic custom work item fixture")
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
        calcMsg.Content = JsonContent.Create(new CalculateEstimateRequest(
            updatedRevision.RowVersion,
            DiscountType: EstimateDiscount.FixedAmount,
            DiscountValue: 500m,
            DiscountReasonCode: "TEST_ONLY_DISCOUNT"));

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
    public async Task IssueQuotation_DraftCalculated_ReturnsEstimateInvalidStateWithoutNumberOrStageChange()
    {
        var (estimate, opportunity, _) = await SetupCalculatedEstimateAsync($"draft-quote-{Guid.NewGuid():N}");

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var initialQuotationCount = await db.Quotations.CountAsync(q => q.EstimateId == estimate.Id);
        var initialCounterValue = await db.DocumentSequenceCounters
            .Where(counter => counter.OrganizationId == estimate.OrganizationId &&
                              counter.DocumentType == DocumentTypes.Quotations)
            .SumAsync(counter => (long?)counter.CurrentValue) ?? 0;

        var issueMessage = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"/api/v1/estimates/{estimate.Id}/quotation",
            "token-org-a",
            MembershipAId);
        issueMessage.Headers.Add("Idempotency-Key", $"idemp-draft-quote-{Guid.NewGuid():N}");
        issueMessage.Content = JsonContent.Create(new IssueQuotationRequest(estimate.RowVersion, opportunity.RowVersion));

        var response = await _client.SendAsync(issueMessage);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        using var problem = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("ESTIMATE_INVALID_STATE", problem.RootElement.GetProperty("code").GetString());

        var refreshedOpportunity = await db.Opportunities.AsNoTracking()
            .FirstAsync(row => row.Id == opportunity.Id);
        var finalQuotationCount = await db.Quotations.CountAsync(q => q.EstimateId == estimate.Id);
        var finalCounterValue = await db.DocumentSequenceCounters
            .Where(counter => counter.OrganizationId == estimate.OrganizationId &&
                              counter.DocumentType == DocumentTypes.Quotations)
            .SumAsync(counter => (long?)counter.CurrentValue) ?? 0;

        Assert.Equal(EstimateRevisionStatus.Draft, estimate.CurrentRevision?.Status);
        Assert.Equal(OpportunityStage.Estimating, refreshedOpportunity.Stage);
        Assert.Equal(opportunity.RowVersion, refreshedOpportunity.RowVersion);
        Assert.Equal(initialQuotationCount, finalQuotationCount);
        Assert.Equal(initialCounterValue, finalCounterValue);
    }

    [Fact]
    public async Task IssueQuotation_DraftReplayIntent_IsRejectedWithoutPersistingIdempotencyRecord()
    {
        var (estimate, opp, _) = await SetupCalculatedEstimateAsync($"replay-{Guid.NewGuid():N}");

        var idempotencyKey = $"idemp-quote-replay-{Guid.NewGuid():N}";

        // Draft quotation attempts are rejected before idempotency or sequence state is persisted.
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
        Assert.Equal(HttpStatusCode.Conflict, issueRes.StatusCode);

        // Repeating the same rejected intent remains rejected.
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
        Assert.Equal(HttpStatusCode.Conflict, replayRes.StatusCode);

        // Assert exactly one Quotation in DB for this estimate
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var quotationsCount = await db.Quotations.CountAsync(q => q.EstimateId == estimate.Id);
        Assert.Equal(0, quotationsCount);

        var stageHistoryCount = await db.OpportunityStageHistories
            .CountAsync(h => h.OpportunityId == opp.Id && h.ToStage == OpportunityStage.Proposed);
        Assert.Equal(0, stageHistoryCount);

        var auditCount = await db.AuditEvents
            .CountAsync(a => a.Action == "quotations.issued" && a.ResourceId == estimate.Id.ToString());
        Assert.Equal(0, auditCount);

        var idempRecordCount = await db.IdempotencyRecords
            .CountAsync(r => r.Operation == "quotations.issue" && r.ResourceId == estimate.Id.ToString());
        Assert.Equal(0, idempRecordCount);
    }

    [Fact]
    public async Task IssueQuotation_TwoDraftEstimates_DoesNotAllocateNumbers()
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

        Assert.Equal(HttpStatusCode.Conflict, responses[0].StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, responses[1].StatusCode);
    }

    [Fact]
    public async Task AcceptQuotation_CannotStartFromUnapprovedDraftEstimate()
    {
        var (estimate, opp, _) = await SetupCalculatedEstimateAsync($"accept-{Guid.NewGuid():N}");

        // An unapproved draft cannot produce a quotation to accept.
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
        Assert.Equal(HttpStatusCode.Conflict, issueRes.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var quotationCount = await db.Quotations.CountAsync(q => q.EstimateId == estimate.Id);
        Assert.Equal(0, quotationCount);

        var wonHistoryCount = await db.OpportunityStageHistories
            .CountAsync(h => h.OpportunityId == opp.Id && h.ToStage == OpportunityStage.Won);
        Assert.Equal(0, wonHistoryCount);
    }
}
