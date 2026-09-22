using TanErp.Domain.Common;

namespace TanErp.Domain.Items;

public class CostRecord : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid ItemId { get; private set; }
    public string Scope { get; private set; } = CostScopeType.Organization;
    public Guid? BranchId { get; private set; }
    public Guid UnitId { get; private set; }
    public string Currency { get; private set; } = "THB";
    public decimal Amount { get; private set; }
    public decimal MinimumQuantity { get; private set; }
    public decimal? MaximumQuantity { get; private set; }
    public DateTimeOffset EffectiveFromUtc { get; private set; }
    public DateTimeOffset? EffectiveToUtc { get; private set; }
    public string Status { get; private set; } = CostRecordStatus.Draft;
    public int Version { get; private set; } = 1;
    public Guid? CostSourceId { get; private set; }
    public string? SourceReference { get; private set; }
    public string? Reason { get; private set; }
    public Guid? EvidenceFileId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public Guid LastFinancialEditorId { get; private set; }
    public Guid? ApprovedByUserId { get; private set; }
    public Guid? PublishedByUserId { get; private set; }
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    // Navigation properties for EF Core
    public virtual Item Item { get; private set; } = null!;
    public virtual UnitOfMeasure Unit { get; private set; } = null!;
    public virtual CostSource? CostSource { get; private set; }
    public virtual ICollection<CostRecordReview> Reviews { get; private set; } = new List<CostRecordReview>();

    protected CostRecord() { }

    public static CostRecord CreateDraft(
        Guid id,
        Guid organizationId,
        Guid itemId,
        string scope,
        Guid? branchId,
        Guid unitId,
        string currency,
        decimal amount,
        decimal minimumQuantity,
        decimal? maximumQuantity,
        DateTimeOffset effectiveFromUtc,
        DateTimeOffset? effectiveToUtc,
        int version,
        Guid? costSourceId,
        string? sourceReference,
        string? reason,
        Guid? evidenceFileId,
        Guid createdByUserId,
        DateTimeOffset createdAtUtc)
    {
        ValidateFinancials(amount, currency, minimumQuantity, maximumQuantity, effectiveFromUtc, effectiveToUtc);
        ValidateScope(scope, branchId);

        if (organizationId == Guid.Empty)
            throw new ItemValidationException("COST_ORG_REQUIRED", "Organization is required.");
        if (itemId == Guid.Empty)
            throw new ItemValidationException("COST_ITEM_REQUIRED", "Item is required.");
        if (unitId == Guid.Empty)
            throw new ItemValidationException("COST_UNIT_REQUIRED", "Unit of measure is required.");
        if (createdByUserId == Guid.Empty)
            throw new ItemValidationException("COST_CREATOR_REQUIRED", "Creator user ID is required.");

        return new CostRecord
        {
            Id = id,
            OrganizationId = organizationId,
            ItemId = itemId,
            Scope = scope.ToLowerInvariant(),
            BranchId = branchId,
            UnitId = unitId,
            Currency = currency.Trim().ToUpperInvariant(),
            Amount = amount,
            MinimumQuantity = Math.Max(0m, minimumQuantity),
            MaximumQuantity = maximumQuantity,
            EffectiveFromUtc = effectiveFromUtc,
            EffectiveToUtc = effectiveToUtc,
            Status = CostRecordStatus.Draft,
            Version = Math.Max(1, version),
            CostSourceId = costSourceId,
            SourceReference = sourceReference?.Trim(),
            Reason = reason?.Trim(),
            EvidenceFileId = evidenceFileId,
            CreatedByUserId = createdByUserId,
            LastFinancialEditorId = createdByUserId,
            RowVersion = Guid.NewGuid(),
            CreatedAtUtc = createdAtUtc,
            UpdatedAtUtc = createdAtUtc
        };
    }

    public void UpdateFinancials(
        decimal amount,
        string currency,
        Guid unitId,
        decimal minimumQuantity,
        decimal? maximumQuantity,
        DateTimeOffset effectiveFromUtc,
        DateTimeOffset? effectiveToUtc,
        Guid editorUserId,
        DateTimeOffset updatedAtUtc)
    {
        EnsureMutable();
        ValidateFinancials(amount, currency, minimumQuantity, maximumQuantity, effectiveFromUtc, effectiveToUtc);

        if (unitId == Guid.Empty)
            throw new ItemValidationException("COST_UNIT_REQUIRED", "Unit of measure is required.");
        if (editorUserId == Guid.Empty)
            throw new ItemValidationException("COST_EDITOR_REQUIRED", "Editor user ID is required.");

        Amount = amount;
        Currency = currency.Trim().ToUpperInvariant();
        UnitId = unitId;
        MinimumQuantity = Math.Max(0m, minimumQuantity);
        MaximumQuantity = maximumQuantity;
        EffectiveFromUtc = effectiveFromUtc;
        EffectiveToUtc = effectiveToUtc;
        LastFinancialEditorId = editorUserId;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
    }

    public void UpdateMetadata(
        string? sourceReference,
        string? reason,
        Guid? evidenceFileId,
        Guid? costSourceId,
        Guid editorUserId,
        DateTimeOffset updatedAtUtc)
    {
        EnsureMutable();
        SourceReference = sourceReference?.Trim();
        Reason = reason?.Trim();
        EvidenceFileId = evidenceFileId;
        CostSourceId = costSourceId;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Submit(Guid actorUserId, DateTimeOffset updatedAtUtc)
    {
        if (Status != CostRecordStatus.Draft && Status != CostRecordStatus.Returned)
        {
            throw new ItemDomainException("COST_INVALID_STATUS_TRANSITION", $"Cannot submit cost record from status '{Status}'.");
        }

        Status = CostRecordStatus.Submitted;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Approve(Guid approverUserId, DateTimeOffset updatedAtUtc)
    {
        if (Status != CostRecordStatus.Submitted)
        {
            throw new ItemDomainException("COST_INVALID_STATUS_TRANSITION", $"Cannot approve cost record from status '{Status}'.");
        }

        if (approverUserId == CreatedByUserId || approverUserId == LastFinancialEditorId)
        {
            throw new ItemDomainException("MAKER_CHECKER_VIOLATION", "Maker cannot approve their own cost record.");
        }

        Status = CostRecordStatus.Approved;
        ApprovedByUserId = approverUserId;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Return(Guid reviewerUserId, string reason, DateTimeOffset updatedAtUtc)
    {
        if (Status != CostRecordStatus.Submitted)
        {
            throw new ItemDomainException("COST_INVALID_STATUS_TRANSITION", $"Cannot return cost record from status '{Status}'.");
        }

        if (reviewerUserId == CreatedByUserId || reviewerUserId == LastFinancialEditorId)
        {
            throw new ItemDomainException("MAKER_CHECKER_VIOLATION", "Maker cannot review their own cost record.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ItemValidationException("COST_RETURN_REASON_REQUIRED", "Return reason is required.");
        }

        Status = CostRecordStatus.Returned;
        Reason = reason.Trim();
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Publish(Guid publisherUserId, DateTimeOffset updatedAtUtc)
    {
        if (Status != CostRecordStatus.Approved && Status != CostRecordStatus.Submitted)
        {
            throw new ItemDomainException("COST_INVALID_STATUS_TRANSITION", $"Cannot publish cost record from status '{Status}'.");
        }

        Status = CostRecordStatus.Published;
        PublishedByUserId = publisherUserId;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Supersede(Guid actorUserId, DateTimeOffset updatedAtUtc)
    {
        if (Status != CostRecordStatus.Published)
        {
            throw new ItemDomainException("COST_INVALID_STATUS_TRANSITION", $"Cannot supersede cost record from status '{Status}'.");
        }

        Status = CostRecordStatus.Superseded;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Disable(Guid actorUserId, string reason, DateTimeOffset updatedAtUtc)
    {
        if (Status == CostRecordStatus.Disabled)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ItemValidationException("COST_DISABLE_REASON_REQUIRED", "Disable reason is required.");
        }

        Status = CostRecordStatus.Disabled;
        Reason = reason.Trim();
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
    }

    private void EnsureMutable()
    {
        if (Status == CostRecordStatus.Published || Status == CostRecordStatus.Superseded || Status == CostRecordStatus.Disabled)
        {
            throw new ItemDomainException("ITEM_COST_IMMUTABLE", $"Cost record with status '{Status}' is immutable.");
        }
    }

    private static void ValidateScope(string scope, Guid? branchId)
    {
        if (!CostScopeType.All.Contains(scope, StringComparer.OrdinalIgnoreCase))
        {
            throw new ItemValidationException("COST_SCOPE_INVALID", $"Cost scope '{scope}' is invalid.");
        }

        if (string.Equals(scope, CostScopeType.Branch, StringComparison.OrdinalIgnoreCase))
        {
            if (!branchId.HasValue || branchId.Value == Guid.Empty)
            {
                throw new ItemValidationException("COST_BRANCH_REQUIRED", "Branch ID is required for branch scoped costs.");
            }
        }
        else if (branchId.HasValue)
        {
            throw new ItemValidationException("COST_ORG_BRANCH_INVALID", "Branch ID must be null for organization scoped costs.");
        }
    }

    private static void ValidateFinancials(
        decimal amount,
        string currency,
        decimal minimumQuantity,
        decimal? maximumQuantity,
        DateTimeOffset effectiveFromUtc,
        DateTimeOffset? effectiveToUtc)
    {
        if (amount <= 0m)
        {
            throw new ItemValidationException("COST_AMOUNT_INVALID", "Cost amount must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ItemValidationException("COST_CURRENCY_REQUIRED", "Currency is required.");
        }

        if (minimumQuantity < 0m)
        {
            throw new ItemValidationException("COST_MIN_QUANTITY_INVALID", "Minimum quantity cannot be negative.");
        }

        if (maximumQuantity.HasValue && maximumQuantity.Value <= minimumQuantity)
        {
            throw new ItemValidationException("COST_QUANTITY_RANGE_INVALID", "Maximum quantity must be greater than minimum quantity.");
        }

        if (effectiveToUtc.HasValue && effectiveToUtc.Value < effectiveFromUtc)
        {
            throw new ItemValidationException("COST_EFFECTIVE_PERIOD_INVALID", "Effective to date cannot be earlier than effective from date.");
        }
    }
}
