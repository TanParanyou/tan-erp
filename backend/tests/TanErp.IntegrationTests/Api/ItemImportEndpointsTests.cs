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
using TanErp.Api.ErrorHandling;
using TanErp.Application.Items.Import;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class ItemImportEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private const string UidA = TestOnlyDataSeeder.TestFirebaseUid;
    private const string UidB = TestOnlyDataSeeder.TestFirebaseUidB;
    private static readonly string Header = string.Join(",", ItemImportCsv.Columns);

    private string _categoryCode = string.Empty;
    private string _unitCode = string.Empty;
    private string _brandCode = string.Empty;

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(idToken switch { "token-org-a" => UidA, "token-org-b" => UidB, _ => null });
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

        _categoryCode = "CAT-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        _unitCode = "UOM-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        _brandCode = "BRD-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();

        await PostMasterAsync("/api/v1/item-categories", new CreateItemCategoryRequest
        {
            Code = _categoryCode,
            Name = new LocalizedTextInput { Thai = "หมวดหมู่ทดสอบ", English = "Test Category" },
            AllowedItemTypes = new List<string> { "Standard", "Service" },
            SortOrder = 1
        });
        await PostMasterAsync("/api/v1/units-of-measure", new CreateUnitOfMeasureRequest
        {
            Code = _unitCode,
            Name = new LocalizedTextInput { Thai = "ชิ้น", English = "Piece" },
            Symbol = "pcs",
            Dimension = "Count",
            DecimalScale = 0,
            RoundingMode = "HalfUp"
        });
        await PostMasterAsync("/api/v1/item-brands", new CreateItemBrandRequest
        {
            Code = _brandCode,
            Name = new LocalizedTextInput { Thai = "แบรนด์ทดสอบ", English = "Test Brand" },
            SortOrder = 1
        });
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private async Task PostMasterAsync(string url, object body)
    {
        var request = Request(HttpMethod.Post, url, idempotencyKey: Guid.NewGuid().ToString("N"));
        request.Content = JsonContent.Create(body);
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static HttpRequestMessage Request(HttpMethod method, string url, string token = "token-org-a", string? idempotencyKey = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Membership-Id", (token == "token-org-b" ? TestOnlyDataSeeder.TestMembershipBId : TestOnlyDataSeeder.TestMembershipId).ToString());
        if (idempotencyKey is not null) request.Headers.Add("Idempotency-Key", idempotencyKey);
        return request;
    }

    private string Row(string code, string name = "ไม้อัด", string? category = null, string? unit = null, string? brand = null, string type = "material") =>
        $"{code},{type},{category ?? _categoryCode},{brand ?? _brandCode},{unit ?? _unitCode},{name},,,,,false,true,true,true,false";

    private static string Csv(params string[] rows) => string.Join("\n", new[] { Header }.Concat(rows));

    private async Task<HttpResponseMessage> PreviewAsync(string content, string token = "token-org-a")
    {
        var request = Request(HttpMethod.Post, "/api/v1/items/imports/preview", token);
        request.Content = JsonContent.Create(new PreviewItemImportRequest(content));
        return await _client.SendAsync(request);
    }

    private async Task<HttpResponseMessage> CommitAsync(string content, string key, string? expectedHash = null, string token = "token-org-a")
    {
        var request = Request(HttpMethod.Post, "/api/v1/items/imports/commit", token, key);
        request.Content = JsonContent.Create(new CommitItemImportRequest(content, expectedHash));
        return await _client.SendAsync(request);
    }

    private async Task<int> CountItemsAsync(string codePrefix)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.Items.CountAsync(i => i.Code.StartsWith(codePrefix));
    }

    [Fact]
    public async Task Preview_ReportsPerRowErrors_AndWritesNothing()
    {
        var content = Csv(
            Row("IMPA-001"),
            Row("IMPA-002", category: "NO-SUCH-CAT"),
            Row("IMPA-001", name: "ซ้ำ"),
            Row("IMPA-004", unit: "NO-SUCH-UNIT", type: "bogus"));

        var response = await PreviewAsync(content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var preview = (await response.Content.ReadFromJsonAsync<ItemImportPreviewResponse>())!;
        Assert.Equal(4, preview.TotalRows);
        Assert.Equal(1, preview.ValidRows);
        Assert.Equal(3, preview.InvalidRows);
        Assert.Equal(64, preview.ContentSha256.Length);

        Assert.True(preview.Rows[0].IsValid);
        Assert.Contains(preview.Rows[1].Errors, e => e is { Field: "categoryCode", Code: ItemImportErrorCodes.NotFound });
        Assert.Contains(preview.Rows[2].Errors, e => e is { Field: "code", Code: ItemImportErrorCodes.DuplicateInFile });
        Assert.Contains(preview.Rows[3].Errors, e => e is { Field: "itemType", Code: ItemImportErrorCodes.Invalid });
        Assert.Equal(0, await CountItemsAsync("IMPA-"));
    }

    [Fact]
    public async Task Preview_InvalidFile_Returns422()
    {
        var response = await PreviewAsync("code,itemType\nA,material");

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        var problem = (await response.Content.ReadFromJsonAsync<ApiProblemDetails>())!;
        Assert.Equal("ITEM_IMPORT_FILE_INVALID", problem.Code);
    }

    [Fact]
    public async Task Commit_AnyInvalidRow_ImportsNothing()
    {
        var content = Csv(Row("IMPB-001"), Row("IMPB-002", category: "NO-SUCH-CAT"));

        var response = await CommitAsync(content, "import-invalid-key-0001");

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        var problem = (await response.Content.ReadFromJsonAsync<ApiProblemDetails>())!;
        Assert.Equal("ITEM_IMPORT_VALIDATION_FAILED", problem.Code);
        Assert.Equal(0, await CountItemsAsync("IMPB-"));
    }

    [Fact]
    public async Task Commit_ValidFile_CreatesDraftItemsWithAuditAndReplaysIdempotently()
    {
        var content = Csv(Row("IMPC-001"), Row("IMPC-002", name: "กาว"), $",service,{_categoryCode},,{_unitCode},ค่าแรงติดตั้ง,,,,,true,true,false,false,false");
        var preview = (await (await PreviewAsync(content)).Content.ReadFromJsonAsync<ItemImportPreviewResponse>())!;
        Assert.Equal(3, preview.ValidRows);

        int costRecordsBefore;
        using (var scope = _factory.Services.CreateScope())
        {
            costRecordsBefore = await scope.ServiceProvider.GetRequiredService<AppDbContext>().CostRecords.CountAsync();
        }

        var first = await CommitAsync(content, "import-valid-key-0001", preview.ContentSha256);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        var result = (await first.Content.ReadFromJsonAsync<ItemImportCommitResponse>())!;
        Assert.Equal(3, result.CreatedCount);
        Assert.False(result.Replayed);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var created = await db.Items.AsNoTracking().Where(i => i.Code.StartsWith("IMPC-")).ToListAsync();
            Assert.Equal(2, created.Count);
            Assert.All(created, i => Assert.Equal("draft", i.Status));
            Assert.True(await db.AuditEvents.AnyAsync(a => a.Action == "items.import" && a.ResourceId == result.BatchId.ToString()));
            var createEvents = await db.AuditEvents.AsNoTracking().Where(a => a.Action == "items.create").ToListAsync();
            Assert.Equal(3, createEvents.Count(a => a.ChangesJson.Contains(result.BatchId.ToString())));
            // Import never touches cost records.
            Assert.Equal(costRecordsBefore, await db.CostRecords.CountAsync());
        }

        var replay = await CommitAsync(content, "import-valid-key-0001");
        Assert.Equal(HttpStatusCode.Created, replay.StatusCode);
        var replayed = (await replay.Content.ReadFromJsonAsync<ItemImportCommitResponse>())!;
        Assert.True(replayed.Replayed);
        Assert.Equal(result.BatchId, replayed.BatchId);
        Assert.Equal(3, replayed.CreatedCount);
        Assert.Equal(2, await CountItemsAsync("IMPC-"));

        var reused = await CommitAsync(Csv(Row("IMPC-099")), "import-valid-key-0001");
        Assert.Equal(HttpStatusCode.Conflict, reused.StatusCode);
    }

    [Fact]
    public async Task Commit_ChangedFileSincePreview_Returns409_AndExistingCodeIsReported()
    {
        var content = Csv(Row("IMPD-001"));
        var preview = (await (await PreviewAsync(content)).Content.ReadFromJsonAsync<ItemImportPreviewResponse>())!;

        var changed = await CommitAsync(Csv(Row("IMPD-002")), "import-changed-key-0001", preview.ContentSha256);
        Assert.Equal(HttpStatusCode.Conflict, changed.StatusCode);
        Assert.Equal(0, await CountItemsAsync("IMPD-"));

        Assert.Equal(HttpStatusCode.Created, (await CommitAsync(content, "import-changed-key-0002")).StatusCode);

        var again = (await (await PreviewAsync(content)).Content.ReadFromJsonAsync<ItemImportPreviewResponse>())!;
        Assert.Contains(again.Rows[0].Errors, e => e is { Field: "code", Code: ItemImportErrorCodes.AlreadyExists });
    }

    [Fact]
    public async Task Import_OtherOrganizationMasterDataIsNotVisible()
    {
        // Org B's membership does not see Org A's category/unit codes.
        var response = await PreviewAsync(Csv(Row("IMPE-001")), "token-org-b");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var preview = (await response.Content.ReadFromJsonAsync<ItemImportPreviewResponse>())!;
            Assert.Contains(preview.Rows[0].Errors, e => e is { Field: "categoryCode", Code: ItemImportErrorCodes.NotFound });
        }
        else
        {
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
