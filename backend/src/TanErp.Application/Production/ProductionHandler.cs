using System.Globalization;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Production;

public sealed record ProductionCaller(string FirebaseUid, Guid MembershipId, string TraceId);

/// <summary>
/// BOM and work order use cases. Permission is resolved from PostgreSQL per call; request shape is validated here
/// and the store owns the transaction (including the stock movements posted through the inventory port).
/// </summary>
public class ProductionHandler
{
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 25;
    public const int MaxLines = 200;

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IProductionStore _store;

    public ProductionHandler(IRequestAccessResolver accessResolver, IProductionStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    private Task<Result<RequestAccessContext>> AccessAsync(ProductionCaller caller, string permission, CancellationToken ct) =>
        _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, ct);

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static (int Page, int PageSize) Paging(int page, int pageSize) =>
        (Math.Max(1, page), pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize));

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string Num(decimal value) => value.ToString(CultureInfo.InvariantCulture);

    private static Result<BomDraftInput> ValidateDraft(BomDraftInput? input)
    {
        if (input is null || input.OutputQuantity <= 0)
        {
            return Fail<BomDraftInput>("BOM_LINE_INVALID", "Output quantity must be greater than zero.");
        }

        if (input.Lines is null || input.Lines.Count == 0 || input.Lines.Count > MaxLines)
        {
            return Fail<BomDraftInput>("BOM_LINE_INVALID", $"A BOM needs between 1 and {MaxLines} components.");
        }

        if (input.Lines.GroupBy(l => l.ComponentItemId).Any(g => g.Count() > 1) || input.Lines.Any(l => l.Quantity <= 0 || l.ScrapPercent < 0 || l.ScrapPercent > 50))
        {
            return Fail<BomDraftInput>("BOM_LINE_INVALID", "Components must be unique, with quantity above zero and scrap between 0 and 50 percent.");
        }

        return Result<BomDraftInput>.Success(input);
    }

    private static string LinesKey(IEnumerable<BomLineInput> lines) =>
        string.Join(";", lines.OrderBy(l => l.ComponentItemId).Select(l => $"{l.ComponentItemId}:{Num(l.Quantity)}:{Num(l.ScrapPercent)}"));

    // ----- BOMs ------------------------------------------------------------------------------------

    public async Task<Result<BomProjection>> CreateBomAsync(ProductionCaller caller, string idempotencyKey, BomInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "boms.manage", ct);
        if (access.IsFailure) return Result<BomProjection>.Failure(access.Error);
        if (input is null || input.ItemId == Guid.Empty) return Fail<BomProjection>("PRODUCTION_FIELD_REQUIRED", "A produced item is required.");
        var valid = ValidateDraft(new BomDraftInput(input.OutputQuantity, input.Note, input.Lines));
        if (valid.IsFailure) return Result<BomProjection>.Failure(valid.Error);

        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute($"{input.ItemId}|{Num(input.OutputQuantity)}|{input.Note?.Trim()}|{LinesKey(input.Lines)}");
        return await _store.CreateBomAsync(access.Value!, input, keyHash, payloadHash, caller.TraceId, ct);
    }

    public async Task<Result<BomProjection>> CreateRevisionAsync(ProductionCaller caller, Guid bomId, BomDraftInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "boms.manage", ct);
        if (access.IsFailure) return Result<BomProjection>.Failure(access.Error);
        var valid = ValidateDraft(input);
        if (valid.IsFailure) return Result<BomProjection>.Failure(valid.Error);
        return await _store.CreateBomRevisionAsync(access.Value!, bomId, valid.Value!, caller.TraceId, ct);
    }

    public async Task<Result<BomProjection>> UpdateDraftAsync(ProductionCaller caller, Guid bomId, Guid revisionId, Guid expectedVersion, BomDraftInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "boms.manage", ct);
        if (access.IsFailure) return Result<BomProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<BomProjection>("PRODUCTION_FIELD_REQUIRED", "Expected version is required.");
        var valid = ValidateDraft(input);
        if (valid.IsFailure) return Result<BomProjection>.Failure(valid.Error);
        return await _store.UpdateBomDraftAsync(access.Value!, bomId, revisionId, expectedVersion, valid.Value!, caller.TraceId, ct);
    }

    public async Task<Result<BomProjection>> BomActionAsync(ProductionCaller caller, Guid bomId, Guid revisionId, Guid expectedVersion, BomAction action, CancellationToken ct = default)
    {
        var permission = action == BomAction.Approve ? "boms.approve" : "boms.manage";
        var access = await AccessAsync(caller, permission, ct);
        if (access.IsFailure) return Result<BomProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<BomProjection>("PRODUCTION_FIELD_REQUIRED", "Expected version is required.");
        return await _store.BomActionAsync(access.Value!, bomId, revisionId, expectedVersion, action, caller.TraceId, ct);
    }

    public async Task<Result<BomProjection>> GetBomAsync(ProductionCaller caller, Guid bomId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "boms.read", ct);
        if (access.IsFailure) return Result<BomProjection>.Failure(access.Error);
        var bom = await _store.GetBomAsync(access.Value!.OrganizationId, bomId, ct);
        return bom is null ? Fail<BomProjection>("RESOURCE_NOT_FOUND", "BOM not found.") : Result<BomProjection>.Success(bom);
    }

    public async Task<Result<PagedBoms>> ListBomsAsync(ProductionCaller caller, string? search, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "boms.read", ct);
        if (access.IsFailure) return Result<PagedBoms>.Failure(access.Error);
        var (p, size) = Paging(page, pageSize);
        return Result<PagedBoms>.Success(await _store.ListBomsAsync(access.Value!.OrganizationId, new BomListQuery(Clean(search), p, size), ct));
    }

    // ----- work orders -----------------------------------------------------------------------------

    public async Task<Result<WorkOrderProjection>> CreateWorkOrderAsync(ProductionCaller caller, string idempotencyKey, WorkOrderInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "work-orders.manage", ct);
        if (access.IsFailure) return Result<WorkOrderProjection>.Failure(access.Error);
        if (input is null || input.ItemId == Guid.Empty || input.WarehouseId == Guid.Empty)
        {
            return Fail<WorkOrderProjection>("PRODUCTION_FIELD_REQUIRED", "A produced item and a warehouse are required.");
        }

        if (input.PlannedQuantity <= 0) return Fail<WorkOrderProjection>("PRODUCTION_QUANTITY_INVALID", "Planned quantity must be greater than zero.");
        if (input.Note is { Length: > 500 }) return Fail<WorkOrderProjection>("PRODUCTION_FIELD_INVALID", "Note cannot exceed 500 characters.");

        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute($"{input.ItemId}|{input.WarehouseId}|{input.ProjectId}|{Num(input.PlannedQuantity)}|{input.Note?.Trim()}");
        return await _store.CreateWorkOrderAsync(access.Value!, input, keyHash, payloadHash, caller.TraceId, ct);
    }

    public async Task<Result<WorkOrderProjection>> WorkOrderActionAsync(ProductionCaller caller, Guid workOrderId, Guid expectedVersion, WorkOrderAction action, string? reason, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "work-orders.manage", ct);
        if (access.IsFailure) return Result<WorkOrderProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<WorkOrderProjection>("PRODUCTION_FIELD_REQUIRED", "Expected version is required.");
        if (reason is { Length: > 500 }) return Fail<WorkOrderProjection>("PRODUCTION_FIELD_INVALID", "Reason cannot exceed 500 characters.");
        if (action == WorkOrderAction.Cancel && string.IsNullOrWhiteSpace(reason)) return Fail<WorkOrderProjection>("PRODUCTION_REASON_REQUIRED", "A reason is required to cancel a work order.");
        return await _store.WorkOrderActionAsync(access.Value!, workOrderId, expectedVersion, action, Clean(reason), caller.TraceId, ct);
    }

    private static Result<IReadOnlyList<WorkOrderMaterialQuantityInput>> ValidateMaterialLines(IReadOnlyList<WorkOrderMaterialQuantityInput>? lines)
    {
        if (lines is null || lines.Count == 0 || lines.Count > MaxLines || lines.Any(l => l.Quantity <= 0) || lines.GroupBy(l => l.ItemId).Any(g => g.Count() > 1))
        {
            return Fail<IReadOnlyList<WorkOrderMaterialQuantityInput>>("PRODUCTION_QUANTITY_INVALID", "Lines need unique items and quantities above zero.");
        }

        return Result<IReadOnlyList<WorkOrderMaterialQuantityInput>>.Success(lines);
    }

    private static string MaterialKey(Guid workOrderId, IEnumerable<WorkOrderMaterialQuantityInput> lines) =>
        $"{workOrderId}|{string.Join(";", lines.OrderBy(l => l.ItemId).Select(l => $"{l.ItemId}:{Num(l.Quantity)}"))}";

    public async Task<Result<WorkOrderProjection>> IssueAsync(ProductionCaller caller, Guid workOrderId, string idempotencyKey, IReadOnlyList<WorkOrderMaterialQuantityInput>? lines, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "work-orders.operate", ct);
        if (access.IsFailure) return Result<WorkOrderProjection>.Failure(access.Error);
        var valid = ValidateMaterialLines(lines);
        if (valid.IsFailure) return Result<WorkOrderProjection>.Failure(valid.Error);
        return await _store.IssueMaterialsAsync(access.Value!, workOrderId, valid.Value!, Sha256Hex.Compute(idempotencyKey), Sha256Hex.Compute(MaterialKey(workOrderId, valid.Value!)), caller.TraceId, ct);
    }

    public async Task<Result<WorkOrderProjection>> ReturnAsync(ProductionCaller caller, Guid workOrderId, string idempotencyKey, IReadOnlyList<WorkOrderMaterialQuantityInput>? lines, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "work-orders.operate", ct);
        if (access.IsFailure) return Result<WorkOrderProjection>.Failure(access.Error);
        var valid = ValidateMaterialLines(lines);
        if (valid.IsFailure) return Result<WorkOrderProjection>.Failure(valid.Error);
        return await _store.ReturnMaterialsAsync(access.Value!, workOrderId, valid.Value!, Sha256Hex.Compute(idempotencyKey), Sha256Hex.Compute(MaterialKey(workOrderId, valid.Value!)), caller.TraceId, ct);
    }

    public async Task<Result<WorkOrderProjection>> CompleteAsync(ProductionCaller caller, Guid workOrderId, string idempotencyKey, decimal quantity, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "work-orders.operate", ct);
        if (access.IsFailure) return Result<WorkOrderProjection>.Failure(access.Error);
        if (quantity <= 0) return Fail<WorkOrderProjection>("PRODUCTION_QUANTITY_INVALID", "Completed quantity must be greater than zero.");
        return await _store.CompleteAsync(access.Value!, workOrderId, quantity, Sha256Hex.Compute(idempotencyKey), Sha256Hex.Compute($"{workOrderId}|{Num(quantity)}"), caller.TraceId, ct);
    }

    public async Task<Result<WorkOrderProjection>> GetWorkOrderAsync(ProductionCaller caller, Guid workOrderId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "work-orders.read", ct);
        if (access.IsFailure) return Result<WorkOrderProjection>.Failure(access.Error);
        var order = await _store.GetWorkOrderAsync(access.Value!.OrganizationId, workOrderId, ct);
        return order is null ? Fail<WorkOrderProjection>("RESOURCE_NOT_FOUND", "Work order not found.") : Result<WorkOrderProjection>.Success(order);
    }

    public async Task<Result<PagedWorkOrders>> ListWorkOrdersAsync(ProductionCaller caller, string? search, string? status, Guid? projectId, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "work-orders.read", ct);
        if (access.IsFailure) return Result<PagedWorkOrders>.Failure(access.Error);
        if (!string.IsNullOrWhiteSpace(status) && !Domain.Production.WorkOrderStatus.All.Contains(status.Trim()))
        {
            return Fail<PagedWorkOrders>("PRODUCTION_FIELD_INVALID", "Work order status filter is invalid.");
        }

        var (p, size) = Paging(page, pageSize);
        return Result<PagedWorkOrders>.Success(await _store.ListWorkOrdersAsync(access.Value!.OrganizationId, new WorkOrderListQuery(Clean(search), Clean(status), projectId, p, size), ct));
    }
}
