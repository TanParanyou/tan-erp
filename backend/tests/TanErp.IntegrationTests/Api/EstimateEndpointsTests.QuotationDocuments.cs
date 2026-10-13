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
    private Task<HttpResponseMessage> GetQuotationPdfAsync(Guid quotationId, string token = "token-org-a", Guid? membership = null) =>
        _client.SendAsync(CreateAuthenticatedRequest(HttpMethod.Get, $"/api/v1/quotations/{quotationId}/document", token, membership ?? MembershipAId));

    // Simulates a quotation issued before G-04: it has no stored document.
    private async Task SimulateLegacyQuotationAsync(Guid quotationId)
    {
        using var scope = _factory.Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.ExecuteSqlInterpolatedAsync($"DELETE FROM commercial.quotation_documents WHERE quotation_id = {quotationId}");
    }

    // A user whose quotations.read is scoped to ANOTHER branch of the same organization.
    private static async Task<Guid> SeedOtherBranchReaderAsync(AppDbContext db)
    {
        var (branchId, userId, membershipId, roleId) = (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        db.Branches.Add(new Branch(branchId, OrgAId, $"QR-{branchId:N}"[..12], "Other Branch"));
        db.Users.Add(new User(userId, "test-only-quotation-reader", "Quotation Reader", "quotation-reader@example.test"));
        db.Memberships.Add(new Membership(membershipId, OrgAId, branchId, userId));
        db.Roles.Add(new Role(roleId, OrgAId, "TEST ONLY OTHER BRANCH", "TEST_ONLY other branch"));
        db.MembershipRoles.Add(new MembershipRole(membershipId, roleId, OrgAId));
        var read = await db.Permissions.SingleAsync(row => row.Key == "quotations.read");
        db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), roleId, OrgAId, read.Id, PermissionScope.Branch, branchId, branchId));
        await db.SaveChangesAsync();
        return membershipId;
    }

    [Fact]
    public async Task QuotationDocument_IsServedFromStorage_AndIsNotChangedByLaterOrganizationProfileChanges()
    {
        var (estimateId, _, quotation) = await IssueQuotedEstimateAsync("pdf-download");
        var summary = (await HistoryAsync(estimateId)).Items.Single().Document!;

        var response = await GetQuotationPdfAsync(quotation.QuotationId);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal($"\"{summary.Sha256}\"", response.Headers.ETag?.Tag);
        var before = await response.Content.ReadAsByteArrayAsync();
        Assert.Equal(summary.Sha256, Sha256Of(before));

        // Master data changes after issue: the stored file stays exactly as issued because it is never re-rendered.
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            (await db.Organizations.SingleAsync(o => o.Id == OrgAId)).UpdateProfile("ชื่อองค์กรใหม่หลังออกเอกสาร", "Renamed after issue", null, "ที่อยู่ใหม่", null, null);
            await db.SaveChangesAsync();
        }

        Assert.Equal(before, await (await GetQuotationPdfAsync(quotation.QuotationId)).Content.ReadAsByteArrayAsync());
    }

    [Fact]
    public async Task QuotationDocument_OutsideTheCallersOrganizationOrBranch_IsNotFound_AndALegacyQuotationHasNone()
    {
        var (_, _, quotation) = await IssueQuotedEstimateAsync("pdf-scope");
        Guid otherBranchMembership;
        using (var scope = _factory.Services.CreateScope())
            otherBranchMembership = await SeedOtherBranchReaderAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>());

        Assert.Equal(HttpStatusCode.NotFound, (await GetQuotationPdfAsync(quotation.QuotationId, "token-org-b", TestOnlyDataSeeder.TestMembershipBId)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await GetQuotationPdfAsync(quotation.QuotationId, "token-quotation-reader", otherBranchMembership)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await GetQuotationPdfAsync(Guid.NewGuid())).StatusCode);

        // A quotation issued before G-04 has no document: a stable code, and the history says so.
        await SimulateLegacyQuotationAsync(quotation.QuotationId);
        var missing = await GetQuotationPdfAsync(quotation.QuotationId);
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        Assert.Equal("QUOTATION_DOCUMENT_NOT_AVAILABLE", await ProblemCodeAsync(missing));
    }

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
