using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Items.Import;

namespace TanErp.Application.Items;

public interface IItemImportStore
{
    /// <summary>Resolves master-data codes and duplicates for each row without writing anything.</summary>
    Task<IReadOnlyList<ItemImportRowResult>> ValidateAsync(
        RequestAccessContext access,
        IReadOnlyList<ItemImportRowDraft> drafts,
        string mode,
        CancellationToken cancellationToken = default);

    /// <summary>Applies every row (create, or create/update in upsert mode) in one transaction, or nothing at all.</summary>
    Task<Result<ItemImportCommitResult>> CommitAsync(
        RequestAccessContext access,
        IReadOnlyList<ItemImportRowDraft> drafts,
        string mode,
        string contentSha256,
        string idempotencyKey,
        string traceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Evaluates one batch and, only when every row is valid, adds its changes to the current DbContext WITHOUT saving.
    /// The caller owns the transaction and SaveChanges, so batch rows and job progress commit together.
    /// <paramref name="duplicateRows"/> is computed once over the whole file so a duplicate code across batches is still caught.
    /// </summary>
    Task<Result<ItemImportBatchOutcome>> StageBatchAsync(
        RequestAccessContext access,
        IReadOnlyList<ItemImportRowDraft> batch,
        string mode,
        IReadOnlySet<int> duplicateRows,
        Guid importBatchId,
        string traceId,
        CancellationToken cancellationToken = default);
}
