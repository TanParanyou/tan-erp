using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Items.Import;

namespace TanErp.Application.Items;

/// <summary>Imports Cost Records as Draft only. Publishing still goes through the existing submit/approve/publish maker-checker.</summary>
public interface ICostImportStore
{
    Task<IReadOnlyList<ItemImportRowResult>> ValidateAsync(
        RequestAccessContext access,
        IReadOnlyList<CostImportRowDraft> drafts,
        CancellationToken cancellationToken = default);

    Task<Result<ItemImportCommitResult>> CommitAsync(
        RequestAccessContext access,
        IReadOnlyList<CostImportRowDraft> drafts,
        string contentSha256,
        string idempotencyKey,
        string traceId,
        CancellationToken cancellationToken = default);

    /// <summary>Same contract as <see cref="IItemImportStore.StageBatchAsync"/>: add to the DbContext, never save.</summary>
    Task<Result<ItemImportBatchOutcome>> StageBatchAsync(
        RequestAccessContext access,
        IReadOnlyList<CostImportRowDraft> batch,
        IReadOnlySet<int> duplicateRows,
        Guid importBatchId,
        string traceId,
        CancellationToken cancellationToken = default);
}
