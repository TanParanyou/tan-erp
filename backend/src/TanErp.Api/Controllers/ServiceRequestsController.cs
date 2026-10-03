using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Service;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Common.Results;
using TanErp.Application.Service;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/service-requests")]
[Authorize]
public class ServiceRequestsController : ServiceControllerBase
{
    private readonly ServiceHandler _handler;

    public ServiceRequestsController(ServiceHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    [ProducesResponseType<ServiceRequestResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] ServiceRequestRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var result = await _handler.CreateServiceRequestAsync(caller, key, new ServiceRequestInput(request.ProjectId, request.Title ?? string.Empty, request.Description ?? string.Empty, request.Priority ?? string.Empty), ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return CreatedAtAction(nameof(Get), new { id = result.Value.Id }, ServiceMapper.To(result.Value));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ServiceRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.GetServiceRequestAsync(caller, id, ct));
    }

    [HttpGet]
    [ProducesResponseType<ServiceRequestsListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> List(
        [FromQuery] string? search, [FromQuery] string? status, [FromQuery] Guid? projectId, [FromQuery] bool? inWarranty,
        [FromQuery] int page = 1, [FromQuery] int pageSize = ServiceHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListServiceRequestsAsync(caller, search, status, projectId, inWarranty, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new ServiceRequestsListResponse(paged.Items.Select(ServiceMapper.To).ToList(), ServiceMapper.Pagination(paged.Page, paged.PageSize, paged.TotalCount)));
    }

    [HttpPost("{id:guid}/schedule")]
    [ProducesResponseType<ServiceRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> Schedule([FromRoute] Guid id, [FromBody] ScheduleRequest request, CancellationToken ct) => CommandAsync(id, new ScheduleServiceRequest(request.Date), ct);

    [HttpPost("{id:guid}/start")]
    [ProducesResponseType<ServiceRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Start([FromRoute] Guid id, CancellationToken ct) => CommandAsync(id, new StartServiceRequest(), ct);

    [HttpPost("{id:guid}/resolve")]
    [ProducesResponseType<ServiceRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> Resolve([FromRoute] Guid id, [FromBody] NoteRequest request, CancellationToken ct) => CommandAsync(id, new ResolveServiceRequest(request.Note ?? string.Empty), ct);

    [HttpPost("{id:guid}/close")]
    [ProducesResponseType<ServiceRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Close([FromRoute] Guid id, CancellationToken ct) => CommandAsync(id, new CloseServiceRequest(), ct);

    [HttpPost("{id:guid}/reopen")]
    [ProducesResponseType<ServiceRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> Reopen([FromRoute] Guid id, [FromBody] ReasonRequest request, CancellationToken ct) => CommandAsync(id, new ReopenServiceRequest(request.Reason ?? string.Empty), ct);

    private async Task<IActionResult> CommandAsync(Guid id, ServiceRequestCommand command, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.ServiceRequestCommandAsync(caller, id, version, command, ct));
    }

    private IActionResult Respond(Result<ServiceRequestProjection> result)
    {
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ServiceMapper.To(result.Value));
    }
}
