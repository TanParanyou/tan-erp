using Microsoft.EntityFrameworkCore;
using TanErp.Application.Audit;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Domain.Common;

namespace TanErp.Infrastructure.Persistence.Audit;

public sealed class AuditEventStore : IAuditEventStore
{
    private readonly AppDbContext _db;

    public AuditEventStore(AppDbContext db) => _db = db;

    public async Task<AuditEventPage> ListAsync(Guid organizationId, AuditEventListQuery q, CancellationToken ct)
    {
        var query = _db.AuditEvents.AsNoTracking().Where(e => e.OrganizationId == organizationId);
        if (q.ResourceType is not null) query = query.Where(e => e.ResourceType == q.ResourceType);
        if (q.ResourceId is not null) query = query.Where(e => e.ResourceId == q.ResourceId);
        if (q.Action is not null) query = query.Where(e => e.Action == q.Action);
        if (q.ActorUserId is not null) query = query.Where(e => e.ActorUserId == q.ActorUserId);
        if (q.From is not null) query = query.Where(e => e.OccurredAtUtc >= q.From);
        if (q.To is not null) query = query.Where(e => e.OccurredAtUtc <= q.To);

        var total = await query.CountAsync(ct);
        var rows = await query
            .OrderByDescending(e => e.OccurredAtUtc).ThenByDescending(e => e.Id)
            .Skip((q.Page - 1) * q.Limit).Take(q.Limit)
            .Join(_db.Users.AsNoTracking(), e => e.ActorUserId, u => u.Id, (e, u) => new
            {
                e.Id, e.OccurredAtUtc, e.Action, e.ResourceType, e.ResourceId, ActorId = u.Id, ActorName = u.DisplayName,
                e.BranchId, e.TraceId, e.RequestId, e.Reason, e.ChangesJson
            })
            .OrderByDescending(r => r.OccurredAtUtc).ThenByDescending(r => r.Id)
            .ToListAsync(ct);

        // Rows written before the redactor existed may still hold person data, so redact on the way out as well.
        var items = rows.Select(r => new AuditEventView(
            r.Id, r.OccurredAtUtc, r.Action, r.ResourceType, r.ResourceId, new AdminRef(r.ActorId, r.ActorName),
            r.BranchId, r.TraceId, r.RequestId, r.Reason, AuditRedactor.Redact(r.ChangesJson))).ToList();
        return new AuditEventPage(items, total, q.Page, q.Limit);
    }

    public async Task RecordExportAsync(
        Guid organizationId, Guid actorUserId, Guid? branchId, Guid? membershipId, RecordExportAuditCommand command, CancellationToken ct)
    {
        var changes = System.Text.Json.JsonSerializer.Serialize(new
        {
            rowCount = command.RowCount,
            format = command.Format,
            filename = command.Filename,
            filterSummary = command.FilterSummary
        });

        var auditEvent = new AuditEvent(
            Guid.NewGuid(),
            organizationId,
            actorUserId,
            "data.exported",
            string.IsNullOrWhiteSpace(command.ResourceType) ? "DataExport" : command.ResourceType.Trim(),
            command.Filename ?? command.ResourceType ?? "export",
            DateTimeOffset.UtcNow,
            command.TraceId ?? string.Empty,
            changes,
            branchId: branchId,
            actorMembershipId: membershipId,
            reason: $"Exported {command.RowCount} rows ({command.Format})");

        _db.AuditEvents.Add(auditEvent);
        await _db.SaveChangesAsync(ct);
    }
}
