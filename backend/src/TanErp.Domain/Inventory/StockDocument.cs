using TanErp.Domain.Common;

namespace TanErp.Domain.Inventory;

/// <summary>Header of one posted stock transaction (receipt, issue, transfer or adjustment). Immutable once posted.</summary>
public class StockDocument : Entity
{
    private readonly List<StockMovement> _movements = new();

    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public string DocumentType { get; private set; } = string.Empty;
    public string Number { get; private set; } = string.Empty;
    public Guid WarehouseId { get; private set; }
    public Guid? ToWarehouseId { get; private set; }
    public Guid? ProjectId { get; private set; }
    public string? SourceType { get; private set; }
    public Guid? SourceId { get; private set; }
    public string? Reason { get; private set; }
    public DateTimeOffset OccurredAtUtc { get; private set; }
    public Guid PostedByUserId { get; private set; }
    public DateTimeOffset PostedAtUtc { get; private set; }

    public IReadOnlyCollection<StockMovement> Movements => _movements.AsReadOnly();

    protected StockDocument() { }

    public StockDocument(
        Guid id, Guid organizationId, Guid branchId, string documentType, string number, Guid warehouseId, Guid? toWarehouseId,
        Guid? projectId, string? sourceType, Guid? sourceId, string? reason, DateTimeOffset occurredAtUtc, Guid postedByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || branchId == Guid.Empty || warehouseId == Guid.Empty || postedByUserId == Guid.Empty)
        {
            throw new ArgumentException("Organization, branch, warehouse and actor are required.");
        }

        if (!StockDocumentType.All.Contains(documentType)) throw new ArgumentException("Unknown stock document type.", nameof(documentType));
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Document number cannot be blank.", nameof(number));

        var trimmedReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        if (trimmedReason is { Length: > 500 })
        {
            throw new InventoryDomainException("INVENTORY_FIELD_INVALID", "Reason cannot exceed 500 characters.");
        }

        OrganizationId = organizationId;
        BranchId = branchId;
        DocumentType = documentType;
        Number = number.Trim();
        WarehouseId = warehouseId;
        ToWarehouseId = toWarehouseId;
        ProjectId = projectId;
        SourceType = sourceType;
        SourceId = sourceId;
        Reason = trimmedReason;
        OccurredAtUtc = occurredAtUtc.ToUniversalTime();
        PostedByUserId = postedByUserId;
        PostedAtUtc = now.ToUniversalTime();
    }

    public void AddMovement(StockMovement movement)
    {
        ArgumentNullException.ThrowIfNull(movement);
        _movements.Add(movement);
    }
}

/// <summary>One signed change of stock for an item in a warehouse. The ledger is append-only.</summary>
public class StockMovement : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid StockDocumentId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid ItemId { get; private set; }
    public Guid UnitId { get; private set; }
    public string Kind { get; private set; } = string.Empty;
    public decimal QuantityDelta { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal ValueDelta { get; private set; }
    public decimal OnHandAfter { get; private set; }
    public DateTimeOffset OccurredAtUtc { get; private set; }
    public DateTimeOffset PostedAtUtc { get; private set; }

    protected StockMovement() { }

    public StockMovement(
        Guid id, Guid organizationId, Guid stockDocumentId, Guid warehouseId, Guid itemId, Guid unitId, string kind,
        decimal quantityDelta, decimal unitCost, decimal valueDelta, decimal onHandAfter, DateTimeOffset occurredAtUtc, DateTimeOffset postedAtUtc) : base(id)
    {
        if (!StockMovementKind.All.Contains(kind)) throw new ArgumentException("Unknown movement kind.", nameof(kind));
        if (quantityDelta == 0) throw new InventoryDomainException("INVENTORY_QUANTITY_INVALID", "A movement cannot be zero.");

        OrganizationId = organizationId;
        StockDocumentId = stockDocumentId;
        WarehouseId = warehouseId;
        ItemId = itemId;
        UnitId = unitId;
        Kind = kind;
        QuantityDelta = quantityDelta;
        UnitCost = unitCost;
        ValueDelta = valueDelta;
        OnHandAfter = onHandAfter;
        OccurredAtUtc = occurredAtUtc.ToUniversalTime();
        PostedAtUtc = postedAtUtc.ToUniversalTime();
    }
}

public class StockReservation : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid ItemId { get; private set; }
    public Guid ProjectId { get; private set; }
    public decimal Quantity { get; private set; }
    public string Status { get; private set; } = ReservationStatus.Active;
    public string? Note { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; }

    protected StockReservation() { }

    public StockReservation(Guid id, Guid organizationId, Guid warehouseId, Guid itemId, Guid projectId, decimal quantity, string? note, Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || warehouseId == Guid.Empty || itemId == Guid.Empty || projectId == Guid.Empty || createdByUserId == Guid.Empty)
        {
            throw new ArgumentException("Organization, warehouse, item, project and actor are required.");
        }

        if (quantity <= 0) throw new InventoryDomainException("INVENTORY_QUANTITY_INVALID", "Quantity must be greater than zero.");
        var trimmed = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (trimmed is { Length: > 500 }) throw new InventoryDomainException("INVENTORY_FIELD_INVALID", "Note cannot exceed 500 characters.");

        OrganizationId = organizationId;
        WarehouseId = warehouseId;
        ItemId = itemId;
        ProjectId = projectId;
        Quantity = decimal.Round(quantity, 4);
        Note = trimmed;
        Status = ReservationStatus.Active;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
        UpdatedAtUtc = CreatedAtUtc;
        RowVersion = Guid.NewGuid();
    }

    /// <summary>Takes up to <paramref name="quantity"/> out of the reservation; returns what was consumed.</summary>
    public decimal Consume(decimal quantity, DateTimeOffset now)
    {
        if (Status != ReservationStatus.Active) return 0m;
        var consumed = Math.Min(quantity, Quantity);
        Quantity -= consumed;
        if (Quantity == 0) Status = ReservationStatus.Consumed;
        Touch(now);
        return consumed;
    }

    public void Release(DateTimeOffset now)
    {
        if (Status != ReservationStatus.Active)
        {
            throw new InventoryDomainException("INVENTORY_RESERVATION_INVALID", $"A {Status} reservation cannot be released.");
        }

        Status = ReservationStatus.Released;
        Touch(now);
    }

    private void Touch(DateTimeOffset now)
    {
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }
}
