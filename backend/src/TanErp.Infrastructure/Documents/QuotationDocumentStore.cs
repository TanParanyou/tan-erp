using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TanErp.Application.Commercial.Documents;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Files;
using TanErp.Domain.Common;
using TanErp.Infrastructure.Persistence;

namespace TanErp.Infrastructure.Documents;

public sealed class QuotationDocumentStore(
    AppDbContext db,
    IFileStorageProvider storage,
    IQuotationDocumentWriter writer,
    IClock clock,
    ILogger<QuotationDocumentStore> logger) : IQuotationDocumentStore
{
    public async Task<Result<QuotationDocumentFile>> OpenAsync(RequestAccessContext access, Guid quotationId, CancellationToken cancellationToken = default)
    {
        var quotation = await db.Quotations.AsNoTracking().FirstOrDefaultAsync(q => q.OrganizationId == access.OrganizationId && q.Id == quotationId, cancellationToken);
        if (quotation is null || !access.HasBranchAccess(quotation.BranchId))
            return Result<QuotationDocumentFile>.Failure(new Error("RESOURCE_NOT_FOUND", "Quotation not found."));

        var file = await LoadAsync(access.OrganizationId, quotationId, cancellationToken);
        return file is null
            ? Result<QuotationDocumentFile>.Failure(new Error("QUOTATION_DOCUMENT_NOT_AVAILABLE", "This quotation has no stored PDF."))
            : Result<QuotationDocumentFile>.Success(file);
    }

    public Task<QuotationDocumentFile?> OpenForAcceptanceAsync(Guid organizationId, Guid quotationId, CancellationToken cancellationToken = default) =>
        LoadAsync(organizationId, quotationId, cancellationToken);

    public async Task<QuotationDocumentSummary?> FindSummaryAsync(Guid organizationId, Guid quotationId, CancellationToken cancellationToken = default) =>
        await db.QuotationDocuments.AsNoTracking()
            .Where(d => d.OrganizationId == organizationId && d.QuotationId == quotationId)
            .Select(d => new QuotationDocumentSummary(d.Sha256, d.ByteCount, d.RendererVersion, d.RenderedAtUtc, d.QuotationId, d.FileId))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Result<QuotationDocumentBackfillResult>> EnsureRenderedAsync(
        RequestAccessContext access,
        Guid quotationId,
        string idempotencyKey,
        string traceId,
        CancellationToken cancellationToken = default)
    {
        const string operation = "quotations.render-document";
        var orgId = access.OrganizationId;
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute(quotationId.ToString("N"));

        var quotation = await db.Quotations.AsNoTracking()
            .FirstOrDefaultAsync(q => q.OrganizationId == orgId && q.Id == quotationId, cancellationToken);
        if (quotation is null || !access.HasBranchAccess(quotation.BranchId))
            return Result<QuotationDocumentBackfillResult>.Failure(new Error("RESOURCE_NOT_FOUND", "Quotation not found."));

        var strategy = db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

            var lockKey = $"{orgId:N}:{operation}:{keyHash}";
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))", cancellationToken);
            var docLockKey = $"{orgId:N}:quotation-document:{quotationId:N}";
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({docLockKey}, 0))", cancellationToken);

            var replay = await db.IdempotencyRecords.AsNoTracking()
                .FirstOrDefaultAsync(r => r.OrganizationId == orgId && r.Operation == operation && r.KeyHash == keyHash, cancellationToken);
            if (replay is not null && replay.PayloadHash != payloadHash)
            {
                return Result<QuotationDocumentBackfillResult>.Failure(
                    new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload."));
            }

            var existing = await FindSummaryAsync(orgId, quotationId, cancellationToken);
            if (existing is not null)
            {
                return Result<QuotationDocumentBackfillResult>.Success(new QuotationDocumentBackfillResult(existing, WasCreated: false));
            }

            var rendered = await writer.RenderAndStoreAsync(quotation, access.ActorUserId, traceId, cancellationToken);
            if (rendered.IsFailure)
            {
                return Result<QuotationDocumentBackfillResult>.Failure(rendered.Error);
            }

            db.IdempotencyRecords.Add(new IdempotencyRecord(
                Guid.NewGuid(), orgId, operation, keyHash, payloadHash, quotationId.ToString(), clock.UtcNow));

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return Result<QuotationDocumentBackfillResult>.Success(new QuotationDocumentBackfillResult(rendered.Value!, WasCreated: true));
        });
    }

    private async Task<QuotationDocumentFile?> LoadAsync(Guid organizationId, Guid quotationId, CancellationToken cancellationToken)
    {
        var row = await (from d in db.QuotationDocuments.AsNoTracking()
                         join f in db.UploadedFiles.AsNoTracking() on new { Id = d.FileId, d.OrganizationId } equals new { f.Id, f.OrganizationId }
                         where d.OrganizationId == organizationId && d.QuotationId == quotationId
                         select new { Document = d, f.StoragePath, f.OriginalFilename }).FirstOrDefaultAsync(cancellationToken);
        if (row is null) return null;

        await using var stream = await storage.OpenReadStreamAsync(row.StoragePath, cancellationToken)
            ?? throw new InvalidOperationException($"The stored PDF of quotation {quotationId} is missing from storage.");
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer, cancellationToken);
        var bytes = buffer.ToArray();

        // The recorded hash is the evidence, so never serve bytes that no longer match it.
        if (!string.Equals(Sha256Hex.ComputeBytes(bytes), row.Document.Sha256, StringComparison.Ordinal))
        {
            logger.LogError("The stored PDF of quotation {QuotationId} no longer matches its recorded SHA-256.", quotationId);
            throw new InvalidOperationException($"The stored PDF of quotation {quotationId} failed its integrity check.");
        }

        var summary = new QuotationDocumentSummary(row.Document.Sha256, row.Document.ByteCount, row.Document.RendererVersion, row.Document.RenderedAtUtc);
        return new QuotationDocumentFile(bytes, row.OriginalFilename, summary);
    }
}
