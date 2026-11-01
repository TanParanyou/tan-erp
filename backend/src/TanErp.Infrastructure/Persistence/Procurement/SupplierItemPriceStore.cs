using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Procurement;
using TanErp.Domain.Common;
using TanErp.Domain.Procurement;

namespace TanErp.Infrastructure.Persistence.Procurement;

public class SupplierItemPriceStore : ISupplierItemPriceStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public SupplierItemPriceStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private void Audit(RequestAccessContext access, string action, Guid id, string traceId, object changes, Guid? rowVersionAfter, DateTimeOffset now)
    {
        _db.AuditEvents.Add(new AuditEvent(
            Guid.NewGuid(), access.OrganizationId, access.ActorUserId, action, "SupplierItemPrice", id.ToString(), now, traceId,
            JsonSerializer.Serialize(changes),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            rowVersionAfter: rowVersionAfter));
    }

    // Serialises price edits of one item so the overlap and single-preferred checks cannot race.
    private Task LockItemAsync(Guid orgId, Guid itemId, CancellationToken ct)
    {
        var lockKey = $"{orgId:N}:supplier-item-price:{itemId:N}";
        return _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))", ct);
    }

    private async Task<SupplierItemPriceProjection> ProjectAsync(SupplierItemPrice p, CancellationToken ct)
    {
        var item = await _db.Items.AsNoTracking().FirstAsync(i => i.Id == p.ItemId, ct);
        return ToProjection(p, item.Code, item.Name.Thai);
    }

    private static SupplierItemPriceProjection ToProjection(SupplierItemPrice p, string itemCode, string itemNameTh) => new(
        p.Id, p.SupplierId, p.ItemId, itemCode, itemNameTh, p.UnitPrice, p.MinOrderQuantity, p.LeadTimeDays,
        p.EffectiveFrom, p.EffectiveTo, p.IsPreferred, p.RowVersion);

    // Shared by create and update; returns a failure when the period overlaps another row or the supplier/item is unusable.
    private async Task<Result<SupplierItemPrice>> SaveAsync(
        RequestAccessContext access, Guid supplierId, SupplierItemPriceInput input, SupplierItemPrice? existing, Guid expectedVersion,
        string traceId, CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        var supplier = await _db.Suppliers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == supplierId && s.OrganizationId == orgId, ct);
        if (supplier is null) return Fail<SupplierItemPrice>("RESOURCE_NOT_FOUND", "Supplier not found.");
        if (supplier.Status != SupplierStatus.Active) return Fail<SupplierItemPrice>("SUPPLIER_INVALID_STATE", "The supplier is not active.");
        var itemExists = await _db.Items.AsNoTracking().AnyAsync(i => i.Id == input.ItemId && i.OrganizationId == orgId, ct);
        if (!itemExists) return Fail<SupplierItemPrice>("RESOURCE_NOT_FOUND", "Item not found.");

        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        await LockItemAsync(orgId, input.ItemId, ct);

        var now = _clock.UtcNow;
        var sameItem = await _db.SupplierItemPrices
            .Where(x => x.OrganizationId == orgId && x.ItemId == input.ItemId)
            .ToListAsync(ct);

        var sameSupplierOverlap = sameItem.Any(x => x.SupplierId == supplierId && x.Id != existing?.Id && x.Overlaps(input.EffectiveFrom, input.EffectiveTo));
        if (sameSupplierOverlap) return Fail<SupplierItemPrice>("SUPPLIER_PRICE_PERIOD_OVERLAP", "The effective period overlaps another price of this supplier for the item.");

        SupplierItemPrice price;
        try
        {
            if (existing is null)
            {
                price = new SupplierItemPrice(Guid.NewGuid(), orgId, supplierId, input.ItemId, input.UnitPrice, input.MinOrderQuantity,
                    input.LeadTimeDays, input.EffectiveFrom, input.EffectiveTo, input.IsPreferred, access.ActorUserId, now);
                _db.SupplierItemPrices.Add(price);
            }
            else
            {
                price = sameItem.First(x => x.Id == existing.Id);
                if (price.RowVersion != expectedVersion) return Fail<SupplierItemPrice>("SUPPLIER_PRICE_VERSION_CONFLICT", "Supplier price version conflict.");
                price.Update(input.UnitPrice, input.MinOrderQuantity, input.LeadTimeDays, input.EffectiveFrom, input.EffectiveTo, input.IsPreferred, access.ActorUserId, now);
            }
        }
        catch (ProcurementDomainException ex)
        {
            return Fail<SupplierItemPrice>(ex.Code, ex.Message);
        }

        // Only one preferred supplier per item may be effective at once, so a new preferred row clears overlapping preferred rows of other suppliers.
        if (price.IsPreferred)
        {
            foreach (var other in sameItem.Where(x => x.Id != price.Id && x.IsPreferred && x.Overlaps(price.EffectiveFrom, price.EffectiveTo)))
            {
                other.ClearPreferred(access.ActorUserId, now);
            }
        }

        Audit(access, existing is null ? "supplier-item-price.created" : "supplier-item-price.updated", price.Id, traceId,
            new { supplierId, itemId = price.ItemId, price.UnitPrice, price.LeadTimeDays, price.IsPreferred }, price.RowVersion, now);
        try
        {
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Fail<SupplierItemPrice>("SUPPLIER_PRICE_VERSION_CONFLICT", "Supplier price version conflict.");
        }

        return Result<SupplierItemPrice>.Success(price);
    }

    public async Task<Result<SupplierItemPriceProjection>> CreateAsync(
        RequestAccessContext access, Guid supplierId, SupplierItemPriceInput input, string traceId, CancellationToken ct = default)
    {
        var saved = await SaveAsync(access, supplierId, input, null, Guid.Empty, traceId, ct);
        return saved.IsFailure ? Result<SupplierItemPriceProjection>.Failure(saved.Error) : Result<SupplierItemPriceProjection>.Success(await ProjectAsync(saved.Value!, ct));
    }

    public async Task<Result<SupplierItemPriceProjection>> UpdateAsync(
        RequestAccessContext access, Guid supplierId, Guid priceId, Guid expectedVersion, SupplierItemPriceInput input, string traceId, CancellationToken ct = default)
    {
        var existing = await _db.SupplierItemPrices.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == priceId && x.SupplierId == supplierId && x.OrganizationId == access.OrganizationId, ct);
        if (existing is null) return Fail<SupplierItemPriceProjection>("RESOURCE_NOT_FOUND", "Supplier price not found.");
        if (existing.ItemId != input.ItemId) return Fail<SupplierItemPriceProjection>("SUPPLIER_PRICE_INVALID", "The item of a price row cannot be changed; delete it and add a new one.");

        var saved = await SaveAsync(access, supplierId, input, existing, expectedVersion, traceId, ct);
        return saved.IsFailure ? Result<SupplierItemPriceProjection>.Failure(saved.Error) : Result<SupplierItemPriceProjection>.Success(await ProjectAsync(saved.Value!, ct));
    }

    public async Task<Result<bool>> DeleteAsync(
        RequestAccessContext access, Guid supplierId, Guid priceId, Guid expectedVersion, string traceId, CancellationToken ct = default)
    {
        var price = await _db.SupplierItemPrices.FirstOrDefaultAsync(
            x => x.Id == priceId && x.SupplierId == supplierId && x.OrganizationId == access.OrganizationId, ct);
        if (price is null) return Fail<bool>("RESOURCE_NOT_FOUND", "Supplier price not found.");
        if (price.RowVersion != expectedVersion) return Fail<bool>("SUPPLIER_PRICE_VERSION_CONFLICT", "Supplier price version conflict.");

        _db.SupplierItemPrices.Remove(price);
        Audit(access, "supplier-item-price.deleted", price.Id, traceId, new { supplierId, itemId = price.ItemId }, null, _clock.UtcNow);
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Fail<bool>("SUPPLIER_PRICE_VERSION_CONFLICT", "Supplier price version conflict.");
        }

        return Result<bool>.Success(true);
    }

    public async Task<PagedSupplierItemPrices> ListAsync(Guid organizationId, Guid supplierId, Guid? itemId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.SupplierItemPrices.AsNoTracking().Where(x => x.OrganizationId == organizationId && x.SupplierId == supplierId);
        if (itemId.HasValue) query = query.Where(x => x.ItemId == itemId.Value);

        var total = await query.CountAsync(ct);
        var rows = await (from p in query
                          join i in _db.Items.AsNoTracking() on p.ItemId equals i.Id
                          orderby i.Code, p.EffectiveFrom descending, p.Id
                          select new { Price = p, i.Code, NameTh = i.Name.Thai })
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedSupplierItemPrices(rows.Select(r => ToProjection(r.Price, r.Code, r.NameTh)).ToList(), total, page, pageSize);
    }

    public async Task<SupplierItemPriceProjection?> ResolveEffectiveAsync(Guid organizationId, Guid supplierId, Guid itemId, DateOnly onDate, CancellationToken ct = default)
    {
        var row = await (from p in _db.SupplierItemPrices.AsNoTracking()
                         join i in _db.Items.AsNoTracking() on p.ItemId equals i.Id
                         where p.OrganizationId == organizationId && p.SupplierId == supplierId && p.ItemId == itemId
                               && p.EffectiveFrom <= onDate && (p.EffectiveTo == null || p.EffectiveTo >= onDate)
                         select new { Price = p, i.Code, NameTh = i.Name.Thai })
            .FirstOrDefaultAsync(ct);
        return row is null ? null : ToProjection(row.Price, row.Code, row.NameTh);
    }
}
