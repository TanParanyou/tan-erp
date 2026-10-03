using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Service;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Common.Results;
using TanErp.Application.Service;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/installations")]
[Authorize]
public class InstallationsController : ServiceControllerBase
{
    private readonly ServiceHandler _handler;

    public InstallationsController(ServiceHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    [ProducesResponseType<InstallationResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] InstallationRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var input = new InstallationInput(
            request.ProjectId, request.ScheduledStart, request.ScheduledEnd, request.CrewName, request.Note,
            request.Checklist?.Select(c => new ChecklistInput(c.Title ?? string.Empty, c.Required)).ToList() ?? new List<ChecklistInput>());
        var result = await _handler.CreateInstallationAsync(caller, key, input, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return CreatedAtAction(nameof(Get), new { id = result.Value.Id }, ServiceMapper.To(result.Value));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<InstallationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.GetInstallationAsync(caller, id, ct));
    }

    [HttpGet]
    [ProducesResponseType<InstallationsListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> List(
        [FromQuery] string? search, [FromQuery] string? status, [FromQuery] Guid? projectId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = ServiceHandler.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListInstallationsAsync(caller, search, status, projectId, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);
        var paged = result.Value!;
        return Ok(new InstallationsListResponse(paged.Items.Select(ServiceMapper.To).ToList(), ServiceMapper.Pagination(paged.Page, paged.PageSize, paged.TotalCount)));
    }

    [HttpPost("{id:guid}/start")]
    [ProducesResponseType<InstallationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Start([FromRoute] Guid id, CancellationToken ct) => CommandAsync(id, new StartInstallation(), ct);

    [HttpPut("{id:guid}/checklist/{itemId:guid}")]
    [ProducesResponseType<InstallationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> SetChecklist([FromRoute] Guid id, [FromRoute] Guid itemId, [FromBody] ChecklistDoneRequest request, CancellationToken ct) =>
        CommandAsync(id, new SetChecklistItem(itemId, request.Done), ct);

    [HttpPost("{id:guid}/defects")]
    [ProducesResponseType<InstallationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> ReportDefect([FromRoute] Guid id, [FromBody] DefectRequest request, CancellationToken ct) =>
        CommandAsync(id, new ReportDefect(request.Description ?? string.Empty, request.Severity ?? string.Empty), ct);

    [HttpPost("{id:guid}/defects/{defectId:guid}/resolve")]
    [ProducesResponseType<InstallationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> ResolveDefect([FromRoute] Guid id, [FromRoute] Guid defectId, [FromBody] NoteRequest request, CancellationToken ct) =>
        CommandAsync(id, new ResolveDefect(defectId, request.Note ?? string.Empty), ct);

    [HttpPost("{id:guid}/defects/{defectId:guid}/verify")]
    [ProducesResponseType<InstallationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> VerifyDefect([FromRoute] Guid id, [FromRoute] Guid defectId, CancellationToken ct) =>
        CommandAsync(id, new VerifyDefect(defectId), ct);

    [HttpPost("{id:guid}/defects/{defectId:guid}/reopen")]
    [ProducesResponseType<InstallationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> ReopenDefect([FromRoute] Guid id, [FromRoute] Guid defectId, [FromBody] ReasonRequest request, CancellationToken ct) =>
        CommandAsync(id, new ReopenDefect(defectId, request.Reason ?? string.Empty), ct);

    [HttpPost("{id:guid}/ready")]
    [ProducesResponseType<InstallationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> Ready([FromRoute] Guid id, CancellationToken ct) => CommandAsync(id, new MarkInstallationReady(), ct);

    [HttpPost("{id:guid}/handover")]
    [ProducesResponseType<InstallationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> Handover([FromRoute] Guid id, [FromBody] HandoverRequest request, CancellationToken ct) =>
        CommandAsync(id, new RecordHandover(request.Outcome ?? string.Empty, request.SignerName ?? string.Empty, request.Note, request.WarrantyMonths, request.HandoverDate), ct);

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType<InstallationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> Cancel([FromRoute] Guid id, [FromBody] ReasonRequest request, CancellationToken ct) =>
        CommandAsync(id, new CancelInstallation(request.Reason ?? string.Empty), ct);

    private async Task<IActionResult> CommandAsync(Guid id, InstallationCommand command, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.InstallationCommandAsync(caller, id, version, command, ct));
    }

    private IActionResult Respond(Result<InstallationProjection> result)
    {
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return Ok(ServiceMapper.To(result.Value));
    }
}
