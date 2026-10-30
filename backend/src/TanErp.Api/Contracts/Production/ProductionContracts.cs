namespace TanErp.Api.Contracts.Production;

public sealed record ProductionPersonResponse(Guid Id, string DisplayName, string? Email);

public sealed record ProductionItemResponse(Guid Id, string Code, string NameTh, string UnitCode);

public sealed record ProductionPaginationResponse(int Page, int PageSize, int TotalCount, int TotalPages);

public sealed record CreateWorkCenterRequest(
    string? Code,
    string Name,
    decimal? DailyCapacityHours = null,
    decimal? HourlyRate = null,
    Guid? BranchId = null);

public sealed record UpdateWorkCenterRequest(
    string Name,
    decimal DailyCapacityHours,
    decimal HourlyRate);

public sealed record WorkCenterRefResponse(
    Guid Id,
    string Code,
    string Name,
    decimal DailyCapacityHours,
    decimal HourlyRate);

public sealed record WorkCenterResponse(
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

public sealed record WorkCenterListResponse(
    IReadOnlyList<WorkCenterResponse> Items,
    ProductionPaginationResponse Pagination);

public sealed record WorkCenterDailyCapacityPointResponse(
    DateOnly Date,
    decimal CapacityHours,
    decimal PlannedHours,
    decimal RemainingHours,
    decimal UtilizationPercent,
    bool IsOverloaded);

public sealed record WorkCenterCapacityRowResponse(
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
    IReadOnlyList<WorkCenterDailyCapacityPointResponse> Days);

public sealed record WorkCenterCapacityViewResponse(
    DateOnly FromDate,
    DateOnly ToDate,
    IReadOnlyList<WorkCenterCapacityRowResponse> WorkCenters);

public sealed record BomLineRequest(Guid ComponentItemId, decimal Quantity, decimal ScrapPercent);

public sealed record BomRoutingStepRequest(
    int Sequence,
    string OperationCode,
    string OperationName,
    Guid? WorkCenterId = null,
    decimal SetupHours = 0m,
    decimal RunHours = 0m,
    bool IsSubcontracted = false,
    decimal StdSubcontractCost = 0m,
    string? Note = null);

public sealed record BomRequest(
    Guid ItemId,
    decimal OutputQuantity,
    string? Note,
    List<BomLineRequest>? Lines,
    List<BomRoutingStepRequest>? RoutingSteps = null);

public sealed record BomDraftRequest(
    decimal OutputQuantity,
    string? Note,
    List<BomLineRequest>? Lines,
    List<BomRoutingStepRequest>? RoutingSteps = null);

public sealed record BomLineResponse(Guid Id, ProductionItemResponse Component, decimal Quantity, decimal ScrapPercent, decimal GrossQuantity);

public sealed record BomRoutingStepResponse(
    Guid Id,
    int Sequence,
    string OperationCode,
    string OperationName,
    WorkCenterRefResponse? WorkCenter,
    decimal SetupHours,
    decimal RunHours,
    bool IsSubcontracted,
    decimal StdSubcontractCost,
    string? Note);

public sealed record BomRevisionResponse(
    Guid Id,
    int RevisionNo,
    string Status,
    decimal OutputQuantity,
    string? Note,
    ProductionPersonResponse CreatedBy,
    DateTimeOffset CreatedAtUtc,
    ProductionPersonResponse? ApprovedBy,
    DateTimeOffset? ApprovedAtUtc,
    Guid RowVersion,
    IReadOnlyList<BomLineResponse> Lines,
    IReadOnlyList<BomRoutingStepResponse> RoutingSteps);

public sealed record BomResponse(Guid Id, string Code, ProductionItemResponse Item, DateTimeOffset CreatedAtUtc, IReadOnlyList<BomRevisionResponse> Revisions);

public sealed record BomListItemResponse(Guid Id, string Code, ProductionItemResponse Item, int? ApprovedRevisionNo, int LatestRevisionNo, string LatestStatus, DateTimeOffset CreatedAtUtc);

public sealed record BomListResponse(IReadOnlyList<BomListItemResponse> Items, ProductionPaginationResponse Pagination);

public sealed record WorkOrderRequest(
    Guid ItemId,
    Guid WarehouseId,
    Guid? ProjectId,
    decimal PlannedQuantity,
    string? Note,
    DateOnly? PlannedStartDate = null,
    DateOnly? PlannedEndDate = null);

public sealed record WorkOrderActionRequest(string? Reason);

public sealed record WorkOrderMaterialLineRequest(Guid ItemId, decimal Quantity);

public sealed record WorkOrderMaterialsRequest(List<WorkOrderMaterialLineRequest>? Lines);

public sealed record WorkOrderCompleteOperationRequest(decimal ActualHours, string? Note = null);

public sealed record WorkOrderSubcontractOperationRequest(Guid PurchaseOrderId, decimal CostAmount, string? Note = null);

public sealed record WorkOrderCompleteRequest(decimal Quantity);

public sealed record WorkOrderMaterialResponse(
    Guid Id,
    ProductionItemResponse Item,
    decimal RequiredQuantity,
    decimal IssuedQuantity,
    decimal ReturnedQuantity,
    decimal NetIssuedQuantity,
    decimal RemainingQuantity,
    decimal IssuedValue,
    decimal ReturnedValue);

public sealed record WorkOrderOperationResponse(
    Guid Id,
    int Sequence,
    string OperationCode,
    string OperationName,
    WorkCenterRefResponse? WorkCenter,
    decimal PlannedSetupHours,
    decimal PlannedRunHours,
    decimal ActualHours,
    bool IsSubcontracted,
    WorkOrderRefResponse? SubcontractPurchaseOrder,
    decimal SubcontractCost,
    string Status,
    string? Note,
    DateTimeOffset? CompletedAtUtc);

public sealed record WorkOrderLaborEntryResponse(
    Guid Id,
    string Number,
    ProductionPersonResponse Worker,
    DateOnly WorkDate,
    decimal Hours,
    decimal HourlyRate,
    decimal CostAmount,
    string TaskDescription,
    DateTimeOffset? DecidedAtUtc);

public sealed record WorkOrderTransactionResponse(
    Guid Id,
    string Kind,
    Guid StockDocumentId,
    string StockDocumentNumber,
    decimal Quantity,
    decimal Value,
    ProductionPersonResponse Actor,
    DateTimeOffset CreatedAtUtc);

public sealed record WorkOrderRefResponse(Guid Id, string Code, string Name);

public sealed record WorkOrderResponse(
    Guid Id,
    Guid BranchId,
    string Number,
    string Status,
    ProductionItemResponse Item,
    Guid BomRevisionId,
    string BomCode,
    int BomRevisionNo,
    WorkOrderRefResponse Warehouse,
    WorkOrderRefResponse? Project,
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
    ProductionPersonResponse CreatedBy,
    DateTimeOffset CreatedAtUtc,
    Guid RowVersion,
    IReadOnlyList<WorkOrderMaterialResponse> Materials,
    IReadOnlyList<WorkOrderOperationResponse> Operations,
    IReadOnlyList<WorkOrderLaborEntryResponse> LaborEntries,
    IReadOnlyList<WorkOrderTransactionResponse> Transactions);

public sealed record WorkOrderListItemResponse(
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

public sealed record WorkOrderListResponse(IReadOnlyList<WorkOrderListItemResponse> Items, ProductionPaginationResponse Pagination);
