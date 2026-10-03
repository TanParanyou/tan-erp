using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Service;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Service;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/warranties")]
[Authorize]
public class WarrantiesController : ServiceControllerBase
{
    private readonly ServiceHandler _handler;

    public WarrantiesController(ServiceHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<WarrantyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.GetWarrantyAsync(caller, id, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(ServiceMapper.To(result.Value!));
    }

    [HttpGet]
    [ProducesResponseType<WarrantiesListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> List(
        [FromQuery] string? search, [FromQuery] string? state, [FromQuery] Guid? projectId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = ServiceHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListWarrantiesAsync(caller, search, state, projectId, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new WarrantiesListResponse(paged.Items.Select(ServiceMapper.To).ToList(), ServiceMapper.Pagination(paged.Page, paged.PageSize, paged.TotalCount)));
    }
}
