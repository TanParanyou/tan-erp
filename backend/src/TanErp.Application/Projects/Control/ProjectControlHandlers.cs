using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Projects.Control;

public sealed record ProjectCaller(string FirebaseUid, Guid MembershipId, string TraceId);

/// <summary>
/// Project control use cases. Each call resolves the caller's permission from PostgreSQL, validates the request
/// shape and then delegates to the store, which owns the transaction.
/// </summary>
public class ProjectControlHandler
{
    public const string ReadPermission = "projects.read";
    public const string UpdatePermission = "projects.update";
    public const string TransitionPermission = "projects.transition";
    public const string ChangeOrderManagePermission = "projects.change-orders.manage";
    public const string ChangeOrderApprovePermission = "projects.change-orders.approve";
    public const int MaxBudgetLines = 100;

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IProjectControlStore _store;

    public ProjectControlHandler(IRequestAccessResolver accessResolver, IProjectControlStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    private async Task<Result<RequestAccessContext>> AccessAsync(ProjectCaller caller, string permission, CancellationToken ct) =>
        await _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, ct);

    private static Result<ProjectControlProjection> Fail(string code, string message) =>
        Result<ProjectControlProjection>.Failure(new Error(code, message));

    public async Task<Result<ProjectControlProjection>> GetAsync(ProjectCaller caller, Guid projectId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, ReadPermission, ct);
        if (access.IsFailure) return Result<ProjectControlProjection>.Failure(access.Error);

        var control = await _store.GetAsync(access.Value!.OrganizationId, projectId, ct);
        return control is null ? Fail("RESOURCE_NOT_FOUND", "Project not found.") : Result<ProjectControlProjection>.Success(control);
    }

    public async Task<Result<ProjectControlProjection>> SetPlanAsync(ProjectCaller caller, Guid projectId, Guid expectedVersion, DateOnly? start, DateOnly? end, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, UpdatePermission, ct);
        if (access.IsFailure) return Result<ProjectControlProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail("PROJECT_FIELD_REQUIRED", "Expected project version is required.");

        return await _store.SetPlanAsync(access.Value!, projectId, expectedVersion, start, end, caller.TraceId, ct);
    }

    public async Task<Result<ProjectControlProjection>> ReplaceBudgetAsync(ProjectCaller caller, Guid projectId, Guid expectedVersion, IReadOnlyList<BudgetLineInput>? lines, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, UpdatePermission, ct);
        if (access.IsFailure) return Result<ProjectControlProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail("PROJECT_FIELD_REQUIRED", "Expected project version is required.");
        if (lines is null || lines.Count == 0 || lines.Count > MaxBudgetLines)
        {
            return Fail("PROJECT_BUDGET_INVALID", $"A budget needs between 1 and {MaxBudgetLines} lines.");
        }

        return await _store.ReplaceBudgetAsync(access.Value!, projectId, expectedVersion, lines, caller.TraceId, ct);
    }

    public async Task<Result<ProjectControlProjection>> AddMilestoneAsync(ProjectCaller caller, Guid projectId, string? name, DateOnly? plannedDate, int weight, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, UpdatePermission, ct);
        if (access.IsFailure) return Result<ProjectControlProjection>.Failure(access.Error);

        return await _store.AddMilestoneAsync(access.Value!, projectId, name ?? string.Empty, plannedDate, weight, caller.TraceId, ct);
    }

    public async Task<Result<ProjectControlProjection>> UpdateMilestoneAsync(ProjectCaller caller, Guid projectId, Guid milestoneId, Guid expectedVersion, string? name, DateOnly? plannedDate, int weight, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, UpdatePermission, ct);
        if (access.IsFailure) return Result<ProjectControlProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail("PROJECT_FIELD_REQUIRED", "Expected milestone version is required.");

        return await _store.UpdateMilestoneAsync(access.Value!, projectId, milestoneId, expectedVersion, name ?? string.Empty, plannedDate, weight, caller.TraceId, ct);
    }

    public async Task<Result<ProjectControlProjection>> CompleteMilestoneAsync(ProjectCaller caller, Guid projectId, Guid milestoneId, Guid expectedVersion, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, UpdatePermission, ct);
        if (access.IsFailure) return Result<ProjectControlProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail("PROJECT_FIELD_REQUIRED", "Expected milestone version is required.");

        return await _store.CompleteMilestoneAsync(access.Value!, projectId, milestoneId, expectedVersion, caller.TraceId, ct);
    }

    public async Task<Result<ProjectControlProjection>> DeleteMilestoneAsync(ProjectCaller caller, Guid projectId, Guid milestoneId, Guid expectedVersion, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, UpdatePermission, ct);
        if (access.IsFailure) return Result<ProjectControlProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail("PROJECT_FIELD_REQUIRED", "Expected milestone version is required.");

        return await _store.DeleteMilestoneAsync(access.Value!, projectId, milestoneId, expectedVersion, caller.TraceId, ct);
    }

    public async Task<Result<ProjectControlProjection>> TransitionAsync(ProjectCaller caller, Guid projectId, Guid expectedVersion, string? targetStatus, string? reason, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, TransitionPermission, ct);
        if (access.IsFailure) return Result<ProjectControlProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty || string.IsNullOrWhiteSpace(targetStatus))
        {
            return Fail("PROJECT_FIELD_REQUIRED", "Expected project version and target status are required.");
        }

        var trimmedReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        if (trimmedReason is { Length: > 500 }) return Fail("PROJECT_FIELD_INVALID", "Reason cannot exceed 500 characters.");

        return await _store.TransitionAsync(access.Value!, projectId, expectedVersion, targetStatus.Trim(), trimmedReason, caller.TraceId, ct);
    }

    public async Task<Result<ProjectControlProjection>> CreateChangeOrderAsync(
        ProjectCaller caller, Guid projectId, string idempotencyKey, string? title, string? reason, decimal budgetDelta, decimal contractDelta, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, ChangeOrderManagePermission, ct);
        if (access.IsFailure) return Result<ProjectControlProjection>.Failure(access.Error);

        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute($"{projectId}|{title?.Trim()}|{reason?.Trim()}|{budgetDelta:0.00}|{contractDelta:0.00}");
        return await _store.CreateChangeOrderAsync(access.Value!, projectId, title ?? string.Empty, reason ?? string.Empty, budgetDelta, contractDelta, keyHash, payloadHash, caller.TraceId, ct);
    }

    public async Task<Result<ProjectControlProjection>> ChangeOrderActionAsync(
        ProjectCaller caller, Guid projectId, Guid changeOrderId, Guid expectedVersion, ChangeOrderAction action, string? note, CancellationToken ct = default)
    {
        var permission = action is ChangeOrderAction.Approve or ChangeOrderAction.Reject ? ChangeOrderApprovePermission : ChangeOrderManagePermission;
        var access = await AccessAsync(caller, permission, ct);
        if (access.IsFailure) return Result<ProjectControlProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail("PROJECT_FIELD_REQUIRED", "Expected change order version is required.");
        if (note is { Length: > 500 }) return Fail("PROJECT_FIELD_INVALID", "Note cannot exceed 500 characters.");

        return await _store.ChangeOrderActionAsync(access.Value!, projectId, changeOrderId, expectedVersion, action, note, caller.TraceId, ct);
    }
}
