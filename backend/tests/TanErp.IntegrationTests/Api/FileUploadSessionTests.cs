using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TanErp.Api;
using TanErp.Api.Contracts.Crm.Opportunities;
using TanErp.Api.Contracts.Files;
using TanErp.Api.ErrorHandling;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Files;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using TanErp.IntegrationTests.Support;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class FileUploadSessionTests : IAsyncLifetime
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

    private const string UidNoPerm = "uid-no-perm-file-test";
    private static readonly Guid MembershipNoPermId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5c01");

    private static readonly Guid OrgBId = TestOnlyDataSeeder.TestOrgBId;
    private static readonly Guid BranchBId = TestOnlyDataSeeder.TestBranchBId;
    private static readonly Guid MembershipBId = TestOnlyDataSeeder.TestMembershipBId;
    private static readonly Guid UserBId = TestOnlyDataSeeder.TestUserIdB;
    private const string UidB = TestOnlyDataSeeder.TestFirebaseUidB;

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
        {
            var uid = idToken switch
            {
                "token-org-a" => UidA,
                "token-org-b" => UidB,
                "token-no-perm" => UidNoPerm,
                _ => null
            };
            return Task.FromResult(uid);
        }
    }

    private readonly InMemoryLoggerProvider _loggerProvider = new();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureLogging(logging => logging.AddProvider(_loggerProvider));
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Database"] = _postgres.GetConnectionString(),
                    ["Storage:BasePath"] = Path.Combine(Path.GetTempPath(), $"tan-erp-test-{Guid.NewGuid():N}")
                });
            });
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(_postgres.GetConnectionString()));

                var authDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFirebaseTokenVerifier));
                if (authDescriptor != null) services.Remove(authDescriptor);
                services.AddSingleton<IFirebaseTokenVerifier, TestFirebaseTokenVerifier>();
            });
        });

        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true);

        // Seed no-perm user
        var userNoPerm = new TanErp.Domain.IdentityAccess.User(Guid.NewGuid(), UidNoPerm, "No Perm", "noperm-file@example.test", true);
        db.Users.Add(userNoPerm);
        db.Memberships.Add(new TanErp.Domain.Organization.Membership(MembershipNoPermId, OrgAId, BranchAId, userNoPerm.Id, true));

        await db.SaveChangesAsync();
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
        string? token,
        Guid? membershipId,
        string? idempotencyKey = null,
        Guid? ifMatch = null)
    {
        var msg = new HttpRequestMessage(method, url);
        if (token != null)
        {
            msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        if (membershipId.HasValue)
        {
            msg.Headers.Add("X-Membership-Id", membershipId.Value.ToString());
        }
        if (idempotencyKey != null)
        {
            msg.Headers.Add("Idempotency-Key", idempotencyKey);
        }
        if (ifMatch.HasValue)
        {
            msg.Headers.Add("If-Match", $"\"{ifMatch.Value:D}\"");
        }
        return msg;
    }

    private async Task<Guid> SetupOpportunityAsync()
    {
        var (id, _) = await SetupOpportunityDetailedAsync();
        return id;
    }

    private async Task<(Guid Id, Guid RowVersion)> SetupOpportunityDetailedAsync(
        Guid? orgId = null,
        Guid? branchId = null,
        Guid? userId = null)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var targetOrgId = orgId ?? OrgAId;
        var targetBranchId = branchId ?? BranchAId;
        var targetUserId = userId ?? UserAId;

        var now = DateTimeOffset.UtcNow;
        var customer = Customer.CreateDraft(
            Guid.NewGuid(), targetOrgId, targetUserId, CustomerType.Person, "ลูกค้าทดสอบ", null, "th",
            new PrimaryContactInput("คุณทดสอบ", null, "0812345678", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        db.Customers.Add(customer);

        var address = new SiteAddressInput("123 ถนนสุขุมวิท", "คลองเตย", "คลองเตย", "กรุงเทพมหานคร", "10110", "TH");
        var site = Site.CreateActive(Guid.NewGuid(), targetOrgId, customer.Id, targetUserId, "บ้านพักอาศัย", address, 13.7m, 100.5m, null, now);
        db.Sites.Add(site);

        var opp = Opportunity.CreateDraft(
            Guid.NewGuid(), targetOrgId, targetBranchId, customer.Id, site.Id, targetUserId, targetUserId,
            "งานบิลท์อินห้องนอน", "ขอบเขตงานตู้เสื้อผ้าและเตียง", new[] { "built-in" }, null, 350000m, "THB",
            new DateOnly(2026, 12, 31), now.AddDays(2), "นัดเข้าวัดพื้นที่", now);
        opp.Qualify(opp.RowVersion);
        db.Opportunities.Add(opp);

        await db.SaveChangesAsync();
        return (opp.Id, opp.RowVersion);
    }

    private async Task<Guid> UploadFileForOpportunityAsync(
        Guid oppId,
        string token,
        Guid membershipId,
        string filename,
        byte[] content)
    {
        var createReq = new CreateUploadSessionRequest(
            "opportunity",
            oppId,
            null,
            [new FileSlotRequest(filename, "image/webp", content.Length)]);

        var createMsg = CreateRequest(HttpMethod.Post, "/api/v1/files/upload-sessions", token, membershipId, $"upload-slot-key-{Guid.NewGuid():N}");
        createMsg.Content = JsonContent.Create(createReq);
        var createRes = await _client.SendAsync(createMsg);
        Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);
        var session = await createRes.Content.ReadFromJsonAsync<CreateUploadSessionResponse>();
        Assert.NotNull(session);

        using var form = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/webp");
        form.Add(fileContent, session.Slots[0].SlotId.ToString(), filename);

        var compMsg = CreateRequest(HttpMethod.Post, $"/api/v1/files/upload-sessions/{session.SessionId}/complete", token, membershipId);
        compMsg.Content = form;
        var compRes = await _client.SendAsync(compMsg);
        Assert.Equal(HttpStatusCode.OK, compRes.StatusCode);
        var compData = await compRes.Content.ReadFromJsonAsync<CompleteUploadSessionResponse>();
        Assert.NotNull(compData);
        return compData.Files[0].FileId;
    }

    // ==========================================
    // Task 1: Tests 1–2
    // ==========================================

    [Fact]
    public async Task CreateSession_ReplaySameIntent_ReturnsSamePersistedSlots()
    {
        var oppId = await SetupOpportunityAsync();
        var key = $"replay-key-{Guid.NewGuid():N}";
        var body = new CreateUploadSessionRequest(
            "opportunity",
            oppId,
            null,
            [
                new FileSlotRequest("photo1.webp", "image/webp", 250_000),
                new FileSlotRequest("photo2.jpeg", "image/jpeg", 450_000)
            ]);

        // First call
        var msg1 = CreateRequest(HttpMethod.Post, "/api/v1/files/upload-sessions", "token-org-a", MembershipAId, key);
        msg1.Content = JsonContent.Create(body);
        var res1 = await _client.SendAsync(msg1);

        Assert.Equal(HttpStatusCode.Created, res1.StatusCode);
        var resp1 = await res1.Content.ReadFromJsonAsync<CreateUploadSessionResponse>();
        Assert.NotNull(resp1);
        Assert.Equal(2, resp1.Slots.Count);

        // Replay call with exact same key and payload
        var msg2 = CreateRequest(HttpMethod.Post, "/api/v1/files/upload-sessions", "token-org-a", MembershipAId, key);
        msg2.Content = JsonContent.Create(body);
        var res2 = await _client.SendAsync(msg2);

        Assert.Equal(HttpStatusCode.Created, res2.StatusCode);
        var resp2 = await res2.Content.ReadFromJsonAsync<CreateUploadSessionResponse>();
        Assert.NotNull(resp2);

        // Must return identical session ID, slots, and expiry
        Assert.Equal(resp1.SessionId, resp2.SessionId);
        Assert.Equal(resp1.ExpiresAtUtc, resp2.ExpiresAtUtc);
        Assert.Equal(resp1.Slots.Count, resp2.Slots.Count);
        for (var i = 0; i < resp1.Slots.Count; i++)
        {
            Assert.Equal(resp1.Slots[i].SlotId, resp2.Slots[i].SlotId);
            Assert.Equal(resp1.Slots[i].Filename, resp2.Slots[i].Filename);
            Assert.Equal(resp1.Slots[i].MediaType, resp2.Slots[i].MediaType);
            Assert.Equal(resp1.Slots[i].FileSizeBytes, resp2.Slots[i].FileSizeBytes);
        }
    }

    [Theory]
    [InlineData("invented-session")]
    [InlineData("mismatched-metadata")]
    public async Task CompleteSession_WithoutMatchingPersistedSlots_ReturnsConflict(string scenario)
    {
        var oppId = await SetupOpportunityAsync();

        if (scenario == "invented-session")
        {
            // Invented / non-existent session ID
            var inventedSessionId = Guid.NewGuid();
            var webpBytes = CreateMinimalWebP();

            using var form = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(webpBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/webp");
            form.Add(fileContent, Guid.NewGuid().ToString(), "photo.webp");

            var msg = CreateRequest(HttpMethod.Post, $"/api/v1/files/upload-sessions/{inventedSessionId}/complete", "token-org-a", MembershipAId);
            msg.Content = form;

            var res = await _client.SendAsync(msg);
            Assert.Equal(HttpStatusCode.Conflict, res.StatusCode);

            var problem = await res.Content.ReadFromJsonAsync<ApiProblemDetails>();
            Assert.Equal("FILE_UPLOAD_SESSION_INVALID", problem?.Code);
        }
        else
        {
            // Create a valid session first
            var createReq = new CreateUploadSessionRequest(
                "opportunity",
                oppId,
                null,
                [new FileSlotRequest("valid.webp", "image/webp", 1024)]);

            var createMsg = CreateRequest(HttpMethod.Post, "/api/v1/files/upload-sessions", "token-org-a", MembershipAId, $"mismatch-key-{Guid.NewGuid():N}");
            createMsg.Content = JsonContent.Create(createReq);
            var createRes = await _client.SendAsync(createMsg);
            Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);
            var session = await createRes.Content.ReadFromJsonAsync<CreateUploadSessionResponse>();
            Assert.NotNull(session);

            // Submit with mismatched filename (declared "valid.webp", sending "different.webp")
            var webpBytes = CreateMinimalWebP();
            using var form = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(webpBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/webp");
            form.Add(fileContent, session.Slots[0].SlotId.ToString(), "different.webp");

            var msg = CreateRequest(HttpMethod.Post, $"/api/v1/files/upload-sessions/{session.SessionId}/complete", "token-org-a", MembershipAId);
            msg.Content = form;

            var res = await _client.SendAsync(msg);
            Assert.Equal(HttpStatusCode.Conflict, res.StatusCode);

            var problem = await res.Content.ReadFromJsonAsync<ApiProblemDetails>();
            Assert.Equal("FILE_UPLOAD_SESSION_INVALID", problem?.Code);
        }
    }

    // ==========================================
    // Task 2: Tests 3–4
    // ==========================================

    [Fact]
    public async Task GetFileContent_AnonymousOrWrongOrganization_DoesNotDiscloseFile()
    {
        var oppId = await SetupOpportunityAsync();
        var webpBytes = CreateMinimalWebP();

        // 1. Create session and complete a valid file upload
        var createReq = new CreateUploadSessionRequest(
            "opportunity",
            oppId,
            null,
            [new FileSlotRequest("photo.webp", "image/webp", webpBytes.Length)]);

        var createMsg = CreateRequest(HttpMethod.Post, "/api/v1/files/upload-sessions", "token-org-a", MembershipAId, $"auth-check-key-{Guid.NewGuid():N}");
        createMsg.Content = JsonContent.Create(createReq);
        var createRes = await _client.SendAsync(createMsg);
        var session = await createRes.Content.ReadFromJsonAsync<CreateUploadSessionResponse>();
        Assert.NotNull(session);

        using var form = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(webpBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/webp");
        form.Add(fileContent, session.Slots[0].SlotId.ToString(), "photo.webp");

        var compMsg = CreateRequest(HttpMethod.Post, $"/api/v1/files/upload-sessions/{session.SessionId}/complete", "token-org-a", MembershipAId);
        compMsg.Content = form;
        var compRes = await _client.SendAsync(compMsg);
        Assert.Equal(HttpStatusCode.OK, compRes.StatusCode);
        var compData = await compRes.Content.ReadFromJsonAsync<CompleteUploadSessionResponse>();
        Assert.NotNull(compData);
        var fileId = compData.Files[0].FileId;

        // 2. Anonymous request -> 401 Unauthorized
        var anonMsg = CreateRequest(HttpMethod.Get, $"/api/v1/files/{fileId}/content", null, null);
        var anonRes = await _client.SendAsync(anonMsg);
        Assert.Equal(HttpStatusCode.Unauthorized, anonRes.StatusCode);

        // 3. User from Org B -> 404 Not Found (does not disclose existence)
        var orgBMsg = CreateRequest(HttpMethod.Get, $"/api/v1/files/{fileId}/content", "token-org-b", MembershipBId);
        var orgBRes = await _client.SendAsync(orgBMsg);
        Assert.Equal(HttpStatusCode.NotFound, orgBRes.StatusCode);
        var problem = await orgBRes.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("RESOURCE_NOT_FOUND", problem?.Code);
    }

    [Fact]
    public async Task GetFileContent_AuthorizedParentReader_StreamsVerifiedImage()
    {
        var oppId = await SetupOpportunityAsync();
        var webpBytes = CreateMinimalWebP();

        // 1. Create and complete upload
        var createReq = new CreateUploadSessionRequest(
            "opportunity",
            oppId,
            null,
            [new FileSlotRequest("site-shot.webp", "image/webp", webpBytes.Length)]);

        var createMsg = CreateRequest(HttpMethod.Post, "/api/v1/files/upload-sessions", "token-org-a", MembershipAId, $"stream-key-{Guid.NewGuid():N}");
        createMsg.Content = JsonContent.Create(createReq);
        var createRes = await _client.SendAsync(createMsg);
        var session = await createRes.Content.ReadFromJsonAsync<CreateUploadSessionResponse>();
        Assert.NotNull(session);

        using var form = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(webpBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/webp");
        form.Add(fileContent, session.Slots[0].SlotId.ToString(), "site-shot.webp");

        var compMsg = CreateRequest(HttpMethod.Post, $"/api/v1/files/upload-sessions/{session.SessionId}/complete", "token-org-a", MembershipAId);
        compMsg.Content = form;
        var compRes = await _client.SendAsync(compMsg);
        var compData = await compRes.Content.ReadFromJsonAsync<CompleteUploadSessionResponse>();
        Assert.NotNull(compData);
        var fileId = compData.Files[0].FileId;

        // 2. Authorized read request
        var readMsg = CreateRequest(HttpMethod.Get, $"/api/v1/files/{fileId}/content", "token-org-a", MembershipAId);
        var readRes = await _client.SendAsync(readMsg);

        Assert.Equal(HttpStatusCode.OK, readRes.StatusCode);
        Assert.Equal("image/webp", readRes.Content.Headers.ContentType?.MediaType);
        var cacheControlStr = readRes.Headers.CacheControl?.ToString() ?? string.Empty;
        Assert.Contains("private", cacheControlStr);
        Assert.Contains("no-store", cacheControlStr);

        var fetchedBytes = await readRes.Content.ReadAsByteArrayAsync();
        Assert.Equal(webpBytes, fetchedBytes);
    }

    // ==========================================
    // Task 3: Test 5
    // ==========================================

    [Fact]
    public async Task BindFile_WrongOrganizationWrongParentOrUnverified_IsRejected()
    {
        var (opp1Id, opp1Version) = await SetupOpportunityDetailedAsync();
        var (opp2Id, opp2Version) = await SetupOpportunityDetailedAsync();
        var (oppBId, oppBVersion) = await SetupOpportunityDetailedAsync(OrgBId, BranchBId, UserBId);

        var webpBytes = CreateMinimalWebP();

        // 1. Upload valid file F1 for Opp1
        var f1Id = await UploadFileForOpportunityAsync(opp1Id, "token-org-a", MembershipAId, "f1.webp", webpBytes);

        // 2. Upload valid file F2 for Opp2
        var f2Id = await UploadFileForOpportunityAsync(opp2Id, "token-org-a", MembershipAId, "f2.webp", webpBytes);

        // 3. Upload valid file F3 for OppB in Org B
        var f3Id = await UploadFileForOpportunityAsync(oppBId, "token-org-b", MembershipBId, "f3.webp", webpBytes);

        // 4. Case A: Try to attach F2 (uploaded for Opp2) to Opp1 -> Rejected (wrong parent ID)
        var attachF2Msg = CreateRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{opp1Id}/work-images",
            "token-org-a",
            MembershipAId,
            $"bind-f2-{Guid.NewGuid():N}",
            opp1Version);
        attachF2Msg.Content = JsonContent.Create(new AttachWorkImagesRequest([new AttachWorkImageItemRequest(f2Id, "F2 on Opp1")]));
        var attachF2Res = await _client.SendAsync(attachF2Msg);

        Assert.Equal(HttpStatusCode.Conflict, attachF2Res.StatusCode);
        var p2 = await attachF2Res.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("OPPORTUNITY_IMAGE_NOT_READY", p2?.Code);

        // Case B: Try to attach F3 (uploaded for Org B) to Opp1 -> Rejected (wrong organization)
        var attachF3Msg = CreateRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{opp1Id}/work-images",
            "token-org-a",
            MembershipAId,
            $"bind-f3-{Guid.NewGuid():N}",
            opp1Version);
        attachF3Msg.Content = JsonContent.Create(new AttachWorkImagesRequest([new AttachWorkImageItemRequest(f3Id, "F3 on Opp1")]));
        var attachF3Res = await _client.SendAsync(attachF3Msg);

        Assert.Equal(HttpStatusCode.Conflict, attachF3Res.StatusCode);
        var p3 = await attachF3Res.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("OPPORTUNITY_IMAGE_NOT_READY", p3?.Code);

        // Case C: Try to attach non-existent/unverified file -> Rejected
        var unverifiedId = Guid.NewGuid();
        var attachUnverifiedMsg = CreateRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{opp1Id}/work-images",
            "token-org-a",
            MembershipAId,
            $"bind-unverified-{Guid.NewGuid():N}",
            opp1Version);
        attachUnverifiedMsg.Content = JsonContent.Create(new AttachWorkImagesRequest([new AttachWorkImageItemRequest(unverifiedId, "Unverified")]));
        var attachUnverifiedRes = await _client.SendAsync(attachUnverifiedMsg);

        Assert.Equal(HttpStatusCode.Conflict, attachUnverifiedRes.StatusCode);
        var pUnverified = await attachUnverifiedRes.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("OPPORTUNITY_IMAGE_NOT_READY", pUnverified?.Code);

        // Case D: Valid attach F1 to Opp1 succeeds
        var attachF1Msg = CreateRequest(
            HttpMethod.Post,
            $"/api/v1/opportunities/{opp1Id}/work-images",
            "token-org-a",
            MembershipAId,
            $"bind-f1-{Guid.NewGuid():N}",
            opp1Version);
        attachF1Msg.Content = JsonContent.Create(new AttachWorkImagesRequest([new AttachWorkImageItemRequest(f1Id, "F1 on Opp1")]));
        var attachF1Res = await _client.SendAsync(attachF1Msg);

        Assert.Equal(HttpStatusCode.Created, attachF1Res.StatusCode);
        var attachedData = await attachF1Res.Content.ReadFromJsonAsync<AttachWorkImagesResponse>();
        Assert.NotNull(attachedData);
        Assert.Single(attachedData.Items);
        Assert.Equal(f1Id, attachedData.Items[0].FileId);

        // Verify that only F1 is attached in DB for Opp1
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var attachedImages = await db.OpportunityWorkImages
            .Where(w => w.OpportunityId == opp1Id && !w.IsDeleted)
            .ToListAsync();
        Assert.Single(attachedImages);
        Assert.Equal(f1Id, attachedImages[0].FileId);
    }

    // ==========================================
    // Existing Permission and Validation Tests
    // ==========================================

    [Fact]
    public async Task CreateSession_MissingPermissions_Returns403()
    {
        var oppId = await SetupOpportunityAsync();
        var body = new CreateUploadSessionRequest(
            "opportunity",
            oppId,
            null,
            [
                new FileSlotRequest("photo.webp", "image/webp", 512 * 1024)
            ]);

        var msg = CreateRequest(HttpMethod.Post, "/api/v1/files/upload-sessions", "token-no-perm", MembershipNoPermId, $"file-key-noperm-{Guid.NewGuid():N}");
        msg.Content = JsonContent.Create(body);

        var res = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        var problem = await res.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("PERMISSION_DENIED", problem?.Code);
    }

    [Fact]
    public async Task CreateSession_NoFiles_Returns400Or422()
    {
        var oppId = await SetupOpportunityAsync();
        var body = new CreateUploadSessionRequest("opportunity", oppId, null, []);
        var msg = CreateRequest(HttpMethod.Post, "/api/v1/files/upload-sessions", "token-org-a", MembershipAId, $"file-key-empty-{Guid.NewGuid():N}");
        msg.Content = JsonContent.Create(body);

        var res = await _client.SendAsync(msg);

        Assert.True(
            res.StatusCode == HttpStatusCode.Conflict ||
            res.StatusCode == HttpStatusCode.UnprocessableEntity ||
            res.StatusCode == HttpStatusCode.BadRequest,
            $"Expected 400, 409, or 422, got {res.StatusCode}");
    }

    [Fact]
    public async Task CreateSession_InvalidMediaType_Returns422()
    {
        var oppId = await SetupOpportunityAsync();
        var body = new CreateUploadSessionRequest(
            "opportunity",
            oppId,
            null,
            [
                new FileSlotRequest("document.pdf", "application/pdf", 512 * 1024)
            ]);
        var msg = CreateRequest(HttpMethod.Post, "/api/v1/files/upload-sessions", "token-org-a", MembershipAId, $"file-key-invalid-mime-{Guid.NewGuid():N}");
        msg.Content = JsonContent.Create(body);

        var res = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.Conflict, res.StatusCode);
        var problem = await res.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("FILE_UPLOAD_SESSION_INVALID", problem?.Code);
    }

    [Fact]
    public async Task CreateSession_InvalidParentType_Returns422()
    {
        var body = new CreateUploadSessionRequest(
            "unknown_parent",
            Guid.NewGuid(),
            null,
            [
                new FileSlotRequest("photo.webp", "image/webp", 512 * 1024)
            ]);
        var msg = CreateRequest(HttpMethod.Post, "/api/v1/files/upload-sessions", "token-org-a", MembershipAId, $"file-key-invalid-parent-{Guid.NewGuid():N}");
        msg.Content = JsonContent.Create(body);

        var res = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, res.StatusCode);
        var problem = await res.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("FILE_PARENT_TYPE_INVALID", problem?.Code);
    }

    // ==========================================
    // Helpers
    // ==========================================

    private static byte[] CreateMinimalWebP()
    {
        var riff = new byte[] { 0x52, 0x49, 0x46, 0x46 }; // "RIFF"
        var size = BitConverter.GetBytes(12); // size placeholder
        var webp = new byte[] { 0x57, 0x45, 0x42, 0x50 }; // "WEBP"
        var vp8l = new byte[] { 0x56, 0x50, 0x38, 0x4C, 0x04, 0x00, 0x00, 0x00, 0x2F, 0x00, 0x00, 0x00 };

        return [.. riff, .. size, .. webp, .. vp8l];
    }
}
