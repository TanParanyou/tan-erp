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

    [HttpPost("{id:guid}/wait-customer")]
    [ProducesResponseType<ServiceRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> WaitCustomer([FromRoute] Guid id, [FromBody] ReasonRequest request, CancellationToken ct) =>
        CommandAsync(id, new WaitCustomerServiceRequest(request.Reason ?? string.Empty), ct);

    [HttpPost("{id:guid}/resume")]
    [ProducesResponseType<ServiceRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Resume([FromRoute] Guid id, CancellationToken ct) =>
        CommandAsync(id, new ResumeServiceRequest(), ct);

    [HttpPost("{id:guid}/charge")]
    [ProducesResponseType<ServiceRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> Charge([FromRoute] Guid id, [FromBody] ChargeServiceRequestRequest request, CancellationToken ct) =>
        CommandAsync(id, new ChargeServiceRequest(request.Amount, request.Note ?? string.Empty), ct);

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

    // ----- technician schedules --------------------------------------------------------------------

    [HttpGet("technician-schedules")]
    [ProducesResponseType<TechnicianSchedulesListResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListTechnicianSchedules(
        [FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate, [FromQuery] Guid? technicianUserId, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;

        var normalizedFrom = fromDate.HasValue && fromDate.Value.Year > 2400
            ? fromDate.Value.AddYears(-543)
            : fromDate;
        var normalizedTo = toDate.HasValue && toDate.Value.Year > 2400
            ? toDate.Value.AddYears(-543)
            : toDate;

        var result = await _handler.ListTechnicianSchedulesAsync(caller, normalizedFrom, normalizedTo, technicianUserId, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        return Ok(new TechnicianSchedulesListResponse(result.Value!.Select(ServiceMapper.To).ToList()));
    }

    [HttpPost("technician-schedules")]
    [ProducesResponseType<TechnicianAssignmentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateTechnicianSchedule([FromBody] CreateTechnicianAssignmentRequest request, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var input = new CreateTechnicianAssignmentInput(
            request.TechnicianUserId,
            request.WorkDate,
            request.AssignmentKind ?? string.Empty,
            request.InstallationJobId,
            request.ServiceRequestId,
            request.Note,
            request.AllowConflict);
        var result = await _handler.CreateTechnicianAssignmentAsync(caller, input, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        return StatusCode(StatusCodes.Status201Created, ServiceMapper.To(result.Value!));
    }

    [HttpDelete("technician-schedules/{assignmentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTechnicianSchedule([FromRoute] Guid assignmentId, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.DeleteTechnicianAssignmentAsync(caller, assignmentId, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        return NoContent();
    }

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
