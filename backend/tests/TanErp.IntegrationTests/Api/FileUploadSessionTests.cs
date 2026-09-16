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
using TanErp.Api.Contracts.Files;
using TanErp.Api.ErrorHandling;
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
    private static readonly Guid MembershipAId = TestOnlyDataSeeder.TestMembershipId;
    private const string UidA = TestOnlyDataSeeder.TestFirebaseUid;

    private const string UidNoPerm = "uid-no-perm-file-test";
    private static readonly Guid MembershipNoPermId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5c01");

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
        {
            var uid = idToken switch
            {
                "token-org-a" => UidA,
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
        db.Memberships.Add(new TanErp.Domain.Organization.Membership(MembershipNoPermId, OrgAId, TestOnlyDataSeeder.TestBranchId, userNoPerm.Id, true));
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
        string token,
        Guid membershipId,
        string? idempotencyKey = null)
    {
        var msg = new HttpRequestMessage(method, url);
        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        msg.Headers.Add("X-Membership-Id", membershipId.ToString());
        if (idempotencyKey != null)
        {
            msg.Headers.Add("Idempotency-Key", idempotencyKey);
        }
        return msg;
    }

    // ==========================================
    // Test 1: Create Upload Session
    // ==========================================

    [Fact]
    public async Task CreateSession_MissingPermissions_Returns403()
    {
        var body = new CreateUploadSessionRequest(
        [
            new FileSlotRequest("photo.webp", "image/webp", 512 * 1024)
        ]);

        var msg = CreateRequest(HttpMethod.Post, "/api/v1/files/sessions", "token-no-perm", MembershipNoPermId, $"file-key-noperm-{Guid.NewGuid():N}");
        msg.Content = JsonContent.Create(body);

        var res = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        var problem = await res.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("PERMISSION_DENIED", problem?.Code);
    }

    [Fact]
    public async Task CreateSession_NoFiles_Returns400()
    {
        var body = new CreateUploadSessionRequest([]);
        var msg = CreateRequest(HttpMethod.Post, "/api/v1/files/sessions", "token-org-a", MembershipAId, $"file-key-empty-{Guid.NewGuid():N}");
        msg.Content = JsonContent.Create(body);

        var res = await _client.SendAsync(msg);

        // 422 from handler validation or 400 from model validation
        Assert.True(
            res.StatusCode == HttpStatusCode.UnprocessableEntity ||
            res.StatusCode == HttpStatusCode.BadRequest,
            $"Expected 400 or 422, got {res.StatusCode}");
    }

    [Fact]
    public async Task CreateSession_InvalidMediaType_Returns422()
    {
        var body = new CreateUploadSessionRequest(
        [
            new FileSlotRequest("document.pdf", "application/pdf", 512 * 1024)
        ]);
        var msg = CreateRequest(HttpMethod.Post, "/api/v1/files/sessions", "token-org-a", MembershipAId, $"file-key-invalid-mime-{Guid.NewGuid():N}");
        msg.Content = JsonContent.Create(body);

        var res = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, res.StatusCode);
        var problem = await res.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("FILE_UPLOAD_SESSION_INVALID", problem?.Code);
    }

    [Fact]
    public async Task CreateSession_ValidRequest_Returns201WithSlots()
    {
        var body = new CreateUploadSessionRequest(
        [
            new FileSlotRequest("front.webp", "image/webp", 800 * 1024),
            new FileSlotRequest("side.jpg", "image/jpeg", 600 * 1024)
        ]);
        var sessionKey = $"file-session-valid-{Guid.NewGuid():N}";
        var msg = CreateRequest(HttpMethod.Post, "/api/v1/files/sessions", "token-org-a", MembershipAId, sessionKey);
        msg.Content = JsonContent.Create(body);

        var res = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        var response = await res.Content.ReadFromJsonAsync<CreateUploadSessionResponse>();
        Assert.NotNull(response);
        Assert.Equal(sessionKey, response.SessionId);
        Assert.Equal(2, response.Slots.Count);
        Assert.All(response.Slots, slot =>
        {
            Assert.NotEmpty(slot.SlotId);
            Assert.NotEmpty(slot.UploadUrl);
        });
    }

    // ==========================================
    // Test 2: Complete Session (multipart upload)
    // ==========================================

    [Fact]
    public async Task CompleteSession_ValidWebPFile_Returns200WithFileId()
    {
        var sessionId = $"complete-session-{Guid.NewGuid():N}";

        // Create a minimal valid WebP file (RIFF....WEBP header)
        var webpContent = CreateMinimalWebP();

        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(webpContent);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/webp");
        content.Add(fileContent, "file", "test-image.webp");

        var msg = CreateRequest(HttpMethod.Post, $"/api/v1/files/sessions/{sessionId}/complete", "token-org-a", MembershipAId);
        msg.Content = content;

        var res = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var response = await res.Content.ReadFromJsonAsync<CompleteUploadSessionResponse>();
        Assert.NotNull(response);
        Assert.Equal(sessionId, response.SessionId);
        Assert.Single(response.Files);
        Assert.NotEqual(Guid.Empty, response.Files[0].FileId);
        Assert.Equal("image/webp", response.Files[0].MediaType);
    }

    [Fact]
    public async Task CompleteSession_DisguisedExecutable_Returns422()
    {
        var sessionId = $"disguised-session-{Guid.NewGuid():N}";

        // ELF binary disguised as WebP
        var elfBytes = new byte[] { 0x7F, 0x45, 0x4C, 0x46, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(elfBytes);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/webp");
        content.Add(fileContent, "file", "fake.webp");

        var msg = CreateRequest(HttpMethod.Post, $"/api/v1/files/sessions/{sessionId}/complete", "token-org-a", MembershipAId);
        msg.Content = content;

        var res = await _client.SendAsync(msg);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, res.StatusCode);
        var problem = await res.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("FILE_UPLOAD_SESSION_INVALID", problem?.Code);
    }

    [Fact]
    public async Task CompleteSession_FileIsPersisted_CanBeFoundInDb()
    {
        var sessionId = $"persist-session-{Guid.NewGuid():N}";
        var webpContent = CreateMinimalWebP();

        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(webpContent);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/webp");
        content.Add(fileContent, "file", "persisted.webp");

        var msg = CreateRequest(HttpMethod.Post, $"/api/v1/files/sessions/{sessionId}/complete", "token-org-a", MembershipAId);
        msg.Content = content;

        var res = await _client.SendAsync(msg);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var response = await res.Content.ReadFromJsonAsync<CompleteUploadSessionResponse>();
        Assert.NotNull(response);

        var fileId = response.Files[0].FileId;

        // Verify persistence in DB
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dbFile = await db.UploadedFiles.FirstOrDefaultAsync(f => f.Id == fileId);

        Assert.NotNull(dbFile);
        Assert.Equal(OrgAId, dbFile.OrganizationId);
        Assert.Equal("image/webp", dbFile.MediaType);
        Assert.Equal(sessionId, dbFile.UploadSessionId);
        Assert.Equal("verified", dbFile.Status);
    }

    // ==========================================
    // Helpers
    // ==========================================

    /// <summary>
    /// Creates a minimal but structurally valid WebP binary (RIFF header + WEBP marker).
    /// Not a displayable image — only passes magic number check.
    /// </summary>
    private static byte[] CreateMinimalWebP()
    {
        // WebP: RIFF[4-byte-size]WEBP + minimal VP8L chunk
        // Magic: 52 49 46 46 xx xx xx xx 57 45 42 50
        var riff = new byte[] { 0x52, 0x49, 0x46, 0x46 }; // "RIFF"
        var size = BitConverter.GetBytes(12); // size placeholder (little-endian)
        var webp = new byte[] { 0x57, 0x45, 0x42, 0x50 }; // "WEBP"
        var vp8l = new byte[] { 0x56, 0x50, 0x38, 0x4C, 0x04, 0x00, 0x00, 0x00, 0x2F, 0x00, 0x00, 0x00 }; // VP8L chunk

        return [.. riff, .. size, .. webp, .. vp8l];
    }
}
