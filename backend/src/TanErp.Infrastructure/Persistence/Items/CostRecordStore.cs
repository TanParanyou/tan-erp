using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Items;
using TanErp.Domain.Common;
using TanErp.Domain.Files;
using TanErp.Domain.Items;

namespace TanErp.Infrastructure.Persistence.Items;

public class CostRecordStore : ICostRecordStore
{
    private readonly AppDbContext _db;

    public CostRecordStore(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<CostRecordDetailProjection>> CreateDraftAsync(
        CreateCostRecordData data,
        RequestAccessContext access,
        string idempotencyKey,
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        const string operation = "item-costs.create";
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute(JsonSerializer.Serialize(new { data, access.ActorUserId }));
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        var replay = await LockAndFindReplayAsync(orgId, data.ItemId, operation, keyHash, ct);
        if (replay != null)
        {
            if (replay.PayloadHash != payloadHash)
                return Result<CostRecordDetailProjection>.Failure(new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key was used with different cost data."));

            var replayId = Guid.Parse(replay.ResourceId);
            var existing = await _db.CostRecords.AsNoTracking()
                .Include(c => c.Unit).Include(c => c.CostSource)
                .FirstOrDefaultAsync(c => c.Id == replayId && c.OrganizationId == orgId && c.ItemId == data.ItemId, ct);
            if (existing != null)
                return Result<CostRecordDetailProjection>.Success(MapToProjection(existing, existing.Unit, existing.CostSource));
        }

        // Verify Item exists in organization
        var item = await _db.Items
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == data.ItemId && i.OrganizationId == orgId, ct);
        if (item == null)
        {
            return Result<CostRecordDetailProjection>.Failure(new Error("ITEM_NOT_FOUND", "Item not found."));
        }

        // Verify Unit exists in organization
        var unit = await _db.Units
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == data.UnitId && u.OrganizationId == orgId, ct);
        if (unit == null)
        {
            return Result<CostRecordDetailProjection>.Failure(new Error("UNIT_OF_MEASURE_NOT_FOUND", "Unit of measure not found."));
        }

        if (data.CostSourceId is not Guid sourceId)
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_SOURCE_REQUIRED", "A manual cost source is required."));
        var costSource = await _db.CostSources.AsNoTracking().FirstOrDefaultAsync(s => s.Id == sourceId && s.OrganizationId == orgId, ct);
        if (costSource is null)
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_SOURCE_NOT_FOUND", "Cost source not found."));
        if (!costSource.IsActive || costSource.SourceType != CostSource.ManualType)
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_SOURCE_INACTIVE", "An active manual cost source is required."));
        if (string.IsNullOrWhiteSpace(data.Reason) || (string.IsNullOrWhiteSpace(data.SourceReference) && !data.EvidenceFileId.HasValue))
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_RECORD_PROVENANCE_REQUIRED", "Reason and a source reference or verified evidence are required."));
        if (data.EvidenceFileId.HasValue)
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_EVIDENCE_INVALID", "Evidence must be uploaded and attached to the saved cost record."));

        // Verify Branch exists in organization if branch scoped
        if (string.Equals(data.Scope, CostScopeType.Branch, StringComparison.OrdinalIgnoreCase))
        {
            if (!data.BranchId.HasValue)
            {
                return Result<CostRecordDetailProjection>.Failure(new Error("COST_BRANCH_REQUIRED", "Branch ID is required for branch scoped costs."));
            }

            var branch = await _db.Branches
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == data.BranchId.Value && b.OrganizationId == orgId, ct);
            if (branch == null)
            {
                return Result<CostRecordDetailProjection>.Failure(new Error("BRANCH_NOT_FOUND", "Branch not found."));
            }
            if (!branch.IsActive)
            {
                return Result<CostRecordDetailProjection>.Failure(new Error("BRANCH_INACTIVE", "Branch is inactive."));
            }
        }

        // Determine next version for this item
        var maxVersion = await _db.CostRecords
            .Where(c => c.OrganizationId == orgId && c.ItemId == data.ItemId)
            .Select(c => (int?)c.Version)
            .MaxAsync(ct) ?? 0;

        var now = DateTimeOffset.UtcNow;
        var costId = Guid.NewGuid();

        CostRecord record;
        try
        {
            record = CostRecord.CreateDraft(
                costId,
                orgId,
                data.ItemId,
                data.Scope,
                data.BranchId,
                data.UnitId,
                data.Currency,
                data.Amount,
                data.MinimumQuantity,
                data.MaximumQuantity,
                data.EffectiveFromUtc,
                data.EffectiveToUtc,
                maxVersion + 1,
                data.CostSourceId,
                data.SourceReference,
                data.Reason,
                data.EvidenceFileId,
                access.ActorUserId,
                now);
        }
        catch (ItemValidationException ex)
        {
            return Result<CostRecordDetailProjection>.Failure(new Error(ex.Code, ex.Message));
        }

        _db.CostRecords.Add(record);
        _db.IdempotencyRecords.Add(new IdempotencyRecord(
            Guid.NewGuid(), orgId, operation, keyHash, payloadHash, record.Id.ToString(), now));

        var audit = new AuditEvent(
            Guid.NewGuid(),
            orgId,
            access.ActorUserId,
            "items.create-cost-draft",
            "cost_record",
            record.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new { data.ItemId, record.Amount, record.Currency, record.Scope, record.Version }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null,
            rowVersionAfter: record.RowVersion);

        _db.AuditEvents.Add(audit);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return Result<CostRecordDetailProjection>.Success(MapToProjection(record, unit, costSource));
    }

    public async Task<Result<CostRecordDetailProjection>> UpdateDraftAsync(
        UpdateCostRecordData data,
        RequestAccessContext access,
        Guid ifMatch,
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var record = await _db.CostRecords
            .Include(c => c.Unit)
            .Include(c => c.CostSource)
            .FirstOrDefaultAsync(c => c.Id == data.CostId && c.OrganizationId == orgId && c.ItemId == data.ItemId, ct);

        if (record == null)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_RECORD_NOT_FOUND", "Cost record not found."));
        }

        if (record.RowVersion != ifMatch)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("ITEM_COST_VERSION_CONFLICT", "Cost record has been modified concurrently."));
        }

        var sourceId = data.CostSourceId;
        if (!sourceId.HasValue)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_SOURCE_REQUIRED", "A manual cost source is required."));
        }
        var source = await _db.CostSources.AsNoTracking().FirstOrDefaultAsync(s => s.Id == sourceId.Value && s.OrganizationId == orgId, ct);
        if (source is null)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_SOURCE_NOT_FOUND", "Cost source not found."));
        }
        if (!source.IsActive || source.SourceType != CostSource.ManualType)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_SOURCE_INACTIVE", "An active manual cost source is required."));
        }
        if (string.IsNullOrWhiteSpace(data.Reason) || (string.IsNullOrWhiteSpace(data.SourceReference) && !data.EvidenceFileId.HasValue))
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_RECORD_PROVENANCE_REQUIRED", "Reason and a source reference or verified evidence are required."));
        }
        if (data.EvidenceFileId.HasValue && !await IsVerifiedCostEvidenceAsync(data.EvidenceFileId.Value, record.Id, orgId, ct))
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_EVIDENCE_INVALID", "Evidence must be verified and attached to this cost record."));
        }

        var unit = await _db.Units
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == data.UnitId && u.OrganizationId == orgId, ct);
        if (unit == null)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("UNIT_OF_MEASURE_NOT_FOUND", "Unit of measure not found."));
        }

        var now = DateTimeOffset.UtcNow;
        try
        {
            record.UpdateFinancials(
                data.Amount,
                data.Currency,
                data.UnitId,
                data.MinimumQuantity,
                data.MaximumQuantity,
                data.EffectiveFromUtc,
                data.EffectiveToUtc,
                access.ActorUserId,
                now);

            record.UpdateMetadata(
                data.SourceReference,
                data.Reason,
                data.EvidenceFileId,
                data.CostSourceId,
                access.ActorUserId,
                now);
        }
        catch (ItemDomainException ex)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error(ex.Code, ex.Message));
        }

        var audit = new AuditEvent(
            Guid.NewGuid(),
            orgId,
            access.ActorUserId,
            "items.update-cost-draft",
            "cost_record",
            record.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new { data.Amount, data.Currency, data.UnitId }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null,
            rowVersionAfter: record.RowVersion);

        _db.AuditEvents.Add(audit);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return Result<CostRecordDetailProjection>.Success(MapToProjection(record, unit, source));
    }

    private async Task<bool> IsVerifiedCostEvidenceAsync(Guid fileId, Guid costRecordId, Guid organizationId, CancellationToken ct)
    {
        return await _db.UploadedFiles.AsNoTracking().AnyAsync(file =>
            file.Id == fileId && file.OrganizationId == organizationId && file.Status == UploadedFileStatus.Verified &&
            (file.ScanStatus == FileScanStatus.ContentVerified || file.ScanStatus == FileScanStatus.Clean) && file.VerifiedAtUtc != null &&
            _db.FileUploadSessions.Any(session => session.Id.ToString() == file.UploadSessionId &&
                session.OrganizationId == organizationId && session.ParentType == FileParentTypes.CostRecord && session.ParentId == costRecordId && session.Status == FileUploadSessionStatus.Consumed), ct);
    }

    public async Task<Result<CostRecordDetailProjection>> SubmitAsync(
        Guid orgId,
        Guid itemId,
        Guid costId,
        RequestAccessContext access,
        Guid ifMatch,
        CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var record = await _db.CostRecords
            .Include(c => c.Unit)
            .Include(c => c.CostSource)
            .FirstOrDefaultAsync(c => c.Id == costId && c.OrganizationId == orgId && c.ItemId == itemId, ct);

        if (record == null)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_RECORD_NOT_FOUND", "Cost record not found."));
        }

        if (record.RowVersion != ifMatch)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("ITEM_COST_VERSION_CONFLICT", "Cost record has been modified concurrently."));
        }

        if (record.CostSource is not { IsActive: true, SourceType: CostSource.ManualType }
            || string.IsNullOrWhiteSpace(record.Reason)
            || (string.IsNullOrWhiteSpace(record.SourceReference) && !record.EvidenceFileId.HasValue))
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_RECORD_PROVENANCE_REQUIRED", "A valid manual source, reason, and source reference or verified evidence are required."));
        }
        if (record.EvidenceFileId.HasValue && !await IsVerifiedCostEvidenceAsync(record.EvidenceFileId.Value, record.Id, orgId, ct))
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_EVIDENCE_INVALID", "Evidence must be verified and attached to this cost record."));
        }

        var now = DateTimeOffset.UtcNow;
        try
        {
            record.Submit(access.ActorUserId, now);
        }
        catch (ItemDomainException ex)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error(ex.Code, ex.Message));
        }

        var audit = new AuditEvent(
            Guid.NewGuid(),
            orgId,
            access.ActorUserId,
            "items.submit-cost",
            "cost_record",
            record.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new { costId, status = record.Status }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null,
            rowVersionAfter: record.RowVersion);

        _db.AuditEvents.Add(audit);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return Result<CostRecordDetailProjection>.Success(MapToProjection(record, record.Unit, record.CostSource));
    }

    public async Task<Result<CostRecordDetailProjection>> ApproveAsync(
        Guid orgId,
        Guid itemId,
        Guid costId,
        RequestAccessContext access,
        Guid ifMatch,
        CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var record = await _db.CostRecords
            .Include(c => c.Unit)
            .Include(c => c.CostSource)
            .FirstOrDefaultAsync(c => c.Id == costId && c.OrganizationId == orgId && c.ItemId == itemId, ct);

        if (record == null)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_RECORD_NOT_FOUND", "Cost record not found."));
        }

        if (record.RowVersion != ifMatch)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("ITEM_COST_VERSION_CONFLICT", "Cost record has been modified concurrently."));
        }

        var now = DateTimeOffset.UtcNow;
        try
        {
            record.Approve(access.ActorUserId, now);
        }
        catch (ItemDomainException ex)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error(ex.Code, ex.Message));
        }

        var review = new CostRecordReview(
            Guid.NewGuid(),
            orgId,
            record.Id,
            "approved",
            null,
            access.ActorUserId,
            authoritySnapshot: $"user:{access.ActorUserId},membership:{access.MembershipId}",
            now);

        _db.CostRecordReviews.Add(review);

        var audit = new AuditEvent(
            Guid.NewGuid(),
            orgId,
            access.ActorUserId,
            "items.approve-cost",
            "cost_record",
            record.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new { costId, approvedBy = access.ActorUserId }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null,
            rowVersionAfter: record.RowVersion);

        _db.AuditEvents.Add(audit);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return Result<CostRecordDetailProjection>.Success(MapToProjection(record, record.Unit, record.CostSource));
    }

    public async Task<Result<CostRecordDetailProjection>> ReturnAsync(
        Guid orgId,
        Guid itemId,
        Guid costId,
        string reason,
        RequestAccessContext access,
        Guid ifMatch,
        CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var record = await _db.CostRecords
            .Include(c => c.Unit)
            .Include(c => c.CostSource)
            .FirstOrDefaultAsync(c => c.Id == costId && c.OrganizationId == orgId && c.ItemId == itemId, ct);

        if (record == null)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_RECORD_NOT_FOUND", "Cost record not found."));
        }

        if (record.RowVersion != ifMatch)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("ITEM_COST_VERSION_CONFLICT", "Cost record has been modified concurrently."));
        }

        var now = DateTimeOffset.UtcNow;
        try
        {
            record.Return(access.ActorUserId, reason, now);
        }
        catch (ItemDomainException ex)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error(ex.Code, ex.Message));
        }

        var review = new CostRecordReview(
            Guid.NewGuid(),
            orgId,
            record.Id,
            "returned",
            reason,
            access.ActorUserId,
            authoritySnapshot: $"user:{access.ActorUserId},membership:{access.MembershipId}",
            now);

        _db.CostRecordReviews.Add(review);

        var audit = new AuditEvent(
            Guid.NewGuid(),
            orgId,
            access.ActorUserId,
            "items.return-cost",
            "cost_record",
            record.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new { costId, reason }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null,
            rowVersionAfter: record.RowVersion);

        _db.AuditEvents.Add(audit);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return Result<CostRecordDetailProjection>.Success(MapToProjection(record, record.Unit, record.CostSource));
    }

    public async Task<Result<CostRecordDetailProjection>> PublishAsync(
        Guid orgId,
        Guid itemId,
        Guid costId,
        RequestAccessContext access,
        Guid ifMatch,
        string idempotencyKey,
        CancellationToken ct)
    {
        const string operation = "item-costs.publish";
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute(JsonSerializer.Serialize(new { costId, ifMatch, access.ActorUserId }));
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        var replay = await LockAndFindReplayAsync(orgId, itemId, operation, keyHash, ct);
        if (replay != null)
        {
            if (replay.PayloadHash != payloadHash)
                return Result<CostRecordDetailProjection>.Failure(new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key was used with a different publication."));

            var publishedReplay = await _db.CostRecords.AsNoTracking()
                .Include(c => c.Unit).Include(c => c.CostSource)
                .FirstOrDefaultAsync(c => c.Id == costId && c.OrganizationId == orgId && c.ItemId == itemId, ct);
            if (publishedReplay != null)
                return Result<CostRecordDetailProjection>.Success(MapToProjection(publishedReplay, publishedReplay.Unit, publishedReplay.CostSource));
        }

        var record = await _db.CostRecords
            .Include(c => c.Unit)
            .Include(c => c.CostSource)
            .FirstOrDefaultAsync(c => c.Id == costId && c.OrganizationId == orgId && c.ItemId == itemId, ct);

        if (record == null)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_RECORD_NOT_FOUND", "Cost record not found."));
        }

        if (record.RowVersion != ifMatch)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("ITEM_COST_VERSION_CONFLICT", "Cost record has been modified concurrently."));
        }

        var published = await _db.CostRecords
            .Where(c => c.OrganizationId == orgId
                && c.ItemId == itemId
                && c.Id != costId
                && c.Status == CostRecordStatus.Published
                && c.Scope == record.Scope
                && c.BranchId == record.BranchId
                && c.UnitId == record.UnitId
                && c.Currency == record.Currency)
            .ToListAsync(ct);

        var overlapping = published.Where(c => CostPublicationPolicy.Overlaps(record, c)).ToList();
        var conflicting = overlapping.Where(c => !CostPublicationPolicy.CanSupersede(record, c)).ToList();

        if (conflicting.Count > 0)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_RECORD_DATE_OVERLAP", "A published cost record overlaps the effective and quantity ranges."));
        }

        var now = DateTimeOffset.UtcNow;
        try
        {
            record.Publish(access.ActorUserId, now);
        }
        catch (ItemDomainException ex)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error(ex.Code, ex.Message));
        }

        var previousPublished = overlapping;

        foreach (var prev in previousPublished)
        {
            prev.Supersede(access.ActorUserId, record.EffectiveFromUtc, now);
        }

        var audit = new AuditEvent(
            Guid.NewGuid(),
            orgId,
            access.ActorUserId,
            "items.publish-cost",
            "cost_record",
            record.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new { costId, version = record.Version, supersededCount = previousPublished.Count }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null,
            rowVersionAfter: record.RowVersion);

        _db.AuditEvents.Add(audit);
        _db.IdempotencyRecords.Add(new IdempotencyRecord(
            Guid.NewGuid(), orgId, operation, keyHash, payloadHash, costId.ToString(), now));

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return Result<CostRecordDetailProjection>.Success(MapToProjection(record, record.Unit, record.CostSource));
    }

    public async Task<Result<CostRecordDetailProjection>> DisableAsync(
        Guid orgId,
        Guid itemId,
        Guid costId,
        string reason,
        RequestAccessContext access,
        Guid ifMatch,
        CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var record = await _db.CostRecords
            .Include(c => c.Unit)
            .Include(c => c.CostSource)
            .FirstOrDefaultAsync(c => c.Id == costId && c.OrganizationId == orgId && c.ItemId == itemId, ct);

        if (record == null)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("COST_RECORD_NOT_FOUND", "Cost record not found."));
        }

        if (record.RowVersion != ifMatch)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("ITEM_COST_VERSION_CONFLICT", "Cost record has been modified concurrently."));
        }

        var now = DateTimeOffset.UtcNow;
        try
        {
            record.Disable(access.ActorUserId, reason, now);
        }
        catch (ItemDomainException ex)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error(ex.Code, ex.Message));
        }

        var audit = new AuditEvent(
            Guid.NewGuid(),
            orgId,
            access.ActorUserId,
            "items.disable-cost",
            "cost_record",
            record.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new { costId, reason }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null,
            rowVersionAfter: record.RowVersion);

        _db.AuditEvents.Add(audit);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return Result<CostRecordDetailProjection>.Success(MapToProjection(record, record.Unit, record.CostSource));
    }

    public async Task<CostRecordDetailProjection?> GetByIdAsync(Guid orgId, Guid itemId, Guid costId, CancellationToken ct)
    {
        var record = await _db.CostRecords
            .AsNoTracking()
            .Include(c => c.Unit)
            .Include(c => c.CostSource)
            .FirstOrDefaultAsync(c => c.Id == costId && c.OrganizationId == orgId && c.ItemId == itemId, ct);

        return record == null ? null : MapToProjection(record, record.Unit, record.CostSource);
    }

    public async Task<IReadOnlyList<CostRecordDetailProjection>> ListForItemAsync(Guid orgId, Guid itemId, CancellationToken ct)
    {
        var records = await _db.CostRecords
            .AsNoTracking()
            .Include(c => c.Unit)
            .Include(c => c.CostSource)
            .Where(c => c.OrganizationId == orgId && c.ItemId == itemId)
            .OrderByDescending(c => c.Version)
            .ToListAsync(ct);

        return records.Select(r => MapToProjection(r, r.Unit, r.CostSource)).ToList();
    }

    private async Task<IdempotencyRecord?> LockAndFindReplayAsync(
        Guid orgId, Guid itemId, string operation, string keyHash, CancellationToken ct)
    {
        var idempotencyLock = $"{orgId:N}:{operation}:{keyHash}";
        await _db.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT pg_advisory_xact_lock(hashtextextended({idempotencyLock}, 0))", ct);
        await _db.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT pg_advisory_xact_lock(hashtextextended({itemId.ToString()}, 0))", ct);

        return await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(r =>
            r.OrganizationId == orgId && r.Operation == operation && r.KeyHash == keyHash, ct);
    }

    private static CostRecordDetailProjection MapToProjection(CostRecord c, UnitOfMeasure u, CostSource? cs)
    {
        return new CostRecordDetailProjection(
            c.Id,
            c.OrganizationId,
            c.ItemId,
            c.Scope,
            c.BranchId,
            c.UnitId,
            u.Name,
            c.Currency,
            c.Amount,
            c.MinimumQuantity,
            c.MaximumQuantity,
            c.EffectiveFromUtc,
            c.EffectiveToUtc,
            c.Status,
            c.Version,
            c.CostSourceId,
            cs?.Name,
            c.SourceReference,
            c.Reason,
            c.EvidenceFileId,
            c.CreatedByUserId,
            c.LastFinancialEditorId,
            c.ApprovedByUserId,
            c.PublishedByUserId,
            c.RowVersion,
            c.CreatedAtUtc,
            c.UpdatedAtUtc);
    }
}
