namespace TanErp.Application.Audit;

/// <summary>Read-only audit access, always scoped to <c>organizationId</c>.</summary>
public interface IAuditEventStore
{
    Task<AuditEventPage> ListAsync(Guid organizationId, AuditEventListQuery query, CancellationToken ct);
}
