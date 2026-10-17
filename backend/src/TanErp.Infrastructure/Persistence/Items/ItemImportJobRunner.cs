using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Items;
using TanErp.Application.Items.Import;
using TanErp.Domain.Common;
using TanErp.Domain.Items;

namespace TanErp.Infrastructure.Persistence.Items;

/// <summary>
/// Runs one import job. The file is split into fixed-size batches; each batch commits (rows + job progress together) in one
/// Serializable transaction, so a batch is applied completely or not at all. A batch with any invalid row is rolled back and its
/// row errors are recorded; later batches still run. A worker that dies is replaced when the lease expires and resumes at the
/// next unfinished batch.
/// </summary>
public class ItemImportJobRunner : IItemImportJobRunner
{
    private const int MaxBatchAttempts = 3;
    private const string BatchConflictCode = "ITEM_IMPORT_BATCH_CONFLICT";
    private const string JobFailedCode = "ITEM_IMPORT_JOB_FAILED";
    private const string JobLostCode = "ITEM_IMPORT_JOB_LOST";

    private readonly AppDbContext _db;
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IItemImportStore _itemStore;
    private readonly ICostImportStore _costStore;
    private readonly ItemImportJobOptions _options;
    private readonly IClock _clock;
    private readonly ILogger<ItemImportJobRunner> _logger;

    public ItemImportJobRunner(
        AppDbContext db,
        IRequestAccessResolver accessResolver,
        IItemImportStore itemStore,
        ICostImportStore costStore,
        ItemImportJobOptions options,
        IClock clock,
        ILogger<ItemImportJobRunner> logger)
    {
        _db = db;
        _accessResolver = accessResolver;
        _itemStore = itemStore;
        _costStore = costStore;
        _options = options;
        _clock = clock;
        _logger = logger;
    }

    public async Task<bool> RunNextAsync(CancellationToken cancellationToken = default)
    {
        var now = _clock.UtcNow;
        var candidate = await _db.ItemImportJobs
            .Where(j => j.Status == ItemImportJobStatus.Queued
                || (j.Status == ItemImportJobStatus.Running && (j.LeaseExpiresAtUtc == null || j.LeaseExpiresAtUtc < now)))
            .OrderBy(j => j.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
        if (candidate is null) return false;

        if (!candidate.TryClaim(now, _options.EffectiveLease)) return false;
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Another worker claimed it first.
            _db.ChangeTracker.Clear();
            return true;
        }

        var jobId = candidate.Id;
        var claimToken = candidate.RowVersion;
        _db.ChangeTracker.Clear();

        try
        {
            await ProcessAsync(jobId, claimToken, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Shutting down: the lease expires and the job resumes at the next unfinished batch.
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Item import job {JobId} failed unexpectedly.", jobId);
            await FailAsync(jobId, JobFailedCode, CancellationToken.None);
        }

        return true;
    }

    private async Task ProcessAsync(Guid jobId, Guid claimToken, CancellationToken ct)
    {
        var job = await _db.ItemImportJobs.AsNoTracking().FirstAsync(j => j.Id == jobId, ct);
        if (job.Content is null)
        {
            await FailAsync(jobId, JobFailedCode, ct);
            return;
        }

        var access = await ItemImportAccess.ResolveAsync(
            _accessResolver, job.RequestedByFirebaseUid, job.RequestedByMembershipId, job.Kind, job.Mode, ct);
        if (access.IsFailure || access.Value!.OrganizationId != job.OrganizationId)
        {
            await FailAsync(jobId, access.IsFailure ? access.Error.Code : "PERMISSION_DENIED", ct);
            return;
        }

        var context = access.Value!;
        Result<Guid> finished;
        if (job.Kind == ItemImportKind.Costs)
        {
            var parsed = CostImportCsv.Parse(job.Content, ItemImportLimits.MaxJobRows, ItemImportLimits.MaxJobContentLength);
            if (parsed.IsFailure) { await FailAsync(jobId, parsed.Error.Code, ct); return; }
            var drafts = parsed.Value!;
            var duplicates = CostImportStore.FindDuplicateRows(drafts);
            finished = await RunBatchesAsync(job, claimToken, drafts,
                (slice, id) => _costStore.StageBatchAsync(context, slice, duplicates, id, job.TraceId, ct), ct);
        }
        else
        {
            var parsed = ItemImportCsv.Parse(job.Content, ItemImportLimits.MaxJobRows, ItemImportLimits.MaxJobContentLength);
            if (parsed.IsFailure) { await FailAsync(jobId, parsed.Error.Code, ct); return; }
            var drafts = parsed.Value!;
            var duplicates = ItemImportCsv.FindDuplicateCodeRows(drafts);
            finished = await RunBatchesAsync(job, claimToken, drafts,
                (slice, id) => _itemStore.StageBatchAsync(context, slice, job.Mode, duplicates, id, job.TraceId, ct), ct);
        }

        if (finished.IsFailure)
        {
            // JobLost means another worker owns the job now; it must not be marked failed from here.
            if (finished.Error.Code != JobLostCode) await FailAsync(jobId, finished.Error.Code, ct);
            return;
        }

        await CompleteAsync(jobId, finished.Value, ct);
    }

    private async Task<Result<Guid>> RunBatchesAsync<TDraft>(
        ItemImportJob job,
        Guid claimToken,
        IReadOnlyList<TDraft> drafts,
        Func<IReadOnlyList<TDraft>, Guid, Task<Result<ItemImportBatchOutcome>>> stage,
        CancellationToken ct)
    {
        var expectedToken = claimToken;
        for (var index = job.NextBatchIndex; index < job.TotalBatches; index++)
        {
            ct.ThrowIfCancellationRequested();
            var slice = drafts.Skip(index * job.BatchSize).Take(job.BatchSize).ToList();

            Result<ItemImportBatchOutcome> result = Result<ItemImportBatchOutcome>.Failure(new Error(BatchConflictCode, "The batch conflicted."));
            var pendingToken = expectedToken;
            for (var attempt = 1; attempt <= MaxBatchAttempts; attempt++)
            {
                result = await SerializableTransactionRunner.RunAsync(_db, async () =>
                {
                    var staged = await stage(slice, job.Id);
                    if (staged.IsFailure) return staged;

                    var tracked = await _db.ItemImportJobs.FirstAsync(j => j.Id == job.Id, ct);
                    if (tracked.RowVersion != expectedToken || tracked.Status != ItemImportJobStatus.Running)
                    {
                        return Result<ItemImportBatchOutcome>.Failure(new Error(JobLostCode, "The job is owned by another worker."));
                    }

                    var now = _clock.UtcNow;
                    var outcome = staged.Value!;
                    if (outcome.IsApplied)
                    {
                        tracked.RecordApplied(slice.Count, outcome.CreatedCount, outcome.UpdatedCount, outcome.UnchangedCount, now, _options.EffectiveLease);
                    }
                    else
                    {
                        tracked.RecordRejected(slice.Count, outcome.InvalidRows.Count, MergeErrors(tracked.ErrorsJson, outcome.InvalidRows), now, _options.EffectiveLease);
                    }

                    try
                    {
                        await _db.SaveChangesAsync(ct);
                    }
                    catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
                    {
                        // A concurrent create took a code after validation; the retry re-evaluates and reports it per row.
                        return Result<ItemImportBatchOutcome>.Failure(new Error(BatchConflictCode, "A concurrent change conflicted with the batch."));
                    }

                    pendingToken = tracked.RowVersion;
                    return staged;
                }, BatchConflictCode, ct);

                if (result.IsSuccess || result.Error.Code != BatchConflictCode) break;
                _db.ChangeTracker.Clear();
            }

            if (result.IsFailure) return Result<Guid>.Failure(result.Error);
            expectedToken = pendingToken;
        }

        return Result<Guid>.Success(expectedToken);
    }

    private static string MergeErrors(string existingJson, IReadOnlyList<ItemImportRowResult> invalid)
    {
        var merged = ItemImportJobStore.ReadErrors(existingJson);
        var room = ItemImportLimits.MaxReportedRowErrors - merged.Count;
        if (room > 0) merged.AddRange(invalid.Take(room));
        return JsonSerializer.Serialize(merged);
    }

    private async Task CompleteAsync(Guid jobId, Guid claimToken, CancellationToken ct)
    {
        _db.ChangeTracker.Clear();
        var job = await _db.ItemImportJobs.FirstAsync(j => j.Id == jobId, ct);
        if (job.RowVersion != claimToken) return;

        var now = _clock.UtcNow;
        job.Complete(now);
        _db.AuditEvents.Add(new AuditEvent(
            Guid.NewGuid(), job.OrganizationId, job.RequestedByUserId, "items.import-job-finished", "item-import-job", job.Id.ToString(), now,
            job.TraceId,
            JsonSerializer.Serialize(new
            {
                job.Kind, job.Mode, status = job.Status, createdCount = job.CreatedCount, updatedCount = job.UpdatedCount,
                unchangedCount = job.UnchangedCount, rejectedBatches = job.RejectedBatchCount, errorRows = job.ErrorRowCount
            }),
            actorMembershipId: job.RequestedByMembershipId));
        await _db.SaveChangesAsync(ct);
    }

    private async Task FailAsync(Guid jobId, string failureCode, CancellationToken ct)
    {
        try
        {
            _db.ChangeTracker.Clear();
            var job = await _db.ItemImportJobs.FirstOrDefaultAsync(j => j.Id == jobId, ct);
            if (job is null) return;
            job.Fail(failureCode, _clock.UtcNow);
            await _db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not record the failure of item import job {JobId}.", jobId);
        }
    }
}
