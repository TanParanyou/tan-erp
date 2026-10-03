using TanErp.Domain.Common;

namespace TanErp.Domain.Production;

public class WorkOrder : Entity
{
    private readonly List<WorkOrderMaterial> _materials = new();

    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public Guid ItemId { get; private set; }
    public Guid BomRevisionId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid? ProjectId { get; private set; }
    public decimal PlannedQuantity { get; private set; }
    public decimal CompletedQuantity { get; private set; }
    public decimal CostAllocated { get; private set; }
    public string Status { get; private set; } = WorkOrderStatus.Draft;
    public string? Note { get; private set; }
    public string? CancelReason { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; }

    public IReadOnlyCollection<WorkOrderMaterial> Materials => _materials.AsReadOnly();

    protected WorkOrder() { }

    public WorkOrder(
        Guid id, Guid organizationId, Guid branchId, string number, Guid itemId, Guid bomRevisionId, Guid warehouseId, Guid? projectId,
        decimal plannedQuantity, string? note, Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || branchId == Guid.Empty || itemId == Guid.Empty || bomRevisionId == Guid.Empty || warehouseId == Guid.Empty || createdByUserId == Guid.Empty)
        {
            throw new ArgumentException("Organization, branch, item, BOM revision, warehouse and actor are required.");
        }

        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Work order number cannot be blank.", nameof(number));
        if (plannedQuantity <= 0 || plannedQuantity > 999_999m)
        {
            throw new ProductionDomainException("PRODUCTION_QUANTITY_INVALID", "Planned quantity must be greater than zero.");
        }

        var trimmed = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (trimmed is { Length: > 500 }) throw new ProductionDomainException("PRODUCTION_QUANTITY_INVALID", "Note cannot exceed 500 characters.");

        OrganizationId = organizationId;
        BranchId = branchId;
        Number = number.Trim();
        ItemId = itemId;
        BomRevisionId = bomRevisionId;
        WarehouseId = warehouseId;
        ProjectId = projectId;
        PlannedQuantity = decimal.Round(plannedQuantity, 4);
        Note = trimmed;
        Status = WorkOrderStatus.Draft;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
        UpdatedAtUtc = CreatedAtUtc;
        RowVersion = Guid.NewGuid();
    }

    public void AddMaterial(WorkOrderMaterial material)
    {
        ArgumentNullException.ThrowIfNull(material);
        _materials.Add(material);
    }

    public decimal NetIssuedValue => _materials.Sum(m => m.IssuedValue - m.ReturnedValue);

    public void Release(DateTimeOffset now)
    {
        if (Status != WorkOrderStatus.Draft)
        {
            throw new ProductionDomainException("PRODUCTION_INVALID_STATE", $"Only a draft work order can be released; current status is '{Status}'.");
        }

        Status = WorkOrderStatus.Released;
        Touch(now);
    }

    public void Cancel(string reason, DateTimeOffset now)
    {
        if (Status is not (WorkOrderStatus.Draft or WorkOrderStatus.Released or WorkOrderStatus.InProgress) || CompletedQuantity > 0)
        {
            throw new ProductionDomainException("PRODUCTION_INVALID_STATE", $"A {Status} work order cannot be cancelled.");
        }

        if (_materials.Any(m => m.NetIssuedQuantity > 0))
        {
            throw new ProductionDomainException("PRODUCTION_HAS_ISSUED_MATERIALS", "Return all issued materials before cancelling.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ProductionDomainException("PRODUCTION_REASON_REQUIRED", "A reason is required to cancel a work order.");
        }

        Status = WorkOrderStatus.Cancelled;
        CancelReason = reason.Trim();
        Touch(now);
    }

    public void EnsureCanIssue()
    {
        if (Status is not (WorkOrderStatus.Released or WorkOrderStatus.InProgress))
        {
            throw new ProductionDomainException("PRODUCTION_INVALID_STATE", $"Materials can only be issued to a released or in-progress work order; current status is '{Status}'.");
        }
    }

    /// <summary>Net issued stock of a material cannot exceed what the BOM snapshot requires.</summary>
    public void ValidateIssue(Guid itemId, decimal quantity)
    {
        EnsureCanIssue();
        var material = FindMaterial(itemId);
        if (quantity <= 0)
        {
            throw new ProductionDomainException("PRODUCTION_QUANTITY_INVALID", "Issued quantity must be greater than zero.");
        }

        if (material.NetIssuedQuantity + quantity > material.RequiredQuantity)
        {
            throw new ProductionDomainException("PRODUCTION_OVER_ISSUED", "The quantity exceeds what the work order still requires.");
        }
    }

    public void RecordIssue(Guid itemId, decimal quantity, decimal value, DateTimeOffset now)
    {
        ValidateIssue(itemId, quantity);
        var material = FindMaterial(itemId);
        material.Issue(quantity, value);
        Status = WorkOrderStatus.InProgress;
        Touch(now);
    }

    /// <summary>Only stock not yet consumed by completed output can go back.</summary>
    public decimal ReturnableQuantity(Guid itemId)
    {
        var material = FindMaterial(itemId);
        var consumed = PlannedQuantity == 0 ? 0m : decimal.Round(material.RequiredQuantity * CompletedQuantity / PlannedQuantity, 4);
        return Math.Max(0m, material.NetIssuedQuantity - consumed);
    }

    /// <summary>Returned stock is valued at the average cost it was issued at, so returns never distort stock valuation.</summary>
    public decimal ReturnValueFor(Guid itemId, decimal quantity)
    {
        var material = FindMaterial(itemId);
        var net = material.NetIssuedQuantity;
        if (net <= 0) return 0m;
        var remainingValue = material.IssuedValue - material.ReturnedValue;
        return quantity >= net ? remainingValue : decimal.Round(remainingValue * quantity / net, 2);
    }

    public void ValidateReturn(Guid itemId, decimal quantity)
    {
        if (Status is not (WorkOrderStatus.Released or WorkOrderStatus.InProgress))
        {
            throw new ProductionDomainException("PRODUCTION_INVALID_STATE", $"Materials can only be returned from a released or in-progress work order; current status is '{Status}'.");
        }

        if (quantity <= 0)
        {
            throw new ProductionDomainException("PRODUCTION_QUANTITY_INVALID", "Returned quantity must be greater than zero.");
        }

        if (quantity > ReturnableQuantity(itemId))
        {
            throw new ProductionDomainException("PRODUCTION_RETURN_EXCEEDS", "The quantity exceeds the materials not yet consumed by completed output.");
        }
    }

    public void RecordReturn(Guid itemId, decimal quantity, decimal value, DateTimeOffset now)
    {
        ValidateReturn(itemId, quantity);
        FindMaterial(itemId).Return(quantity, value);
        Touch(now);
    }

    /// <summary>
    /// Completes part of the order. Output is limited to the planned quantity, and every material must already be
    /// issued for the cumulative completed share. Returns the material value allocated to this completion.
    /// </summary>
    public decimal Complete(decimal quantity, DateTimeOffset now)
    {
        if (Status != WorkOrderStatus.InProgress)
        {
            throw new ProductionDomainException("PRODUCTION_INVALID_STATE", $"Only an in-progress work order can be completed; current status is '{Status}'.");
        }

        var rounded = decimal.Round(quantity, 4);
        if (rounded <= 0)
        {
            throw new ProductionDomainException("PRODUCTION_QUANTITY_INVALID", "Completed quantity must be greater than zero.");
        }

        var newCompleted = CompletedQuantity + rounded;
        if (newCompleted > PlannedQuantity)
        {
            throw new ProductionDomainException("PRODUCTION_OVER_COMPLETED", "Completed quantity cannot exceed the planned quantity.");
        }

        foreach (var material in _materials)
        {
            var needed = decimal.Round(material.RequiredQuantity * newCompleted / PlannedQuantity, 4);
            if (material.NetIssuedQuantity < needed)
            {
                throw new ProductionDomainException("PRODUCTION_MATERIAL_SHORTAGE", "Not enough material has been issued for this completion.");
            }
        }

        var net = NetIssuedValue;
        var value = newCompleted == PlannedQuantity
            ? Math.Max(0m, net - CostAllocated)
            : decimal.Round(net * rounded / PlannedQuantity, 2);

        CompletedQuantity = newCompleted;
        CostAllocated += value;
        if (newCompleted == PlannedQuantity) Status = WorkOrderStatus.Completed;
        Touch(now);
        return value;
    }

    private WorkOrderMaterial FindMaterial(Guid itemId) =>
        _materials.FirstOrDefault(m => m.ItemId == itemId)
        ?? throw new ProductionDomainException("PRODUCTION_MATERIAL_INVALID", "The item is not a material of this work order.");

    private void Touch(DateTimeOffset now)
    {
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }
}

public class WorkOrderMaterial : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid WorkOrderId { get; private set; }
    public Guid ItemId { get; private set; }
    public decimal RequiredQuantity { get; private set; }
    public decimal IssuedQuantity { get; private set; }
    public decimal ReturnedQuantity { get; private set; }
    public decimal IssuedValue { get; private set; }
    public decimal ReturnedValue { get; private set; }
    public int SortOrder { get; private set; }

    protected WorkOrderMaterial() { }

    public WorkOrderMaterial(Guid id, Guid organizationId, Guid workOrderId, Guid itemId, decimal requiredQuantity, int sortOrder) : base(id)
    {
        if (organizationId == Guid.Empty || workOrderId == Guid.Empty || itemId == Guid.Empty) throw new ArgumentException("Organization, work order and item are required.");
        if (requiredQuantity <= 0) throw new ProductionDomainException("PRODUCTION_QUANTITY_INVALID", "Required quantity must be greater than zero.");

        OrganizationId = organizationId;
        WorkOrderId = workOrderId;
        ItemId = itemId;
        RequiredQuantity = decimal.Round(requiredQuantity, 4);
        SortOrder = sortOrder;
    }

    public decimal NetIssuedQuantity => IssuedQuantity - ReturnedQuantity;

    public void Issue(decimal quantity, decimal value)
    {
        if (quantity <= 0) throw new ProductionDomainException("PRODUCTION_QUANTITY_INVALID", "Issued quantity must be greater than zero.");
        IssuedQuantity += decimal.Round(quantity, 4);
        IssuedValue += value;
    }

    public void Return(decimal quantity, decimal value)
    {
        if (quantity <= 0) throw new ProductionDomainException("PRODUCTION_QUANTITY_INVALID", "Returned quantity must be greater than zero.");
        ReturnedQuantity += decimal.Round(quantity, 4);
        ReturnedValue += value;
    }
}

/// <summary>Link between a work order step and the stock document it posted.</summary>
public class WorkOrderTransaction : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid WorkOrderId { get; private set; }
    public string Kind { get; private set; } = string.Empty;
    public Guid StockDocumentId { get; private set; }
    public string StockDocumentNumber { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal Value { get; private set; }
    public Guid ActorUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    protected WorkOrderTransaction() { }

    public WorkOrderTransaction(Guid id, Guid organizationId, Guid workOrderId, string kind, Guid stockDocumentId, string stockDocumentNumber, decimal quantity, decimal value, Guid actorUserId, DateTimeOffset now) : base(id)
    {
        OrganizationId = organizationId;
        WorkOrderId = workOrderId;
        Kind = kind;
        StockDocumentId = stockDocumentId;
        StockDocumentNumber = stockDocumentNumber;
        Quantity = quantity;
        Value = value;
        ActorUserId = actorUserId;
        CreatedAtUtc = now.ToUniversalTime();
    }
}
