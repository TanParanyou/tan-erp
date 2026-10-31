namespace TanErp.Application.Audit;

/// <summary>Audit access, always scoped to <c>organizationId</c>.</summary>
public interface IAuditEventStore
{
    Task<AuditEventPage> ListAsync(Guid organizationId, AuditEventListQuery query, CancellationToken ct);
    Task RecordExportAsync(Guid organizationId, Guid actorUserId, Guid? branchId, Guid? membershipId, RecordExportAuditCommand command, CancellationToken ct);
}
