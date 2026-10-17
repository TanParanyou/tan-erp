using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Items;
using TanErp.Application.Items.Import;
using TanErp.Domain.Common;
using TanErp.Domain.Items;

namespace TanErp.Infrastructure.Persistence.Items;

public class ItemImportJobStore : IItemImportJobStore
{
    private const int MaxTraceIdLength = 128;

    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public ItemImportJobStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result<ItemImportJobSnapshot>> EnqueueAsync(
        RequestAccessContext access, EnqueueItemImportJobData data, CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        var keyHash = Sha256Hex.Compute(data.IdempotencyKey);

        var existing = await FindByKeyAsync(orgId, keyHash, cancellationToken);
        if (existing is not null) return Replay(existing, data);

        var traceId = data.TraceId.Length > MaxTraceIdLength ? data.TraceId[..MaxTraceIdLength] : data.TraceId;
        var job = ItemImportJob.Enqueue(
            Guid.NewGuid(), orgId, data.Kind, data.Mode, data.ContentSha256, data.Content, data.TotalRows, data.BatchSize,
            access.ActorUserId, access.MembershipId, data.FirebaseUid, keyHash, traceId, _clock.UtcNow);
        _db.ItemImportJobs.Add(job);
        _db.AuditEvents.Add(new AuditEvent(
            Guid.NewGuid(), orgId, access.ActorUserId, "items.import-job-queued", "item-import-job", job.Id.ToString(), job.CreatedAtUtc, traceId,
            JsonSerializer.Serialize(new { job.Kind, job.Mode, job.TotalRows, job.BatchSize, contentSha256 = job.ContentSha256 }),
            branchId: access.BranchId, actorMembershipId: access.MembershipId));

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // A concurrent request with the same Idempotency-Key won the unique index; replay its job, otherwise this is a real failure.
            var raced = await TryFindAfterConflictAsync(orgId, keyHash, cancellationToken);
            if (raced is null) throw;
            return Replay(raced, data);
        }

        return Result<ItemImportJobSnapshot>.Success(ToSnapshot(job, replayed: false));
    }

    public async Task<ItemImportJobSnapshot?> GetAsync(Guid organizationId, Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await _db.ItemImportJobs.AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId && j.OrganizationId == organizationId, cancellationToken);
        return job is null ? null : ToSnapshot(job, replayed: false);
    }

    private Task<ItemImportJob?> FindByKeyAsync(Guid orgId, string keyHash, CancellationToken ct) =>
        _db.ItemImportJobs.AsNoTracking().FirstOrDefaultAsync(j => j.OrganizationId == orgId && j.IdempotencyKeyHash == keyHash, ct);

    private async Task<ItemImportJob?> TryFindAfterConflictAsync(Guid orgId, string keyHash, CancellationToken ct)
    {
        _db.ChangeTracker.Clear();
        return await FindByKeyAsync(orgId, keyHash, ct);
    }

    private static Result<ItemImportJobSnapshot> Replay(ItemImportJob existing, EnqueueItemImportJobData data) =>
        existing.ContentSha256 != data.ContentSha256 || existing.Kind != data.Kind || existing.Mode != data.Mode
            ? Result<ItemImportJobSnapshot>.Failure(new Error("IDEMPOTENCY_KEY_REUSED", "Idempotency key was reused with a different payload."))
            : Result<ItemImportJobSnapshot>.Success(ToSnapshot(existing, replayed: true));

    internal static ItemImportJobSnapshot ToSnapshot(ItemImportJob job, bool replayed)
    {
        var errors = ReadErrors(job.ErrorsJson);
        return new ItemImportJobSnapshot(
            job.Id, job.Kind, job.Mode, job.Status, job.ContentSha256, job.TotalRows, job.ProcessedRows, job.BatchSize,
            job.TotalBatches, job.NextBatchIndex - job.RejectedBatchCount, job.RejectedBatchCount, job.CreatedCount,
            job.UpdatedCount, job.UnchangedCount, job.ErrorRowCount, job.ErrorRowCount > errors.Count, errors, job.FailureCode,
            job.CreatedAtUtc, job.StartedAtUtc, job.CompletedAtUtc, replayed);
    }

    internal static List<ItemImportRowResult> ReadErrors(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<List<ItemImportRowResult>>(json) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }
}
