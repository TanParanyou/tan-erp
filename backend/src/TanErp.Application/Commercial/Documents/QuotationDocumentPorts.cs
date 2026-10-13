using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Domain.Commercial;

namespace TanErp.Application.Commercial.Documents;

/// <summary>What is recorded about a stored quotation PDF. The storage path and file id are never exposed.</summary>
public sealed record QuotationDocumentSummary(string Sha256, long ByteCount, string RendererVersion, DateTimeOffset RenderedAtUtc);

/// <summary>The stored bytes (hash-verified on read) with the name to offer to the client.</summary>
public sealed record QuotationDocumentFile(byte[] Content, string FileName, QuotationDocumentSummary Summary);

/// <summary>Raised when the renderer fails. The caller's transaction rolls back, so nothing is half-issued; surfaces as a 500.</summary>
public sealed class QuotationDocumentRenderException(string message, Exception inner) : Exception(message, inner);

/// <summary>
/// Write side, used inside an issue/amend unit of work. The quotation may still be unsaved (its estimate revision, organization and
/// branch already exist). Renders once, stores the blob and ADDS the file, document and audit rows to the caller's DbContext without
/// saving, so the caller's one SaveChanges commits the quotation and its document together.
/// </summary>
public interface IQuotationDocumentWriter
{
    Task<Result<QuotationDocumentSummary>> StageAsync(Quotation quotation, Guid actorUserId, string traceId, CancellationToken cancellationToken = default);
}

public interface IQuotationDocumentStore
{
    /// <summary>Organization and branch scoped. Out of scope or unknown: RESOURCE_NOT_FOUND. No document: QUOTATION_DOCUMENT_NOT_AVAILABLE.</summary>
    Task<Result<QuotationDocumentFile>> OpenAsync(RequestAccessContext access, Guid quotationId, CancellationToken cancellationToken = default);

    Task<QuotationDocumentSummary?> FindSummaryAsync(Guid organizationId, Guid quotationId, CancellationToken cancellationToken = default);

    /// <summary>For the public acceptance link only (the token already proved access). Null when there is no document.</summary>
    Task<QuotationDocumentFile?> OpenForAcceptanceAsync(Guid organizationId, Guid quotationId, CancellationToken cancellationToken = default);
}
