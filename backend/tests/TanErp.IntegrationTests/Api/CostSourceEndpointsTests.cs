using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
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

public sealed class CostSourceEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private sealed class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(idToken == "token-org-a" ? TestOnlyDataSeeder.TestFirebaseUid : null);
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = _postgres.GetConnectionString(), ["SeedTestData"] = "true"
            }));
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFirebaseTokenVerifier));
                if (descriptor is not null) services.Remove(descriptor);
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

    [Fact]
    public async Task CreateCostSource_IsIdempotent_AndUpdateRequiresCurrentEtag()
    {
        var key = $"source-{Guid.NewGuid():N}";
        var request = new CostSourceRequest { Code = "MANUAL-01", Name = new LocalizedTextInput { Thai = "ราคาผู้ใช้", English = "Manual" } };
        var first = await SendCreate(request, key);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        var created = await first.Content.ReadFromJsonAsync<CostSourceResponse>();
        Assert.NotNull(created);

        using var replay = await SendCreate(request, key);
        Assert.Equal(HttpStatusCode.Created, replay.StatusCode);
        var replayed = await replay.Content.ReadFromJsonAsync<CostSourceResponse>();
        Assert.Equal(created!.Id, replayed!.Id);

        using var duplicate = await SendCreate(request with { Code = "manual-01" }, $"source-{Guid.NewGuid():N}");
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);

        var update = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/cost-sources/{created.Id}")
        {
            Content = JsonContent.Create(new UpdateCostSourceRequest { Code = request.Code!, Name = new LocalizedTextInput { Thai = "ราคาปรับปรุง", English = "Updated" } })
        };
        AddAuth(update);
        update.Headers.Add("If-Match", $"\"{created.RowVersion}\"");
        using var updatedResponse = await _client.SendAsync(update);
        Assert.Equal(HttpStatusCode.OK, updatedResponse.StatusCode);
        var updated = await updatedResponse.Content.ReadFromJsonAsync<CostSourceResponse>();

        var stale = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/cost-sources/{created.Id}")
        {
            Content = JsonContent.Create(new UpdateCostSourceRequest { Code = request.Code!, Name = request.Name })
        };
        AddAuth(stale);
        stale.Headers.Add("If-Match", $"\"{created.RowVersion}\"");
        using var staleResponse = await _client.SendAsync(stale);
        Assert.Equal(HttpStatusCode.Conflict, staleResponse.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(2, await db.AuditEvents.CountAsync(e => e.ResourceType == "CostSource"));
        Assert.NotEqual(created.RowVersion, updated!.RowVersion);
    }

    [Fact]
    public async Task CreateCostSource_WithoutCode_GeneratesCodeAndAuditsGeneration()
    {
        using var response = await SendCreate(new CostSourceRequest { Name = new LocalizedTextInput { Thai = "ระบบสร้างรหัส" } }, $"generated-source-{Guid.NewGuid():N}");

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var source = await response.Content.ReadFromJsonAsync<CostSourceResponse>();
        Assert.NotNull(source);
        Assert.StartsWith("SRC-", source.Code);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var audit = await db.AuditEvents.SingleAsync(entry => entry.ResourceType == "CostSource" && entry.ResourceId == source.Id.ToString());
        using var auditJson = JsonDocument.Parse(audit.ChangesJson);
        Assert.True(auditJson.RootElement.GetProperty("codeGenerated").GetBoolean());
    }

    [Fact]
    public async Task LegacySourceTypeCannotBeCreatedThroughApi()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/cost-sources")
        {
            Content = JsonContent.Create(new CostSourceRequest { Code = "LEGACY", SourceType = "legacy", Name = new LocalizedTextInput { Thai = "เก่า" } })
        };
        AddAuth(request);
        request.Headers.Add("Idempotency-Key", $"source-{Guid.NewGuid():N}");
        using var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    private async Task<HttpResponseMessage> SendCreate(CostSourceRequest body, string key)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/cost-sources") { Content = JsonContent.Create(body) };
        AddAuth(request);
        request.Headers.Add("Idempotency-Key", key);
        return await _client.SendAsync(request);
    }

    private static void AddAuth(HttpRequestMessage request)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-org-a");
        request.Headers.Add("X-Membership-Id", TestOnlyDataSeeder.TestMembershipId.ToString());
    }
}
