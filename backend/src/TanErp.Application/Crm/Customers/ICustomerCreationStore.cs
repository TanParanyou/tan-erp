using TanErp.Application.Common.Results;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;

namespace TanErp.Application.Crm.Customers;

public sealed record PersistCustomerCreation(
    Customer Customer,
    string Operation,
    string IdempotencyKeyHash,
    string PayloadHash,
    IReadOnlyList<AuditEvent> AuditEvents,
    bool IncludeContactPii);

public sealed record PersistCustomerCreationResult(
    CustomerProjection Customer,
    IReadOnlyList<DuplicateCustomerProjection> DuplicateCandidates,
    bool WasReplayed);

public interface ICustomerCreationStore
{
    Task<Result<PersistCustomerCreationResult>> CreateAsync(
        PersistCustomerCreation request,
        CancellationToken cancellationToken = default);
}
