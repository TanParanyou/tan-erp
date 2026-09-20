using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.DocumentNumbering;
using TanErp.Domain.DocumentNumbering;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class DocumentSequencesEndpointsTests : IAsyncLifetime
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

    private HttpRequestMessage CreateAuthenticatedRequest(HttpMethod method, string url, string token = "valid-token", Guid? membershipId = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Membership-Id", (membershipId ?? TestOnlyDataSeeder.TestMembershipId).ToString());
        return request;
    }

    [Fact]
    public async Task GetDocumentSequences_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync("/api/v1/settings/document-sequences");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetDocumentSequences_WithValidAuth_ReturnsListOfSupportedSequences()
    {
        var request = CreateAuthenticatedRequest(HttpMethod.Get, "/api/v1/settings/document-sequences");
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var list = await response.Content.ReadFromJsonAsync<List<DocumentSequenceResponse>>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);

        var estimateSeq = list.FirstOrDefault(s => s.DocumentType == "estimates");
        Assert.NotNull(estimateSeq);
        Assert.Equal("EST", estimateSeq.Prefix);
        Assert.NotNull(estimateSeq.SamplePreview);
        Assert.StartsWith("EST-", estimateSeq.SamplePreview);
    }

    [Fact]
    public async Task PreviewDocumentSequence_ReturnsFormattedSample()
    {
        var request = CreateAuthenticatedRequest(HttpMethod.Post, "/api/v1/settings/document-sequences/preview");
        request.Content = JsonContent.Create(new PreviewDocumentSequenceRequest
        {
            Prefix = "QT-",
            FormatPattern = "{PREFIX}{BBBB}{MM}-{SEQ:4}",
            BranchCode = "B01",
            Padding = 4,
            SampleSequence = 42
        });

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PreviewDocumentSequenceResponse>();
        Assert.NotNull(result);
        Assert.NotNull(result.Preview);

        var now = DateTime.UtcNow;
        var buddhistYear = now.Year + 543;
        var expectedPrefix = $"QT-{buddhistYear}{now:MM}-0042";
        Assert.Equal(expectedPrefix, result.Preview);
    }

    [Fact]
    public async Task UpdateDocumentSequence_UpdatesAndReturnsConfiguredSequence()
    {
        var getInitialReq = CreateAuthenticatedRequest(HttpMethod.Get, "/api/v1/settings/document-sequences");
        var getInitialRes = await _client.SendAsync(getInitialReq);
        Assert.Equal(HttpStatusCode.OK, getInitialRes.StatusCode);
        var initialList = await getInitialRes.Content.ReadFromJsonAsync<List<DocumentSequenceResponse>>();
        var initialEst = initialList!.First(s => s.DocumentType == "estimates");

        var updateRequest = CreateAuthenticatedRequest(HttpMethod.Put, "/api/v1/settings/document-sequences/estimates");
        updateRequest.Headers.Add("If-Match", $"\"{initialEst.RowVersion}\"");
        updateRequest.Content = JsonContent.Create(new UpdateDocumentSequenceRequest
        {
            Prefix = "EST-NEW-",
            FormatPattern = "{PREFIX}{YYYY}-{SEQ:5}",
            ResetPeriod = "Yearly",
            Padding = 5,
            IsBranchSpecific = false
        });

        var response = await _client.SendAsync(updateRequest);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await response.Content.ReadFromJsonAsync<DocumentSequenceResponse>();
        Assert.NotNull(updated);
        Assert.Equal("estimates", updated.DocumentType);
        Assert.Equal("EST-NEW-", updated.Prefix);
        Assert.Equal("{PREFIX}{YYYY}-{SEQ:5}", updated.FormatPattern);
        Assert.Equal("Yearly", updated.ResetPeriod);
        Assert.Equal(5, updated.Padding);
        Assert.False(updated.IsBranchSpecific);
        Assert.NotEqual(Guid.Empty, updated.RowVersion);

        // Verify that GET also reflects the updated sequence
        var getRequest = CreateAuthenticatedRequest(HttpMethod.Get, "/api/v1/settings/document-sequences");
        var getResponse = await _client.SendAsync(getRequest);
        var list = await getResponse.Content.ReadFromJsonAsync<List<DocumentSequenceResponse>>();
        Assert.NotNull(list);

        var estimateSeq = list.FirstOrDefault(s => s.DocumentType == "estimates");
        Assert.NotNull(estimateSeq);
        Assert.Equal("EST-NEW-", estimateSeq.Prefix);
        Assert.Equal("{PREFIX}{YYYY}-{SEQ:5}", estimateSeq.FormatPattern);
        Assert.StartsWith("EST-NEW-", estimateSeq.SamplePreview);
        Assert.Equal(updated.RowVersion, estimateSeq.RowVersion);
    }

    [Fact]
    public async Task UpdateDocumentSequence_WithInvalidPadding_Returns400BadRequest()
    {
        var getInitialReq = CreateAuthenticatedRequest(HttpMethod.Get, "/api/v1/settings/document-sequences");
        var getInitialRes = await _client.SendAsync(getInitialReq);
        var initialList = await getInitialRes.Content.ReadFromJsonAsync<List<DocumentSequenceResponse>>();
        var initialEst = initialList!.First(s => s.DocumentType == "estimates");

        var request = CreateAuthenticatedRequest(HttpMethod.Put, "/api/v1/settings/document-sequences/estimates");
        request.Headers.Add("If-Match", $"\"{initialEst.RowVersion}\"");
        request.Content = JsonContent.Create(new UpdateDocumentSequenceRequest
        {
            Prefix = "EST-",
            FormatPattern = "{PREFIX}{YYYY}-{SEQ:4}",
            ResetPeriod = "Yearly",
            Padding = 0 // Invalid padding: must be 1-10
        });

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDocumentSequence_WithInvalidDocType_Returns400BadRequest()
    {
        var request = CreateAuthenticatedRequest(HttpMethod.Put, "/api/v1/settings/document-sequences/unsupported-type");
        request.Headers.Add("If-Match", $"\"{Guid.NewGuid()}\"");
        request.Content = JsonContent.Create(new UpdateDocumentSequenceRequest
        {
            Prefix = "UNKNOWN-",
            FormatPattern = "{PREFIX}{YYYY}-{SEQ:4}",
            ResetPeriod = "Yearly",
            Padding = 4
        });

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task MultiTenant_UpdatingOrgASequence_DoesNotLeakToOrgB()
    {
        // 1. Get Org A initial version
        var orgAGetReq = CreateAuthenticatedRequest(HttpMethod.Get, "/api/v1/settings/document-sequences");
        var orgAGetRes = await _client.SendAsync(orgAGetReq);
        var orgAList = await orgAGetRes.Content.ReadFromJsonAsync<List<DocumentSequenceResponse>>();
        var orgAInitial = orgAList!.First(s => s.DocumentType == "estimates");

        // 2. Org A updates its estimate sequence
        var updateRequest = CreateAuthenticatedRequest(HttpMethod.Put, "/api/v1/settings/document-sequences/estimates");
        updateRequest.Headers.Add("If-Match", $"\"{orgAInitial.RowVersion}\"");
        updateRequest.Content = JsonContent.Create(new UpdateDocumentSequenceRequest
        {
            Prefix = "TENANT-A-",
            FormatPattern = "{PREFIX}{YYYY}-{SEQ:4}",
            ResetPeriod = "Yearly",
            Padding = 4
        });
        var updateResponse = await _client.SendAsync(updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        // 3. Org B reads its estimate sequence
        var orgBRequest = CreateAuthenticatedRequest(
            HttpMethod.Get,
            "/api/v1/settings/document-sequences",
            token: "org-b-token",
            membershipId: TestOnlyDataSeeder.TestMembershipBId);

        var orgBResponse = await _client.SendAsync(orgBRequest);
        Assert.Equal(HttpStatusCode.OK, orgBResponse.StatusCode);

        var orgBList = await orgBResponse.Content.ReadFromJsonAsync<List<DocumentSequenceResponse>>();
        Assert.NotNull(orgBList);
        var orgBEstimate = orgBList.First(s => s.DocumentType == "estimates");

        // Org B must still have the default prefix "EST", NOT "TENANT-A-"
        Assert.Equal("EST", orgBEstimate.Prefix);
        Assert.StartsWith("EST-", orgBEstimate.SamplePreview);
    }

    [Theory]
    [InlineData("Success", HttpStatusCode.OK)]
    [InlineData("MissingPermission", HttpStatusCode.Forbidden)]
    [InlineData("StaleVersion", HttpStatusCode.Conflict)]
    [InlineData("InvalidReset", HttpStatusCode.BadRequest)]
    [InlineData("InvalidToken", HttpStatusCode.BadRequest)]
    [InlineData("MissingSequenceToken", HttpStatusCode.BadRequest)]
    [InlineData("DuplicateSequenceToken", HttpStatusCode.BadRequest)]
    public async Task DocumentSequence_UpdateRequiresPermissionVersionAndValidPattern(string scenario, HttpStatusCode expectedStatus)
    {
        // 1. Read baseline state
        var getReq = CreateAuthenticatedRequest(HttpMethod.Get, "/api/v1/settings/document-sequences");
        var getRes = await _client.SendAsync(getReq);
        Assert.Equal(HttpStatusCode.OK, getRes.StatusCode);
        var list = await getRes.Content.ReadFromJsonAsync<List<DocumentSequenceResponse>>();
        var baseline = list!.First(s => s.DocumentType == "quotations");
        var initialVersion = baseline.RowVersion;

        string ifMatchHeader = $"\"{initialVersion}\"";
        string prefix = baseline.Prefix;
        string pattern = "{PREFIX}-{YYYY}-{SEQ:4}";
        string reset = "Yearly";
        bool deactivatePermission = false;

        switch (scenario)
        {
            case "Success":
                prefix = "QT-SUCC-";
                pattern = "{PREFIX}-{YYYY}-{MM}-{SEQ:5}";
                reset = "Monthly";
                break;
            case "MissingPermission":
                deactivatePermission = true;
                prefix = "QT-NOPERM-";
                break;
            case "StaleVersion":
                ifMatchHeader = $"\"{Guid.NewGuid()}\"";
                prefix = "QT-STALE-";
                break;
            case "InvalidReset":
                reset = "Weekly";
                prefix = "QT-BADRESET-";
                break;
            case "InvalidToken":
                pattern = "{PREFIX}-{UNKNOWN_TOKEN}-{SEQ:4}";
                prefix = "QT-BADTOKEN-";
                break;
            case "MissingSequenceToken":
                pattern = "{PREFIX}-{YYYY}-{MM}";
                prefix = "QT-NOSEQ-";
                break;
            case "DuplicateSequenceToken":
                pattern = "{PREFIX}-{SEQ:4}-{SEQ:4}";
                prefix = "QT-DUPSEQ-";
                break;
        }

        if (deactivatePermission)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var perm = await db.Permissions.FirstAsync(p => p.Key == "document-sequences.manage");
            perm.Deactivate();
            await db.SaveChangesAsync();
        }

        try
        {
            var updateReq = CreateAuthenticatedRequest(HttpMethod.Put, "/api/v1/settings/document-sequences/quotations");
            updateReq.Headers.Add("If-Match", ifMatchHeader);
            updateReq.Content = JsonContent.Create(new UpdateDocumentSequenceRequest
            {
                Prefix = prefix,
                FormatPattern = pattern,
                ResetPeriod = reset,
                Padding = 4,
                IsBranchSpecific = false
            });

            var updateRes = await _client.SendAsync(updateReq);
            Assert.Equal(expectedStatus, updateRes.StatusCode);

            if (scenario == "Success")
            {
                Assert.NotNull(updateRes.Headers.ETag);
                var body = await updateRes.Content.ReadFromJsonAsync<DocumentSequenceResponse>();
                Assert.NotNull(body);
                Assert.NotEqual(initialVersion, body.RowVersion);
                Assert.Equal($"\"{body.RowVersion}\"", updateRes.Headers.ETag.Tag);

                // Verify updated definition
                var verifyReq = CreateAuthenticatedRequest(HttpMethod.Get, "/api/v1/settings/document-sequences");
                var verifyRes = await _client.SendAsync(verifyReq);
                var verifyList = await verifyRes.Content.ReadFromJsonAsync<List<DocumentSequenceResponse>>();
                var current = verifyList!.First(s => s.DocumentType == "quotations");
                Assert.Equal("QT-SUCC-", current.Prefix);
                Assert.Equal(pattern, current.FormatPattern);
                Assert.Equal(body.RowVersion, current.RowVersion);
            }
            else
            {
                // Definition must remain unchanged
                var verifyReq = CreateAuthenticatedRequest(HttpMethod.Get, "/api/v1/settings/document-sequences");
                var verifyRes = await _client.SendAsync(verifyReq);
                var verifyList = await verifyRes.Content.ReadFromJsonAsync<List<DocumentSequenceResponse>>();
                var current = verifyList!.First(s => s.DocumentType == "quotations");
                Assert.Equal(baseline.Prefix, current.Prefix);
                Assert.Equal(baseline.FormatPattern, current.FormatPattern);
                Assert.Equal(initialVersion, current.RowVersion);
            }
        }
        finally
        {
            if (deactivatePermission)
            {
                using var scope = _factory.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var perm = await db.Permissions.FirstAsync(p => p.Key == "document-sequences.manage");
                perm.Activate();
                await db.SaveChangesAsync();
            }
        }
    }
}

