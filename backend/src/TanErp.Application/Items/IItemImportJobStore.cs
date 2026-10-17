using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Items.Import;

namespace TanErp.Application.Items;

public sealed record EnqueueItemImportJobData(
    string Kind,
    string Mode,
    string Content,
    string ContentSha256,
    int TotalRows,
    int BatchSize,
    string FirebaseUid,
    string IdempotencyKey,
    string TraceId);

public interface IItemImportJobStore
{
    /// <summary>Creates a Queued job. The same Idempotency-Key with the same file returns the existing job (Replayed).</summary>
    Task<Result<ItemImportJobSnapshot>> EnqueueAsync(RequestAccessContext access, EnqueueItemImportJobData data, CancellationToken cancellationToken = default);

    Task<ItemImportJobSnapshot?> GetAsync(Guid organizationId, Guid requestedByUserId, Guid jobId, CancellationToken cancellationToken = default);
}

/// <summary>Processes at most one queued (or lease-expired) job. Called by the background worker; returns true when a job was handled.</summary>
public interface IItemImportJobRunner
{
    Task<bool> RunNextAsync(CancellationToken cancellationToken = default);
}
