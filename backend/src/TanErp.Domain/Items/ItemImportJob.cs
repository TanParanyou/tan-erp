using TanErp.Domain.Common;

namespace TanErp.Domain.Items;

public static class ItemImportJobStatus
{
    public const string Queued = "queued";
    public const string Running = "running";
    public const string Completed = "completed";
    public const string CompletedWithErrors = "completed_with_errors";
    public const string Failed = "failed";

    public static readonly string[] All = [Queued, Running, Completed, CompletedWithErrors, Failed];

    public static bool IsTerminal(string status) => status is Completed or CompletedWithErrors or Failed;
}

/// <summary>
/// A large import file processed in the background, in fixed-size batches. Each batch commits atomically (all of its rows or none),
/// and the progress of a batch is saved in the same transaction as its rows so a restarted worker resumes at NextBatchIndex.
/// The file text is kept only until the job ends.
/// </summary>
public class ItemImportJob : Entity
{
    public Guid OrganizationId { get; private set; }
    public string Kind { get; private set; } = string.Empty;
    public string Mode { get; private set; } = string.Empty;
    public string Status { get; private set; } = ItemImportJobStatus.Queued;
    public string ContentSha256 { get; private set; } = string.Empty;
    public string? Content { get; private set; }
    public int TotalRows { get; private set; }
    public int BatchSize { get; private set; }
    public int TotalBatches { get; private set; }
    public int NextBatchIndex { get; private set; }
    public int ProcessedRows { get; private set; }
    public int CreatedCount { get; private set; }
    public int UpdatedCount { get; private set; }
    public int UnchangedCount { get; private set; }
    public int RejectedBatchCount { get; private set; }
    public int ErrorRowCount { get; private set; }
    public string ErrorsJson { get; private set; } = "[]";
    public string? FailureCode { get; private set; }
    public Guid RequestedByUserId { get; private set; }
    public Guid RequestedByMembershipId { get; private set; }
    public string RequestedByFirebaseUid { get; private set; } = string.Empty;
    public string IdempotencyKeyHash { get; private set; } = string.Empty;
    public string TraceId { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? StartedAtUtc { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }
    public DateTimeOffset? LeaseExpiresAtUtc { get; private set; }
    public Guid RowVersion { get; private set; }

    protected ItemImportJob() { }

    public static ItemImportJob Enqueue(
        Guid id,
        Guid organizationId,
        string kind,
        string mode,
        string contentSha256,
        string content,
        int totalRows,
        int batchSize,
        Guid requestedByUserId,
        Guid requestedByMembershipId,
        string requestedByFirebaseUid,
        string idempotencyKeyHash,
        string traceId,
        DateTimeOffset now)
    {
        if (totalRows <= 0) throw new ItemDomainException("ITEM_IMPORT_FILE_INVALID", "A job needs at least one row.");
        if (batchSize <= 0) throw new ItemDomainException("ITEM_IMPORT_FILE_INVALID", "Batch size must be positive.");

        return new ItemImportJob
        {
            Id = id,
            OrganizationId = organizationId,
            Kind = kind,
            Mode = mode,
            Status = ItemImportJobStatus.Queued,
            ContentSha256 = contentSha256,
            Content = content,
            TotalRows = totalRows,
            BatchSize = batchSize,
            TotalBatches = (totalRows + batchSize - 1) / batchSize,
            RequestedByUserId = requestedByUserId,
            RequestedByMembershipId = requestedByMembershipId,
            RequestedByFirebaseUid = requestedByFirebaseUid,
            IdempotencyKeyHash = idempotencyKeyHash,
            TraceId = traceId,
            CreatedAtUtc = now,
            RowVersion = Guid.NewGuid()
        };
    }

    /// <summary>Takes the job for a worker. A job whose lease expired (the worker died) can be taken again and resumes at NextBatchIndex.</summary>
    public bool TryClaim(DateTimeOffset now, TimeSpan lease)
    {
        var claimable = Status == ItemImportJobStatus.Queued
            || (Status == ItemImportJobStatus.Running && (LeaseExpiresAtUtc is null || LeaseExpiresAtUtc < now));
        if (!claimable) return false;

        Status = ItemImportJobStatus.Running;
        StartedAtUtc ??= now;
        LeaseExpiresAtUtc = now + lease;
        RowVersion = Guid.NewGuid();
        return true;
    }

    public void RecordApplied(int rows, int created, int updated, int unchanged, DateTimeOffset now, TimeSpan lease)
    {
        EnsureRunning();
        NextBatchIndex++;
        ProcessedRows += rows;
        CreatedCount += created;
        UpdatedCount += updated;
        UnchangedCount += unchanged;
        Touch(now, lease);
    }

    /// <summary>The batch was rolled back; its rows are counted as processed and the invalid ones are kept (up to the cap).</summary>
    public void RecordRejected(int rows, int invalidRowCount, string mergedErrorsJson, DateTimeOffset now, TimeSpan lease)
    {
        EnsureRunning();
        NextBatchIndex++;
        ProcessedRows += rows;
        RejectedBatchCount++;
        ErrorRowCount += invalidRowCount;
        ErrorsJson = mergedErrorsJson;
        Touch(now, lease);
    }

    public void Complete(DateTimeOffset now)
    {
        EnsureRunning();
        Status = RejectedBatchCount > 0 ? ItemImportJobStatus.CompletedWithErrors : ItemImportJobStatus.Completed;
        End(now);
    }

    public void Fail(string failureCode, DateTimeOffset now)
    {
        if (ItemImportJobStatus.IsTerminal(Status)) return;
        Status = ItemImportJobStatus.Failed;
        FailureCode = failureCode;
        End(now);
    }

    private void End(DateTimeOffset now)
    {
        Content = null;
        LeaseExpiresAtUtc = null;
        CompletedAtUtc = now;
        RowVersion = Guid.NewGuid();
    }

    private void Touch(DateTimeOffset now, TimeSpan lease)
    {
        LeaseExpiresAtUtc = now + lease;
        RowVersion = Guid.NewGuid();
    }

    private void EnsureRunning()
    {
        if (Status != ItemImportJobStatus.Running)
            throw new ItemDomainException("ITEM_IMPORT_JOB_INVALID_STATE", "The import job is not running.");
    }
}
