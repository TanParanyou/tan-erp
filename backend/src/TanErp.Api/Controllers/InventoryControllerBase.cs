using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Inventory;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Inventory;

namespace TanErp.Api.Controllers;

public abstract class InventoryControllerBase : ControllerBase
{
    protected InventoryCaller? ReadAuthenticated(out IActionResult? failure)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            failure = ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
            return null;
        }

        failure = null;
        var auth = contextResult.Value!;
        return new InventoryCaller(auth.FirebaseUid, auth.MembershipId, HttpContext.TraceIdentifier);
    }

    protected InventoryCaller? ReadConditional(out Guid expectedVersion, out IActionResult? failure)
    {
        var contextResult = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            expectedVersion = Guid.Empty;
            failure = ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
            return null;
        }

        failure = null;
        var auth = contextResult.Value!;
        expectedVersion = auth.IfMatchRowVersion;
        return new InventoryCaller(auth.FirebaseUid, auth.MembershipId, HttpContext.TraceIdentifier);
    }

    protected InventoryCaller? ReadIdempotent(out string idempotencyKey, out IActionResult? failure)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            idempotencyKey = string.Empty;
            failure = ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
            return null;
        }

        failure = null;
        var auth = contextResult.Value!;
        idempotencyKey = auth.IdempotencyKey;
        return new InventoryCaller(auth.FirebaseUid, auth.MembershipId, HttpContext.TraceIdentifier);
    }

    protected IActionResult Problem(string code) => ProblemDetailsMapper.CreateProblemResult(code, HttpContext);

    protected static InventoryPaginationResponse Pagination(int page, int pageSize, int total) =>
        new(page, pageSize, total, pageSize == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize));

    protected static InventoryWarehouseRefResponse ToResponse(InventoryWarehouseRef w) => new(w.Id, w.Code, w.Name);

    protected static InventoryItemRefResponse ToResponse(InventoryItemRef i) => new(i.Id, i.Code, i.NameTh, i.UnitCode);

    protected static StockMovementResponse ToResponse(StockMovementProjection m) => new(
        m.Id, m.StockDocumentId, m.DocumentType, m.DocumentNumber, ToResponse(m.Warehouse), ToResponse(m.Item), m.Kind,
        m.QuantityDelta, m.UnitCost, m.ValueDelta, m.OnHandAfter, m.OccurredAtUtc, m.PostedAtUtc);

    protected static StockDocumentResponse ToResponse(StockDocumentProjection d) => new(
        d.Id, d.DocumentType, d.Number, ToResponse(d.Warehouse), d.ToWarehouse is null ? null : ToResponse(d.ToWarehouse),
        d.ProjectId, d.SourceType, d.SourceId, d.Reason, d.OccurredAtUtc, new InventoryPersonResponse(d.PostedBy.Id, d.PostedBy.DisplayName, d.PostedBy.Email),
        d.PostedAtUtc, d.Movements.Select(ToResponse).ToList());

    protected static ReservationResponse ToResponse(ReservationProjection r) => new(
        r.Id, ToResponse(r.Warehouse), ToResponse(r.Item), r.ProjectId, r.ProjectCode, r.Quantity, r.Status, r.Note, r.RowVersion, r.CreatedAtUtc);
}
