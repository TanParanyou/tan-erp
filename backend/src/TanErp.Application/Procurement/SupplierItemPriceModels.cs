using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Procurement;

public sealed record SupplierItemPriceInput(
    Guid ItemId,
    decimal UnitPrice,
    decimal MinOrderQuantity,
    int LeadTimeDays,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    bool IsPreferred);

public sealed record SupplierItemPriceProjection(
    Guid Id,
    Guid SupplierId,
    Guid ItemId,
    string ItemCode,
    string ItemNameTh,
    decimal UnitPrice,
    decimal MinOrderQuantity,
    int LeadTimeDays,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    bool IsPreferred,
    Guid RowVersion);

public sealed record PagedSupplierItemPrices(IReadOnlyList<SupplierItemPriceProjection> Items, int TotalCount, int Page, int PageSize);

public interface ISupplierItemPriceStore
{
    Task<Result<SupplierItemPriceProjection>> CreateAsync(RequestAccessContext access, Guid supplierId, SupplierItemPriceInput input, string traceId, CancellationToken ct = default);
    Task<Result<SupplierItemPriceProjection>> UpdateAsync(RequestAccessContext access, Guid supplierId, Guid priceId, Guid expectedVersion, SupplierItemPriceInput input, string traceId, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(RequestAccessContext access, Guid supplierId, Guid priceId, Guid expectedVersion, string traceId, CancellationToken ct = default);
    Task<PagedSupplierItemPrices> ListAsync(Guid organizationId, Guid supplierId, Guid? itemId, int page, int pageSize, CancellationToken ct = default);

    /// <summary>The price valid on <paramref name="onDate"/> for a supplier/item pair, or null when none is configured.</summary>
    Task<SupplierItemPriceProjection?> ResolveEffectiveAsync(Guid organizationId, Guid supplierId, Guid itemId, DateOnly onDate, CancellationToken ct = default);
}
