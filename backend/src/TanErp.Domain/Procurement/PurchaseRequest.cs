using TanErp.Domain.Common;

namespace TanErp.Domain.Procurement;

public class PurchaseRequest : Entity
{
    private readonly List<PurchaseRequestLine> _lines = new();

    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid? ProjectId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public string Status { get; private set; } = PurchaseRequestStatus.Draft;
    public string Currency { get; private set; } = "THB";
    public decimal TotalEstimatedAmount { get; private set; }
    public DateOnly? RequiredByDate { get; private set; }
    public string? Note { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? SubmittedAtUtc { get; private set; }
    public Guid? DecidedByUserId { get; private set; }
    public DateTimeOffset? DecidedAtUtc { get; private set; }
    public string? DecisionNote { get; private set; }
    public string? CancelReason { get; private set; }
    public Guid? ConvertedPurchaseOrderId { get; private set; }
    public DateTimeOffset? ConvertedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<PurchaseRequestLine> Lines => _lines.AsReadOnly();

    protected PurchaseRequest() { }

    public PurchaseRequest(
        Guid id,
        Guid organizationId,
        Guid branchId,
        Guid? projectId,
        string number,
        DateOnly? requiredByDate,
        string? note,
        Guid createdByUserId,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (branchId == Guid.Empty) throw new ArgumentException("Branch ID cannot be empty.", nameof(branchId));
        if (createdByUserId == Guid.Empty) throw new ArgumentException("Created by user ID cannot be empty.", nameof(createdByUserId));
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Purchase request number cannot be blank.", nameof(number));

        OrganizationId = organizationId;
        BranchId = branchId;
        ProjectId = projectId;
        Number = number.Trim();
        Status = PurchaseRequestStatus.Draft;
        RequiredByDate = requiredByDate;
        Note = CleanNote(note);
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
        UpdatedAtUtc = CreatedAtUtc;
        RowVersion = Guid.NewGuid();
    }

    public void AddLine(PurchaseRequestLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        if (_lines.Count >= 200)
        {
            throw new ProcurementDomainException("PURCHASE_REQUEST_LINE_INVALID", "A purchase request can have at most 200 lines.");
        }

        if (_lines.Any(l => l.ItemId == line.ItemId))
        {
            throw new ProcurementDomainException("PURCHASE_REQUEST_LINE_INVALID", "Each item may appear only once on a purchase request.");
        }

        _lines.Add(line);
        TotalEstimatedAmount = decimal.Round(_lines.Sum(l => l.EstimatedLineTotal), 2, MidpointRounding.AwayFromZero);
    }

    public void EditDraft(Guid? projectId, DateOnly? requiredByDate, string? note, DateTimeOffset now)
    {
        EnsureStatus(PurchaseRequestStatus.Draft, "Only a draft purchase request can be edited.");
        ProjectId = projectId;
        RequiredByDate = requiredByDate;
        Note = CleanNote(note);
        Touch(now);
    }

    public void ReplaceLines(IEnumerable<PurchaseRequestLine> lines)
    {
        EnsureStatus(PurchaseRequestStatus.Draft, "Only a draft purchase request can be edited.");
        var materialized = lines.ToList();
        ValidateLineCollection(materialized);
        _lines.Clear();
        _lines.AddRange(materialized);
        TotalEstimatedAmount = decimal.Round(_lines.Sum(l => l.EstimatedLineTotal), 2, MidpointRounding.AwayFromZero);
    }

    public void Submit(DateTimeOffset now)
    {
        EnsureStatus(PurchaseRequestStatus.Draft, $"Only a draft purchase request can be submitted; current status is '{Status}'.");
        ValidateLineCollection(_lines);

        Status = PurchaseRequestStatus.Submitted;
        SubmittedAtUtc = now.ToUniversalTime();
        Touch(now);
    }

    /// <summary>Maker-checker: whoever created the purchase request cannot approve or reject it.</summary>
    public void Decide(bool approve, Guid deciderUserId, string? note, DateTimeOffset now)
    {
        EnsureStatus(PurchaseRequestStatus.Submitted, $"Only a submitted purchase request can be decided; current status is '{Status}'.");
        if (deciderUserId == CreatedByUserId)
        {
            throw new ProcurementDomainException("PURCHASE_REQUEST_SELF_APPROVAL", "A purchase request cannot be decided by its creator.");
        }

        if (!approve && string.IsNullOrWhiteSpace(note))
        {
            throw new ProcurementDomainException("PROCUREMENT_REASON_REQUIRED", "A note is required to reject a purchase request.");
        }

        Status = approve ? PurchaseRequestStatus.Approved : PurchaseRequestStatus.Rejected;
        DecidedByUserId = deciderUserId;
        DecidedAtUtc = now.ToUniversalTime();
        DecisionNote = CleanNote(note);
        Touch(now);
    }

    public void Cancel(string reason, DateTimeOffset now)
    {
        if (Status is not (PurchaseRequestStatus.Draft or PurchaseRequestStatus.Submitted or PurchaseRequestStatus.Approved))
        {
            throw new ProcurementDomainException("PURCHASE_REQUEST_INVALID_STATE", $"A {Status} purchase request cannot be cancelled.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ProcurementDomainException("PROCUREMENT_REASON_REQUIRED", "A reason is required to cancel a purchase request.");
        }

        var trimmed = reason.Trim();
        if (trimmed.Length > 500)
        {
            throw new ProcurementDomainException("PROCUREMENT_FIELD_INVALID", "Cancel reason cannot exceed 500 characters.");
        }

        Status = PurchaseRequestStatus.Cancelled;
        CancelReason = trimmed;
        Touch(now);
    }

    public void MarkConverted(Guid purchaseOrderId, DateTimeOffset now)
    {
        EnsureStatus(PurchaseRequestStatus.Approved, $"Only an approved purchase request can be converted to a purchase order; current status is '{Status}'.");
        if (purchaseOrderId == Guid.Empty)
        {
            throw new ArgumentException("Purchase order ID cannot be empty.", nameof(purchaseOrderId));
        }

        Status = PurchaseRequestStatus.Converted;
        ConvertedPurchaseOrderId = purchaseOrderId;
        ConvertedAtUtc = now.ToUniversalTime();
        Touch(now);
    }

    private static void ValidateLineCollection(IReadOnlyCollection<PurchaseRequestLine> lines)
    {
        if (lines.Count is < 1 or > 200)
        {
            throw new ProcurementDomainException("PURCHASE_REQUEST_LINE_INVALID", "A purchase request must have between 1 and 200 lines.");
        }

        if (lines.Select(l => l.ItemId).Distinct().Count() != lines.Count)
        {
            throw new ProcurementDomainException("PURCHASE_REQUEST_LINE_INVALID", "Each item may appear only once on a purchase request.");
        }
    }

    private void EnsureStatus(string expected, string message)
    {
        if (Status != expected)
        {
            throw new ProcurementDomainException("PURCHASE_REQUEST_INVALID_STATE", message);
        }
    }

    private static string? CleanNote(string? note)
    {
        var trimmed = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (trimmed is { Length: > 500 })
        {
            throw new ProcurementDomainException("PURCHASE_REQUEST_LINE_INVALID", "Note cannot exceed 500 characters.");
        }

        return trimmed;
    }

    private void Touch(DateTimeOffset now)
    {
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }
}
