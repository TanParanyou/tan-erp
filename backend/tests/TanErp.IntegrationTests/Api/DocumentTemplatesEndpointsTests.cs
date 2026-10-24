using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.DocumentTemplates;
using TanErp.Domain.DocumentTemplates;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class DocumentTemplatesEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<string?>(idToken switch
            {
                "valid-token" => TestOnlyDataSeeder.TestFirebaseUid,
                "org-b-token" => TestOnlyDataSeeder.TestFirebaseUidB,
                _ => null
            });
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
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private void Authenticate(string token = "valid-token", string? membershipId = null)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _client.DefaultRequestHeaders.Remove("X-Membership-Id");
        _client.DefaultRequestHeaders.Add("X-Membership-Id", membershipId ?? TestOnlyDataSeeder.TestMembershipId.ToString());
    }

    [Fact]
    public async Task Endpoints_Require_Authentication()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        _client.DefaultRequestHeaders.Remove("X-Membership-Id");

        var response = await _client.GetAsync("/api/v1/settings/document-templates");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task List_Returns_SynthesizedDefault_WhenNoCustomTemplatesExist()
    {
        Authenticate();

        var response = await _client.GetAsync("/api/v1/settings/document-templates?documentType=quotations");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var list = await response.Content.ReadFromJsonAsync<List<DocumentTemplateResponse>>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
        var first = list.First();
        Assert.Equal("quotations", first.DocumentType);
        Assert.True(first.IsDefault);
        Assert.Contains("ใบเสนอราคา", first.LayoutConfigJson);
    }

    [Fact]
    public async Task Full_Crud_Lifecycle_Works_Correctly()
    {
        Authenticate();

        var config = DocumentTemplateLayoutConfig.DefaultFor("quotations");
        var configJson = JsonSerializer.Serialize(config);

        // 1. Create a new custom template
        var createRequest = new CreateDocumentTemplateRequest
        {
            DocumentType = "quotations",
            Name = "VIP Customer Quotation",
            IsDefault = false,
            LayoutConfigJson = configJson,
            LogoFileId = null,
            BranchId = null
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/settings/document-templates", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<DocumentTemplateResponse>();
        Assert.NotNull(created);
        Assert.Equal("VIP Customer Quotation", created.Name);
        Assert.False(created.IsDefault);

        var etag = createResponse.Headers.ETag?.Tag;
        Assert.NotNull(etag);

        // 2. Get by Id
        var getResponse = await _client.GetAsync($"/api/v1/settings/document-templates/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var retrieved = await getResponse.Content.ReadFromJsonAsync<DocumentTemplateResponse>();
        Assert.NotNull(retrieved);
        Assert.Equal(created.Id, retrieved.Id);

        // 3. Update with valid If-Match
        var updateRequest = new UpdateDocumentTemplateRequest
        {
            Name = "VIP Customer Quotation (Updated)",
            IsDefault = false,
            LayoutConfigJson = configJson,
            LogoFileId = null
        };

        var updateMessage = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/settings/document-templates/{created.Id}")
        {
            Content = JsonContent.Create(updateRequest)
        };
        updateMessage.Headers.TryAddWithoutValidation("If-Match", etag);

        var updateResponse = await _client.SendAsync(updateMessage);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await updateResponse.Content.ReadFromJsonAsync<DocumentTemplateResponse>();
        Assert.NotNull(updated);
        Assert.Equal("VIP Customer Quotation (Updated)", updated.Name);

        // 4. Update with outdated If-Match returns Conflict (409)
        var staleUpdateMessage = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/settings/document-templates/{created.Id}")
        {
            Content = JsonContent.Create(updateRequest)
        };
        staleUpdateMessage.Headers.TryAddWithoutValidation("If-Match", etag); // old etag

        var staleResponse = await _client.SendAsync(staleUpdateMessage);
        Assert.Equal(HttpStatusCode.Conflict, staleResponse.StatusCode);

        // 5. Delete the custom template
        var deleteResponse = await _client.DeleteAsync($"/api/v1/settings/document-templates/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // 6. Verify it is no longer retrievable
        var verifyDeletedResponse = await _client.GetAsync($"/api/v1/settings/document-templates/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, verifyDeletedResponse.StatusCode);
    }
}
