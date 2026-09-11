using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.MasterData;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.MasterData.Seed;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class AddressMasterEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<string?>(idToken == "valid-token" ? TestOnlyDataSeeder.TestFirebaseUid : null);
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
        await AddressMasterDataSeeder.SeedAsync(db);
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private void AuthenticateClient(string token = "valid-token")
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _client.DefaultRequestHeaders.Add("X-Membership-Id", TestOnlyDataSeeder.TestMembershipId.ToString());
    }

    [Fact]
    public async Task Search_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/master-data/addresses/search?q=10110");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Search_EmptyQuery_ReturnsOkEmptyItems()
    {
        AuthenticateClient();
        var response = await _client.GetAsync("/api/v1/master-data/addresses/search?q=");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AddressSearchResponse>();
        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task Search_PostalCode_ReturnsMatchingAddressesWithCacheHeader()
    {
        AuthenticateClient();
        var response = await _client.GetAsync("/api/v1/master-data/addresses/search?q=10110&limit=5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.CacheControl?.Public);
        Assert.Equal(TimeSpan.FromSeconds(86400), response.Headers.CacheControl?.MaxAge);

        var result = await response.Content.ReadFromJsonAsync<AddressSearchResponse>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
        Assert.True(result.Items.Count <= 5);

        var first = result.Items[0];
        Assert.Equal("10110", first.PostalCode);
        Assert.Equal("กรุงเทพมหานคร", first.Province);
        Assert.Equal("TH", first.CountryCode);
        Assert.NotEmpty(first.Subdistrict);
        Assert.NotEmpty(first.District);
        Assert.Contains(first.Subdistrict, first.DisplayText);
    }

    [Fact]
    public async Task Search_ThaiText_ReturnsMatchingSubdistrict()
    {
        AuthenticateClient();
        var url = $"/api/v1/master-data/addresses/search?q={Uri.EscapeDataString("วัฒนา")}";
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AddressSearchResponse>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
        Assert.All(result.Items, item =>
            Assert.True(item.District.Contains("วัฒนา") || item.Subdistrict.Contains("วัฒนา") || item.DisplayText.Contains("วัฒนา")));
    }
}
