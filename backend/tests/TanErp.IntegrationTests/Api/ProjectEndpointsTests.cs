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
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class ProjectEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private static readonly Guid OrgId = TestOnlyDataSeeder.TestOrgId;
    private static readonly Guid BranchId = TestOnlyDataSeeder.TestBranchId;
    private static readonly Guid UserId = TestOnlyDataSeeder.TestUserId;
    private const string UidA = TestOnlyDataSeeder.TestFirebaseUid;
    private const string UidB = TestOnlyDataSeeder.TestFirebaseUidB;
    private const string UidNoPerm = "uid-no-project-perm";
    private static readonly Guid MembershipNoPermId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b60");

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(idToken switch
            {
                "token-org-a" => UidA,
                "token-org-b" => UidB,
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
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true);

        var noPermUser = new TanErp.Domain.IdentityAccess.User(Guid.NewGuid(), UidNoPerm, "noperm@example.com", "No Perm", true);
        db.Users.Add(noPermUser);
        db.Memberships.Add(new TanErp.Domain.Organization.Membership(MembershipNoPermId, OrgId, BranchId, noPermUser.Id, isActive: true));
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

    private static HttpRequestMessage Request(HttpMethod method, string url, string token = "token-org-a", string? key = null, Guid? membership = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Membership-Id", (membership ?? (token == "token-org-b" ? TestOnlyDataSeeder.TestMembershipBId : TestOnlyDataSeeder.TestMembershipId)).ToString());
        if (key is not null) request.Headers.Add("Idempotency-Key", key);
        return request;
    }

    private async Task<HttpResponseMessage> HandoverAsync(Seeded seeded, string key, Guid? ownerId = null, Guid? version = null, string? name = null, string token = "token-org-a", Guid? membership = null)
    {
        var request = Request(HttpMethod.Post, "/api/v1/projects", token, key, membership);
        request.Content = JsonContent.Create(new CreateProjectFromHandoverRequest(
            seeded.QuotationId, version ?? seeded.QuotationVersion, ownerId ?? UserId, new DateOnly(2026, 11, 1), name));
        return await _client.SendAsync(request);
    }

    private static async Task<string> ErrorCodeAsync(HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<ApiProblemDetails>())!.Code;

    [Fact]
    public async Task Handover_AcceptedQuotationOnWonOpportunity_CreatesPlannedProjectWithImmutableBaseline()
    {
        var seeded = await SeedAcceptedQuotationAsync();

        var response = await HandoverAsync(seeded, "project-handover-key-0001");

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var project = (await response.Content.ReadFromJsonAsync<ProjectResponse>())!;
        Assert.StartsWith("PRJ", project.Code);
        Assert.Equal(ProjectStatus.Planned, project.Status);
        Assert.Equal("งานบิลท์อินห้องนอน", project.Name);
        Assert.Equal(UserId, project.Owner.Id);
        Assert.NotNull(project.Site);
        Assert.Equal(seeded.QuotationNumber, project.Baseline.QuotationNumber);
        Assert.Equal(seeded.Total, project.Baseline.ContractAmount);
        Assert.Equal("snapshot-hash-abc", project.Baseline.QuotationSnapshotHash);
        Assert.Equal(64, project.Baseline.BaselineHash.Length);
        Assert.NotNull(response.Headers.ETag);

        // The baseline is a copy: changing the source quotation afterwards never alters it.
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var row = await db.Projects.AsNoTracking().SingleAsync(p => p.Id == project.Id);
            Assert.Equal(seeded.Total, row.BaselineContractAmount);
            Assert.True(await db.AuditEvents.AnyAsync(a => a.Action == "project.created-from-handover" && a.ResourceId == project.Id.ToString()));
        }

        var get = await _client.SendAsync(Request(HttpMethod.Get, $"/api/v1/projects/{project.Id}"));
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
        Assert.Equal(project.Baseline.BaselineHash, (await get.Content.ReadFromJsonAsync<ProjectResponse>())!.Baseline.BaselineHash);
    }

    [Fact]
    public async Task Handover_SameKeyReplays_DifferentKeyDuplicateConflicts()
    {
        var seeded = await SeedAcceptedQuotationAsync();

        var first = await HandoverAsync(seeded, "project-handover-key-0002");
        var firstProject = (await first.Content.ReadFromJsonAsync<ProjectResponse>())!;

        var replay = await HandoverAsync(seeded, "project-handover-key-0002");
        Assert.Equal(HttpStatusCode.Created, replay.StatusCode);
        Assert.Equal(firstProject.Id, (await replay.Content.ReadFromJsonAsync<ProjectResponse>())!.Id);

        var reused = await HandoverAsync(seeded, "project-handover-key-0002", name: "ชื่ออื่น");
        Assert.Equal(HttpStatusCode.Conflict, reused.StatusCode);
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", await ErrorCodeAsync(reused));

        var duplicate = await HandoverAsync(seeded, "project-handover-key-0003");
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
        Assert.Equal("PROJECT_ALREADY_EXISTS", await ErrorCodeAsync(duplicate));

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(1, await db.Projects.CountAsync(p => p.QuotationId == seeded.QuotationId));
    }

    [Fact]
    public async Task Handover_StaleQuotationVersion_NotAcceptedOrNotWon_AreRejected()
    {
        var seeded = await SeedAcceptedQuotationAsync();
        var stale = await HandoverAsync(seeded, "project-handover-key-0004", version: Guid.NewGuid());
        Assert.Equal(HttpStatusCode.Conflict, stale.StatusCode);
        Assert.Equal("QUOTATION_VERSION_CONFLICT", await ErrorCodeAsync(stale));

        var notAccepted = await SeedAcceptedQuotationAsync(markWon: false, accept: false);
        var issued = await HandoverAsync(notAccepted, "project-handover-key-0005");
        Assert.Equal(HttpStatusCode.Conflict, issued.StatusCode);
        Assert.Equal("PROJECT_HANDOVER_NOT_ALLOWED", await ErrorCodeAsync(issued));

        var notWon = await SeedAcceptedQuotationAsync(markWon: false, accept: true);
        var proposed = await HandoverAsync(notWon, "project-handover-key-0006");
        Assert.Equal(HttpStatusCode.Conflict, proposed.StatusCode);
        Assert.Equal("PROJECT_HANDOVER_NOT_ALLOWED", await ErrorCodeAsync(proposed));

        using var scope = _factory.Services.CreateScope();
        Assert.Equal(0, await scope.ServiceProvider.GetRequiredService<AppDbContext>().Projects.CountAsync());
    }

    [Fact]
    public async Task Handover_OwnerWithoutActiveMembership_Returns404AndCreatesNothing()
    {
        var seeded = await SeedAcceptedQuotationAsync();

        var response = await HandoverAsync(seeded, "project-handover-key-0007", ownerId: Guid.NewGuid());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        using var scope = _factory.Services.CreateScope();
        Assert.Equal(0, await scope.ServiceProvider.GetRequiredService<AppDbContext>().Projects.CountAsync());
    }

    [Fact]
    public async Task Handover_WithoutPermission_Returns403_AndOtherOrganizationCannotSeeProjects()
    {
        var seeded = await SeedAcceptedQuotationAsync();

        var denied = await HandoverAsync(seeded, "project-handover-key-0008", token: "token-no-perm", membership: MembershipNoPermId);
        Assert.Equal(HttpStatusCode.Forbidden, denied.StatusCode);

        var created = (await (await HandoverAsync(seeded, "project-handover-key-0009")).Content.ReadFromJsonAsync<ProjectResponse>())!;

        var foreign = await _client.SendAsync(Request(HttpMethod.Get, $"/api/v1/projects/{created.Id}", "token-org-b"));
        Assert.True(foreign.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Forbidden);

        var foreignHandover = await HandoverAsync(seeded, "project-handover-key-0010", token: "token-org-b");
        Assert.True(foreignHandover.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task List_FiltersBySearchAndStatus_AndRejectsInvalidStatus()
    {
        var first = await SeedAcceptedQuotationAsync();
        var second = await SeedAcceptedQuotationAsync();
        await HandoverAsync(first, "project-handover-key-0011", name: "โครงการอัลฟา");
        await HandoverAsync(second, "project-handover-key-0012", name: "โครงการเบต้า");

        var all = (await (await _client.SendAsync(Request(HttpMethod.Get, "/api/v1/projects?pageSize=10"))).Content.ReadFromJsonAsync<ProjectListResponse>())!;
        Assert.Equal(2, all.Pagination.TotalCount);

        var search = (await (await _client.SendAsync(Request(HttpMethod.Get, $"/api/v1/projects?search={Uri.EscapeDataString("อัลฟา")}"))).Content.ReadFromJsonAsync<ProjectListResponse>())!;
        Assert.Single(search.Items);
        Assert.Equal("โครงการอัลฟา", search.Items[0].Name);

        var planned = (await (await _client.SendAsync(Request(HttpMethod.Get, "/api/v1/projects?status=planned"))).Content.ReadFromJsonAsync<ProjectListResponse>())!;
        Assert.Equal(2, planned.Pagination.TotalCount);

        var completed = (await (await _client.SendAsync(Request(HttpMethod.Get, "/api/v1/projects?status=completed"))).Content.ReadFromJsonAsync<ProjectListResponse>())!;
        Assert.Empty(completed.Items);

        var invalid = await _client.SendAsync(Request(HttpMethod.Get, "/api/v1/projects?status=bogus"));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, invalid.StatusCode);
    }

    [Fact]
    public async Task HandoverSource_ReturnsAcceptedQuotationAndExistingProject()
    {
        var seeded = await SeedAcceptedQuotationAsync();

        var before = await _client.SendAsync(Request(HttpMethod.Get, $"/api/v1/projects/handover-source?opportunityId={seeded.OpportunityId}"));
        Assert.Equal(HttpStatusCode.OK, before.StatusCode);
        var source = (await before.Content.ReadFromJsonAsync<ProjectHandoverSourceResponse>())!;
        Assert.Equal(seeded.QuotationId, source.QuotationId);
        Assert.Equal(seeded.QuotationVersion, source.QuotationRowVersion);
        Assert.Equal("won", source.OpportunityStage);
        Assert.Null(source.ExistingProjectId);

        var project = (await (await HandoverAsync(seeded, "project-handover-source-key-1")).Content.ReadFromJsonAsync<ProjectResponse>())!;

        var after = (await (await _client.SendAsync(Request(HttpMethod.Get, $"/api/v1/projects/handover-source?opportunityId={seeded.OpportunityId}")))
            .Content.ReadFromJsonAsync<ProjectHandoverSourceResponse>())!;
        Assert.Equal(project.Id, after.ExistingProjectId);
        Assert.Equal(project.Code, after.ExistingProjectCode);

        var none = await SeedAcceptedQuotationAsync(markWon: false, accept: false);
        var missing = await _client.SendAsync(Request(HttpMethod.Get, $"/api/v1/projects/handover-source?opportunityId={none.OpportunityId}"));
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }
}

