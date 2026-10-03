using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Domain.Crm.Customers;

namespace TanErp.Application.Crm.Customers;

public interface ICustomerContactStore
{
    Task<IReadOnlyList<CustomerContactDetailProjection>?> ListAsync(Guid organizationId, Guid customerId, CancellationToken cancellationToken = default);
    Task<Result<CustomerContactDetailProjection>> CreateAsync(RequestAccessContext access, Guid customerId, PrimaryContactInput input, string keyHash, string payloadHash, string traceId, CancellationToken cancellationToken = default);
    Task<Result<CustomerContactDetailProjection>> UpdateAsync(RequestAccessContext access, Guid customerId, Guid contactId, Guid expectedRowVersion, PrimaryContactInput input, string traceId, CancellationToken cancellationToken = default);
    Task<Result<CustomerContactDetailProjection>> SetPrimaryAsync(RequestAccessContext access, Guid customerId, Guid contactId, Guid expectedRowVersion, string traceId, CancellationToken cancellationToken = default);
    Task<Result<CustomerContactDetailProjection>> DeactivateAsync(RequestAccessContext access, Guid customerId, Guid contactId, Guid expectedRowVersion, string traceId, CancellationToken cancellationToken = default);
}
