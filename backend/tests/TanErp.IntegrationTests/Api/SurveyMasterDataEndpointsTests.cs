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
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class SurveyMasterDataEndpointsTests : IAsyncLifetime
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

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:Database"] = _postgres.GetConnectionString(),
                        ["RunDatabaseMigrationsOnStartup"] = "true",
                        ["Database:CommandTimeoutSeconds"] = "60"
                    });
                });
                builder.ConfigureServices(services =>
                {
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

    private void AddHeaders(HttpRequestMessage request)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token-org-a");
        request.Headers.Add("X-Organization-Id", OrgAId.ToString());
        request.Headers.Add("X-Branch-Id", BranchAId.ToString());
        request.Headers.Add("X-Membership-Id", MembershipAId.ToString());
    }

    [Fact]
    public async Task ListDesignStyles_Returns200WithSeededDefaults()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/settings/surveys/design-styles");
        AddHeaders(request);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var styles = await response.Content.ReadFromJsonAsync<IReadOnlyList<SurveyDesignStyleResponse>>();
        Assert.NotNull(styles);
        Assert.NotEmpty(styles);
        Assert.Contains(styles, s => s.Code == "modern_luxury" && !string.IsNullOrWhiteSpace(s.Name.Thai));
    }

    [Fact]
    public async Task CreateAndEditDesignStyle_PersistsJsonbSuccessfully()
    {
        // 1. Create a custom style
        var createPayload = new CreateSurveyDesignStyleRequest(
            "wabi_sabi",
            new SurveyLocalizedNameContract("วาบิ-ซาบิ (Wabi-Sabi)", "Wabi-Sabi Natural Imperfection"),
            new SurveyLocalizedNameContract("ความงามในความไม่สมบูรณ์แบบ เน้นไม้เก่า ดินเผา และธรรมชาติ", "Beauty in imperfection with weathered wood and raw earth textures"),
            99);

        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/settings/surveys/design-styles")
        {
            Content = JsonContent.Create(createPayload)
        };
        AddHeaders(createRequest);

        var createResponse = await _client.SendAsync(createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<SurveyDesignStyleResponse>();
        Assert.NotNull(created);
        Assert.Equal("wabi_sabi", created.Code);
        Assert.Equal("วาบิ-ซาบิ (Wabi-Sabi)", created.Name.Thai);
        Assert.Equal("Wabi-Sabi Natural Imperfection", created.Name.English);

        // 2. Update the style
        var updatePayload = new UpdateSurveyDesignStyleRequest(
            new SurveyLocalizedNameContract("วาบิ-ซาบิ ปรับปรุง", "Wabi-Sabi Updated"),
            new SurveyLocalizedNameContract("คำอธิบายใหม่", "Updated Description"),
            10,
            true);

        using var updateRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/settings/surveys/design-styles/{created.Id}")
        {
            Content = JsonContent.Create(updatePayload)
        };
        AddHeaders(updateRequest);

        var updateResponse = await _client.SendAsync(updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<SurveyDesignStyleResponse>();
        Assert.NotNull(updated);
        Assert.Equal("วาบิ-ซาบิ ปรับปรุง", updated.Name.Thai);
        Assert.Equal("Wabi-Sabi Updated", updated.Name.English);

        // 3. Verify it shows in Reference Data
        using var refRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/surveys/reference-data");
        AddHeaders(refRequest);

        var refResponse = await _client.SendAsync(refRequest);
        Assert.Equal(HttpStatusCode.OK, refResponse.StatusCode);
        var refData = await refResponse.Content.ReadFromJsonAsync<SurveyReferenceDataResponse>();
        Assert.NotNull(refData);
        Assert.Contains(refData.DesignStyles, s => s.Code == "wabi_sabi" && s.Name.Thai == "วาบิ-ซาบิ ปรับปรุง");
    }

    [Fact]
    public async Task CreateAndListMoodTones_PersistsColorPaletteJsonb()
    {
        var createPayload = new CreateSurveyMoodToneRequest(
            "sage_terracotta",
            new SurveyLocalizedNameContract("เซจ แอนด์ เทอร์ราคอตตา", "Sage & Terracotta"),
            new SurveyLocalizedNameContract("โทนเขียวอมเทาตัดกับดินเผาอบอุ่น", "Earthy muted sage green paired with warm terracotta"),
            new[] { "#9CAF88", "#E07A5F", "#F4F1DE", "#3D405B" },
            20);

        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/settings/surveys/mood-tones")
        {
            Content = JsonContent.Create(createPayload)
        };
        AddHeaders(createRequest);

        var createResponse = await _client.SendAsync(createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<SurveyMoodToneResponse>();
        Assert.NotNull(created);
        Assert.Equal("sage_terracotta", created.Code);
        Assert.Equal(4, created.ColorPalette.Count);
        Assert.Contains("#9CAF88", created.ColorPalette);

        // Verify list
        using var listRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/settings/surveys/mood-tones");
        AddHeaders(listRequest);

        var listResponse = await _client.SendAsync(listRequest);
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var tones = await listResponse.Content.ReadFromJsonAsync<IReadOnlyList<SurveyMoodToneResponse>>();
        Assert.NotNull(tones);
        Assert.Contains(tones, t => t.Code == "sage_terracotta");
    }
}
