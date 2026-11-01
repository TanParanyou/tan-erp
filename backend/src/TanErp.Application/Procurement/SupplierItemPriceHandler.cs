using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Procurement;

/// <summary>Supplier price list use cases. Reuses the existing suppliers.read / suppliers.manage permissions.</summary>
public class SupplierItemPriceHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly ISupplierItemPriceStore _store;

    public SupplierItemPriceHandler(IRequestAccessResolver accessResolver, ISupplierItemPriceStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    private Task<Result<TanErp.Application.Common.Models.RequestAccessContext>> AccessAsync(ProcurementCaller caller, string permission, CancellationToken ct) =>
        _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, ct);

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static bool IsValid(SupplierItemPriceInput? input) =>
        input is not null && input.ItemId != Guid.Empty;

    public async Task<Result<SupplierItemPriceProjection>> CreateAsync(ProcurementCaller caller, Guid supplierId, SupplierItemPriceInput input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "suppliers.manage", ct);
        if (access.IsFailure) return Result<SupplierItemPriceProjection>.Failure(access.Error);
        if (!IsValid(input)) return Fail<SupplierItemPriceProjection>("SUPPLIER_PRICE_INVALID", "Item is required.");
        return await _store.CreateAsync(access.Value!, supplierId, input, caller.TraceId, ct);
    }

    public async Task<Result<SupplierItemPriceProjection>> UpdateAsync(ProcurementCaller caller, Guid supplierId, Guid priceId, Guid expectedVersion, SupplierItemPriceInput input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "suppliers.manage", ct);
        if (access.IsFailure) return Result<SupplierItemPriceProjection>.Failure(access.Error);
        if (!IsValid(input) || expectedVersion == Guid.Empty) return Fail<SupplierItemPriceProjection>("SUPPLIER_PRICE_INVALID", "Item and expected version are required.");
        return await _store.UpdateAsync(access.Value!, supplierId, priceId, expectedVersion, input, caller.TraceId, ct);
    }

    public async Task<Result<bool>> DeleteAsync(ProcurementCaller caller, Guid supplierId, Guid priceId, Guid expectedVersion, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "suppliers.manage", ct);
        if (access.IsFailure) return Result<bool>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<bool>("SUPPLIER_PRICE_INVALID", "Expected version is required.");
        return await _store.DeleteAsync(access.Value!, supplierId, priceId, expectedVersion, caller.TraceId, ct);
    }

    public async Task<Result<PagedSupplierItemPrices>> ListAsync(ProcurementCaller caller, Guid supplierId, Guid? itemId, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "suppliers.read", ct);
        if (access.IsFailure) return Result<PagedSupplierItemPrices>.Failure(access.Error);
        var p = Math.Max(1, page);
        var size = pageSize <= 0 ? ProcurementHandler.DefaultPageSize : Math.Min(pageSize, ProcurementHandler.MaxPageSize);
        return Result<PagedSupplierItemPrices>.Success(await _store.ListAsync(access.Value!.OrganizationId, supplierId, itemId, p, size, ct));
    }

    public async Task<Result<SupplierItemPriceProjection>> ResolveEffectiveAsync(ProcurementCaller caller, Guid supplierId, Guid itemId, DateOnly onDate, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "suppliers.read", ct);
        if (access.IsFailure) return Result<SupplierItemPriceProjection>.Failure(access.Error);
        var price = await _store.ResolveEffectiveAsync(access.Value!.OrganizationId, supplierId, itemId, onDate, ct);
        return price is null
            ? Fail<SupplierItemPriceProjection>("SUPPLIER_PRICE_NOT_FOUND", "No supplier price is effective for this item on the requested date.")
            : Result<SupplierItemPriceProjection>.Success(price);
    }
}
