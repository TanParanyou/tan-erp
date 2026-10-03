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
        CancellationToken cancellationToken = default);

    /// <summary>Creates every row as a Draft Item in one transaction, or nothing at all.</summary>
    Task<Result<ItemImportCommitResult>> CommitAsync(
        RequestAccessContext access,
        IReadOnlyList<ItemImportRowDraft> drafts,
        string contentSha256,
        string idempotencyKey,
        string traceId,
        CancellationToken cancellationToken = default);
}
