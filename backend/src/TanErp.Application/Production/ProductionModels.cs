namespace TanErp.Application.Production;

public sealed record ProductionPerson(Guid Id, string DisplayName, string? Email);

public sealed record ProductionItemRef(Guid Id, string Code, string NameTh, string UnitCode);

public sealed record BomLineInput(Guid ComponentItemId, decimal Quantity, decimal ScrapPercent);

public sealed record BomInput(Guid ItemId, decimal OutputQuantity, string? Note, IReadOnlyList<BomLineInput> Lines);

public sealed record BomDraftInput(decimal OutputQuantity, string? Note, IReadOnlyList<BomLineInput> Lines);

public sealed record BomLineProjection(Guid Id, ProductionItemRef Component, decimal Quantity, decimal ScrapPercent, decimal GrossQuantity);

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
    IReadOnlyList<BomLineProjection> Lines);

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

public sealed record WorkOrderInput(Guid ItemId, Guid WarehouseId, Guid? ProjectId, decimal PlannedQuantity, string? Note);

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
    decimal CostAllocated,
    string? Note,
    string? CancelReason,
    ProductionPerson CreatedBy,
    DateTimeOffset CreatedAtUtc,
    Guid RowVersion,
    IReadOnlyList<WorkOrderMaterialProjection> Materials,
    IReadOnlyList<WorkOrderTransactionProjection> Transactions);

public sealed record WorkOrderListItemProjection(
    Guid Id,
    string Number,
    string Status,
    string ItemCode,
    string ItemNameTh,
    decimal PlannedQuantity,
    decimal CompletedQuantity,
    string? ProjectCode,
    DateTimeOffset CreatedAtUtc);

public sealed record WorkOrderListQuery(string? Search, string? Status, Guid? ProjectId, int Page, int PageSize);

public sealed record PagedWorkOrders(IReadOnlyList<WorkOrderListItemProjection> Items, int TotalCount, int Page, int PageSize);

public sealed record WorkOrderMaterialQuantityInput(Guid ItemId, decimal Quantity);

public enum WorkOrderAction
{
    Release,
    Cancel
}
