using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Items;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Items.Catalog;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/estimate-catalog/items")]
[Authorize]
public class EstimateCatalogController : ControllerBase
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IEstimateCatalogReader _catalogReader;

    public EstimateCatalogController(
        IRequestAccessResolver accessResolver,
        IEstimateCatalogReader catalogReader)
    {
        _accessResolver = accessResolver;
        _catalogReader = catalogReader;
    }

    [HttpGet]
    [ProducesResponseType<EstimateCatalogResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Search(
        [FromQuery, Required] Guid branchId,
        [FromQuery] string? search,
        [FromQuery] string? itemType,
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? brandId,
        [FromQuery] bool? hasCost,
        [FromQuery] string? cursor,
        [FromQuery] int pageSize = 25,
        CancellationToken ct = default)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveBranchAccessAsync(auth.FirebaseUid, auth.MembershipId, "items.read", branchId, ct);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);

        var access = accessResult.Value!;

        var query = new EstimateCatalogQuery(
            access.OrganizationId,
            branchId,
            search,
            itemType,
            categoryId,
            brandId,
            hasCost,
            cursor,
            pageSize);

        var result = await _catalogReader.SearchAsync(query, ct);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var response = ItemResponseMapper.ToResponse(result.Value!);
        return Ok(response);
    }
}
