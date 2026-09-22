using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Items;
using TanErp.Domain.Common;
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
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;

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

        // Verify Branch exists in organization if branch scoped
        if (string.Equals(data.Scope, CostScopeType.Branch, StringComparison.OrdinalIgnoreCase))
        {
            if (!data.BranchId.HasValue)
            {
                return Result<CostRecordDetailProjection>.Failure(new Error("COST_BRANCH_REQUIRED", "Branch ID is required for branch scoped costs."));
            }

            var branchExists = await _db.Branches
                .AsNoTracking()
                .AnyAsync(b => b.Id == data.BranchId.Value && b.OrganizationId == orgId, ct);
            if (!branchExists)
            {
                return Result<CostRecordDetailProjection>.Failure(new Error("BRANCH_NOT_FOUND", "Branch not found."));
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

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        _db.CostRecords.Add(record);

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

        return Result<CostRecordDetailProjection>.Success(MapToProjection(record, unit, null));
    }

    public async Task<Result<CostRecordDetailProjection>> UpdateDraftAsync(
        UpdateCostRecordData data,
        RequestAccessContext access,
        Guid? ifMatch,
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

        if (ifMatch.HasValue && record.RowVersion != ifMatch.Value)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("ITEM_COST_VERSION_CONFLICT", "Cost record has been modified concurrently."));
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

        return Result<CostRecordDetailProjection>.Success(MapToProjection(record, unit, record.CostSource));
    }

    public async Task<Result<CostRecordDetailProjection>> SubmitAsync(
        Guid orgId,
        Guid itemId,
        Guid costId,
        RequestAccessContext access,
        Guid? ifMatch,
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

        if (ifMatch.HasValue && record.RowVersion != ifMatch.Value)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("ITEM_COST_VERSION_CONFLICT", "Cost record has been modified concurrently."));
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
        Guid? ifMatch,
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

        if (ifMatch.HasValue && record.RowVersion != ifMatch.Value)
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
        Guid? ifMatch,
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

        if (ifMatch.HasValue && record.RowVersion != ifMatch.Value)
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
        Guid? ifMatch,
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

        if (ifMatch.HasValue && record.RowVersion != ifMatch.Value)
        {
            await tx.RollbackAsync(ct);
            return Result<CostRecordDetailProjection>.Failure(new Error("ITEM_COST_VERSION_CONFLICT", "Cost record has been modified concurrently."));
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

        // Supersede previous published cost record with identical scope, unit, and currency
        var previousPublished = await _db.CostRecords
            .Where(c => c.OrganizationId == orgId
                && c.ItemId == itemId
                && c.Id != costId
                && c.Status == CostRecordStatus.Published
                && c.Scope == record.Scope
                && c.BranchId == record.BranchId
                && c.UnitId == record.UnitId
                && c.Currency == record.Currency
                && c.EffectiveFromUtc <= record.EffectiveFromUtc)
            .ToListAsync(ct);

        foreach (var prev in previousPublished)
        {
            prev.Supersede(access.ActorUserId, now);
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
        Guid? ifMatch,
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

        if (ifMatch.HasValue && record.RowVersion != ifMatch.Value)
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
