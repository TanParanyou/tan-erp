using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.IdentityAccess.ApprovalLimits;
using TanErp.Application.Notifications;
using TanErp.Application.Projects;
using TanErp.Application.Projects.Control;
using TanErp.Domain.Common;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Procurement;
using TanErp.Domain.Production;
using TanErp.Domain.Projects;
using TanErp.Infrastructure.Persistence.Notifications;

namespace TanErp.Infrastructure.Persistence.Projects;

public class ProjectControlStore : IProjectControlStore
{
    private const string ChangeOrderCreateOperation = "projects.change-order.create";
    private const string TimesheetCreateOperation = "projects.timesheet.create";

    private static readonly string[] CommittedPoStatuses =
    {
        PurchaseOrderStatus.Approved, PurchaseOrderStatus.PartiallyReceived, PurchaseOrderStatus.Received
    };

    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IDocumentNumberGenerator _documentNumberGenerator;
    private readonly INotificationPublisher _notifications;
    private readonly IApprovalAuthority _approvalAuthority;

    public ProjectControlStore(AppDbContext db, IClock clock, IDocumentNumberGenerator documentNumberGenerator, INotificationPublisher notifications, IApprovalAuthority approvalAuthority)
    {
        _db = db;
        _clock = clock;
        _documentNumberGenerator = documentNumberGenerator;
        _notifications = notifications;
        _approvalAuthority = approvalAuthority;
    }

    private static Result<ProjectControlProjection> Fail(string code, string message) =>
        Result<ProjectControlProjection>.Failure(new Error(code, message));

    private static Result<ProjectControlProjection> Fail(ProjectDomainException ex) => Fail(ex.Code, ex.Message);

    private static bool IsUniqueViolation(DbUpdateException ex) =>
        ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };

    // ----- reads -----------------------------------------------------------------------------------

    public async Task<ProjectControlProjection?> GetAsync(Guid organizationId, Guid projectId, CancellationToken ct = default)
    {
        var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == projectId && p.OrganizationId == organizationId, ct);
        if (project is null) return null;

        var wbsNodes = await _db.ProjectWbsNodes.AsNoTracking()
            .Where(w => w.OrganizationId == organizationId && w.ProjectId == projectId)
            .OrderBy(w => w.Level).ThenBy(w => w.SortOrder).ThenBy(w => w.Code).ToListAsync(ct);
        var lines = await _db.ProjectBudgetLines.AsNoTracking()
            .Where(l => l.OrganizationId == organizationId && l.ProjectId == projectId)
            .OrderBy(l => l.SortOrder).ThenBy(l => l.Id).ToListAsync(ct);
        var ledgerEntries = await _db.ProjectCostLedgerEntries.AsNoTracking()
            .Where(e => e.OrganizationId == organizationId && e.ProjectId == projectId)
            .OrderByDescending(e => e.OccurredAtUtc).ThenByDescending(e => e.Id).ToListAsync(ct);
        var laborRates = await _db.LaborRates.AsNoTracking()
            .Where(r => r.OrganizationId == organizationId)
            .OrderBy(r => r.WorkerUserId).ToListAsync(ct);
        var timesheets = await _db.LaborTimesheetEntries.AsNoTracking()
            .Where(t => t.OrganizationId == organizationId && t.ProjectId == projectId)
            .OrderByDescending(t => t.WorkDate).ThenByDescending(t => t.CreatedAtUtc).ThenByDescending(t => t.Id).ToListAsync(ct);
        var milestones = await _db.ProjectMilestones.AsNoTracking()
            .Where(m => m.OrganizationId == organizationId && m.ProjectId == projectId)
            .OrderBy(m => m.SortOrder).ThenBy(m => m.Id).ToListAsync(ct);
        var changeOrders = await _db.ProjectChangeOrders.AsNoTracking()
            .Where(c => c.OrganizationId == organizationId && c.ProjectId == projectId)
            .OrderByDescending(c => c.CreatedAtUtc).ThenByDescending(c => c.Id).ToListAsync(ct);
        var history = await _db.ProjectStatusHistories.AsNoTracking()
            .Where(h => h.OrganizationId == organizationId && h.ProjectId == projectId)
            .OrderByDescending(h => h.OccurredAtUtc).ThenByDescending(h => h.Id).ToListAsync(ct);

        var workOrderIds = timesheets.Where(t => t.WorkOrderId.HasValue).Select(t => t.WorkOrderId!.Value).Distinct().ToList();
        var workOrders = workOrderIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await _db.WorkOrders.AsNoTracking()
                .Where(w => w.OrganizationId == organizationId && workOrderIds.Contains(w.Id))
                .ToDictionaryAsync(w => w.Id, w => w.Number, ct);

        var userIds = milestones.Where(m => m.CompletedByUserId.HasValue).Select(m => m.CompletedByUserId!.Value)
            .Concat(changeOrders.SelectMany(c => new[] { c.CreatedByUserId, c.DecidedByUserId ?? Guid.Empty }))
            .Concat(history.Select(h => h.ActorUserId))
            .Concat(ledgerEntries.Select(e => e.ActorUserId))
            .Concat(laborRates.Select(r => r.WorkerUserId))
            .Concat(timesheets.SelectMany(t => new[] { t.WorkerUserId, t.CreatedByUserId, t.DecidedByUserId ?? Guid.Empty }))
            .Where(id => id != Guid.Empty).Distinct().ToList();
        var people = await _db.Users.AsNoTracking().Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => new ProjectPersonProjection(u.Id, u.DisplayName, u.Email), ct);

        ProjectPersonProjection? Person(Guid? id) => id.HasValue && people.TryGetValue(id.Value, out var person) ? person : null;
        ProjectPersonProjection RequiredPerson(Guid id) => Person(id) ?? new ProjectPersonProjection(id, string.Empty, null);

        var wbsById = wbsNodes.ToDictionary(n => n.Id);
        var approved = changeOrders.Where(c => c.Status == ChangeOrderStatus.Approved).ToList();
        var approvedBudgetDelta = approved.Sum(c => c.BudgetDelta);
        var approvedContractDelta = approved.Sum(c => c.ContractDelta);

        // Commitments from cost ledger + fallback for any approved POs not yet in the ledger.
        var recordedPoIds = ledgerEntries
            .Where(e => e.SourceType == ProjectCostSourceType.PurchaseOrder)
            .Select(e => e.SourceId)
            .Distinct()
            .ToList();
        var unrecordedPoCommitted = await _db.PurchaseOrders.AsNoTracking()
            .Where(o => o.OrganizationId == organizationId && o.ProjectId == projectId && CommittedPoStatuses.Contains(o.Status) && !recordedPoIds.Contains(o.Id))
            .SumAsync(o => (decimal?)o.TotalAmount, ct) ?? 0m;

        var ledgerCommitted = ledgerEntries.Where(e => e.EntryType == ProjectCostEntryType.Commitment).Sum(e => e.Amount);
        var committed = Math.Max(0m, ledgerCommitted + unrecordedPoCommitted);
        var actual = ledgerEntries.Where(e => e.EntryType == ProjectCostEntryType.Actual).Sum(e => e.Amount);

        decimal? currentBudget = project.BaselineBudgetTotal.HasValue ? project.BaselineBudgetTotal.Value + approvedBudgetDelta : null;
        decimal? variance = currentBudget.HasValue ? currentBudget.Value - (committed + actual) : null;

        var wbsCostSummary = BuildWbsCostSummary(wbsNodes, lines, ledgerEntries, unrecordedPoCommitted);

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
                currentBudget,
                committed,
                actual,
                variance,
                variance,
                lines.Select(l =>
                {
                    var wbs = l.WbsNodeId.HasValue && wbsById.TryGetValue(l.WbsNodeId.Value, out var node) ? node : null;
                    return new ProjectBudgetLineProjection(l.Id, l.Category, l.Description, l.Amount, l.SortOrder, l.WbsNodeId, wbs?.Code, wbs?.Name);
                }).ToList()),
            new ProjectContractProjection(project.BaselineContractAmount, approvedContractDelta, project.BaselineContractAmount + approvedContractDelta),
            new ProjectProgressProjection(milestones.Count, milestones.Count(m => m.CompletedAtUtc.HasValue), totalWeight, completedWeight, percent),
            milestones.Select(m => new ProjectMilestoneProjection(m.Id, m.Name, m.PlannedDate, m.Weight, m.SortOrder, m.CompletedAtUtc, Person(m.CompletedByUserId), m.RowVersion)).ToList(),
            changeOrders.Select(c => new ProjectChangeOrderProjection(
                c.Id, c.Number, c.Title, c.Reason, c.BudgetDelta, c.ContractDelta, c.Status, RequiredPerson(c.CreatedByUserId), c.CreatedAtUtc,
                c.SubmittedAtUtc, Person(c.DecidedByUserId), c.DecidedAtUtc, c.DecisionNote, c.RowVersion)).ToList(),
            history.Select(h => new ProjectStatusHistoryProjection(h.Id, h.FromStatus, h.ToStatus, h.Reason, RequiredPerson(h.ActorUserId), h.OccurredAtUtc)).ToList(),
            wbsNodes.Select(w => new ProjectWbsNodeProjection(w.Id, w.ParentId, w.Code, w.Name, w.Level, w.SortOrder, w.RowVersion)).ToList(),
            wbsCostSummary,
            ledgerEntries.Select(e =>
            {
                var wbs = e.WbsNodeId.HasValue && wbsById.TryGetValue(e.WbsNodeId.Value, out var node) ? node : null;
                return new ProjectCostLedgerEntryProjection(
                    e.Id, e.WbsNodeId, wbs?.Code, e.Category, e.EntryType, e.SourceType, e.SourceId, e.SourceNumber,
                    e.Description, e.Amount, e.IsReversal, RequiredPerson(e.ActorUserId), e.OccurredAtUtc);
            }).ToList(),
            laborRates.Select(r => new LaborRateProjection(r.Id, RequiredPerson(r.WorkerUserId), r.HourlyRate, r.UpdatedAtUtc, r.RowVersion)).ToList(),
            timesheets.Select(t =>
            {
                var wbs = t.WbsNodeId.HasValue && wbsById.TryGetValue(t.WbsNodeId.Value, out var node) ? node : null;
                var woNumber = t.WorkOrderId.HasValue && workOrders.TryGetValue(t.WorkOrderId.Value, out var num) ? num : null;
                return new LaborTimesheetProjection(
                    t.Id, t.Number, RequiredPerson(t.WorkerUserId), t.WorkDate, t.Hours, t.HourlyRate, t.CostAmount,
                    t.WbsNodeId, wbs?.Code, t.WorkOrderId, woNumber, t.TaskDescription, t.Status, t.IsOvertimeWarning,
                    RequiredPerson(t.CreatedByUserId), t.CreatedAtUtc, t.SubmittedAtUtc, Person(t.DecidedByUserId), t.DecidedAtUtc, t.DecisionNote, t.RowVersion);
            }).ToList());
    }

    private static IReadOnlyList<ProjectWbsCostSummaryProjection> BuildWbsCostSummary(
        IReadOnlyList<ProjectWbsNode> wbsNodes,
        IReadOnlyList<ProjectBudgetLine> lines,
        IReadOnlyList<ProjectCostLedgerEntry> ledgerEntries,
        decimal unrecordedPoCommitted)
    {
        var childrenLookup = wbsNodes
            .Where(n => n.ParentId.HasValue)
            .GroupBy(n => n.ParentId!.Value)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Id).ToList());

        HashSet<Guid> GetSubtreeIds(Guid rootId)
        {
            var set = new HashSet<Guid> { rootId };
            var queue = new Queue<Guid>();
            queue.Enqueue(rootId);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (!childrenLookup.TryGetValue(current, out var children)) continue;
                foreach (var childId in children)
                {
                    if (set.Add(childId)) queue.Enqueue(childId);
                }
            }
            return set;
        }

        var summaries = new List<ProjectWbsCostSummaryProjection>();
        foreach (var node in wbsNodes)
        {
            var subtree = GetSubtreeIds(node.Id);
            var baseline = lines.Where(l => l.WbsNodeId.HasValue && subtree.Contains(l.WbsNodeId.Value)).Sum(l => l.Amount);
            var committed = Math.Max(0m, ledgerEntries
                .Where(e => e.WbsNodeId.HasValue && subtree.Contains(e.WbsNodeId.Value) && e.EntryType == ProjectCostEntryType.Commitment)
                .Sum(e => e.Amount));
            var actual = ledgerEntries
                .Where(e => e.WbsNodeId.HasValue && subtree.Contains(e.WbsNodeId.Value) && e.EntryType == ProjectCostEntryType.Actual)
                .Sum(e => e.Amount);
            var remaining = baseline - (committed + actual);
            summaries.Add(new ProjectWbsCostSummaryProjection(
                node.Id,
                node.Code,
                node.Name,
                node.Level,
                baseline,
                committed,
                actual,
                remaining,
                committed + actual > baseline));
        }

        var unassignedBaseline = lines.Where(l => !l.WbsNodeId.HasValue).Sum(l => l.Amount);
        var unassignedCommitted = Math.Max(0m, ledgerEntries
            .Where(e => !e.WbsNodeId.HasValue && e.EntryType == ProjectCostEntryType.Commitment)
            .Sum(e => e.Amount) + unrecordedPoCommitted);
        var unassignedActual = ledgerEntries
            .Where(e => !e.WbsNodeId.HasValue && e.EntryType == ProjectCostEntryType.Actual)
            .Sum(e => e.Amount);

        if (unassignedBaseline > 0m || unassignedCommitted > 0m || unassignedActual > 0m)
        {
            var remaining = unassignedBaseline - (unassignedCommitted + unassignedActual);
            summaries.Add(new ProjectWbsCostSummaryProjection(
                null,
                "UNASSIGNED",
                "Unassigned / Project-Level",
                0,
                unassignedBaseline,
                unassignedCommitted,
                unassignedActual,
                remaining,
                unassignedCommitted + unassignedActual > unassignedBaseline));
        }

        return summaries;
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
            var requestedWbsIds = lines.Where(l => l.WbsNodeId.HasValue).Select(l => l.WbsNodeId!.Value).Distinct().ToList();
            if (requestedWbsIds.Count > 0)
            {
                var validCount = await _db.ProjectWbsNodes.CountAsync(
                    w => w.OrganizationId == access.OrganizationId && w.ProjectId == project.Id && requestedWbsIds.Contains(w.Id), ct);
                if (validCount != requestedWbsIds.Count)
                {
                    return Fail("PROJECT_WBS_INVALID", "One or more WBS nodes do not belong to this project.");
                }
            }

            var built = new List<ProjectBudgetLine>();
            for (var i = 0; i < lines.Count; i++)
            {
                built.Add(new ProjectBudgetLine(Guid.NewGuid(), access.OrganizationId, project.Id, lines[i].Category, lines[i].Description, lines[i].Amount, i + 1, lines[i].WbsNodeId));
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

            // Closing a project needs every installation handed over or cancelled (handover gate).
            if (targetStatus == ProjectStatus.Completed
                && await _db.InstallationJobs.AnyAsync(j => j.OrganizationId == access.OrganizationId && j.ProjectId == project.Id
                    && j.Status != TanErp.Domain.Service.InstallationStatus.HandedOver && j.Status != TanErp.Domain.Service.InstallationStatus.Cancelled, ct))
            {
                return Fail("PROJECT_OPEN_INSTALLATION", "Every installation must be handed over or cancelled before the project is completed.");
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

    // ----- WBS hierarchy ---------------------------------------------------------------------------

    public async Task<Result<ProjectControlProjection>> AddWbsNodeAsync(
        RequestAccessContext access, Guid projectId, Guid? parentId, string code, string name, string traceId, CancellationToken ct = default)
    {
        return await WbsTransactionAsync(access, projectId, async (project, now) =>
        {
            var level = 1;
            if (parentId.HasValue)
            {
                var parent = await _db.ProjectWbsNodes.AsNoTracking()
                    .FirstOrDefaultAsync(w => w.Id == parentId.Value && w.ProjectId == project.Id && w.OrganizationId == access.OrganizationId, ct);
                if (parent is null) return Fail("RESOURCE_NOT_FOUND", "Parent WBS node not found.");
                level = parent.Level + 1;
            }

            var trimmedCode = code?.Trim() ?? string.Empty;
            if (trimmedCode.Length > 0 && await _db.ProjectWbsNodes.AnyAsync(
                w => w.OrganizationId == access.OrganizationId && w.ProjectId == project.Id && w.Code.ToLower() == trimmedCode.ToLower(), ct))
            {
                return Fail("PROJECT_WBS_CODE_CONFLICT", "A WBS node with this code already exists in the project.");
            }

            var nextOrder = (await _db.ProjectWbsNodes
                .Where(w => w.OrganizationId == access.OrganizationId && w.ProjectId == project.Id)
                .Select(w => (int?)w.SortOrder).MaxAsync(ct) ?? 0) + 1;

            var node = new ProjectWbsNode(Guid.NewGuid(), access.OrganizationId, project.Id, parentId, code ?? string.Empty, name ?? string.Empty, level, nextOrder);
            _db.ProjectWbsNodes.Add(node);
            Audit(access, "project.wbs-added", project.Id, traceId, new { wbsNodeId = node.Id, code = node.Code, level = node.Level }, null, now);
            return null;
        }, ct);
    }

    public async Task<Result<ProjectControlProjection>> UpdateWbsNodeAsync(
        RequestAccessContext access, Guid projectId, Guid wbsNodeId, Guid expectedVersion, string code, string name, string traceId, CancellationToken ct = default)
    {
        return await WbsTransactionAsync(access, projectId, async (project, now) =>
        {
            var node = await _db.ProjectWbsNodes.FirstOrDefaultAsync(w => w.Id == wbsNodeId && w.ProjectId == project.Id && w.OrganizationId == access.OrganizationId, ct);
            if (node is null) return Fail("RESOURCE_NOT_FOUND", "WBS node not found.");
            if (node.RowVersion != expectedVersion) return Fail("PROJECT_WBS_VERSION_CONFLICT", "WBS node version conflict.");

            var trimmedCode = code?.Trim() ?? string.Empty;
            if (trimmedCode.Length > 0 && await _db.ProjectWbsNodes.AnyAsync(
                w => w.OrganizationId == access.OrganizationId && w.ProjectId == project.Id && w.Id != wbsNodeId && w.Code.ToLower() == trimmedCode.ToLower(), ct))
            {
                return Fail("PROJECT_WBS_CODE_CONFLICT", "A WBS node with this code already exists in the project.");
            }

            node.Update(code ?? string.Empty, name ?? string.Empty);
            Audit(access, "project.wbs-updated", project.Id, traceId, new { wbsNodeId, code = node.Code, name = node.Name }, null, now);
            return null;
        }, ct);
    }

    public async Task<Result<ProjectControlProjection>> DeleteWbsNodeAsync(
        RequestAccessContext access, Guid projectId, Guid wbsNodeId, Guid expectedVersion, string traceId, CancellationToken ct = default)
    {
        return await WbsTransactionAsync(access, projectId, async (project, now) =>
        {
            var node = await _db.ProjectWbsNodes.FirstOrDefaultAsync(w => w.Id == wbsNodeId && w.ProjectId == project.Id && w.OrganizationId == access.OrganizationId, ct);
            if (node is null) return Fail("RESOURCE_NOT_FOUND", "WBS node not found.");
            if (node.RowVersion != expectedVersion) return Fail("PROJECT_WBS_VERSION_CONFLICT", "WBS node version conflict.");

            var hasChildren = await _db.ProjectWbsNodes.AnyAsync(w => w.OrganizationId == access.OrganizationId && w.ParentId == wbsNodeId, ct);
            var hasBudgetLines = await _db.ProjectBudgetLines.AnyAsync(l => l.OrganizationId == access.OrganizationId && l.WbsNodeId == wbsNodeId, ct);
            var hasLedgerEntries = await _db.ProjectCostLedgerEntries.AnyAsync(e => e.OrganizationId == access.OrganizationId && e.WbsNodeId == wbsNodeId, ct);
            var hasTimesheets = await _db.LaborTimesheetEntries.AnyAsync(t => t.OrganizationId == access.OrganizationId && t.WbsNodeId == wbsNodeId, ct);

            if (hasChildren || hasBudgetLines || hasLedgerEntries || hasTimesheets)
            {
                return Fail("PROJECT_WBS_IN_USE", "Cannot delete a WBS node that has child nodes, budget lines, timesheets, or cost ledger entries.");
            }

            _db.ProjectWbsNodes.Remove(node);
            Audit(access, "project.wbs-removed", project.Id, traceId, new { wbsNodeId, code = node.Code }, null, now);
            return null;
        }, ct);
    }

    private async Task<Result<ProjectControlProjection>> WbsTransactionAsync(
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
            if (!CanEditPlan(project)) return Fail("PROJECT_INVALID_STATE", $"WBS nodes cannot be changed while the project is '{project.Status}'.");

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
                return Fail("PROJECT_WBS_VERSION_CONFLICT", "WBS node version conflict.");
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Fail("PROJECT_WBS_CODE_CONFLICT", "A WBS node with this code already exists in the project.");
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

                            // Approval Authority (G-03c): deltas can be negative; the larger magnitude is what the approver is trusted with.
                            // Projects carry no currency column, so project money is THB.
                            var authorityError = await _approvalAuthority.EnsureAsync(new ApprovalAuthorityCheck(
                                orgId, access.MembershipId, TanErp.Domain.IdentityAccess.ApprovalDocumentType.ChangeOrder,
                                project.BranchId, Math.Max(Math.Abs(order.ContractDelta), Math.Abs(order.BudgetDelta)),
                                TanErp.Domain.IdentityAccess.ApprovalLimitRules.Currency), ct);
                            if (authorityError is not null) return Result<ProjectControlProjection>.Failure(authorityError);
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

            if (action == ChangeOrderAction.Submit)
            {
                await _notifications.PublishAsync(
                    NotificationEvents.ChangeOrderSubmitted(
                        orgId, project.BranchId, order.RowVersion, access.ActorUserId, order.Id, projectId, order.Number, order.CreatedByUserId),
                    ct);
            }

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail("PROJECT_CHANGE_ORDER_VERSION_CONFLICT", "Change order version conflict.");
            }
            catch (DbUpdateException ex) when (NotificationDedupeConflict.Is(ex))
            {
                // A concurrent duplicate already committed this transition's notification: report the same outcome as a stale version.
                return Fail("PROJECT_CHANGE_ORDER_VERSION_CONFLICT", "Change order version conflict.");
            }

            return Result<ProjectControlProjection>.Success((await GetAsync(orgId, projectId, ct))!);
        });
    }

    // ----- labor rates & timesheets ----------------------------------------------------------------

    public async Task<Result<ProjectControlProjection>> SetLaborRateAsync(
        RequestAccessContext access, Guid projectId, Guid workerUserId, decimal hourlyRate, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == projectId && p.OrganizationId == orgId, ct);
            if (project is null) return Fail("RESOURCE_NOT_FOUND", "Project not found.");

            var workerExists = await _db.Memberships.AsNoTracking().AnyAsync(m => m.OrganizationId == orgId && m.UserId == workerUserId, ct);
            if (!workerExists) return Fail("RESOURCE_NOT_FOUND", "Worker not found in this organization.");

            var now = _clock.UtcNow;
            try
            {
                var existing = await _db.LaborRates.FirstOrDefaultAsync(r => r.OrganizationId == orgId && r.WorkerUserId == workerUserId, ct);
                if (existing is null)
                {
                    var created = new LaborRate(Guid.NewGuid(), orgId, workerUserId, hourlyRate, access.ActorUserId, now);
                    _db.LaborRates.Add(created);
                }
                else
                {
                    existing.Update(hourlyRate, access.ActorUserId, now);
                }
            }
            catch (ProjectDomainException ex)
            {
                return Fail(ex);
            }

            Audit(access, "project.labor-rate-set", projectId, traceId, new { workerUserId, hourlyRate }, null, now);
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return Result<ProjectControlProjection>.Success((await GetAsync(orgId, projectId, ct))!);
        });
    }

    public async Task<Result<ProjectControlProjection>> CreateTimesheetAsync(
        RequestAccessContext access, Guid projectId, TimesheetInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(
                r => r.OrganizationId == orgId && r.Operation == TimesheetCreateOperation && r.KeyHash == keyHash, ct);
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
            if (project.Status != ProjectStatus.Active)
            {
                return Fail("TIMESHEET_PROJECT_NOT_ACTIVE", "Timesheets can only be logged on an active project.");
            }

            var workerExists = await _db.Memberships.AsNoTracking().AnyAsync(m => m.OrganizationId == orgId && m.UserId == input.WorkerUserId, ct);
            if (!workerExists) return Fail("RESOURCE_NOT_FOUND", "Worker not found in this organization.");

            if (input.WbsNodeId.HasValue)
            {
                var wbsExists = await _db.ProjectWbsNodes.AsNoTracking().AnyAsync(
                    w => w.Id == input.WbsNodeId.Value && w.ProjectId == projectId && w.OrganizationId == orgId, ct);
                if (!wbsExists) return Fail("PROJECT_WBS_INVALID", "Referenced WBS node does not belong to this project.");
            }

            if (input.WorkOrderId.HasValue)
            {
                var workOrder = await _db.WorkOrders.AsNoTracking().FirstOrDefaultAsync(
                    w => w.Id == input.WorkOrderId.Value && w.OrganizationId == orgId, ct);
                if (workOrder is null) return Fail("RESOURCE_NOT_FOUND", "Work order not found.");
                if (workOrder.ProjectId.HasValue && workOrder.ProjectId.Value != projectId)
                {
                    return Fail("TIMESHEET_FIELD_INVALID", "Work order does not belong to this project.");
                }

                if (workOrder.Status is TanErp.Domain.Production.WorkOrderStatus.Completed or TanErp.Domain.Production.WorkOrderStatus.Cancelled)
                {
                    return Fail("PRODUCTION_INVALID_STATE", "Timesheets cannot be logged against a completed or cancelled work order.");
                }
            }

            var rateRow = await _db.LaborRates.AsNoTracking()
                .FirstOrDefaultAsync(r => r.OrganizationId == orgId && r.WorkerUserId == input.WorkerUserId, ct);
            var hourlyRate = rateRow?.HourlyRate ?? LaborRate.DefaultTestOnlyHourlyRate;

            var existingDayHours = await _db.LaborTimesheetEntries.AsNoTracking()
                .Where(t => t.OrganizationId == orgId && t.WorkerUserId == input.WorkerUserId && t.WorkDate == input.WorkDate
                    && t.Status != TimesheetStatus.Cancelled && t.Status != TimesheetStatus.Rejected)
                .SumAsync(t => (decimal?)t.Hours, ct) ?? 0m;

            var now = _clock.UtcNow;
            string number;
            try
            {
                number = await _documentNumberGenerator.GenerateAsync(orgId, DocumentTypes.LaborTimesheets, project.BranchId, now, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Fail("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate timesheet document number.");
            }

            LaborTimesheetEntry entry;
            try
            {
                entry = new LaborTimesheetEntry(
                    Guid.NewGuid(), orgId, project.BranchId, projectId, input.WbsNodeId, input.WorkOrderId,
                    number, input.WorkerUserId, input.WorkDate, input.Hours, hourlyRate, existingDayHours,
                    input.TaskDescription, access.ActorUserId, now);
            }
            catch (ProjectDomainException ex)
            {
                return Fail(ex);
            }

            _db.LaborTimesheetEntries.Add(entry);
            Audit(access, "project.timesheet-created", projectId, traceId,
                new { number, workerUserId = entry.WorkerUserId, workDate = entry.WorkDate, hours = entry.Hours, costAmount = entry.CostAmount }, null, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, TimesheetCreateOperation, keyHash, payloadHash, entry.Id.ToString(), now));
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return Result<ProjectControlProjection>.Success((await GetAsync(orgId, projectId, ct))!);
        });
    }

    public async Task<Result<ProjectControlProjection>> TimesheetActionAsync(
        RequestAccessContext access, Guid projectId, Guid timesheetId, Guid expectedVersion, TimesheetAction action, string? note, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == projectId && p.OrganizationId == orgId, ct);
            if (project is null) return Fail("RESOURCE_NOT_FOUND", "Project not found.");

            var entry = await _db.LaborTimesheetEntries.FirstOrDefaultAsync(
                t => t.Id == timesheetId && t.ProjectId == projectId && t.OrganizationId == orgId, ct);
            if (entry is null) return Fail("RESOURCE_NOT_FOUND", "Timesheet not found.");
            if (entry.Status == TimesheetStatus.Approved)
            {
                return Fail("TIMESHEET_IMMUTABLE", "An approved timesheet is immutable.");
            }

            if (entry.RowVersion != expectedVersion) return Fail("TIMESHEET_VERSION_CONFLICT", "Timesheet version conflict.");

            var now = _clock.UtcNow;
            ProjectCostLedgerEntry? costLedgerEntry = null;
            try
            {
                switch (action)
                {
                    case TimesheetAction.Submit:
                        entry.Submit(now);
                        break;
                    case TimesheetAction.Cancel:
                        entry.Cancel();
                        break;
                    default:
                        if (project.Status is not (ProjectStatus.Active or ProjectStatus.OnHold))
                        {
                            return Fail("TIMESHEET_PROJECT_NOT_ACTIVE", "Timesheets can only be decided while the project is active or on hold.");
                        }

                        entry.Decide(action == TimesheetAction.Approve, access.ActorUserId, note, now);
                        if (action == TimesheetAction.Approve)
                        {
                            if (entry.WorkOrderId.HasValue)
                            {
                                var workOrder = await _db.WorkOrders.FirstOrDefaultAsync(
                                    w => w.Id == entry.WorkOrderId.Value && w.OrganizationId == orgId, ct);
                                if (workOrder is null) return Fail("RESOURCE_NOT_FOUND", "Work order not found.");
                                if (workOrder.Status is TanErp.Domain.Production.WorkOrderStatus.Completed or TanErp.Domain.Production.WorkOrderStatus.Cancelled)
                                {
                                    return Fail("PRODUCTION_INVALID_STATE", "Cannot approve a timesheet linked to a completed or cancelled work order.");
                                }

                                workOrder.RecordLabor(entry.Hours, entry.CostAmount, now);
                            }

                            costLedgerEntry = new ProjectCostLedgerEntry(
                                Guid.NewGuid(),
                                orgId,
                                projectId,
                                entry.WbsNodeId,
                                ProjectBudgetCategory.Labor,
                                ProjectCostEntryType.Actual,
                                ProjectCostSourceType.LaborTimesheet,
                                entry.Id,
                                entry.Number,
                                $"Labor timesheet {entry.Number} ({entry.Hours:0.##}h): {entry.TaskDescription}",
                                entry.CostAmount,
                                false,
                                access.ActorUserId,
                                now);
                            _db.ProjectCostLedgerEntries.Add(costLedgerEntry);
                        }
                        break;
                }
            }
            catch (ProjectDomainException ex)
            {
                return Fail(ex);
            }
            catch (TanErp.Domain.Production.ProductionDomainException ex)
            {
                return Fail(ex.Code, ex.Message);
            }

            Audit(access, $"project.timesheet-{action.ToString().ToLowerInvariant()}", projectId, traceId,
                new { number = entry.Number, status = entry.Status, hours = entry.Hours, costAmount = entry.CostAmount }, null, now);

            if (costLedgerEntry is not null)
            {
                await CheckAndPublishBudgetOverrunAsync(orgId, project, costLedgerEntry, access.ActorUserId, ct);
            }

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail("TIMESHEET_VERSION_CONFLICT", "Timesheet version conflict.");
            }
            catch (DbUpdateException ex) when (NotificationDedupeConflict.Is(ex))
            {
                return Fail("TIMESHEET_VERSION_CONFLICT", "Timesheet version conflict.");
            }

            return Result<ProjectControlProjection>.Success((await GetAsync(orgId, projectId, ct))!);
        });
    }

    private async Task CheckAndPublishBudgetOverrunAsync(
        Guid orgId, Project project, ProjectCostLedgerEntry addedEntry, Guid actorUserId, CancellationToken ct)
    {
        if (!project.BaselineBudgetTotal.HasValue) return;

        var approvedDelta = await _db.ProjectChangeOrders.AsNoTracking()
            .Where(c => c.OrganizationId == orgId && c.ProjectId == project.Id && c.Status == ChangeOrderStatus.Approved)
            .SumAsync(c => (decimal?)c.BudgetDelta, ct) ?? 0m;
        var currentBudget = project.BaselineBudgetTotal.Value + approvedDelta;
        if (currentBudget <= 0m) return;

        var existingTotal = await _db.ProjectCostLedgerEntries.AsNoTracking()
            .Where(e => e.OrganizationId == orgId && e.ProjectId == project.Id)
            .SumAsync(e => (decimal?)e.Amount, ct) ?? 0m;

        if (existingTotal + addedEntry.Amount > currentBudget)
        {
            await _notifications.PublishAsync(
                NotificationEvents.ProjectBudgetOverrun(
                    orgId, project.BranchId, addedEntry.Id, actorUserId, project.Id, project.Code),
                ct);
        }
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
