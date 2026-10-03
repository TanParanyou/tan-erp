using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Projects;
using TanErp.Application.Projects.Control;
using TanErp.Domain.Common;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Projects;

namespace TanErp.Infrastructure.Persistence.Projects;

public class ProjectControlStore : IProjectControlStore
{
    private const string ChangeOrderCreateOperation = "projects.change-order.create";

    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IDocumentNumberGenerator _documentNumberGenerator;

    public ProjectControlStore(AppDbContext db, IClock clock, IDocumentNumberGenerator documentNumberGenerator)
    {
        _db = db;
        _clock = clock;
        _documentNumberGenerator = documentNumberGenerator;
    }

    private static Result<ProjectControlProjection> Fail(string code, string message) =>
        Result<ProjectControlProjection>.Failure(new Error(code, message));

    private static Result<ProjectControlProjection> Fail(ProjectDomainException ex) => Fail(ex.Code, ex.Message);

    // ----- reads -----------------------------------------------------------------------------------

    public async Task<ProjectControlProjection?> GetAsync(Guid organizationId, Guid projectId, CancellationToken ct = default)
    {
        var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == projectId && p.OrganizationId == organizationId, ct);
        if (project is null) return null;

        var lines = await _db.ProjectBudgetLines.AsNoTracking()
            .Where(l => l.OrganizationId == organizationId && l.ProjectId == projectId)
            .OrderBy(l => l.SortOrder).ThenBy(l => l.Id).ToListAsync(ct);
        var milestones = await _db.ProjectMilestones.AsNoTracking()
            .Where(m => m.OrganizationId == organizationId && m.ProjectId == projectId)
            .OrderBy(m => m.SortOrder).ThenBy(m => m.Id).ToListAsync(ct);
        var changeOrders = await _db.ProjectChangeOrders.AsNoTracking()
            .Where(c => c.OrganizationId == organizationId && c.ProjectId == projectId)
            .OrderByDescending(c => c.CreatedAtUtc).ThenByDescending(c => c.Id).ToListAsync(ct);
        var history = await _db.ProjectStatusHistories.AsNoTracking()
            .Where(h => h.OrganizationId == organizationId && h.ProjectId == projectId)
            .OrderByDescending(h => h.OccurredAtUtc).ThenByDescending(h => h.Id).ToListAsync(ct);

        var userIds = milestones.Where(m => m.CompletedByUserId.HasValue).Select(m => m.CompletedByUserId!.Value)
            .Concat(changeOrders.SelectMany(c => new[] { c.CreatedByUserId, c.DecidedByUserId ?? Guid.Empty }))
            .Concat(history.Select(h => h.ActorUserId))
            .Where(id => id != Guid.Empty).Distinct().ToList();
        var people = await _db.Users.AsNoTracking().Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => new ProjectPersonProjection(u.Id, u.DisplayName, u.Email), ct);

        ProjectPersonProjection? Person(Guid? id) => id.HasValue && people.TryGetValue(id.Value, out var person) ? person : null;
        ProjectPersonProjection RequiredPerson(Guid id) => Person(id) ?? new ProjectPersonProjection(id, string.Empty, null);

        var approved = changeOrders.Where(c => c.Status == ChangeOrderStatus.Approved).ToList();
        var approvedBudgetDelta = approved.Sum(c => c.BudgetDelta);
        var approvedContractDelta = approved.Sum(c => c.ContractDelta);

        var totalWeight = milestones.Sum(m => m.Weight);
        var completedWeight = milestones.Where(m => m.CompletedAtUtc.HasValue).Sum(m => m.Weight);
        var percent = totalWeight == 0 ? 0m : decimal.Round(completedWeight * 100m / totalWeight, 2);

        return new ProjectControlProjection(
            project.Id,
            project.Status,
            project.StatusReason,
            project.PlannedStartDate,
            project.PlannedEndDate,
            project.ActivatedAtUtc,
            project.CompletedAtUtc,
            project.RowVersion,
            new ProjectBudgetProjection(
                project.BaselineBudgetTotal,
                project.BaselineBudgetHash,
                project.IsBudgetFrozen,
                project.BudgetFrozenAtUtc,
                approvedBudgetDelta,
                project.BaselineBudgetTotal.HasValue ? project.BaselineBudgetTotal.Value + approvedBudgetDelta : null,
                lines.Select(l => new ProjectBudgetLineProjection(l.Id, l.Category, l.Description, l.Amount, l.SortOrder)).ToList()),
            new ProjectContractProjection(project.BaselineContractAmount, approvedContractDelta, project.BaselineContractAmount + approvedContractDelta),
            new ProjectProgressProjection(milestones.Count, milestones.Count(m => m.CompletedAtUtc.HasValue), totalWeight, completedWeight, percent),
            milestones.Select(m => new ProjectMilestoneProjection(m.Id, m.Name, m.PlannedDate, m.Weight, m.SortOrder, m.CompletedAtUtc, Person(m.CompletedByUserId), m.RowVersion)).ToList(),
            changeOrders.Select(c => new ProjectChangeOrderProjection(
                c.Id, c.Number, c.Title, c.Reason, c.BudgetDelta, c.ContractDelta, c.Status, RequiredPerson(c.CreatedByUserId), c.CreatedAtUtc,
                c.SubmittedAtUtc, Person(c.DecidedByUserId), c.DecidedAtUtc, c.DecisionNote, c.RowVersion)).ToList(),
            history.Select(h => new ProjectStatusHistoryProjection(h.Id, h.FromStatus, h.ToStatus, h.Reason, RequiredPerson(h.ActorUserId), h.OccurredAtUtc)).ToList());
    }

    // ----- project-level writes (optimistic concurrency on the project row version) ----------------

    public async Task<Result<ProjectControlProjection>> SetPlanAsync(
        RequestAccessContext access, Guid projectId, Guid expectedVersion, DateOnly? start, DateOnly? end, string traceId, CancellationToken ct = default)
    {
        return await InTransactionAsync(access, projectId, expectedVersion, async (project, now) =>
        {
            project.SetPlan(start, end, now);
            Audit(access, "project.plan-updated", project.Id, traceId, new { plannedStartDate = start, plannedEndDate = end }, project.RowVersion, now);
            await Task.CompletedTask;
            return null;
        }, ct);
    }

    public async Task<Result<ProjectControlProjection>> ReplaceBudgetAsync(
        RequestAccessContext access, Guid projectId, Guid expectedVersion, IReadOnlyList<BudgetLineInput> lines, string traceId, CancellationToken ct = default)
    {
        return await InTransactionAsync(access, projectId, expectedVersion, async (project, now) =>
        {
            var built = new List<ProjectBudgetLine>();
            for (var i = 0; i < lines.Count; i++)
            {
                built.Add(new ProjectBudgetLine(Guid.NewGuid(), access.OrganizationId, project.Id, lines[i].Category, lines[i].Description, lines[i].Amount, i + 1));
            }

            var total = built.Sum(l => l.Amount);
            var hash = Sha256Hex.Compute(JsonSerializer.Serialize(built.Select(l => new
            {
                l.Category,
                l.Description,
                amount = l.Amount.ToString("0.00", CultureInfo.InvariantCulture)
            })));

            project.SetBaselineBudget(total, hash, now);

            var existing = await _db.ProjectBudgetLines.Where(l => l.OrganizationId == access.OrganizationId && l.ProjectId == project.Id).ToListAsync(ct);
            _db.ProjectBudgetLines.RemoveRange(existing);
            _db.ProjectBudgetLines.AddRange(built);

            Audit(access, "project.budget-set", project.Id, traceId, new { lineCount = built.Count, total, hash }, project.RowVersion, now);
            return null;
        }, ct);
    }

    public async Task<Result<ProjectControlProjection>> TransitionAsync(
        RequestAccessContext access, Guid projectId, Guid expectedVersion, string targetStatus, string? reason, string traceId, CancellationToken ct = default)
    {
        return await InTransactionAsync(access, projectId, expectedVersion, async (project, now) =>
        {
            var fromStatus = project.Status;
            var lineCount = await _db.ProjectBudgetLines.CountAsync(l => l.OrganizationId == access.OrganizationId && l.ProjectId == project.Id, ct);

            if (targetStatus is ProjectStatus.ReadyForHandover or ProjectStatus.Completed)
            {
                var hasOpenMilestone = await _db.ProjectMilestones.AnyAsync(
                    m => m.OrganizationId == access.OrganizationId && m.ProjectId == project.Id && m.CompletedAtUtc == null, ct);
                var hasPendingChangeOrder = await _db.ProjectChangeOrders.AnyAsync(
                    c => c.OrganizationId == access.OrganizationId && c.ProjectId == project.Id && c.Status == ChangeOrderStatus.Submitted, ct);
                if (targetStatus == ProjectStatus.ReadyForHandover && hasOpenMilestone)
                {
                    return Fail("PROJECT_NOT_READY", "Every milestone must be completed before the project is ready for handover.");
                }

                if (hasPendingChangeOrder)
                {
                    return Fail("PROJECT_NOT_READY", "Submitted change orders must be decided first.");
                }
            }

            project.TransitionTo(targetStatus, reason, lineCount, now);
            _db.ProjectStatusHistories.Add(new ProjectStatusHistory(Guid.NewGuid(), access.OrganizationId, project.Id, fromStatus, project.Status, reason, access.ActorUserId, now));
            Audit(access, "project.status-changed", project.Id, traceId, new { fromStatus, toStatus = project.Status }, project.RowVersion, now);
            return null;
        }, ct);
    }

    private async Task<Result<ProjectControlProjection>> InTransactionAsync(
        RequestAccessContext access,
        Guid projectId,
        Guid expectedVersion,
        Func<Project, DateTimeOffset, Task<Result<ProjectControlProjection>?>> mutate,
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId && p.OrganizationId == orgId, ct);
            if (project is null) return Fail("RESOURCE_NOT_FOUND", "Project not found.");
            if (project.RowVersion != expectedVersion) return Fail("PROJECT_VERSION_CONFLICT", "Project version conflict.");

            try
            {
                var failure = await mutate(project, _clock.UtcNow);
                if (failure is not null) return failure;
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (ProjectDomainException ex)
            {
                return Fail(ex);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail("PROJECT_VERSION_CONFLICT", "Project version conflict.");
            }

            return Result<ProjectControlProjection>.Success((await GetAsync(orgId, projectId, ct))!);
        });
    }

    // ----- milestones ------------------------------------------------------------------------------

    private static bool CanEditPlan(Project project) =>
        project.Status is ProjectStatus.Planned or ProjectStatus.Active or ProjectStatus.OnHold;

    public async Task<Result<ProjectControlProjection>> AddMilestoneAsync(
        RequestAccessContext access, Guid projectId, string name, DateOnly? plannedDate, int weight, string traceId, CancellationToken ct = default)
    {
        return await MilestoneTransactionAsync(access, projectId, async (project, now) =>
        {
            var nextOrder = (await _db.ProjectMilestones
                .Where(m => m.OrganizationId == access.OrganizationId && m.ProjectId == project.Id)
                .Select(m => (int?)m.SortOrder).MaxAsync(ct) ?? 0) + 1;
            var milestone = new ProjectMilestone(Guid.NewGuid(), access.OrganizationId, project.Id, name, plannedDate, weight, nextOrder);
            _db.ProjectMilestones.Add(milestone);
            Audit(access, "project.milestone-added", project.Id, traceId, new { milestoneId = milestone.Id, weight }, null, now);
            return null;
        }, ct);
    }

    public async Task<Result<ProjectControlProjection>> UpdateMilestoneAsync(
        RequestAccessContext access, Guid projectId, Guid milestoneId, Guid expectedVersion, string name, DateOnly? plannedDate, int weight, string traceId, CancellationToken ct = default)
    {
        return await MilestoneTransactionAsync(access, projectId, async (project, now) =>
        {
            var milestone = await FindMilestoneAsync(access.OrganizationId, project.Id, milestoneId, ct);
            if (milestone is null) return Fail("RESOURCE_NOT_FOUND", "Milestone not found.");
            if (milestone.RowVersion != expectedVersion) return Fail("PROJECT_MILESTONE_VERSION_CONFLICT", "Milestone version conflict.");
            milestone.Update(name, plannedDate, weight);
            Audit(access, "project.milestone-updated", project.Id, traceId, new { milestoneId, weight }, null, now);
            return null;
        }, ct);
    }

    public async Task<Result<ProjectControlProjection>> CompleteMilestoneAsync(
        RequestAccessContext access, Guid projectId, Guid milestoneId, Guid expectedVersion, string traceId, CancellationToken ct = default)
    {
        return await MilestoneTransactionAsync(access, projectId, async (project, now) =>
        {
            if (project.Status != ProjectStatus.Active)
            {
                return Fail("PROJECT_INVALID_STATE", "Milestones can only be completed while the project is active.");
            }

            var milestone = await FindMilestoneAsync(access.OrganizationId, project.Id, milestoneId, ct);
            if (milestone is null) return Fail("RESOURCE_NOT_FOUND", "Milestone not found.");
            if (milestone.RowVersion != expectedVersion) return Fail("PROJECT_MILESTONE_VERSION_CONFLICT", "Milestone version conflict.");
            milestone.Complete(access.ActorUserId, now);
            Audit(access, "project.milestone-completed", project.Id, traceId, new { milestoneId }, null, now);
            return null;
        }, ct);
    }

    public async Task<Result<ProjectControlProjection>> DeleteMilestoneAsync(
        RequestAccessContext access, Guid projectId, Guid milestoneId, Guid expectedVersion, string traceId, CancellationToken ct = default)
    {
        return await MilestoneTransactionAsync(access, projectId, async (project, now) =>
        {
            var milestone = await FindMilestoneAsync(access.OrganizationId, project.Id, milestoneId, ct);
            if (milestone is null) return Fail("RESOURCE_NOT_FOUND", "Milestone not found.");
            if (milestone.RowVersion != expectedVersion) return Fail("PROJECT_MILESTONE_VERSION_CONFLICT", "Milestone version conflict.");
            if (milestone.CompletedAtUtc.HasValue) return Fail("PROJECT_MILESTONE_COMPLETED", "A completed milestone cannot be deleted.");
            _db.ProjectMilestones.Remove(milestone);
            Audit(access, "project.milestone-removed", project.Id, traceId, new { milestoneId }, null, now);
            return null;
        }, ct);
    }

    private Task<ProjectMilestone?> FindMilestoneAsync(Guid orgId, Guid projectId, Guid milestoneId, CancellationToken ct) =>
        _db.ProjectMilestones.FirstOrDefaultAsync(m => m.Id == milestoneId && m.ProjectId == projectId && m.OrganizationId == orgId, ct);

    /// <summary>Milestones carry their own row version, so the project row version is left untouched.</summary>
    private async Task<Result<ProjectControlProjection>> MilestoneTransactionAsync(
        RequestAccessContext access,
        Guid projectId,
        Func<Project, DateTimeOffset, Task<Result<ProjectControlProjection>?>> mutate,
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == projectId && p.OrganizationId == orgId, ct);
            if (project is null) return Fail("RESOURCE_NOT_FOUND", "Project not found.");
            if (!CanEditPlan(project)) return Fail("PROJECT_INVALID_STATE", $"Milestones cannot be changed while the project is '{project.Status}'.");

            try
            {
                var failure = await mutate(project, _clock.UtcNow);
                if (failure is not null) return failure;
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (ProjectDomainException ex)
            {
                return Fail(ex);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail("PROJECT_MILESTONE_VERSION_CONFLICT", "Milestone version conflict.");
            }

            return Result<ProjectControlProjection>.Success((await GetAsync(orgId, projectId, ct))!);
        });
    }

    // ----- change orders ---------------------------------------------------------------------------

    public async Task<Result<ProjectControlProjection>> CreateChangeOrderAsync(
        RequestAccessContext access, Guid projectId, string title, string reason, decimal budgetDelta, decimal contractDelta,
        string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(
                r => r.OrganizationId == orgId && r.Operation == ChangeOrderCreateOperation && r.KeyHash == keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash)
                {
                    return Fail("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                }

                return Result<ProjectControlProjection>.Success((await GetAsync(orgId, projectId, ct))!);
            }

            var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == projectId && p.OrganizationId == orgId, ct);
            if (project is null) return Fail("RESOURCE_NOT_FOUND", "Project not found.");
            if (project.Status is not (ProjectStatus.Active or ProjectStatus.OnHold))
            {
                return Fail("PROJECT_INVALID_STATE", "Change orders can only be raised on an active or on-hold project.");
            }

            var now = _clock.UtcNow;
            string number;
            try
            {
                number = await _documentNumberGenerator.GenerateAsync(orgId, DocumentTypes.ProjectChangeOrders, project.BranchId, now, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Fail("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate change order document number.");
            }

            ProjectChangeOrder order;
            try
            {
                order = new ProjectChangeOrder(Guid.NewGuid(), orgId, projectId, number, title, reason, budgetDelta, contractDelta, access.ActorUserId, now);
            }
            catch (ProjectDomainException ex)
            {
                return Fail(ex);
            }

            _db.ProjectChangeOrders.Add(order);
            Audit(access, "project.change-order-created", projectId, traceId, new { number, budgetDelta, contractDelta }, null, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, ChangeOrderCreateOperation, keyHash, payloadHash, order.Id.ToString(), now));
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return Result<ProjectControlProjection>.Success((await GetAsync(orgId, projectId, ct))!);
        });
    }

    public async Task<Result<ProjectControlProjection>> ChangeOrderActionAsync(
        RequestAccessContext access, Guid projectId, Guid changeOrderId, Guid expectedVersion, ChangeOrderAction action, string? note, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == projectId && p.OrganizationId == orgId, ct);
            if (project is null) return Fail("RESOURCE_NOT_FOUND", "Project not found.");

            var order = await _db.ProjectChangeOrders.FirstOrDefaultAsync(c => c.Id == changeOrderId && c.ProjectId == projectId && c.OrganizationId == orgId, ct);
            if (order is null) return Fail("RESOURCE_NOT_FOUND", "Change order not found.");
            if (order.RowVersion != expectedVersion) return Fail("PROJECT_CHANGE_ORDER_VERSION_CONFLICT", "Change order version conflict.");

            var now = _clock.UtcNow;
            try
            {
                switch (action)
                {
                    case ChangeOrderAction.Submit:
                        order.Submit(now);
                        break;
                    case ChangeOrderAction.Cancel:
                        order.Cancel();
                        break;
                    default:
                        if (project.Status is not (ProjectStatus.Active or ProjectStatus.OnHold))
                        {
                            return Fail("PROJECT_INVALID_STATE", "Change orders can only be decided while the project is active or on hold.");
                        }

                        if (action == ChangeOrderAction.Approve)
                        {
                            // Over-budget control: approved deltas may never push the current budget below zero.
                            var approvedDelta = await _db.ProjectChangeOrders
                                .Where(c => c.OrganizationId == orgId && c.ProjectId == projectId && c.Status == ChangeOrderStatus.Approved)
                                .SumAsync(c => (decimal?)c.BudgetDelta, ct) ?? 0m;
                            if ((project.BaselineBudgetTotal ?? 0m) + approvedDelta + order.BudgetDelta < 0m)
                            {
                                return Fail("PROJECT_BUDGET_NEGATIVE", "Approving this change order would make the project budget negative.");
                            }
                        }

                        order.Decide(action == ChangeOrderAction.Approve, access.ActorUserId, note, now);
                        break;
                }
            }
            catch (ProjectDomainException ex)
            {
                return Fail(ex);
            }

            Audit(access, $"project.change-order-{action.ToString().ToLowerInvariant()}", projectId, traceId,
                new { number = order.Number, status = order.Status, budgetDelta = order.BudgetDelta, contractDelta = order.ContractDelta }, null, now);

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail("PROJECT_CHANGE_ORDER_VERSION_CONFLICT", "Change order version conflict.");
            }

            return Result<ProjectControlProjection>.Success((await GetAsync(orgId, projectId, ct))!);
        });
    }

    private void Audit(RequestAccessContext access, string action, Guid projectId, string traceId, object changes, Guid? rowVersionAfter, DateTimeOffset now)
    {
        _db.AuditEvents.Add(new AuditEvent(
            Guid.NewGuid(), access.OrganizationId, access.ActorUserId, action, "Project", projectId.ToString(), now, traceId,
            JsonSerializer.Serialize(changes),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            rowVersionAfter: rowVersionAfter));
    }
}
