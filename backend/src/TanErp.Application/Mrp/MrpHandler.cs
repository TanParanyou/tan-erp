using System.Globalization;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Procurement;
using TanErp.Application.Production;
using TanErp.Domain.Mrp;

namespace TanErp.Application.Mrp;

public sealed record MrpCaller(string FirebaseUid, Guid MembershipId, string TraceId);

/// <summary>
/// Planning-run use cases. A run is a frozen snapshot plus its plan; recommendations only become Purchase Orders or
/// Work Orders after an independent approval, and always as drafts that follow those modules' own workflows.
/// </summary>
public class MrpHandler
{
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 25;
    public const int MaxDemands = 200;
    public const int MaxLeadTimeDays = 365;

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IMrpStore _store;
    private readonly IProcurementStore _procurement;
    private readonly IProductionStore _production;

    public MrpHandler(IRequestAccessResolver accessResolver, IMrpStore store, IProcurementStore procurement, IProductionStore production)
    {
        _accessResolver = accessResolver;
        _store = store;
        _procurement = procurement;
        _production = production;
    }

    private Task<Result<RequestAccessContext>> AccessAsync(MrpCaller caller, string permission, CancellationToken ct) =>
        _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, ct);

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static string Num(decimal value) => value.ToString(CultureInfo.InvariantCulture);

    public async Task<Result<MrpRunProjection>> CreateRunAsync(MrpCaller caller, string idempotencyKey, MrpRunInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "mrp.run", ct);
        if (access.IsFailure) return Result<MrpRunProjection>.Failure(access.Error);
        if (input is null) return Fail<MrpRunProjection>("MRP_FIELD_REQUIRED", "A planning request is required.");
        if (input.PurchaseLeadTimeDays is < 0 or > MaxLeadTimeDays || input.ProductionLeadTimeDays is < 0 or > MaxLeadTimeDays)
        {
            return Fail<MrpRunProjection>("MRP_FIELD_INVALID", $"Lead times must be between 0 and {MaxLeadTimeDays} days.");
        }

        var demands = input.Demands ?? Array.Empty<MrpDemandRequestInput>();
        if (demands.Count > MaxDemands || demands.Any(d => d.ItemId == Guid.Empty || d.Quantity <= 0 || d.NeedBy < input.AsOfDate || d.Reference is { Length: > 100 }))
        {
            return Fail<MrpRunProjection>("MRP_DEMAND_INVALID", "Demands need an item, a quantity above zero and a need date on or after the as-of date.");
        }

        if (demands.Count == 0 && !input.IncludeOpenWorkOrders)
        {
            return Fail<MrpRunProjection>("MRP_DEMAND_INVALID", "Add at least one demand or include open work orders.");
        }

        var lines = string.Join(";", demands.OrderBy(d => d.ItemId).ThenBy(d => d.NeedBy).Select(d => $"{d.ItemId}:{Num(d.Quantity)}:{d.NeedBy:O}:{d.Reference?.Trim()}"));
        var payloadHash = Sha256Hex.Compute($"{input.AsOfDate:O}|{input.PurchaseLeadTimeDays}|{input.ProductionLeadTimeDays}|{input.IncludeOpenWorkOrders}|{lines}");
        return await _store.CreateRunAsync(access.Value!, input with { Demands = demands }, Sha256Hex.Compute(idempotencyKey), payloadHash, caller.TraceId, ct);
    }

    public async Task<Result<MrpRunProjection>> GetRunAsync(MrpCaller caller, Guid runId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "mrp.read", ct);
        if (access.IsFailure) return Result<MrpRunProjection>.Failure(access.Error);
        var run = await _store.GetRunAsync(access.Value!.OrganizationId, runId, ct);
        return run is null ? Fail<MrpRunProjection>("RESOURCE_NOT_FOUND", "Planning run not found.") : Result<MrpRunProjection>.Success(run);
    }

    public async Task<Result<PagedMrpRuns>> ListRunsAsync(MrpCaller caller, string? search, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "mrp.read", ct);
        if (access.IsFailure) return Result<PagedMrpRuns>.Failure(access.Error);
        var size = pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);
        return Result<PagedMrpRuns>.Success(await _store.ListRunsAsync(access.Value!.OrganizationId, new MrpRunListQuery(string.IsNullOrWhiteSpace(search) ? null : search.Trim(), Math.Max(1, page), size), ct));
    }

    public async Task<Result<MrpRunProjection>> DecideAsync(MrpCaller caller, Guid runId, Guid recommendationId, Guid expectedVersion, bool approve, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "mrp.approve", ct);
        if (access.IsFailure) return Result<MrpRunProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<MrpRunProjection>("MRP_FIELD_REQUIRED", "Expected version is required.");
        return await _store.DecideAsync(access.Value!, runId, recommendationId, expectedVersion, approve, caller.TraceId, ct);
    }

    /// <summary>
    /// Turns an approved recommendation into a draft Work Order ("make") or draft Purchase Order ("buy") through those
    /// modules' own stores, keyed by the recommendation id so a retry returns the same document, then records the link.
    /// </summary>
    public async Task<Result<MrpRunProjection>> ConvertAsync(MrpCaller caller, Guid runId, Guid recommendationId, Guid expectedVersion, MrpConvertInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "mrp.convert", ct);
        if (access.IsFailure) return Result<MrpRunProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<MrpRunProjection>("MRP_FIELD_REQUIRED", "Expected version is required.");

        var target = await _store.GetConvertTargetAsync(access.Value!.OrganizationId, runId, recommendationId, ct);
        if (target is null) return Fail<MrpRunProjection>("RESOURCE_NOT_FOUND", "Recommendation not found.");
        if (target.RowVersion != expectedVersion) return Fail<MrpRunProjection>("MRP_VERSION_CONFLICT", "Recommendation version conflict.");
        if (target.Status != MrpRecommendationStatus.Approved) return Fail<MrpRunProjection>("MRP_INVALID_STATE", "Only an approved recommendation can be converted.");

        var keyHash = Sha256Hex.Compute($"mrp-convert:{target.RecommendationId}");
        var note = $"MRP {target.RunNumber} #{target.LineNo}";

        if (target.Action == MrpAction.Make)
        {
            if (input?.WarehouseId is null || input.WarehouseId == Guid.Empty) return Fail<MrpRunProjection>("MRP_CONVERT_INPUT_REQUIRED", "A warehouse is required to create a work order.");
            var production = await AccessAsync(caller, "work-orders.manage", ct);
            if (production.IsFailure) return Result<MrpRunProjection>.Failure(production.Error);

            var order = await _production.CreateWorkOrderAsync(
                production.Value!, new WorkOrderInput(target.ItemId, input.WarehouseId.Value, null, target.Quantity, note),
                keyHash, Sha256Hex.Compute($"{target.ItemId}|{input.WarehouseId}|{Num(target.Quantity)}"), caller.TraceId, ct);
            if (order.IsFailure) return Result<MrpRunProjection>.Failure(order.Error);
            return await _store.MarkConvertedAsync(access.Value!, runId, recommendationId, expectedVersion, "work_order", order.Value!.Id, order.Value.Number, caller.TraceId, ct);
        }

        if (target.Action == MrpAction.Buy)
        {
            if (input?.SupplierId is null || input.SupplierId == Guid.Empty || input.UnitPrice is null || input.UnitPrice < 0)
            {
                return Fail<MrpRunProjection>("MRP_CONVERT_INPUT_REQUIRED", "A supplier and a unit price are required to create a purchase order.");
            }

            var purchasing = await AccessAsync(caller, "purchase-orders.create", ct);
            if (purchasing.IsFailure) return Result<MrpRunProjection>.Failure(purchasing.Error);

            var order = await _procurement.CreatePurchaseOrderAsync(
                purchasing.Value!,
                new PurchaseOrderInput(input.SupplierId.Value, null, target.NeedBy, note, new[] { new PurchaseOrderLineInput(target.ItemId, target.Quantity, input.UnitPrice.Value) }),
                keyHash, Sha256Hex.Compute($"{input.SupplierId}|{target.ItemId}|{Num(target.Quantity)}|{Num(input.UnitPrice.Value)}|{target.NeedBy:O}"), caller.TraceId, ct);
            if (order.IsFailure) return Result<MrpRunProjection>.Failure(order.Error);
            return await _store.MarkConvertedAsync(access.Value!, runId, recommendationId, expectedVersion, "purchase_order", order.Value!.Id, order.Value.Number, caller.TraceId, ct);
        }

        return Fail<MrpRunProjection>("MRP_NOT_CONVERTIBLE", "This recommendation has no sourcing route and cannot be converted.");
    }
}
