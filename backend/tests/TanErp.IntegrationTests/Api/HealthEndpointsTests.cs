using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Infrastructure.Identity;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class HealthEndpointsTests : IAsyncLifetime
{
    private const string UnreachableConnection = "Host=127.0.0.1;Port=1;Database=health;Username=secret-user;Password=secret-password;Timeout=1";

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private WebApplicationFactory<Program> _healthy = null!;
    private WebApplicationFactory<Program> _unreachable = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _healthy = CreateFactory(_postgres.GetConnectionString());
        _unreachable = CreateFactory(UnreachableConnection);
    }

    public async Task DisposeAsync()
    {
        await _healthy.DisposeAsync();
        await _unreachable.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task Live_WithoutAuthentication_ReturnsHealthyEvenWhenDatabaseIsDown()
    {
        var response = await _unreachable.CreateClient().GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await ReadStatus(response));
    }

    [Fact]
    public async Task Ready_WhenDatabaseIsReachable_ReturnsHealthyWithDatabaseCheck()
    {
        var response = await _healthy.CreateClient().GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Healthy", json.RootElement.GetProperty("status").GetString());
        Assert.Equal("Healthy", json.RootElement.GetProperty("checks").GetProperty("database").GetString());
    }

    [Fact]
    public async Task Ready_WhenDatabaseIsDown_Returns503WithoutLeakingConnectionDetails()
    {
        var response = await _unreachable.CreateClient().GetAsync("/health/ready");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Contains("Unhealthy", body);
        Assert.DoesNotContain("secret-password", body);
        Assert.DoesNotContain("secret-user", body);
        Assert.DoesNotContain("127.0.0.1", body);
        Assert.Equal("no-store", response.Headers.CacheControl?.ToString());
    }

    private static WebApplicationFactory<Program> CreateFactory(string connectionString) =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Database"] = connectionString
                });
            });
            builder.ConfigureServices(services =>
            {
                // The authentication pipeline runs for every request, so the real Firebase verifier (which needs credentials) is replaced.
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFirebaseTokenVerifier));
                if (descriptor != null) services.Remove(descriptor);
                services.AddSingleton<IFirebaseTokenVerifier, NoTokenVerifier>();
            });
        });

    private sealed class NoTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(null);
    }

    private static async Task<string?> ReadStatus(HttpResponseMessage response)
    {
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("status").GetString();
    }
}
