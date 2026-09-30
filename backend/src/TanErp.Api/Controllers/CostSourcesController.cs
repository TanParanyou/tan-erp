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
[Authorize]
[Route("api/v1/cost-sources")]
public sealed class CostSourcesController : ControllerBase
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly ICostSourceStore _store;

    public CostSourcesController(IRequestAccessResolver accessResolver, ICostSourceStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CostSourceResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "cost-sources.read", ct);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        var rows = await _store.ListAsync(accessResult.Value!.OrganizationId, ct);
        return Ok(rows.Select(CostSourceResponseMapper.ToResponse));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<CostSourceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var access = await Resolve("cost-sources.read", ct);
        if (access.Error is not null) return access.Error;
        var row = await _store.GetAsync(access.Context!.OrganizationId, id, ct);
        return row is null ? ProblemDetailsMapper.CreateProblemResult("RESOURCE_NOT_FOUND", HttpContext) : Ok(CostSourceResponseMapper.ToResponse(row));
    }

    [HttpPost]
    [ProducesResponseType<CostSourceResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(CostSourceRequest request, [FromHeader(Name = "Idempotency-Key")] string idempotencyKey, CancellationToken ct)
    {
        var authResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        if (request.SourceType != CostSource.ManualType) return ProblemDetailsMapper.CreateProblemResult("COST_SOURCE_TYPE_INVALID", HttpContext);
        var auth = authResult.Value!;
        if (!string.Equals(auth.IdempotencyKey, idempotencyKey, StringComparison.Ordinal))
            return ProblemDetailsMapper.CreateProblemResult("IDEMPOTENCY_KEY_INVALID", HttpContext);
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "cost-sources.manage", ct);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        var result = await _store.CreateAsync(new CreateCostSourceData(request.Code, new LocalizedText(request.Name.Thai, request.Name.English)), accessResult.Value!, auth.IdempotencyKey, HttpContext.TraceIdentifier, ct);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        var response = CostSourceResponseMapper.ToResponse(result.Value!);
        Response.Headers.ETag = $"\"{response.RowVersion}\"";
        return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<CostSourceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> Update(Guid id, UpdateCostSourceRequest request, CancellationToken ct)
    {
        var authResult = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        if (request.SourceType != CostSource.ManualType) return ProblemDetailsMapper.CreateProblemResult("COST_SOURCE_TYPE_INVALID", HttpContext);
        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "cost-sources.manage", ct);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        var result = await _store.UpdateAsync(new UpdateCostSourceData(id, auth.IfMatchRowVersion, request.Code, new LocalizedText(request.Name.Thai, request.Name.English)), accessResult.Value!, HttpContext.TraceIdentifier, ct);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        var response = CostSourceResponseMapper.ToResponse(result.Value!);
        Response.Headers.ETag = $"\"{response.RowVersion}\"";
        return Ok(response);
    }

    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType<CostSourceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        var authResult = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, "cost-sources.manage", ct);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        var result = await _store.DeactivateAsync(id, auth.IfMatchRowVersion, accessResult.Value!, HttpContext.TraceIdentifier, ct);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        var response = CostSourceResponseMapper.ToResponse(result.Value!);
        Response.Headers.ETag = $"\"{response.RowVersion}\"";
        return Ok(response);
    }

    private async Task<(TanErp.Application.Common.Models.RequestAccessContext? Context, IActionResult? Error)> Resolve(string permission, CancellationToken ct)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return (null, ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext));
        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(auth.FirebaseUid, auth.MembershipId, permission, ct);
        return accessResult.IsFailure
            ? (null, ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext))
            : (accessResult.Value, null);
    }
}
