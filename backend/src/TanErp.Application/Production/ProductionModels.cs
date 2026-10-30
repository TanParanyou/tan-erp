namespace TanErp.Application.Production;

public sealed record ProductionPerson(Guid Id, string DisplayName, string? Email);

public sealed record ProductionItemRef(Guid Id, string Code, string NameTh, string UnitCode);

public sealed record WorkCenterInput(
    string? Code,
    string Name,
    decimal? DailyCapacityHours,
    decimal? HourlyRate,
    Guid? BranchId = null);

public sealed record WorkCenterUpdateInput(
    string Name,
    decimal DailyCapacityHours,
    decimal HourlyRate);

public sealed record WorkCenterRef(
    Guid Id,
    string Code,
    string Name,
    decimal DailyCapacityHours,
    decimal HourlyRate);

public sealed record WorkCenterProjection(
    Guid Id,
    Guid BranchId,
    string Code,
    string Name,
    decimal DailyCapacityHours,
    decimal HourlyRate,
    string Status,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record WorkCenterListQuery(
    Guid? BranchId,
    string? Status,
    string? Search,
    int Page,
    int PageSize);

public sealed record PagedWorkCenters(
    IReadOnlyList<WorkCenterProjection> Items,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record WorkCenterDailyCapacityPointProjection(
    DateOnly Date,
    decimal CapacityHours,
    decimal PlannedHours,
    decimal RemainingHours,
    decimal UtilizationPercent,
    bool IsOverloaded);

public sealed record WorkCenterCapacityRowProjection(
    Guid WorkCenterId,
    Guid BranchId,
    string Code,
    string Name,
    decimal DailyCapacityHours,
    decimal HourlyRate,
    decimal TotalCapacityHours,
    decimal TotalPlannedHours,
    decimal UtilizationPercent,
    bool IsOverloaded,
    IReadOnlyList<WorkCenterDailyCapacityPointProjection> Days);

public sealed record WorkCenterCapacityViewProjection(
    DateOnly FromDate,
    DateOnly ToDate,
    IReadOnlyList<WorkCenterCapacityRowProjection> WorkCenters);

public sealed record BomLineInput(Guid ComponentItemId, decimal Quantity, decimal ScrapPercent);

public sealed record BomRoutingStepInput(
    int Sequence,
    string OperationCode,
    string OperationName,
    Guid? WorkCenterId,
    decimal SetupHours,
    decimal RunHours,
    bool IsSubcontracted = false,
    decimal StdSubcontractCost = 0m,
    string? Note = null);

public sealed record BomInput(
    Guid ItemId,
    decimal OutputQuantity,
    string? Note,
    IReadOnlyList<BomLineInput> Lines,
    IReadOnlyList<BomRoutingStepInput>? RoutingSteps = null);

public sealed record BomDraftInput(
    decimal OutputQuantity,
    string? Note,
    IReadOnlyList<BomLineInput> Lines,
    IReadOnlyList<BomRoutingStepInput>? RoutingSteps = null);

public sealed record BomLineProjection(Guid Id, ProductionItemRef Component, decimal Quantity, decimal ScrapPercent, decimal GrossQuantity);

public sealed record BomRoutingStepProjection(
    Guid Id,
    int Sequence,
    string OperationCode,
    string OperationName,
    WorkCenterRef? WorkCenter,
    decimal SetupHours,
    decimal RunHours,
    bool IsSubcontracted,
    decimal StdSubcontractCost,
    string? Note);

public sealed record BomRevisionProjection(
    Guid Id,
    int RevisionNo,
    string Status,
    decimal OutputQuantity,
    string? Note,
    ProductionPerson CreatedBy,
    DateTimeOffset CreatedAtUtc,
    ProductionPerson? ApprovedBy,
    DateTimeOffset? ApprovedAtUtc,
    Guid RowVersion,
    IReadOnlyList<BomLineProjection> Lines,
    IReadOnlyList<BomRoutingStepProjection> RoutingSteps);

public sealed record BomProjection(
    Guid Id,
    string Code,
    ProductionItemRef Item,
    DateTimeOffset CreatedAtUtc,
    IReadOnlyList<BomRevisionProjection> Revisions);

public sealed record BomListItemProjection(
    Guid Id,
    string Code,
    ProductionItemRef Item,
    int? ApprovedRevisionNo,
    int LatestRevisionNo,
    string LatestStatus,
    DateTimeOffset CreatedAtUtc);

public sealed record BomListQuery(string? Search, int Page, int PageSize);

public sealed record PagedBoms(IReadOnlyList<BomListItemProjection> Items, int TotalCount, int Page, int PageSize);

public enum BomAction
{
    Approve,
    Obsolete
}

public sealed record WorkOrderInput(
    Guid ItemId,
    Guid WarehouseId,
    Guid? ProjectId,
    decimal PlannedQuantity,
    string? Note,
    DateOnly? PlannedStartDate = null,
    DateOnly? PlannedEndDate = null);

public sealed record WorkOrderMaterialProjection(
    Guid Id,
    ProductionItemRef Item,
    decimal RequiredQuantity,
    decimal IssuedQuantity,
    decimal ReturnedQuantity,
    decimal NetIssuedQuantity,
    decimal RemainingQuantity,
    decimal IssuedValue,
    decimal ReturnedValue);

public sealed record WorkOrderOperationProjection(
    Guid Id,
    int Sequence,
    string OperationCode,
    string OperationName,
    WorkCenterRef? WorkCenter,
    decimal PlannedSetupHours,
    decimal PlannedRunHours,
    decimal ActualHours,
    bool IsSubcontracted,
    WorkOrderRef? SubcontractPurchaseOrder,
    decimal SubcontractCost,
    string Status,
    string? Note,
    DateTimeOffset? CompletedAtUtc);

public sealed record WorkOrderLaborEntryProjection(
    Guid Id,
    string Number,
    ProductionPerson Worker,
    DateOnly WorkDate,
    decimal Hours,
    decimal HourlyRate,
    decimal CostAmount,
    string TaskDescription,
    DateTimeOffset? DecidedAtUtc);

public sealed record WorkOrderTransactionProjection(
    Guid Id,
    string Kind,
    Guid StockDocumentId,
    string StockDocumentNumber,
    decimal Quantity,
    decimal Value,
    ProductionPerson Actor,
    DateTimeOffset CreatedAtUtc);

public sealed record WorkOrderRef(Guid Id, string Code, string Name);

public sealed record WorkOrderWarehouseRef(Guid Id, string Code, string Name);

public sealed record WorkOrderProjection(
    Guid Id,
    Guid BranchId,
    string Number,
    string Status,
    ProductionItemRef Item,
    Guid BomRevisionId,
    string BomCode,
    int BomRevisionNo,
    WorkOrderWarehouseRef Warehouse,
    WorkOrderRef? Project,
    decimal PlannedQuantity,
    decimal CompletedQuantity,
    DateOnly PlannedStartDate,
    DateOnly PlannedEndDate,
    decimal NetMaterialValue,
    decimal LaborHours,
    decimal LaborCost,
    decimal SubcontractCost,
    decimal TotalManufacturingCost,
    decimal CostAllocated,
    string? Note,
    string? CancelReason,
    ProductionPerson CreatedBy,
    DateTimeOffset CreatedAtUtc,
    Guid RowVersion,
    IReadOnlyList<WorkOrderMaterialProjection> Materials,
    IReadOnlyList<WorkOrderOperationProjection> Operations,
    IReadOnlyList<WorkOrderLaborEntryProjection> LaborEntries,
    IReadOnlyList<WorkOrderTransactionProjection> Transactions);

public sealed record WorkOrderListItemProjection(
    Guid Id,
    string Number,
    string Status,
    string ItemCode,
    string ItemNameTh,
    decimal PlannedQuantity,
    decimal CompletedQuantity,
    DateOnly PlannedStartDate,
    DateOnly PlannedEndDate,
    decimal TotalManufacturingCost,
    string? ProjectCode,
    DateTimeOffset CreatedAtUtc);

public sealed record WorkOrderListQuery(string? Search, string? Status, Guid? ProjectId, int Page, int PageSize);

public sealed record PagedWorkOrders(IReadOnlyList<WorkOrderListItemProjection> Items, int TotalCount, int Page, int PageSize);

public sealed record WorkOrderMaterialQuantityInput(Guid ItemId, decimal Quantity);

public sealed record CompleteOperationInput(decimal ActualHours, string? Note = null);

public sealed record RecordSubcontractInput(Guid PurchaseOrderId, decimal CostAmount, string? Note = null);

public enum WorkOrderAction
{
    Release,
    Cancel
}
