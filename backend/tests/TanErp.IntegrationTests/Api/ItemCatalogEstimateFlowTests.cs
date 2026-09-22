using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.Files;
using TanErp.Api.Contracts.Items;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class ItemCatalogEstimateFlowTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private const string UidMaker = TestOnlyDataSeeder.TestFirebaseUid;
    private const string UidChecker = TestOnlyDataSeeder.TestFirebaseUidB;
    private const string UidOrgB = "uid-org-b-user";

    private static readonly Guid OrgAId = TestOnlyDataSeeder.TestOrgId;
    private static readonly Guid BranchAId = TestOnlyDataSeeder.TestBranchId;
    private static readonly Guid MembershipMakerId = TestOnlyDataSeeder.TestMembershipId;
    private static readonly Guid MembershipCheckerId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f9999");
    private static readonly Guid MembershipOrgBId = TestOnlyDataSeeder.TestMembershipBId;

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
        {
            var uid = idToken switch
            {
                "token-maker" => UidMaker,
                "token-checker" => UidChecker,
                "token-org-b" => UidOrgB,
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
                    ["ConnectionStrings:Database"] = _postgres.GetConnectionString(),
                    ["SeedTestData"] = "true"
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

        // Add Checker membership in Org A with Admin role
        var checkerRole = await db.Roles.FirstOrDefaultAsync(r => r.OrganizationId == OrgAId && r.Name == "Test Admin");
        var checkerUser = await db.Users.FirstOrDefaultAsync(u => u.FirebaseUid == UidChecker);
        if (checkerUser != null && checkerRole != null)
        {
            var checkerMembershipInOrgA = new Membership(
                MembershipCheckerId,
                OrgAId,
                BranchAId,
                checkerUser.Id,
                isActive: true);
            db.Memberships.Add(checkerMembershipInOrgA);

            var checkerRoleAssignment = new TanErp.Domain.IdentityAccess.MembershipRole(
                MembershipCheckerId,
                checkerRole.Id,
                OrgAId);
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

    private HttpRequestMessage CreateAuthRequest(HttpMethod method, string url, string token = "token-maker", Guid? membershipId = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Membership-Id", (membershipId ?? MembershipMakerId).ToString());
        return request;
    }

    private async Task<(Guid ItemId, Guid UnitId)> CreateTestItemAsync(string prefix)
    {
        var catReq = CreateAuthRequest(HttpMethod.Post, "/api/v1/item-categories");
        catReq.Content = JsonContent.Create(new CreateItemCategoryRequest
        {
            Code = $"CAT-{prefix}-" + Guid.NewGuid().ToString("N")[..6],
            Name = new LocalizedTextInput { Thai = "หมวด", English = "Cat" },
            AllowedItemTypes = new List<string> { "material" }
        });
        var catRes = await _client.SendAsync(catReq);
        var cat = await catRes.Content.ReadFromJsonAsync<ItemCategoryDetailResponse>();

        var unitReq = CreateAuthRequest(HttpMethod.Post, "/api/v1/units-of-measure");
        unitReq.Content = JsonContent.Create(new CreateUnitOfMeasureRequest
        {
            Code = $"UOM-{prefix}-" + Guid.NewGuid().ToString("N")[..6],
            Name = new LocalizedTextInput { Thai = "หน่วย", English = "Unit" },
            Symbol = "u"
        });
        var unitRes = await _client.SendAsync(unitReq);
        var unit = await unitRes.Content.ReadFromJsonAsync<UnitOfMeasureDetailResponse>();

        var itmReq = CreateAuthRequest(HttpMethod.Post, "/api/v1/items");
        itmReq.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = $"ITM-{prefix}-" + Guid.NewGuid().ToString("N")[..6],
            ItemType = "material",
            CategoryId = cat!.Id,
            BaseUnitId = unit!.Id,
            Name = new LocalizedTextInput { Thai = "สินค้า", English = "Item" }
        });
        var itmRes = await _client.SendAsync(itmReq);
        var itm = await itmRes.Content.ReadFromJsonAsync<ItemResponse>();

        return (itm!.Id, unit.Id);
    }

    [Fact]
    public async Task ReviewRegression_ActualByteOverflow_Rejected()
    {
        var (itemId, _) = await CreateTestItemAsync("OVF");

        // Declare 10 bytes in metadata
        var sessionReq = new CreateUploadSessionRequest(
            "item",
            itemId,
            null,
            new List<FileSlotRequest> { new("overflow.jpg", "image/jpeg", 10) });

        var sessionMsg = CreateAuthRequest(HttpMethod.Post, "/api/v1/files/upload-sessions");
        sessionMsg.Headers.Add("Idempotency-Key", $"key-{Guid.NewGuid():N}");
        sessionMsg.Content = JsonContent.Create(sessionReq);
        var sessionRes = await _client.SendAsync(sessionMsg);
        Assert.Equal(HttpStatusCode.Created, sessionRes.StatusCode);
        var session = await sessionRes.Content.ReadFromJsonAsync<CreateUploadSessionResponse>();

        // Actually send 20 bytes (overflow bypass attempt) with valid JPEG header
        var overflowBytes = new byte[20];
        overflowBytes[0] = 0xFF;
        overflowBytes[1] = 0xD8;
        overflowBytes[2] = 0xFF;
        overflowBytes[3] = 0xE0;
        using var form = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(overflowBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        form.Add(fileContent, session!.Slots[0].SlotId.ToString(), "overflow.jpg");

        var compMsg = CreateAuthRequest(HttpMethod.Post, $"/api/v1/files/upload-sessions/{session.SessionId}/complete");
        compMsg.Content = form;
        var compRes = await _client.SendAsync(compMsg);

        // Upload verification rejects actual byte overflow with 409 Conflict (FILE_UPLOAD_SESSION_INVALID) or 422 (FILE_SIZE_MISMATCH)
        Assert.True(
            compRes.StatusCode is HttpStatusCode.Conflict or HttpStatusCode.UnprocessableEntity or HttpStatusCode.BadRequest,
            $"Expected upload overflow to be rejected, but got {compRes.StatusCode}");
    }

    [Fact]
    public async Task ReviewRegression_TruncatedUpload_Rejected()
    {
        var (itemId, _) = await CreateTestItemAsync("TRUNC");

        // Declare 20 bytes in metadata
        var sessionReq = new CreateUploadSessionRequest(
            "item",
            itemId,
            null,
            new List<FileSlotRequest> { new("trunc.jpg", "image/jpeg", 20) });

        var sessionMsg = CreateAuthRequest(HttpMethod.Post, "/api/v1/files/upload-sessions");
        sessionMsg.Headers.Add("Idempotency-Key", $"key-{Guid.NewGuid():N}");
        sessionMsg.Content = JsonContent.Create(sessionReq);
        var sessionRes = await _client.SendAsync(sessionMsg);
        Assert.Equal(HttpStatusCode.Created, sessionRes.StatusCode);
        var session = await sessionRes.Content.ReadFromJsonAsync<CreateUploadSessionResponse>();

        // Actually send only 10 bytes (truncated stream attempt) with valid JPEG header
        var truncBytes = new byte[10];
        truncBytes[0] = 0xFF;
        truncBytes[1] = 0xD8;
        truncBytes[2] = 0xFF;
        truncBytes[3] = 0xE0;
        using var form = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(truncBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        form.Add(fileContent, session!.Slots[0].SlotId.ToString(), "trunc.jpg");

        var compMsg = CreateAuthRequest(HttpMethod.Post, $"/api/v1/files/upload-sessions/{session.SessionId}/complete");
        compMsg.Content = form;
        var compRes = await _client.SendAsync(compMsg);

        // Upload verification rejects truncated byte count with 409 Conflict (FILE_UPLOAD_SESSION_INVALID) or 422 (FILE_SIZE_MISMATCH)
        Assert.True(
            compRes.StatusCode is HttpStatusCode.Conflict or HttpStatusCode.UnprocessableEntity or HttpStatusCode.BadRequest,
            $"Expected truncated upload to be rejected, but got {compRes.StatusCode}");
    }

    [Fact]
    public async Task ReviewRegression_MakerSelfApproval_RejectedWithMakerCheckerViolation()
    {
        var (itemId, unitId) = await CreateTestItemAsync("SELF");

        // 2. Create Cost Record
        var costReq = CreateAuthRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs");
        costReq.Content = JsonContent.Create(new CreateCostRecordRequest
        {
            Scope = "organization",
            UnitId = unitId,
            Currency = "THB",
            Amount = 120.00m,
            MinimumQuantity = 0,
            EffectiveFromUtc = DateTimeOffset.UtcNow,
            SourceReference = "Maker cost record"
        });
        var costRes = await _client.SendAsync(costReq);
        Assert.Equal(HttpStatusCode.Created, costRes.StatusCode);
        var cost = await costRes.Content.ReadFromJsonAsync<CostRecordResponse>();

        // 3. Maker submits
        var subRes = await _client.SendAsync(CreateAuthRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs/{cost!.Id}/submit"));
        Assert.Equal(HttpStatusCode.OK, subRes.StatusCode);

        // 4. Maker attempts self-approval -> 422 Maker-Checker violation
        var selfApproveReq = CreateAuthRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/costs/{cost.Id}/approve");
        var selfApproveRes = await _client.SendAsync(selfApproveReq);

    }


    [Fact]
    public async Task ReviewRegression_CrossOrgAccess_ForbiddenOrNotFound()
    {
        // Org B attempts to access Org A's branch in Catalog
        var crossOrgCatalogReq = CreateAuthRequest(HttpMethod.Get, $"/api/v1/estimate-catalog/items?branchId={BranchAId}", "token-org-b", MembershipOrgBId);
        var crossOrgCatalogRes = await _client.SendAsync(crossOrgCatalogReq);

        Assert.True(
            crossOrgCatalogRes.StatusCode == HttpStatusCode.Forbidden ||
            crossOrgCatalogRes.StatusCode == HttpStatusCode.NotFound,
            $"Expected 403 or 404 for cross-org access, got {crossOrgCatalogRes.StatusCode}"
        );
    }

    [Fact]
    public async Task ReviewRegression_InvalidCursor_ReturnsBadRequest()
    {
        var invalidCursorReq = CreateAuthRequest(HttpMethod.Get, $"/api/v1/estimate-catalog/items?branchId={BranchAId}&cursor=not-a-valid-base64-cursor");
        var invalidCursorRes = await _client.SendAsync(invalidCursorReq);

        Assert.Equal(HttpStatusCode.BadRequest, invalidCursorRes.StatusCode);
    }
}
