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

        var patchOpportunityPath = paths["/api/v1/opportunities/{id}"]?["patch"]?.AsObject();
        Assert.NotNull(patchOpportunityPath);

        var putOpportunityPath = paths["/api/v1/opportunities/{id}"]?["put"]?.AsObject();
        Assert.NotNull(putOpportunityPath);

        Assert.True(paths.ContainsKey("/api/v1/opportunities/{id}/stage-transitions"), "OpenAPI must contain path /api/v1/opportunities/{id}/stage-transitions");
        var transitionPath = paths["/api/v1/opportunities/{id}/stage-transitions"]?["post"]?.AsObject();
        Assert.NotNull(transitionPath);

        Assert.True(paths.ContainsKey("/api/v1/opportunities/{id}/owner-changes"), "OpenAPI must contain path /api/v1/opportunities/{id}/owner-changes");
        var ownerChangesPath = paths["/api/v1/opportunities/{id}/owner-changes"]?["post"]?.AsObject();
        Assert.NotNull(ownerChangesPath);

        Assert.True(paths.ContainsKey("/api/v1/opportunities/{id}/stage-history"), "OpenAPI must contain path /api/v1/opportunities/{id}/stage-history");
        var stageHistoryPath = paths["/api/v1/opportunities/{id}/stage-history"]?["get"]?.AsObject();
        Assert.NotNull(stageHistoryPath);

        Assert.True(paths.ContainsKey("/api/v1/opportunities/{opportunityId}/surveys"), "OpenAPI must contain path /api/v1/opportunities/{opportunityId}/surveys");
        var surveyPath = paths["/api/v1/opportunities/{opportunityId}/surveys"]?.AsObject();
        Assert.NotNull(surveyPath);
        Assert.NotNull(surveyPath["post"]?.AsObject());
        Assert.NotNull(surveyPath["get"]?.AsObject());

        Assert.True(paths.ContainsKey("/api/v1/estimates"), "OpenAPI must contain path /api/v1/estimates");
        var createEstimatePath = paths["/api/v1/estimates"]?["post"]?.AsObject();
        Assert.NotNull(createEstimatePath);

        Assert.True(paths.ContainsKey("/api/v1/estimates/{id}"), "OpenAPI must contain path /api/v1/estimates/{id}");
        var getEstimatePath = paths["/api/v1/estimates/{id}"]?["get"]?.AsObject();
        Assert.NotNull(getEstimatePath);

        Assert.True(paths.ContainsKey("/api/v1/opportunities/{opportunityId}/estimates"), "OpenAPI must contain path /api/v1/opportunities/{opportunityId}/estimates");
        var getOpportunityEstimatesPath = paths["/api/v1/opportunities/{opportunityId}/estimates"]?["get"]?.AsObject();
        Assert.NotNull(getOpportunityEstimatesPath);

        Assert.True(paths.ContainsKey("/api/v1/estimates/{id}/revisions/{revisionId}/draft"), "OpenAPI must contain path /api/v1/estimates/{id}/revisions/{revisionId}/draft");
        var updateEstimateDraftPath = paths["/api/v1/estimates/{id}/revisions/{revisionId}/draft"]?["put"]?.AsObject();
        Assert.NotNull(updateEstimateDraftPath);

        Assert.True(paths.ContainsKey("/api/v1/estimates/{id}/revisions/{revisionId}/calculate"), "OpenAPI must contain path /api/v1/estimates/{id}/revisions/{revisionId}/calculate");
        var calculateEstimatePath = paths["/api/v1/estimates/{id}/revisions/{revisionId}/calculate"]?["post"]?.AsObject();
        Assert.NotNull(calculateEstimatePath);

        Assert.True(paths.ContainsKey("/api/v1/estimates/{id}/quotation"), "OpenAPI must contain path /api/v1/estimates/{id}/quotation");
        var issueQuotationPath = paths["/api/v1/estimates/{id}/quotation"]?["post"]?.AsObject();
        Assert.NotNull(issueQuotationPath);

        Assert.True(paths.ContainsKey("/api/v1/estimates/{id}/quotation/accept"), "OpenAPI must contain path /api/v1/estimates/{id}/quotation/accept");
        var acceptQuotationPath = paths["/api/v1/estimates/{id}/quotation/accept"]?["post"]?.AsObject();
        Assert.NotNull(acceptQuotationPath);

        Assert.True(paths.ContainsKey("/api/v1/users"), "OpenAPI must contain path /api/v1/users");
        var listUsersPath = paths["/api/v1/users"]?["get"]?.AsObject();
        Assert.NotNull(listUsersPath);

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
        Assert.True(schemas.ContainsKey("PaginationMetadataResponse"), "Must define PaginationMetadataResponse schema");
        Assert.True(schemas.ContainsKey("CreateCustomerRequest"), "Must define CreateCustomerRequest schema");
        Assert.True(schemas.ContainsKey("SiteResponse"), "Must define SiteResponse schema");
        Assert.True(schemas.ContainsKey("SiteListResponse"), "Must define SiteListResponse schema");
        Assert.True(schemas.ContainsKey("CreateSiteRequest"), "Must define CreateSiteRequest schema");
        Assert.True(schemas.ContainsKey("OpportunityResponse"), "Must define OpportunityResponse schema");
        Assert.True(schemas.ContainsKey("OpportunityListResponse"), "Must define OpportunityListResponse schema");
        Assert.True(schemas.ContainsKey("CreateOpportunityRequest"), "Must define CreateOpportunityRequest schema");
        Assert.True(schemas.ContainsKey("UpdateDraftQGateRequest"), "Must define UpdateDraftQGateRequest schema");
        Assert.True(schemas.ContainsKey("UpdateOpenOpportunityRequest"), "Must define UpdateOpenOpportunityRequest schema");
        Assert.True(schemas.ContainsKey("ReassignOpportunityOwnerRequest"), "Must define ReassignOpportunityOwnerRequest schema");
        Assert.True(schemas.ContainsKey("TransitionOpportunityStageRequest"), "Must define TransitionOpportunityStageRequest schema");
        Assert.True(schemas.ContainsKey("OpportunityStageHistoryListResponse"), "Must define OpportunityStageHistoryListResponse schema");
        Assert.True(schemas.ContainsKey("OpportunityStageHistoryItemResponse"), "Must define OpportunityStageHistoryItemResponse schema");
        Assert.True(schemas.ContainsKey("AddressSearchResponse"), "Must define AddressSearchResponse schema");
        Assert.True(schemas.ContainsKey("AddressSearchResultItem"), "Must define AddressSearchResultItem schema");
        Assert.True(schemas.ContainsKey("CreateSiteSurveyRequest"), "Must define CreateSiteSurveyRequest schema");
        Assert.True(schemas.ContainsKey("SiteSurveyResponse"), "Must define SiteSurveyResponse schema");
        Assert.True(schemas.ContainsKey("SiteSurveyRevisionResponse"), "Must define SiteSurveyRevisionResponse schema");
        Assert.True(schemas.ContainsKey("CreateEstimateDraftRequest"), "Must define CreateEstimateDraftRequest schema");
        Assert.True(schemas.ContainsKey("EstimateDetailResponse"), "Must define EstimateDetailResponse schema");
        Assert.True(schemas.ContainsKey("EstimateRevisionResponse"), "Must define EstimateRevisionResponse schema");
        Assert.True(schemas.ContainsKey("UpdateEstimateDraftRequest"), "Must define UpdateEstimateDraftRequest schema");
        Assert.True(schemas.ContainsKey("CalculateEstimateRequest"), "Must define CalculateEstimateRequest schema");
        Assert.True(schemas.ContainsKey("IssueQuotationRequest"), "Must define IssueQuotationRequest schema");
        Assert.True(schemas.ContainsKey("QuotationResponse"), "Must define QuotationResponse schema");
        Assert.True(schemas.ContainsKey("AcceptQuotationRequest"), "Must define AcceptQuotationRequest schema");
        Assert.True(schemas.ContainsKey("AcceptQuotationResponse"), "Must define AcceptQuotationResponse schema");
        Assert.True(schemas.ContainsKey("UserListResponse"), "Must define UserListResponse schema");
        Assert.True(schemas.ContainsKey("UserListItemResponse"), "Must define UserListItemResponse schema");
        Assert.True(schemas.ContainsKey("BranchSummaryResponse"), "Must define BranchSummaryResponse schema");
        Assert.True(schemas.ContainsKey("DocumentSequenceResponse"), "Must define DocumentSequenceResponse schema");
        Assert.True(schemas.ContainsKey("UpdateDocumentSequenceRequest"), "Must define UpdateDocumentSequenceRequest schema");
        Assert.True(schemas.ContainsKey("ApiProblemDetails"), "Must define ApiProblemDetails schema");

        // Assert required fields on mutation requests
        var issueSchema = schemas["IssueQuotationRequest"]?.AsObject();
        Assert.NotNull(issueSchema);
        var issueProperties = issueSchema["properties"]?.AsObject();
        Assert.NotNull(issueProperties);
        Assert.True(issueProperties.ContainsKey("expectedEstimateVersion"));
        Assert.True(issueProperties.ContainsKey("expectedOpportunityVersion"));

        var acceptSchema = schemas["AcceptQuotationRequest"]?.AsObject();
        Assert.NotNull(acceptSchema);
        var acceptProperties = acceptSchema["properties"]?.AsObject();
        Assert.NotNull(acceptProperties);
        Assert.True(acceptProperties.ContainsKey("expectedOpportunityVersion"));

        var updateSeqSchema = schemas["UpdateDocumentSequenceRequest"]?.AsObject();
        Assert.NotNull(updateSeqSchema);
        var updateSeqRequired = updateSeqSchema["required"]?.AsArray().Select(n => n?.GetValue<string>()).ToList();
        Assert.NotNull(updateSeqRequired);
        Assert.Contains("formatPattern", updateSeqRequired);
        Assert.Contains("resetPeriod", updateSeqRequired);

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
