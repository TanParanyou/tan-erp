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

    private async Task<(Guid customerId, Guid siteId, Guid opportunityId, Guid oppVersion)> SetupEstimatingOpportunityAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTimeOffset.UtcNow;

        var customer = Customer.CreateDraft(
            Guid.NewGuid(), OrgAId, UserAId, CustomerType.Person, "คุณลูกค้า สำหรับ Estimate", null, "th",
            new PrimaryContactInput("คุณสมชาย", null, "0812345678", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        db.Customers.Add(customer);

        var address = new SiteAddressInput("123 ถนนสุขุมวิท", "คลองเตย", "คลองเตย", "กรุงเทพมหานคร", "10110", "TH");
        var site = Site.CreateActive(Guid.NewGuid(), OrgAId, customer.Id, UserAId, "บ้านพักอาศัย", address, 13.7m, 100.5m, null, now);
        db.Sites.Add(site);

        var opp = Opportunity.CreateDraft(
            Guid.NewGuid(), OrgAId, BranchAId, customer.Id, site.Id, UserAId, UserAId,
            "งานประเมินราคาบิลท์อิน", "ขอบเขตงานตู้เสื้อผ้า", new[] { "built-in" }, null, 500000m, "THB",
            new DateOnly(2026, 12, 31), now.AddDays(2), "เตรียมประเมินราคา", now);
        opp.Qualify(opp.RowVersion);
        opp.EnterSurveying(opp.RowVersion, site.Id);
        opp.EnterEstimating(opp.RowVersion);
        db.Opportunities.Add(opp);

        await db.SaveChangesAsync();

        return (customer.Id, site.Id, opp.Id, opp.RowVersion);
    }

    [Fact]
    public async Task CreateEstimateDraft_ValidScope_ReturnsCreated()
    {
        var (customerId, _, oppId, _) = await SetupEstimatingOpportunityAsync();

        var requestMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/v1/estimates",
            "token-org-a",
            MembershipAId);
        requestMsg.Headers.Add("Idempotency-Key", "idemp-estimate-create-0001");
        requestMsg.Content = JsonContent.Create(new CreateEstimateDraftRequest(
            customerId,
            oppId,
            BranchAId,
            SiteSurveyRevisionId: null,
            SiteSurveySnapshotHash: null,
            Currency: "THB"));

        var response = await _client.SendAsync(requestMsg);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var estimate = await response.Content.ReadFromJsonAsync<EstimateDetailResponse>();
        Assert.NotNull(estimate);
        Assert.Equal(oppId, estimate.OpportunityId);
        Assert.Equal(customerId, estimate.CustomerId);
        Assert.Equal(BranchAId, estimate.BranchId);
        Assert.Equal(EstimateStatus.Draft, estimate.Status);
        Assert.StartsWith("EST-", estimate.Number);
        Assert.Equal(1, estimate.CurrentRevisionNo);
        Assert.NotNull(estimate.CurrentRevision);
        Assert.Equal(1, estimate.CurrentRevision.RevisionNo);
        Assert.Equal(EstimateRevisionStatus.Draft, estimate.CurrentRevision.Status);
    }

    [Fact]
    public async Task UpdateEstimateDraft_ValidSections_UpdatesAndRotatesRowVersion()
    {
        var (customerId, _, oppId, _) = await SetupEstimatingOpportunityAsync();

        // 1. Create estimate
        var createMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/v1/estimates",
            "token-org-a",
            MembershipAId);
        createMsg.Headers.Add("Idempotency-Key", "idemp-estimate-create-0002");
        createMsg.Content = JsonContent.Create(new CreateEstimateDraftRequest(
            customerId,
            oppId,
            BranchAId,
            null,
            null,
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
        var (customerId, _, oppId, _) = await SetupEstimatingOpportunityAsync();

        // 1. Create estimate
        var createMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/v1/estimates",
            "token-org-a",
            MembershipAId);
        createMsg.Headers.Add("Idempotency-Key", "idemp-estimate-create-0003");
        createMsg.Content = JsonContent.Create(new CreateEstimateDraftRequest(
            customerId,
            oppId,
            BranchAId,
            null,
            null,
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
        var (customerId, _, oppId, _) = await SetupEstimatingOpportunityAsync();

        // 1. Create estimate
        var createMsg = CreateAuthenticatedRequest(
            HttpMethod.Post,
            "/api/v1/estimates",
            "token-org-a",
            MembershipAId);
        createMsg.Headers.Add("Idempotency-Key", "idemp-estimate-create-0004");
        createMsg.Content = JsonContent.Create(new CreateEstimateDraftRequest(
            customerId,
            oppId,
            BranchAId,
            null,
            null,
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
}
