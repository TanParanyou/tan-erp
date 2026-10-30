using TanErp.Domain.Common;

namespace TanErp.Domain.Production;

public class WorkOrderOperation : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid WorkOrderId { get; private set; }
    public int Sequence { get; private set; }
    public string OperationCode { get; private set; } = string.Empty;
    public string OperationName { get; private set; } = string.Empty;
    public Guid? WorkCenterId { get; private set; }
    public decimal PlannedSetupHours { get; private set; }
    public decimal PlannedRunHours { get; private set; }
    public decimal ActualHours { get; private set; }
    public bool IsSubcontracted { get; private set; }
    public Guid? SubcontractPurchaseOrderId { get; private set; }
    public decimal SubcontractCost { get; private set; }
    public string Status { get; private set; } = WorkOrderOperationStatus.Pending;
    public string? Note { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }

    protected WorkOrderOperation() { }

    public WorkOrderOperation(
        Guid id,
        Guid organizationId,
        Guid workOrderId,
        int sequence,
        string operationCode,
        string operationName,
        Guid? workCenterId,
        decimal plannedSetupHours,
        decimal plannedRunHours,
        bool isSubcontracted,
        string? note = null) : base(id)
    {
        if (organizationId == Guid.Empty || workOrderId == Guid.Empty)
        {
            throw new ArgumentException("Organization and work order are required.");
        }

        if (sequence <= 0)
        {
            throw new ProductionDomainException("PRODUCTION_OPERATION_INVALID", "Operation sequence must be greater than zero.");
        }

        var trimmedCode = operationCode?.Trim() ?? string.Empty;
        var trimmedName = operationName?.Trim() ?? string.Empty;
        if (trimmedCode.Length == 0 || trimmedName.Length == 0)
        {
            throw new ProductionDomainException("PRODUCTION_OPERATION_INVALID", "Operation code and name are required.");
        }

        if (plannedSetupHours < 0m || plannedRunHours < 0m)
        {
            throw new ProductionDomainException("PRODUCTION_OPERATION_INVALID", "Planned hours cannot be negative.");
        }

        OrganizationId = organizationId;
        WorkOrderId = workOrderId;
        Sequence = sequence;
        OperationCode = trimmedCode.ToUpperInvariant();
        OperationName = trimmedName;
        WorkCenterId = workCenterId == Guid.Empty ? null : workCenterId;
        PlannedSetupHours = decimal.Round(plannedSetupHours, 4);
        PlannedRunHours = decimal.Round(plannedRunHours, 4);
        IsSubcontracted = isSubcontracted;
        Status = WorkOrderOperationStatus.Pending;
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    }

    public void CompleteInternal(decimal actualHours, string? note, DateTimeOffset now)
    {
        if (Status == WorkOrderOperationStatus.Completed)
        {
            throw new ProductionDomainException("PRODUCTION_OPERATION_ALREADY_COMPLETED", "The operation has already been completed.");
        }

        if (IsSubcontracted)
        {
            throw new ProductionDomainException("PRODUCTION_OPERATION_INVALID", "A subcontracted operation must be completed via subcontract recording.");
        }

        if (actualHours <= 0m)
        {
            throw new ProductionDomainException("PRODUCTION_QUANTITY_INVALID", "Actual hours must be greater than zero.");
        }

        var trimmedNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (trimmedNote is { Length: > 500 })
        {
            throw new ProductionDomainException("PRODUCTION_FIELD_INVALID", "Note cannot exceed 500 characters.");
        }

        ActualHours = decimal.Round(actualHours, 4);
        if (trimmedNote is not null) Note = trimmedNote;
        Status = WorkOrderOperationStatus.Completed;
        CompletedAtUtc = now.ToUniversalTime();
    }

    public void CompleteSubcontract(Guid purchaseOrderId, decimal costAmount, string? note, DateTimeOffset now)
    {
        if (!IsSubcontracted)
        {
            throw new ProductionDomainException("PRODUCTION_OPERATION_NOT_SUBCONTRACTED", "Only a subcontracted operation can be linked to a subcontract purchase order.");
        }

        if (Status == WorkOrderOperationStatus.Completed)
        {
            throw new ProductionDomainException("PRODUCTION_OPERATION_ALREADY_COMPLETED", "The operation has already been completed.");
        }

        if (purchaseOrderId == Guid.Empty)
        {
            throw new ProductionDomainException("PRODUCTION_FIELD_REQUIRED", "Purchase order ID is required.");
        }

        if (costAmount <= 0m)
        {
            throw new ProductionDomainException("PRODUCTION_QUANTITY_INVALID", "Subcontract cost amount must be greater than zero.");
        }

        var trimmedNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (trimmedNote is { Length: > 500 })
        {
            throw new ProductionDomainException("PRODUCTION_FIELD_INVALID", "Note cannot exceed 500 characters.");
        }

        SubcontractPurchaseOrderId = purchaseOrderId;
        SubcontractCost = decimal.Round(costAmount, 2);
        if (trimmedNote is not null) Note = trimmedNote;
        Status = WorkOrderOperationStatus.Completed;
        CompletedAtUtc = now.ToUniversalTime();
    }
}
