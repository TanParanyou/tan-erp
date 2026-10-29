using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TanErp.Application.Commercial.Documents;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Files;
using TanErp.Domain.Commercial;
using TanErp.Domain.Common;
using TanErp.Domain.Files;
using TanErp.Infrastructure.Persistence;

namespace TanErp.Infrastructure.Documents;

public sealed partial class QuotationDocumentWriter(
    AppDbContext db, IQuotationDocumentRenderer renderer, IFileStorageProvider storage, IClock clock, ILogger<QuotationDocumentWriter> logger) : IQuotationDocumentWriter
{
    public const string UploadSessionPrefix = "system:quotation-document:";

    [GeneratedRegex("[^A-Za-z0-9._-]")]
    private static partial Regex UnsafeFileNameChars();

    public static string FileNameFor(string quotationNumber) => UnsafeFileNameChars().Replace(quotationNumber, "_") + ".pdf";

    public async Task<Result<QuotationDocumentSummary>> StageAsync(Quotation quotation, Guid actorUserId, string traceId, CancellationToken cancellationToken = default)
    {
        var (organizationId, quotationId) = (quotation.OrganizationId, quotation.Id);
        var model = await QuotationPdfModelBuilder.BuildAsync(db, quotation, cancellationToken, storage);
        if (model.IsFailure) return Result<QuotationDocumentSummary>.Failure(model.Error);

        byte[] bytes;
        try
        {
            bytes = renderer.Render(model.Value!);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Rendering the PDF of quotation {QuotationId} failed.", quotationId); // identifiers only: the model holds customer data
            throw new QuotationDocumentRenderException("The quotation PDF could not be rendered.", ex);
        }

        var now = clock.UtcNow;
        var number = model.Value!.Number;
        var sha256 = Sha256Hex.ComputeBytes(bytes);
        string storagePath;
        using (var content = new MemoryStream(bytes, writable: false))
            storagePath = await storage.SaveAsync(organizationId, $"quotation-document-{quotationId:N}", FileNameFor(number), content, cancellationToken);

        var file = new UploadedFile(Guid.NewGuid(), organizationId, storagePath, FileNameFor(number), "application/pdf", bytes.Length,
            UploadSessionPrefix + quotationId.ToString("N"), actorUserId, now, contentSha256: sha256);
        db.UploadedFiles.Add(file);
        db.QuotationDocuments.Add(new QuotationDocument(Guid.NewGuid(), organizationId, quotationId, file.Id, sha256, bytes.Length, renderer.Version, now, actorUserId));
        db.AddAuditEvent(new AuditEvent(Guid.NewGuid(), organizationId, actorUserId, "quotations.document-rendered", "Quotation", quotationId.ToString(), now, traceId,
            JsonSerializer.Serialize(new { quotationNumber = number, sha256, byteCount = bytes.Length, rendererVersion = renderer.Version })));

        return Result<QuotationDocumentSummary>.Success(new QuotationDocumentSummary(sha256, bytes.Length, renderer.Version, now));
    }
}
