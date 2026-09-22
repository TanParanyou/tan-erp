using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.Items;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class CostRecordEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private const string UidMaker = TestOnlyDataSeeder.TestFirebaseUid;
    private const string UidChecker = TestOnlyDataSeeder.TestFirebaseUidB;
    private static readonly Guid MembershipMakerId = TestOnlyDataSeeder.TestMembershipId;
    private static readonly Guid MembershipCheckerId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f9999");

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
        {
            var uid = idToken switch
            {
                "token-maker" => UidMaker,
                "token-checker" => UidChecker,
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
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                services.AddSingleton<IFirebaseTokenVerifier, TestFirebaseTokenVerifier>();
            });
        });

        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true);

        // Add Checker to Org A with same admin permissions to test maker-checker rule
        var checkerRole = await db.Roles.FirstOrDefaultAsync(r => r.OrganizationId == TestOnlyDataSeeder.TestOrgId && r.Name == "Test Admin");
        var checkerUser = await db.Users.FirstOrDefaultAsync(u => u.FirebaseUid == UidChecker);
        if (checkerUser != null && checkerRole != null)
        {
            var checkerMembershipInOrgA = new TanErp.Domain.Organization.Membership(
                MembershipCheckerId,
                TestOnlyDataSeeder.TestOrgId,
                TestOnlyDataSeeder.TestBranchId,
                checkerUser.Id,
                isActive: true);
            db.Memberships.Add(checkerMembershipInOrgA);

            var checkerRoleAssignment = new TanErp.Domain.IdentityAccess.MembershipRole(
                MembershipCheckerId,
                checkerRole.Id,
                TestOnlyDataSeeder.TestOrgId);
            db.MembershipRoles.Add(checkerRoleAssignment);
            await db.SaveChangesAsync();
        }
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private HttpRequestMessage CreateRequest(
        HttpMethod method,
        string url,
        string token = "token-maker",
        Guid? membershipId = null,
        Guid? ifMatch = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Membership-Id", (membershipId ?? MembershipMakerId).ToString());

        if (ifMatch.HasValue)
        {
            request.Headers.Add("If-Match", $"\"{ifMatch.Value}\"");
        }

        return request;
    }

    private async Task<(Guid ItemId, Guid UnitId)> CreateTestItemAsync()
    {
        // 1. Create Category
        var catReq = CreateRequest(HttpMethod.Post, "/api/v1/item-categories");
        catReq.Content = JsonContent.Create(new CreateItemCategoryRequest
        {
            Code = "CAT-" + Guid.NewGuid().ToString("N")[..6],
            Name = new LocalizedTextInput { Thai = "หมวดหมู่ทดสอบต้นทุน", English = "Cost Test Cat" },
            AllowedItemTypes = new List<string> { "material" }
        });
        var catRes = await _client.SendAsync(catReq);
        var cat = await catRes.Content.ReadFromJsonAsync<ItemCategoryDetailResponse>();

        // 2. Create Unit
        var unitReq = CreateRequest(HttpMethod.Post, "/api/v1/units-of-measure");
        unitReq.Content = JsonContent.Create(new CreateUnitOfMeasureRequest
        {
            Code = "UOM-" + Guid.NewGuid().ToString("N")[..6],
            Name = new LocalizedTextInput { Thai = "ชิ้น", English = "Piece" },
            Symbol = "pcs"
        });
        var unitRes = await _client.SendAsync(unitReq);
        var unit = await unitRes.Content.ReadFromJsonAsync<UnitOfMeasureDetailResponse>();

        // 3. Create Item
        var itemReq = CreateRequest(HttpMethod.Post, "/api/v1/items");
        itemReq.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = "COST-ITEM-" + Guid.NewGuid().ToString("N")[..6],
            ItemType = "material",
            CategoryId = cat!.Id,
            BaseUnitId = unit!.Id,
            Name = new LocalizedTextInput { Thai = "สินค้าทดสอบต้นทุน", English = "Cost Test Item" }
        });
        var itemRes = await _client.SendAsync(itemReq);
        var item = await itemRes.Content.ReadFromJsonAsync<ItemResponse>();
        return (item!.Id, unit.Id);
    }

    [Fact]
    public async Task CreateDraft_Valid_CreatesDraftAndReturnsETag()
    {
        var (itemId, unitId) = await CreateTestItemAsync();
        var now = DateTimeOffset.UtcNow;

        var req = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs");
        req.Content = JsonContent.Create(new CreateCostRecordRequest
        {
            Scope = "organization",
            UnitId = unitId,
            Currency = "THB",
            Amount = 150.00m,
            MinimumQuantity = 0,
            EffectiveFromUtc = now,
            SourceReference = "SUPPLIER-QUOTE-123"
        });

        var res = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        Assert.NotNull(res.Headers.ETag);

        var cost = await res.Content.ReadFromJsonAsync<CostRecordResponse>();
        Assert.NotNull(cost);
        Assert.Equal(150.00m, cost.Amount);
        Assert.Equal("draft", cost.Status);
        Assert.Equal(1, cost.Version);
    }

    [Fact]
    public async Task UpdateDraft_StaleETag_ReturnsConflict()
    {
        var (itemId, unitId) = await CreateTestItemAsync();
        var now = DateTimeOffset.UtcNow;

        // 1. Create draft
        var createReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs");
        createReq.Content = JsonContent.Create(new CreateCostRecordRequest
        {
            Scope = "organization",
            UnitId = unitId,
            Currency = "THB",
            Amount = 100.00m,
            EffectiveFromUtc = now
        });
        var createRes = await _client.SendAsync(createReq);
        var cost = await createRes.Content.ReadFromJsonAsync<CostRecordResponse>();

        // 2. Update with bogus If-Match
        var updateReq = CreateRequest(HttpMethod.Put, $"/api/v1/items/{itemId}/costs/{cost!.Id}", ifMatch: Guid.NewGuid());
        updateReq.Content = JsonContent.Create(new UpdateCostRecordRequest
        {
            Amount = 200.00m,
            Currency = "THB",
            UnitId = unitId,
            EffectiveFromUtc = now
        });
        var updateRes = await _client.SendAsync(updateReq);

        Assert.Equal(HttpStatusCode.Conflict, updateRes.StatusCode);
    }

    [Fact]
    public async Task MakerChecker_MakerCannotApproveOwnCost_CheckerCanApprove()
    {
        var (itemId, unitId) = await CreateTestItemAsync();
        var now = DateTimeOffset.UtcNow;

        // 1. Maker creates draft
        var createReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs", token: "token-maker", membershipId: MembershipMakerId);
        createReq.Content = JsonContent.Create(new CreateCostRecordRequest
        {
            Scope = "organization",
            UnitId = unitId,
            Currency = "THB",
            Amount = 500.00m,
            EffectiveFromUtc = now
        });
        var createRes = await _client.SendAsync(createReq);
        var cost = await createRes.Content.ReadFromJsonAsync<CostRecordResponse>();

        // 2. Maker submits
        var submitReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs/{cost!.Id}/submit", token: "token-maker", membershipId: MembershipMakerId);
        var submitRes = await _client.SendAsync(submitReq);
        Assert.Equal(HttpStatusCode.OK, submitRes.StatusCode);
        var submittedCost = await submitRes.Content.ReadFromJsonAsync<CostRecordResponse>();
        Assert.Equal("submitted", submittedCost!.Status);

        // 3. Maker attempts to approve own record -> 422 Maker-Checker violation
        var approveMakerReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs/{cost.Id}/approve", token: "token-maker", membershipId: MembershipMakerId);
        var approveMakerRes = await _client.SendAsync(approveMakerReq);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, approveMakerRes.StatusCode);

        // 4. Checker approves record -> 200 OK
        var approveCheckerReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs/{cost.Id}/approve", token: "token-checker", membershipId: MembershipCheckerId);
        var approveCheckerRes = await _client.SendAsync(approveCheckerReq);
        Assert.Equal(HttpStatusCode.OK, approveCheckerRes.StatusCode);
        var approvedCost = await approveCheckerRes.Content.ReadFromJsonAsync<CostRecordResponse>();
        Assert.Equal("approved", approvedCost!.Status);

        // 5. Checker publishes record -> 200 OK
        var publishReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs/{cost.Id}/publish", token: "token-checker", membershipId: MembershipCheckerId);
        var publishRes = await _client.SendAsync(publishReq);
        Assert.Equal(HttpStatusCode.OK, publishRes.StatusCode);
        var publishedCost = await publishRes.Content.ReadFromJsonAsync<CostRecordResponse>();
        Assert.Equal("published", publishedCost!.Status);

        // 6. Attempt to modify published record -> 422 Immutable
        var editPublishedReq = CreateRequest(HttpMethod.Put, $"/api/v1/items/{itemId}/costs/{cost.Id}", token: "token-maker", membershipId: MembershipMakerId);
        editPublishedReq.Content = JsonContent.Create(new UpdateCostRecordRequest
        {
            Amount = 999.00m,
            Currency = "THB",
            UnitId = unitId,
            EffectiveFromUtc = now
        });
        var editPublishedRes = await _client.SendAsync(editPublishedReq);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, editPublishedRes.StatusCode);
    }

    [Fact]
    public async Task ReturnDraft_RequiresReason_CreatesReviewRecord()
    {
        var (itemId, unitId) = await CreateTestItemAsync();
        var now = DateTimeOffset.UtcNow;

        // Maker creates and submits
        var createReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs", token: "token-maker", membershipId: MembershipMakerId);
        createReq.Content = JsonContent.Create(new CreateCostRecordRequest
        {
            Scope = "organization",
            UnitId = unitId,
            Currency = "THB",
            Amount = 300.00m,
            EffectiveFromUtc = now
        });
        var createRes = await _client.SendAsync(createReq);
        var cost = await createRes.Content.ReadFromJsonAsync<CostRecordResponse>();

        var submitReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs/{cost!.Id}/submit", token: "token-maker", membershipId: MembershipMakerId);
        await _client.SendAsync(submitReq);

        // Checker returns with reason
        var returnReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs/{cost.Id}/return", token: "token-checker", membershipId: MembershipCheckerId);
        returnReq.Content = JsonContent.Create(new ReturnCostRecordRequest { Reason = "ต้นทุนสูงเกินไป ให้ต่อรองใหม่" });
        var returnRes = await _client.SendAsync(returnReq);
        Assert.Equal(HttpStatusCode.OK, returnRes.StatusCode);

        var returnedCost = await returnRes.Content.ReadFromJsonAsync<CostRecordResponse>();
        Assert.Equal("returned", returnedCost!.Status);

        // Verify review entry
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var reviews = await db.CostRecordReviews.Where(r => r.CostRecordId == cost.Id).ToListAsync();
        Assert.Contains(reviews, r => r.Decision == "returned" && r.Reason == "ต้นทุนสูงเกินไป ให้ต่อรองใหม่");
    }

    [Fact]
    public async Task Disable_RequiresReason_CreatesReviewRecord()
    {
        var (itemId, unitId) = await CreateTestItemAsync();
        var now = DateTimeOffset.UtcNow;

        var createReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs", token: "token-maker", membershipId: MembershipMakerId);
        createReq.Content = JsonContent.Create(new CreateCostRecordRequest
        {
            Scope = "organization",
            UnitId = unitId,
            Currency = "THB",
            Amount = 400.00m,
            EffectiveFromUtc = now
        });
        var createRes = await _client.SendAsync(createReq);
        var cost = await createRes.Content.ReadFromJsonAsync<CostRecordResponse>();

        // Checker disables with reason
        var disableReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs/{cost!.Id}/disable", token: "token-checker", membershipId: MembershipCheckerId);
        disableReq.Content = JsonContent.Create(new DisableCostRecordRequest { Reason = "ซัพพลายเออร์ยกเลิกราคานี้" });
        var disableRes = await _client.SendAsync(disableReq);
        Assert.Equal(HttpStatusCode.OK, disableRes.StatusCode);

        var disabledCost = await disableRes.Content.ReadFromJsonAsync<CostRecordResponse>();
        Assert.Equal("disabled", disabledCost!.Status);
        Assert.Equal("ซัพพลายเออร์ยกเลิกราคานี้", disabledCost.Reason);
    }

    [Fact]
    public async Task Publish_OverlappingRanges_ReturnsConflict()
    {
        var (itemId, unitId) = await CreateTestItemAsync();
        var effectiveDate = new DateTimeOffset(2026, 9, 23, 0, 0, 0, TimeSpan.Zero);

        // 1. Create, submit, approve, and publish Record 1
        var createReq1 = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs", token: "token-maker", membershipId: MembershipMakerId);
        createReq1.Content = JsonContent.Create(new CreateCostRecordRequest
        {
            Scope = "organization",
            UnitId = unitId,
            Currency = "THB",
            Amount = 100.00m,
            MinimumQuantity = 10,
            EffectiveFromUtc = effectiveDate
        });
        var createRes1 = await _client.SendAsync(createReq1);
        var cost1 = await createRes1.Content.ReadFromJsonAsync<CostRecordResponse>();

        var submitReq1 = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs/{cost1!.Id}/submit", token: "token-maker", membershipId: MembershipMakerId);
        await _client.SendAsync(submitReq1);

        var approveReq1 = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs/{cost1.Id}/approve", token: "token-checker", membershipId: MembershipCheckerId);
        await _client.SendAsync(approveReq1);

        var publishReq1 = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs/{cost1.Id}/publish", token: "token-checker", membershipId: MembershipCheckerId);
        var publishRes1 = await _client.SendAsync(publishReq1);
        Assert.Equal(HttpStatusCode.OK, publishRes1.StatusCode);

        // 2. Create, submit, approve Record 2 with exact same effective date & min quantity
        var createReq2 = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs", token: "token-maker", membershipId: MembershipMakerId);
        createReq2.Content = JsonContent.Create(new CreateCostRecordRequest
        {
            Scope = "organization",
            UnitId = unitId,
            Currency = "THB",
            Amount = 120.00m,
            MinimumQuantity = 10,
            EffectiveFromUtc = effectiveDate
        });
        var createRes2 = await _client.SendAsync(createReq2);
        var cost2 = await createRes2.Content.ReadFromJsonAsync<CostRecordResponse>();

        var submitReq2 = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs/{cost2!.Id}/submit", token: "token-maker", membershipId: MembershipMakerId);
        await _client.SendAsync(submitReq2);

        var approveReq2 = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs/{cost2.Id}/approve", token: "token-checker", membershipId: MembershipCheckerId);
        await _client.SendAsync(approveReq2);

        // 3. Attempt to publish Record 2 -> Conflict 409 COST_RECORD_DATE_OVERLAP
        var publishReq2 = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs/{cost2.Id}/publish", token: "token-checker", membershipId: MembershipCheckerId);
        var publishRes2 = await _client.SendAsync(publishReq2);
        Assert.Equal(HttpStatusCode.Conflict, publishRes2.StatusCode);
    }
}
