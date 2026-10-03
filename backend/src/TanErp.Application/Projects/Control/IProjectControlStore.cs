using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Projects.Control;

public interface IProjectControlStore
{
    Task<ProjectControlProjection?> GetAsync(Guid organizationId, Guid projectId, CancellationToken ct = default);

    Task<Result<ProjectControlProjection>> SetPlanAsync(RequestAccessContext access, Guid projectId, Guid expectedVersion, DateOnly? start, DateOnly? end, string traceId, CancellationToken ct = default);

    Task<Result<ProjectControlProjection>> ReplaceBudgetAsync(RequestAccessContext access, Guid projectId, Guid expectedVersion, IReadOnlyList<BudgetLineInput> lines, string traceId, CancellationToken ct = default);

    Task<Result<ProjectControlProjection>> AddMilestoneAsync(RequestAccessContext access, Guid projectId, string name, DateOnly? plannedDate, int weight, string traceId, CancellationToken ct = default);

    Task<Result<ProjectControlProjection>> UpdateMilestoneAsync(RequestAccessContext access, Guid projectId, Guid milestoneId, Guid expectedVersion, string name, DateOnly? plannedDate, int weight, string traceId, CancellationToken ct = default);

    Task<Result<ProjectControlProjection>> CompleteMilestoneAsync(RequestAccessContext access, Guid projectId, Guid milestoneId, Guid expectedVersion, string traceId, CancellationToken ct = default);

    Task<Result<ProjectControlProjection>> DeleteMilestoneAsync(RequestAccessContext access, Guid projectId, Guid milestoneId, Guid expectedVersion, string traceId, CancellationToken ct = default);

    Task<Result<ProjectControlProjection>> TransitionAsync(RequestAccessContext access, Guid projectId, Guid expectedVersion, string targetStatus, string? reason, string traceId, CancellationToken ct = default);

    Task<Result<ProjectControlProjection>> CreateChangeOrderAsync(RequestAccessContext access, Guid projectId, string title, string reason, decimal budgetDelta, decimal contractDelta, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);

    Task<Result<ProjectControlProjection>> ChangeOrderActionAsync(RequestAccessContext access, Guid projectId, Guid changeOrderId, Guid expectedVersion, ChangeOrderAction action, string? note, string traceId, CancellationToken ct = default);
}

public enum ChangeOrderAction
{
    Submit,
    Approve,
    Reject,
    Cancel
}
