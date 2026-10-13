using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TanErp.Application.Commercial.Documents;
using TanErp.Application.Files;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Persistence;
using Xunit;

namespace TanErp.IntegrationTests.Api;

// G-04: the server-side quotation PDF. Shares the harness (database, tokens, helpers) of the main file.
public partial class EstimateEndpointsTests
{
    private sealed class ThrowingQuotationRenderer : IQuotationDocumentRenderer
    {
        public string Version => "test-throwing/v0";
        public byte[] Render(QuotationPdfModel model) => throw new InvalidOperationException("TEST_ONLY renderer failure");
    }

    private static string Sha256Of(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

    // G-04 tests of later tasks are inserted below this line.

    [Fact]
    public async Task IssueQuotation_StoresOneDocumentRow_WhoseHashMatchesTheStoredBytes()
    {
        var (estimateId, _, quotation) = await IssueQuotedEstimateAsync("pdf-issue");
        var item = (await HistoryAsync(estimateId)).Items.Single();
        Assert.NotNull(item.Document);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var document = await db.QuotationDocuments.AsNoTracking().SingleAsync(d => d.QuotationId == quotation.QuotationId);
        var file = await db.UploadedFiles.AsNoTracking().SingleAsync(f => f.Id == document.FileId);
        Assert.Equal(("application/pdf", document.Sha256, document.ByteCount), (file.MediaType, file.ContentSha256, file.FileSizeBytes));
        Assert.Equal(item.Document.Sha256, document.Sha256);

        await using var stream = (await scope.ServiceProvider.GetRequiredService<IFileStorageProvider>().OpenReadStreamAsync(file.StoragePath))!;
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer);
        Assert.StartsWith("%PDF-", Encoding.ASCII.GetString(buffer.ToArray(), 0, 5));
        Assert.Equal(document.Sha256, Sha256Of(buffer.ToArray()));

        var audit = await db.AuditEvents.AsNoTracking().SingleAsync(a => a.Action == "quotations.document-rendered" && a.ResourceId == quotation.QuotationId.ToString());
        Assert.Contains(document.Sha256, audit.ChangesJson);
        Assert.DoesNotContain("ลูกค้า", audit.ChangesJson);
    }

    [Fact]
    public async Task IssueQuotation_WhenRenderingFails_RollsBackTheWholeIssue()
    {
        using var failing = _factory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            services.RemoveAll<IQuotationDocumentRenderer>();
            services.AddSingleton<IQuotationDocumentRenderer, ThrowingQuotationRenderer>();
        }));
        using var failingClient = failing.CreateClient();

        var (estimateId, oppId, _) = await IssueQuotedEstimateAsync("pdf-fail", failingClient, HttpStatusCode.InternalServerError);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(0, await db.Quotations.CountAsync(q => q.EstimateId == estimateId));
        Assert.Equal(0, await db.QuotationDocuments.CountAsync());
        Assert.Equal(0, await db.IdempotencyRecords.CountAsync(r => r.Operation == "quotations.issue"));
        Assert.Equal(OpportunityStage.Estimating, (await db.Opportunities.AsNoTracking().SingleAsync(o => o.Id == oppId)).Stage);
    }

    [Fact]
    public async Task AmendQuotation_RendersTheReplacementOnce_AndKeepsTheOriginalDocumentUntouched()
    {
        var (estimateId, _, original) = await IssueQuotedEstimateAsync("pdf-amend");
        var before = (await HistoryAsync(estimateId)).Items.Single();
        var amended = await _client.SendAsync(LifecycleRequest($"/api/v1/quotations/{original.QuotationId}/amend", before.RowVersion, "ปรับเงื่อนไข", $"pdf-amend-{Guid.NewGuid():N}"));
        Assert.Equal(HttpStatusCode.Created, amended.StatusCode);

        var items = (await amended.Content.ReadFromJsonAsync<TanErp.Api.Contracts.Commercial.QuotationHistoryResponse>())!.Items;
        Assert.Equal(before.Document!.Sha256, items.Single(i => i.Id == original.QuotationId).Document!.Sha256);
        Assert.NotEqual(before.Document.Sha256, items.Single(i => i.Id != original.QuotationId).Document!.Sha256);
    }

}
