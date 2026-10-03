using TanErp.Domain.Common;

namespace TanErp.Domain.Production;

/// <summary>Production bill of materials of one produced item. Separate from an Estimate BOQ; changes go through revisions.</summary>
public class Bom : Entity
{
    private readonly List<BomRevision> _revisions = new();

    public Guid OrganizationId { get; private set; }
    public Guid ItemId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string NormalizedCode { get; private set; } = string.Empty;
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<BomRevision> Revisions => _revisions.AsReadOnly();

    protected Bom() { }

    public Bom(Guid id, Guid organizationId, Guid itemId, string code, Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || itemId == Guid.Empty || createdByUserId == Guid.Empty) throw new ArgumentException("Organization, item and actor are required.");
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("BOM code cannot be blank.", nameof(code));

        OrganizationId = organizationId;
        ItemId = itemId;
        Code = code.Trim();
        NormalizedCode = Code.ToUpperInvariant();
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
    }

    public void AddRevision(BomRevision revision)
    {
        ArgumentNullException.ThrowIfNull(revision);
        _revisions.Add(revision);
    }
}

public class BomRevision : Entity
{
    private readonly List<BomLine> _lines = new();

    public Guid OrganizationId { get; private set; }
    public Guid BomId { get; private set; }
    public int RevisionNo { get; private set; }
    public string Status { get; private set; } = BomRevisionStatus.Draft;
    public decimal OutputQuantity { get; private set; }
    public string? Note { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid? ApprovedByUserId { get; private set; }
    public DateTimeOffset? ApprovedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; }

    public IReadOnlyCollection<BomLine> Lines => _lines.AsReadOnly();

    protected BomRevision() { }

    public BomRevision(Guid id, Guid organizationId, Guid bomId, int revisionNo, decimal outputQuantity, string? note, Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || bomId == Guid.Empty || createdByUserId == Guid.Empty) throw new ArgumentException("Organization, BOM and actor are required.");
        if (revisionNo <= 0) throw new ArgumentOutOfRangeException(nameof(revisionNo));

        OrganizationId = organizationId;
        BomId = bomId;
        RevisionNo = revisionNo;
        ApplyHeader(outputQuantity, note);
        Status = BomRevisionStatus.Draft;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
        RowVersion = Guid.NewGuid();
    }

    public void EditDraft(decimal outputQuantity, string? note)
    {
        EnsureDraft();
        ApplyHeader(outputQuantity, note);
        RowVersion = Guid.NewGuid();
    }

    public void ReplaceLines(IEnumerable<BomLine> lines)
    {
        EnsureDraft();
        _lines.Clear();
        _lines.AddRange(lines);
        RowVersion = Guid.NewGuid();
    }

    public void AddLine(BomLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        _lines.Add(line);
    }

    /// <summary>Maker–checker: the approver cannot be the author. A revision needs at least one component.</summary>
    public void Approve(Guid approverUserId, DateTimeOffset now)
    {
        EnsureDraft();
        if (approverUserId == CreatedByUserId)
        {
            throw new ProductionDomainException("BOM_SELF_APPROVAL", "A BOM revision cannot be approved by its author.");
        }

        if (_lines.Count == 0)
        {
            throw new ProductionDomainException("BOM_LINE_INVALID", "A BOM revision needs at least one component.");
        }

        Status = BomRevisionStatus.Approved;
        ApprovedByUserId = approverUserId;
        ApprovedAtUtc = now.ToUniversalTime();
        RowVersion = Guid.NewGuid();
    }

    public void Obsolete()
    {
        if (Status != BomRevisionStatus.Approved)
        {
            throw new ProductionDomainException("BOM_INVALID_STATE", $"Only an approved revision can become obsolete; current status is '{Status}'.");
        }

        Status = BomRevisionStatus.Obsolete;
        RowVersion = Guid.NewGuid();
    }

    private void EnsureDraft()
    {
        if (Status != BomRevisionStatus.Draft)
        {
            throw new ProductionDomainException("BOM_INVALID_STATE", $"Only a draft revision can be edited or approved; current status is '{Status}'.");
        }
    }

    private void ApplyHeader(decimal outputQuantity, string? note)
    {
        if (outputQuantity <= 0 || outputQuantity > 999_999m)
        {
            throw new ProductionDomainException("BOM_LINE_INVALID", "Output quantity must be greater than zero.");
        }

        var trimmed = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (trimmed is { Length: > 500 })
        {
            throw new ProductionDomainException("BOM_LINE_INVALID", "Note cannot exceed 500 characters.");
        }

        OutputQuantity = decimal.Round(outputQuantity, 4);
        Note = trimmed;
    }
}

public class BomLine : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid BomRevisionId { get; private set; }
    public Guid ComponentItemId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal ScrapPercent { get; private set; }
    public int SortOrder { get; private set; }

    protected BomLine() { }

    public BomLine(Guid id, Guid organizationId, Guid bomRevisionId, Guid componentItemId, decimal quantity, decimal scrapPercent, int sortOrder) : base(id)
    {
        if (organizationId == Guid.Empty || bomRevisionId == Guid.Empty || componentItemId == Guid.Empty) throw new ArgumentException("Organization, revision and component are required.");
        if (quantity <= 0 || quantity > 999_999m)
        {
            throw new ProductionDomainException("BOM_LINE_INVALID", "Component quantity must be greater than zero.");
        }

        if (scrapPercent < 0 || scrapPercent > 50m)
        {
            throw new ProductionDomainException("BOM_LINE_INVALID", "Scrap percent must be between 0 and 50.");
        }

        OrganizationId = organizationId;
        BomRevisionId = bomRevisionId;
        ComponentItemId = componentItemId;
        Quantity = decimal.Round(quantity, 4);
        ScrapPercent = decimal.Round(scrapPercent, 2);
        SortOrder = sortOrder;
    }
}
