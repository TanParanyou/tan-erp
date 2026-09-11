using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class OpenApiContractTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

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
                    ["ConnectionStrings:Database"] = _postgres.GetConnectionString()
                });
            });
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(_postgres.GetConnectionString()));
            });
        });

        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task OpenApi_MeEndpoint_And_Schemas_Exist_And_Are_Committed()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var jsonString = await response.Content.ReadAsStringAsync();
        var doc = JsonNode.Parse(jsonString);
        Assert.NotNull(doc);

        // 1. Assert path /api/v1/me exists
        var paths = doc["paths"]?.AsObject();
        Assert.NotNull(paths);
        Assert.True(paths.ContainsKey("/api/v1/me"), "OpenAPI must contain path /api/v1/me");
        Assert.True(paths.ContainsKey("/api/v1/customers"), "OpenAPI must contain path /api/v1/customers");
        Assert.True(paths.ContainsKey("/api/v1/customers/{id}"), "OpenAPI must contain path /api/v1/customers/{id}");
        Assert.True(paths.ContainsKey("/api/v1/customers/{customerId}/sites"), "OpenAPI must contain path /api/v1/customers/{customerId}/sites");
        Assert.True(paths.ContainsKey("/api/v1/opportunities"), "OpenAPI must contain path /api/v1/opportunities");
        Assert.True(paths.ContainsKey("/api/v1/opportunities/{id}"), "OpenAPI must contain path /api/v1/opportunities/{id}");
        Assert.True(paths.ContainsKey("/api/v1/master-data/addresses/search"), "OpenAPI must contain path /api/v1/master-data/addresses/search");

        var searchAddressPath = paths["/api/v1/master-data/addresses/search"]?["get"]?.AsObject();
        Assert.NotNull(searchAddressPath);

        var mePath = paths["/api/v1/me"]?["get"]?.AsObject();
        Assert.NotNull(mePath);

        var listCustomerPath = paths["/api/v1/customers"]?["get"]?.AsObject();
        Assert.NotNull(listCustomerPath);

        var createCustomerPath = paths["/api/v1/customers"]?["post"]?.AsObject();
        Assert.NotNull(createCustomerPath);

        var getCustomerPath = paths["/api/v1/customers/{id}"]?["get"]?.AsObject();
        Assert.NotNull(getCustomerPath);

        var listSitesPath = paths["/api/v1/customers/{customerId}/sites"]?["get"]?.AsObject();
        Assert.NotNull(listSitesPath);

        var createSitePath = paths["/api/v1/customers/{customerId}/sites"]?["post"]?.AsObject();
        Assert.NotNull(createSitePath);

        var listOpportunitiesPath = paths["/api/v1/opportunities"]?["get"]?.AsObject();
        Assert.NotNull(listOpportunitiesPath);

        var createOpportunityPath = paths["/api/v1/opportunities"]?["post"]?.AsObject();
        Assert.NotNull(createOpportunityPath);

        var getOpportunityPath = paths["/api/v1/opportunities/{id}"]?["get"]?.AsObject();
        Assert.NotNull(getOpportunityPath);

        // 2. Assert operation responses 200, 401, 403
        var responses = mePath["responses"]?.AsObject();
        Assert.NotNull(responses);
        Assert.True(responses.ContainsKey("200"), "Must have 200 response");
        Assert.True(responses.ContainsKey("401"), "Must have 401 response");
        Assert.True(responses.ContainsKey("403"), "Must have 403 response");

        // 3. Assert bearer security scheme
        var components = doc["components"]?.AsObject();
        Assert.NotNull(components);
        var securitySchemes = components["securitySchemes"]?.AsObject();
        Assert.NotNull(securitySchemes);
        Assert.True(securitySchemes.ContainsKey("Bearer"), "Must define Bearer security scheme");

        // 4. Assert schemas
        var schemas = components["schemas"]?.AsObject();
        Assert.NotNull(schemas);
        Assert.True(schemas.ContainsKey("CurrentUserResponse"), "Must define CurrentUserResponse schema");
        Assert.True(schemas.ContainsKey("CustomerResponse"), "Must define CustomerResponse schema");
        Assert.True(schemas.ContainsKey("CustomerListResponse"), "Must define CustomerListResponse schema");
        Assert.True(schemas.ContainsKey("CreateCustomerRequest"), "Must define CreateCustomerRequest schema");
        Assert.True(schemas.ContainsKey("SiteResponse"), "Must define SiteResponse schema");
        Assert.True(schemas.ContainsKey("SiteListResponse"), "Must define SiteListResponse schema");
        Assert.True(schemas.ContainsKey("CreateSiteRequest"), "Must define CreateSiteRequest schema");
        Assert.True(schemas.ContainsKey("OpportunityResponse"), "Must define OpportunityResponse schema");
        Assert.True(schemas.ContainsKey("OpportunityListResponse"), "Must define OpportunityListResponse schema");
        Assert.True(schemas.ContainsKey("CreateOpportunityRequest"), "Must define CreateOpportunityRequest schema");
        Assert.True(schemas.ContainsKey("AddressSearchResponse"), "Must define AddressSearchResponse schema");
        Assert.True(schemas.ContainsKey("AddressSearchResultItem"), "Must define AddressSearchResultItem schema");
        Assert.True(schemas.ContainsKey("ApiProblemDetails"), "Must define ApiProblemDetails schema");

        // 5. Ensure contracts/openapi/tan-erp.v1.json exists and matches
        var contractsDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../../contracts/openapi"));
        Directory.CreateDirectory(contractsDir);
        var contractPath = Path.Combine(contractsDir, "tan-erp.v1.json");

        // Pretty print JSON
        var formattedJson = JsonSerializer.Serialize(doc, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        if (!File.Exists(contractPath) || Environment.GetEnvironmentVariable("UPDATE_OPENAPI") == "1")
        {
            await File.WriteAllTextAsync(contractPath, formattedJson);
        }

        var committedContent = await File.ReadAllTextAsync(contractPath);
        var committedDoc = JsonNode.Parse(committedContent);

        // Assert structural equality
        Assert.Equal(
            JsonSerializer.Serialize(doc),
            JsonSerializer.Serialize(committedDoc));
    }
}
