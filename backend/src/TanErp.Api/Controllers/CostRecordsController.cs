using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Items;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Items;
using TanErp.Domain.Items;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/items/{itemId:guid}/costs")]
[Authorize]
public class CostRecordsController : ControllerBase
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly ICostRecordStore _costStore;

    public CostRecordsController(
        IRequestAccessResolver accessResolver,
        ICostRecordStore costStore)
    {
        _accessResolver = accessResolver;
        _costStore = costStore;
    }

    [HttpPost]
    [ProducesResponseType<CostRecordResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateDraft(
        [FromRoute] Guid itemId,
        [FromBody] CreateCostRecordRequest request,
        CancellationToken ct)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);

        var auth = authResult.Value!;
        var accessResult = string.Equals(request.Scope, CostScopeType.Branch, StringComparison.OrdinalIgnoreCase) && request.BranchId.HasValue
            ? await _accessResolver.ResolveBranchAccessAsync(auth.FirebaseUid, auth.MembershipId, "cost-records.create", request.BranchId.Value, ct)
            : await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "cost-records.create", ct);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);

        var access = accessResult.Value!;
        var data = new CreateCostRecordData(
            itemId,
            request.Scope,
            request.BranchId,
            request.UnitId,
            request.Currency,
            request.Amount,
            request.MinimumQuantity,
            request.MaximumQuantity,
            request.EffectiveFromUtc,
            request.EffectiveToUtc,
            request.CostSourceId,
            request.SourceReference,
            request.Reason,
            request.EvidenceFileId);

        var result = await _costStore.CreateDraftAsync(data, access, ct);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var response = ItemResponseMapper.ToResponse(result.Value!);
        Response.Headers.ETag = $"\"{response.RowVersion}\"";
        return CreatedAtAction(nameof(GetById), new { itemId, costId = response.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType<List<CostRecordResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListForItem(
        [FromRoute] Guid itemId,
        CancellationToken ct)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "cost-records.read", ct);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);

        var access = accessResult.Value!;
        var list = await _costStore.ListForItemAsync(access.OrganizationId, itemId, ct);
        var response = list.Select(ItemResponseMapper.ToResponse).ToList();
        return Ok(response);
    }

    [HttpGet("{costId:guid}")]
    [ProducesResponseType<CostRecordResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid itemId,
        [FromRoute] Guid costId,
        CancellationToken ct)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "cost-records.read", ct);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);

        var access = accessResult.Value!;
        var projection = await _costStore.GetByIdAsync(access.OrganizationId, itemId, costId, ct);
        if (projection == null)
        {
            return ProblemDetailsMapper.CreateProblemResult("COST_RECORD_NOT_FOUND", HttpContext);
        }

        var response = ItemResponseMapper.ToResponse(projection);
        Response.Headers.ETag = $"\"{response.RowVersion}\"";
        return Ok(response);
    }

    [HttpPut("{costId:guid}")]
    [ProducesResponseType<CostRecordResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateDraft(
        [FromRoute] Guid itemId,
        [FromRoute] Guid costId,
        [FromBody] UpdateCostRecordRequest request,
        CancellationToken ct)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "cost-records.create", ct);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);

        var access = accessResult.Value!;
        var ifMatch = ParseIfMatchHeader();

        var data = new UpdateCostRecordData(
            costId,
            itemId,
            request.Amount,
            request.Currency,
            request.UnitId,
            request.MinimumQuantity,
            request.MaximumQuantity,
            request.EffectiveFromUtc,
            request.EffectiveToUtc,
            request.CostSourceId,
            request.SourceReference,
            request.Reason,
            request.EvidenceFileId);

        var result = await _costStore.UpdateDraftAsync(data, access, ifMatch, ct);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var response = ItemResponseMapper.ToResponse(result.Value!);
        Response.Headers.ETag = $"\"{response.RowVersion}\"";
        return Ok(response);
    }

    [HttpPost("{costId:guid}/submit")]
    [ProducesResponseType<CostRecordResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Submit(
        [FromRoute] Guid itemId,
        [FromRoute] Guid costId,
        CancellationToken ct)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "cost-records.submit", ct);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);

        var access = accessResult.Value!;
        var ifMatch = ParseIfMatchHeader();
        var result = await _costStore.SubmitAsync(access.OrganizationId, itemId, costId, access, ifMatch, ct);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var response = ItemResponseMapper.ToResponse(result.Value!);
        Response.Headers.ETag = $"\"{response.RowVersion}\"";
        return Ok(response);
    }

    [HttpPost("{costId:guid}/approve")]
    [ProducesResponseType<CostRecordResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Approve(
        [FromRoute] Guid itemId,
        [FromRoute] Guid costId,
        CancellationToken ct)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "cost-records.approve", ct);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);

        var access = accessResult.Value!;
        var ifMatch = ParseIfMatchHeader();
        var result = await _costStore.ApproveAsync(access.OrganizationId, itemId, costId, access, ifMatch, ct);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var response = ItemResponseMapper.ToResponse(result.Value!);
        Response.Headers.ETag = $"\"{response.RowVersion}\"";
        return Ok(response);
    }

    [HttpPost("{costId:guid}/return")]
    [ProducesResponseType<CostRecordResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Return(
        [FromRoute] Guid itemId,
        [FromRoute] Guid costId,
        [FromBody] ReturnCostRecordRequest request,
        CancellationToken ct)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "cost-records.approve", ct);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);

        var access = accessResult.Value!;
        var ifMatch = ParseIfMatchHeader();
        var result = await _costStore.ReturnAsync(access.OrganizationId, itemId, costId, request.Reason, access, ifMatch, ct);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var response = ItemResponseMapper.ToResponse(result.Value!);
        Response.Headers.ETag = $"\"{response.RowVersion}\"";
        return Ok(response);
    }

    [HttpPost("{costId:guid}/publish")]
    [ProducesResponseType<CostRecordResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Publish(
        [FromRoute] Guid itemId,
        [FromRoute] Guid costId,
        CancellationToken ct)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "cost-records.publish", ct);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);

        var access = accessResult.Value!;
        var ifMatch = ParseIfMatchHeader();
        var result = await _costStore.PublishAsync(access.OrganizationId, itemId, costId, access, ifMatch, ct);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var response = ItemResponseMapper.ToResponse(result.Value!);
        Response.Headers.ETag = $"\"{response.RowVersion}\"";
        return Ok(response);
    }

    [HttpPost("{costId:guid}/disable")]
    [ProducesResponseType<CostRecordResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Disable(
        [FromRoute] Guid itemId,
        [FromRoute] Guid costId,
        [FromBody] DisableCostRecordRequest request,
        CancellationToken ct)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "cost-records.disable", ct);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);

        var access = accessResult.Value!;
        var ifMatch = ParseIfMatchHeader();
        var result = await _costStore.DisableAsync(access.OrganizationId, itemId, costId, request.Reason, access, ifMatch, ct);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var response = ItemResponseMapper.ToResponse(result.Value!);
        Response.Headers.ETag = $"\"{response.RowVersion}\"";
        return Ok(response);
    }

    private Guid? ParseIfMatchHeader()
    {
        var header = Request.Headers.IfMatch.ToString();
        if (string.IsNullOrWhiteSpace(header)) return null;

        var clean = header.Trim('\"', ' ', '\'');
        return Guid.TryParse(clean, out var guid) ? guid : null;
    }
}
