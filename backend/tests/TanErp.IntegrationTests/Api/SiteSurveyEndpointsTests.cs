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
        Assert.Equal(SurveyDefaults.BaselineTemplateVersion, survey.CurrentRevision.SurveyTemplateVersion);

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
}
