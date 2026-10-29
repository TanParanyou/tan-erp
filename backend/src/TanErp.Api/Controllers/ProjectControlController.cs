using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Projects;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Common.Results;
using TanErp.Application.Projects;
using TanErp.Application.Projects.Control;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/projects/{projectId:guid}")]
[Authorize]
public class ProjectControlController : ControllerBase
{
    private readonly ProjectControlHandler _handler;

    public ProjectControlController(ProjectControlHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("control")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid projectId, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.GetAsync(caller, projectId, ct));
    }

    [HttpPut("plan")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> SetPlan([FromRoute] Guid projectId, [FromBody] SetProjectPlanRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.SetPlanAsync(caller, projectId, version, request.PlannedStartDate, request.PlannedEndDate, ct));
    }

    [HttpPut("budget")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ReplaceBudget([FromRoute] Guid projectId, [FromBody] ReplaceProjectBudgetRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        var lines = request.Lines?.Select(l => new BudgetLineInput(l.Category, l.Description, l.Amount, l.WbsNodeId)).ToList();
        return Respond(await _handler.ReplaceBudgetAsync(caller, projectId, version, lines, ct));
    }

    [HttpPost("wbs")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AddWbsNode([FromRoute] Guid projectId, [FromBody] AddProjectWbsNodeRequest request, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.AddWbsNodeAsync(caller, projectId, request.ParentId, request.Code, request.Name, ct), StatusCodes.Status201Created);
    }

    [HttpPut("wbs/{wbsNodeId:guid}")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateWbsNode([FromRoute] Guid projectId, [FromRoute] Guid wbsNodeId, [FromBody] UpdateProjectWbsNodeRequest request, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.UpdateWbsNodeAsync(caller, projectId, wbsNodeId, request.ExpectedVersion, request.Code, request.Name, ct));
    }

    [HttpPost("wbs/{wbsNodeId:guid}/delete")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteWbsNode([FromRoute] Guid projectId, [FromRoute] Guid wbsNodeId, [FromBody] ProjectWbsNodeVersionRequest request, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.DeleteWbsNodeAsync(caller, projectId, wbsNodeId, request.ExpectedVersion, ct));
    }

    [HttpPut("labor-rates")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> SetLaborRate([FromRoute] Guid projectId, [FromBody] SetProjectLaborRateRequest request, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.SetLaborRateAsync(caller, projectId, request.WorkerUserId, request.HourlyRate, ct));
    }

    [HttpPost("timesheets")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateTimesheet([FromRoute] Guid projectId, [FromBody] CreateProjectTimesheetRequest request, CancellationToken ct)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        var auth = contextResult.Value!;
        var caller = new ProjectCaller(auth.FirebaseUid, auth.MembershipId, HttpContext.TraceIdentifier);
        return Respond(
            await _handler.CreateTimesheetAsync(
                caller,
                projectId,
                auth.IdempotencyKey,
                new TimesheetInput(request.WorkerUserId, request.WorkDate, request.Hours, request.WbsNodeId, request.WorkOrderId, request.TaskDescription),
                ct),
            StatusCodes.Status201Created);
    }

    [HttpPost("timesheets/{timesheetId:guid}/submit")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> SubmitTimesheet([FromRoute] Guid projectId, [FromRoute] Guid timesheetId, [FromBody] ProjectTimesheetActionRequest request, CancellationToken ct) =>
        TimesheetAsync(projectId, timesheetId, request, TimesheetAction.Submit, ct);

    [HttpPost("timesheets/{timesheetId:guid}/approve")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> ApproveTimesheet([FromRoute] Guid projectId, [FromRoute] Guid timesheetId, [FromBody] ProjectTimesheetActionRequest request, CancellationToken ct) =>
        TimesheetAsync(projectId, timesheetId, request, TimesheetAction.Approve, ct);

    [HttpPost("timesheets/{timesheetId:guid}/reject")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> RejectTimesheet([FromRoute] Guid projectId, [FromRoute] Guid timesheetId, [FromBody] ProjectTimesheetActionRequest request, CancellationToken ct) =>
        TimesheetAsync(projectId, timesheetId, request, TimesheetAction.Reject, ct);

    [HttpPost("timesheets/{timesheetId:guid}/cancel")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> CancelTimesheet([FromRoute] Guid projectId, [FromRoute] Guid timesheetId, [FromBody] ProjectTimesheetActionRequest request, CancellationToken ct) =>
        TimesheetAsync(projectId, timesheetId, request, TimesheetAction.Cancel, ct);

    private async Task<IActionResult> TimesheetAsync(Guid projectId, Guid timesheetId, ProjectTimesheetActionRequest request, TimesheetAction action, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.TimesheetActionAsync(caller, projectId, timesheetId, request.ExpectedVersion, action, request.Note, ct));
    }

    [HttpPost("milestones")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AddMilestone([FromRoute] Guid projectId, [FromBody] AddProjectMilestoneRequest request, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.AddMilestoneAsync(caller, projectId, request.Name, request.PlannedDate, request.Weight, ct), StatusCodes.Status201Created);
    }

    [HttpPut("milestones/{milestoneId:guid}")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateMilestone([FromRoute] Guid projectId, [FromRoute] Guid milestoneId, [FromBody] UpdateProjectMilestoneRequest request, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.UpdateMilestoneAsync(caller, projectId, milestoneId, request.ExpectedVersion, request.Name, request.PlannedDate, request.Weight, ct));
    }

    [HttpPost("milestones/{milestoneId:guid}/complete")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CompleteMilestone([FromRoute] Guid projectId, [FromRoute] Guid milestoneId, [FromBody] ProjectMilestoneVersionRequest request, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.CompleteMilestoneAsync(caller, projectId, milestoneId, request.ExpectedVersion, ct));
    }

    [HttpPost("milestones/{milestoneId:guid}/delete")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteMilestone([FromRoute] Guid projectId, [FromRoute] Guid milestoneId, [FromBody] ProjectMilestoneVersionRequest request, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.DeleteMilestoneAsync(caller, projectId, milestoneId, request.ExpectedVersion, ct));
    }

    [HttpPost("transitions")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Transition([FromRoute] Guid projectId, [FromBody] TransitionProjectRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.TransitionAsync(caller, projectId, version, request.TargetStatus, request.Reason, ct));
    }

    [HttpPost("change-orders")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateChangeOrder([FromRoute] Guid projectId, [FromBody] CreateProjectChangeOrderRequest request, CancellationToken ct)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        var auth = contextResult.Value!;
        var caller = new ProjectCaller(auth.FirebaseUid, auth.MembershipId, HttpContext.TraceIdentifier);
        return Respond(
            await _handler.CreateChangeOrderAsync(caller, projectId, auth.IdempotencyKey, request.Title, request.Reason, request.BudgetDelta, request.ContractDelta, ct),
            StatusCodes.Status201Created);
    }

    [HttpPost("change-orders/{changeOrderId:guid}/submit")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> SubmitChangeOrder([FromRoute] Guid projectId, [FromRoute] Guid changeOrderId, [FromBody] ProjectChangeOrderActionRequest request, CancellationToken ct) =>
        ChangeOrderAsync(projectId, changeOrderId, request, ChangeOrderAction.Submit, ct);

    [HttpPost("change-orders/{changeOrderId:guid}/approve")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> ApproveChangeOrder([FromRoute] Guid projectId, [FromRoute] Guid changeOrderId, [FromBody] ProjectChangeOrderActionRequest request, CancellationToken ct) =>
        ChangeOrderAsync(projectId, changeOrderId, request, ChangeOrderAction.Approve, ct);

    [HttpPost("change-orders/{changeOrderId:guid}/reject")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> RejectChangeOrder([FromRoute] Guid projectId, [FromRoute] Guid changeOrderId, [FromBody] ProjectChangeOrderActionRequest request, CancellationToken ct) =>
        ChangeOrderAsync(projectId, changeOrderId, request, ChangeOrderAction.Reject, ct);

    [HttpPost("change-orders/{changeOrderId:guid}/cancel")]
    [ProducesResponseType<ProjectControlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<IActionResult> CancelChangeOrder([FromRoute] Guid projectId, [FromRoute] Guid changeOrderId, [FromBody] ProjectChangeOrderActionRequest request, CancellationToken ct) =>
        ChangeOrderAsync(projectId, changeOrderId, request, ChangeOrderAction.Cancel, ct);

    private async Task<IActionResult> ChangeOrderAsync(Guid projectId, Guid changeOrderId, ProjectChangeOrderActionRequest request, ChangeOrderAction action, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.ChangeOrderActionAsync(caller, projectId, changeOrderId, request.ExpectedVersion, action, request.Note, ct));
    }

    private ProjectCaller? ReadAuthenticated(out IActionResult? failure)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            failure = ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
            return null;
        }

        failure = null;
        var auth = contextResult.Value!;
        return new ProjectCaller(auth.FirebaseUid, auth.MembershipId, HttpContext.TraceIdentifier);
    }

    private ProjectCaller? ReadConditional(out Guid expectedVersion, out IActionResult? failure)
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
        return new ProjectCaller(auth.FirebaseUid, auth.MembershipId, HttpContext.TraceIdentifier);
    }

    private IActionResult Respond(Result<ProjectControlProjection> result, int successStatus = StatusCodes.Status200OK)
    {
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var response = ToResponse(result.Value!);
        Response.Headers.ETag = $"\"{response.RowVersion}\"";
        return StatusCode(successStatus, response);
    }

    private static ProjectPersonResponse? ToPerson(ProjectPersonProjection? p) => p is null ? null : new ProjectPersonResponse(p.Id, p.DisplayName, p.Email);

    private static ProjectControlResponse ToResponse(ProjectControlProjection c) => new(
        c.ProjectId, c.Status, c.StatusReason, c.PlannedStartDate, c.PlannedEndDate, c.ActivatedAtUtc, c.CompletedAtUtc, c.RowVersion,
        new ProjectBudgetResponse(
            c.Budget.BaselineTotal, c.Budget.BaselineHash, c.Budget.IsFrozen, c.Budget.FrozenAtUtc, c.Budget.ApprovedBudgetDelta, c.Budget.CurrentTotal, c.Budget.CommittedAmount, c.Budget.ActualAmount, c.Budget.AvailableBudget, c.Budget.VarianceAmount,
            c.Budget.Lines.Select(l => new ProjectBudgetLineResponse(l.Id, l.Category, l.Description, l.Amount, l.SortOrder, l.WbsNodeId, l.WbsCode, l.WbsName)).ToList()),
        new ProjectContractResponse(c.Contract.BaselineAmount, c.Contract.ApprovedDelta, c.Contract.CurrentAmount),
        new ProjectProgressResponse(c.Progress.TotalMilestones, c.Progress.CompletedMilestones, c.Progress.TotalWeight, c.Progress.CompletedWeight, c.Progress.Percent),
        c.WbsNodes.Select(w => new ProjectWbsNodeResponse(w.Id, w.ParentId, w.Code, w.Name, w.Level, w.SortOrder, w.RowVersion)).ToList(),
        c.WbsCostSummary.Select(s => new ProjectWbsCostSummaryResponse(
            s.WbsNodeId, s.WbsCode, s.WbsName, s.Level, s.BaselineBudget, s.CommittedAmount, s.ActualAmount, s.RemainingBudget, s.IsOverrun)).ToList(),
        c.CostLedger.Select(e => new ProjectCostLedgerEntryResponse(
            e.Id, e.WbsNodeId, e.WbsCode, e.Category, e.EntryType, e.SourceType, e.SourceId, e.SourceNumber,
            e.Description, e.Amount, e.IsReversal, ToPerson(e.Actor)!, e.OccurredAtUtc)).ToList(),
        c.LaborRates.Select(r => new LaborRateResponse(r.Id, ToPerson(r.Worker)!, r.HourlyRate, r.UpdatedAtUtc, r.RowVersion)).ToList(),
        c.Timesheets.Select(t => new LaborTimesheetResponse(
            t.Id, t.Number, ToPerson(t.Worker)!, t.WorkDate, t.Hours, t.HourlyRate, t.CostAmount, t.WbsNodeId, t.WbsCode, t.WorkOrderId, t.WorkOrderNumber,
            t.TaskDescription, t.Status, t.IsOvertimeWarning,
            ToPerson(t.CreatedBy)!, t.CreatedAtUtc, t.SubmittedAtUtc, ToPerson(t.DecidedBy), t.DecidedAtUtc, t.DecisionNote, t.RowVersion)).ToList(),
        c.Milestones.Select(m => new ProjectMilestoneResponse(m.Id, m.Name, m.PlannedDate, m.Weight, m.SortOrder, m.CompletedAtUtc, ToPerson(m.CompletedBy), m.RowVersion)).ToList(),
        c.ChangeOrders.Select(o => new ProjectChangeOrderResponse(
            o.Id, o.Number, o.Title, o.Reason, o.BudgetDelta, o.ContractDelta, o.Status, ToPerson(o.CreatedBy)!, o.CreatedAtUtc,
            o.SubmittedAtUtc, ToPerson(o.DecidedBy), o.DecidedAtUtc, o.DecisionNote, o.RowVersion)).ToList(),
        c.History.Select(h => new ProjectStatusHistoryResponse(h.Id, h.FromStatus, h.ToStatus, h.Reason, ToPerson(h.Actor)!, h.OccurredAtUtc)).ToList());
}
