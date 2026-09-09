using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Crm.Customers;

public interface ICustomerLifecycleStore
{
    Task<Result<CustomerProjection>> ActivateAsync(
        RequestAccessContext access,
        Guid customerId,
        Guid expectedRowVersion,
        string keyHash,
        string payloadHash,
        string traceId,
        CancellationToken cancellationToken = default);
}
